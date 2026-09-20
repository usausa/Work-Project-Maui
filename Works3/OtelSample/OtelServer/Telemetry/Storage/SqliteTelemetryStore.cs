namespace OtelServer.Telemetry.Storage;

using System.Globalization;
using System.Text.Json;

using Microsoft.Data.Sqlite;

using OtelServer.Telemetry.Models;

// 受信したテレメトリを SQLite に保持する。書き込みは直列化し、件数の上限はテーブルごとに受信のたびに適用する
public sealed class SqliteTelemetryStore : ITelemetryStore
{
    private const string LastReceivedAtKey = "last_received_at";

    private const string MinuteFormat = "yyyy-MM-dd'T'HH:mm";

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly string connectionString;

    private readonly TelemetryStoreOptions options;

    private readonly Lock sync = new();

    public event EventHandler<EventArgs>? Changed;

    public SqliteTelemetryStore(TelemetryStoreOptions options, IHostEnvironment hostEnvironment)
    {
        this.options = options;

        var databasePath = Path.IsPathRooted(options.DatabasePath)
            ? options.DatabasePath
            : Path.GetFullPath(Path.Combine(hostEnvironment.ContentRootPath, options.DatabasePath));
        var databaseDirectory = Path.GetDirectoryName(databasePath);
        if (!String.IsNullOrEmpty(databaseDirectory))
        {
            Directory.CreateDirectory(databaseDirectory);
        }

        connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
            Cache = SqliteCacheMode.Shared,
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();

        InitializeDatabase();
    }

    //--------------------------------------------------------------------------------
    // Add
    //--------------------------------------------------------------------------------

