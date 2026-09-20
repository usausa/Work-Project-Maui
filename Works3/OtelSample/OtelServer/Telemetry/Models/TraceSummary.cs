namespace OtelServer.Telemetry.Models;

public sealed record TraceSummary(
    string TraceId,
    string PrimaryService,
    string DeviceId,
    IReadOnlyList<string> Services,
    string RootSpanName,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    int SpanCount,
    int ErrorCount)
{
    public TimeSpan Duration => EndTime - StartTime;
}

public sealed record TraceDetail(
    string TraceId,
    string PrimaryService,
    string DeviceId,
    IReadOnlyList<string> Services,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    int ErrorCount,
    IReadOnlyList<SpanEntry> Spans)
{
    public TimeSpan Duration => EndTime - StartTime;
}
