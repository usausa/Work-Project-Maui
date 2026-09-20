# 📈クラッシュレポート / テレメトリ基盤 検討資料

MAUI アプリ(本テンプレート)からクラッシュレポート・ログ・メトリクスをサーバーへ送る基盤を検討するための初期資料。検討の候補は「DeviceManager 型のサーバー + 端末向けライブラリ」「OpenTelemetry(OTLP)で汎用の収集基盤へ送る」「外部サービス」。本書は現状・候補・論点をまとめたもので、方式の決定と設計は別途行なう。

## 🧭1. 現状

### 📱アプリ(Template.MobileApp)

| 項目 | 現状 | 位置 |
| --- | --- | --- |
| ログ | `Microsoft.Extensions.Logging` + `LoggerMessage`(`Log.cs` = 起動 / データ / 診断、`Services/Log.cs` = 通信系)。出力先は Debug(DEBUG 時)/ Android logcat / ファイル(`AddFileLogger`、外部ファイル領域の `log/`、7 日保持)。サーバーへは送らない | `MauiProgram.cs`(`ConfigureLogging`)、`Log.cs`、`Services/Log.cs` |
| クラッシュ | `TaskScheduler.UnobservedTaskException` と `AndroidEnvironment.UnhandledExceptionRaiser` で例外をファイルに保存し、次回起動時にダイアログで表示する(サーバー送信なし) | `Helpers/CrashReport.cs` + `.android.cs`、`App.xaml.cs`(`OnStart` の `ShowReport`) |
| メトリクス | 診断パネル(DEBUG 限定)にワーキングセットの推移、`Extender/LeakDetectionPlugin` でビューのリーク検出。収集・送信の仕組みは無い | `Shell/DiagnosticPanel`、`Extender/` |
| 端末の状態 | SignalR(`MonitorHub`)へ電池 / ネットワーク / 機種を 10 秒ごとに報告(Network > Realtime の画面にいる間だけ) | `Services/MonitorConnection.cs`、`Modules/Network/NetworkRealtimeViewModel.cs` |
| 識別子 | 端末 ID = `Settings.UniqueId`(初回起動時の GUID)。機種 / OS は `IDeviceInfo`、アプリのバージョンは `AppInfo` | `State/Settings.cs` |
| 設定の配布 | 接続先やキーは設定画面の QR(サーバーの `/qr`)で投入する | `Modules/Main/SettingViewModel.cs` |
| 拡張の置き場 | ナビゲーションのプラグイン(`Extender/`)と DI 登録(`MauiProgram.cs`)。DEBUG 限定の登録例あり | `Extender/LeakDetectionPlugin.cs` |

### 🖥️サーバー側(既存資産)

| 資産 | 内容 | 位置 |
| --- | --- | --- |
| template-maui-server | 本テンプレートの対向サーバー。Serilog(ファイル)、OpenTelemetry(OTLP エクスポーター / Prometheus)、ヘルスチェック、SignalR ハブ(端末の状態と通知)。テレメトリの受信機能は無い | `D:\GitHubTemplate\template-maui-server` |
| DeviceManager | MAUI 端末の管理サーバー(Blazor Server + Minimal API + SignalR + gRPC)。メトリクス(ダッシュボード)/ ログ / エラーレポート / ストレージ / メッセージ / モック Function / 設定配信。端末向け SDK(`DeviceManager.Client`)と WPF テストクライアント付き | `D:\GitHubTemplate\DeviceManager`(`README.md`、`docs/features.md`、`docs/sdk.md`) |

DeviceManager の SDK(`DeviceManager.Client`、net10.0)の要点:

- テレメトリ(ステータス / ログ / クラッシュレポート)は gRPC(`TelemetryService`、8083、h2c)。非同期の有界キュー + タンキング(オンメモリ / SQLite の `ITelemetryStore`)+ 指数バックオフの自動再送(persist-then-send、at-least-once)。クラッシュレポートは即時フラッシュ
- `DeviceManagerLoggerProvider`(`ILoggerProvider`)で `Microsoft.Extensions.Logging` のログを転送(最小レベル指定、SDK 自身のログは除外)
- 登録 / メッセージ / 設定配信は SignalR(`/hubs/device`)、ストレージと設定取得は REST。認証は `X-Api-Key`
- 端末情報と状態はプロバイダー(`IDeviceInfoProvider` / `IDeviceStatusProvider`)を利用側が実装する

## 📏2. 要件と論点

| 観点 | 内容 |
| --- | --- |
| 収集するもの | クラッシュ(未処理例外 + スタックトレース + 端末 / アプリ情報)、ログ(レベル・カテゴリ・メッセージ・例外)、メトリクス(電池 / メモリ / ネットワーク状態 / 画面遷移 / 通信の成否と所要時間)、必要ならトレース(通信・画面単位のスパン) |
| 取りこぼしの防止 | 端末は回線が切れる前提。送信前に永続化して成功時に消す(at-least-once)、再起動後の再送、キューの上限と保持期間、順序 |
| クラッシュの扱い | プロセスが落ちる直前に送れないことが多いため「保存 → 次回起動時に送信」が基本(現在の `CrashReport` と同じ)。ANR / ネイティブクラッシュは .NET のフックでは取れない |
| 送信の制御 | バッチ化とフラッシュ間隔、バックグラウンド移行時のフラッシュ、通信量と電池への影響、Wi-Fi 限定などの条件 |
| 識別と属性 | 端末 ID(`Settings.UniqueId`)、アプリのバージョン、OS、機種、ユーザー(ログイン ID)、環境(dev / prod)。個人情報の扱いと同意 |
| 認証と接続先 | API キー / JWT、TLS、接続先の配布(設定 QR)。テレメトリ用の接続先は API と分けられるようにする |
| アプリ側の組み込み口 | `Extender/` のプラグイン(画面遷移の計測)、`ILoggerProvider`(ログ転送)、`CrashReport`(クラッシュ送信)、`DeviceState`(端末状態)、`NetworkOperator`(通信結果) |
| 抽象化 | アプリは送信先に依存しない薄い抽象(例: `ITelemetry`)に依存し、DeviceManager / OTLP / 外部サービス / 無効(Null)を DI で差し替える |
| サーバーの運用 | 自前(DeviceManager 型)か既製の収集基盤(OTel Collector + Grafana 等)か外部サービスか。保持期間、ダッシュボード、アラート |
| ビルド | トリミング / AOT との相性(リフレクションを使うライブラリ)、Debug と Release で送信先や有効無効を切り替える |

## 💡3. 候補

### 🖥️A. DeviceManager 型のサーバー + 端末向けライブラリ

- 既存の `DeviceManager` と `DeviceManager.Client` をそのまま、または本テンプレート向けに整理して使う
- 組み込み先: `IDeviceInfoProvider` = `Settings.UniqueId` + `IDeviceInfo`、`IDeviceStatusProvider` = `DeviceState`(電池 / ネットワーク)、`AddDeviceManagerLogging()` でログ転送、`CrashReport.LogException` から `ReportCrash`、`Extender/` のプラグインで画面遷移をイベントとして送る
- 利点: 端末管理(登録 / 一覧 / ダッシュボード)・ログ・エラーレポートが一体、タンキングと再送が実装済み、設定配信やメッセージ(プッシュ相当)も同じ経路
- 制約: 独自プロトコル(gRPC + SignalR)で他の収集基盤とは繋がらない、gRPC は h2c の別ポート(8083)、認証は共有キー、サーバーの運用は自前

### 📡B. OpenTelemetry(OTLP)で汎用の収集基盤へ送る