    public void AddMetrics(IEnumerable<MetricPoint> points)
    {
        var list = points.ToList();
        if (list.Count == 0)
        {
            return;
        }

        lock (sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            using var insert = connection.CreateCommand();
            insert.Transaction = transaction;
            insert.CommandText =
                """
                INSERT INTO metrics (service_name, device_id, received_at_utc, resource_json, scope_name, metric_name, description, unit, kind, timestamp_utc, value, count, min, max, attributes_json)
                VALUES ($service_name, $device_id, $received_at_utc, $resource_json, $scope_name, $metric_name, $description, $unit, $kind, $timestamp_utc, $value, $count, $min, $max, $attributes_json);
                """;
            var serviceName = insert.Parameters.Add("$service_name", SqliteType.Text);
            var deviceId = insert.Parameters.Add("$device_id", SqliteType.Text);
            var receivedAt = insert.Parameters.Add("$received_at_utc", SqliteType.Text);
            var resourceJson = insert.Parameters.Add("$resource_json", SqliteType.Text);
            var scopeName = insert.Parameters.Add("$scope_name", SqliteType.Text);
            var metricName = insert.Parameters.Add("$metric_name", SqliteType.Text);
            var description = insert.Parameters.Add("$description", SqliteType.Text);
            var unit = insert.Parameters.Add("$unit", SqliteType.Text);
            var kind = insert.Parameters.Add("$kind", SqliteType.Integer);
            var timestamp = insert.Parameters.Add("$timestamp_utc", SqliteType.Text);
            var value = insert.Parameters.Add("$value", SqliteType.Real);
            var count = insert.Parameters.Add("$count", SqliteType.Integer);
            var min = insert.Parameters.Add("$min", SqliteType.Real);
            var max = insert.Parameters.Add("$max", SqliteType.Real);
            var attributesJson = insert.Parameters.Add("$attributes_json", SqliteType.Text);

            var series = new HashSet<(string Service, string Metric)>();
            var devices = new Dictionary<string, (ResourceInfo Resource, DateTimeOffset ReceivedAt)>(StringComparer.Ordinal);
            foreach (var point in list)
            {
                serviceName.Value = point.Resource.ServiceName;
                deviceId.Value = point.Resource.DeviceId;
                receivedAt.Value = FormatDateTime(point.ReceivedAt);
                resourceJson.Value = SerializeAttributes(point.Resource.Attributes);
                scopeName.Value = point.ScopeName;
                metricName.Value = point.Name;
                description.Value = (object?)point.Description ?? DBNull.Value;
                unit.Value = (object?)point.Unit ?? DBNull.Value;
                kind.Value = (int)point.Kind;
                timestamp.Value = FormatDateTime(point.Timestamp);
                value.Value = point.Value;
                count.Value = ToDbValue(point.Count);
                min.Value = ToDbValue(point.Min);
                max.Value = ToDbValue(point.Max);
                attributesJson.Value = SerializeAttributes(point.Attributes);
                insert.ExecuteNonQuery();

                series.Add((point.Resource.ServiceName, point.Name));
                TrackDevice(devices, point.Resource, point.ReceivedAt);
            }

            foreach (var (service, metric) in series)
            {
                TrimMetricSeries(connection, transaction, service, metric);
            }

            UpsertDevices(connection, transaction, devices);
            UpdateLastReceivedAt(connection, transaction);
            transaction.Commit();
        }

        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void AddLogs(IEnumerable<LogEntry> logs)
    {
        var list = logs.ToList();
        if (list.Count == 0)
        {
            return;
        }

        lock (sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            using var insert = connection.CreateCommand();
            insert.Transaction = transaction;
            insert.CommandText =
                """
                INSERT INTO logs (service_name, device_id, received_at_utc, resource_json, scope_name, timestamp_utc, severity_text, severity_number, body, trace_id, span_id, attributes_json)
                VALUES ($service_name, $device_id, $received_at_utc, $resource_json, $scope_name, $timestamp_utc, $severity_text, $severity_number, $body, $trace_id, $span_id, $attributes_json);
                """;
            var serviceName = insert.Parameters.Add("$service_name", SqliteType.Text);
            var deviceId = insert.Parameters.Add("$device_id", SqliteType.Text);
            var receivedAt = insert.Parameters.Add("$received_at_utc", SqliteType.Text);
            var resourceJson = insert.Parameters.Add("$resource_json", SqliteType.Text);
            var scopeName = insert.Parameters.Add("$scope_name", SqliteType.Text);
            var timestamp = insert.Parameters.Add("$timestamp_utc", SqliteType.Text);
            var severityText = insert.Parameters.Add("$severity_text", SqliteType.Text);
            var severityNumber = insert.Parameters.Add("$severity_number", SqliteType.Integer);
            var body = insert.Parameters.Add("$body", SqliteType.Text);
            var traceId = insert.Parameters.Add("$trace_id", SqliteType.Text);
            var spanId = insert.Parameters.Add("$span_id", SqliteType.Text);
            var attributesJson = insert.Parameters.Add("$attributes_json", SqliteType.Text);

            var services = new HashSet<string>(StringComparer.Ordinal);
            var devices = new Dictionary<string, (ResourceInfo Resource, DateTimeOffset ReceivedAt)>(StringComparer.Ordinal);
            foreach (var log in list)
            {
                serviceName.Value = log.Resource.ServiceName;
                deviceId.Value = log.Resource.DeviceId;
                receivedAt.Value = FormatDateTime(log.ReceivedAt);
                resourceJson.Value = SerializeAttributes(log.Resource.Attributes);
                scopeName.Value = log.ScopeName;
                timestamp.Value = FormatDateTime(log.Timestamp);
                severityText.Value = log.SeverityText;
                severityNumber.Value = log.SeverityNumber;
                body.Value = log.Body;
                traceId.Value = (object?)log.TraceId ?? DBNull.Value;
                spanId.Value = (object?)log.SpanId ?? DBNull.Value;
                attributesJson.Value = SerializeAttributes(log.Attributes);
                insert.ExecuteNonQuery();

                services.Add(log.Resource.ServiceName);
                TrackDevice(devices, log.Resource, log.ReceivedAt);
            }

            foreach (var service in services)
            {
                TrimLogs(connection, transaction, service);
            }

            UpsertDevices(connection, transaction, devices);
            UpdateLastReceivedAt(connection, transaction);
            transaction.Commit();
        }

        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void AddSpans(IEnumerable<SpanEntry> spans)
    {
        var list = spans.ToList();
        if (list.Count == 0)
        {
            return;
        }

        lock (sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            using var insert = connection.CreateCommand();
            insert.Transaction = transaction;
            insert.CommandText =
                """
                INSERT INTO spans (trace_id, span_id, parent_span_id, service_name, device_id, received_at_utc, resource_json, scope_name, name, kind, start_time_utc, end_time_utc, status_code, status_message, attributes_json)
                VALUES ($trace_id, $span_id, $parent_span_id, $service_name, $device_id, $received_at_utc, $resource_json, $scope_name, $name, $kind, $start_time_utc, $end_time_utc, $status_code, $status_message, $attributes_json);
                """;
            var traceId = insert.Parameters.Add("$trace_id", SqliteType.Text);
            var spanId = insert.Parameters.Add("$span_id", SqliteType.Text);
            var parentSpanId = insert.Parameters.Add("$parent_span_id", SqliteType.Text);
            var serviceName = insert.Parameters.Add("$service_name", SqliteType.Text);
            var deviceId = insert.Parameters.Add("$device_id", SqliteType.Text);
            var receivedAt = insert.Parameters.Add("$received_at_utc", SqliteType.Text);
            var resourceJson = insert.Parameters.Add("$resource_json", SqliteType.Text);
            var scopeName = insert.Parameters.Add("$scope_name", SqliteType.Text);
            var name = insert.Parameters.Add("$name", SqliteType.Text);
            var kind = insert.Parameters.Add("$kind", SqliteType.Text);
            var startTime = insert.Parameters.Add("$start_time_utc", SqliteType.Text);
            var endTime = insert.Parameters.Add("$end_time_utc", SqliteType.Text);
            var statusCode = insert.Parameters.Add("$status_code", SqliteType.Text);
            var statusMessage = insert.Parameters.Add("$status_message", SqliteType.Text);
            var attributesJson = insert.Parameters.Add("$attributes_json", SqliteType.Text);

            var traces = new HashSet<string>(StringComparer.Ordinal);
            var devices = new Dictionary<string, (ResourceInfo Resource, DateTimeOffset ReceivedAt)>(StringComparer.Ordinal);
            foreach (var span in list)
            {
                traceId.Value = span.TraceId;
                spanId.Value = span.SpanId;
                parentSpanId.Value = (object?)span.ParentSpanId ?? DBNull.Value;
                serviceName.Value = span.Resource.ServiceName;
                deviceId.Value = span.Resource.DeviceId;
                receivedAt.Value = FormatDateTime(span.ReceivedAt);
                resourceJson.Value = SerializeAttributes(span.Resource.Attributes);
                scopeName.Value = span.ScopeName;
                name.Value = span.Name;
                kind.Value = span.Kind;
                startTime.Value = FormatDateTime(span.StartTime);
                endTime.Value = FormatDateTime(span.EndTime);
                statusCode.Value = span.StatusCode;
                statusMessage.Value = (object?)span.StatusMessage ?? DBNull.Value;
                attributesJson.Value = SerializeAttributes(span.Attributes);
                insert.ExecuteNonQuery();

                traces.Add(span.TraceId);
                TrackDevice(devices, span.Resource, span.ReceivedAt);
            }

            foreach (var trace in traces)
            {
                TrimTraceSpans(connection, transaction, trace);
                RefreshTraceInfo(connection, transaction, trace);
            }

            EnforceTraceLimit(connection, transaction);
            UpsertDevices(connection, transaction, devices);
            UpdateLastReceivedAt(connection, transaction);
            transaction.Commit();
        }

        Changed?.Invoke(this, EventArgs.Empty);
    }

    //--------------------------------------------------------------------------------
    // Query
    //--------------------------------------------------------------------------------

    public IReadOnlyList<string> GetServiceNames()
    {
        lock (sync)
        {
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT service_name
                FROM (SELECT service_name FROM metrics UNION SELECT service_name FROM logs UNION SELECT service_name FROM spans)
                ORDER BY service_name;
                """;
            return ReadStrings(command);
        }
    }

    public IReadOnlyList<string> GetDeviceIds()
    {
        lock (sync)
        {
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT device_id FROM devices ORDER BY device_id;";
            return ReadStrings(command);
        }
    }

    public IReadOnlyList<DeviceInfo> GetDevices()
    {
        lock (sync)
        {
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT device_id, service_name, resource_json, first_seen_utc, last_seen_utc,
                    (SELECT COUNT(*) FROM logs WHERE logs.device_id = devices.device_id),
                    (SELECT COUNT(*) FROM spans WHERE spans.device_id = devices.device_id),
                    (SELECT COUNT(*) FROM metrics WHERE metrics.device_id = devices.device_id)
                FROM devices
                ORDER BY last_seen_utc DESC;
                """;

            var devices = new List<DeviceInfo>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var resource = DeserializeResource(reader.GetString(2));
                devices.Add(new DeviceInfo(
                    reader.GetString(0),
                    reader.GetString(1),
                    resource.GetAttribute(ResourceInfo.ServiceVersionKey),
                    resource.GetAttribute(ResourceInfo.DeviceModelKey),
                    resource.GetAttribute(ResourceInfo.OsNameKey),
                    resource.GetAttribute(ResourceInfo.OsVersionKey),
                    ParseDateTime(reader.GetString(3)),
                    ParseDateTime(reader.GetString(4)),
                    reader.GetInt32(5),
                    reader.GetInt32(6),
                    reader.GetInt32(7)));
            }

            return devices;
        }
    }

    public IReadOnlyList<string> GetMetricNames(string serviceName)
    {
        lock (sync)
        {
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT DISTINCT metric_name FROM metrics WHERE service_name = $service_name ORDER BY metric_name;";
            command.Parameters.AddWithValue("$service_name", serviceName);
            return ReadStrings(command);
        }
    }

    public MetricSeriesSnapshot? GetMetricSeries(string serviceName, string metricName, int maxPoints = 500)
    {
        lock (sync)
        {
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT received_at_utc, resource_json, scope_name, description, unit, kind, timestamp_utc, value, count, min, max, attributes_json
                FROM (
                    SELECT id, received_at_utc, resource_json, scope_name, description, unit, kind, timestamp_utc, value, count, min, max, attributes_json
                    FROM metrics
                    WHERE service_name = $service_name AND metric_name = $metric_name
                    ORDER BY id DESC
                    LIMIT $max_points
                )
                ORDER BY id;
                """;
            command.Parameters.AddWithValue("$service_name", serviceName);
            command.Parameters.AddWithValue("$metric_name", metricName);
            command.Parameters.AddWithValue("$max_points", maxPoints);

            var points = new List<MetricPoint>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                points.Add(new MetricPoint(
                    ParseDateTime(reader.GetString(0)),
                    DeserializeResource(reader.GetString(1)),
                    reader.GetString(2),
                    metricName,
                    reader.IsDBNull(3) ? null : reader.GetString(3),
                    reader.IsDBNull(4) ? null : reader.GetString(4),
                    (MetricKind)reader.GetInt32(5),
                    ParseDateTime(reader.GetString(6)),
                    reader.GetDouble(7),
                    reader.IsDBNull(8) ? null : reader.GetInt64(8),
                    reader.IsDBNull(9) ? null : reader.GetDouble(9),
                    reader.IsDBNull(10) ? null : reader.GetDouble(10),
                    DeserializeAttributes(reader.GetString(11))));
            }

            if (points.Count == 0)
            {
                return null;
            }

            // 説明 / 単位 / 種別は最新のデータポイントのもの
            var latest = points[^1];
            return new MetricSeriesSnapshot(serviceName, metricName, latest.Unit, latest.Description, latest.Kind, points);
        }
    }

    public IReadOnlyList<LogEntry> GetLogs(LogQuery query)
    {
        lock (sync)
        {
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT received_at_utc, resource_json, scope_name, timestamp_utc, severity_text, severity_number, body, trace_id, span_id, attributes_json
                FROM logs
                WHERE ($service_name IS NULL OR service_name = $service_name)
                  AND ($device_id IS NULL OR device_id = $device_id)
                  AND severity_number >= $min_severity
                  AND ($text IS NULL OR instr(lower(body), lower($text)) > 0)
                  AND ($trace_id IS NULL OR trace_id = $trace_id)
                ORDER BY timestamp_utc DESC, id DESC
                LIMIT $max_count;
                """;
            command.Parameters.AddWithValue("$service_name", (object?)query.ServiceName ?? DBNull.Value);
            command.Parameters.AddWithValue("$device_id", (object?)query.DeviceId ?? DBNull.Value);
            command.Parameters.AddWithValue("$min_severity", query.MinSeverity);
            command.Parameters.AddWithValue("$text", String.IsNullOrWhiteSpace(query.Text) ? DBNull.Value : query.Text);
            command.Parameters.AddWithValue("$trace_id", (object?)query.TraceId ?? DBNull.Value);
            command.Parameters.AddWithValue("$max_count", query.MaxCount);

            var logs = new List<LogEntry>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                logs.Add(new LogEntry(
                    ParseDateTime(reader.GetString(0)),
                    DeserializeResource(reader.GetString(1)),
                    reader.GetString(2),
                    ParseDateTime(reader.GetString(3)),
                    reader.GetString(4),
                    reader.GetInt32(5),
                    reader.GetString(6),
                    reader.IsDBNull(7) ? null : reader.GetString(7),
                    reader.IsDBNull(8) ? null : reader.GetString(8),
                    DeserializeAttributes(reader.GetString(9))));
            }

            return logs;
        }
    }

