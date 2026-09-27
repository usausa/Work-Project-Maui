# 🗄️テレメトリのサーバー側の実装計画(保存・端末の管理・ダッシュボード・テレメトリ画面)

template-maui-server で OTLP の受信内容を端末ごとの SQLite ファイルに保存し、ダッシュボード(テレメトリのサマリ、端末一覧と端末の管理)とテレメトリ画面(端末を選んでメトリクス・トレース・ログ)に表示する。受信した内容はサーバー内のバスで通知し、表示中の画面をリアルタイムに更新する。`Telemetry_Plan.md` の 1-3 / 1-4 の詳細で、番号は `Task_Checklist.md` の 1 節と同じ。

| 番号 | 段階 | 内容 |
| --- | --- | --- |
| 1-3-1 | 保存 | 端末ごとの DB、受信からの保存(重複の排除、拒否は `partial_success`、保存の失敗は送り直させる)|
| 1-3-2 | 端末の登録とキャッシュとバス | 端末の登録(受信での自動登録、無効の端末の拒否)、ダッシュボード用のキャッシュ、受信の通知 |
| 1-3-3 | 保持期間 | 期限を過ぎた行と、受信が途絶えた端末のファイルの削除 |
| 1-4-1 | ダッシュボード | テレメトリのサマリ、端末一覧と端末の管理(検索・追加・編集・削除)|
| 1-4-2 | テレメトリ画面 | 端末の選択とメトリクスのグラフ |
| 1-4-3 | テレメトリ画面 | トレースのウォーターフォール |
| 1-4-4 | テレメトリ画面 | ログ |

ファイルパスは、(server) が `template-maui-server/src/Template.MobileServer.Web/`、(core) が `template-maui-server/src/Template.MobileServer.Core/`、(test) が `template-maui-server/tests/Template.MobileServer.UnitTests/` からの相対。

## ⚖️決定事項

| 項目 | 決定 |
| --- | --- |
| 保存先 | テレメトリは端末ごとの SQLite ファイル `telemetry/<端末 ID>.db`(フォルダーはオプション `TelemetryStorage:Root`)。端末の登録は業務データの `data.db` の `Device` |
| 端末の識別 | Resource の `device.id`、無ければ `app.installation.id`。ファイル名に使うので英数字・`-`・`_` の 64 文字以内に限る(このアプリの `ANDROID_ID` と GUID は該当)。識別できないリソースは保存せず、`partial_success` の拒否件数と理由で返す |
| 端末の登録 | 名前・グループ・メモ・有効を持つ。受信した端末が未登録なら自動で登録する(名前の既定は機種、無ければ端末 ID)。ダッシュボードで検索・追加・編集・削除する。削除は登録とテレメトリのファイルを消す(送信が続けば自動で登録し直される。止めるときは無効にする)|
| 無効の端末 | 受信しても保存しない(`partial_success` の拒否件数と理由で返すので、端末は送り直さない)。ダッシュボードでは薄く表示し、サマリの件数に含めない。保存済みのテレメトリは見られる |
| 接続 | Microsoft.Data.Sqlite の接続プール(接続文字列は端末ごと。`Cache=Shared` は付けない)。ファイルを作るときに WAL とスキーマ、開くたびに `synchronous=NORMAL` と `cache_size`(256 KiB。オプション)。スキーマの版は `user_version` で持ち、その端末をプロセスで最初に開いたときに確かめる。画面からの照会ではファイルを作らない |
| 書き込み | 端末ごとに 1 つずつ(プロセス内の端末ごとのロック。保持期間の削除と端末の削除も同じロック)。1 回の Export の 1 端末分を 1 トランザクション。画面の読み取りはロックを取らない(WAL)|
| 重複 | 送り直しで同じ内容が届いても 1 件にする。スパン = (TraceId, SpanId)、メトリクスの点 = (系列, 時刻)、ログ = (時刻, 内容のハッシュ)の一意キーで `INSERT OR IGNORE` |
| 時刻 | UTC の Unix ナノ秒(INTEGER)。表示の範囲と保持期間は端末が付けた時刻で判定する。最終受信と登録日時は `TimeProvider` の時刻(OTLP/HTTP の受信口にはサービスコンテキストが無い)|
| 保持期間 | ログ・トレース 7 日、メトリクス 30 日。最後の受信から 30 日を過ぎた端末はテレメトリのファイルを削除する(登録は残る)。オプション `TelemetryRetention` |
| ダッシュボードの値 | メモリのキャッシュ(`TelemetryDeviceRegistry`)。起動時に登録と全端末のファイルから作り、受信と登録の変更のたびに更新する。ダッシュボードは DB を読まない |
| 画面への通知 | サーバー内のバス(`TelemetryBus`)。受信処理が保存の後に保存した内容を、登録の変更の後に端末 ID を載せて通知する。画面は表示している間だけ購読する |
| 描画 | グラフは SVG、ウォーターフォールは HTML。どちらもサーバーで描画し、JavaScript を使わない(CSP の範囲)|
| 受信の結果 | 拒否した項目は `partial_success`。保存の失敗(DB・I/O)は HTTP = 503、gRPC = `UNAVAILABLE`(端末は取っておいて送り直す)|
| 受信のログ | 1 回ごとの受信は Debug(内容は保存するので Information には出さない)。自動登録は Information、保存の失敗は Error |

