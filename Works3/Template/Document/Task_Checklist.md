# 残作業チェックリスト

残作業(実機確認 / 実テスト / 保留)のマスターチェックリスト。経緯・実装内容・ナレッジ・開発ポリシーは `Change_Summary.md`(付録含む)を参照。
優先順 = 0 節 → 2 節 → 3 節。以降は 1 節(要 SSH サーバ)。

## サマリ

| Category | Feature | 章 |
| --- | --- | --- |
| Review | 未コミット分の確認(0-1〜0-8) | 0 |
| Network | SCP の実機確認と転送実テスト(要 SSH サーバ) | 1 |
| Device | Biometric(生体認証) | 2-1 |
| Device | Push(FCM) | 2-2 |
| Network | Offline sync(未送信キュー・差分同期・競合解決) | 2-3 |
| Basic | Startup screen(初期化の進捗・失敗・再試行) | 2-4 |
| Basic | App structure(機能プロファイル / 診断画面 / サンプルカタログ / 遅延初期化) | 2-5 |
| Basic | .NET 10 API(未適用 API の反映) | 3-2 |
| Diagnostics | Layout metrics(レイアウト診断メトリクス) | 3-3 |
| Basic | Global xmlns | 3-4 |
| View | StyleClass(文字サイズ × 配置) | 3-5 |
| View | Material 3(UseMaterial3) | 3-10 |
| Device | Background task(WorkManager) | 3-12 |

## 運用ルール

- 作業はこの番号で指示・進行する(例:「2-1 を実施」)。完了した項目は本書から削除し、内容は `Change_Summary.md` に記録する
- **【判断】印の項目はユーザーが決定**(勝手に進めない)。デザイン判断を伴う差分は 1 項目ずつ指示を受けて実施
- 実装・変更を行なう場合の完了条件 = **ビルド警告ゼロ** + `Change_Summary.md` への記録(開発ポリシーは同 付録A)
- コミットはユーザーが実施(グループ単位を推奨)
- `README.md` の TODO 表は本書のサマリ表(2〜3 節)と同期させる(項目の追加・削除・完了時に両方を更新。TODO 表に本書の番号は書かない)
- 描画・性能の計測は **Release ビルド + 実機**(手順は `Development.md` の「Releaseビルドでの検証と計測」)

## 前提(環境)

- **環境制約 (不具合ではない)**: ①地図タイルは Google Maps API キー未設定だと非表示 (ピン・カメラ移動は動作) ②SampleCvNet 系は AI エンドポイント未設定だと画面に入れない ③CommunityToolkit CameraView の `CaptureAsync` がまれに未完了になり Function キーが無反応化 (再起動で回復)
- 現在実機に入っているのは **Debug ビルド**(2026-09-19 デプロイ。性能・描画の確認時は Release へ入れ替える)

---

## 0.【最優先】未コミット分の確認

Control メニュー新設以降(2026-09-13〜19)の未コミット分のうち、未確認のもの(確認済みの項目は削除済み)。確認できたグループから順にコミットする(ビルド 0 警告 / inspectcode 0 件 / 実機確認は実施済み)

- コミット対象外: `.claude/worktrees/angry-feistel-0b79a0/`(worktree は登録解除・ブランチ削除済み。空のディレクトリだけがロックで残っているので、アプリの再起動後に削除)

### 0-1 Azure AI Vision / Ollama チャット / 音声入力

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Usecase/AzureVisionUsecase.cs` | Image Analysis 4.0(物体 / 人物 / タグ / 文字) | — |
| `Modules/Sample/SampleCvNetObjectView.xaml` / `Tag` / `People` / `Ocr`(各 `.xaml` + `ViewModel.cs`)/ `Graphics/Drawing/DetectDrawing.cs` | 撮影 → 解析 → 枠(ラベル + 信頼度)/ タグのパネル | Setting の AI EndPoint / Key が必要。Tag は日本語のタグ |
| `Modules/Sample/SampleChatView.xaml` + `SampleChatViewModel.cs` | Ollama のストリーミング応答(未設定は疑似応答)、音声入力(端末の音声認識)と Ollama の抽出 | 冒頭のあいさつに動作モード、応答、音声フローの 4 ステップ |
| `State/Settings.cs` / `Modules/Main/SettingView.xaml` + `SettingViewModel.cs` | `OllamaEndPoint` / `OllamaModel`(QR のキー名も同じ) | Setting 画面の Ollama / Model の行 |
| `Document/Sample_CvNet_Tag.png` | 画像 | README |
| `Platforms/Android/AndroidManifest.xml` | `usesCleartextTraffic="true"`(localhost 以外への平文 HTTP) | Wi-Fi 経由の Ollama(`http://<PC の IP>:<port>`)で Chat が応答 |

