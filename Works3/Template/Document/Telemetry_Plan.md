# 📈テレメトリ(OpenTelemetry)の実装計画

端末(Template.MobileApp)のログ・トレース・メトリクスを OTLP/HTTP(protobuf)で template-maui-server へ送り、サーバーで保存・表示する。作業は次の順に進め、番号は `Task_Checklist.md` の 1 節と同じ。

| 番号 | 段階 | 内容 |
| --- | --- | --- |
| 1-1 | サーバー | OTLP/gRPC の受信口(受信内容はログに出すだけ)|
| 1-2-1 | 端末 | 収集の移設(`Diagnostics` 名前空間。パネルは表示だけ)|
| 1-2-2 | 端末 | 起動・停止と再起動の制御 |
| 1-2-3 | 端末 | OTEL の送信(サーバーに OTLP/HTTP の受信口を足す)|
| 1-3 | サーバー | 保存 |
| 1-4 | サーバー | 一覧と詳細の画面 |

ファイルパスは `Template.MobileApp/` からの相対。(server) は `template-maui-server/src/Template.MobileServer.Web/`、(core) は `template-maui-server/src/Template.MobileServer.Core/` からの相対。検討資料は `Telemetry_Study.md`、実証サンプルは `Works3/OtelSample`(同フォルダの README に完結)。

## 📐方式と版

| 項目 | 採用 |
| --- | --- |
| OTLP | 仕様 1.11.0(トレース / メトリクス / ログは Stable)。proto は opentelemetry-proto v1.11.0 |
| 転送 | 端末は HTTP/protobuf(4318。送信先にシグナルごとの `v1/traces` / `v1/metrics` / `v1/logs` を付ける)。サーバーは HTTP(4318)と gRPC(4317、h2c)の両方で受ける。gzip。受信上限はオプションで指定(既定 16 MiB。HTTP は展開後の本文に適用。gRPC の既定は 4 MB。gRPC の単項の呼び出しは Kestrel の `MaxRequestBodySize`(30 MB)も受けるため 16 MiB まで)。不正な項目は `partial_success` の件数で返し、再送してよい失敗(保存の失敗など)は HTTP = 503、gRPC = `UNAVAILABLE` で返す |
| SDK | `OpenTelemetry` / `OpenTelemetry.Exporter.OpenTelemetryProtocol` 1.19.1 |
| セマンティック規約 | 1.44.0 |
| 送信の失敗 | ディスクには退避しない(送れなかった分は欠けてよい)。送れなかった送信の中身はメモリのリングバッファー(全種類で共有、60 件。メトリクスだけなら 30 分で埋まる)に取っておき、どれかの送信が成功したときに古い順に 5 件まで送り直す(送り直しは同時に 1 つだけ。上限を超えた古い分とプロセスの終了で失われる)。メトリクスの差分は集計(30 秒)ごとに基準が進むので、送れなかった区間が次の送信に足されることはない。送信の失敗は、失敗し始めたときと復旧したときだけログに出す |

## ⚖️決定事項