## 🗄️データ

### 📒端末の登録(`data.db`)

| テーブル | 主な列 | キー |
| --- | --- | --- |
| `Device` | DeviceId、Name、GroupName、Note、IsEnabled、RegisteredAt | 主キー DeviceId |

スキーマは既存の `Assets/Data/Schema.sql` に足す。

### 📁テレメトリ(端末ごとのファイル)

| テーブル | 主な列 | キー・索引 |
| --- | --- | --- |
| `DeviceInfo` | DeviceId、InstallationId、Manufacturer、Model、OsName、OsVersion、ServiceName、ServiceVersion、FirstReceivedAt、LastReceivedAt | 1 行だけ(受信した Resource から。受信のたびに更新)|
| `Resource` | Id、Hash、ServiceInstanceId、ServiceVersion、AttributesJson、FirstSeenAt | 一意 Hash(属性をキー順に並べた JSON の SHA-256)|
| `MetricSeries` | Id、Name、ScopeName、Unit、Kind(Gauge / Sum / Histogram / ExponentialHistogram / Summary)、Temporality、IsMonotonic、AttributesKey、AttributesJson | 一意 (Name, ScopeName, AttributesKey) |
| `MetricPoint` | SeriesId、TimeUnixNano、StartTimeUnixNano、Value(ゲージ・合計)、Count、Sum、Min、Max、Detail(ヒストグラムの境界と件数、指数ヒストグラム、サマリーの分位の JSON)| 主キー (SeriesId, TimeUnixNano)(WITHOUT ROWID)、(TimeUnixNano) |
| `Span` | TraceId、SpanId、ParentSpanId、Name、Kind、StartTimeUnixNano、EndTimeUnixNano、StatusCode、StatusMessage、ScopeName、ResourceId、AttributesJson、EventsJson、LinksJson | 主キー (TraceId, SpanId)(WITHOUT ROWID)、(StartTimeUnixNano) |
| `Trace` | TraceId、RootName、StartTimeUnixNano、EndTimeUnixNano、SpanCount、ErrorCount | 主キー TraceId、(StartTimeUnixNano)。スパンを保存したトランザクションで、そのトレースのスパンから集計し直す |
| `Log` | Id、TimeUnixNano、ObservedTimeUnixNano、SeverityNumber、SeverityText、EventName、Body、TraceId、SpanId、ScopeName、ResourceId、AttributesJson、Hash | 一意 (TimeUnixNano, Hash)(時刻の索引を兼ねる)、(TraceId)(空を除く部分索引)|

