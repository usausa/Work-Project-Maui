# 📈テレメトリ(OpenTelemetry)の実装計画

端末(Template.MobileApp)のログ・トレース・メトリクスを OTLP/gRPC で template-maui-server へ送り、サーバーで保存・表示する。作業は次の順に進め、番号は `Task_Checklist.md` の 1 節と同じ。

| 番号 | 段階 | 内容 |
| --- | --- | --- |
| 1-1 | サーバー | OTLP/gRPC の受信口(受信内容はログに出すだけ)|
| 1-2-1 | 端末 | 収集の移設(`Diagnostics` 名前空間。パネルは表示だけ)|
| 1-2-2 | 端末 | 起動・停止と再起動の制御 |
| 1-2-3 | 端末 | OTEL の送信 |
| 1-3 | サーバー | 保存 |
| 1-4 | サーバー | 一覧と詳細の画面 |

ファイルパスは `Template.MobileApp/` からの相対。(server) は `template-maui-server/src/Template.MobileServer.Web/`、(core) は `template-maui-server/src/Template.MobileServer.Core/` からの相対。検討資料は `Telemetry_Study.md`、実証サンプルは `Works3/OtelSample`(同フォルダの README に完結)。

## 📐方式と版

| 項目 | 採用 |
| --- | --- |
| OTLP | 仕様 1.11.0(トレース / メトリクス / ログは Stable)。proto は opentelemetry-proto v1.11.0 |
| 転送 | gRPC(4317、h2c)。gzip。受信上限はオプションで指定(既定 16 MiB。gRPC の既定は 4 MB。単項の呼び出しは Kestrel の `MaxRequestBodySize`(30 MB)も受けるため 16 MiB まで)。不正な項目は `partial_success` の件数で返し、再送してよい失敗(保存の失敗など)は `UNAVAILABLE` で返す |
| SDK | `OpenTelemetry` / `OpenTelemetry.Exporter.OpenTelemetryProtocol` 1.19.1 |
| セマンティック規約 | 1.44.0 |
| 端末の退避 | エクスポーターのディスク再送(`OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY=disk`、実験的)。送信に失敗したバッチだけを保存し、60 秒ごとに再送する |

## ⚖️決定事項

| 項目 | 決定 |
| --- | --- |
| 受信口の置き場 | (server) Web プロジェクト直下の `Telemetry/`(名前空間 `Template.MobileServer.Web.Telemetry`)。サーバー自身の計測の `Application/Telemetry` とは別 |
| 端末側の置き場 | 収集・送信・制御を `Diagnostics/`(名前空間 `Template.MobileApp.Diagnostics`)に置く。パネル(`Shell/DiagnosticPanel`)は今の場所のまま、値を表示するだけにする |
| 端末の識別 | Resource に `device.id`(Android の `ANDROID_ID`。再インストールで変わらない。規約では業務端末向けの Opt-In)と `app.installation.id`(`Settings.UniqueId`。インストールごと)を載せる。メトリクスの属性には入れない(サーバーは Resource の `device.id` で端末を分ける)。SignalR の端末状態(`DeviceStatusMessage.DeviceId`)も `device.id` に揃える |
| 保存先 | 別ファイル `telemetry.db`(WAL)。業務データの `data.db` とは別の `IDbProvider` を使う |
| 受信口の認証 | なし(他の API と同じ)|
| メトリクス | 現在値(ゲージ)を基本にする。回数・時間の計器(カウンター / ヒストグラム)は Delta で送り、各点がその送信間隔の値になるようにする |
| 保持期間 | ログ・トレース 7 日、メトリクス 30 日(オプションで変更できる)|
| HTTP の計測 | `NetworkUsecase` の操作単位のスパンと所要時間だけ。`System.Net.Http` のスパン・メトリクスは送らない(URL のパスの ID とエクスポーター自身の通信を含まない)|
| パネルの表示 | 今のまま(DEBUG のヘッダーボタンで一時的に表示)|

## 📤送る内容

### Resource(すべてのシグナルに付く)

