# ✅残作業チェックリスト

残作業(実機確認 / 実テスト / 保留)のマスターチェックリスト。経緯・実装内容・ナレッジ・開発ポリシーは `Change_Summary.md`(付録含む)を参照。
優先順 = 2 節(バックグラウンドタスク / タンキング送信)→ 3 節(プッシュ通知)→ 4 節(生体認証)→ 7 節(App のミニアプリ)。6 節(保留中の判断)はユーザーの決定待ち。小さな項目は「取り込み候補」の章にまとめ(現在は無し)、大きな項目は章を分けている。

## 📋サマリ

| Category | Feature | 章 |
| --- | --- | --- |
| Device | Background task(WorkManager) | 2-1 |
| Network | Tanking send(端末で溜めて、通信できるときにまとめて送る) | 2-2 |
| Device | Push(FCM) | 3-2 |
| Device | Biometric(生体認証) | 4 |
| Decision | 保留中の判断(CoreCLR の扱い) | 6 |
| App | Timer / ToDo / 2048 / Minesweeper | 7-1〜7-4 |
| UI | Weather(ダミーのデータ。採用は判断待ち) | 7-5 |
| Control | Tab(ニュースのダミーのデータ。採用は判断待ち) | 7-6 |

## 📏運用ルール

- 作業はこの番号で指示・進行する(例:「2-1 を実施」)。完了した項目は本書から削除し、内容は `Change_Summary.md` に記録する
- **⚖️【判断】印の項目はユーザーが決定**(勝手に進めない)。デザイン判断を伴う差分は 1 項目ずつ指示を受けて実施
- 実装・変更を行なう場合の完了条件 = **ビルド警告ゼロ** + `Change_Summary.md` への記録(開発ポリシーは同 付録A)
- コミットはユーザーが実施(グループ単位を推奨)
- `README.md` の TODO 表は本書のサマリ表(2〜7 節)と同期させる(項目の追加・削除・完了時に両方を更新。TODO 表に本書の番号は書かない)
- リンク集 `■MAUI.txd` は全件に判定を付記済み(🟩 取り込む / 🟦 取り込まないが記事として有用 / 🟥 古い・参照不要 / 🟨 要判断)。🟩 の項目は本書へ移し、元行は同書から削除する
- 描画・性能の計測は **Release ビルド + 実機**(手順は `Development.md` の「Releaseビルドでの検証と計測」)

## 🧭前提(環境)

- **環境制約 (不具合ではない)**: ①地図タイルは Google Maps API キー未設定だと非表示 (ピン・カメラ移動は動作) ②Sample > CV Net は AI エンドポイント未設定だと画面に入れない ③CommunityToolkit CameraView の `CaptureAsync` がまれに未完了になる(5 秒で打ち切って「撮影できませんでした」を出し、プレビューのまま続行できる)
- 現在実機に入っているのは **Debug ビルド**(2026-09-23 デプロイ。Mono ランタイム。性能・描画の確認時は Release へ入れ替える)

---

## ⏰2. バックグラウンドタスク(WorkManager / タンキング送信)

遅延可で再起動後も残る処理を `WorkManager` に載せる(2-1)。端末で作ったデータを溜めておき、通信できるときにまとめてサーバーへ送る(2-2)。ファイルパスは `Template.MobileApp/` からの相対。

### ⏰2-1 バックグラウンド定期タスク(WorkManager)

参照: https://www.nuget.org/packages/Shaunebu.MAUI.BackgroundTaskManager — Android は `WorkManager`(最短 15 分間隔)、iOS は `BGTaskScheduler` を使い、CRON 式でフォアグラウンド / バックグラウンドのジョブを登録して `Preferences` に永続化する薄いラッパー(`RegisterJob<T>()` / `Schedule(jobId, cron, callback)` / `ScheduleInBackground(jobId, cron, jobType)`。2025-10、230 DL、リポジトリ公開なし)。採用はせず API の形(ジョブ登録 / CRON 近似 / 永続化)だけ参考にし、`AndroidX.Work` を直接使う。