- 属性は型を残した JSON(`{"キー": 値}`。配列・キー値リスト・バイト列(16 進)を含む)。ID は小文字の 16 進(無ければ空文字)
- ログの時刻は `time_unix_nano`、無ければ `observed_time_unix_nano`。本文は文字列ならそのまま、それ以外は JSON。Hash は LogRecord のバイト列の SHA-256 の先頭 8 バイト
- トレースの RootName は親の無いスパンの名前(まだ届いていなければ空で、画面は最初のスパンの名前を出す)
- メトリクス: `NO_RECORDED_VALUE` の点と Exemplar は保存しない。Temporality はそのまま保存する(このアプリはカウンターとヒストグラムを Delta で送る)。系列に Resource は含めない(アプリを再起動しても同じ系列)
- 系列と Resource の Id は端末ごとにメモリに持ち、無いものだけ DB に足す

## 🧠端末の登録とキャッシュ(`TelemetryDeviceRegistry`)

端末ごとに、登録とテレメトリの要約をメモリに持つ(ダッシュボードとテレメトリ画面の端末の一覧はここから読む)。

| 値 | 内容 | 作り方 |
| --- | --- | --- |
| 登録 | 名前、グループ、メモ、有効、登録日時 | 起動時に `data.db` から / 自動登録と管理の操作 |
| 端末の情報 | `DeviceInfo`(機種、OS、アプリの版、初回・最終受信)| 起動時に全ファイルから / 受信のたび |
| 最新値 | 電池、無線 LAN の信号強度、CPU、メモリ(値と時刻)| 起動時は各系列の最後の点 / 受信した点(時刻の新しいほう)|
| 件数(24 時間)| エラー(重大度 ERROR 以上)とクラッシュ(イベント名 `exception` の FATAL)を 1 時間ごとに数えた 24 個 | 起動時は `Log` を集計 / 受信したログ |
| 直近のエラー | 全端末を通して新しい 20 件(端末、時刻、本文の 1 行目)| 同上 |
| 受信の推移 | 全端末の 1 分ごとの件数(点・スパン・ログ)60 個 | サーバーの起動から(保存しない)|

| 操作 | 内容 |
| --- | --- |
| 起動時の読み込み | `data.db` の登録と全端末のファイルを読む。登録の無いファイルは登録する |
| 受信 | 未登録なら自動で登録する。無効なら拒否を返す。保存した内容で要約を更新する |
| 管理 | 追加・編集・削除は `data.db`(削除はテレメトリのファイルも)に書いてから反映する |
| 保持期間 | テレメトリのファイルを削除した端末は、要約だけを消す(登録は残る)|

- 画面には読み取り専用のスナップショットを返す(更新はロックの中で作り直し、読み取りはロックを取らない)
- 受信中 = 最後の受信から 2 分以内(メトリクスは 30 秒ごとに届く)

## 🔔バス(`TelemetryBus`)

| 項目 | 内容 |
| --- | --- |
| 通知 | `Received`(端末、受信時刻、保存した点(系列の情報付き)・スパン・集計し直したトレース・ログ。重複で捨てた分は含めない)、`DeviceChanged`(登録の追加・変更・削除、テレメトリのファイルの削除。端末 ID)|
| 発行 | `Received` は受信処理が保存と要約の更新の後に、`DeviceChanged` は `TelemetryDeviceRegistry` が反映の後に発行する。同じ端末の通知が前後することがある(送り直しの古い分など)ので、受け取る側は時刻で並べる |
| 購読 | 画面が表示している間だけ購読する(`OnInitialized` で登録、`Dispose` で解除。既存の `NotificationBus` と同じ .NET のイベント)|
| 購読側の決まり | イベントの中では描画しない。受け取った内容を溜めて更新を予約するだけですぐ戻り、描画は `RefreshTimer` がまとめて行う(受信のスレッドで描画しない、多数の端末からの通知を間引く)|
| 例外 | 購読側の例外はバスがログに出して次の購読側へ進む(受信は失敗させない)|