- [ ] **0-1** 上記(端末には Foundry の EndPoint / Key と Ollama(`http://192.168.100.9:12321` = PC の Wi-Fi 側 / `gemma2`)を設定済み。不要なら Setting の QR で上書き)

### 0-2 プッシュ通知の自前サンプル(別フォルダ `Works3/PushSample`)

本アプリのコードは無変更。構成 / 実行手順 / 再接続の設計 / 確認済みの動作 / 解析上の制約はすべて `Works3/PushSample/README.md`。

- [ ] **0-2** `Works3/PushSample`(README に沿って確認。`PushHub` の CA1812 抑止と `CommunityToolkit.Mvvm` の採用を含む)

### 0-3 ネットワーク(対向サーバー = template-maui-server)

対向サーバーは `D:\GitHubTemplate\template-maui-server`(別リポジトリ、こちらも未コミット)。起動と端末の接続は `Document/Development.md`「サーバー処理」。サーバー側のパスは `template-maui-server/src/Template.MobileServer.Web/` からの相対((server) 印)。

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Modules/Network/NetworkMenuView.xaml` + `NetworkMenuViewModel.cs` | Network メニュー | Download \| Upload の行が HTTP \| Storage に、Realtime \| gRPC の 2 列、SCP \| 空き。単発ボタン(server time / data list / secure / login / logout / error / delay)は残置 |
| `Modules/Network/NetworkHttpView.xaml` + `NetworkHttpViewModel.cs` | Web API(Data の CRUD) | ログイン状態(Id / 有効期限)、一覧(20 件ずつ追加読み込み)、行選択で詳細、作成 / 更新 / 削除 / クリア、未ログインの作成は 401、重複は 409、10 秒待つ API とキャンセル、ログ。F2 = Reload |
| `Modules/Network/NetworkStorageView.xaml` + `.xaml.cs` + `NetworkStorageViewModel.cs` | ストレージ(簡易 FTP API) | ディレクトリの一覧 / 下階層 / 上へ、ファイル(FilePicker)と写真(MediaPicker)のアップロード、ダウンロード(公開フォルダ)、削除(確認ダイアログ)、進捗とキャンセル。F2 = Reload |
| `Modules/Network/NetworkRealtimeView.xaml` + `NetworkRealtimeViewModel.cs` | SignalR(MonitorHub、認証なし) | 遷移時に `Connect()` を購読 / 離脱時に破棄(= 切断)、受信は `ServerStatus` / `Notifications` の購読、状態 / 接続 ID / サーバー時刻、サーバーの CPU / メモリ / 接続数のグラフ、端末の状態を 10 秒ごとに送信、通知の一覧(前面 = トースト、バックグラウンド = ローカル通知)。F2 = Connect |
| `Modules/Network/NetworkGrpcView.xaml` + `NetworkGrpcViewModel.cs` | gRPC(チャット) | 接続先 / 状態 / 未配送数、単項 RPC(サーバー時刻)、チャット(管理画面 `/chat` と相互)、切断中の送信は再接続後に配送。F2 = Connect |
| `Modules/Network/NetworkScpViewModel.cs` | SCP | 転送を BusyState の外で実行(キャンセル可能に) |
| `Services/HttpService.cs` / `ApiContext.cs` / `Log.cs`(新規) | API 呼び出し、認証状態(`LoginId` / `TokenExpires`)、通信系ログ | PUT / DELETE は `HttpClient` を `RestResponse` に包む。ログは `Services/Log.cs` に分離 |
| `Helpers/ReactiveSignalR.cs`(Rx ベースで作り直し) | 汎用の SignalR 接続維持(`Connect()` = 購読で接続・破棄で切断、`On<T>()`、`CreateRetryPolicy`) | 初回接続のバックオフ再試行(`RetryWhen`)/ ネットワーク復帰で待ち打ち切り(`Amb`)/ 自動再接続 / `Closed` 後のやり直し(`Repeat`)/ 破棄で `StopAsync` |
| `Services/MonitorConnection.cs`(新規) | MonitorHub 固有(`HubConnection` の構築、`Connect(baseAddress)`、`ServerStatus` / `Notifications`、`ReportDeviceStatusAsync`) | 認証なし。状態のログ、購読の破棄で `HubConnection` を破棄 |
| `Services/Chat/ChatClient.cs` + `ChatConnectionState.cs` / `ChatMessageEntry.cs` / `ChatMessageEventArgs.cs` / `ChatStateEventArgs.cs` / `chat.proto` / `server.proto` | gRPC チャット(サーバーの WPF サンプルの移植)と proto のコピー | 指数バックオフ再接続、送信キュー、トークンは接続ごとに取得 |
| `Usecase/NetworkOperator.cs` / `NetworkUsecase.cs` | 401 の再ログイン再送、CRUD / ストレージ / 遅延のユースケース、gRPC チャット用の `EnsureLoginAsync` / `GetTokenAsync` | 再ログインは 1 回だけ、`ExecuteTransfer` はインジケーターなし |
| `Models/Api/DataListResponse.cs`(`Id` long / `Total`)/ `DataResponse.cs` / `DataCreateRequest.cs` / `DataCreateResponse.cs` / `DataUpdateRequest.cs` / `StorageListResponse.cs` / `MonitorMessages.cs` | 契約 DTO | サーバー側と同じ形 |
| `Helpers/JwtHelper.cs`(新規)/ `Extensions.cs` | JWT の有効期限の取り出し、`ConnectivityChangedAsObservable` | — |
| `State/Settings.cs` / `Modules/Main/SettingView.xaml` + `SettingViewModel.cs` | `MonitorEndPoint` → `GrpcEndPoint`(QR キー同名)、Setting の gRPC 行、QR 読み取り後の表示更新 | Setting 画面の Network セクション |
| `State/Session.cs` / `App.xaml.cs` | 前面かどうか(`IsForeground`)を Window の Resumed / Stopped で更新 | — |
| `MauiProgram.cs` / `Modules/ViewId.cs` / `Markup/AppIcons.cs` / `Template.MobileApp.csproj` | Rester の JSON を PascalCase(大文字小文字を区別しない)に、DI、`NetworkStorage`、アイコン(`Http` / `FolderOpen` / `Hub`)、proto の参照 | ビルド |
| `Document/Development.md` | 「サーバー処理」 | 起動 / ポート / `adb reverse` / QR の手順 |
| (server) `Components/Pages/QrPage.razor(.cs)` | 設定 QR(全キー。値の管理は 0-7 の `Setting` テーブル) | `/qr` の内容 |
| (server) `Endpoints/DataEndpoints.cs` / `Models/Api/DataListResponse.cs` / `Core/Services/DataService.cs` / `Core/Models/RangeResult.cs` | 一覧の範囲取得(`offset` / `size`、`Total`) | 省略時は全件 |
| (server) `Hubs/MonitorHub.cs` / `Infrastructure/Monitor/DeviceRegistry.cs` / `DeviceEntry.cs` / `MonitorNotifier.cs` / `Models/Api/MonitorMessages.cs` / `Workers/ServerStatusWorker.cs` / `NotificationRelayWorker.cs` / `Application/ApplicationExtensions.cs` / `Program.cs` / `Application/Log.cs` | SignalR ハブと状態配信 / 通知 | 認証なし、KeepAlive 15 秒 / ClientTimeout 30 秒、`DeviceRegistry.Disconnect` |
| (server) `Components/Pages/DevicesPage.razor(.cs)` / `Layout/NavMenu.razor` / `Pages/Home.razor(.cs)` / `wwwroot/css/app.css` | 管理画面の Devices(端末一覧、通知の送信、切断 = `HubCallerContext.Abort`)、Home の接続数 | 切断で端末が `Closed` → 新 ID で再接続 |
| (server) `Protos/server.proto` / `Handlers/ServerInfoHandler.cs` | 単項 RPC(サーバー時刻、匿名) | — |
| (server) `appsettings.Development.json` / `Assembly.cs` | JWT 有効期限 5 分(開発)、テストへの `InternalsVisibleTo` | — |
| (server) `tests/.../DeviceRegistryTests.cs` / `DevicesPageTests.cs` / `QrPageTests.cs` / `NavMenuTests.cs` / `README.md` | テスト(35 件)と README(API 一覧 / SignalR(認証なし、切断)/ QR の書式) | — |

- [ ] **0-3** 上記(実機確認は `Change_Summary.md` の「ネットワーク実装」参照)

### 0-4 .NET 10 API の適用 / IChatClient 抽象化 / ドキュメント

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Modules/Basic/BasicSettingView.xaml` | `SearchBar.SearchIconColor` / `ReturnType`、`Switch.OffColor` | 検索アイコンが青、Switch オフ時の色 |
| `Modules/Device/DeviceMiscViewModel.cs` | `IVibration.IsSupported` / `IHapticFeedback.IsSupported` でボタンの有効化 | Pixel では有効のまま |
| `Modules/Device/DeviceLocationViewModel.cs` + `DeviceLocationView.xaml` | `IGeolocation.IsEnabled` で空状態に「Location service is disabled」 | 位置情報サービスを切ると表示 |
| `Services/AiChatClientFactory.cs`(新規)/ `Modules/Sample/SampleChatViewModel.cs` / `MauiProgram.cs` | チャットの依存を `IChatClient`(`Microsoft.Extensions.AI`)に。履歴は VM が保持、抽出は `GetResponseAsync` | Ollama で応答と 2 回目の文脈、未設定時の疑似応答 |
| `Document/Development.md` | `dotnet run --device <シリアル>` の手順 | — |
| `Document/Other_App_Candidates.md`(新規) | 本サンプルでは対象外、別アプリで導入を検討する項目(ディープリンク) | — |
| `Document/Telemetry_Study.md`(新規) | クラッシュレポート / テレメトリ基盤の検討資料(DeviceManager 型 / OpenTelemetry / 外部サービス、Aspire の位置付け) | 別セッションでの検討の入力 |