| 項目 | 決定 |
| --- | --- |
| 受信口の置き場 | (server) Web プロジェクト直下の `Telemetry/`(名前空間 `Template.MobileServer.Web.Telemetry`)。サーバー自身の計測の `Application/Telemetry` とは別 |
| 端末側の置き場 | 送信・計器・制御を `Diagnostics/`(名前空間 `Template.MobileApp.Diagnostics`)に置く(ほかはクラッシュレポートと直近のログだけ)。画面は `TelemetryService` を役割ごとの interface(`ITelemetryControl` / `ITelemetryStatus`)で使う。診断パネル(`Shell/DiagnosticPanel`)とスナップショット(`Shell/DiagnosticSampler`)は `Shell` で管理する(サンプラーの開始・停止は `MainPageViewModel`)。端末とプロセスの情報の取得(`DeviceInformation`)は、画面・テレメトリ・SignalR の端末状態で使うので `Components/` に置く(`DeviceInformation_Plan.md`) |
| 端末の識別 | Resource に `device.id`(Android の `ANDROID_ID`。再インストールで変わらない。規約では業務端末向けの Opt-In)と `app.installation.id`(`Settings.UniqueId`。インストールごと)を載せる。メトリクスの属性には入れない(サーバーは Resource の `device.id` で端末を分ける)。SignalR の端末状態(`DeviceStatusMessage.DeviceId`)も `device.id` に揃える |
| 保存先 | 別ファイル `telemetry.db`(WAL)。業務データの `data.db` とは別の `IDbProvider` を使う |
| 受信口の認証 | なし(他の API と同じ)|
| メトリクス | 現在値(ゲージ)を基本にする。回数・時間の計器(カウンター / ヒストグラム)は Delta で送り、各点がその送信間隔の値になるようにする |
| 保持期間 | ログ・トレース 7 日、メトリクス 30 日(オプションで変更できる)|
| HTTP の計測 | 組み込みの `System.Net.Http` の `http.client.request.duration` だけ(要求 1 回ごとなのでリトライは別の点になる。属性に URL のパスを含まない)。エクスポーター自身の送信は含めない。HTTP のスパンは送らない(URL のパスに ID を含むため)|
| パネルの表示 | DEBUG 前提(ヘッダーボタンで一時的に表示)。パネル用の値は取得が重くてもよいが、通常の送信には取得の軽い値だけを使う(「取得のコスト」)|
| 計測の分担 | パネル(`Shell/DiagnosticSampler`)と送信(`DiagnosticsInstrumentation`)は別にする。両方で使う取得の軽い値は `DeviceInformation` から読む(読むたびに取得する。送信は同じ集計の計器で 1 回の読み取りを共有する 500 ms のキャッシュを持つ)。割合や差分は使う側が自分の前回の値から求める |
| MAUI のレイアウトの計測 | Release では MAUI の `IDiagnosticsManager` の登録を外す(`MauiProgram.ConfigureDiagnostics()`)。`MetricsSupport=true` だと購読がなくても Measure / Arrange のたびにタグを作るため。公開の設定が無いので内部の型名で探す(型名が変わると外れず、外さないときと同じ動作になる)。DEBUG はパネルの回数に使うので残す |

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
| `process.cpu.utilization` | ゲージ / `1` | 前回の計測からの CPU 時間(`Environment.CpuUsage`)÷ 経過時間 ÷ CPU 数(送信の間隔の平均。規約の定義どおり)|
| `process.memory.usage` | 現在値 / `By` | ワーキングセット(`/proc/self/stat` の常駐ページ数)|
| `process.thread.count` | 現在値 / `{thread}` | スレッド数(`/proc/self/stat`)|
| `application.gc.last_collection.heap.size` | 現在値 / `By` | 直前の GC 時点のマネージドヒープ(`GC.GetGCMemoryInfo`。組み込みの `dotnet.gc.last_collection.heap.size` は Mono で常に 0)|
| `hw.battery.charge` | ゲージ / `1` | 電池残量(0〜1。規約の hardware の計器)。属性 `hw.id` = `battery`(規約で必須)。`ACTION_BATTERY_CHANGED` の受信で保持した値(`DeviceInformation`)|
| `application.wifi.signal_strength` | ゲージ / `dBm` | 接続中の無線 LAN の信号強度(`DeviceInformation` が `NetworkCallback` の `WifiInfo` から保持)。接続していなければ送らない |
| `dotnet.gc.collections` / `dotnet.gc.heap.total_allocated` / `dotnet.exceptions` | Delta | 組み込みの `System.Runtime` Meter(この 3 つ以外は View で落とす)。`dotnet.exceptions` には送信の失敗でエクスポーターの中で出た例外も入る |
| `http.client.request.duration` | Delta のヒストグラム / `s` | 組み込みの `System.Net.Http` Meter(要求 1 回ごと。属性は `http.request.method` / `http.response.status_code` / `error.type` / `server.address` / `server.port` / `url.scheme` / `network.protocol.version`。ほかの計器は View で落とす)|

- CPU・メモリ・スレッド・ヒープは送信のたびに `DeviceInformation` から読む(同じ送信の 4 計器で読み取りは 1 回。ヒープは GC 回数が変わったときだけ読む)。中断中(背面)でも送る
- FPS と Measure / Arrange の回数は常時の監視・購読が要るので送らない(パネル専用)
- 独自の計器と属性は `application.` 接頭辞にする(アプリの値とわかる名前。規約の名前空間 `app.` には置かない。サーバーの独自の計器 `application.uptime` と同じ)