`RefreshTimer`(画面の共通部品)は 1 秒ごとに、予約があれば `InvokeAsync` の中で反映して描画する。通知が続いても 1 秒に 1 回は描画し(通知が止むのを待たない)、予約が無くても指定の間隔(10 秒)で更新する(受信が途絶えた端末の状態や、グラフの時間軸を進めるため)。

## 📥受信の流れ

1. 受信口(HTTP / gRPC)が本文を読み、`OtlpReceiver.ReceiveAsync` へ渡す
2. リソースごとに端末を決め(識別できなければ拒否件数に足す)、`TelemetryDeviceRegistry` で登録を確かめる(未登録なら自動で登録、無効なら拒否件数に足す)
3. `OtlpMapper` で保存用のまとまりへ変換する(不正な項目は拒否件数に足す)
4. `TelemetryService.SaveAsync`(core)が端末のロックの中で 1 トランザクションで保存する(Resource・系列を足し、点・スパン・ログを `INSERT OR IGNORE`、`Trace` を集計し直し、`DeviceInfo` を更新)。新しく入った分を返す
5. `TelemetryDeviceRegistry` の要約を更新し、`TelemetryBus` に通知する
6. 拒否があれば `partial_success`(件数と理由)を返す。保存の失敗は 503 / `UNAVAILABLE`

## 🖥️画面

### 📋ダッシュボード(`/dashboard`)

| 部分 | 内容 |
| --- | --- |
| サマリ | カード: 端末(受信中 / 有効な端末)、エラー(24 時間)、クラッシュ(24 時間)、電池 20% 未満の端末、受信(1 分あたりの件数と直近 60 分のスパークライン)。無効の端末は数えない |
| ツールバー | 検索(端末 ID・名前・グループの部分一致)、端末の追加 |
| 端末一覧 | 状態(受信中 / 途絶 / 受信なし)、名前(端末 ID・グループ)、機種、OS、アプリの版、電池、無線 LAN、CPU、メモリ、エラー・クラッシュ(24 時間)、最終受信(経過時間)、操作(編集・削除)。無効の端末は薄く表示する。行を選ぶとテレメトリ画面へ |
| 直近のエラー | 新しい 10 件(端末、時刻、本文の 1 行目)。行を選ぶとその端末のログへ |

- 追加・編集は `DeviceEditDialog`(端末 ID(追加のときだけ)、名前、グループ、メモ、有効)。削除は確認のうえ、登録とテレメトリのファイルを消す
- キャッシュだけを読む。バスの通知(1 秒ごとにまとめる)と 10 秒ごとに更新する

### 📈テレメトリ画面(`/telemetry/{DeviceId?}`)

| 部分 | 内容 |
| --- | --- |
| 見出し | 端末の選択(登録の一覧。受信中・無効の印付き)、名前・グループ・メモ、機種・OS・アプリの版・最終受信、範囲(15 分 / 1 時間 / 6 時間 / 24 時間 / 7 日 / 30 日)。タブ・範囲・選んだトレースは URL のクエリ(`tab` / `range` / `trace`)に持つ |
| メトリクス | 計器ごとのグラフのカード(属性の組み合わせごとに線。凡例と最新値)。既知の計器は名前と単位を整えて先に並べ、それ以外は受信した名前と単位のまま後ろに並べる |
| トレース | トレースの一覧(開始、ルートスパン、所要時間、スパン数、エラー。エラーだけ・名前で絞り込み)。選んだトレースのウォーターフォールとスパンの詳細、そのトレースのログ |
| ログ | 一覧(時刻、重大度、本文の 1 行目、スコープ、トレースへのリンク)。重大度・本文(部分一致)・トレースで絞り込み。行を開くと属性、例外のスタックトレース、ID、Resource。新しい順に 100 件ずつ(続きは「さらに読み込む」)|

