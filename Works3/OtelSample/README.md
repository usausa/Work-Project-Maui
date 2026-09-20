# OtelSample — MAUI に OpenTelemetry を組み込む最小サンプル

MAUI アプリ(Android)からログ / トレース(スパン)/ メトリクスを OpenTelemetry SDK で収集し、OTLP/HTTP(protobuf)または OTLP/gRPC で自前の受信サーバへ送る構成。サーバは受信した内容を SQLite に保持し、Blazor(MudBlazor)のダッシュボードで表示する(端末 / ログ / トレースのタイムライン / メトリクスのグラフ)。`Works3/Template` とは独立したソリューションで、同じ解析設定(`.editorconfig` / `Directory.Build.props` / `Analyzers.ruleset` / `.sln.DotSettings`)を使う。

| プロジェクト | 内容 |
| --- | --- |
| `OtelServer` | ASP.NET Core(Blazor Server + Minimal API + gRPC)。OTLP/HTTP の受け口(8080、`POST /v1/logs` / `/v1/traces` / `/v1/metrics`)と OTLP/gRPC の受け口(4317、HTTP/2 のみ)+ SQLite のストア(件数と期間の上限付き)+ ダッシュボード(`/dashboard` / `/logs` / `/traces` / `/metrics` / `/dummy-data`、受信のたびに更新)+ `GET /api/*`(JSON)。OTLP のメッセージ定義は opentelemetry-proto v1.7.0 の `.proto` を Grpc.Tools でコンパイル(collector の 3 サービスはサーバ側を生成) |
| `OtelClient` | .NET MAUI(Android のみ、`otelsample.client`)。OpenTelemetry SDK のプロバイダー(Tracer / Meter / Logger)を `TelemetryHost` が保持し、画面の操作でログ / スパン / メトリクス / クラッシュを発生させる |

## ファイル構成