    public IReadOnlyList<TraceSummary> GetTraces(TraceQuery query)
    {
        lock (sync)
        {
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT trace_id, primary_service, device_id, services_json, root_span_name, start_time_utc, end_time_utc, span_count, error_count
                FROM trace_info
                WHERE ($service_name IS NULL OR EXISTS (SELECT 1 FROM spans WHERE spans.trace_id = trace_info.trace_id AND spans.service_name = $service_name))
                  AND ($device_id IS NULL OR EXISTS (SELECT 1 FROM spans WHERE spans.trace_id = trace_info.trace_id AND spans.device_id = $device_id))
                  AND ($errors_only = 0 OR error_count > 0)
                  AND ($text IS NULL OR instr(lower(root_span_name), lower($text)) > 0)
                ORDER BY last_updated_utc DESC
                LIMIT $max_count;
                """;
            command.Parameters.AddWithValue("$service_name", (object?)query.ServiceName ?? DBNull.Value);
            command.Parameters.AddWithValue("$device_id", (object?)query.DeviceId ?? DBNull.Value);
            command.Parameters.AddWithValue("$errors_only", query.ErrorsOnly ? 1 : 0);
            command.Parameters.AddWithValue("$text", String.IsNullOrWhiteSpace(query.Text) ? DBNull.Value : query.Text);
            command.Parameters.AddWithValue("$max_count", query.MaxCount);

            var traces = new List<TraceSummary>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                traces.Add(new TraceSummary(
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    DeserializeStrings(reader.GetString(3)),
                    reader.GetString(4),
                    ParseDateTime(reader.GetString(5)),
                    ParseDateTime(reader.GetString(6)),
                    reader.GetInt32(7),
                    reader.GetInt32(8)));
            }

            return traces;
        }
    }

    public TraceDetail? GetTrace(string traceId)
    {
        lock (sync)
        {
            using var connection = OpenConnection();
            var spans = ReadTraceSpans(connection, traceId, null);
            if (spans.Count == 0)
            {
                return null;
            }

            var info = ComputeTraceInfo(spans);
            return new TraceDetail(traceId, info.PrimaryService, info.DeviceId, info.Services, info.StartTime, info.EndTime, info.ErrorCount, spans);
        }
    }

    public IngestHistory GetIngestHistory(TimeSpan window)
    {
        var now = DateTimeOffset.UtcNow;
        var from = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, TimeSpan.Zero) - window;
        var cutoff = FormatDateTime(from);
        lock (sync)
        {
            using var connection = OpenConnection();
            using var logs = connection.CreateCommand();
            logs.CommandText = "SELECT substr(received_at_utc, 1, 16) AS minute, COUNT(*) FROM logs WHERE received_at_utc >= $cutoff GROUP BY minute;";
            using var spans = connection.CreateCommand();
            spans.CommandText = "SELECT substr(received_at_utc, 1, 16) AS minute, COUNT(*) FROM spans WHERE received_at_utc >= $cutoff GROUP BY minute;";
            using var metrics = connection.CreateCommand();
            metrics.CommandText = "SELECT substr(received_at_utc, 1, 16) AS minute, COUNT(*) FROM metrics WHERE received_at_utc >= $cutoff GROUP BY minute;";
            return new IngestHistory(
                ReadIngest(logs, cutoff, from, now),
                ReadIngest(spans, cutoff, from, now),
                ReadIngest(metrics, cutoff, from, now));
        }
    }

    public TelemetrySummary GetSummary()
    {
        lock (sync)
        {
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT
                    (SELECT COUNT(*) FROM (SELECT service_name FROM metrics UNION SELECT service_name FROM logs UNION SELECT service_name FROM spans)),
                    (SELECT COUNT(*) FROM devices),
                    (SELECT COUNT(*) FROM metrics),
                    (SELECT COUNT(*) FROM logs),
                    (SELECT COUNT(*) FROM spans),
                    (SELECT COUNT(*) FROM trace_info),
                    (SELECT value FROM metadata WHERE key = $key);
                """;
            command.Parameters.AddWithValue("$key", LastReceivedAtKey);

            using var reader = command.ExecuteReader();
            reader.Read();
            return new TelemetrySummary(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetInt32(2),
                reader.GetInt32(3),
                reader.GetInt32(4),
                reader.GetInt32(5),
                reader.IsDBNull(6) ? null : ParseDateTime(reader.GetString(6)));
        }
    }