### トレース

| スパン | 内容 |
| --- | --- |
| 画面遷移(`Navigate`)| 遷移先の `app.screen.name`、遷移元の `application.navigation.from`、遷移の種類の `application.navigation.attribute`。遷移の開始から表示まで |

`Microsoft.Maui` のレイアウトのスパンと、HTTP(`System.Net.Http`)のスパンは送らない。

### ログ

| 対象 | 内容 |
| --- | --- |
| アプリのカテゴリ(`Template.MobileApp*`)の Warning 以上 | 本文、属性、例外(`exception.type` / `exception.message` / `exception.stacktrace`)、trace / span id |
| クラッシュ | イベント名 `exception`、重大度 FATAL、`app.crash.id`、`exception.*`(本文にクラッシュの時刻)|
| リーク検知の警告 | Warning(DEBUG のみ)|

OpenTelemetry 自身のカテゴリは送らない(送信のループを防ぐ)。

## 📏取得のコスト

Pixel 9a(Debug、Mono、スレッド 51)で 1 回の取得を測った値。パネルは DEBUG だけで動かし、通常の送信には取得の軽い値だけを使う。

| 値 | 取得方法 | 1 回の時間 | 割り当て | パネル(DEBUG、1 秒ごと)| 送信(30 秒ごと)|
| --- | --- | --- | --- | --- | --- |
| CPU 使用率 | 共通: `Environment.CpuUsage` の差分 ÷ 経過時間 ÷ CPU 数 | 1.4 µs | 0 | ○(直近 1 秒)| ○(送信の間隔の平均)|
| メモリ(常駐)| 共通: `/proc/self/stat` を開いたまま先頭から読み直す | 4.8 µs | 0 | ○(推移も)| ○ |
| スレッド数 | 共通: メモリと同じ読み取り | (メモリに含む)| (メモリに含む)| ○ | ○ |
| GC 回数(世代別)| 共通: `GC.CollectionCount` ×3 | 0.2 µs | 0 | ○(1 秒ごとの差分)| ○(`System.Runtime`)|
| 割り当て量 | 共通: `GC.GetTotalAllocatedBytes` | 0.08 µs | 0 | ○(1 秒あたり)| ○(`System.Runtime`)|
| 直前の GC 時点のヒープ | 共通: `GC.GetGCMemoryInfo`(GC 回数が変わったときだけ)| 0.3 µs | 288 B | − | ○ |
| 現在のマネージドメモリ | `GC.GetTotalMemory(false)`(Mono はヒープを数える)| 101 µs | 0 | ○ | − |
| FPS | Choreographer で毎フレーム監視(`IDisplay`)| UI スレッドで約 47 ms/s | − | ○ | − |
| Measure / Arrange の回数 | `Microsoft.Maui` の Meter を購読 | レイアウトのたび | − | ○ | − |
| 例外数 | 例外が起きるたびに加算(`System.Runtime`)| 例外のときだけ | − | − | ○ |
| HTTP の所要時間 | 要求ごとに記録(`System.Net.Http`)| 要求のときだけ | − | − | ○ |
| 電池残量 | 共通: `ACTION_BATTERY_CHANGED` の受信で保持した値(受信 1 回 0.08〜0.43 ms、UI スレッド。登録は起動時に 1 回 2.5 ms)| 保持した値を読むだけ | 0 | − | ○ |