| ファイル | 何用か |
| --- | --- |
| `OtelSample.slnx` / `.editorconfig` / `Directory.Build.props` / `Analyzers.ruleset` / `OtelSample.sln.DotSettings` | ソリューションと解析設定(`Directory.Build.props` が `Analyzers.ruleset` を `CodeAnalysisRuleSet` で参照) |
| `OtelServer/Protos/opentelemetry/proto/**/*.proto` | OTLP のメッセージ定義(common / resource / logs / trace / metrics と collector の `*_service.proto`)。csproj の `<Protobuf>` はメッセージが `GrpcServices="None"`、`*_service.proto` が `GrpcServices="Server"`。`logs/` ディレクトリはリポジトリ直下の `.gitignore`(`[Ll]ogs/`)に当たるため `OtelSample/.gitignore` の `!**/logs/` で打ち消す |
| `OtelServer/Program.cs` | MudBlazor / Razor コンポーネント(InteractiveServer)/ gRPC / `AddRequestDecompression`(gzip 本文)の登録、ストアと保持期間サービスの DI、OTLP/gRPC の 3 サービス(`MapGrpcService`)、OTLP/HTTP の受け口(本文を `ExportXxxServiceRequest.Parser` で読み `TelemetryMapper` でモデルへ。壊れていれば 400)、JSON API(`/api/summary` / `/api/devices` / `/api/logs` / `/api/traces` / `/api/traces/{id}` / `/api/metrics` / `/api/metrics/{name}`)、`GET /api/time`(クライアントの HttpClient 計装の確認用)、`/` → `/dashboard` |
| `OtelServer/Telemetry/Models/*.cs` | ストアのモデル(`LogEntry` / `SpanEntry` / `MetricPoint`(ヒストグラムは Value = 平均、Count / Min / Max)/ `TraceSummary` / `TraceDetail` / `DeviceInfo` / `TelemetrySummary` / `IngestHistory`)と絞り込み条件(`LogQuery` / `TraceQuery`)、`ResourceInfo`(`service.name` / `device.*` / `os.*` の取り出し) |
| `OtelServer/Telemetry/Storage/SqliteTelemetryStore.cs` / `ITelemetryStore.cs` / `TelemetryStoreOptions.cs` | SQLite(`App_Data/otel.db`、WAL)。`metrics` / `logs` / `spans` / `trace_info`(トレースの集約)/ `devices`(端末ごとの最新リソースと初回 / 最終受信)/ `metadata`。受信のたびに件数の上限(系列 / サービス / トレースごと)を適用し、`Changed` で画面へ通知。`PurgeExpired` が保持期間を過ぎた行を削除 |
| `OtelServer/Telemetry/Services/OtlpGrpcServices.cs` | OTLP/gRPC の受け口(`LogsService` / `TraceService` / `MetricsService` の `Export`。中身は OTLP/HTTP と同じ `TelemetryMapper` → ストア) |
| `OtelServer/Telemetry/Services/TelemetryMapper.cs` / `TelemetryRetentionService.cs` / `DummyDataGenerator.cs` | OTLP → モデルの平坦化(時刻は 100ns 精度、ID は小文字 16 進)、1 時間ごとの期限切れ削除、画面確認用のダミーデータ(端末 3 台、Work → Compute + GET のトレースとトレース ID 付きのログ、5 種のメトリクス。`RandomNumberGenerator` を使用) |
| `OtelServer/Components/App.razor` / `Routes.razor` / `_Imports.razor` / `Layout/MainLayout.razor` / `NavMenu.razor` / `ReconnectModal.razor(.css/.js)` | Blazor のホスト(MudBlazor のダークテーマ、ドロワーのメニュー、切断時のモーダルはテンプレートのまま) |
| `OtelServer/Components/Pages/Dashboard.razor` | 件数カード(サービス / 端末 / ログ / スパン / トレース / メトリクス / 最終受信 / 24 時間のエラー数)、受信件数の推移(直近 30 分、1 分ごと)、端末一覧(機種 / OS / バージョン / 初回・最終受信 / 件数。件数からログ / トレースへ)、直近のエラー(ログ / トレース)、接続のヒント |
| `OtelServer/Components/Pages/Logs.razor` | サービス / 端末 / 重大度 / 本文 / 件数で絞り込み(`?service=` / `?device=` / `?severity=` / `?trace=` でも指定)。行のクリックで本文の全文・属性・リソース・トレース / スパン ID を展開、Trace のクリックでトレースへ |
| `OtelServer/Components/Pages/Traces.razor` | サービス / 端末 / エラーのみ / Root span 名で絞り込み(`?trace=` で直接展開)。行のクリックでスパンのガントチャート(`SpanGantt`)、スパンの一覧(スコープ / 種別 / 開始オフセット / 所要 / 状態 / 属性)、同じトレース ID のログ |
| `OtelServer/Components/Pages/Metrics.razor` | サービスの計器をカードで一覧(最新値 / 単位 / 種別 / スパークライン / 系列数)、選んだ計器の推移(`TimeSeriesChart`。属性の組み合わせごとに系列)と直近 50 点の表(Count / Min / Max / 端末 / 属性) |
| `OtelServer/Components/Pages/DummyData.razor` | ダミーデータの投入(Metrics / Logs / Traces / All)とストアのクリア |
| `OtelServer/Components/Common/TimeSeriesChart.razor` / `SpanGantt.razor` / `Sparkline.razor` / `AttributeList.razor` / `ChartSeries.cs` / `Formats.cs` | SVG の折れ線グラフ(目盛 / 凡例 / 点のツールチップ)、ガントチャート(深さで字下げ、状態で色)、スパークライン、属性の表、表示書式(時刻 / 所要時間 / 桁の丸め / 重大度の色) |
| `OtelServer/wwwroot/app.css` / `favicon.ico` | 補助のスタイル(等幅 / 折り返し / 属性の表 / カードの選択) |
| `OtelServer/GlobalSuppressions.cs` / `Log.cs` / `appsettings.json` / `Properties/launchSettings.json` | CA1515 の抑止(Razor コンポーネントは public で生成される)、`LoggerMessage`、`TelemetryStore` の設定、`Kestrel:Endpoints`(`Http` = `http://0.0.0.0:8080`、`Grpc` = `http://0.0.0.0:4317` / `Protocols=Http2`。launchSettings に `applicationUrl` は書かない) |
| `OtelClient/Services/TelemetryHost.cs` | 送信基盤の本体。プロバイダーの構築 / 停止 / Flush、`ILoggerProvider` としてアプリの `ILogger` を転送、`ActivitySource` / `Meter` と計測 API(`RunWorkAsync` / `CountClick`)、未処理例外のフック |
| `OtelClient/Services/SdkEventListener.cs` | OpenTelemetry SDK / エクスポーターの自己診断(`EventSource`)を拾う(送信失敗などは例外にならずここに出る) |
| `OtelClient/Services/TelemetrySettings.cs` / `TelemetryOptions.cs` / `WorkResult.cs` | 設定キー(接続先 / gRPC / MAUI スパン / 端末 ID)とポート(HTTP 8080 / gRPC 4317)、適用する設定(`ApiEndpoint` = gRPC のときは HTTP ポートの URL)、処理の結果 |
| `OtelClient/Platforms/Android/TelemetryHost.android.cs` | `AndroidEnvironment.UnhandledExceptionRaiser` のフック(Java 側へ伝播して落ちる前に送る) |
| `OtelClient/Platforms/Android/AndroidManifest.xml` / `MainActivity.cs` / `MainApplication.cs` | 権限(`INTERNET` / `ACCESS_NETWORK_STATE` / `BATTERY_STATS`)、平文 HTTP の許可 |
| `OtelClient/MainPage.xaml` + `MainViewModel.cs` | 接続先 / OTLP/gRPC の切替(切替時に接続先のポートが 8080 ⇔ 4317 なら入れ替える)/ MAUI スパンの切替 / 適用(開始)/ 停止 / Flush / ログ 3 種 / 処理(成功・失敗)/ クラッシュ / 状態 / 端末 ID / 再送待ち / SDK の自己診断 / 操作の記録 |
| `OtelClient/MauiProgram.cs` / `App.xaml(.cs)` / `Log.cs` / `GlobalUsing.cs` | DI(`AddMetrics`、`TelemetryHost` = シングルトン + `ILoggerProvider`)、起動時のフック登録と保存済み接続先での自動開始、`LoggerMessage` |
| `OtelClient/OtelClient.csproj` | パッケージ(`OpenTelemetry` / `Exporter.OpenTelemetryProtocol` / `Instrumentation.Http` / `Instrumentation.Runtime` 1.18.0、`Microsoft.Extensions.Diagnostics`)と `EventSourceSupport=true` |

