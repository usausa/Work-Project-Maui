# OtelSample — MAUI に OpenTelemetry を組み込む最小サンプル

MAUI アプリ(Android)からログ / トレース(スパン)/ メトリクスを OpenTelemetry SDK で収集し、OTLP/HTTP(protobuf)で自前の受信サーバへ送る最小構成。サーバは受信した内容をメモリに保持してブラウザの一覧に表示する。`Works3/Template` とは独立したソリューションで、同じ解析設定(`.editorconfig` / `Directory.Build.props` / `Analyzers.ruleset` / `.sln.DotSettings`)を使う。

| プロジェクト | 内容 |
| --- | --- |
| `OtelServer` | ASP.NET Core Minimal API。OTLP/HTTP の受け口(`POST /v1/logs` / `/v1/traces` / `/v1/metrics`)+ メモリ上のストア(直近 300 件)+ 一覧ページ(`/`、3 秒ごとに更新)+ `GET /api/*`(JSON)。OTLP のメッセージ定義は opentelemetry-proto v1.7.0 の `.proto` を Grpc.Tools でコンパイル(サービスは生成しない) |
| `OtelClient` | .NET MAUI(Android のみ、`otelsample.client`)。OpenTelemetry SDK のプロバイダー(Tracer / Meter / Logger)を `TelemetryHost` が保持し、画面の操作でログ / スパン / メトリクス / クラッシュを発生させる |

## ファイル構成

| ファイル | 何用か |
| --- | --- |
| `OtelSample.slnx` / `.editorconfig` / `Directory.Build.props` / `Analyzers.ruleset` / `OtelSample.sln.DotSettings` | ソリューションと解析設定(`Directory.Build.props` が `Analyzers.ruleset` を `CodeAnalysisRuleSet` で参照) |
| `OtelServer/Protos/opentelemetry/proto/**/*.proto` | OTLP のメッセージ定義(common / resource / logs / trace / metrics と collector の `*_service.proto`)。csproj の `<Protobuf ... GrpcServices="None" />`。`logs/` ディレクトリはリポジトリ直下の `.gitignore`(`[Ll]ogs/`)に当たるため `OtelSample/.gitignore` の `!**/logs/` で打ち消す |
| `OtelServer/Program.cs` | `AddRequestDecompression`(gzip 本文)、OTLP の受け口(本文を `ExportXxxServiceRequest.Parser` で読む。壊れていれば 400)、`GET /api/summary` / `/api/logs` / `/api/spans` / `/api/metrics`、`GET /api/time`(クライアントの HttpClient 計装の確認用) |
| `OtelServer/TelemetryStore.cs` / `TelemetryEntries.cs` | 受信した OTLP を画面向けに平坦化(`LogEntry` / `SpanEntry` / `MetricEntry` / `Summary`)して直近 300 件だけ保持。リソース属性 `service.name` / `device.id` を取り出す。ヒストグラムは count / sum / min / max |
| `OtelServer/wwwroot/index.html` | 一覧ページ(ログ / スパン / メトリクスの表、要約、端末一覧)。長い属性値は 160 文字で切り詰め、全文はツールチップ |
| `OtelServer/Log.cs` / `Properties/launchSettings.json` | `LoggerMessage`、`http://localhost:4318`(OTLP/HTTP の既定ポート) |
| `OtelClient/Services/TelemetryHost.cs` | 送信基盤の本体。プロバイダーの構築 / 停止 / Flush、`ILoggerProvider` としてアプリの `ILogger` を転送、`ActivitySource` / `Meter` と計測 API(`RunWorkAsync` / `CountClick`)、未処理例外のフック |
| `OtelClient/Services/SdkEventListener.cs` | OpenTelemetry SDK / エクスポーターの自己診断(`EventSource`)を拾う(送信失敗などは例外にならずここに出る) |
| `OtelClient/Services/TelemetrySettings.cs` / `TelemetryOptions.cs` / `WorkResult.cs` | 設定キー(接続先 / MAUI スパン / 端末 ID)、適用する設定、処理の結果 |
| `OtelClient/Platforms/Android/TelemetryHost.android.cs` | `AndroidEnvironment.UnhandledExceptionRaiser` のフック(Java 側へ伝播して落ちる前に送る) |
| `OtelClient/Platforms/Android/AndroidManifest.xml` / `MainActivity.cs` / `MainApplication.cs` | 権限(`INTERNET` / `ACCESS_NETWORK_STATE` / `BATTERY_STATS`)、平文 HTTP の許可 |
| `OtelClient/MainPage.xaml` + `MainViewModel.cs` | 接続先 / MAUI スパンの切替 / 適用(開始)/ 停止 / Flush / ログ 3 種 / 処理(成功・失敗)/ クラッシュ / 状態 / 端末 ID / 再送待ち / SDK の自己診断 / 操作の記録 |
| `OtelClient/MauiProgram.cs` / `App.xaml(.cs)` / `Log.cs` / `GlobalUsing.cs` | DI(`AddMetrics`、`TelemetryHost` = シングルトン + `ILoggerProvider`)、起動時のフック登録と保存済み接続先での自動開始、`LoggerMessage` |
| `OtelClient/OtelClient.csproj` | パッケージ(`OpenTelemetry` / `Exporter.OpenTelemetryProtocol` / `Instrumentation.Http` / `Instrumentation.Runtime` 1.18.0、`Microsoft.Extensions.Diagnostics`)と `EventSourceSupport=true` |