- 表示中のタブだけ読み込む。バスの通知はこの端末の分だけ受け取り、表示中のタブに足す(グラフは点、トレースは一覧と表示中のウォーターフォール、ログは絞り込みに合うもの)
- メトリクスの値: ゲージは平均、Delta の合計は 1 分あたり、ヒストグラムは平均(Sum / Count)と回数。範囲に応じて時間で束ねる(15 分・1 時間はそのまま、6 時間は 1 分、24 時間は 5 分、7 日は 30 分、30 日は 2 時間)。通知で届いた点も同じ束に足す

| 既知の計器 | 表示 |
| --- | --- |
| `process.cpu.utilization` | CPU(%)|
| `process.memory.usage` | メモリ(MB)|
| `application.gc.last_collection.heap.size` | ヒープ(MB)|
| `process.thread.count` | スレッド |
| `hw.battery.charge` | 電池(%)|
| `application.wifi.signal_strength` | 無線 LAN(dBm)|
| `dotnet.gc.collections` | GC(回 / 分、世代ごと)|
| `dotnet.gc.heap.total_allocated` | 割り当て(MB / 分)|
| `dotnet.exceptions` | 例外(件 / 分)|
| `http.client.request.duration` | HTTP(平均 ms と回数。属性の組み合わせごと)|

### 🧵ウォーターフォール

| 項目 | 内容 |
| --- | --- |
| 並び | 親子の木の順(兄弟は開始時刻順)。字下げで深さを示し、子を持つ行は開閉できる。親が届いていないスパンは最上位に置く |
| 時間軸 | トレースの開始からの相対時間。区切りのよい目盛り(ms / s)|
| バー | 開始位置と長さ(幅は最低 2 px)、所要時間。エラーは赤。スパンのイベントの時刻に印(`exception` は赤)|
| 詳細 | 行を選ぶと、名前、種類、状態とメッセージ、開始(トレースの開始から)・所要時間、ID、属性、イベント(例外のスタックトレース)、リンク、Resource を横に出す(狭い画面では下)|

### 🧩部品

| ファイル | 役割 |
| --- | --- |
| (server) `Components/Telemetry/TimeSeriesChart.razor(.cs)` | 折れ線(複数系列)。幅に合わせて伸縮、区切りのよい目盛り(24 時間以上は日付付き)、欠けた区間は線を切る。点が 150 以下なら点ごとに時刻と値の `<title>` |
| (server) `Components/Telemetry/Sparkline.razor` | 軸の無い小さな折れ線 |
| (server) `Components/Telemetry/TraceWaterfall.razor(.cs)` / `SpanDetail.razor` | ウォーターフォールとスパンの詳細 |
| (server) `Components/Telemetry/AttributeTable.razor` | 属性の JSON をキーと値の表に(値は折り返す)|
| (server) `Components/Telemetry/MetricChartModel.cs` / `WaterfallModel.cs` | 表示用の計算(束ね・通知の点の追加・既知の計器の名前と単位、木の順と開閉)|
| (server) `Components/Telemetry/TelemetryFormat.cs` | 時刻・経過時間・所要時間・バイト数・割合・重大度の表示と色 |
| (server) `Components/Dialogs/DeviceEditDialog.razor(.cs)` / `DeviceDialogExtensions.cs` | 端末の追加・編集(`DataEditDialog` と同じ作り。入力の検証は FluentValidation、端末 ID は受信と同じ文字の制限)|
| (server) `Components/RefreshTimer.cs` | バスの通知をまとめて描画する(「バス」)|

## 📏容量と負荷

開発機(Windows 11)で、端末ごとのファイルに 30 秒ごとの送信(12 点、10 回に 1 回ログ 1 件)を端末 100 台 × 12 時間分書いて測った値。