    //--------------------------------------------------------------------------------
    // Maintenance
    //--------------------------------------------------------------------------------

    public void Clear()
    {
        lock (sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText =
                """
                DELETE FROM metrics;
                DELETE FROM logs;
                DELETE FROM spans;
                DELETE FROM trace_info;
                DELETE FROM devices;
                UPDATE metadata SET value = NULL WHERE key = $key;
                """;
            command.Parameters.AddWithValue("$key", LastReceivedAtKey);
            command.ExecuteNonQuery();
            transaction.Commit();
        }

        Changed?.Invoke(this, EventArgs.Empty);
    }

    public int PurgeExpired()
    {
        if (options.RetentionDays <= 0)
        {
            return 0;
        }

        // 時刻は UTC の往復書式で保存しているので文字列の比較で並ぶ
        var cutoff = FormatDateTime(DateTimeOffset.UtcNow.AddDays(-options.RetentionDays));
        int deleted;
        lock (sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.Parameters.AddWithValue("$cutoff", cutoff);
            command.CommandText = "DELETE FROM metrics WHERE received_at_utc < $cutoff;";
            deleted = command.ExecuteNonQuery();
            command.CommandText = "DELETE FROM logs WHERE received_at_utc < $cutoff;";
            deleted += command.ExecuteNonQuery();
            command.CommandText = "DELETE FROM spans WHERE received_at_utc < $cutoff;";
            deleted += command.ExecuteNonQuery();
            command.CommandText = "DELETE FROM trace_info WHERE last_updated_utc < $cutoff;";
            deleted += command.ExecuteNonQuery();
            command.CommandText = "DELETE FROM trace_info WHERE trace_id NOT IN (SELECT trace_id FROM spans);";
            deleted += command.ExecuteNonQuery();
            command.CommandText = "DELETE FROM devices WHERE last_seen_utc < $cutoff;";
            deleted += command.ExecuteNonQuery();
            transaction.Commit();
        }

        if (deleted > 0)
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        return deleted;
    }