- [ ] **0-4** 上記

### 0-5 OpenTelemetry の組み込みサンプル(別フォルダ `Works3/OtelSample`)

本アプリのコードは無変更。構成 / 実行手順 / 送信の設計(OTLP/HTTP、ILogger の転送、ディスク退避と再送、クラッシュ、MAUI のレイアウト計測)/ サーバの画面 / 確認済みの動作 / 解析上の制約 / ナレッジはすべて `Works3/OtelSample/README.md`。`Document/Telemetry_Study.md` の候補 B の実証。サーバ(`OtelServer`)は 2026-09-19 に Blazor(MudBlazor)+ SQLite のダッシュボード(Dashboard / Logs / Traces / Metrics / Dummy Data)へ刷新し、OTLP/gRPC の受け口(4317)も追加。クライアントは HTTP / gRPC を切替可(未コミット。`App_Data/` は `.gitignore`)。

- [ ] **0-5** `Works3/OtelSample`(README に沿って確認。クライアント = `EventSourceSupport=true`、`AddMetrics`、`AddView` によるタグの集約、`OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY=disk` の採用 / サーバ = Razor コンポーネントのための CA1515 抑止(`GlobalSuppressions.cs`)、MudBlazor 9.10.0 と Microsoft.Data.Sqlite 10.0.12 の採用、ダッシュボードの各画面(実機の Pixel 9a とダミーデータで確認済み))