| 属性 | 値 |
| --- | --- |
| `service.name` / `service.version` | `Template.MobileApp` / アプリの版 |
| `service.instance.id` | プロセスの起動ごとの GUID |
| `device.id` | `ANDROID_ID` |
| `app.installation.id` | `Settings.UniqueId` |
| `device.manufacturer` / `device.model.identifier` | `DeviceInfo.Manufacturer` / `DeviceInfo.Model` |
| `os.type` / `os.name` / `os.version` | `linux` / `Android` / `DeviceInfo.VersionString` |

### メトリクス(送信の間隔は 30 秒)

| 計器 | 種類 / 単位 | 出どころ |
| --- | --- | --- |
| `process.cpu.utilization` | ゲージ / `1` | サンプラー(CPU 時間の差分 ÷ 経過時間 ÷ CPU 数)|
| `process.memory.usage` | 現在値 / `By` | サンプラー(ワーキングセット)|
| `process.thread.count` | 現在値 / `{thread}` | サンプラー |
| `template.ui.frame_rate` | ゲージ / `{frame}/s` | サンプラー(Choreographer のフレーム間隔)|
| `dotnet.gc.collections` / `dotnet.gc.heap.total_allocated` / `dotnet.gc.last_collection.heap.size` / `dotnet.exceptions` | Delta または現在値 | 組み込みの `System.Runtime` Meter(この 4 つ以外は View で落とす)|
| `maui.layout.measure_count` / `maui.layout.arrange_count` | Delta / `{times}` | `Microsoft.Maui` Meter(属性は `element.type` だけに集約)|
| `template.network.operation.duration` | Delta のヒストグラム / `s` | `NetworkUsecase`(属性 = 操作名、結果の種別)|

- サンプラーが止まっているあいだ、ゲージは値を返さない(古い値を送り続けない)
- 独自の計器は `template.` 接頭辞にする(規約の名前空間には置かない)

### トレース

| スパン | 内容 |
| --- | --- |
| 画面遷移 | 遷移先の `app.screen.name`、遷移の種類 |
| `NetworkUsecase` の操作 | 操作名、結果の種別、`http.response.status_code`、`error.type` |

`Microsoft.Maui` のレイアウトのスパンは送らない。

### ログ

| 対象 | 内容 |
| --- | --- |
| アプリのカテゴリ(`Template.MobileApp*`)の Warning 以上 | 本文、属性、例外(`exception.type` / `exception.message` / `exception.stacktrace`)、trace / span id |
| クラッシュ | イベント名 `exception`、重大度 FATAL、`app.crash.id` |
| リーク検知の警告 | Warning(DEBUG のみ)|

OpenTelemetry 自身のカテゴリは送らない(送信のループを防ぐ)。

## 🏗️構成

### 端末

| ファイル | 役割 |
| --- | --- |
| `Diagnostics/DiagnosticsSampler.cs` | 1 秒ごとに CPU / スレッド / メモリ / GC 回数の差分 / 割り当て速度 / FPS / Measure・Arrange の回数を計算し、不変のスナップショット(しきい値の判定 Safe / Warning / Critical を含む)とメモリの推移(60 点)を公開する |
| `Diagnostics/LayoutMetrics.cs` | `Microsoft.Maui` Meter の Measure / Arrange の回数(`Shell/` から移動)|
| `Diagnostics/CrashReport.cs` + `.android.cs` | 未処理の例外の捕捉と保存、送信(`Helpers/` から移動)|
| `Diagnostics/DiagnosticLogProvider.cs` | 診断画面の直近のログ(`Components/` から移動)|
| `Diagnostics/DeviceIdentity.cs` | `device.id` と `app.installation.id` |
| `Diagnostics/TelemetryHost.cs` | Tracer / Meter / Logger プロバイダーの構築と破棄、送信 |
| `Diagnostics/DiagnosticsInstrumentation.cs` | アプリの `ActivitySource` / `Meter` と、スナップショットを読むゲージ |
| `Diagnostics/SdkEventListener.cs` | SDK の自己診断(Warning 以上)をログへ |
| `Diagnostics/DiagnosticsController.cs` | サンプラーとテレメトリの開始・停止・再起動 |
| `Shell/DiagnosticPanel.xaml(.cs)` | スナップショットを表示するだけ(タイマー・`IDisplay` の購読・計算を持たない)|