- 共通 = `DeviceInformation`(`ReadProcessStatistics()`、ヒープは `ReadHeapSize()`、電池は通知で保持した値)。1 回の読み取りは約 7 µs で割り当てなし(GC が起きた後だけ `GCMemoryInfo` の 288 B)。パネルの 1 秒ごとの計測は約 110 µs(FPS とレイアウトを除く)。送信は 4 つのゲージが同じ送信の中で続けて読むので、読み取りは 1 回
- `Process` の `WorkingSet64` / `Threads` は読むたびに全スレッドの `/proc/self/task/*/stat` を読み直すため 1.4〜1.6 ms・61〜65 KB かかる。使うのは生成時の起動時刻(`StartTime`)の 1 回だけ
- 計器の観測は値を返すだけにする(`Func<T>`。観測 1 回ごとに .NET が配列と列挙子を作る = 80 B)。返した結果を .NET が観測の中で使い終えることや、同じ計器を同時に観測しないことは約束されていないので、結果の入れ物は使い回さない
- 電池残量: `IBattery.ChargeLevel` と `BatteryManager.GetIntProperty(Capacity)` は読むたびにシステムへ問い合わせる(0.4〜2.1 ms、集計の最初の 1 回は 6.8〜10.7 ms)。MAUI の `IBattery.BatteryInfoChanged` も、受信のたびに残量・状態・電源の 3 つを問い合わせ直す。`ACTION_BATTERY_CHANGED` は残量を Intent に入れて届けるので、受信した Intent から読んで保持する(USB 充電中の 2 分半で受信 2 回)
- MAUI のレイアウトの計測(`MetricsSupport=true` で有効になる)による Label の Measure / Arrange 1 回: 計測なし 10 / 21 µs・32 / 80 B、購読なし 13 / 25 µs・432 / 480 B、SDK の集計とサンプラーが購読したとき 50 / 67 µs

## 🏗️構成

### 端末

| ファイル | 役割 |
| --- | --- |
| `Shell/DiagnosticSampler.cs` | パネル用(DEBUG 前提。パネルを表示していて前面にあるあいだだけ `MainPageViewModel` が動かす)。1 秒ごとに CPU / スレッド / メモリ / GC 回数の差分 / 割り当て速度 / 電池残量 / 無線 LAN の信号強度を `DeviceInformation` の値から求め、FPS(MauiComponents の `IDisplay` のフレーム)/ Measure・Arrange の回数(`Microsoft.Maui` の Meter を購読。`ExcludeLayout(Element)` で登録した要素と子孫は数えない)/ 現在のマネージドメモリを自身で計測して、スナップショット(しきい値の判定 Safe / Warning / Critical を含む)とメモリの推移(60 点)を更新する。スナップショットは 1 つを使い回し、推移は `Helpers/RingBuffer` に入れる |
| `Helpers/RingBuffer.cs` | 固定長のリングバッファー(満杯なら最も古い値を上書き。添字は古い順)。メモリの推移に使う |
| `Diagnostics/TelemetrySendHandler.cs` | エクスポーターの `HttpClient` に挟む中継(3 種類で 1 つを共有。クラッシュ用は別で、取っておかずに数えるだけ)。成功した送信を数え、送れなかった中身を `RingBuffer` に取っておいて、どれかの送信が成功したときに古い順に送り直す。成否を `TelemetryService` に知らせる(ログは状態が変わったときだけ)。再送を待っている件数を返す |
| `Diagnostics/CrashReport.cs` + `.android.cs` | 未処理の例外の捕捉と保存、次の起動でのダイアログ(`Helpers/` から移動)。送信の状態は持たない |
| `Diagnostics/DiagnosticLogProvider.cs` | 診断画面の直近のログ(`Components/` から移動)|
| `Components/DeviceInformation.cs` + `.android.cs` | 端末とプロセスの情報の取得(`DeviceInformation_Plan.md`)。識別(`ANDROID_ID`。テレメトリの `device.id` と SignalR の端末状態)、電池・通信・無線 LAN(通知で保持し、変化をイベントで知らせる)、プロセスの値(CPU 時間、スレッド数と常駐メモリ(`/proc/self/stat`)、GC 回数、割り当て量、直前の GC 時点のヒープ)。アプリの情報は持たない |
| `Diagnostics/TelemetryService.cs` + `.android.cs` | 送信の開始・停止の実体(1 つ)。使う側は役割ごとの interface で受け取り、DI では 2 つとも同じインスタンスに解決する: `ITelemetryControl`(中断・再開、送信先、Flush)/ `ITelemetryStatus`(送信中か、送信先、直近の送信の結果、再送を待っている件数、未送信のクラッシュ)。Tracer / Meter / Logger プロバイダーの構築と破棄(バックグラウンドで 1 つずつ)、クラッシュの送信(専用のエクスポーターで同期で送り、届いたかを判定する)。エクスポーターの `HttpClient` は `AndroidMessageHandler` を直接使う(`HttpClientHandler` と違い HTTP の計測を挟まないので、エクスポーター自身の送信が HTTP のメトリクスに入らない)|
| `Diagnostics/DiagnosticsInstrumentation.cs` | アプリの `ActivitySource` / `Meter` と、`DeviceInformation` の値を送るゲージ(サンプラーは使わない) |
| `Diagnostics/SdkEventListener.cs` | SDK の自己診断(Warning 以上)をログへ |
| `Diagnostics/TelemetryLoggerProvider.cs` | アプリのログを `TelemetryService` へ渡すだけの `ILoggerProvider`(送るかの判断と送信は `TelemetryService`)。`ILoggerFactory` を作るときに要るため依存を持たず、`TelemetryService` が生成時に自身を登録する(`TelemetryService` は `ILoggerFactory` を使うので、コンストラクターで受け取ると DI が循環する)|
| `Shell/DiagnosticPanel.xaml(.cs)` | スナップショットを表示するだけ(タイマー・`IDisplay` の購読・計算を持たない)。サンプラーは使う側の ViewModel から `Sampler` プロパティで受け取り、自身のレイアウトは受け取ったサンプラーの `ExcludeLayout(this)` で計測値から除く(見分け方はサンプラーが持つ)|
| `Extender/NavigationTelemetryPlugin.cs` | 画面遷移のスパン |
| `Modules/Network/NetworkTelemetryView.xaml(.cs)` + `NetworkTelemetryViewModel.cs` | 送信のデモ(Network メニューの Telemetry)。警告 / エラー / スパン / 通信 / Flush / クラッシュの送信 |
| `Modules/Main/DiagnosticsView.xaml` + `DiagnosticsViewModel.cs` | 端末とアプリの識別(`device.id` / `app.installation.id`)、起動時刻(`DeviceInformation.StartTime`)、Telemetry のカード(送信中か、直近の送信の結果、再送を待っている件数、未送信のクラッシュ。稼働時間と一緒に、表示中は 1 秒ごとに読み直す)|