## 実行

```bash
# サーバ(OTLP/HTTP + 画面 = http://0.0.0.0:8080、OTLP/gRPC = http://0.0.0.0:4317。ダッシュボードは http://localhost:8080/dashboard)
dotnet run --project OtelServer

# クライアント(USB の端末。adb reverse で端末の localhost を PC へ転送。LAN なら接続先に PC の IP を指定し、Windows のファイアウォールで OtelServer.exe の受信を許可)
adb reverse tcp:8080 tcp:8080
adb reverse tcp:4317 tcp:4317
dotnet build OtelClient -f net10.0-android -t:Run

# 受信内容(JSON)
curl http://localhost:8080/api/summary
curl http://localhost:8080/api/devices
curl "http://localhost:8080/api/logs?severity=17&count=20"
curl "http://localhost:8080/api/traces?errors=true"
curl http://localhost:8080/api/traces/<traceId>
curl "http://localhost:8080/api/metrics?service=OtelClient"
curl "http://localhost:8080/api/metrics/app.work.duration?service=OtelClient&count=100"
```

サーバの設定(`appsettings.json` の `TelemetryStore`):

| キー | 既定 | 内容 |
| --- | --- | --- |
| `DatabasePath` | `App_Data/otel.db` | SQLite のパス(相対はコンテンツルート基準。`App_Data/` は `.gitignore`) |
| `RetentionDays` | `7` | この日数より古い行を 1 時間ごとに削除(`0` で無効) |
| `MaxPointsPerMetricSeries` | `500` | サービス × 計器ごとの保持点数 |
| `MaxLogsPerService` | `2000` | サービスごとのログの保持件数 |
| `MaxTraces` | `500` | トレースの保持数 |
| `MaxSpansPerTrace` | `200` | トレースごとのスパンの保持数 |

