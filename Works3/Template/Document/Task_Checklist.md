# ✅残作業チェックリスト

残作業(実機確認 / 実テスト / 保留)のマスターチェックリスト。経緯・実装内容・ナレッジ・開発ポリシーは `Change_Summary.md`(付録含む)を参照。
優先順 = 0 節 → 1 節(OpenTelemetry)→ 2 節(バックグラウンドタスク / オフライン同期)→ 3 節(プッシュ通知)→ 4 節(生体認証)。小さな項目は「取り込み候補」の章にまとめ(現在は無し)、大きな項目は章を分けている。

## 📋サマリ

| Category | Feature | 章 |
| --- | --- | --- |
| Review | 未コミット分の確認 | 0-1〜0-4 / 1-1 / 3-1 |
| Diagnostics | OpenTelemetry(クラッシュレポート / テレメトリ基盤の組み込み) | 1-2 |
| Device | Background task(WorkManager) | 2-1 |
| Network | Offline sync(未送信キュー・差分同期・競合解決) | 2-2 |
| Device | Push(FCM) | 3-2 |
| Device | Biometric(生体認証) | 4 |

## 📏運用ルール

- 作業はこの番号で指示・進行する(例:「1-1 を実施」)。完了した項目は本書から削除し、内容は `Change_Summary.md` に記録する
- **⚖️【判断】印の項目はユーザーが決定**(勝手に進めない)。デザイン判断を伴う差分は 1 項目ずつ指示を受けて実施
- 実装・変更を行なう場合の完了条件 = **ビルド警告ゼロ** + `Change_Summary.md` への記録(開発ポリシーは同 付録A)
- コミットはユーザーが実施(グループ単位を推奨)
- `README.md` の TODO 表は本書のサマリ表(1〜4 節)と同期させる(項目の追加・削除・完了時に両方を更新。TODO 表に本書の番号は書かない)
- リンク集 `■MAUI.txd` は全件に判定を付記済み(🟩 取り込む / 🟦 取り込まないが記事として有用 / 🟥 古い・参照不要 / 🟨 要判断)。🟩 の項目は本書へ移し、元行は同書から削除する
- 描画・性能の計測は **Release ビルド + 実機**(手順は `Development.md` の「Releaseビルドでの検証と計測」)

## 🧭前提(環境)

- **環境制約 (不具合ではない)**: ①地図タイルは Google Maps API キー未設定だと非表示 (ピン・カメラ移動は動作) ②Sample > CV Net は AI エンドポイント未設定だと画面に入れない ③CommunityToolkit CameraView の `CaptureAsync` がまれに未完了になる(5 秒で打ち切って「撮影できませんでした」を出し、プレビューのまま続行できる)
- 現在実機に入っているのは **Debug ビルド**(2026-09-19 デプロイ。性能・描画の確認時は Release へ入れ替える)

---

## 🔥0.【最優先】未コミット分の確認

Control メニュー新設以降(2026-09-13〜20)の未コミット分のうち、未確認のもの(確認済みの項目は削除済み)。ネットワーク系は機能ごとに分けている(0-1 REST / 0-2 UL・DL / 0-3 SignalR / 0-4 gRPC)。OpenTelemetry の組み込みサンプル(`Works3/OtelSample`)は 1-1、プッシュ通知の自前サンプル(`Works3/PushSample`)は 3-1。確認できたグループから順にコミットする(ビルド 0 警告 / inspectcode 0 件 / 実機確認は実施済み)。

対向サーバーは `D:\GitHubTemplate\template-maui-server`(別リポジトリ、こちらも未コミット)。起動と端末の接続は `Document/Development.md`「サーバー処理」。サーバー側のパスは `template-maui-server/src/Template.MobileServer.Web/` からの相対((server) 印)。

- コミット対象外: `.claude/worktrees/angry-feistel-0b79a0/`(worktree は登録解除・ブランチ削除済み。空のディレクトリだけがロックで残っているので、アプリの再起動後に削除)