### サーバー

| ファイル | 役割 |
| --- | --- |
| (server) `Telemetry/Protos/*.proto` | opentelemetry-proto v1.11.0(直下に置き、`import` はファイル名だけ)|
| (server) `Telemetry/OtlpReceiver.cs` | 受信の処理(HTTP と gRPC で共通)|
| (server) `Telemetry/OtlpHttpEndpoints.cs` | HTTP の受信口(`POST /v1/traces` / `/v1/metrics` / `/v1/logs`、4318 に限定。protobuf だけ、gzip を展開)|
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
| インストールの識別子 | `Settings.UniqueId`(起動処理が `ITelemetryControl.InstallationId` へ渡す。送信の開始時に Resource へ入る)|
| 中断 / 再開 | `MainPageViewModel` の `IAppLifecycle` → `ITelemetryControl.Suspend`(既定は中断。`OnCreated` / `OnResumed` = 再開、`OnStopped` / `OnDestroying` = 中断。背面と破棄ではサンプラーも止める)|

| 状態 | 条件 |
| --- | --- |
| サンプラーが動く | パネルの表示中、かつ前面(`MainPageViewModel` が開始・停止する。送信はサンプラーを使わない)|
| テレメトリが動作中 | 有効、かつ送信先が設定済み(`Settings.GetTelemetryEndPoint()` が null でない)|

