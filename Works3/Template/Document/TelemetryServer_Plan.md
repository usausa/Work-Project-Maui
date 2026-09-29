# 🗄️テレメトリのサーバー側の実装計画(保存・端末の管理・ダッシュボード・テレメトリ画面)

template-maui-server で OTLP の受信内容を端末ごとの SQLite ファイルに保存し、ダッシュボード(テレメトリのサマリ、端末一覧と端末の管理)とテレメトリ画面(端末を選んでメトリクス・トレース・ログ)に表示する。受信した内容はサーバー内のバスで通知し、表示中の画面をリアルタイムに更新する。`Telemetry_Plan.md` の 1-3 / 1-4 の詳細で、番号は `Task_Checklist.md` の 1 節と同じ。Accessor と SQL の書き方は example-maui-pos(`.claude/rules/accessor.md` / `sql.md`)に合わせる。

| 番号 | 段階 | 内容 |
| --- | --- | --- |
| 1-3-1 | 保存 | 端末ごとの DB、受信からの保存(重複の排除、拒否は `partial_success`、保存の失敗は送り直させる)|
| 1-3-2 | 端末の登録とキャッシュとバス | 端末の登録(受信での自動登録、無効の端末の拒否)、ダッシュボード用のキャッシュ、受信の通知 |
| 1-3-3 | 保持期間 | 期限を過ぎた行と、受信が途絶えた端末のファイルの削除 |
| 1-3-4 | 端末からの登録 | 端末が自分を登録する API(サーバー)と、その呼び出し(端末)|
| 1-4-1 | ダッシュボード | テレメトリのサマリ、端末一覧と端末の管理(検索・追加・編集・削除)|
| 1-4-2 | テレメトリ画面 | 端末の選択とメトリクスのグラフ |
| 1-4-3 | テレメトリ画面 | トレースのウォーターフォール |
| 1-4-4 | テレメトリ画面 | ログ |

ファイルパスは、(server) が `template-maui-server/src/Template.MobileServer.Web/`、(core) が `template-maui-server/src/Template.MobileServer.Core/`、(test) が `template-maui-server/tests/Template.MobileServer.UnitTests/`、(app) が `Template.MobileApp/` からの相対。

## ⚖️決定事項