アプリの操作:

1. 接続先(既定 `http://192.168.100.10:8080/`。USB なら `http://localhost:8080/`、LAN なら `http://<PC の IP>:8080/`)を入力して「適用 (開始)」。「OTLP/gRPC で送る」を入れると接続先のポートが 4317 に変わり、gRPC で送る(`api/time` の呼び出しは 8080 のまま)。接続先は保存され、次回の起動時は自動で送信を始める。「Telemetry started」のログと、5 秒ごとのメトリクスがサーバに届く
2. 「Info」「Warning」「Error (例外)」は本文欄の文字列を `ILogger` で記録する(Error は例外付き。`exception.type` / `exception.message` / `exception.stacktrace` 属性が付く)
3. 「処理 (成功)」「処理 (失敗)」は `Work` スパン(子スパン `Compute` + `GET /api/time` の HttpClient スパン)と、所要時間のヒストグラム / ボタン回数のカウンターを記録する。スパンの中で出したログにはトレース ID が付く
4. 「MAUI のレイアウト計測をスパンとして送る」を入れて「適用」すると、MAUI 自身の `Measure` / `Arrange` のスパンも送る(量が多いので既定は切)
5. 「Flush」は溜まっている分を今すぐ送る。「停止」はプロバイダーを破棄する(溜まっている分を送ってから閉じる)
6. 「クラッシュ」は未処理例外で落ちる。落ちる前に Critical のログが届く
7. サーバを止めた状態で操作すると、赤字に SDK の自己診断(送信失敗と件数)が出て「再送待ち (ディスク退避)」が増える。サーバを戻すと最大 60 秒後に再送され 0 に戻る

## サーバの画面

| 画面 | 内容 |
| --- | --- |
| Dashboard | 件数カード、受信件数の推移(1 分ごと、ログ / スパン / メトリクス)、端末一覧(`device.id` ごとの機種 / OS / アプリのバージョン / 初回・最終受信 / 件数)、直近のエラー(ログはレベル Error 以上、トレースはエラーのスパンを含むもの)、接続のヒント |
| Logs | 絞り込み(サービス / 端末 / 重大度以上 / 本文の部分一致 / 件数)、行の展開で本文の全文(スタックトレースも折り返して全文)・属性・リソース、Trace のクリックでトレースへ。`Auto` を切ると受信で更新しない |
| Traces | 絞り込み(サービス / 端末 / エラーのみ / Root span 名)、行の展開でガントチャート + スパンの一覧 + 同じトレース ID のログ(`Logs (n)` で Logs 画面へ) |
| Metrics | 計器のカード(最新値 / スパークライン)をクリックして選択、推移のグラフ(属性の組み合わせごとに系列。ヒストグラムは平均)、直近 50 点の表(Count / Min / Max) |
| Dummy Data | 画面確認用のデータ投入とクリア(OTLP の受け口は通らない) |

- 受信のたびにストアの `Changed` が上がり、開いている画面が `InvokeAsync(StateHasChanged)` で更新される(Logs / Traces / Metrics は `Auto` で止められる)
- 時刻は UTC で保存し、表示でローカル時刻にする。受信件数の推移は `received_at_utc`(ISO 8601)の先頭 16 文字(分)で集計
- 端末は `device.id` で識別し、機種 / OS / バージョンは最後に受信したリソース属性

## 送信の設計