## 実行

```bash
# サーバ(http://localhost:4318。一覧は http://localhost:4318/)
dotnet run --project OtelServer
# LAN の端末から繋ぐ場合
dotnet run --project OtelServer --urls http://0.0.0.0:4318

# クライアント(USB の端末。adb reverse で端末の localhost:4318 を PC へ転送)
adb reverse tcp:4318 tcp:4318
dotnet build OtelClient -f net10.0-android -t:Run

# 受信内容(JSON)
curl http://localhost:4318/api/summary
curl http://localhost:4318/api/logs
```

アプリの操作:

1. 接続先(既定 `http://localhost:4318/`。LAN なら `http://<PC の IP>:4318/`)を入力して「適用 (開始)」。接続先は保存され、次回の起動時は自動で送信を始める。「Telemetry started」のログと、5 秒ごとのメトリクスがサーバに届く
2. 「Info」「Warning」「Error (例外)」は本文欄の文字列を `ILogger` で記録する(Error は例外付き。`exception.type` / `exception.message` / `exception.stacktrace` 属性が付く)
3. 「処理 (成功)」「処理 (失敗)」は `Work` スパン(子スパン `Compute` + `GET /api/time` の HttpClient スパン)と、所要時間のヒストグラム / ボタン回数のカウンターを記録する。スパンの中で出したログにはトレース ID が付く
4. 「MAUI のレイアウト計測をスパンとして送る」を入れて「適用」すると、MAUI 自身の `Measure` / `Arrange` のスパンも送る(量が多いので既定は切)
5. 「Flush」は溜まっている分を今すぐ送る。「停止」はプロバイダーを破棄する(溜まっている分を送ってから閉じる)
6. 「クラッシュ」は未処理例外で落ちる。落ちる前に Critical のログが届く
7. サーバを止めた状態で操作すると、赤字に SDK の自己診断(送信失敗と件数)が出て「再送待ち (ディスク退避)」が増える。サーバを戻すと最大 60 秒後に再送され 0 に戻る

## 送信の設計

