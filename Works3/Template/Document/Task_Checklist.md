# ✅残作業チェックリスト

残作業(実機確認 / 実テスト / 保留)のマスターチェックリスト。経緯・実装内容・ナレッジ・開発ポリシーは `Change_Summary.md`(付録含む)を参照。
優先順 = 2 節(バックグラウンドタスク / タンキング送信)→ 3 節(プッシュ通知)→ 4 節(生体認証)。6 節(保留中の判断)はユーザーの決定待ち。8 節(コマンドの受け付けの見直し)は進行中。小さな項目は「取り込み候補」の章にまとめ(現在は無し)、大きな項目は章を分けている。

## 📋サマリ

| Category | Feature | 章 |
| --- | --- | --- |
| Device | Background task(WorkManager) | 2-1 |
| Network | Tanking send(端末で溜めて、通信できるときにまとめて送る) | 2-2 |
| Device | Push(FCM) | 3-2 |
| Device | Biometric(生体認証) | 4-1〜4-3、4-5 |
| Device | Passkey(パスキー) | 4-4 |
| Decision | 保留中の判断(CoreCLR の扱い) | 6 |
| Decision | コマンドの受け付けの見直し(遷移でのアクティブのタイミング、Simple の使い分け) | 8 |

## 📏運用ルール

- 作業はこの番号で指示・進行する(例:「2-1 を実施」)。完了した項目は本書から削除し、内容は `Change_Summary.md` に記録する
- **⚖️【判断】印の項目はユーザーが決定**(勝手に進めない)。デザイン判断を伴う差分は 1 項目ずつ指示を受けて実施
- 実装・変更を行なう場合の完了条件 = **ビルド警告ゼロ** + `Change_Summary.md` への記録(開発ポリシーは同 付録A)
- コミットはユーザーが実施(グループ単位を推奨)
- `README.md` の TODO 表は本書のサマリ表(2〜6 節)と同期させる(項目の追加・削除・完了時に両方を更新。TODO 表に本書の番号は書かない)
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

Device > Biometric に、本人確認、生体認証で守る秘密(Android Keystore の鍵で暗号化して保存し、生体認証で復号して取り出す)、鍵での署名(サーバーでの認証の流れを端末の中で疑似的に確かめる)を作る。Android 本体の `BiometricPrompt` / `BiometricManager` を直接使う。パスキーは計画だけ(4-4)。計画は `Biometric_Plan.md`。

- [ ] **4-1** 本人確認(使えるかどうかの 3 区分の表示・認証・結果の表示)— 実装済み。残りは画面ロックと指紋を登録した端末での確認(成功・キャンセル・失敗回数の超過・PIN。登録と認証は利用者の操作)
- [ ] **4-2** 生体認証で守る秘密(鍵の作成・暗号化して保存・生体認証で復号して表示・削除・鍵が使えなくなったときの作り直し)— 画面ロックの無い端末では鍵を作れないため、画面ロックと指紋を登録した端末が用意できるまで保留
- [ ] **4-3** 鍵での署名(サーバー認証の疑似。署名の鍵の作成と公開鍵の登録・チャレンジへの生体認証付きの署名・端末の中での検証・改ざんと使い回しの検出・鍵の削除と作り直し)— 実装済み。残りは画面ロックと指紋を登録した端末での確認(登録と署名の成功・キャンセル・生体情報の登録を足したときの無効化。登録と認証は利用者の操作)
- [ ] **4-5** 本人確認と鍵の追加(ダイアログのサブタイトルと説明・弱い生体認証・結果の方式・端末に合う文言・最後の本人確認からの時間・登録の画面・署名の鍵の StrongBox と置き場所の表示)— 実装済み。残りは実機での画面の確認と、画面ロックと指紋を登録した端末での確認

### 🔑4-4 パスキー

Credential Manager(`androidx.credentials`)でパスキーを作ってログインする。前提と流れは `Biometric_Plan.md` の 4-4。

- [ ] **4-4-0**⚖️【判断】パスキー(Credential Manager + WebAuthn)の要否 — 当面対応しない。サーバーの WebAuthn の API(template-maui-server)、アプリとドメインの関連付け(`assetlinks.json`)、`Xamarin.AndroidX.Credentials` のダウンロードが前提

## ⏸️6. 保留中の判断

### ⚙️6-2 CoreCLR ランタイムの扱い

csproj は `UseMonoRuntime=false`(CoreCLR)。Shiny の `[Export]` ライフサイクルコールバックで起動時にクラッシュする(dotnet/android#10996、.NET 11 で修正)。「外部待ち」の表と README の Pending に記載。

- [ ] **6-2-0**⚖️【判断】.NET 11 まで CoreCLR のままにする(実機検証は `-p:UseMonoRuntime=true` の Mono ビルド)か、csproj を Mono に戻すか

## 🚦8. コマンドの受け付けの見直し

遷移でアクティブ・非アクティブにするタイミングと、コントロールからの通知のコマンド(`CommandMode.Simple`)の使い分けを見直す。方針(Simple もアクティブを見る、遷移イベントのときはアクティブ)は Smart.Navigation 3.15.0 と Smart.Maui 2.31.0 で入れ、実機で確かめた。遷移の順番・確認の結果・Simple の一覧と理由・未確認は `Command_Plan.md`。

- [ ] **8-5** Simple の一覧と理由の更新(見直しが終わるまで、コマンドを変えるたびに)