| 項目 | 内容 |
| --- | --- |
| プロトコル | OTLP/HTTP(protobuf)= `OtlpExporterOptions.Protocol = HttpProtobuf`、エンドポイントはシグナルごとに `v1/traces` / `v1/metrics` / `v1/logs` まで指定。OTLP/gRPC = `Protocol = Grpc`、エンドポイントは `http://host:4317/`(パス無し)、`HttpClientFactory` で `SocketsHttpHandler` の `HttpClient` を渡す(Android 既定の `AndroidMessageHandler` は HTTP/2 を話せない)。どちらもタイムアウト 10 秒 |
| リソース属性 | `service.name` = `OtelClient`、`service.version`、`device.id`(初回に生成して `Preferences` に保存)、`device.model`、`os.name`、`os.version` |
| トレース | `Sdk.CreateTracerProviderBuilder()` + `AddSource("OtelClient")` + `AddHttpClientInstrumentation()`(+ 設定時 `AddSource("Microsoft.Maui")`)。バッチ送信 2 秒間隔 |
| メトリクス | `Sdk.CreateMeterProviderBuilder()` + `AddMeter("OtelClient" / "Microsoft.Maui" / "System.Net.Http" / "System.Net.NameResolution")` + `AddRuntimeInstrumentation()`。5 秒ごとに送信。自前の計器は `app.button.clicks`(カウンター)/ `app.work.duration`(ヒストグラム、ms)/ `device.battery.level`(観測ゲージ) |
| ログ | `ServiceCollection` + `AddLogging(b => b.AddOpenTelemetry(...))` を専用の DI コンテナに閉じ込め、`ILoggerFactory` と `LoggerProvider`(Flush 用)を取り出す。`IncludeFormattedMessage` / `IncludeScopes`、バッチ送信 2 秒間隔 |
| アプリの `ILogger` との接続 | `TelemetryHost` を `ILoggerProvider` として MAUI の DI に登録し、送信中だけ内部の `ILoggerFactory` へ転送する(`ForwardingLogger`)。停止中は `IsEnabled` が false。接続先を変えるとプロバイダーだけ作り直せる |
| 開始 / 停止 / Flush | `Lock` で直列化。`Stop` はプロバイダーの `Dispose`(溜まっている分の送信を待つ。プロバイダーごとに最大 5 秒)、`Flush` は `ForceFlush(3 秒)`。どちらも接続先が落ちていると数秒ブロックするため VM は `Task.Run` で呼ぶ |
| オフライン時の退避 | 環境変数 `OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY=disk` + `OTEL_DOTNET_EXPERIMENTAL_OTLP_DISK_RETRY_DIRECTORY_PATH=<CacheDirectory>/otlp`(`Start` の前に `Environment.SetEnvironmentVariable`)。送信に失敗したバッチ(接続不能 / 429 / 502 / 503 / 504)はシグナルごとのサブフォルダに `*.blob` として保存され、エクスポーターの再送スレッドが 60 秒ごとに 1 シグナル 10 ファイルずつ再送する。画面の「再送待ち」は `*.blob` の数 |
| SDK の自己診断 | `EventListener` で名前が `OpenTelemetry` で始まる `EventSource` を Warning 以上で購読(`OpenTelemetry-Sdk` / `OpenTelemetry-Exporter-OpenTelemetryProtocol`)。Android は `EventSourceSupport` が既定 false のため csproj で true にする(これが無いと何も拾えない) |
| クラッシュ | `AndroidEnvironment.UnhandledExceptionRaiser`(Java 側へ伝播する前)+ `AppDomain.UnhandledException` + `TaskScheduler.UnobservedTaskException`(Error ログのみ)。1 回だけ Critical ログを記録して `Flush` し、そのまま落ちる。接続先が落ちていればディスクに退避され、次回起動の再送で届く |
| MAUI の計測(.NET 10) | `Microsoft.Maui` という名前の `ActivitySource` / `Meter` でレイアウトの `Measure` / `Arrange` を計測する(`maui.layout.measure_count` / `measure_duration`(ns)/ `arrange_count` / `arrange_duration`)。`Meter` は DI に `IMeterFactory` があるときだけ作られるので `builder.Services.AddMetrics()` が必要。タグに `element.id`(GUID)/ `element.frame` など要素ごとの値が付き系列が際限なく増えるため、`AddView("maui.layout.*", new MetricStreamConfiguration { TagKeys = ["element.type"] })` で型だけに集約する。所要時間のヒストグラムは `ActivitySource` を購読しているとき(スパン送信が有効なとき)だけ記録される |