| 項目 | 決定 |
| --- | --- |
| 保存先 | テレメトリは端末ごとの SQLite ファイル `telemetry/<端末 ID>.db`(フォルダーはオプション `TelemetryStorage:Root`、実行フォルダーからの相対)。端末の登録は業務データの `data.db` の `Device` |
| 端末の識別 | Resource の `device.id`、無ければ `app.installation.id`。ファイル名に使うので英数字・`-`・`_` の 64 文字以内に限る(`Domain/DeviceIdFormat`。このアプリの `ANDROID_ID` と GUID は該当)。識別できないリソースは保存せず、`partial_success` の拒否件数と理由で返す |
| 端末の登録 | 名前・グループ・メモ・有効を持つ。受信した端末が未登録なら自動で登録する(名前の既定は機種、無ければ端末 ID)。端末からも API で登録できる(名前を送る。登録済みなら名前を更新する)。ダッシュボードで検索・追加・編集・削除する。削除は登録とテレメトリのファイルを消す(送信が続けば自動で登録し直される。止めるときは無効にする)|
| 無効の端末 | 受信しても保存しない(`partial_success` の拒否件数と理由で返すので、端末は送り直さない)。ダッシュボードでは薄く表示し、サマリの件数に含めない。保存済みのテレメトリは見られる |
| 接続 | Microsoft.Data.Sqlite の接続プール(接続文字列は端末ごと。`Cache=Shared` は付けない)。プロセスで最初に開くときに `user_version` を見て、0 ならスキーマ(WAL を含む)を入れる。開くたびに `synchronous=NORMAL` と `cache_size`(256 KiB。オプション)。PRAGMA とスキーマの実行は `GenericAccessor`。画面からの照会ではファイルを作らない |
| 書き込み | 端末ごとに 1 つずつ(server の `TelemetryStore` が端末ごとのロックと保存済みの Id を持つ。保持期間の削除と端末の削除も同じロック。core の `TelemetryService` は状態を持たない)。1 回の Export の 1 端末分を 1 トランザクション。画面の読み取りはロックを取らない(WAL)|
| 重複 | 送り直しで同じ内容が届いても 1 件にする。スパン = (TraceId, SpanId)、メトリクスの点 = (系列, 時刻)、ログ = (時刻, 内容のハッシュ)の一意キーで `ON CONFLICT ... DO NOTHING` |
| 列挙型 | 系列の種類と Temporality、スパンの種類と状態は `Models/Enums` の列挙型にし、DB には名前の文字列で保存する(`Accessors/DataProfile` の `EnumTextConverter`。列挙型を含む INSERT は列ごとの引数で渡す)|
| 時刻 | テレメトリの時刻は UTC の Unix ナノ秒(INTEGER)。表示の範囲と保持期間は端末が付けた時刻で判定する。最終受信と登録日時はサービスコンテキストの時刻(OTLP/HTTP の受信口にも `ServiceContextEndpointFilter` を付ける。起動時の読み込みと保持期間の削除はスコープを明示して始める)。登録日時は UTC の文字列(`DateTimeTextConverter`)|
| 保持期間 | ログ・トレース 7 日、メトリクス 30 日。最後の受信から 30 日を過ぎた端末はテレメトリのファイルを削除する(登録は残る)。オプション `TelemetryRetention` |
| ダッシュボードの値 | メモリのキャッシュ(`TelemetryDeviceRegistry`)。起動時に登録と全端末のファイルから作り、受信と登録の変更のたびに更新する。ダッシュボードは DB を読まない |
| 画面への通知 | サーバー内のバス(`TelemetryBus`)。受信処理が保存の後に保存した内容を、登録の変更の後に端末 ID を載せて通知する。画面は表示している間だけ購読する |
| 描画 | グラフは SVG、ウォーターフォールは HTML。どちらもサーバーで描画し、JavaScript を使わない(CSP の範囲)|
| 受信の結果 | 拒否した項目は `partial_success`。保存の失敗(DB・I/O)は HTTP = 503、gRPC = `UNAVAILABLE`(端末は取っておいて送り直す)|
| 受信のログ | 1 回ごとの受信は Debug(受けた件数と保存した件数。内容は保存するので Information には出さない)。端末を識別できないリソースは Warning、無効の端末は Debug、自動登録は Information、保存の失敗は Error |
| GC の計器 | 端末の標準の計器(`dotnet.gc.collections` / `dotnet.gc.heap.total_allocated`)を届いたまま表示する(補正も置き換えもしない)。端末は将来 CoreCLR になる前提で、CoreCLR では正しい値になる。Mono では gen0 と割り当ての差分が負になることがある(理由は `Change_Summary.md` の区間 17 のナレッジ)|

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
| `MetricSeries` | Id、Name、ScopeName、Unit、Kind(Gauge / Sum / Histogram / ExponentialHistogram / Summary)、Temporality、IsMonotonic、AttributesJson(キー順)| 一意 (Name, ScopeName, AttributesJson) |
| `MetricPoint` | SeriesId、TimeUnixNano、StartTimeUnixNano、Value(ゲージ・合計)、Count、Sum、Min、Max、Detail(ヒストグラムの境界と件数、指数ヒストグラム、サマリーの分位の JSON)| 主キー (SeriesId, TimeUnixNano)(WITHOUT ROWID)、(TimeUnixNano) |
| `Span` | TraceId、SpanId、ParentSpanId、Name、Kind、StartTimeUnixNano、EndTimeUnixNano、StatusCode、StatusMessage、ScopeName、ResourceId、AttributesJson、EventsJson、LinksJson | 主キー (TraceId, SpanId)(WITHOUT ROWID)、(StartTimeUnixNano) |
| `Trace` | TraceId、RootName、StartTimeUnixNano、EndTimeUnixNano、SpanCount、ErrorCount | 主キー TraceId、(StartTimeUnixNano)。スパンを保存したトランザクションで、そのトレースのスパンから集計し直す |
| `Log` | Id、TimeUnixNano、ObservedTimeUnixNano、SeverityNumber、SeverityText、EventName、Body、TraceId、SpanId、ScopeName、ResourceId、AttributesJson、Hash | 一意 (TimeUnixNano, Hash)(時刻の索引を兼ねる)、(TraceId)(空を除く部分索引)|

- 属性は型を残した JSON(`{"キー": 値}`。配列・キー値リスト・バイト列(16 進)を含む)。ID は小文字の 16 進(無ければ空文字)
- ログの時刻は `time_unix_nano`、無ければ `observed_time_unix_nano`。本文は文字列ならそのまま、それ以外は JSON。Hash は LogRecord のバイト列の SHA-256 の先頭 8 バイト
- トレースの RootName は親の無いスパンの名前(まだ届いていなければ最初のスパンの名前)
- メトリクス: `NO_RECORDED_VALUE` の点と Exemplar は保存しない。Temporality はそのまま保存する(このアプリはカウンターとヒストグラムを Delta で送る。メモリ・スレッド数・ヒープは Sum の Cumulative(単調でない)で届く)。系列に Resource は含めない(アプリを再起動しても同じ系列)
- 保存しない項目: ID の長さが違うスパン、時刻の無い点とログ、値の無い数値の点
- 系列と Resource の Id は端末ごとにメモリに持ち(最初の書き込みで DB から読む)、無いものだけ DB に足す(コミットの後にメモリへ足す)

## 🧠端末の登録とキャッシュ(`TelemetryDeviceRegistry`)

端末ごとに、登録とテレメトリの要約をメモリに持つ(ダッシュボードとテレメトリ画面の端末の一覧はここから読む)。

| 値 | 内容 | 作り方 |
| --- | --- | --- |
| 登録 | 名前、グループ、メモ、有効、登録日時 | 起動時に `data.db` から / 自動登録と管理の操作 |
| 端末の情報 | `DeviceInfo`(機種、OS、アプリの版、初回・最終受信)| 起動時に全ファイルから / 受信のたび |
| 最新値 | 電池、無線 LAN の信号強度、CPU、メモリ、アプリケーション固有値の値 1・2(`application.custom.value1` / `value2`)(値と時刻)| 起動時は各系列の最後の点 / 受信した点(時刻の新しいほう)|
| 件数(24 時間)| エラー(重大度 ERROR 以上)とクラッシュ(FATAL)を 1 時間ごとに数えた 24 個 | 起動時は `Log` を集計 / 受信したログ |
| 直近のエラー | 全端末を通して新しい 20 件(端末、時刻、本文の 1 行目)| 同上 |
| 受信の推移 | 全端末の 1 分ごとの件数(点・スパン・ログ)60 個 | サーバーの起動から(保存しない)|