- OpenTelemetry .NET(`OpenTelemetry` / `OpenTelemetry.Exporter.OpenTelemetryProtocol`)は MAUI(Android)でも動く。ログは `ILoggingBuilder.AddOpenTelemetry` + OTLP エクスポーター、メトリクスは `Meter` / `MeterProvider`、トレースは `ActivitySource` / `TracerProvider`。.NET 10 の MAUI 自体が `Microsoft.Maui` の `ActivitySource` / `Meter`(レイアウトの measure / arrange 回数と所要時間)を持つ
- OTLP は HTTP/protobuf(`http://…:4318`)が端末向き(h2c のポート分離が不要)。受け口は OTel Collector、Aspire ダッシュボード(開発時)、Grafana(Tempo / Loki / Mimir)、Seq、Azure Monitor(Azure Monitor OpenTelemetry Exporter)など。既存の template-maui-server も OTLP エクスポーターを持つため、同じ収集基盤にサーバーと端末の両方を送れる
- 例外は Logs のレコード(`exception.type` / `exception.message` / `exception.stacktrace` の属性)として送る。クラッシュは未処理例外のフック(`AndroidEnvironment.UnhandledExceptionRaiser`)で Critical ログを記録して `ForceFlush` すれば落ちる前に届く(接続先が落ちていれば下記の退避で次回起動時に届く)
- オフライン: OTLP エクスポーターの実験的機能 `OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY=disk`(+ `..._DISK_RETRY_DIRECTORY_PATH`)で、送信に失敗したバッチをディスクに退避し 60 秒ごとに再送できる(退避されるのは失敗したバッチだけで、キューの未送信分ではない)。端末管理・ダッシュボードは収集基盤側に依存する。トリミング / AOT の警告に注意
- 実証: `Works3/OtelSample`(同フォルダの README)。OTLP/HTTP の自前受信サーバ + MAUI クライアントで、ログ / スパン(HttpClient 計装を含む)/ メトリクス(ランタイム / MAUI レイアウト / 電池)/ クラッシュ / ディスク退避と再送を Pixel 9a で確認済み。組み込み時の注意(`EventSourceSupport`、`AddMetrics`、`AddView` でのタグ集約、`Stop` のブロック)も README に記載
- Aspire との関係: .NET 10 の「MAUI 用 Aspire service defaults」は開発時の仕組み(AppHost がサーバーを起動し、MAUI アプリにサービスディスカバリと OTLP の接続先(Aspire ダッシュボード)を渡す)で、エミュレーター専用ではないが本番の基盤ではない。本番は Collector 等の OTLP 受け口を設定 QR で配る形になるため、Aspire はこの候補の「開発時の受け口」として扱う

### ☁️C. 外部サービス

| サービス | 内容 | 参照 |
| --- | --- | --- |
| Sentry(`Sentry.Maui`) | クラッシュ / エラー / パンくず / 性能。オフラインキャッシュ、リリース・端末情報の自動付与。MAUI の公式 SDK あり | https://docs.sentry.io/platforms/dotnet/guides/maui/ |
| Firebase Crashlytics | Android / iOS のクラッシュ収集。FCM を採用する場合は同じ `google-services.json`。`Plugin.Firebase` または Xamarin バインディング | https://github.com/mattleibow/CloudyCrashReporting(DataDog / Dynatrace / Crashlytics / New Relic / Raygun / Sentry の比較) |
| Application Insights(TinyInsights) | `UseTinyInsights(connectionString)` でクラッシュ / ページビュー / 依存呼び出し / カスタムイベント。専用ダッシュボードあり | https://github.com/dhindrik/TinyInsights.Maui |
| OpenTelemetry → Application Insights | OTel の経路で Azure Monitor へ | https://www.docswell.com/s/tanaka_733/K4P265-maui-observability |

## ⚖️4. 比較