### サーバー

| ファイル | 役割 |
| --- | --- |
| (server) `Telemetry/Protos/**` | opentelemetry-proto v1.11.0 |
| (server) `Telemetry/OtlpTraceHandler.cs` / `OtlpMetricsHandler.cs` / `OtlpLogsHandler.cs` | gRPC の受信口(4317 に限定)|
| (server) `Telemetry/OtlpHelper.cs` | リソースの属性(`service.name`、`device.id` → 無ければ `app.installation.id`)、ID の 16 進、時刻、計器の点の数と temporality、ログ用の整形 |
| (server) `Telemetry/OtlpMapper.cs` | OTLP → エンティティ |
| (server) `Telemetry/TelemetryReceiverOption.cs` / `Telemetry/Log.cs` | 受信の設定(セクション `TelemetryReceiver`)と LoggerMessage |
| (server) `Telemetry/TelemetryNotifier.cs` | 保存の通知(画面の更新用。間引く)|
| (core) `Models/Entity/Telemetry*Entity.cs` / `Accessors/TelemetryAccessor.cs` + `Accessors/Sql/TelemetryAccessor.*.sql` / `Services/TelemetryService.cs` | 保存と照会 |
| (server) `Workers/TelemetryRetentionWorker.cs` + `TelemetryRetentionOption.cs` | 保持期間を過ぎた行の削除 |
| (server) `Components/Pages/Telemetry*Page.razor(.cs)` / `Components/Common/*` | 画面と SVG 部品 |

## 🔁端末の起動・停止

| 入力 | 出どころ |
| --- | --- |
| パネルの表示 | `MainPageViewModel`(ヘッダーボタン)|
| テレメトリの有効 / 無効 | `Settings.TelemetryEnabled`(新規、既定 false。設定画面で切り替え)|
| 送信先 | `Settings.OtelEndPoint`(QR で投入)|
| 前面 / 背面 | `Window.Resumed` / `Stopped` |

| 状態 | 条件 |
| --- | --- |
| サンプラーが動く | 前面、かつ(パネルの表示中 または テレメトリが動作中)|
| テレメトリが動作中 | 有効、かつ送信先が設定済み(`IsOtelConfigured`)|

- 入力が変わるたびに望ましい状態を求め、差分だけを実行する(開始 / 停止 / 送信先が変わったときの作り直し)。実行はバックグラウンドで 1 つずつ(プロバイダーの破棄は最大 5 秒かかるので UI スレッドで待たない)
- 背面に移るときは完了を待たずに `ForceFlush` し、サンプラーを止める。前面に戻ったらサンプラーの基準値を取り直す
- Settings はサービスへ注入しない。画面と起動処理がコントローラーへ値を渡す(`ApiContext` と同じ)
- クラッシュ: テレメトリが動作中なら FATAL のログを出して短い時間だけ `ForceFlush` する(UI を止めない)。送れなかった分は保存したクラッシュを次回の起動時に送り、送信済みにする

## 🗄️サーバーのデータ(`telemetry.db`)

| テーブル | 主な列 | 索引・制約 |
| --- | --- | --- |
| `TelemetryDevice` | DeviceId(`device.id`。無ければ `app.installation.id`)、InstallationId、ServiceName、ServiceVersion、Manufacturer、Model、OsName、OsVersion、ResourceJson、FirstSeenAt、LastSeenAt | 主キー DeviceId |
| `TelemetryLog` | DeviceId、ServiceInstanceId、ScopeName、TimeUnixNano、ObservedTimeUnixNano、SeverityNumber、SeverityText、EventName、Body、TraceId、SpanId、AttributesJson、ReceivedAt | (DeviceId, TimeUnixNano)、(TraceId)、(SeverityNumber, TimeUnixNano) |
| `TelemetrySpan` | DeviceId、TraceId、SpanId、ParentSpanId、Name、Kind、StartTimeUnixNano、EndTimeUnixNano、StatusCode、StatusMessage、ScopeName、AttributesJson、EventsJson、LinksJson、ReceivedAt | 一意 (TraceId, SpanId)、(DeviceId, StartTimeUnixNano)、(TraceId) |
| `TelemetryMetricPoint` | DeviceId、MetricName、Unit、Type、Temporality、IsMonotonic、StartTimeUnixNano、TimeUnixNano、Value、Count、Sum、Min、Max、BucketsJson、AttributesJson、SeriesKey(属性を正規化した文字列)、ReceivedAt | 一意 (DeviceId, MetricName, SeriesKey, TimeUnixNano)、(DeviceId, MetricName, TimeUnixNano) |