### 0-6 Drawer の背景スワイプ(Bottom sheet / Drawer の確認後に追加)

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Controls/SideDrawer.cs` | 背景にも `PanGestureRecognizer`(パネルのドラッグと同じ扱い) | 開いた状態で背景を左へスワイプ / ドラッグすると閉じる(右へのフリックは開いたまま)。タップでも閉じる |

- [ ] **0-6** 上記

### 0-7 設定画面の OTEL 欄と表示の詰め

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `State/Settings.cs` / `Modules/Main/SettingViewModel.cs` | `OtelEndPoint`(QR のキー名も同じ。送信先として使うのは OTEL 対応時) | QR の `OtelEndPoint=` が保存されて表示される |
| `Modules/Main/SettingView.xaml` | 設定値のパネルは内容の高さ(`Auto`)、カメラが残りを全て使う。行は 1 行表示(長い値は中央省略、キーは伏せ字)、行間 2 / 行の高さ 22 / 文字 13、見出し 15 | Network に OTEL 行、全項目が 1 行で収まる |
| (server) `Settings/ClientSettingKeys.cs` / `Components/Pages/QrPage.razor(.cs)` / `Core/Accessors/SettingAccessor.cs` + `Sql/SettingAccessor.*.sql` / `Core/Services/SettingService.cs` / `Core/Models/Entity/SettingEntity.cs` / `Database.sql` / `appsettings.json`(`Client` 削除)/ `README.md` / `tests/.../QrPageTests.cs` | QR の値は `Setting` テーブル(Key / Value)で管理し `/qr` で編集・保存。接続先(API / gRPC / OTEL)はサーバーの URL から決めて保存しない | `/qr` で値を保存 → 再読込で残る、空欄で行が消える、生成値に `OtelEndPoint=` が出る(テスト 39 件) |

- [ ] **0-7** 上記

### 0-8 Windows 検証コンソール(別フォルダ `Works3/AiSample`)

本アプリのコードは無変更。0-1 の `Usecase/AzureVisionUsecase.cs` と `Services/AiChatClientFactory.cs` をそのままコピーし、Sample > CV Net(画像ファイル → 解析 → 枠を描いた PNG)と Sample > Chat(Ollama のストリーミング応答 / 音声入力 4 ステップ)を Windows のコンソールで実行する。構成 / 設定(引数 / 環境変数 / appsettings.json)/ Azure の設定 / 確認済みの動作 / 解析上の制約はすべて `Works3/AiSample/README.md`。

- [ ] **0-8** `Works3/AiSample`(README に沿って確認。コピーが無変更であること = `AiShared/Usecase/AzureVisionUsecase.cs` / `AiShared/Services/AiChatClientFactory.cs`)

---

## 1. SCP の実機確認と転送実テスト(要 SSH サーバ)

sshd は対向サーバー(template-maui-server、4 節)に含まれない。

- [ ] **1-1** Network メニュー: 「SCP」が追加され遷移できる(未設定時は接続先が「未設定 (設定画面の QR で投入)」でボタン無効)
- [ ] **1-2** Main > Setting: 項目の**ラベルと現在値が横並び**で表示される。SCP セクション(Host/User/Password)があり、QR(`ScpHost=...` 形式)を読むと反映される
- [ ] **1-3** Network > SCP: QR 投入後、「アップロード」でファイル選択 → 進捗バー → 完了ログ。「ダウンロード」で同ファイルがキャッシュへ取得される。転送中「キャンセル」で中断。接続後にサーバのホスト鍵指紋が参考表示される(照合は行わない)

---

## 2.【優先】旧 `tmpl-plan-maui.md` からの移管課題

旧 `D:\GitHubTemplate\tmpl-plan-maui.md`(MAUI トラック強化プラン。2026-09-17 に本節へ取り込んで削除)の未対応項目(旧番号を併記)。keyboard / blazor 向けは `D:\GitHubTemplate\tmpl-plan-maui-keyboard.md` / `tmpl-plan-maui-blazor.md`(別セッション)。iOS(旧 3-13)は保留継続、SocialControls の TODO 整理 / TimeProvider / Analyzers.ruleset 正典差分(旧 3-9 / 3-10 / 3-12)は対応不要(`Change_Summary.md` 付録)。
参照サンプルは `C:\Users\machi\Desktop\Maui`(残置 18 件)。ファイルパスは `Template.MobileApp/` からの相対。

### 機能実装(同書 §3)

2-1 は画面・`ViewId`・メニューボタンが配置済み(`DeviceMenuView.xaml` の該当ボタンが `IsEnabled="False"`、画面は `Not implemented` 表示、ViewModel は 8 行)。プラットフォーム実装は MauiComponents の `WiFi.cs` + `WiFi.WiFiManager.cs` / `.android.cs` と同じ構成(共通インターフェース + `*.android.cs`)に揃える。

#### 2-1 生体認証(同書 3-2)

参照: `Bio`(`Maui.Biometric-main` / `MauiBiometricPluginSample-main` / `NET-MAUI-FingerPrint-main`)

| 種別 | URL | 概要 | 適用先 |
| --- | --- | --- | --- |
| ライブラリ | https://github.com/oscoreio/Maui.Biometric | `Plugin.Fingerprint` の後継。`IBiometricAuthentication.CheckAvailabilityAsync` が `AvailabilityResult`(`AuthenticationAvailability`: NoSensor / NoBiometric / TemporaryUnavailable / NoPermission / NotSupported 等 + 検出した `BiometricSensor` の集合)を返し、`AuthenticateAsync(new AuthenticationRequest(title, reason) { Authenticators = Biometric \| DeviceCredential, ConfirmationRequired })` が `AuthenticationResult` を返す。Android 実装は `AndroidX.Biometric.BiometricPrompt`。`.UseBiometricAuthentication()` で DI 登録。v2.5.1(2026-03) | 案B の候補。可用性 3 区分は `NoSensor` / `NoBiometric` / `TemporaryUnavailable` が対応。`sample/MainViewModel.cs` が可用性表示 + 認証 + 結果表示の最小例 |
| 公式 | https://developer.android.com/training/sign-in/biometric-auth?hl=ja | 「生体認証ダイアログを表示する」。`BiometricManager.canAuthenticate` による可用性判定(`BIOMETRIC_SUCCESS` / `ERROR_NO_HARDWARE` / `ERROR_NONE_ENROLLED` / `ERROR_HW_UNAVAILABLE`)、`BiometricPrompt.PromptInfo` の組み立て、認証コールバック、`CryptoObject` | 案A の一次資料。可用性 3 区分は `canAuthenticate` の戻り値をそのまま対応付ける |

- [ ] **2-1-0**【判断】実装方式 — 案A `Components/Biometric.cs` + `.android.cs` を自作(`Xamarin.AndroidX.Biometric` を追加)/ 案B `Maui.Biometric` パッケージを参照

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

- [ ] **2-1** 可用性は「ハードウェア無し / 未登録 / 一時利用不可」を区別して表示する。範囲は認証成否の表示まで(鍵の解錠に使う `CryptoObject` は対象外)
  - 制約: `androidx.biometric` は `androidx.fragment` に依存する。csproj は `Xamarin.AndroidX.Fragment.Ktx` をピン止めしているため、追加後に `dotnet list package --include-transitive` で競合を確認する
  - `BiometricPrompt` が要求する `FragmentActivity` は `MainActivity`(`MauiAppCompatActivity` 派生)で満たしている。基底クラスの変更は不要

#### 2-2 プッシュ通知 FCM(同書 3-4)

参照: `0_maui-samples/10.0/WebServices/PushNotificationsDemo`(`Xamarin.Firebase.Messaging` + 自前 `FirebaseMessagingService`)。ローカル通知は `Components/NotificationService.cs` + `.android.cs` で実装済み(即時 / スケジュール / アクションボタン / タップ時ペイロード。Device > Misc の Notification カード)。FCM を使わない自前配信(SignalR + 前景サービス)は `Works3/PushSample`(同フォルダの README)。

| 種別 | URL | 概要 | 適用先 |
| --- | --- | --- | --- |
| 記事 | https://www.andreasnesheim.no/push-notifications-in-net-maui-with-firebase/ | FCM を `Plugin.Firebase` で扱う手順。Firebase Console 登録 → `google-services.json` 配置 → `MauiProgram` 初期化 → `CrossFirebaseCloudMessaging.Current.GetTokenAsync()` でトークン取得 → Console からテスト送信(2022-09) | 採用時の方式候補①(`Plugin.Firebase`)。方式候補②は上記 PushNotificationsDemo |
| ライブラリ | https://github.com/Gekidoku/BetterFireBaseNotificationsPlugin | `CrossFirebasePlugin` の MAUI 移植。データ付き通知 / アクションボタン / サイレント通知 / アプリ終了時の受信に対応。NuGet 配布はなくプロジェクト参照前提(2026-01) | 実装範囲(データ / アクション / サイレント)のチェックリスト |

- [ ] **2-2-0**【判断】プッシュ通知(FCM)の要否 — Firebase プロジェクトと `google-services.json` が前提。採用する場合、トークン表示と受信ログを Notification カードに追記する(低優先)

---

### 追加アイデア(同書 付録)

2026-08-31 のソースレビュー由来。ディープリンク(→ `Other_App_Candidates.md`)とアクセシビリティ・外観設定は対象外。

#### 2-3 オフライン同期(同書 付録 1)

- [ ] **2-3-0**【判断】採否 — ローカル DB に未送信の変更を保持し、接続回復後に差分同期する。競合解決の画面を含む。採用する場合は対向(template-maui-server)の API 追加も要る

#### 2-4 起動状態と再試行画面(同書 付録 4)

- [ ] **2-4-0**【判断】採否 — DB 初期化 / 同期 / 設定読込の進捗・失敗・再試行を明示する起動画面。現状は `MainPageViewModel` の `StartupState` で初期化完了を待って初期遷移するだけで、失敗時の再試行導線が無い

#### 2-5 アプリ構成(同書 付録 5)

- [ ] **2-5-0**【判断】採否 — 生成時に画面・権限・パッケージを選ぶ機能プロファイル / アプリ内診断画面(DB・API・端末情報・初期化時間・直近エラー)/ サンプル機能カタログ(カテゴリ・対応 OS・必要権限で検索できるランチャー)/ 重い機能の遅延初期化によるモジュール分割。個別に採否を決める

## 3. リンク集からの取り込み候補

`■MAUI.txd`(リンク集)は全件に判定を付記済み(🟩 取り込む / 🟦 取り込まないが記事として有用 / 🟥 古い・参照不要 / 🟨 要判断)。本節へ移した項目の元行は同書から削除している。本節は取り込む価値のあるトピックだけ。ファイルパスは `Template.MobileApp/` からの相対。

### 3-2 .NET 10 の未適用 API の反映

参照: https://learn.microsoft.com/ja-jp/dotnet/maui/whats-new/dotnet-10?view=net-maui-10.0(要約: https://www.telerik.com/blogs/recap-whats-new-net-maui-net-10)
適用済み: `MauiXamlInflator=SourceGen` / `UseMonoRuntime=false`(CoreCLR)/ `SafeAreaEdges` / Async 系アニメーション API(`TranslateToAsync` 等)/ `Switch.OffColor` / `SearchBar.SearchIconColor` / `SearchBar.ReturnType`(Basic > Setting)/ `Geolocation.IsEnabled`(Device > Location の空状態)/ `Vibration.IsSupported` / `HapticFeedback.IsSupported`(Device > Misc のボタン有効化)/ `dotnet run --device`(`Development.md`)。`ListView` / `TableView` / `MessagingCenter` / `DisplayAlert` / `Page.IsBusy` は未使用のため非推奨化の影響なし。`Shell.NavBarVisibilityAnimationEnabled` は Shell 不使用のため対象外。

残りは UI の追加や共有ライブラリの変更を伴うもの。

| API | 概要 | 現在のファイル名 | 変更 |
| --- | --- | --- | --- |
| `Picker` の Open / Close API、`DatePicker.Date` / `TimePicker.Time` の nullable 化 | プログラムからの開閉、未選択状態(null) | `Modules/Basic/BasicSettingView.xaml` + `BasicSettingViewModel.cs` | 未選択の表現(null)と開くボタン |
| `RefreshView.IsRefreshEnabled` | `IsEnabled` と分離した引き下げ更新の有効 / 無効 | `Modules/Control/ControlRefreshView.xaml` + `ControlRefreshViewModel.cs` | 切替スイッチを追加 |
| `SpeechOptions.Rate` | 読み上げ速度 | `Modules/Device/DeviceMiscView.xaml` + `DeviceMiscViewModel.cs`(Speech カード)、MauiComponents の `ISpeechService.SpeakAsync` | 速度の引数(共有ライブラリ側)と速度スライダー |
| `HybridWebView.WebResourceRequested` / `InvokeJavaScriptAsync`(戻り値なし)/ `WebViewInitializing` / `WebViewInitialized` | リクエストの横取り(ローカル応答・ヘッダ変更)、初期化イベント、JS 例外の .NET 側再スロー | `Modules/Sample/SampleWebAppView.xaml` + `SampleWebAppViewModel.cs` | ローカル応答のデモ |
| `WebView` の Android 全画面動画(`allowfullscreen`)/ JavaScript 有効・無効の platform-specific | Android 固有の WebView 設定 | `Modules/Sample/SampleWebBasicView.xaml` | 設定を追加 |

- [ ] **3-2-1** Basic > Setting(Picker の Open / Close、DatePicker / TimePicker の null)
- [ ] **3-2-2** Control > Refresh(`IsRefreshEnabled`)
- [ ] **3-2-3** Device > Misc(`SpeechOptions.Rate`。MauiComponents 側の変更を含む)
- [ ] **3-2-4** Sample > HybridWebView / Web view

### 3-3 レイアウト診断メトリクス(DiagnosticPanel)

参照: https://learn.microsoft.com/ja-jp/dotnet/maui/whats-new/dotnet-10?view=net-maui-10.0(「Diagnostics」)、https://github.com/dotnet/maui/pull/31058 — `ActivitySource` / `Meter` 名 `"Microsoft.Maui"` で `IView.Measure` / `Arrange` の回数と所要時間(`maui.layout.measure_count` / `measure_duration` / `arrange_count` / `arrange_duration`)を記録する。`System.Diagnostics.Metrics.Meter.IsSupported` のフィーチャースイッチで AOT / トリミング時に無効化できる。

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Shell/DiagnosticPanel.xaml` + `.xaml.cs` | メモリ推移のスパークライン(DEBUG 限定) | `MeterListener` で measure / arrange の回数・平均時間を購読し、メモリの横に表示 |
| `Template.MobileApp.csproj` | ビルド設定 | Release で `Meter.IsSupported` が無効であることを確認(必要なら `RuntimeHostConfigurationOption` で false) |