## 確認済みの動作(Pixel 9a / Android 16、Debug ビルド)

1. 適用 → サーバに `Telemetry started`(Information)と、5 秒ごとのメトリクス(`dotnet.*` のランタイム計測 / `maui.layout.*` / `http.client.*` / `device.battery.level` = 100 %)
2. Info / Warning / Error → 各レベルのログ。Error は `exception.type=InvalidOperationException` と `exception.stacktrace` 付き。属性 `scope` はロガーのカテゴリ(`OtelClient.MainViewModel`)
3. 処理 (成功) → `Work`(約 190 ms、Ok)→ `Compute`(約 120 ms)/ `GET`(約 25 ms、`url.full` / `http.response.status_code=200`、scope `System.Net.Http`)の 3 スパンが同じトレース ID で届き、`Work completed` のログに同じトレース ID が付く。処理 (失敗)は `Work` が Error、`Work failed` のログに例外
4. MAUI スパンを入れて適用 → `Arrange Grid` を親に `Arrange CollectionView` / `Arrange Label` … の木が届き、`maui.layout.arrange_duration` のヒストグラムも届く
5. サーバ停止中に Info / 処理 → 赤字に `Error (n) OpenTelemetry-Exporter-OpenTelemetryProtocol: Exporter failed send data to collector to http://localhost:8080/v1/metrics endpoint. Data will not be sent. Exception: System.Net.WebException: unexpected end of stream on com.android.okhttp.Address@…`、再送待ちが増える(`cache/otlp/{logs,traces,metrics}/*.blob`)。停止は 10.1 秒(サーバ稼働時は 137 ms)、その間も画面は操作できる
6. サーバ再起動 → 約 60 秒後に退避分がまとめて届き(元の時刻のまま)、再送待ちは 0 に戻る
7. クラッシュ → `Crash button pressed`(Warning)と `Unhandled exception.`(Critical、`exception.message=Crash test (sample).`)が届いてからプロセスが落ちる(logcat の `FATAL EXCEPTION: main` は約 80 ms 後)。再起動すると保存済みの接続先で自動的に送信を再開する
8. 「OTLP/gRPC で送る」を入れて適用(USB は `adb reverse tcp:4317 tcp:4317` + `http://localhost:4317/`)→ サーバのログに `Received. signal=[metrics (gRPC)]` のように届き、画面の表示は同じ
9. ダッシュボード(`http://localhost:8080/dashboard`)で上記が表示される。受信件数は `curl http://localhost:8080/api/summary`、端末側の退避ファイルは `adb shell run-as otelsample.client find cache/otlp -name "*.blob"`

## 解析設定と実装上の制約