| 観点 | A. DeviceManager 型 | B. OpenTelemetry | C. 外部サービス |
| --- | --- | --- | --- |
| クラッシュ | 対応(即時フラッシュ + タンキング) | ログとして送る(保存 → 次回送信は自前) | 対応(SDK が保存・再送) |
| ログ / メトリクス / トレース | ログ / ステータス(メトリクス)。トレースなし | 3 種すべて(標準の意味論) | サービスによる |
| オフライン耐性 | タンキング(SQLite)+ 再送 | メモリのみ(永続化は自前) | SDK による(Sentry はキャッシュあり) |
| 端末管理 / 設定配信 / メッセージ | あり | なし | なし |
| サーバー | 自前(Blazor 管理画面付き) | Collector + 保存先(既製) | SaaS |
| プロトコル / 認証 | gRPC(h2c)+ SignalR + REST / API キー | OTLP(HTTP または gRPC)/ ヘッダー | HTTPS / DSN・キー |
| 他システムとの接続 | 独自 | 標準(サーバー側と同じ基盤に集約できる) | サービス内 |

## 🧩5. アプリ側の組み込み設計(共通)

- `ITelemetry`(例: `TrackEvent` / `TrackMetric` / `ReportCrash` / `Flush`)を `Components` または `Services` に置き、実装(DeviceManager / OTLP / Null)を `MauiProgram.cs` で切り替える。ログは `ILoggerProvider` として差し込む
- 接続先とキーは `Settings`(QR キー: 例 `TelemetryEndPoint` / `TelemetryKey`)。未設定なら Null 実装で動く
- 収集点: `CrashReport`(保存済みレポートの送信)、`Extender/` のナビゲーションプラグイン(画面遷移)、`DeviceState`(電池 / ネットワーク)、`NetworkOperator`(通信の成否・所要時間・ステータス)、`DiagnosticPanel` の数値(メモリ)
- 送信の制御: バックグラウンド移行(`Window.Stopped`)でフラッシュ、Wi-Fi 限定などの条件は `DeviceState.NetworkProfile` で判定

## ❓6. 検討の論点(次の作業で決めること)

1. 方式: A / B / C のどれか、または A のサーバーに OTLP の受け口を足す・B の永続キューを自前で足すといった組み合わせ
2. 収集する項目と粒度(クラッシュのみから始めるか、ログ / メトリクスまで含めるか)
3. 端末の識別と個人情報の扱い(ID の種類、ログイン ID を付けるか、同意の要否)
4. サーバーの配置と運用(自前 / 既製 / SaaS、保持期間、ダッシュボード、アラート)
5. ライブラリの形(DeviceManager.Client を NuGet 化するか、本テンプレート内の `Components` として持つか)
6. 本テンプレートでの見せ方(Device > Misc に送信テストのカードを置くか、設定画面に接続先を出すか)

## 🔗7. 参考

- OpenTelemetry .NET: https://opentelemetry.io/docs/languages/dotnet/ 、OTLP エクスポーター: https://github.com/open-telemetry/opentelemetry-dotnet/tree/main/src/OpenTelemetry.Exporter.OpenTelemetryProtocol
- .NET 10 MAUI の診断(`Microsoft.Maui` の ActivitySource / Meter): https://learn.microsoft.com/ja-jp/dotnet/maui/whats-new/dotnet-10?view=net-maui-10.0
- MAUI と Aspire: https://learn.microsoft.com/ja-jp/dotnet/maui/data-cloud/aspire-integration?view=net-maui-10.0 、補足: https://egvijayanand.in/2025/10/29/integrating-dotnet-maui-with-aspire-a-comprehensive-guide/
- クラッシュ収集 SDK の比較: https://github.com/mattleibow/CloudyCrashReporting
- TinyInsights: https://github.com/dhindrik/TinyInsights.Maui 、OpenTelemetry → Application Insights: https://www.docswell.com/s/tanaka_733/K4P265-maui-observability
- DeviceManager: `D:\GitHubTemplate\DeviceManager`(`README.md` / `docs/features.md` / `docs/sdk.md`)
- 候補 B の実証サンプル: `Works3/OtelSample`(`README.md`)