- 入力が変わるたびに望ましい状態を求め、差分だけを実行する(開始 / 停止 / 送信先が変わったときの作り直し)。実行はバックグラウンドで 1 つずつ(プロバイダーの破棄は最大 5 秒かかるので UI スレッドで待たない)
- 中断するときは完了を待たずに `ForceFlush` する。サンプラーは開始のたびに基準値を取り直す(背面から戻ったときも)
- Settings はサービスへ注入しない。画面と起動処理が `ITelemetryControl` へ値を渡す(`ApiContext` と同じ)
- クラッシュ: テレメトリが動作中なら、クラッシュのログ(FATAL)を専用のエクスポーター(`SimpleLogRecordExportProcessor`)で同期で送り(最大 2 秒)、届いたとき(HTTP の成功)だけ、送れたクラッシュの ID をテレメトリ側のファイル(アプリのデータ領域の `telemetry-crash-sent.txt`。キャッシュは消されることがあるので使わない)に記録する。プロセスが終了する(`CrashEventArgs.IsTerminating`)ときは、溜まっているログも送るためにログのプロバイダーを `Shutdown` する(最大 1 秒。`ForceFlush` はバッチを取り出した時点で戻ることがある)。`crash.json` の最後のクラッシュが記録した ID と違えば、次にテレメトリを開始したときに送る。`CrashInfo` は送信の状態を持たない(持つのは表示済みの `Shown` だけ)

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
| OtelSample のクライアント | `TelemetryHost`(プロバイダーの手動構築、作り直せるログ転送、`AddView`、ディスク再送、クラッシュのフック)、`SdkEventListener`、csproj の `EventSourceSupport=true`、`AddMetrics()` | エクスポーター自身の送信がトレース・メトリクスに混ざる、MAUI のレイアウトのスパンでトレースが埋まる、Resource の属性名(`device.model` → `device.manufacturer` / `device.model.identifier`、`os.type` の追加)、独自の計器の名前と単位、クラッシュの Flush が UI スレッドを待つ |
| OtelSample のサーバー | proto と csproj の `<Protobuf>` 定義、`OtlpGrpcServices`、`TelemetryMapper`、SVG 部品、`DummyDataGenerator` | メトリクスの系列に端末が入っていない、累積値をそのまま保存する、スパンのイベント・リンク・バケット・temporality を捨てる、重複を排除しない、`partial_success` を返さない、DB の失敗が再送されない、単一のロックでの同期 I/O |
| DeviceManager | 退避と再送の考え方(送る前に保存、成功した分だけ削除、上限付きのバックオフ)、保持期間の掃除 | 通信は独自の gRPC 契約なので使わない |

## 🪜段階ごとの作業

### 📡1-1 サーバー: OTLP/gRPC の受信口

| ファイル | 変更 |
| --- | --- |
| (server) `Telemetry/Protos/*.proto` | 新規。common / resource / logs / trace / metrics と collector の `*_service.proto` を直下に置き、`import` をファイル名だけにする(上流の `opentelemetry/proto/<種類>/v1/` の階層は使わない。生成型の名前空間は上流の `OpenTelemetry.Proto.*` のまま)|
| (server) `Template.MobileServer.Web.csproj` | `<Protobuf Include="Telemetry\Protos\*.proto" ProtoRoot="Telemetry\Protos" GrpcServices="None" />` と、`<Protobuf Update="Telemetry\Protos\*_service.proto" GrpcServices="Server" />`(サービスの無い proto に `Server` を付けると空の `*Grpc.cs` が生成され SA1518 が出る。既存の `Handlers\Protos` の定義と重ねない)|
| (server) `Telemetry/Otlp*Handler.cs` / `OtlpHelper.cs` / `TelemetryReceiverOption.cs` / `Log.cs` | 新規。`Export` を受け、リソースごとに `service.name`・端末・件数を Information、リソースの属性と 1 件ごとの内容を Debug で出して空の応答を返す |
| (server) `Application/ApplicationExtensions.cs` / `appsettings.json` | 受信口の登録(受信上限を `AddServiceOptions<Otlp*Handler>` で指定)と `MapGrpcService<Otlp*Handler>().RequirePort(Otel のポート)`。ASP.NET Core の計装から `/opentelemetry.proto.collector.` を除外する。`TelemetryReceiver` セクション |
| (server) `Components/Pages/QrPage.razor.cs` / `README.md` / `tests/.../QrPageTests.cs` | `OtelEndPoint` を 4317 に(`MakeGrpcEndPoint(BaseUri, Kestrel:Endpoints:Otel:Url)`)|
| (server) テスト | `Telemetry/OtlpHandlerTests.cs`(3 シグナルの件数と空の応答)/ `OtlpHelperTests.cs`(端末の取り出し、属性・点の整形、期間)|

確認: OtelSample のクライアント(gRPC)を 4317 へ向け、サーバーのログに受信が出る。

完了(2026-09-23)。結果は `Change_Summary.md` の区間 17。

### 📊1-2-1 端末: 収集の移設