| 操作 | 内容 |
| --- | --- |
| 起動時の読み込み | `data.db` の登録と全端末のファイルを読む。登録の無いファイルは登録する |
| 受信 | 未登録なら自動で登録する。無効なら拒否を返す。保存した内容で要約を更新する |
| 管理 | 追加・編集・削除と端末からの登録は `data.db`(削除はテレメトリのファイルも)に書いてから反映する |
| 保持期間 | テレメトリのファイルを削除した端末は、要約だけを消す(登録は残る)|

- 画面には、読むたびにロックの中で作った読み取り専用の一覧を返す(端末の行 `TelemetryDeviceSummary`、直近のエラー、受信の推移)
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
3. `OtlpMapper` で保存用のまとまり(`TelemetryBatch`)へ変換する(不正な項目は拒否件数に足す)
4. `TelemetryStore`(server)が端末のロックを取り、保存済みの Id(最初の保存でファイルから読む)を渡して、`TelemetryService.SaveAsync`(core)が 1 トランザクションで保存する(Resource・系列を足し、点・スパン・ログを `ON CONFLICT ... DO NOTHING` で入れ、`Trace` を集計し直し、`DeviceInfo` を更新)。新しく入った分を `TelemetrySaveResult` で返し、新しく足した Resource と系列の Id は `TelemetryStore` が保存済みの Id に足す
5. `TelemetryDeviceRegistry` の要約を更新し、`TelemetryBus` に通知する
6. 拒否があれば `partial_success`(件数と理由)を返す。保存の失敗は 503 / `UNAVAILABLE`

## 🖥️画面

### 📋ダッシュボード(`/`)

| 部分 | 内容 |
| --- | --- |
| サマリ | アイコン付きのカード: 端末(受信中 / 有効と、受信中・途絶・受信なしの内訳の帯)、エラー(24 時間。ERROR 以上なのでクラッシュを含む)、クラッシュ(24 時間)、電池の少ない端末(残量 20% 未満)、受信(直前の 1 分の件数と直近 60 分のスパークライン)。無効の端末は数えない。件数が 1 以上なら色を付ける |
| ツールバー | 検索(端末 ID・名前・グループの部分一致)、端末の追加 |
| 端末一覧 | 状態(受信中 = 緑 / 途絶 = 橙 / 受信なし = 枠だけのバッジ。下に最終受信からの経過、ホバーで日時)、名前(下に端末 ID・グループ、ホバーでメモ)、機種(下に OS とアプリの版)、電池(残量の段階のアイコン)、無線 LAN(電波のアイコン)、CPU(横棒と %)、メモリ、値 1・2(アプリケーション固有値。0〜100 の横棒と値。良し悪しの色は付けない)、エラー・クラッシュ(1 以上は色付きのバッジ)、操作(編集・削除)。最新値はホバーで値と測った時刻を出す。無効の端末は薄く表示する。行を選ぶとテレメトリ画面へ |
| 直近のエラー | 新しい 10 件(時刻、端末、重大度、本文の 1 行目)。行を選ぶとその端末のログへ |

| 値の色 | 緑 | 橙 | 赤 |
| --- | --- | --- | --- |
| 電池 | 50% 以上 | 20% 以上 | 20% 未満 |
| CPU | 50% 未満 | 80% 未満 | 80% 以上 |
| 無線 LAN | -67 dBm 以上 | -80 dBm 以上 | -80 dBm 未満 |

- 無線 LAN のアイコンは 4 段(-55 dBm 以上 / -67 dBm 以上 / -80 dBm 以上 / それ未満)。色の区切りは端末の診断パネルと同じ
- 追加・編集は `DeviceEditDialog`(端末 ID(追加のときだけ)、名前、グループ、メモ、有効)。削除は確認のうえ、登録とテレメトリのファイルを消す
- キャッシュだけを読む。バスの通知(1 秒ごとにまとめる)と 10 秒ごとに更新する

### 📈テレメトリ画面(`/telemetry/{DeviceId?}`)

| 部分 | 内容 |
| --- | --- |
| 見出し | 端末の選択(登録の一覧。状態の色の印と無効の印付き)、状態・名前・グループ・メモ、機種・OS・アプリの版・最終受信、最新値(電池・無線 LAN・CPU・メモリ。ダッシュボードと同じ表し方)、範囲(15 分 / 1 時間 / 6 時間 / 24 時間 / 7 日 / 30 日)。タブ・範囲・選んだトレース・ログの重大度は URL のクエリ(`tab` / `range` / `trace` / `level`)に持つ(タブ・範囲・重大度の切り替えは履歴を増やさずに置き換え、端末とトレースの選択は履歴に残す)|
| メトリクス | 計器ごとのグラフのカード(属性の組み合わせごとに線。凡例と最新値)。既知の計器は名前と単位を整えて先に並べ、それ以外は受信した名前と単位のまま後ろに並べる |
| トレース | トレースの一覧(開始、ルートスパン、所要時間、スパン数、エラー。エラーだけ・名前で絞り込み)。選んだトレースのウォーターフォールとスパンの詳細、そのトレースのログ |
| ログ | 一覧(時刻、重大度、本文の 1 行目、スコープ(名前空間を除く)、トレースのタブへのリンク)。重大度(以上。`level`)・本文(部分一致)・トレース(URL の `trace`。外せる)で絞り込み。行を開くと本文の全体、重大度・イベント・観測時刻・スコープ・ID、属性、例外のスタックトレース、Resource(開いたときに読む)。新しい順に 100 件ずつ(続きは「さらに読み込む」。時刻と Id の前から)|