| 項目 | 値 |
| --- | --- |
| 書き込み(送信 1 回分)| 中央値 0.2 ms。8 並列で 15,900 回/秒、1 つずつで約 4,200 回/秒(端末 1,000 台のメトリクスは 33 回/秒)|
| 大きさ | メトリクスは 1 台 12 時間で 0.6 MB(30 日で約 36 MB)|
| 読み取り | 1 系列の直近 1 時間 0.03 ms、12 時間を 5 分で束ねて 0.3 ms。プールに無い接続を開くと +0.7 ms |
| 保持期間の削除 | 最も古い 1 時間分が 1 台 約 2 ms |
| 接続 | プールの接続 1 本(受信中の端末 1 台)でメモリ 0.18〜0.55 MB(`cache_size` 64 KiB 〜 既定の 2 MiB。256 KiB で 0.37 MB)とハンドル 5(ファイルは `.db` / `-wal` / `-shm`)。使われない接続は 4〜8 分で閉じ、`-wal` も消える |
| 起動時の読み込み | 初めて開く接続での書き込みと読み取り 1 回で約 4 ms(キャッシュは起動時に全端末分を読む)|

## 🏗️構成

| ファイル | 役割 |
| --- | --- |
| (server) `Telemetry/OtlpReceiver.cs` | 受信の処理(非同期。リソースごとに端末を決め、登録の確認・変換・保存・要約の更新・通知)|
| (server) `Telemetry/OtlpMapper.cs` | OTLP → 保存用のまとまり(`TelemetryBatch`)|
| (server) `Telemetry/SqliteTelemetryDbProvider.cs` + `TelemetryStorageOption.cs` | 端末ごとのファイルの接続(作成時のスキーマ、開くたびの PRAGMA)、端末の一覧(ファイル名)、削除(プールを空けてから `-wal` / `-shm` も)|
| (server) `Telemetry/TelemetryDeviceRegistry.cs` | 端末の登録とテレメトリの要約(ダッシュボード用のキャッシュ)、自動登録と管理の操作 |
| (server) `Telemetry/TelemetryBus.cs` + `TelemetryReceivedEventArgs.cs` | 受信と登録の変更の通知 |
| (server) `Assets/Data/TelemetrySchema.sql` / `Assets/Data/Schema.sql` | 端末ごとのファイルのスキーマ / `data.db` の `Device` |
| (server) `Workers/TelemetryRetentionWorker.cs` + `TelemetryRetentionWorkerOption.cs` | 保持期間の削除 |
| (server) `Components/Pages/DashboardPage.razor(.cs)` / `TelemetryPage.razor(.cs)` | 画面 |
| (server) `Components/Telemetry/*` / `Components/Dialogs/DeviceEditDialog.*` / `Components/RefreshTimer.cs` | 部品 |
| (core) `Infrastructure/Telemetry/ITelemetryDbProvider.cs` | 端末ごとの接続の取得(実装は server。core は SQLite を参照しない)|
| (core) `Services/TelemetryService.cs` | テレメトリの保存・照会・削除(端末ごとのロック、系列と Resource の Id)|
| (core) `Services/DeviceService.cs` | 端末の登録の照会・追加・更新・削除、自動登録(`DataService` と同じ作り。結果は `DataWriteStatus`)|
| (core) `Accessors/TelemetryAccessor.cs` / `DeviceAccessor.cs` + `Accessors/Sql/*.sql` | SQL(テレメトリは接続・トランザクションを引数で受け取る。登録は既定の `IDbProvider`)|
| (core) `Models/Entity/DeviceEntity.cs` / `Telemetry*Entity.cs` / `Models/Telemetry*.cs` | エンティティ、保存のまとまりと結果、照会の結果 |

## 🪜段階ごとの作業

### 🗄️1-3-1 保存