| 項目 | 内容 |
| --- | --- |
| プロトコル | OTLP/HTTP(protobuf)。`OtlpExporterOptions.Protocol = HttpProtobuf`、エンドポイントはシグナルごとに `v1/traces` / `v1/metrics` / `v1/logs` まで指定、タイムアウト 10 秒 |
| リソース属性 | `service.name` = `OtelClient`、`service.version`、`device.id`(初回に生成して `Preferences` に保存)、`device.model`、`os.name`、`os.version` |
| トレース | `Sdk.CreateTracerProviderBuilder()` + `AddSource("OtelClient")` + `AddHttpClientInstrumentation()`(+ 設定時 `AddSource("Microsoft.Maui")`)。バッチ送信 2 秒間隔 |
| メトリクス | `Sdk.CreateMeterProviderBuilder()` + `AddMeter("OtelClient" / "Microsoft.Maui" / "System.Net.Http")` + `AddRuntimeInstrumentation()`。5 秒ごとに送信。自前の計器は `app.button.clicks`(カウンター)/ `app.work.duration`(ヒストグラム、ms)/ `device.battery.level`(観測ゲージ) |
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
5. サーバ停止中に Info / 処理 → 赤字に `Error (n) OpenTelemetry-Exporter-OpenTelemetryProtocol: Exporter failed send data to collector to http://localhost:4318/v1/metrics endpoint. Data will not be sent. Exception: System.Net.WebException: unexpected end of stream on com.android.okhttp.Address@…`、再送待ちが増える(`cache/otlp/{logs,traces,metrics}/*.blob`)。停止は 10.1 秒(サーバ稼働時は 137 ms)、その間も画面は操作できる
6. サーバ再起動 → 約 60 秒後に退避分がまとめて届き(元の時刻のまま)、再送待ちは 0 に戻る
7. クラッシュ → `Crash button pressed`(Warning)と `Unhandled exception.`(Critical、`exception.message=Crash test (sample).`)が届いてからプロセスが落ちる(logcat の `FATAL EXCEPTION: main` は約 80 ms 後)。再起動すると保存済みの接続先で自動的に送信を再開する
8. 一覧ページ(`http://localhost:4318/`)で上記が表示される。受信件数は `curl http://localhost:4318/api/summary`、端末側の退避ファイルは `adb shell run-as otelsample.client find cache/otlp -name "*.blob"`

## 解析設定と実装上の制約

- ビルドはサーバ / クライアントとも Debug / Release で 0 警告、`jb inspectcode OtelSample.slnx -f=xml -o=results.xml --no-build --no-swea --properties:Configuration=Release` は 0 件
- サーバは exe のため型は internal(CA1515)。`TelemetryStore` は `AddSingleton(new TelemetryStore())` で登録(CA1812 回避)、ログの引数は先に変数へ受ける(CA1873)
- クライアント: `ILoggerFactory` / `LoggerProvider` を `IDisposable` のフィールドで持つと CA2213 になるため `ServiceProvider` だけを持ち都度解決する。ログ出力は `LoggerMessage`(CA1848)。`Random` は使わない(CA5394)。UI 側の `await` は `ConfigureAwait(true)` を明示。`Platforms/Android` は IDE0130 を抑止して `OtelClient.Services` 名前空間、`App` は `Android.App` との衝突(CA1724)を抑止、`GlobalUsing.cs` は `#pragma warning disable`
- inspectcode 向け: `ActivitySource.StartActivity(name)` の `name` は `[CallerMemberName]` なので明示すると `ExplicitCallerInfoArgument` になる(該当メソッドを `// ReSharper disable/restore` で抑止)。ロック外で読む `logServices` は `InconsistentlySynchronizedField`(理由付きで抑止)。`HttpClient` は static(`ShortLivedHttpClient`)。`App` / `MainPage` の code-behind は基底型を書かない、既定テンプレートの `Styles.xaml` にある `Shell` / `NavigationPage` / `TabbedPage` のスタイルは削除(`Xaml.RedundantPropertyTypeQualifier`)、`App.xaml` の未使用 `xmlns:local` は削除
- MVVM は `CommunityToolkit.Mvvm`(`[ObservableProperty]` の partial プロパティ + `[RelayCommand]`。非同期コマンドは実行中に自動で無効化)

## ナレッジ

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