- 表示中のタブだけ読み込む。バスの通知はこの端末の分だけ受け取り、表示中のタブに足す(グラフは点、トレースは一覧と表示中のウォーターフォール、ログは絞り込みに合うもの)
- メトリクスの値: ゲージと単調でない合計(メモリなど)は平均、単調な Delta の合計は 1 分あたり(点の区間の長さで割る)、ヒストグラムは平均(Sum / Count)と回数。範囲に応じて時間で束ねる(15 分・1 時間はそのまま、6 時間は 1 分、24 時間は 5 分、7 日は 30 分、30 日は 2 時間)。束は SQL で集計し、範囲の前から始まる束は範囲の始まりに置く
- 通知で届いた点は同じ束に足す。読み込んだ集計の最後の点より後の点だけを足す(読み込みと通知が重なっても二重に数えない)。時間軸は 10 秒ごとに進め、範囲から外れた束を消す

| 既知の計器 | 表示 |
| --- | --- |
| `process.cpu.utilization` | CPU(%)|
| `process.memory.usage` | メモリ(MB)|
| `application.gc.last_collection.heap.size` | ヒープ(MB)|
| `process.thread.count` | スレッド(個)|
| `hw.battery.charge` | 電池(%)|
| `application.wifi.signal_strength` | 無線 LAN(dBm)|
| `dotnet.gc.collections` | GC(回 / 分、世代ごと)|
| `dotnet.gc.heap.total_allocated` | 割り当て(MB / 分)|
| `dotnet.exceptions` | 例外(件 / 分、種類ごと)|
| `http.client.request.duration` | HTTP(平均 ms と回数。属性の組み合わせごと)|
| `application.custom.value{N}` | 値 N(アプリケーション固有値。既知の計器の後に番号の順。ほかの計器は名前のまま最後)|

### 🧵ウォーターフォール

| 項目 | 内容 |
| --- | --- |
| 並び | 親子の木の順(兄弟は開始時刻順)。字下げで深さを示し、子を持つ行は開閉できる。親が届いていないスパンは最上位に置く |
| 時間軸 | トレースの開始からの相対時間。区切りのよい目盛り(µs / ms / s)|
| バー | 開始位置と長さ(幅は最低 2 px)、所要時間。エラーは赤。スパンのイベントの時刻に印(`exception` は赤)|
| 詳細 | 行を選ぶと(開いたときはルート)、名前、種類、状態とメッセージ、開始(トレースの開始から)・所要時間、ID、属性、イベント(例外のスタックトレースは表の外に)、リンク、Resource を横に出す(狭い画面では下)|
| 一覧 | 範囲に始まったトレースを新しい順に 200 件まで(開始、ルートスパン、所要時間、スパン数、エラー)。エラーのあるものだけ・ルートスパンの名前の部分一致で絞る(SQL)。選んだトレースは URL の `trace` に持ち、履歴に残す |
| 通知 | 受信で集計し直したトレースを一覧に反映し、表示中のトレースのスパンかログが届いたら詳細を読み直す(開閉と選んだスパンは引き継ぐ)|

### 🧩部品