- ビルドはサーバ / クライアントとも Debug / Release で 0 警告、`jb inspectcode OtelSample.slnx -f=xml -o=results.xml --no-build --no-swea --properties:Configuration=Release` は 0 件
- サーバ: Razor コンポーネントは public で生成されるため CA1515 は `GlobalSuppressions.cs` で抑止し、DI で作る型も public にする(internal だと CA1812)。`SqliteCommand.CommandText` はリテラルだけを代入する(文字列の引数や補間は CA2100。テーブルごとに別の命令を作る)。null 許容の値型を `(object?)x ?? DBNull.Value` と書くと CA1508 になるので `x is { } v ? v : DBNull.Value`。名前空間 `Components.Shared` は CA1716(`Shared` は予約語)なので `Components.Common`。Razor の `@foreach (var attribute in ...)` は `@attribute` ディレクティブと衝突する。ダミーデータの乱数は `RandomNumberGenerator`(CA5394)。ログの引数は先に変数へ受ける(CA1873)
- クライアント: `ILoggerFactory` / `LoggerProvider` を `IDisposable` のフィールドで持つと CA2213 になるため `ServiceProvider` だけを持ち都度解決する。ログ出力は `LoggerMessage`(CA1848)。`Random` は使わない(CA5394)。UI 側の `await` は `ConfigureAwait(true)` を明示。`Platforms/Android` は IDE0130 を抑止して `OtelClient.Services` 名前空間、`App` は `Android.App` との衝突(CA1724)を抑止、`GlobalUsing.cs` は `#pragma warning disable`
- inspectcode 向け: `ActivitySource.StartActivity(name)` の `name` は `[CallerMemberName]` なので明示すると `ExplicitCallerInfoArgument` になる(該当メソッドを `// ReSharper disable/restore` で抑止)。ロック外で読む `logServices` は `InconsistentlySynchronizedField`(理由付きで抑止)。`HttpClient` は static(`ShortLivedHttpClient`)。`App` / `MainPage` の code-behind は基底型を書かない、既定テンプレートの `Styles.xaml` にある `Shell` / `NavigationPage` / `TabbedPage` のスタイルは削除(`Xaml.RedundantPropertyTypeQualifier`)、`App.xaml` の未使用 `xmlns:local` は削除
- MVVM は `CommunityToolkit.Mvvm`(`[ObservableProperty]` の partial プロパティ + `[RelayCommand]`。非同期コマンドは実行中に自動で無効化)

## ナレッジ

- **OTLP/HTTP と Blazor は同じポートで動く**(HTTP/1.1)。OTLP/gRPC は平文の HTTP/2(h2c)で HTTP/1.1 と同居できないため別ポート(4317、`Protocols=Http2`)。`Kestrel:Endpoints` を書くと launchSettings の `applicationUrl` は無視される(警告が出る)ので書かない
- **Android から h2c の gRPC**: `HttpClient` の既定ハンドラ(`AndroidMessageHandler`)は HTTP/2 非対応で、OTLP エクスポーターは接続できずタイムアウトする。`OtlpExporterOptions.HttpClientFactory` で `SocketsHttpHandler` を渡す。LAN 経由は Windows のファイアウォールが OtelServer.exe の受信を通していないと HTTP(8080)/ gRPC(4317)とも同じくタイムアウトする。初回起動の警告で許可しなかった場合は exe 単位の「ブロック」規則(プライベート)が残り、ポート単位の許可規則より優先される。その規則を許可に変えるか削除して起動し直す(管理者 PowerShell: `Get-NetFirewallRule -DisplayName OtelServer | Set-NetFirewallRule -Action Allow`。exe のパスごとの規則なので Debug / Release で別)。切り分けは端末の `adb shell` から `(echo; sleep 3) | nc <PC の IP> 8080`(`adb reverse` 経由の localhost は通る)
- **他の OTLP クライアント**(Claude Code など)からも受けられる: `OTEL_EXPORTER_OTLP_PROTOCOL=grpc` + `OTEL_EXPORTER_OTLP_ENDPOINT=http://<host>:4317`、または `http/protobuf` + `http://<host>:8080`
- **MudBlazor 9 + .NET 10 の静的アセット**: `MapStaticAssets` と `@Assets[...]` / `<ImportMap />` を使う。`{Project}.styles.css` は `.razor.css` があるときだけ生成される
- **`MudTable` の行の展開**: `ChildRowContent` を条件付きで描き、`OnRowClick` で開閉する。行の同一性は毎回の再取得で失われるので、時刻 + ID のキーで持つ
- **クエリ文字列の初期値**: `[SupplyParameterFromQuery(Name = "...")]` で受け、`OnInitialized` で絞り込みに写す(他の画面からのリンクに使う)
- **SQLite**: `PRAGMA journal_mode = WAL` + `busy_timeout` で受信(書き込み)と画面(読み取り)が競合しない。書き込みは `Lock` で直列化し、件数の上限は同じトランザクションで `DELETE ... WHERE id NOT IN (SELECT id ... ORDER BY id DESC LIMIT n)`

