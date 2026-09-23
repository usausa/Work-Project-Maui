# ✅残作業チェックリスト

残作業(実機確認 / 実テスト / 保留)のマスターチェックリスト。経緯・実装内容・ナレッジ・開発ポリシーは `Change_Summary.md`(付録含む)を参照。
優先順 = 1 節(OpenTelemetry)→ 2 節(バックグラウンドタスク / オフライン同期)→ 3 節(プッシュ通知)→ 4 節(生体認証)→ 5 節(見た目の属性の残り)。6 節(保留中の判断)はユーザーの決定待ち。小さな項目は「取り込み候補」の章にまとめ(現在は無し)、大きな項目は章を分けている。

## 📋サマリ

| Category | Feature | 章 |
| --- | --- | --- |
| Diagnostics | OpenTelemetry(クラッシュレポート / テレメトリ基盤の組み込み) | 1-2-1〜1-4 |
| Device | Background task(WorkManager) | 2-1 |
| Network | Offline sync(未送信キュー・差分同期・競合解決) | 2-2 |
| Device | Push(FCM) | 3-2 |
| Device | Biometric(生体認証) | 4 |
| UI | 見た目の属性の残り(配置・サイズ・色・文字)をスタイルへ | 5 |
| Decision | 保留中の判断(通信設定が未設定のときの止め方 / CoreCLR の扱い) | 6 |

## 📏運用ルール

- 作業はこの番号で指示・進行する(例:「5-1 を実施」)。完了した項目は本書から削除し、内容は `Change_Summary.md` に記録する
- **⚖️【判断】印の項目はユーザーが決定**(勝手に進めない)。デザイン判断を伴う差分は 1 項目ずつ指示を受けて実施
- 実装・変更を行なう場合の完了条件 = **ビルド警告ゼロ** + `Change_Summary.md` への記録(開発ポリシーは同 付録A)
- コミットはユーザーが実施(グループ単位を推奨)
- `README.md` の TODO 表は本書のサマリ表(1〜6 節)と同期させる(項目の追加・削除・完了時に両方を更新。TODO 表に本書の番号は書かない)
- リンク集 `■MAUI.txd` は全件に判定を付記済み(🟩 取り込む / 🟦 取り込まないが記事として有用 / 🟥 古い・参照不要 / 🟨 要判断)。🟩 の項目は本書へ移し、元行は同書から削除する
- 描画・性能の計測は **Release ビルド + 実機**(手順は `Development.md` の「Releaseビルドでの検証と計測」)

## 🧭前提(環境)

- **環境制約 (不具合ではない)**: ①地図タイルは Google Maps API キー未設定だと非表示 (ピン・カメラ移動は動作) ②Sample > CV Net は AI エンドポイント未設定だと画面に入れない ③CommunityToolkit CameraView の `CaptureAsync` がまれに未完了になる(5 秒で打ち切って「撮影できませんでした」を出し、プレビューのまま続行できる)
- 現在実機に入っているのは **Debug ビルド**(2026-09-23 デプロイ。CoreCLR の起動クラッシュを避けるため `-p:UseMonoRuntime=true` の Mono ビルド。性能・描画の確認時は Release へ入れ替える)

---

## 📈1. OpenTelemetry(クラッシュレポート / テレメトリ)

端末のログ・トレース・メトリクスを OTLP/gRPC(4317)で template-maui-server へ送り、サーバーで保存・表示する。方式・決定事項・送る内容・構成・各段階の変更ファイルと確認は `Document/Telemetry_Plan.md`(検討資料は `Telemetry_Study.md`)。1-1(サーバーの受信口)は完了(`Change_Summary.md` の区間 17)。

- [ ] **1-2-1** 端末: 収集の移設(`Diagnostics` 名前空間。パネルは今の場所のまま表示だけにする)
- [ ] **1-2-2** 端末: 起動・停止と再起動の制御
- [ ] **1-2-3** 端末: OTEL の送信
- [ ] **1-3** サーバー: 保存
- [ ] **1-4** サーバー: 一覧と詳細の画面

## ⏰2. バックグラウンドタスク(WorkManager / オフライン同期)

遅延可で再起動後も残る処理を `WorkManager` に載せ(2-1)、その上で未送信の変更をローカル DB に保持して接続回復後に差分同期する(2-2)。ファイルパスは `Template.MobileApp/` からの相対。

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

### 🔄2-2 オフライン同期

ローカル DB に未送信の変更を保持し、接続回復後に差分同期する。競合解決の画面を含む。対向(template-maui-server)の API 追加も要る。

- [ ] **2-2-0**⚖️【判断】採否 — 範囲: Network > HTTP(Data の CRUD)を対象に、未送信キュー(ローカル DB)/ `Connectivity` 復帰で送信 / サーバーの更新時刻による競合検出 / 競合一覧で端末側・サーバー側を選ぶ画面

## 🔔3. プッシュ通知