    //--------------------------------------------------------------------------------
    // Database
    //--------------------------------------------------------------------------------

    private void InitializeDatabase()
    {
        lock (sync)
        {
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText =
                """
                PRAGMA journal_mode = WAL;

                CREATE TABLE IF NOT EXISTS metrics (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    service_name TEXT NOT NULL,
                    device_id TEXT NOT NULL,
                    received_at_utc TEXT NOT NULL,
                    resource_json TEXT NOT NULL,
                    scope_name TEXT NOT NULL,
                    metric_name TEXT NOT NULL,
                    description TEXT NULL,
                    unit TEXT NULL,
                    kind INTEGER NOT NULL,
                    timestamp_utc TEXT NOT NULL,
                    value REAL NOT NULL,
                    count INTEGER NULL,
                    min REAL NULL,
                    max REAL NULL,
                    attributes_json TEXT NOT NULL
                );
                CREATE INDEX IF NOT EXISTS ix_metrics_service_metric_id ON metrics (service_name, metric_name, id DESC);
                CREATE INDEX IF NOT EXISTS ix_metrics_received ON metrics (received_at_utc);

                CREATE TABLE IF NOT EXISTS logs (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    service_name TEXT NOT NULL,
                    device_id TEXT NOT NULL,
                    received_at_utc TEXT NOT NULL,
                    resource_json TEXT NOT NULL,
                    scope_name TEXT NOT NULL,
                    timestamp_utc TEXT NOT NULL,
                    severity_text TEXT NOT NULL,
                    severity_number INTEGER NOT NULL,
                    body TEXT NOT NULL,
                    trace_id TEXT NULL,
                    span_id TEXT NULL,
                    attributes_json TEXT NOT NULL
                );
                CREATE INDEX IF NOT EXISTS ix_logs_service_id ON logs (service_name, id DESC);
                CREATE INDEX IF NOT EXISTS ix_logs_timestamp_id ON logs (timestamp_utc DESC, id DESC);
                CREATE INDEX IF NOT EXISTS ix_logs_trace ON logs (trace_id);
                CREATE INDEX IF NOT EXISTS ix_logs_received ON logs (received_at_utc);

                CREATE TABLE IF NOT EXISTS spans (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    trace_id TEXT NOT NULL,
                    span_id TEXT NOT NULL,
                    parent_span_id TEXT NULL,
                    service_name TEXT NOT NULL,
                    device_id TEXT NOT NULL,
                    received_at_utc TEXT NOT NULL,
                    resource_json TEXT NOT NULL,
                    scope_name TEXT NOT NULL,
                    name TEXT NOT NULL,
                    kind TEXT NOT NULL,
                    start_time_utc TEXT NOT NULL,
                    end_time_utc TEXT NOT NULL,
                    status_code TEXT NOT NULL,
                    status_message TEXT NULL,
                    attributes_json TEXT NOT NULL
                );
                CREATE INDEX IF NOT EXISTS ix_spans_trace_start_id ON spans (trace_id, start_time_utc, id);
                CREATE INDEX IF NOT EXISTS ix_spans_service_trace ON spans (service_name, trace_id);
                CREATE INDEX IF NOT EXISTS ix_spans_device_trace ON spans (device_id, trace_id);
                CREATE INDEX IF NOT EXISTS ix_spans_received ON spans (received_at_utc);

                CREATE TABLE IF NOT EXISTS trace_info (
                    trace_id TEXT PRIMARY KEY,
                    primary_service TEXT NOT NULL,
                    device_id TEXT NOT NULL,
                    services_json TEXT NOT NULL,
                    root_span_name TEXT NOT NULL,
                    start_time_utc TEXT NOT NULL,
                    end_time_utc TEXT NOT NULL,
                    last_updated_utc TEXT NOT NULL,
                    span_count INTEGER NOT NULL,
                    error_count INTEGER NOT NULL
                );
                CREATE INDEX IF NOT EXISTS ix_trace_info_last_updated ON trace_info (last_updated_utc DESC);

                CREATE TABLE IF NOT EXISTS devices (
                    device_id TEXT PRIMARY KEY,
                    service_name TEXT NOT NULL,
                    resource_json TEXT NOT NULL,
                    first_seen_utc TEXT NOT NULL,
                    last_seen_utc TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS metadata (
                    key TEXT PRIMARY KEY,
                    value TEXT NULL
                );
                INSERT INTO metadata (key, value) VALUES ($key, NULL) ON CONFLICT(key) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$key", LastReceivedAtKey);
            command.ExecuteNonQuery();
        }
    }

    private SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA busy_timeout = 5000;";
        command.ExecuteNonQuery();
        return connection;
    }

    private static List<string> ReadStrings(SqliteCommand command)
    {
        var values = new List<string>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            values.Add(reader.GetString(0));
        }

        return values;
    }

    private static List<IngestPoint> ReadIngest(SqliteCommand command, string cutoff, DateTimeOffset from, DateTimeOffset to)
    {
        command.Parameters.AddWithValue("$cutoff", cutoff);

        var counts = new Dictionary<DateTimeOffset, int>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var minute = DateTimeOffset.ParseExact(reader.GetString(0), MinuteFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            counts[minute] = reader.GetInt32(1);
        }

        // 受信の無い分は 0 で埋める
        var points = new List<IngestPoint>();
        for (var minute = from; minute <= to; minute = minute.AddMinutes(1))
        {
            points.Add(new IngestPoint(minute, counts.GetValueOrDefault(minute)));
        }

        return points;
    }

    private void TrimMetricSeries(SqliteConnection connection, SqliteTransaction transaction, string serviceName, string metricName)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            """
            DELETE FROM metrics
            WHERE service_name = $service_name AND metric_name = $metric_name
              AND id NOT IN (SELECT id FROM metrics WHERE service_name = $service_name AND metric_name = $metric_name ORDER BY id DESC LIMIT $limit);
            """;
        command.Parameters.AddWithValue("$service_name", serviceName);
        command.Parameters.AddWithValue("$metric_name", metricName);
        command.Parameters.AddWithValue("$limit", options.MaxPointsPerMetricSeries);
        command.ExecuteNonQuery();
    }

    private void TrimLogs(SqliteConnection connection, SqliteTransaction transaction, string serviceName)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            """
            DELETE FROM logs
            WHERE service_name = $service_name
              AND id NOT IN (SELECT id FROM logs WHERE service_name = $service_name ORDER BY id DESC LIMIT $limit);
            """;
        command.Parameters.AddWithValue("$service_name", serviceName);
        command.Parameters.AddWithValue("$limit", options.MaxLogsPerService);
        command.ExecuteNonQuery();
    }

    private void TrimTraceSpans(SqliteConnection connection, SqliteTransaction transaction, string traceId)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            """
            DELETE FROM spans
            WHERE trace_id = $trace_id
              AND id NOT IN (SELECT id FROM spans WHERE trace_id = $trace_id ORDER BY start_time_utc DESC, id DESC LIMIT $limit);
            """;
        command.Parameters.AddWithValue("$trace_id", traceId);
        command.Parameters.AddWithValue("$limit", options.MaxSpansPerTrace);
        command.ExecuteNonQuery();
    }

    private static void RefreshTraceInfo(SqliteConnection connection, SqliteTransaction transaction, string traceId)
    {
        var spans = ReadTraceSpans(connection, traceId, transaction);
        if (spans.Count == 0)
        {
            using var delete = connection.CreateCommand();
            delete.Transaction = transaction;
            delete.CommandText = "DELETE FROM trace_info WHERE trace_id = $trace_id;";
            delete.Parameters.AddWithValue("$trace_id", traceId);
            delete.ExecuteNonQuery();
            return;
        }

        var info = ComputeTraceInfo(spans);
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            """
            INSERT INTO trace_info (trace_id, primary_service, device_id, services_json, root_span_name, start_time_utc, end_time_utc, last_updated_utc, span_count, error_count)
            VALUES ($trace_id, $primary_service, $device_id, $services_json, $root_span_name, $start_time_utc, $end_time_utc, $last_updated_utc, $span_count, $error_count)
            ON CONFLICT(trace_id) DO UPDATE SET
                primary_service = excluded.primary_service,
                device_id = excluded.device_id,
                services_json = excluded.services_json,
                root_span_name = excluded.root_span_name,
                start_time_utc = excluded.start_time_utc,
                end_time_utc = excluded.end_time_utc,
                last_updated_utc = excluded.last_updated_utc,
                span_count = excluded.span_count,
                error_count = excluded.error_count;
            """;
        command.Parameters.AddWithValue("$trace_id", traceId);
        command.Parameters.AddWithValue("$primary_service", info.PrimaryService);
        command.Parameters.AddWithValue("$device_id", info.DeviceId);
        command.Parameters.AddWithValue("$services_json", JsonSerializer.Serialize(info.Services, SerializerOptions));
        command.Parameters.AddWithValue("$root_span_name", info.RootSpanName);
        command.Parameters.AddWithValue("$start_time_utc", FormatDateTime(info.StartTime));
        command.Parameters.AddWithValue("$end_time_utc", FormatDateTime(info.EndTime));
        command.Parameters.AddWithValue("$last_updated_utc", FormatDateTime(DateTimeOffset.UtcNow));
        command.Parameters.AddWithValue("$span_count", spans.Count);
        command.Parameters.AddWithValue("$error_count", info.ErrorCount);
        command.ExecuteNonQuery();
    }

    private void EnforceTraceLimit(SqliteConnection connection, SqliteTransaction transaction)
    {
        using var query = connection.CreateCommand();
        query.Transaction = transaction;
        query.CommandText =
            """
            SELECT trace_id
            FROM trace_info
            ORDER BY last_updated_utc ASC, trace_id ASC
            LIMIT (SELECT CASE WHEN COUNT(*) > $limit THEN COUNT(*) - $limit ELSE 0 END FROM trace_info);
            """;
        query.Parameters.AddWithValue("$limit", options.MaxTraces);
        var traceIds = ReadStrings(query);

        foreach (var traceId in traceIds)
        {
            using var delete = connection.CreateCommand();
            delete.Transaction = transaction;
            delete.CommandText = "DELETE FROM spans WHERE trace_id = $trace_id; DELETE FROM trace_info WHERE trace_id = $trace_id;";
            delete.Parameters.AddWithValue("$trace_id", traceId);
            delete.ExecuteNonQuery();
        }
    }

    private static void TrackDevice(Dictionary<string, (ResourceInfo Resource, DateTimeOffset ReceivedAt)> devices, ResourceInfo resource, DateTimeOffset receivedAt)
    {
        var deviceId = resource.DeviceId;
        if (deviceId.Length == 0)
        {
            return;
        }

        if (!devices.TryGetValue(deviceId, out var current) || (current.ReceivedAt <= receivedAt))
        {
            devices[deviceId] = (resource, receivedAt);
        }
    }

    private static void UpsertDevices(SqliteConnection connection, SqliteTransaction transaction, Dictionary<string, (ResourceInfo Resource, DateTimeOffset ReceivedAt)> devices)
    {
        if (devices.Count == 0)
        {
            return;
        }

        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            """
            INSERT INTO devices (device_id, service_name, resource_json, first_seen_utc, last_seen_utc)
            VALUES ($device_id, $service_name, $resource_json, $seen_utc, $seen_utc)
            ON CONFLICT(device_id) DO UPDATE SET
                service_name = excluded.service_name,
                resource_json = excluded.resource_json,
                last_seen_utc = excluded.last_seen_utc;
            """;
        var deviceId = command.Parameters.Add("$device_id", SqliteType.Text);
        var serviceName = command.Parameters.Add("$service_name", SqliteType.Text);
        var resourceJson = command.Parameters.Add("$resource_json", SqliteType.Text);
        var seen = command.Parameters.Add("$seen_utc", SqliteType.Text);
        foreach (var (id, (resource, receivedAt)) in devices)
        {
            deviceId.Value = id;
            serviceName.Value = resource.ServiceName;
            resourceJson.Value = SerializeAttributes(resource.Attributes);
            seen.Value = FormatDateTime(receivedAt);
            command.ExecuteNonQuery();
        }
    }

    private static void UpdateLastReceivedAt(SqliteConnection connection, SqliteTransaction transaction)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "INSERT INTO metadata (key, value) VALUES ($key, $value) ON CONFLICT(key) DO UPDATE SET value = excluded.value;";
        command.Parameters.AddWithValue("$key", LastReceivedAtKey);
        command.Parameters.AddWithValue("$value", FormatDateTime(DateTimeOffset.UtcNow));
        command.ExecuteNonQuery();
    }

    private static List<SpanEntry> ReadTraceSpans(SqliteConnection connection, string traceId, SqliteTransaction? transaction)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            """
            SELECT received_at_utc, resource_json, scope_name, trace_id, span_id, parent_span_id, name, kind, start_time_utc, end_time_utc, status_code, status_message, attributes_json
            FROM spans
            WHERE trace_id = $trace_id
            ORDER BY start_time_utc, id;
            """;
        command.Parameters.AddWithValue("$trace_id", traceId);

        var spans = new List<SpanEntry>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            spans.Add(new SpanEntry(
                ParseDateTime(reader.GetString(0)),
                DeserializeResource(reader.GetString(1)),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.IsDBNull(5) ? null : reader.GetString(5),
                reader.GetString(6),
                reader.GetString(7),
                ParseDateTime(reader.GetString(8)),
                ParseDateTime(reader.GetString(9)),
                reader.GetString(10),
                reader.IsDBNull(11) ? null : reader.GetString(11),
                DeserializeAttributes(reader.GetString(12))));
        }

        return spans;
    }

    // ルートは親が無い (または親が届いていない) 最初のスパン
    private static TraceInfo ComputeTraceInfo(List<SpanEntry> spans)
    {
        var spanIds = new HashSet<string>(spans.Select(static x => x.SpanId), StringComparer.Ordinal);
        var root = spans.FirstOrDefault(x => String.IsNullOrEmpty(x.ParentSpanId) || !spanIds.Contains(x.ParentSpanId)) ?? spans[0];
        var services = spans.Select(static x => x.Resource.ServiceName).Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToList();
        return new TraceInfo(
            root.Resource.ServiceName,
            root.Resource.DeviceId,
            services,
            root.Name,
            spans.Min(static x => x.StartTime),
            spans.Max(static x => x.EndTime),
            spans.Count(static x => String.Equals(x.StatusCode, "ERROR", StringComparison.OrdinalIgnoreCase)));
    }

    private static object ToDbValue<T>(T? value)
        where T : struct =>
        value.HasValue ? value.Value : DBNull.Value;

    private static string FormatDateTime(DateTimeOffset value) =>
        value.UtcDateTime.ToString("O", CultureInfo.InvariantCulture);

    private static DateTimeOffset ParseDateTime(string value) =>
        DateTimeOffset.ParseExact(value, "O", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

    private static string SerializeAttributes(IReadOnlyList<KeyValueAttr> attributes) =>
        JsonSerializer.Serialize(attributes, SerializerOptions);

    private static List<KeyValueAttr> DeserializeAttributes(string json) =>
        JsonSerializer.Deserialize<List<KeyValueAttr>>(json, SerializerOptions) ?? [];

    private static ResourceInfo DeserializeResource(string json) =>
        new(DeserializeAttributes(json));

    private static List<string> DeserializeStrings(string json) =>
        JsonSerializer.Deserialize<List<string>>(json, SerializerOptions) ?? [];

    private sealed record TraceInfo(
        string PrimaryService,
        string DeviceId,
        IReadOnlyList<string> Services,
        string RootSpanName,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime,
        int ErrorCount);
}