| ファイル | 変更 |
| --- | --- |
| (server) `Assets/Data/TelemetrySchema.sql` | 新規(「テレメトリ」のテーブルと索引、`journal_mode=WAL`、`user_version`)|
| (server) `Telemetry/SqliteTelemetryDbProvider.cs` + `TelemetryStorageOption.cs` / (core) `Infrastructure/Telemetry/ITelemetryDbProvider.cs` | 新規 |
| (core) `Models/Entity/Telemetry*Entity.cs` / `Models/Telemetry*.cs` / `Accessors/TelemetryAccessor.cs` + SQL / `Services/TelemetryService.cs` | 新規(保存と、確認用の件数の照会)|
| (server) `Telemetry/OtlpMapper.cs` | 新規 |
| (server) `Telemetry/OtlpReceiver.cs` / `OtlpHttpEndpoints.cs` / `Otlp*Handler.cs` / `Log.cs` | 非同期にして保存へ。拒否は `partial_success`、保存の失敗は 503 / `UNAVAILABLE`。1 回ごとの受信のログは Debug |
| (server) `Application/ApplicationExtensions.cs` / `appsettings.json` / `.gitignore` | プロバイダーとオプション(`TelemetryStorage`)の登録、起動時にフォルダーを作る、`telemetry/` を除外 |
| (test) `Telemetry/OtlpMapperTests.cs` / `Telemetry/TelemetryServiceTests.cs`(一時フォルダーの SQLite)/ `OtlpReceiverTests.cs` / `OtlpHttpEndpointsTests.cs` | 変換、保存と重複、拒否と 503 |

確認: 端末の実データ(メトリクス、Telemetry デモのスパンとログ、クラッシュ)がファイルに入る、同じ内容の送り直しで増えない、識別できないリソースの拒否、保存できないときの 503 と端末の送り直し。

### 🔔1-3-2 端末の登録とキャッシュとバス

| ファイル | 変更 |
| --- | --- |
| (server) `Assets/Data/Schema.sql` / (core) `Models/Entity/DeviceEntity.cs` / `Accessors/DeviceAccessor.cs` + SQL / `Services/DeviceService.cs` | `Device` のテーブルと、登録の照会・追加・更新・削除・自動登録 |
| (server) `Telemetry/TelemetryDeviceRegistry.cs` / `TelemetryBus.cs` + `TelemetryReceivedEventArgs.cs` | 新規(「端末の登録とキャッシュ」「バス」)|
| (server) `Telemetry/OtlpReceiver.cs` | 登録の確認(自動登録、無効の拒否)、保存の後に要約の更新と通知 |
| (core) `Services/TelemetryService.cs` + SQL | 端末の要約の読み込み(起動時のキャッシュ用)、端末のファイルの削除 |
| (server) `Components/RefreshTimer.cs` | 新規 |
| (server) `Application/ApplicationExtensions.cs` | 登録、起動時のキャッシュの読み込み |
| (test) `Telemetry/TelemetryDeviceRegistryTests.cs` / `TelemetryBusTests.cs` / `Components/RefreshTimerTests.cs` / `Services/DeviceServiceTests.cs` | 集計(24 時間の区切り)、起動時の読み込み(登録の無いファイル)、自動登録と無効の拒否、削除、購読側の例外、まとめての描画 |

確認: 新しい端末の自動登録、無効にした端末の拒否(端末は送り直さない)、サーバーを再起動してもキャッシュが戻る。

### 🧹1-3-3 保持期間

| ファイル | 変更 |
| --- | --- |
| (server) `Workers/TelemetryRetentionWorker.cs` + `TelemetryRetentionWorkerOption.cs` / `Workers/Log.cs` | 新規。起動の直後と 1 時間ごとに、端末ごとに期限を過ぎた行を削除する(端末のロックの中。スパンはトレースの単位)。最後の受信から保持期間を過ぎた端末は、テレメトリのファイルを削除する(登録は残す)。1 台の失敗はログに出して次の端末へ進む |
| (core) `Services/TelemetryService.cs` + SQL | 期限での削除 |
| (server) `Application/ApplicationExtensions.cs` / `appsettings.json` | `TelemetryRetention`(有効、間隔、ログ・トレース・メトリクス・端末の日数)|
| (test) `Telemetry/TelemetryServiceTests.cs` / `Workers/TelemetryRetentionWorkerTests.cs` | 期限での削除、ファイルの削除と登録の維持 |

確認: 時刻をずらしたテストデータでの削除。

### 📋1-4-1 ダッシュボード