| ファイル | 変更 |
| --- | --- |
| `Shell/DiagnosticSampler.cs` | 新規(「構成」のとおり。スナップショットの `DiagnosticSnapshot` と判定の `DiagnosticLevel` はサンプラーと同じファイル)。`Start` のたびに GC 回数・FPS などの基準値を取り直す |
| `Shell/LayoutMetrics.cs` | 削除(Measure・Arrange の数え方は `DiagnosticSampler` に統合)|
| `Helpers/CrashReport.cs(.android.cs)` / `Components/DiagnosticLogProvider.cs` → `Diagnostics/` | 移動。`CrashReport` は `AppDomain.UnhandledException` も捕捉し(同じ例外は 1 回だけ保存)、アプリと端末の情報を付けて `crash.json` に保存する。表示済み(`Shown`)の印を持つ |
| `Components/DeviceInformation.cs` + `.android.cs` | 新規。端末の情報(`ANDROID_ID`)|
| `Shell/DiagnosticPanel.xaml(.cs)` | スナップショットを表示するだけにする(サンプラーは `MainPageViewModel.DiagnosticSampler` から `Sampler` プロパティで受け取り、自身のレイアウトはサンプラーの `ExcludeLayout(this)` で除く)|
| `MainPageViewModel.cs` | パネルの表示中かつ前面のあいだだけサンプラーを動かす |
| `Modules/Main/DiagnosticsViewModel.cs` + `DiagnosticsView.xaml` | `DiagnosticLogEntry` の名前空間 |
| `Modules/Network/NetworkRealtimeViewModel.cs` | SignalR の端末状態の `DeviceId` を `device.id` に |
| `MauiProgram.cs` / `App.xaml.cs` | サンプラーと `DeviceInformation` の登録、名前空間 |

確認: パネルの各値(メモリとスレッド数が更新されること、表示を始めた直後の GC 回数・FPS)、診断画面、Realtime の端末一覧。

完了(2026-09-23)。結果は `Change_Summary.md` の区間 17。

### 🔁1-2-2 端末: 起動・停止と再起動

| ファイル | 変更 |
| --- | --- |
| `Diagnostics/TelemetryService.cs` | 新規(「端末の起動・停止」のとおり)。入力はプロパティ `ITelemetryControl.Suspend` / `EndPoint`。送信先の変化(送信の開始・停止・作り直しは 1-2-3)|
| `State/Settings.cs` | `TelemetryEnabled` と、有効かつ設定済みのときの送信先を返す `GetTelemetryEndPoint()` |
| `Modules/Main/SettingView.xaml` + `SettingViewModel.cs` | Network の OTEL の下にテレメトリの有効 / 無効のスイッチ。切り替えたときと QR で `OtelEndPoint` が変わったときに `ITelemetryControl.EndPoint` へ送信先を渡す |
| `MainPageViewModel.cs` | 中断・再開を `ITelemetryControl` へ渡す |
| `MauiProgram.cs` / `Log.cs` | `ConfigureDiagnostics()` で `TelemetryService` と 2 つの interface を登録、起動時の送信先。送信先の変化(Information)とサンプラーの開始・停止(Debug)のログ |

確認: パネルの表示・非表示とテレメトリの有効・無効の組み合わせでのサンプラーの動作、中断での停止と再開での開始。

完了(2026-09-23)。結果は `Change_Summary.md` の区間 17。

### 📤1-2-3 端末: OTEL の送信