| ファイル | 役割 |
| --- | --- |
| (server) `Components/Telemetry/TimeSeriesChart.razor(.cs)` | 折れ線(複数系列)。幅に合わせて伸縮、区切りのよい目盛り(値は 0 以上なら 0 から、時刻は現地時刻の区切りで 24 時間以上は日付付き)、欠けた区間(点の間隔の中央値の 3 倍より空いた所)は線を切り、前後が欠けた点は丸で出す。点が 150 以下なら点ごとにホバーで時刻と値(ヒストグラムは最大と回数も)。凡例に最新値 |
| (server) `Components/Telemetry/TelemetryRange.cs` / `TelemetryLogLevel.cs` / `TelemetryLinks.cs` | 範囲と束ねる間隔(経過時間を含む範囲)、ログの重大度の絞り込み、テレメトリ画面の URL とタブの値 |
| (server) `Components/Telemetry/LogDetail.razor(.cs)` | ログの詳細 |
| (server) `Components/Telemetry/Sparkline.razor(.cs)` | 軸の無い小さな折れ線 |
| (server) `Components/Telemetry/TraceWaterfall.razor(.cs)` / `SpanDetail.razor(.cs)` | ウォーターフォールとスパンの詳細 |
| (server) `Components/Telemetry/AttributeTable.razor(.cs)` | 属性の JSON をキーと値の表に(値は折り返す。別に出すキーは除ける)|
| (server) `Components/Telemetry/MetricChartModel.cs` / `WaterfallModel.cs` | 表示用の計算(束の値・通知の点の追加・範囲から外れた束の削除・既知の計器の名前と単位と線の名前にする属性、木の順と開閉)|
| (server) `Components/ViewHelper.cs` | 時刻・経過時間・所要時間・割合・信号強度・状態・重大度の表示と、値の良し悪し(`TelemetryLevel`)の色。`_Imports.razor` で static インポート |
| (server) `Components/Telemetry/MetricCell.razor(.cs)` | 最新値のセル(電池は残量の段階のアイコン、無線 LAN は電波のアイコン、CPU は横棒と %、メモリは MB、アプリケーション固有値は 0〜100 の横棒と値。ホバーで値と測った時刻)|
| (server) `Components/Telemetry/CountBadge.razor(.cs)` | 件数(1 以上は色付きのバッジ、0 は薄く)|
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
| (server) `Telemetry/SqliteTelemetryDbProvider.cs` + `TelemetryStorageOption.cs` | 端末ごとのファイルの接続(スキーマの確認と PRAGMA)、端末の一覧(ファイル名)、削除(プールを空けてから `-wal` / `-shm` も)|
| (server) `Telemetry/TelemetryStore.cs` | 端末ごとの書き込み(保存・保持期間の削除・ファイルの削除)のロックと保存済みの Id(ファイルを削除したら捨てる)、テレメトリのファイルがある端末の一覧 |
| (server) `Telemetry/TelemetryDeviceRegistry.cs` | 端末の登録とテレメトリの要約(ダッシュボード用のキャッシュ)、自動登録と管理の操作 |
| (server) `Telemetry/TelemetryBus.cs` + `TelemetryReceivedEventArgs.cs` | 受信と登録の変更の通知 |
| (server) `Assets/Data/TelemetrySchema.sql` / `Assets/Data/Schema.sql` | 端末ごとのファイルのスキーマ / `data.db` の `Device` |
| (server) `Endpoints/DeviceEndpoints.cs` | 端末からの登録の API |
| (server) `Workers/TelemetryRetentionWorker.cs` + `TelemetryRetentionWorkerOption.cs` | 保持期間の削除 |
| (server) `Components/Pages/DashboardPage.razor(.cs)` / `TelemetryPage.razor(.cs)` | 画面 |
| (server) `Components/Telemetry/*` / `Components/Dialogs/DeviceEditDialog.*` / `Components/RefreshTimer.cs` | 部品 |
| (core) `Services/TelemetryService.cs` | テレメトリの保存・照会・保持期間の削除(状態を持たない)。保存済みの Id(Resource = Hash、系列 = `TelemetrySeriesKey`)は呼び出し側から受け取り、新しく足した Resource と系列を結果で返す。保存済みの Id の照会(`QueryIdsAsync`。結果は `Models/Views/TelemetryIdView.cs`)。結果の `TelemetrySaveResult` と `TelemetrySeriesKey` は同じファイルの先頭 |
| (core) `Services/DeviceService.cs` | 端末の登録の照会・追加・更新・削除、自動登録、端末からの登録(`DataService` と同じ作り。結果は `DataWriteStatus`)|
| (core) `Accessors/TelemetryAccessor.cs` / `DeviceAccessor.cs` + `Accessors/Sql/*.sql` | SQL(テレメトリは接続・トランザクションを引数で受け取る。登録は既定の `IDbProvider`)。`[ExecuteConfig(typeof(DataProfile))]` |
| (core) `Accessors/GenericAccessor.cs` | スキーマの実行、`user_version` の読み取り、テレメトリの DB の PRAGMA |
| (core) `Accessors/DataProfile.cs` / `Infrastructure/Data/EnumTextConverter.cs` | 列挙型を名前の文字列で保存する |
| (core) `Infrastructure/Telemetry/ITelemetryDbProvider.cs` | 端末ごとの接続の取得(実装は server。core は SQLite を参照しない)|
| (core) `Models/Entity/DeviceEntity.cs` / `Telemetry*Entity.cs` | エンティティ(テーブル名は `[Name]`、キーは `[Key]`)|
| (core) `Models/Enums/Telemetry*.cs` / `Models/Parameters/TelemetryBatch.cs` / `TelemetryMetric.cs` | 列挙型、保存のまとまり |
| (core) `Domain/DeviceIdFormat.cs` / `Length.cs` | 端末 ID の形式と長さ |
| (app) `Services/HttpService.cs` / `Usecase/NetworkUsecase.cs` / `Modules/Network/NetworkMenuView.xaml` + `NetworkMenuViewModel.cs` | 端末からの登録の呼び出し(Network メニューの最後の Device registration。専用の画面は無い)|

## 🪜段階ごとの作業

### 🗄️1-3-1 保存