### 🔗0-1 REST(Web API)

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Modules/Network/NetworkMenuView.xaml` + `NetworkMenuViewModel.cs` | 通信のメニュー | 時刻取得(ダイアログ表示の直接呼び出し)だけを残し、あとは画面遷移(HTTP (Data) / HTTP (Auth) / Storage / Realtime / gRPC / SCP)。未設定時は無効 |
| `Modules/Network/NetworkHttpView.xaml` + `NetworkHttpViewModel.cs` | HTTP (Data): Web API(Data の CRUD、匿名) | 一覧(20 件ずつ追加読み込み)、行選択で詳細、作成 / 更新 / 削除 / クリア、重複は 409、全件を Work テーブルへ保存、テスト API(エラー 500 = 再試行の確認 2 回 → 通知、遅延 5 秒 = インジケーター、10 秒待つ API とキャンセル)、ログ。F2 = Reload |
| `Modules/Network/NetworkAuthView.xaml` + `.xaml.cs` + `NetworkAuthViewModel.cs`(新規) | HTTP (Auth): 認証 API | ID だけでログイン(既定 `user`)/ ログアウト / 状態(ログイン ID・トークン期限)、Secure(JWT 必須 API。未ログインは 401 の通知)、トークンを無効化 → Secure で再ログイン再送(`NetworkUsecase.InvalidateToken`) |
| `Services/HttpService.cs` / `ApiContext.cs` | API 呼び出し、認証状態(`LoginId` / `TokenExpires`) | 更新は Rester 2.17.0 の `PutAsync`、削除と本文の無い GET は `SendAsync(HttpMethod.Delete / Get)` |
| `Usecase/NetworkUsecase.cs` | 通信の共通処理(未接続 / インジケーター / 401 の再ログイン再送 / エラー種別ごとの通知と再試行)と API ごとの処理。戻り値は `NetworkResult` / `NetworkResult<T>`(`Type` + `StatusCode` + `Value`) | 再ログインは `authenticated: true` の呼び出し(Secure)だけ・1 回だけ。Error(500)は再試行の確認 2 回 → 3 回目は通知のみ |
| `Services/HttpService.cs`(先頭の契約 DTO: `DataListResponse` + `DataListEntry` / `DataResponse` / `DataCreateRequest` / `DataCreateResponse` / `DataUpdateRequest`)| 契約 DTO | サーバー側 `Endpoints/DataEndpoints.cs` の先頭と同じ形 |
| `Helpers/JwtHelper.cs`(新規) | JWT の有効期限の取り出し | — |
| (server) `Endpoints/DataEndpoints.cs`(先頭に契約 DTO)/ `Core/Services/DataService.cs` / `Core/Models/RangeResult.cs` | 一覧の範囲取得(`offset` / `size`、`Total`) | 省略時は全件 |

- [ ] **0-1** 上記(実機確認は `Change_Summary.md` の「ネットワーク実装」参照)

### 📤0-2 UL / DL(ストレージ)

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Modules/Network/NetworkStorageView.xaml` + `.xaml.cs` + `NetworkStorageViewModel.cs` | ストレージ(簡易 FTP API) | ディレクトリの一覧 / 下階層 / 上へ、ファイル(FilePicker)と写真(MediaPicker)のアップロード、ダウンロード(公開フォルダ)、削除(確認ダイアログ)、進捗とキャンセル。F2 = Reload |
| `Usecase/NetworkUsecase.cs`(ストレージ) | 一覧 / アップロード / ダウンロード / 削除のユースケース | `ExecuteTransferAsync` はインジケーターなし(進捗とキャンセルは画面側) |
| `Services/HttpService.cs`(先頭の `StorageListResponse` + `StorageListEntry`)| 契約 DTO | サーバー側 `Endpoints/StorageEndpoints.cs` の先頭と同じ形 |

- [ ] **0-2** 上記(実機確認は `Change_Summary.md` の「ネットワーク実装」参照)