| ファイル | 変更 |
| --- | --- |
| `Template.MobileApp.csproj` | `OpenTelemetry` / `OpenTelemetry.Exporter.OpenTelemetryProtocol` 1.19.1 を追加。`MetricsSupport` を全構成で true、`EventSourceSupport=true` |
| `MauiProgram.cs` | 診断の登録を `ConfigureDiagnostics()`(`ConfigureLogging()` の前)にまとめる(`AddMetrics()` を DEBUG の外へ、Release では MAUI の `IDiagnosticsManager` を外す、`DiagnosticsInstrumentation`、`TelemetryService` と 2 つの interface。サンプラーは Shell として `ConfigureComponents` で登録)。`TelemetryLoggerProvider` をログの `ILoggerProvider` として登録、ナビゲーションのプラグインの登録 |
| `Diagnostics/TelemetryService.cs` + `.android.cs` / `TelemetryLoggerProvider.cs` / `DiagnosticsInstrumentation.cs` / `SdkEventListener.cs` | 新規(「送る内容」のとおり)。送信は OTLP/HTTP(protobuf)。送れなかった中身はメモリのリングバッファーから送り直す(`TelemetrySendHandler`)。HTTP のメトリクスは `System.Net.Http` の `http.client.request.duration`(エクスポーター自身の送信は `AndroidMessageHandler` で除く)|
| `Diagnostics/TelemetryService.cs`(制御)| 送信の開始・停止・作り直し、中断での Flush、クラッシュの送信、SDK の警告のログ(送信の失敗は除く) |
| `Diagnostics/CrashReport.*` | `Crashed` イベント(プロセスが終了するか `IsTerminating` を含む)|
| `Extender/NavigationTelemetryPlugin.cs` | 画面遷移のスパン |
| `Usecase/NetworkUsecase.cs` | 実行の補助メソッド(`ExecuteVerboseAsync` など)の `CancellationToken` を最後の省略可能な引数に |
| `Modules/Network/NetworkTelemetryView.xaml(.cs)` + `NetworkTelemetryViewModel.cs` / `NetworkMenuView.xaml` / `Modules/Main/DiagnosticsView.xaml` + `DiagnosticsViewModel.cs` / `Modules/ViewId.cs` / `Markup/AppIcons.cs` / `Log.cs` | 送信のデモ(Network メニューの Telemetry)、診断画面の Telemetry のカード、アイコン、ログのメッセージ |
| (server) `Telemetry/OtlpReceiver.cs` / `OtlpHttpEndpoints.cs` | 新規。HTTP の受信口(4318)。受信の処理を gRPC と共通にする(`Otlp*Handler` は `OtlpReceiver` を呼ぶだけ)|
| (server) `Application/ApplicationExtensions.cs` / `appsettings.json` | Kestrel の `OtelHttp`(4318、HTTP/1.1)、`MapOtlpHttpEndpoints`、ASP.NET Core の計装から `/v1` を除外 |
| (server) `Components/Pages/QrPage.razor.cs` / `README.md` | `OtelEndPoint` を HTTP の 4318 に(`MakeEndPoint(BaseUri, Kestrel:Endpoints:OtelHttp:Url)`)|
| (server) テスト | `Telemetry/OtlpReceiverTests.cs`(`OtlpHandlerTests` から改名)/ `OtlpHttpEndpointsTests.cs`(protobuf、gzip、415 / 413 / 400)/ `QrPageTests` |

確認: 有効 / 無効の切り替え、送信先の変更での作り直し、サーバー停止中の失敗のログとメモリからの送り直し、クラッシュの送信(次回の起動時を含む)、中断での Flush。

完了(2026-09-23)。結果は `Change_Summary.md` の区間 17。

### 🗄️1-3 サーバー: 保存

| ファイル | 変更 |
| --- | --- |
| (server) `Assets/Data/TelemetrySchema.sql` | 新規。「サーバーのデータ」のテーブルと索引 |
| (server) `Application/ApplicationExtensions.cs` / `appsettings.json` | `telemetry.db` の接続(WAL)と `IDbProvider`、起動時のスキーマ適用 |
| (core) `Models/Entity/Telemetry*Entity.cs` / `Accessors/TelemetryAccessor.cs` + `Sql/*.sql` / `Services/TelemetryService.cs` | 新規。保存(1 トランザクション)、照会、期間での削除 |
| (server) `Telemetry/OtlpMapper.cs` | 新規 |
| (server) `Telemetry/OtlpReceiver.cs` / `OtlpHttpEndpoints.cs` / `Otlp*Handler.cs` | ログ出力から保存へ。不正な項目は `partial_success`、保存の失敗は HTTP = 503 / gRPC = `UNAVAILABLE` |
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
| `MetricsSupport` を Release でも有効にしたときの影響 | 起動時間(Release の実機で。サイズは `Change_Summary.md` の区間 17)| 1-2-3 |

## 📝記録

各段階の完了時に `Change_Summary.md`、`Task_Checklist.md`(完了した番号を削除)、サーバーの `README.md`(受信口のポートと Telemetry)、`Telemetry_Study.md`(方式の決定、`NetworkOperator` などの古い記述)を更新する。
