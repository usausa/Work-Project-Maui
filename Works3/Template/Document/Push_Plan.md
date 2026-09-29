# 🔔独自プッシュの実装計画(サーバーから端末への通知)

template-maui-server から端末へ通知を送る(FCM を使わない)。端末はアプリを開いている間だけ SignalR で繋ぎ、受けた通知を邪魔にならない形(トースト)で出す。接続していない端末宛ての通知(アプリを閉じている間に送ったものを含む)はサーバーが溜めておき、端末が接続したときに届ける。番号は `Task_Checklist.md` の 3-1 と同じ。

| 番号 | 段階 | 内容 |
| --- | --- | --- |
| 3-1-1 | サーバー: 保存と配信 | 通知のテーブル(端末ごとの未達)、ハブ(端末 ID での接続、未達の送信、受け取りの応答)、送信の API |
| 3-1-2 | サーバー: 送信の画面 | ダッシュボードから端末 / 全端末へ送る、端末ごとの未達の件数 |
| 3-1-3 | 端末: 受信と表示 | アプリを開いている間の接続(設定画面のスイッチでオン / オフ、切れたら繋ぎ直す)、トースト、受け取りの応答 |

ファイルパスは、(server) が `template-maui-server/src/Template.MobileServer.Web/`、(core) が `template-maui-server/src/Template.MobileServer.Core/`、(app) が `Template.MobileApp/` からの相対。

## ⚖️決定事項

| 項目 | 決定 |
| --- | --- |
| 方式 | SignalR の接続でサーバーから送る(FCM は使わない。FCM は 3-2)|
| 端末での表示 | 邪魔にならない形。トースト(「件名: 本文」の 1 文)。操作を止めるダイアログは使わない |
| 未接続の端末 | サーバーが通知を溜めておき、端末が接続したときに届ける |
| ハブ | 通知専用の `PushHub`(`/hubs/push`)を足す(Network > Realtime の `MonitorHub` とは分ける)|
| 接続する期間 | アプリを開いている間(前面)だけ。背面に回ると切り、前面に戻ると繋ぐ。閉じている間に送った通知は、次に開いて繋いだときに届く(前景サービス・常駐の通知・電池の最適化からの除外は要らない)|
| 宛先 | 端末ごと(登録済みの端末 ID)と全端末のどちらも選べる(全端末は送った時点の登録済みで有効な端末へ 1 件ずつ)|
| 溜める期間 | 7 日(オプション)。期限を過ぎた行は届いた / 届いていないにかかわらず消す |
| 受け取りのオン / オフ | 設定画面(Main > Setting)の Push のスイッチ。オンの間、アプリを開いているときに繋ぐ |
| 端末の画面 | 受信用の画面は作らない。接続の状態はヘッダー(Diagnostic の左)に出す |
| 端末の置き場所 | 端末側は `Services/` に置く。接続は `PushConnection`、繋ぐ・切るとトースト・応答は `PushService`。接続の状態は `PushConnection` が列挙 `PushStatus`(Stopped / Connecting / Connected / Retrying)で画面に知らせる |

## 🗄️データ(`data.db`)

| 列 | 内容 |
| --- | --- |
| `Id` | 連番 |
| `DeviceId` | 宛先の端末 ID |
| `Title` / `Body` | 件名・本文 |
| `CreatedAt` | 送った日時 |
| `DeliveredAt` | 端末が受け取りを応答した日時(未達は NULL)|

- 全端末宛ては、送った時点の登録済みで有効な端末ごとに 1 行ずつ作る(端末ごとに届いたかを持つ)。端末を指定した送信も、登録済みで有効な端末だけ
- 期限を過ぎた行は保持期間の処理で消す。端末の登録を削除すると、その端末宛ての行も消す
- 件名は 100 文字、本文は 500 文字まで(`Length.Title` / `Length.Body`)
- 索引: 未達の照会と件数は `IX_PushMessage_DeviceId`(未達の行だけ)、期限の削除は `IX_PushMessage_CreatedAt`。`Id` は `AUTOINCREMENT`(消した行の番号を使い回さない)

## 🔁配信の流れ

1. 送信(API / 画面)で行を作り、宛先の端末が接続中ならすぐにハブで送る
2. 端末は接続の URL のクエリ(`deviceId`)で端末 ID を渡す(`DeviceIdFormat` で確かめ、違えば切る)。ハブは接続を端末 ID のグループに入れてから、未達の行を古い順に送る(入れてから読むので、その間に送った通知も落とさない。重なった分は端末が捨てる)
3. 端末は表示した後に `Acknowledge(id)` を呼び、サーバーは `DeliveredAt` を記録する
4. 応答が無いまま切れた通知は、次の接続で送り直す(端末は同じ Id を 2 回出さない)

## 📱端末