ローカル通知は `Components/NotificationService.cs` + `.android.cs` で実装済み(即時 / スケジュール / アクションボタン / タップ時ペイロード。Device > Misc の Notification カード)。FCM を使わない自前配信(SignalR + 前景サービス)は `Works3/PushSample`(同フォルダの README に完結)。

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

画面・`ViewId`・メニューボタンが配置済み(`DeviceMenuView.xaml` の該当ボタンが `IsEnabled="False"`、画面は `Not implemented` 表示、ViewModel は 8 行)。プラットフォーム実装は MauiComponents の `WiFi.cs` + `WiFi.WiFiManager.cs` / `.android.cs` と同じ構成(共通インターフェース + `*.android.cs`)に揃える。

参照: `Bio`(`Maui.Biometric-main` / `MauiBiometricPluginSample-main` / `NET-MAUI-FingerPrint-main`)

| 種別 | URL | 概要 | 適用先 |
| --- | --- | --- | --- |
| ライブラリ | https://github.com/oscoreio/Maui.Biometric | `Plugin.Fingerprint` の後継。`IBiometricAuthentication.CheckAvailabilityAsync` が `AvailabilityResult`(`AuthenticationAvailability`: NoSensor / NoBiometric / TemporaryUnavailable / NoPermission / NotSupported 等 + 検出した `BiometricSensor` の集合)を返し、`AuthenticateAsync(new AuthenticationRequest(title, reason) { Authenticators = Biometric \| DeviceCredential, ConfirmationRequired })` が `AuthenticationResult` を返す。Android 実装は `AndroidX.Biometric.BiometricPrompt`。`.UseBiometricAuthentication()` で DI 登録。v2.5.1(2026-03) | 案B の候補。可用性 3 区分は `NoSensor` / `NoBiometric` / `TemporaryUnavailable` が対応。`sample/MainViewModel.cs` が可用性表示 + 認証 + 結果表示の最小例 |
| 公式 | https://developer.android.com/training/sign-in/biometric-auth?hl=ja | 「生体認証ダイアログを表示する」。`BiometricManager.canAuthenticate` による可用性判定(`BIOMETRIC_SUCCESS` / `ERROR_NO_HARDWARE` / `ERROR_NONE_ENROLLED` / `ERROR_HW_UNAVAILABLE`)、`BiometricPrompt.PromptInfo` の組み立て、認証コールバック、`CryptoObject` | 案A の一次資料。可用性 3 区分は `canAuthenticate` の戻り値をそのまま対応付ける |