### 📡0-3 SignalR(Realtime)

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Modules/Network/NetworkRealtimeView.xaml` + `NetworkRealtimeViewModel.cs` | SignalR(MonitorHub、認証なし) | 遷移時に `Connect()` を購読 / 離脱時に破棄(= 切断)、受信は `ServerStatus` / `Notifications` の購読、状態 / 接続 ID / サーバー時刻、サーバーの CPU / メモリ / 接続数のグラフ、端末の状態を 10 秒ごとに送信、通知の一覧(前面 = トースト、バックグラウンド = ローカル通知)。F2 = Connect |
| (NuGet) `Mofucat.ReactiveHub` 0.1.0(旧 `Helpers/ReactiveHubConnection.cs`、`D:\GitHub\Mofucat-ReactiveHub`) | 汎用の SignalR 接続維持(`HubConnection` の生成・維持・破棄をクラスが持つ。`Connect(url, configure, build, resume)` = 購読で接続・破棄で切断・後勝ち、`On<T>()`、`TrySendAsync` / `TryInvokeAsync`(未接続は false)、`InvokeAsync<TResult>`、`Status` / `IsConnected` / `ConnectionId`、`DisposeAsync`。初回接続と自動再接続の間隔は ctor の `retryDelays`) | 初回接続のバックオフ再試行 / ネットワーク復帰で待ち打ち切り / 自動再接続 / `Closed` 後のやり直し / 破棄で `StopAsync` / 再購読は前の接続を止めてから新しい接続(同時に 2 本にならない)/ Dispose 後は `ObjectDisposedException`(ライブラリのテスト 17 件で確認。Template 側はパッケージ置換後の実機確認が未) |
| `Services/MonitorConnection.cs`(新規) | MonitorHub 固有(ハブのパス / KeepAlive / ServerTimeout、`Connect(baseAddress)`、`ServerStatus` / `Notifications`、`ReportDeviceStatusAsync`、ログ) | 認証なし。接続の管理は `Mofucat.ReactiveHub` の `ReactiveHubConnection` |
| `Services/MonitorConnection.cs`(先頭の `DeviceStatusMessage` / `ServerStatusMessage` / `NotificationMessage`)| ハブのメッセージ | サーバー側 `Hubs/MonitorHub.cs` の先頭と同じ形 |
| `Extensions.cs` | `ConnectivityChangedAsObservable`(ネットワーク復帰で接続の待ちを打ち切る) | — |
| `State/Session.cs` / `App.xaml.cs` | 前面かどうか(`IsForeground`)を Window の Resumed / Stopped で更新 | — |
| (server) `Hubs/MonitorHub.cs` / `Services/DeviceRegistry.cs` / `DeviceEntry.cs` / `MonitorNotifier.cs` / `Services/MonitorOptions.cs` / `Workers/ServerStatusWorker.cs` / `NotificationRelayWorker.cs` / `Application/ApplicationExtensions.cs` / `Program.cs` / `Application/Log.cs` | SignalR ハブと状態配信 / 通知 | 認証なし、KeepAlive 15 秒 / ClientTimeout 30 秒、`DeviceRegistry.Disconnect` |
| (server) `Components/Pages/DevicesPage.razor(.cs)` / `Layout/NavMenu.razor` / `Pages/Home.razor(.cs)` / `wwwroot/css/app.css` | 管理画面の Devices(端末一覧、通知の送信、切断 = `HubCallerContext.Abort`)、Home の接続数 | 切断で端末が `Closed` → 新 ID で再接続 |

- [ ] **0-3** 上記(実機確認は `Change_Summary.md` の「ネットワーク実装」と「SignalR 接続維持の Mofucat.ReactiveHub 0.1.0 への置換」参照。パッケージ版も 2026-09-21 に Pixel 9a で再確認済み)

### 💬0-4 gRPC(チャット)

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Modules/Network/NetworkGrpcView.xaml` + `NetworkGrpcViewModel.cs` | gRPC(チャット) | 接続先 / 状態 / 未配送数、単項 RPC(サーバー時刻)、チャット(管理画面 `/chat` と相互)、切断中の送信は再接続後に配送。F2 = Connect |
| `Services/ChatRoomClient.cs`(関連型も同じファイル)/ `Services/Protos/chat.proto` / `server.proto` | gRPC チャット(サーバーの WPF サンプルの移植)と proto のコピー | 認証なし。ユーザー名(端末名)は各メッセージで送る、指数バックオフ再接続、送信キュー |
| (server) `Handlers/Protos/server.proto` / `Handlers/ServerInfoHandler.cs` | 単項 RPC(サーバー時刻、匿名) | — |

- [ ] **0-4** 上記(実機確認は `Change_Summary.md` の「ネットワーク実装」参照)