| 項目 | 内容 |
| --- | --- |
| 接続 | `Services/PushService` が `Services/PushConnection` で繋ぐ(`Mofucat.ReactiveHub` で繋ぎ直す。通信の状態が戻ったら待たずに繋ぎ直す)。接続先は API の接続先(`ApiContext.BaseAddress`)、端末 ID(`DeviceInformation.DeviceId`)は URL のクエリ |
| 繋ぐ・切る | Push のスイッチがオンなら、起動の完了(`MainPageViewModel.OnCreated`)と前面に戻ったとき(`OnResumed`)に繋ぎ、背面に回ったとき(`OnStopped`)と画面を破棄するとき(`OnDestroying`)に切る。スイッチの切り替えですぐに繋ぐ / 切る。QR で接続先が変わると繋ぎ直す。接続先が未設定なら繋がない |
| 表示 | `IDialog.Toast`(CommunityToolkit.Maui。「件名: 本文」の 1 文)。受信は UI スレッドで受ける(繋ぐときの `ObserveOnCurrentContext`)|
| ヘッダー | Diagnostic(📈)の左に接続の状態(↔️ 接続中 / 🔄 接続の途中 / ⛔ 失敗して繋ぎ直している)。止まっている間は出さない。VM は `PushStatus` を持ち、絵文字は XAML の `s:MapToTextConverter`、表示の切り替えは `s:CompareToBool` |
| 応答 | 表示の後に `Acknowledge(id)`(`TrySendAsync`。受信のハンドラーの中で呼び出しの完了を待たない)。直近に出した 64 件の Id を覚えておき、送り直された通知は出さずに応答だけ返す |

## 🏗️構成

| ファイル | 役割 |
| --- | --- |
| (server) `Hubs/PushHub.cs` + `HubRoutes.cs` | 接続(クエリ `deviceId` の端末 ID のグループ)、未達の送信、受け取りの応答(`Acknowledge`)。メッセージは `PushMessage`(`Id` / `Title` / `Body` / `CreatedAt`)|
| (server) `Services/PushNotifier.cs` | 送信(行を作ってから、宛先の端末が接続中ならハブで送る)と受け取りの記録。未達が変わったら `Changed` で知らせる |
| (server) `Endpoints/PushEndpoints.cs` | 送信の API(`POST /api/push`。宛先の端末 ID(省略で全端末)、件名、本文。送った件数を返し、宛先の端末が無ければ 404)|
| (server) `Components/Pages/DashboardPage.razor(.cs)` + `Components/Dialogs/PushSendDialog.razor(.cs)` | 端末の行と全端末からの送信(件名と本文のダイアログ)、端末ごとの未達の件数 |
| (server) `Workers/PushRetentionWorker.cs` + `PushRetentionWorkerOption.cs` | 期限を過ぎた行の削除(`PushRetention`。60 分ごと、7 日)|
| (core) `Services/PushService.cs` | 行の作成(宛先の登録済みで有効な端末ごと)、未達の照会、端末ごとの未達の件数、受け取りの記録、期限の削除 |
| (core) `Accessors/PushAccessor.cs` + SQL / `Models/Entity/PushMessageEntity.cs` / (server) `Assets/Data/Schema.sql` | テーブルと SQL |
| (core) `Services/DeviceService.cs` | 端末の登録の削除で、その端末宛ての通知も消す |
| (app) `Services/PushConnection.cs` / `Log.cs`、`MauiProgram.cs` | 接続と受信、接続の状態(`PushStatus`)、出した Id の記憶、応答、ログ、DI の登録 |
| (app) `Services/PushService.cs` | 繋ぐ・切る(接続先が変わっていれば繋ぎ直す)、受けた通知のトーストと応答 |
| (app) `MainPage.xaml` + `MainPageViewModel.cs` / `Resources/Styles/Styles.xaml` | 起動・前面・背面での繋ぐ・切る、ヘッダーの接続の状態(絵文字と表示の切り替えは XAML のコンバーター)|
| (app) `State/Settings.cs` / `Modules/Main/SettingView.xaml` + `SettingViewModel.cs` | Push のスイッチ(設定に保存)|

## 🪜段階ごとの作業

### 🗄️3-1-1 サーバー: 保存と配信