- [ ] **3-3** 上記。画面遷移で回数が増え、静止時は増えないことを確認

### 3-4 XAML の global xmlns 化

参照: https://learn.microsoft.com/ja-jp/dotnet/maui/whats-new/dotnet-10?view=net-maui-10.0(「Implicit and Global XML namespaces」)— `GlobalXmlns.cs` に `[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "Template.MobileApp.Controls")]` 等を並べ、XAML 側は `xmlns="http://schemas.microsoft.com/dotnet/maui/global"` の 1 行で `controls:` / `behaviors:` / `converters:` 等の接頭辞を省略できる。`XmlnsPrefix` で接頭辞を残す運用も可。ルートの `xmlns` / `xmlns:x` まで省く implicit 版は `MauiAllowImplicitXmlnsDeclaration` + `EnablePreviewFeatures` のプレビュー機能。

- [ ] **3-4-0**【判断】適用範囲 — 案A `GlobalXmlns.cs` の追加のみ(既存 XAML は変更不要、新規画面から接頭辞を省略)/ 案B 全 XAML(約 120 ファイル)の接頭辞を一括で除去 / 見送り。implicit 版(プレビュー)は対象外

### 3-5 StyleClass の活用(文字サイズ × 配置)

方針(付録A): スタイルの基本は共有 `Styles.xaml` からの `BasedOn` 派生。`StyleClass` は複数指定できる利点を、文字サイズ × 配置のように**直交する属性の組み合わせ**にだけ使い、サイズ × 配置の数だけスタイルを用意しなくて済むようにする。色や余白などは Style 側に置き、同じプロパティを Style と StyleClass の両方で指定しない。Crosswind(https://github.com/sthewissen/Plugin.Maui.Crosswind)のような全面的なユーティリティクラスは採用しない。

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- | 
| `Resources/Styles/StyleClasses.xaml` | — | 新規。`Label` 向けのクラス(`Class` 付き Style)を定義: サイズ `size-10` / `size-11` / `size-12` / `size-14` / `size-16` / `size-18` / `size-20` / `size-24` / `size-28` / `size-36` / `size-48`(付録A の許可値のうち使用中のもの)、水平配置 `align-start` / `align-center` / `align-end`、太字 `bold` |
| `App.xaml` | リソース辞書のマージ | `StyleClasses.xaml` を追加(共有 `Styles.xaml` は変更しない) |
| `Modules/Network/NetworkHttpView.xaml` / `NetworkStorageView.xaml` / `NetworkRealtimeView.xaml` / `NetworkGrpcView.xaml` / `NetworkScpView.xaml` | 第 1 弾の適用先 | 画面ローカルの `CaptionLabel` / `ValueLabel` / `LogLabel` 等のうち FontSize だけを持つものを `StyleClass="size-12"` 等に置き換え(色を持つものは BasedOn 派生に残しサイズだけクラスへ) |
| `Document/Change_Summary.md` 付録A | 開発ポリシー | 上記の方針を追記(3-5-1 で実施) |

- [ ] **3-5-1** クラスの定義と `App.xaml` へのマージ、付録A への方針の追記
- [ ] **3-5-2** Network の 5 画面へ適用し、表示が変わらないことを実機で確認
- [ ] **3-5-3** 既存画面は触るときに適用する(一括変更はしない)。Style と StyleClass の同一プロパティ指定が無いことを inspectcode / 目視で確認

### 3-10 Material 3(`UseMaterial3`)

参照: https://devblogs.microsoft.com/dotnet/dotnet-maui-material-3/ — Android の Material 3(Material You)対応(2026-05)。`Microsoft.Maui.Controls` 10.0.60 以降(本プロジェクトは 10.0.100)で csproj に `<UseMaterial3>true</UseMaterial3>` を置くだけで有効(既定 false。Handler / `styles.xml` の変更は不要)。対象は Entry / Editor / SearchBar / RadioButton / ProgressBar / Slider / Picker / TimePicker / DatePicker / CheckBox / Switch / ImageButton / Button / Shell の既定外観(Entry / Editor は outlined の `TextInputLayout`、DatePicker はカレンダー ダイアログ)。XAML / C# で明示した色・スタイルは優先される。未対応: コントロール単位の opt-in、動的カラー トークンの API、NavigationPage / TabbedPage / FlyoutPage / CollectionView / Border の再スタイル。

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Template.MobileApp.csproj` | ビルド設定 | `<UseMaterial3>true</UseMaterial3>` を追加 |
| `Behaviors/EntryOption.android.cs` | Entry / Editor の NoBorder(`BackgroundTintList`)/ フォーカス枠 | outlined `TextInputLayout` 化との干渉を確認(枠線が二重になる場合は NoBorder の実装を Material 3 用に切替) |
| `Resources/Styles/Styles.xaml` | 共有スタイル | 変更しない(明示指定が優先されるため差分は既定外観のみ) |

- [ ] **3-10-0**【判断】採否 — 有効化は 1 行だが Android の入力系コントロールの既定外観が全画面で変わる(Basic / Setting / Kit 系の Entry・Switch・DatePicker が主な影響範囲)。採用時は全画面の目視確認と `Document/*.png` の撮り直しが必要

### 3-12 バックグラウンド定期タスク(WorkManager)

参照: https://www.nuget.org/packages/Shaunebu.MAUI.BackgroundTaskManager — Android は `WorkManager`(最短 15 分間隔)、iOS は `BGTaskScheduler` を使い、CRON 式でフォアグラウンド / バックグラウンドのジョブを登録して `Preferences` に永続化する薄いラッパー(`RegisterJob<T>()` / `Schedule(jobId, cron, callback)` / `ScheduleInBackground(jobId, cron, jobType)`。2025-10、230 DL、リポジトリ公開なし)。採用はせず API の形(ジョブ登録 / CRON 近似 / 永続化)だけ参考にし、`AndroidX.Work` を直接使う。

範囲: 制約付き(ネットワーク接続時 / 充電中)の一回限りワーク + 15 分周期の定期ワーク。実行結果はローカル通知(`Components/NotificationService`)か画面のログに出す。正確な時刻指定は `AlarmManager`(通知で実装済み)の担当のままとし、WorkManager は遅延可・再起動後も残る処理に限定する。

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Template.MobileApp.csproj` | パッケージ | `Xamarin.AndroidX.Work.Runtime` を追加(`dotnet list package --include-transitive` で `Fragment.Ktx` ピン止めとの競合を確認) |
| `Components/BackgroundTask.cs` + `.android.cs` | — | 新規。登録(一回 / 定期)/ 取消 / 状態取得。`AndroidX.Work.Worker` 派生の `DoWork()` で処理 |
| `Modules/Device/DeviceMiscView.xaml` + `DeviceMiscViewModel.cs` | 雑多なデバイス機能(Device メニューは満杯) | `InfoCard` を追加(登録 / 取消 / 最終実行時刻) |
| `MauiProgram.cs` | `ConfigureComponents` | DI 登録 |

- [ ] **3-12-0**【判断】要否 — 再起動後も残る遅延処理(同期 / 送信キュー)の需要があるか。WorkManager は再起動後に自動で再スケジュールされるため `RECEIVE_BOOT_COMPLETED` は不要。常駐(前景サービス)は対象外