| ファイル | 変更 |
| --- | --- |
| (server) `Assets/Data/TelemetrySchema.sql` | 新規(「テレメトリ」のテーブルと索引、`journal_mode=WAL`、`user_version`)|
| (server) `Telemetry/SqliteTelemetryDbProvider.cs` + `TelemetryStorageOption.cs` / (core) `Infrastructure/Telemetry/ITelemetryDbProvider.cs` | 新規 |
| (core) `Models/Entity/Telemetry*Entity.cs` / `Models/Enums/Telemetry*.cs` / `Models/Parameters/TelemetryBatch.cs` + `TelemetryMetric.cs` / `Accessors/TelemetryAccessor.cs` + SQL / `Services/TelemetryService.cs` | 新規 |
| (core) `Accessors/GenericAccessor.cs` + SQL / `Accessors/DataProfile.cs` / `Infrastructure/Data/EnumTextConverter.cs` / `Domain/DeviceIdFormat.cs` / `Length.cs` / `GlobalUsing.cs` | PRAGMA と `user_version`、列挙型の変換、端末 ID の形式、`KeyAttribute` の別名 |
| (server) `Telemetry/OtlpMapper.cs` | 新規 |
| (server) `Telemetry/OtlpReceiver.cs` / `OtlpHttpEndpoints.cs` / `Otlp*Handler.cs` / `OtlpHelper.cs` / `Log.cs` | 非同期にして保存へ。拒否は `partial_success`、保存の失敗は 503 / `UNAVAILABLE`。OTLP/HTTP にサービスコンテキストのフィルター |
| (server) `Application/ApplicationExtensions.cs` / `appsettings.json` / `GlobalUsing.cs` | プロバイダーとオプション(`TelemetryStorage`)の登録、起動時にフォルダーを作る |
| (test) `Telemetry/OtlpMapperTests.cs` / `Services/TelemetryServiceTests.cs` / `Telemetry/TelemetryStoreTests.cs` / `TelemetryTestStorage.cs`(一時フォルダーの DB)/ `OtlpReceiverTests.cs` / `OtlpHttpEndpointsTests.cs` | 変換、保存と重複、保存済みの Id(同じ Resource と系列を足さない、ファイルの削除で捨てる)、拒否と 503 |

確認: 端末の実データ(メトリクス、Telemetry デモのスパンとログ)がファイルに入る、同じ内容の送り直しで増えない、識別できないリソースの拒否、保存できないときの 503。

完了(2026-09-27)。結果は `Change_Summary.md` の区間 17。

### 🔔1-3-2 端末の登録とキャッシュとバス

| ファイル | 変更 |
| --- | --- |
| (server) `Assets/Data/Schema.sql` / (core) `Models/Entity/DeviceEntity.cs` / `Accessors/DeviceAccessor.cs` + SQL / `Services/DeviceService.cs` | `Device` のテーブルと、登録の照会・追加(登録済みなら Duplicate)・更新(更新後の行)・削除 |
| (core) `Infrastructure/Data/DateTimeTextConverter.cs` / `Accessors/DataProfile.cs` | 日時を UTC の文字列で保存する |
| (core) `Models/Views/TelemetryDeviceSummaryView.cs` / `TelemetryLatestValueView.cs` / `TelemetryLogSummaryView.cs` / `Domain/TelemetrySeverity.cs` / `GlobalUsing.cs` | 端末の要約、重大度の区切り(ERROR = 17、FATAL = 21)|
| (core) `Services/TelemetryService.cs` / `Accessors/TelemetryAccessor.cs` + SQL | 端末の一覧(ファイル)、要約の照会(端末の情報、系列ごとの最後の点、1 時間ごとのエラーとクラッシュ、直近のエラー)、端末のファイルの削除 |
| (server) `Telemetry/TelemetryDeviceRegistry.cs` + `TelemetryDeviceSummary.cs` | 新規(「端末の登録とキャッシュ」)|
| (server) `Telemetry/TelemetryBus.cs` | 新規(「バス」。イベントの引数は同じファイルの先頭)|
| (server) `Telemetry/OtlpReceiver.cs` / `Log.cs` | 登録の確認(自動登録、無効の拒否)、保存の後に要約の更新と通知 |
| (server) `Components/RefreshTimer.cs` | 新規 |
| (server) `Application/ApplicationExtensions.cs` / `GlobalUsing.cs` | 登録、起動時のキャッシュの読み込み |
| (test) `Telemetry/TelemetryDeviceRegistryTests.cs` / `TelemetryBusTests.cs` / `Components/RefreshTimerTests.cs` / `Services/DeviceServiceTests.cs` / `Telemetry/OtlpReceiverTests.cs` / `TelemetryTestStorage.cs` | 自動登録と無効の拒否、要約の更新、起動時の読み込み(登録の無いファイル)、削除、受け手の例外、まとめての描画 |

確認: 新しい端末の自動登録、無効にした端末の拒否(端末は送り直さない)、サーバーを再起動してもキャッシュが戻る。

完了(2026-09-27)。結果は `Change_Summary.md` の区間 17。

### 🧹1-3-3 保持期間

| ファイル | 変更 |
| --- | --- |
| (server) `Workers/TelemetryRetentionWorker.cs` + `TelemetryRetentionWorkerOption.cs` / `Workers/Log.cs` | 新規。起動の直後と一定の間隔(既定 60 分)で、端末ごとに期限を過ぎた行を削除する。最後の受信から保持期間を過ぎた端末は、テレメトリのファイルを削除してキャッシュの要約を消す(登録は残す)。1 台の失敗はログに出して次の端末へ進む |
| (core) `Services/TelemetryService.cs` + `Accessors/TelemetryAccessor.cs` + SQL | 期限での削除(端末のロックの中、1 トランザクション。スパンはトレースの開始で判定してトレースの単位で消す)と、ファイルごとの削除。結果の `TelemetryDeleteResult` は同じファイルの先頭 |
| (server) `Application/ApplicationExtensions.cs` / `appsettings.json` | `TelemetryRetention`(`Enable`、`IntervalMinutes`、`LogDays` = 7、`TraceDays` = 7、`MetricDays` = 30、`DeviceDays` = 30)|
| (test) `Services/TelemetryServiceTests.cs` / `Workers/TelemetryRetentionWorkerTests.cs` | 期限での削除、ファイルの削除と登録の維持 |