- イベントの時刻は UTC の Unix ナノ秒(OTLP のまま)、受信時刻はサービスコンテキストの時刻
- 1 回の `Export` を 1 トランザクションで保存する。一意制約で再送の重複を捨てる
- AnyValue(配列・キー値リスト・バイト列を含む)は JSON、ID は 16 進。ヒストグラムのバケット、指数ヒストグラム、サマリーの分位は BucketsJson
- 削除は `TelemetryRetentionWorker` が 1 時間ごとに少しずつ行う

## 🖥️画面

| 画面 | 内容 |
| --- | --- |
| `/telemetry` | 概要(稼働端末数、エラー・クラッシュの件数、受信件数の推移)、端末一覧(端末 / 機種 / OS / アプリの版 / 最終受信 / エラー数。`MudDataGrid` のサーバーページング)、直近のエラー |
| `/telemetry/devices/{id}` | 端末の Resource と、ログ(重大度・本文・期間・trace id で絞り込み)/ トレース(一覧)/ メトリクス(計器の一覧 → 時系列グラフ)のタブ。ログの詳細はダイアログ(属性、例外のスタックトレース)|
| `/telemetry/traces/{traceId}` | スパンのガント、スパンの属性とイベント、同じトレースのログ |

- グラフとガントは OtelSample の SVG 部品(`SpanGantt` / `TimeSeriesChart` / `Sparkline` / `AttributeList`)を Smart.Blazor(`Condition` / `ListItem`)と CSP の範囲で作り直す
- 保存の通知で画面を更新する(間引く)。メニューに追加する

## ♻️既存資産の扱い

| 資産 | 使うもの | 直すもの |
| --- | --- | --- |
| OtelSample のクライアント | `TelemetryHost`(プロバイダーの手動構築、作り直せるログ転送、gRPC 用の `SocketsHttpHandler`、`AddView`、ディスク再送、クラッシュのフック)、`SdkEventListener`、csproj の `EventSourceSupport=true`、`AddMetrics()` | エクスポーター自身の送信がトレース・メトリクスに混ざる、MAUI のレイアウトのスパンでトレースが埋まる、Resource の属性名(`device.model` → `device.manufacturer` / `device.model.identifier`、`os.type` の追加)、独自の計器の名前と単位、クラッシュの Flush が UI スレッドを待つ |
| OtelSample のサーバー | proto と csproj の `<Protobuf>` 定義、`OtlpGrpcServices`、`TelemetryMapper`、SVG 部品、`DummyDataGenerator` | メトリクスの系列に端末が入っていない、累積値をそのまま保存する、スパンのイベント・リンク・バケット・temporality を捨てる、重複を排除しない、`partial_success` を返さない、DB の失敗が再送されない、単一のロックでの同期 I/O |
| DeviceManager | 退避と再送の考え方(送る前に保存、成功した分だけ削除、上限付きのバックオフ)、保持期間の掃除 | 通信は独自の gRPC 契約なので使わない |

## 🪜段階ごとの作業

### 📡1-1 サーバー: OTLP/gRPC の受信口