範囲: 制約付き(ネットワーク接続時 / 充電中)の一回限りワーク + 15 分周期の定期ワーク。実行結果はローカル通知(`Components/NotificationService`)か画面のログに出す。正確な時刻指定は `AlarmManager`(通知で実装済み)の担当のままとし、WorkManager は遅延可・再起動後も残る処理に限定する。

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Template.MobileApp.csproj` | パッケージ | `Xamarin.AndroidX.Work.Runtime` を追加(`dotnet list package --include-transitive` で `Fragment.Ktx` ピン止めとの競合を確認) |
| `Components/BackgroundTask.cs` + `.android.cs` | — | 新規。登録(一回 / 定期)/ 取消 / 状態取得。`AndroidX.Work.Worker` 派生の `DoWork()` で処理 |
| `Modules/Device/DeviceMiscView.xaml` + `DeviceMiscViewModel.cs` | 雑多なデバイス機能(Device メニューは満杯) | `InfoCard` を追加(登録 / 取消 / 最終実行時刻) |
| `MauiProgram.cs` | `ConfigureComponents` | DI 登録 |

- [ ] **2-1-0**⚖️【判断】要否 — 再起動後も残る遅延処理(同期 / 送信キュー)の需要があるか。WorkManager は再起動後に自動で再スケジュールされるため `RECEIVE_BOOT_COMPLETED` は不要。常駐(前景サービス)は対象外

### 📦2-2 タンキング送信

端末で作ったデータを端末の DB に溜めておき、通信できるときにまとめてサーバーへ送る(圏外でも記録を続けられる)。対向(template-maui-server)の受け口も要る。差分同期・競合解決は扱わない。

- [ ] **2-2-0**⚖️【判断】範囲 — 送るデータ(サンプルの記録の形)、送るきっかけ(通信の回復 / 手動 / 2-1 の WorkManager のネットワーク条件付きワーク)、再送で重複させない仕組み(端末が Id を決め、サーバーは同じ Id を受け流す)、送った分の扱い(削除 / 送信済みとして残す)、未送信の件数の見せ方

## 🔔3. プッシュ通知

ローカル通知は `Components/NotificationService.cs` + `.android.cs` で実装済み(即時 / スケジュール / アクションボタン / タップ時ペイロード。Device > Misc の Notification カード)。サーバーからの通知は、FCM を使わない独自の仕組み(SignalR。アプリを開いている間に受け取る。`Push_Plan.md`)を実装済みで、残りは FCM(3-2)。

### ☁️3-2 FCM

参照: `0_maui-samples/10.0/WebServices/PushNotificationsDemo`(`Xamarin.Firebase.Messaging` + 自前 `FirebaseMessagingService`)。

| 種別 | URL | 概要 | 適用先 |
| --- | --- | --- | --- |
| 記事 | https://www.andreasnesheim.no/push-notifications-in-net-maui-with-firebase/ | FCM を `Plugin.Firebase` で扱う手順。Firebase Console 登録 → `google-services.json` 配置 → `MauiProgram` 初期化 → `CrossFirebaseCloudMessaging.Current.GetTokenAsync()` でトークン取得 → Console からテスト送信(2022-09) | 採用時の方式候補①(`Plugin.Firebase`)。方式候補②は上記 PushNotificationsDemo |
| ライブラリ | https://github.com/Gekidoku/BetterFireBaseNotificationsPlugin | `CrossFirebasePlugin` の MAUI 移植。データ付き通知 / アクションボタン / サイレント通知 / アプリ終了時の受信に対応。NuGet 配布はなくプロジェクト参照前提(2026-01) | 実装範囲(データ / アクション / サイレント)のチェックリスト |

- [ ] **3-2-0**⚖️【判断】プッシュ通知(FCM)の要否 — Firebase プロジェクトと `google-services.json` が前提。採用する場合、トークン表示と受信ログを Notification カードに追記する

## ⏳外部待ち(README の Pending と同期)

| 項目 | 待ち先 | 状態 |
| --- | --- | --- |
| XAML の global xmlns(`http://schemas.microsoft.com/dotnet/maui/global`) | ReSharper の対応(ビルドは通るが inspectcode が解決できない) | 保留。名前の衝突と範囲は `Change_Summary.md` 区間 16 |
| CoreCLR ランタイム(`UseMonoRuntime=false`) | .NET 11(Shiny の `[Export]` で起動クラッシュ: dotnet/android#10996) | csproj は CoreCLR のまま、実機検証は `-p:UseMonoRuntime=true` の Mono ビルド |

## 🔐4. 生体認証

画面・`ViewId`・メニューボタンが配置済み(`DeviceMenuView.xaml` の該当ボタンが `IsEnabled="False"`、画面は `Not implemented` 表示、ViewModel は 8 行)。本人確認と、生体認証で守る秘密(Android Keystore の鍵で暗号化して保存し、生体認証で復号して取り出す)を作る。計画は `Biometric_Plan.md`。

- [ ] **4-1** 本人確認(使えるかどうかの 3 区分の表示・認証・結果の表示)
- [ ] **4-2** 生体認証で守る秘密(鍵の作成・暗号化して保存・生体認証で復号して表示・削除・鍵が使えなくなったときの作り直し)

## ⏸️6. 保留中の判断

### ⚙️6-2 CoreCLR ランタイムの扱い

csproj は `UseMonoRuntime=false`(CoreCLR)。Shiny の `[Export]` ライフサイクルコールバックで起動時にクラッシュする(dotnet/android#10996、.NET 11 で修正)。「外部待ち」の表と README の Pending に記載。

- [ ] **6-2-0**⚖️【判断】.NET 11 まで CoreCLR のままにする(実機検証は `-p:UseMonoRuntime=true` の Mono ビルド)か、csproj を Mono に戻すか

## 🧩7. App のミニアプリ

App のメニューに、アプリ・画面・モデルの実装の見本になる小さなアプリを足す。あわせて、ダミーのデータで既存の分類を強化する天気(UI)とニュース(Control のタブ)を検討する。計画は `App_Plan.md`。

- [ ] **7-1** タイマー(ストップウォッチとカウントダウン。背面やアプリの終了をまたいでも合う時間、終了の通知)
- [ ] **7-2** ToDo(一覧と、Push / Pop で開くダイアログ的な編集。SQLite(既存の `data.db`)への保存)
- [ ] **7-3** 2048(スワイプ、モデルの結果から作るタイルのアニメーション、途中の盤面の保存)
- [ ] **7-4** マインスイーパー(描画で作る盤面、長押しで旗、最初の 1 手を安全にする配置)
- [ ] **7-5-0**⚖️【判断】天気(UI)の採用 — ダミーのデータで、横の一覧をまたぐ気温の折れ線、週の範囲に合わせたバー、日の出・日の入りの弧を見せる画面(UI 2 のメニューの空き)
- [ ] **7-5** 天気(UI)
- [ ] **7-6-0**⚖️【判断】ニュース(Control のタブ)の採用 — ダミーのニュースで、`SfTabView` と自作のタブ(見出し + `CarouselView`)を比べる画面と、詳細から戻ったときのタブと一覧の位置の保持(Control のメニューの Refresh の隣)
- [ ] **7-6** ニュース(Control のタブ)