確認: 時刻をずらしたテストデータでの削除。

完了(2026-09-27)。結果は `Change_Summary.md` の区間 17。

### 📲1-3-4 端末からの登録

| ファイル | 変更 |
| --- | --- |
| (server) `Endpoints/DeviceEndpoints.cs`(新規)/ `ApiRoutes.cs` / `Application/ApplicationExtensions.cs` | `PUT /api/device/{deviceId}`(本文は名前。匿名)。未登録なら登録して 201(`Location` 付き)、登録済みなら名前を更新して 200。応答は端末 ID・名前・グループ・有効・登録日時。端末 ID は受信と同じ形式、名前は必須で 50 文字以内(違反は 400)|
| (core) `Domain/DeviceIdFormat.cs` | 入力検証用の正規表現 `Pattern`(`IsValid` と同じ判定)|
| (core) `Services/DeviceService.cs` / `Accessors/DeviceAccessor.cs` + SQL | 名前だけの更新(`UpdateNameAsync`。更新後の行)|
| (server) `Telemetry/TelemetryDeviceRegistry.cs` | `RegisterAsync`(登録または名前の更新をキャッシュに反映して `DeviceChanged` を通知)|
| (app) `Services/HttpService.cs` / `Usecase/NetworkUsecase.cs` | `PutDeviceAsync`、`RegisterDeviceAsync`(登録して結果(新規か名前の更新か、名前・グループ・有効 / 無効)をダイアログで出す)|
| (app) `Modules/Network/NetworkMenuView.xaml` + `NetworkMenuViewModel.cs` / `Markup/AppIcons.cs` | Network メニューの最後に Device registration(`AppRegistration`)。端末 ID と端末の名前で登録する(API が未設定なら無効)|
| (test) `Domain/DeviceIdFormatTests.cs` / `Telemetry/TelemetryDeviceRegistryTests.cs` | 端末 ID の形式(正規表現と同じ判定)、登録と名前の更新 |

確認: 端末からの登録(自動登録済みの端末の名前の更新)、新しい端末 ID の登録(201)、不正な端末 ID と空の名前(400)。無効の端末の結果の表示は 1-4-1 の編集で無効にして確かめる。

完了(2026-09-27)。結果は `Change_Summary.md` の区間 17。

### 📋1-4-1 ダッシュボード

| ファイル | 変更 |
| --- | --- |
| (server) `Components/Pages/DashboardPage.razor(.cs)` | 新規(「ダッシュボード」。行の選択とエラーからの移動は 1-4-2 / 1-4-4)|
| (server) `Components/Dialogs/DeviceEditDialog.razor(.cs)` / `DeviceDialogExtensions.cs` | 新規(`DeviceFormValidator` の端末 ID は受信と同じ形式)|
| (server) `Components/Telemetry/Sparkline.razor(.cs)` / `MetricCell.razor(.cs)` / `CountBadge.razor(.cs)` | 新規 |
| (server) `Components/ViewHelper.cs` | テレメトリの表示と色 |
| (server) `Telemetry/TelemetryDeviceSummary.cs` / `TelemetryDeviceRegistry.cs` | 端末の状態(`TelemetryDeviceState`、`GetState`)|
| (core) `Domain/TelemetrySeverity.cs` | TRACE / DEBUG / INFO の区切り |
| (server) `Components/_Imports.razor` / `Components/Layout/NavMenu.razor` | `Components.Telemetry` のインポート、Dashboard を先頭(`/`)、Server(`/server`)を最後 |
| (server) `wwwroot/css/app.css` | ダッシュボード・状態の帯・横棒・スパークラインのクラス |
| (test) `Components/Pages/DashboardPageTests.cs` / `Components/Dialogs/DeviceFormValidatorTests.cs` / `Components/ViewHelperTests.cs` / `Components/Telemetry/MetricCellTests.cs` / `Components/Layout/NavMenuTests.cs` | 表示・検索・通知での読み直し、入力の検証、色と段の区切り、セルの表示、リンクの数(6 → 7)|

確認: 端末の実データ、端末の送信を止めると途絶に変わる、エラー・クラッシュの件数、追加・編集・無効・削除(端末からの登録の結果にも反映される)、端末からの名前の変更が表示中の画面に反映される。

完了(2026-09-27)。結果は `Change_Summary.md` の区間 17。

### 📈1-4-2 テレメトリ画面: 端末の選択とメトリクス