- **MAUI で OpenTelemetry を動かす形**: `OpenTelemetry.Extensions.Hosting` の `AddOpenTelemetry()` は `IHostedService` でプロバイダーを起動する前提で、MAUI にはその仕組みが無い。`Sdk.CreateTracerProviderBuilder()` / `CreateMeterProviderBuilder()` で自前に構築し、ログだけ `AddLogging(b => b.AddOpenTelemetry())` を専用の `ServiceCollection` で建てる。アプリ本体の `ILogger` へは `ILoggerProvider` の転送で繋ぐと、接続先の変更や停止でプロバイダーを作り直せる
- **`EventSourceSupport`**: .NET for Android は既定で `EventSource` を無効化する(`Microsoft.Android.Sdk.DefaultProperties.targets`)。OpenTelemetry の送信失敗は例外にならず `EventSource` にしか出ないため、これを true にしないと失敗が見えない
- **`ActivitySource` の名前**: MAUI 10 のレイアウト計測は `Microsoft.Maui`(版 `1.0.0`)。`Microsoft.Maui.RuntimeFeature.EnableDiagnostics` / `EnableMauiDiagnostics` は XAML のバインド診断向けで、レイアウト計測には関係ない(`System.Diagnostics.Metrics.Meter.IsSupported` と `IMeterFactory` の有無で決まる)
- **メトリクスのカーディナリティ**: 要素ごとのタグ(`element.id` / `element.frame`)が付く計器は `AddView` の `TagKeys` で集約しないと系列が増え続ける(SDK の上限は計器あたり 2,000 系列。超えた分は捨てられる)
- **`System.Net.Http` の計器**: .NET 8 以降の `HttpClient` は `MeterListener` があると `http.client.request.duration` などを出す。`AddMeter("System.Net.Http")` していないと「Instrument belongs to a Meter not subscribed by this provider」の警告が自己診断に出続ける
- **電池残量**: `Battery.Default.ChargeLevel` は manifest に `BATTERY_STATS` の宣言が無いと `PermissionException`(観測ゲージのコールバックで投げると SDK が握って自己診断に出す)
- **`Stop` / `Dispose` のブロック**: プロバイダーの `Dispose` は 5 秒を上限に送信を待つ。接続先が落ちていると 3 プロバイダーで 10 秒ほど止まるので、アプリの終了 / 設定変更で UI スレッドから同期的に呼ばない
- **ディスク退避の粒度**: 退避されるのは「送信に失敗したバッチ」で、キューに溜まっている未送信分ではない。落ちる前に届けたいものは `ForceFlush` で送信を試みる(失敗すればそこで退避される)。再送は 60 秒間隔 / 1 シグナル 10 ファイルなので、大量に溜まると捌けるまで時間がかかる
- **`adb reverse` と停止中のサーバ**: 接続はいったん受け付けられてから切れる(`unexpected end of stream`)。OTLP エクスポーターは状態コード無しの失敗を再送対象として扱うので退避される
- **`CollectionView` の先頭挿入**: Android の `RecyclerView` は先頭に挿入しても表示位置を保つため、新しい行が見えない。操作の記録は `Label` にまとめて出している

## 本番で追加が必要なもの

- 認証と TLS: `OtlpExporterOptions.Headers`(`Authorization` など)、`https` にして manifest の `usesCleartextTraffic` を外す
- 受け側: 本サンプルのサーバはメモリ保持のみ。実運用は OpenTelemetry Collector か、永続化と検索を持つバックエンド(Aspire ダッシュボードは開発時のみ)
- サンプリングと量の制御: `SetSampler`、`AddView` での計器の絞り込み、MAUI スパンは性能調査時だけ有効化
- 個人情報: ログの本文 / 属性に入れない。`device.id` は端末を識別する値の扱いを決める
- 終了時の送信: バックグラウンド遷移時に `ForceFlush` を非同期で呼ぶ。プロセス終了時は待てないため、退避 + 次回起動の再送に頼る