| ファイル | 変更 |
| --- | --- |
| (server) `Telemetry/Protos/opentelemetry/proto/**` | 新規。common / resource / logs / trace / metrics と collector の `*_service.proto`(生成型の名前空間は上流の `OpenTelemetry.Proto.*` のまま)|
| (server) `Template.MobileServer.Web.csproj` | `<Protobuf Include="Telemetry\Protos\**\*.proto" ProtoRoot="Telemetry\Protos" GrpcServices="None" />` と、collector の proto だけ `<Protobuf Update="...\collector\**\*.proto" GrpcServices="Server" />`(サービスの無い proto に `Server` を付けると空の `*Grpc.cs` が生成され SA1518 が出る。既存の `Handlers\Protos` の定義と重ねない)|
| (server) `Telemetry/Otlp*Handler.cs` / `OtlpHelper.cs` / `TelemetryReceiverOption.cs` / `Log.cs` | 新規。`Export` を受け、リソースごとに `service.name`・端末・件数を Information、リソースの属性と 1 件ごとの内容を Debug で出して空の応答を返す |
| (server) `Application/ApplicationExtensions.cs` / `appsettings.json` | 受信口の登録(受信上限を `AddServiceOptions<Otlp*Handler>` で指定)と `MapGrpcService<Otlp*Handler>().RequirePort(Otel のポート)`。ASP.NET Core の計装から `/opentelemetry.proto.collector.` を除外する。`TelemetryReceiver` セクション |
| (server) `Components/Pages/QrPage.razor.cs` / `README.md` / `tests/.../QrPageTests.cs` | `OtelEndPoint` を 4317 に(`MakeGrpcEndPoint(BaseUri, Kestrel:Endpoints:Otel:Url)`)|
| (server) テスト | `Telemetry/OtlpHandlerTests.cs`(3 シグナルの件数と空の応答)/ `OtlpHelperTests.cs`(端末の取り出し、属性・点の整形、期間)|

確認: OtelSample のクライアント(gRPC)を 4317 へ向け、サーバーのログに受信が出る。

完了(2026-09-23)。結果は `Change_Summary.md` の区間 17。

### 📊1-2-1 端末: 収集の移設

| ファイル | 変更 |
| --- | --- |
| `Diagnostics/DiagnosticsSampler.cs` | 新規(「構成」のとおり)。`Process.Refresh()` を毎回呼び、開始時と前面への復帰時に GC 回数・FPS などの基準値を取り直す |
| `Shell/LayoutMetrics.cs` / `Helpers/CrashReport.cs(.android.cs)` / `Components/DiagnosticLogProvider.cs` → `Diagnostics/` | 移動。`CrashReport` は `AppDomain.UnhandledException` も捕捉し、アプリと端末の情報を付けて保存、送信済みの印を持つ |
| `Diagnostics/DeviceIdentity.cs` | 新規。`device.id`(`ANDROID_ID`)と `app.installation.id` |
| `Shell/DiagnosticPanel.xaml(.cs)` | スナップショットを表示するだけにする |
| `Modules/Main/DiagnosticsViewModel.cs` | `LoadRuntime` の同じ計算をスナップショットに置き換える |
| `Modules/Network/NetworkRealtimeViewModel.cs` | SignalR の端末状態の `DeviceId` を `device.id` に |

確認: パネルの各値(メモリとスレッド数が更新されること、表示を始めた直後の GC 回数・FPS)、診断画面、Realtime の端末一覧。

### 🔁1-2-2 端末: 起動・停止と再起動

| ファイル | 変更 |
| --- | --- |
| `Diagnostics/DiagnosticsController.cs` | 新規(「端末の起動・停止」のとおり)|
| `State/Settings.cs` | `TelemetryEnabled` を追加 |
| `Modules/Main/SettingView.xaml` + `SettingViewModel.cs` | テレメトリの有効 / 無効の切り替え。QR で `OtelEndPoint` などが変わったらコントローラーへ渡す |
| `MainPageViewModel.cs` / `App.xaml.cs` / `MauiProgram.cs` | パネルの表示・非表示、起動時の設定値、前面・背面をコントローラーへ渡す |

確認: パネルの表示・非表示とテレメトリの有効・無効の組み合わせでのサンプラーの動作、背面での停止と前面での再開。

### 📤1-2-3 端末: OTEL の送信