| ファイル | 変更 |
| --- | --- |
| (server) `Components/Pages/TelemetryPage.razor(.cs)` | 新規(見出し・タブ・メトリクス)|
| (server) `Components/Telemetry/TimeSeriesChart.razor(.cs)` / `MetricChartModel.cs` / `TelemetryRange.cs` | 新規 |
| (server) `Components/ViewHelper.cs` | グラフの値の表示(桁に合わせて小数を減らす)|
| (server) `Components/Layout/NavMenu.razor` / `Components/Pages/DashboardPage.razor(.cs)` | Dashboard の次に Telemetry(リンク 7 → 8)、ダッシュボードの端末の名前からテレメトリ画面へ |
| (server) `wwwroot/css/app.css` | テレメトリ画面とグラフのクラス |
| (core) `Services/TelemetryService.cs` / `Accessors/TelemetryAccessor.cs` + SQL / `Models/Views/TelemetryMetricHistoryView.cs` / `TelemetryMetricBucketView.cs` | 全系列と、範囲の点を系列ごと・間隔ごとに束ねた集計(`QueryMetricHistoryAsync` / `QueryMetricBucketListAsync`)|
| (test) `Components/Telemetry/MetricChartModelTests.cs` / `Components/Pages/TelemetryPageTests.cs` / `Services/TelemetryServiceTests.cs` / `Components/Layout/NavMenuTests.cs` | 種類ごとの値と単位、通知の点の追加、範囲外の削除、画面の表示と通知での追加、束ねの集計、リンクの数 |

確認: ダッシュボードから移動、表示中に受信した点が足され範囲から外れた点が消える、範囲の切り替え(URL と履歴)、24 時間以上の日付付きの目盛り、アプリを止めた区間で線が切れる。

完了(2026-09-27)。結果は `Change_Summary.md` の区間 17。

### 🧵1-4-3 テレメトリ画面: トレース

| ファイル | 変更 |
| --- | --- |
| (server) `Components/Telemetry/TraceWaterfall.razor(.cs)` / `WaterfallModel.cs` / `SpanDetail.razor(.cs)` / `AttributeTable.razor(.cs)` | 新規 |
| (server) `Components/ViewHelper.cs` / `wwwroot/css/app.css` | 所要時間の表示、トレースの一覧・ウォーターフォール・詳細のクラス |
| (server) `Components/Pages/TelemetryPage.razor(.cs)` | トレースのタブ(表示中のタブだけ読み込む形に)|
| (core) `Services/TelemetryService.cs` / `Accessors/TelemetryAccessor.cs` + SQL / `Models/Views/TelemetryTraceDetailView.cs` | トレースの一覧(`QueryTraceListAsync`。部分一致は `IDialect` の `Match`)、詳細(`QueryTraceDetailAsync`。読み取りのトランザクションでトレース・スパン・ログ・Resource)|
| (test) `Components/Telemetry/WaterfallModelTests.cs` / `Services/TelemetryServiceTests.cs` / `Components/Pages/TelemetryPageTests.cs` / `Components/ViewHelperTests.cs` / `Telemetry/TelemetryTestStorage.cs` | 木の順、親の欠け、開閉、イベント、絞り込みと詳細、URL のクエリで開くトレース、所要時間の表示 |

確認: 端末の Telemetry デモのスパン(親子)とログ、表示中に新しいトレースが一覧に出る、開閉とスパンの選択、エラーだけ・名前の絞り込み。

完了(2026-09-27)。結果は `Change_Summary.md` の区間 17。

### 📜1-4-4 テレメトリ画面: ログ

| ファイル | 変更 |
| --- | --- |
| (server) `Components/Pages/TelemetryPage.razor(.cs)` | ログのタブ |
| (server) `Components/Telemetry/LogDetail.razor(.cs)` / `TelemetryLogLevel.cs` / `TelemetryLinks.cs` / `TelemetryRange.cs` | 新規(ログの詳細、重大度、URL)、経過時間を含む範囲 |
| (server) `Components/Pages/DashboardPage.razor(.cs)` / `wwwroot/css/app.css` | 直近のエラーの行からその端末のログ(ERROR 以上、エラーの時刻を含む範囲)へ、ログのクラス |
| (core) `Services/TelemetryService.cs` / `Accessors/TelemetryAccessor.cs` + SQL / `Models/Parameters/TelemetryLogQuery.cs` | 絞り込みと続きの読み込み(`QueryLogListAsync`。時刻と Id の続きから)、Resource(`QueryResourceAsync`)|
| (test) `Components/Pages/TelemetryPageTests.cs` / `DashboardPageTests.cs` / `Services/TelemetryServiceTests.cs` | 重大度の絞り込み・行を開く・通知の追加、エラーからの移動、絞り込みと続きの読み込み |

確認: 警告・エラー・クラッシュのログ、トレースへの移動、表示中の追加、ダッシュボードのエラーからの移動、トレースでの絞り込み。

完了(2026-09-27)。結果は `Change_Summary.md` の区間 17。

## 🚫対象外

| 項目 | 内容 |
| --- | --- |
| 端末をまたぐ検索 | トレース ID・ログの本文は、端末を選んでから探す |
| 分位 | ヒストグラムは平均・最大・回数だけ(バケットは保存する)|
| 累積値 | Cumulative の点は差分にせず、そのまま表示する |
| グラフの操作 | 拡大・移動・ドラッグでの範囲の選択は無い(範囲は選択肢で切り替える)|
| 受信 | OTLP/JSON、認証、流量の制御(429)は無い |
| その他 | JSON の API、ダミーデータの投入画面は無い |