| ファイル | 変更 |
| --- | --- |
| (server) `Components/Pages/DashboardPage.razor(.cs)` | 新規(「ダッシュボード」)|
| (server) `Components/Dialogs/DeviceEditDialog.razor(.cs)` / `DeviceDialogExtensions.cs` | 新規 |
| (server) `Components/Telemetry/Sparkline.razor` / `TelemetryFormat.cs` | 新規 |
| (server) `Components/Layout/NavMenu.razor` | Home の次に Dashboard と Telemetry |
| (server) `wwwroot/css/app.css` | ダッシュボードのクラス |
| (test) `Components/Pages/DashboardPageTests.cs` / `Components/Dialogs/DeviceFormValidatorTests.cs` / `Components/Layout/NavMenuTests.cs` | 表示と検索、入力の検証、リンクの数(6 → 8)|

確認: 端末の実データ、端末の送信を止めると途絶に変わる、エラー・クラッシュの件数、追加・編集・無効・削除(別の画面にも反映される)。

### 📈1-4-2 テレメトリ画面: 端末の選択とメトリクス

| ファイル | 変更 |
| --- | --- |
| (server) `Components/Pages/TelemetryPage.razor(.cs)` | 新規(見出し・タブ・メトリクス)|
| (server) `Components/Telemetry/TimeSeriesChart.razor(.cs)` / `MetricChartModel.cs` | 新規 |
| (core) `Services/TelemetryService.cs` + SQL | 系列の一覧、範囲と束ねる間隔での点の照会 |
| (test) `Components/Telemetry/MetricChartModelTests.cs` / `Components/Pages/TelemetryPageTests.cs` | 束ね、通知の点の追加、単位 |

確認: 表示中にグラフが 30 秒ごとに伸びる、範囲の切り替え、端末の切り替え。

### 🧵1-4-3 テレメトリ画面: トレース

| ファイル | 変更 |
| --- | --- |
| (server) `Components/Telemetry/TraceWaterfall.razor(.cs)` / `WaterfallModel.cs` / `SpanDetail.razor` / `AttributeTable.razor` | 新規 |
| (server) `Components/Pages/TelemetryPage.razor(.cs)` | トレースのタブ |
| (core) `Services/TelemetryService.cs` + SQL | トレースの一覧、スパン、トレースのログ |
| (test) `Components/Telemetry/WaterfallModelTests.cs` | 木の順、親の欠け、開閉 |

確認: 端末の Telemetry デモのスパン(親子)とログ、表示中に新しいトレースが一覧に出る。

### 📜1-4-4 テレメトリ画面: ログ

| ファイル | 変更 |
| --- | --- |
| (server) `Components/Pages/TelemetryPage.razor(.cs)` | ログのタブ |
| (core) `Services/TelemetryService.cs` + SQL | 絞り込みと続きの読み込み(時刻と Id の続きから)|
| (test) `Components/Pages/TelemetryPageTests.cs` | 絞り込み、通知の追加 |

確認: 警告・エラー・クラッシュのログ、トレースへの移動、表示中の追加。

## 🚫対象外

| 項目 | 内容 |
| --- | --- |
| 端末をまたぐ検索 | トレース ID・ログの本文は、端末を選んでから探す |
| 分位 | ヒストグラムは平均・最大・回数だけ(バケットは保存する)|
| 累積値 | Cumulative の点は差分にせず、そのまま表示する |
| グラフの操作 | 拡大・移動・ドラッグでの範囲の選択は無い(範囲は選択肢で切り替える)|
| 受信 | OTLP/JSON、認証、流量の制御(429)は無い |
| その他 | JSON の API、ダミーデータの投入画面は無い |

## 🔍着手前に確かめること

| 項目 | 内容 | 段階 |
| --- | --- | --- |
| Smart.Data.Accessor | トランザクションを受け取るメソッドで、エンティティの引数を SQL に展開できるか。`ON CONFLICT` / `RETURNING` を SQL ファイルに書けるか | 1-3-1 |
| タブとクエリ | MudTabs の切り替えを URL のクエリに反映する方法(履歴を増やさずに置き換える)| 1-4-2 |