---

## 📈1. OpenTelemetry(クラッシュレポート / テレメトリ)

検討資料は `Document/Telemetry_Study.md`(現状 / 候補 A・B・C / 比較 / アプリ側の組み込み設計 / 論点)。候補 B(OTLP)の実証サンプルは `Works3/OtelSample`(同フォルダの README に完結)。

### 🧪1-1 組み込みサンプル(`Works3/OtelSample`)の確認

本アプリのコードは無変更。構成 / 実行手順 / 送信の設計(OTLP/HTTP、ILogger の転送、ディスク退避と再送、クラッシュ、MAUI のレイアウト計測)/ サーバの画面 / 確認済みの動作 / 解析上の制約 / ナレッジはすべて `Works3/OtelSample/README.md`。`Document/Telemetry_Study.md` の候補 B の実証。サーバ(`OtelServer`)は 2026-09-19 に Blazor(MudBlazor)+ SQLite のダッシュボード(Dashboard / Logs / Traces / Metrics / Dummy Data)へ刷新し、OTLP/gRPC の受け口(4317)も追加。クライアントは HTTP / gRPC を切替可(未コミット。`App_Data/` は `.gitignore`)。

- [ ] **1-1** `Works3/OtelSample`(README に沿って確認。クライアント = `EventSourceSupport=true`、`AddMetrics`、`AddView` によるタグの集約、`OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY=disk` の採用 / サーバ = Razor コンポーネントのための CA1515 抑止(`GlobalSuppressions.cs`)、MudBlazor 9.10.0 と Microsoft.Data.Sqlite 10.0.12 の採用、ダッシュボードの各画面(実機の Pixel 9a とダミーデータで確認済み))

### 🧩1-2 本アプリへの組み込み

- [ ] **1-2-0**⚖️【判断】方式と範囲 — `Telemetry_Study.md` 6 節の論点(方式 A / B / C または組み合わせ、収集する項目と粒度、端末の識別と個人情報、サーバーの配置と運用、ライブラリの形、本テンプレートでの見せ方)。組み込み点は同 5 節(`ITelemetry` の抽象化と Null 実装、`Settings` の接続先(`OtelEndPoint` は投入済み)、`CrashReport` / ナビゲーション / `DeviceState` / `NetworkOperator` / `DiagnosticPanel` からの収集、バックグラウンド移行でのフラッシュ)

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

### 📦3-1 自前配信のサンプル(`Works3/PushSample`)の確認

本アプリのコードは無変更。構成 / 実行手順 / 再接続の設計 / 確認済みの動作 / 解析上の制約はすべて `Works3/PushSample/README.md`。

- [ ] **3-1** `Works3/PushSample`(README に沿って確認。`PushHub` の CA1812 抑止と `CommunityToolkit.Mvvm` の採用を含む)

### ☁️3-2 FCM

参照: `0_maui-samples/10.0/WebServices/PushNotificationsDemo`(`Xamarin.Firebase.Messaging` + 自前 `FirebaseMessagingService`)。

| 種別 | URL | 概要 | 適用先 |
| --- | --- | --- | --- |
| 記事 | https://www.andreasnesheim.no/push-notifications-in-net-maui-with-firebase/ | FCM を `Plugin.Firebase` で扱う手順。Firebase Console 登録 → `google-services.json` 配置 → `MauiProgram` 初期化 → `CrossFirebaseCloudMessaging.Current.GetTokenAsync()` でトークン取得 → Console からテスト送信(2022-09) | 採用時の方式候補①(`Plugin.Firebase`)。方式候補②は上記 PushNotificationsDemo |
| ライブラリ | https://github.com/Gekidoku/BetterFireBaseNotificationsPlugin | `CrossFirebasePlugin` の MAUI 移植。データ付き通知 / アクションボタン / サイレント通知 / アプリ終了時の受信に対応。NuGet 配布はなくプロジェクト参照前提(2026-01) | 実装範囲(データ / アクション / サイレント)のチェックリスト |

- [ ] **3-2-0**⚖️【判断】プッシュ通知(FCM)の要否 — Firebase プロジェクトと `google-services.json` が前提。採用する場合、トークン表示と受信ログを Notification カードに追記する

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