| ファイル | 変更 |
| --- | --- |
| (server) `Assets/Data/Schema.sql` | `PushMessage` の表と索引 |
| (core) `Models/Entity/PushMessageEntity.cs` / `Accessors/PushAccessor.cs` + SQL / `Services/PushService.cs` | 新規 |
| (core) `Services/DeviceService.cs` / `Domain/Length.cs` | 端末の登録の削除で通知も消す、件名と本文の長さ |
| (server) `Hubs/PushHub.cs` / `Services/PushNotifier.cs` / `Endpoints/PushEndpoints.cs` / `Workers/PushRetentionWorker.cs` + `PushRetentionWorkerOption.cs` | 新規 |
| (server) `Hubs/HubRoutes.cs` / `Log.cs`、`Endpoints/ApiRoutes.cs`、`Services/Log.cs`、`Workers/Log.cs`、`Application/ApplicationExtensions.cs` / `appsettings.json` | 経路・ログ・登録(`PushRetention`)|
| (test) `Services/PushServiceTests.cs` / `PushNotifierTests.cs` / `DeviceServiceTests.cs`、`Hubs/PushHubTests.cs`、`Workers/PushRetentionWorkerTests.cs`、`Telemetry/TelemetryTestStorage.cs` | 宛先(登録済みで有効な端末だけ)、未達と応答、期限の削除、接続での未達の送信と形式の違う端末 ID の切断、グループへの送信、端末の削除 |

確認: 接続中の端末に届く、未接続の間に送った通知が接続したときに届く、応答の無いまま切れた通知が次の接続で届き直す、応答で届いた日時が入る、入力の誤り(400)と宛先の端末が無い(404)、形式の違う端末 ID の接続を切る(PC の SignalR クライアントで確かめた)。

完了(2026-09-28)。結果は `Change_Summary.md` の区間 17。

### 🖥️3-1-2 サーバー: 送信の画面

| ファイル | 変更 |
| --- | --- |
| (server) `Components/Pages/DashboardPage.razor(.cs)` / `wwwroot/css/app.css` | 端末の行に送信のボタン(無効の端末は押せない)、検索と追加の行に「全端末へ通知」(有効な端末が無ければ押せない)、「未達」の列。未達の件数は `data.db` から読み、`PushNotifier.Changed` と一定の間隔で読み直す。表と検索・追加の行の上限幅を 1360px に |
| (server) `Components/Dialogs/PushSendDialog.razor(.cs)` + `PushDialogExtensions.cs` | 新規。件名(必須、100 文字)と本文(500 文字)の入力 |
| (server) `Services/PushNotifier.cs` / `Hubs/PushHub.cs` | `Changed`(行を作ったときと届いた日時を記録したとき)、受け取りの記録 `AcknowledgeAsync`(ハブの `Acknowledge` から)|
| (core) `Services/PushService.cs` / `Accessors/PushAccessor.cs` + SQL / `Models/Views/PushPendingSummaryView.cs` | 端末ごとの未達の件数 `QueryPendingSummaryAsync` |
| (test) `Components/Pages/DashboardPageTests.cs` / `Components/Dialogs/PushFormValidatorTests.cs` / `Services/PushNotifierTests.cs` / `Hubs/PushHubTests.cs` | 未達の件数と応答での読み直し、無効の端末の送信のボタン、入力の検証、`Changed` |

確認: 端末の行からのダイアログで入力の検証が効き、送ると未達が 1 になる。全端末へ送ると「1 台へ送信しました。」。PC の SignalR クライアントを端末として繋ぐと未達の 2 件が古い順に届き、応答で画面の未達が読み込み直さずに 0 に戻る。

完了(2026-09-28)。結果は `Change_Summary.md` の区間 17。

### 📱3-1-3 端末: 受信と表示

| ファイル | 変更 |
| --- | --- |
| (app) `Services/PushConnection.cs` / `Log.cs` | 新規(`Log.cs` は追記)。接続(`hubs/push?deviceId=`)、受信(`Receive`)、接続の状態(`PushStatus`)、出した Id の記憶(直近 64 件)、応答(`Acknowledge`)、独自プッシュのログ |
| (app) `Services/PushService.cs` | 新規。繋ぐ・切る、受けた通知のトーストと応答 |
| (app) `MainPageViewModel.cs` | 起動の完了と前面に戻ったときに繋ぎ、背面に回ったときに切る |
| (app) `State/Settings.cs` / `Modules/Main/SettingView.xaml` + `SettingViewModel.cs` / `MauiProgram.cs` | `PushEnabled`、Push のスイッチ、DI の登録 |

確認: 前面で受けるとトースト。背面に回ると切れ、その間に送った通知は前面に戻って繋いだときにトーストで出る。スイッチのオフで切れ、オンで繋ぐ。アプリからの通知(常駐の通知)は出ない。

完了(2026-09-29)。結果は `Change_Summary.md` の区間 17。

## 🚫対象外

| 項目 | 内容 |
| --- | --- |
| アプリを閉じている間の受信 | 前景サービスで接続を保つ作りは `Works3/PushSample`(README に完結)。FCM は 3-2 |
| 認証 | 端末 ID をそのまま信じる(端末の登録の API と同じ)|
| 大量の端末 | スケールアウト(Redis のバックプレーンなど)は扱わない |