- [ ] **4-1-0**⚖️【判断】実装方式 — 案A `Components/Biometric.cs` + `.android.cs` を自作(`Xamarin.AndroidX.Biometric` を追加)/ 案B `Maui.Biometric` パッケージを参照

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Components/Biometric.cs` | — | 新規。`IBiometricAuthenticator`(可用性判定 / 認証 / 結果種別) |
| `Components/Biometric.android.cs` | — | 新規。`AndroidX.Biometric.BiometricPrompt` 実装 |
| `Modules/Device/DeviceBiometricViewModel.cs` | 空スタブ | 実装 |
| `Modules/Device/DeviceBiometricView.xaml` | 空状態表示 | 可用性表示 + 認証ボタン + 結果表示 |
| `Modules/Device/DeviceMenuView.xaml` | `Grid.Row="7" Grid.Column="1"` | `IsEnabled="False"` を削除 |
| `MauiProgram.cs` | `ConfigureComponents` | DI 登録 |
| `Platforms/Android/AndroidManifest.xml` | 権限 | `USE_BIOMETRIC` 追加 |
| `Template.MobileApp.csproj` | パッケージ | 案A: `Xamarin.AndroidX.Biometric` 追加 |

- [ ] **4-1** 可用性は「ハードウェア無し / 未登録 / 一時利用不可」を区別して表示する。範囲は認証成否の表示まで(鍵の解錠に使う `CryptoObject` は対象外)
  - 制約: `androidx.biometric` は `androidx.fragment` に依存する。csproj は `Xamarin.AndroidX.Fragment.Ktx` をピン止めしているため、追加後に `dotnet list package --include-transitive` で競合を確認する
  - `BiometricPrompt` が要求する `FragmentActivity` は `MainActivity`(`MauiAppCompatActivity` 派生)で満たしている。基底クラスの変更は不要

## 🎨5. 見た目の属性の残りの整理

要素に書いた配置・サイズ・色・文字の属性を、共有スタイル(`Styles.xaml` の `Basic` / `Card`)か画面ローカルのスタイルへ移す(`Margin` / `Padding` / `Spacing` は移行済み)。対象外 = `Controls/` と `Shell/DiagnosticPanel`(Style を使わない部品)、デモの内容そのもの(Basic Font のフォント見本、View Layout の領域の色分けとセルの大きさ、Control Toolkit の AvatarView の人ごとの色)、ブラシ・影・`FontImageSource` の中身、バインドの値。ファイルパスは `Template.MobileApp/Modules/` からの相対。

| 分類 | 現在のファイル名 | 残っている属性 |
| --- | --- | --- |
| 背景色 | `Control/ControlChartView.xaml` / `ControlCarouselView.xaml`、`Device/DeviceActivityView.xaml`、`Sample/SampleCvLocalView.xaml` / `SampleCvNetView.xaml` / `SampleWebAppView.xaml`、`UI/UILoadView.xaml` / `UIMeterView.xaml` / `UIRadarView.xaml` / `UIScheduleView.xaml` / `UITreeMapView.xaml` / `UIWheelView.xaml` / `UIPosView.xaml` / `UIMailView.xaml`(SwipeItem) | `BackgroundColor` |
| 文字 | `UI/UIMeterView.xaml`(速度・単位)/ `UIMoneyView.xaml`(金額)/ `UIPosView.xaml`、`Device/DeviceCameraView.xaml` / `DeviceQrScanView.xaml`、`Main/DiagnosticsView.xaml` | `FontSize` / `FontFamily` / `TextColor` / `HorizontalTextAlignment` / `FontAttributes` |
| 固定サイズ | `Control/ControlSfChartView.xaml`(チャートの高さ)/ `ControlCustomView.xaml`、`View/ViewGraphicsView.xaml` / `ViewStateView.xaml`、`Device/DeviceSensorView.xaml`、`Sample/SampleMediaView.xaml` / `SampleWebAppView.xaml`(ActivityIndicator)、`UI/UIGraphView.xaml` / `UIProfileView.xaml` / `UIScheduleView.xaml` / `UITimelineView.xaml` / `UIKitNotifyView.xaml`(未読ドット) | `WidthRequest` / `HeightRequest`(同じ要素の配置・色も) |
| 配置 | `Control/ControlDrawerView.xaml` / `ControlRefreshView.xaml`、`UI/UIKitOnboardView.xaml` / `UIGraph2View.xaml` / `UITimelineView.xaml` / `UIMoneyView.xaml`、`Sample/SamplePdfView.xaml`、`Network/NetworkStorageView.xaml`、`View/ViewLottieView.xaml` / `ViewAnimationView.xaml`、`Device/DeviceBleScanView.xaml`、`Navigation/Shared/SharedInputView.xaml`、`App/AppCalcView.xaml` | `HorizontalOptions` / `VerticalOptions` |
| 形・枠 | `Control/ControlBottomSheetView.xaml`(シートの角丸)/ `ControlSfChartView.xaml`(系列の線色)、`UI/UICharacterView.xaml` / `UIVisitView.xaml` / `UIChatView.xaml` | `CornerRadius` / `StrokeShape` / `StrokeThickness` / `Stroke` / `Color` |

- [ ] **5-0**⚖️【判断】対象の範囲 — 次を内容として要素に残すか: Device NFC の処理段階の色(`ProcessColor`)、アニメーションの引数(`FadeToAnimation` の `Opacity`)、フラッシュ用 `BoxView` の初期値(`Color` / `Opacity`)、タッチを受ける透明な `BoxView`、`Popup` の幅(`x:Static`)
- [ ] **5-1** 背景色
- [ ] **5-2** 文字
- [ ] **5-3** 固定サイズ
- [ ] **5-4** 配置と形・枠

## ⏸️6. 保留中の判断

### 📡6-1 通信設定が未設定のときの止め方

判定は `State/Settings.cs` の拡張メソッド(`IsApiConfigured` / `IsGrpcConfigured` / `IsScpConfigured` / `IsAIServiceConfiguredAsync` / `IsOllamaConfigured`)に集約済みで、止め方が画面によって違う。

| 画面 | 現状 |
| --- | --- |
| Sample > CV Net / Chat | メニューでダイアログを出し、画面に入らない |
| Network > Realtime / gRPC / SCP | 画面に入り「未設定」を表示し、操作を無効にする |
| Network メニューの時刻取得 | ボタンを無効にする(理由の表示なし) |
| Network > HTTP (Data) / HTTP (Auth) / Storage | 止めない(通信して失敗する) |

- [ ] **6-1-0**⚖️【判断】案A: すべてメニューで止める(ダイアログを出して画面に入らない。画面側の `Configured` 分岐が不要になる)/ 案B: すべて画面に入って「未設定」を表示し、操作を無効にする

### ⚙️6-2 CoreCLR ランタイムの扱い

csproj は `UseMonoRuntime=false`(CoreCLR)。Shiny の `[Export]` ライフサイクルコールバックで起動時にクラッシュする(dotnet/android#10996、.NET 11 で修正)。「外部待ち」の表と README の Pending に記載。

- [ ] **6-2-0**⚖️【判断】.NET 11 まで CoreCLR のままにする(実機検証は `-p:UseMonoRuntime=true` の Mono ビルド)か、csproj を Mono に戻すか