| ファイル | 変更 |
| --- | --- |
| `Template.MobileApp.csproj` | `OpenTelemetry` / `OpenTelemetry.Exporter.OpenTelemetryProtocol` 1.19.1 を追加。`MetricsSupport` を全構成で true、`EventSourceSupport=true` |
| `MauiProgram.cs` | `AddMetrics()` を DEBUG の外へ。ログ転送の `ILoggerProvider` を登録 |
| `Diagnostics/TelemetryHost.cs` / `DiagnosticsInstrumentation.cs` / `SdkEventListener.cs` | 新規(「送る内容」のとおり)|
| `Extender/`(ナビゲーションのプラグイン)| 画面遷移のスパン |
| `Usecase/NetworkUsecase.cs` | `ExecuteCoreAsync` に操作単位のスパンと所要時間 |
| `Diagnostics/CrashReport.*` | クラッシュの送信と、次回の起動時の送信 |

確認: 有効 / 無効の切り替え、送信先の変更での作り直し、サーバー停止中の退避と再送、クラッシュの送信(次回の起動時を含む)、背面での Flush。

### 🗄️1-3 サーバー: 保存

| ファイル | 変更 |
| --- | --- |
| (server) `Assets/Data/TelemetrySchema.sql` | 新規。「サーバーのデータ」のテーブルと索引 |
| (server) `Application/ApplicationExtensions.cs` / `appsettings.json` | `telemetry.db` の接続(WAL)と `IDbProvider`、起動時のスキーマ適用 |
| (core) `Models/Entity/Telemetry*Entity.cs` / `Accessors/TelemetryAccessor.cs` + `Sql/*.sql` / `Services/TelemetryService.cs` | 新規。保存(1 トランザクション)、照会、期間での削除 |
| (server) `Telemetry/OtlpMapper.cs` | 新規 |
| (server) `Telemetry/Otlp*Handler.cs` | ログ出力から保存へ。不正な項目は `partial_success`、保存の失敗は `UNAVAILABLE` |
| (server) `Workers/TelemetryRetentionWorker.cs` + `TelemetryRetentionOption.cs` | 新規 |
| (server) テスト | Mapper の単体テスト、インメモリ SQLite での保存・照会・削除 |

確認: ダミーデータの投入と端末からの実データ、再送の重複が入らないこと、保持期間での削除。

### 🖥️1-4 サーバー: 一覧と詳細の画面

| ファイル | 変更 |
| --- | --- |
| (server) `Components/Pages/TelemetryPage.razor(.cs)` / `TelemetryDevicePage.razor(.cs)` / `TelemetryTracePage.razor(.cs)` | 新規(「画面」のとおり)|
| (server) `Components/Common/*` | SVG 部品 |
| (server) `Telemetry/TelemetryNotifier.cs` | 新規 |
| (server) `Components/Layout/NavMenu.razor` + `NavMenuTests` | メニューに追加(テストの件数 6 → 7)|

確認: bUnit のテストと実データでの各画面。

## 🔍着手前に確かめること

| 項目 | 内容 | 段階 |
| --- | --- | --- |
| Smart.Data.Accessor の名前付きプロバイダー | `[Provider]` で `telemetry.db` 用の `IDbProvider` を選べるか。選べなければ、アクセサーに渡す `IDbProvider` を DI で分ける | 1-3 |
| `ANDROID_ID` | 同じ署名のアプリなら再インストールで変わらないこと、Debug と Release(署名が違う)で値が変わること | 1-2-1 |
| gRPC(h2c)の送信 | Android 既定のハンドラーは HTTP/2 を使えないため `SocketsHttpHandler` を渡す(OtelSample で確認済み)| 1-2-3 |
| `MetricsSupport` を Release でも有効にしたときの影響 | アプリのサイズと起動時間 | 1-2-3 |

## 📝記録

各段階の完了時に `Change_Summary.md`、`Task_Checklist.md`(完了した番号を削除)、サーバーの `README.md`(4317 と Telemetry)、`Telemetry_Study.md`(方式の決定、`NetworkOperator` などの古い記述)を更新する。
