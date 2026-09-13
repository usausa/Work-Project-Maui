# 残作業チェックリスト

残作業(実機確認 / 実テスト / 保留)のマスターチェックリスト。経緯・実装内容・ナレッジ・開発ポリシーは `Change_Summary.md`(付録含む)を参照。
優先順 = 1 節 → 5 節 → 6 節。以降は 2〜4 節。

## 運用ルール

- 作業はこの番号で指示・進行する(例:「5-2 を実施」)。完了した項目は本書から削除し、内容は `Change_Summary.md` に記録する
- **【判断】印の項目はユーザーが決定**(勝手に進めない)。デザイン判断を伴う差分は 1 項目ずつ指示を受けて実施
- 実装・変更を行なう場合の完了条件 = **ビルド警告ゼロ** + `Change_Summary.md` への記録(開発ポリシーは同 付録A)
- コミットはユーザーが実施(グループ単位を推奨)
- 描画・性能の計測は **Release ビルド + 実機**(手順は `Development.md` の「Releaseビルドでの検証と計測」)

## 前提(環境)

- **環境制約 (不具合ではない)**: ①地図タイルは Google Maps API キー未設定だと非表示 (ピン・カメラ移動は動作) ②SampleCvNet 系は AI エンドポイント未設定だと画面に入れない ③CommunityToolkit CameraView の `CaptureAsync` がまれに未完了になり Function キーが無反応化 (再起動で回復)
- 現在実機に入っているのは **Debug ビルド**(2026-09-13 デプロイ。性能・描画の確認時は Release へ入れ替える)

---

## 1. 実機確認で判明した要対応項目

- [ ] **1-1** Basic > Behavior: **`MaskedBehavior` が正しく動作しない**(電話番号の `000-0000-0000` 整形が効かない)。原因調査から
- [ ] **1-2** Basic > Validation: パスワード一致の相関検証が**フォーカスを外した時のみ**動く → **入力が変わる度**に検証する方式へ
- [ ] **1-3** App > Sudoku: 盤面の**線が描画されていない箇所がある**
- [ ] **1-4** App > Calculator: **ボタンをもう少し大きく**したい(上部に余白があるため活用できる)
- [ ] **1-5** View > DragDrop: **ドロップ先が分かりやすい**表現がほしい(現状はゴミ箱のみハイライト)

---

## 2. SCP の実機確認と転送実テスト(要 SSH サーバ)

- [ ] **2-1** Network メニュー: 「SCP」が追加され遷移できる(未設定時は接続先が「未設定 (設定画面の QR で投入)」でボタン無効)
- [ ] **2-2** Main > Setting: 項目の**ラベルと現在値が横並び**で表示される。SCP セクション(Host/User/Password)があり、QR(`ScpHost=...` 形式)を読むと反映される
- [ ] **2-3** Network > SCP: QR 投入後、「アップロード」でファイル選択 → 進捗バー → 完了ログ。「ダウンロード」で同ファイルがキャッシュへ取得される。転送中「キャンセル」で中断。接続後にサーバのホスト鍵指紋が参考表示される(照合は行わない)

---

## 3. 他案件の残課題(優先度低)

- [ ] **3-1** `SecureStorage.GetAsync` の復元・キーストア無効化時の例外が未捕捉 (エッジケース)
- [ ] **3-2** `HttpService` の `CancellationToken` を `NetworkOperator` のデリゲート型経由で呼び出し側から渡せるようにする (現在は口が無く未使用。転送は 10 分の有限タイムアウトで暫定対応済み)

---

## 4. バックログ(任意・後日。指示があれば着手)

- [ ] **4-1**【判断】`Controls/ChatView` バブル色のバインダブル化(C-13・D18): 検討扱い・未確定
- [ ] **4-2** UISocial 背景の専用化(1080×1920 / 9:16 のゲーム風背景)
- [ ] **4-3** `AnimationOption.ResetEnter` の Scale 1 固定リセット(静的 Scale+EnterAnimation 併用が将来出た場合に、TranslationY と同じ基準値退避パターンで対処)

---

## 5.【優先】`tmpl-plan-maui.md` からの移管課題

`D:\GitHubTemplate\tmpl-plan-maui.md`(MAUI トラック強化プラン)の未対応項目(同書の番号を併記)。**keyboard / blazor 向けの対応(同書 §4 / §5)と iOS 対応(同書 3-13。保留継続)は対象外**。
参照サンプルは `C:\Users\machi\Desktop\Maui`(残置 18 件)。ファイルパスは `Template.MobileApp/` からの相対。

### template-maui への反映(同書 §1)

- [ ] **5-1** template-maui README の TODO 実態同期(同書 1-9)。実装済みの **Chat / Chart / Gauge / Calendar / Media / Cognitive(SampleCvNetFace+Azure.AI.Vision)/ HybridWebView(WebViewBind・WebViewController)** が TODO に残っている。未実装の WiFi manager / Biometric / Bottom sheet(独自実装)/ Push / Local notification は TODO のまま残す

### 機能実装(同書 §3)

5-2 / 5-3 は画面・`ViewId`・メニューボタンが配置済み(`DeviceMenuView.xaml` の該当ボタンが `IsEnabled="False"`、画面は `Not implemented` 表示、ViewModel は 8 行)。プラットフォーム実装は `Components/Nfc.cs` + `Nfc.android.cs` と同じ構成(共通インターフェース + `*.android.cs`)に揃える。

#### 5-2 WiFi manager(同書 3-1)

参照: `■MauiWifi`(`maui_wifi_manager-main` + `IWifiNetworkService.shared.cs`)

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Components/WiFiManager.cs` | `IWiFiManager`(6 行、`StateChanged` のみ。実装・DI 登録なし) | 取得 API を追加 |
| `Components/WiFiManager.android.cs` | — | 新規。`Android.Net.Wifi.WifiManager` 実装 |
| `Modules/Device/DeviceWiFiViewModel.cs` | 空スタブ | 実装 |
| `Modules/Device/DeviceWiFiView.xaml` | 空状態表示 | `InfoCard` 構成へ |
| `Modules/Device/DeviceMenuView.xaml` | `Grid.Row="4" Grid.Column="0"` | `IsEnabled="False"` を削除 |
| `MauiProgram.cs` | `ConfigureComponents` の `// Components` 節 | `services.AddSingleton<IWiFiManager, WiFiManager>()` |
| `Platforms/Android/AndroidManifest.xml` | 権限 | `CHANGE_WIFI_STATE` / `NEARBY_WIFI_DEVICES` 追加(`ACCESS_WIFI_STATE` は既存) |
| `Permissions.cs` | 権限要求 | `NEARBY_WIFI_DEVICES` 用の `BasePlatformPermission` 派生 + 要求メソッド(`ActivityRecognition` と同形) |

- [ ] **5-2** 表示: SSID / BSSID / IP アドレス / 信号強度 / リンク速度 / 周波数帯 / 接続状態
  - 制約: Android 10 以降、`WifiInfo.SSID` は位置情報権限が無いと `<unknown ssid>`。既存の `Permissions.RequestLocationAsync()` を `OnNavigatedToAsync` で要求する
  - 制約: `getScanResults` は Android 9 以降スロットリング対象。スキャン一覧を出す場合は結果キャッシュを再利用する
  - 制約: Android 10 以降、任意 SSID への直接接続は不可(`WifiNetworkSuggestion` / `WifiNetworkSpecifier` 経由のみ)

#### 5-3 生体認証(同書 3-2)

参照: `Bio`(`Maui.Biometric-main` / `MauiBiometricPluginSample-main` / `NET-MAUI-FingerPrint-main`)

- [ ] **5-3-0**【判断】実装方式 — 案A `Components/Biometric.cs` + `.android.cs` を自作(`Xamarin.AndroidX.Biometric` を追加)/ 案B `Maui.Biometric` パッケージを参照

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

- [ ] **5-3** 可用性は「ハードウェア無し / 未登録 / 一時利用不可」を区別して表示する。範囲は認証成否の表示まで(鍵の解錠に使う `CryptoObject` は対象外)
  - 制約: `androidx.biometric` は `androidx.fragment` に依存する。csproj は `Xamarin.AndroidX.Fragment.Ktx` をピン止めしているため、追加後に `dotnet list package --include-transitive` で競合を確認する
  - `BiometricPrompt` が要求する `FragmentActivity` は `MainActivity`(`MauiAppCompatActivity` 派生)で満たしている。基底クラスの変更は不要

#### 5-4 Bottom sheet の独自実装(同書 3-3)

View > Toolkit の `SfBottomSheet`(`Modules/View/ViewToolkitView.xaml`)とは別に、Syncfusion に依存しない自作版を検討する。

- [ ] **5-4**【判断】自作版の要否と範囲(ドラッグで開閉するシート + 背景の暗転。`Controls/` へ `Layout` / `ContentView` 派生で追加し、View メニューの画面で実演)

#### 5-5 通知(同書 3-4)

参照: `Plugin.LocalNotification-master`(`Sample/Direct` / `Sample/Maui`)、`0_maui-samples/10.0/PlatformIntegration/LocalNotificationsDemo`、`0_MauiSamples/MauiNotifications`、FCM は `0_maui-samples/10.0/WebServices/PushNotificationsDemo`。`NotificationManager` / `NotificationCompat` の参照は 0 件。

- [ ] **5-5-0**【判断】ローカル通知の実装方式 — 案A `Components/Notification.cs` + `.android.cs` を自作 / 案B `Plugin.LocalNotification` を参照
- [ ] **5-5-1**【判断】画面の置き場 — Device メニューは 9 行 × 2 列で満杯。案A `DeviceMiscView` に `InfoCard` を追加(現在 Screen / Feedback / Light / Speech の 4 枚)/ 案B 新規画面 + Device メニュー 10 行目(9 段規約から外れる)
- [ ] **5-5-2**【判断】プッシュ通知(FCM)の要否 — Firebase プロジェクトと `google-services.json` が前提。採用する場合、トークン表示と受信ログをローカル通知の画面に追記する

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Components/Notification.cs` | — | 新規。即時 / スケジュール / アクションボタン / タップ時ペイロード |
| `Components/Notification.android.cs` | — | 新規。通知チャンネル作成を含む |
| `Modules/Device/DeviceMiscView.xaml` + `DeviceMiscViewModel.cs` | 雑多なデバイス機能 | 案A: `InfoCard` 追加 |
| `Modules/ViewId.cs` + 新規 View / ViewModel | — | 案B: 画面追加 |
| `MauiProgram.cs` | `ConfigureComponents` | DI 登録 |
| `Platforms/Android/AndroidManifest.xml` | 権限 | `POST_NOTIFICATIONS` 追加 |
| `Permissions.cs` | 権限要求 | `POST_NOTIFICATIONS`(Android 13+)の要求メソッド |
| `Platforms/Android/MainActivity.cs` | 起動 Intent | 通知タップ時のペイロード受け取り |

- [ ] **5-5** ローカル通知(即時 / スケジュール / アクションボタン / タップ時ペイロード)
  - 制約: Android 8 以降、通知チャンネルの事前作成が必須。チャンネル ID は `Components/Notification.cs` に定数化
  - 制約: 通知タップからの画面遷移は `StartupState` の初期遷移完了を待ってから行う(`Change_Summary.md` 区間8)
  - 制約: スケジュール通知で `AlarmManager` の完全一致指定を使う場合、Android 12 以降は `SCHEDULE_EXACT_ALARM` が必要

#### その他(同書 §3)

- [ ] **5-6** DB マイグレーション機構(同書 3-5。`DataService.RebuildAsync`=毎起動で物理削除→再作成の user_version ベース置換)。参照: `0_MauiSamples/Database`
- [ ] **5-7** ダークモード(同書 3-6。`UserAppTheme=Light` 固定・`AppThemeBinding` 0 件。Colors.xaml は 4 テンプレートでバイト一致のため**対応するなら 4 本同時が効率的**)。参照: `0_maui-samples/10.0/UserInterface/ThemingDemo` + `SystemThemesDemo`、`maui-toolkit-samples-master/Theme`
- [ ] **5-8** ローカライズ拡充(同書 3-7。resx は Messages / Names とも 5 件のみ。機構は動作済み)。参照: `0_maui-samples/10.0/Fundamentals/Localization`、`0_MauiSamples/MauiLocalization`
- [ ] **5-9** `Controls/SocialControls.cs` の TODO 10 件整理(同書 3-9)
- [ ] **5-10** 【判断】TimeProvider の MAUI 方式(同書 3-10。設定は EmbeddedBuildProperty のビルド時注入方式のため、wpf / avalonia の `AddOptions<T>().ValidateOnStart()` はそのまま移植不可)
- [ ] **5-11** 【判断】Analyzers.ruleset 正典差分 11 ルールの扱い(同書 3-12。CA1416 / CA2007 は MAUI 固有の合理性あり単純追随不可。CA1014 / CA1305 / CA1824 / CA1861 は再検討余地。正典統一トラック〈aidd 側セッション〉と連動)

### ソースレビュー由来(同書 付録)

- [ ] **5-12** `Converters/MailDateTimeStringConverter.cs` の `ConvertBack` 是正(現状 `NotSupportedException` を throw。`Binding.DoNothing` 返却か OneWay 専用の明示へ)

---

## 6. 参照サンプルからの追加機能(`C:\Users\machi\Desktop\Maui`)

`tmpl-plan-maui.md` に無い追加候補。ファイルパスは `Template.MobileApp/` からの相対。

### 6-1 SfNavigationDrawer

参照: `maui-toolkit-samples-master/NavigationDrawer`、`SyncFusion`(ソース)。`Syncfusion.Maui.Toolkit` の `SfNavigationDrawer` は参照 0 件。追加パッケージ不要。

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Modules/View/ViewToolkitView.xaml` + `ViewToolkitViewModel.cs` | Syncfusion / CommunityToolkit の未使用機能の実演(6 種掲載) | ドロワーを追加 |

- [ ] **6-1** ドロワーの追加
  - 制約: Syncfusion の URL 形式 xmlns で解決できない場合は `clr-namespace` 指定(MAUIG1001)

### 6-2 メモリ監視オーバーレイ

参照: `maude-main`。計測は `[SceneStats]` の `Android.Util.Log` 出力のみで、アプリ内の可視化は無い。

- [ ] **6-2-0**【判断】全画面共通のオーバーレイ(`MainPage` の `AbsoluteLayout` 上)にするか、専用画面にするか

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Graphics/Drawing/ChartDrawing.cs` | `ChartKind` = Line / Bar / Donut / Candle / Stacked / Scatter / Heat | `Line` をメモリ推移の描画に流用(新規チャートは作らない) |
| `Modules/Device/DeviceSensorViewModel.cs` | `IAccelerometer` を注入済み | 起動トリガは `IAccelerometer.ShakeDetected` を使う(シェイク判定の自作は不要) |
| 新規オーバーレイ View / ViewModel | — | `GC.GetTotalMemory` / `Process.WorkingSet64` の定期サンプリング + ライブチャート |

- [ ] **6-2** サンプリングとライブチャート

### 6-3 リーク検出(DEBUG 限定)

参照: `MemoryToolkit.Maui-main`。`DisconnectHandler()` の呼び出しは 0 件。

- [ ] **6-3-0**【判断】案A `MemoryToolkit.Maui` を参照(`TrimmerRootAssembly` 追加)/ 案B ページ離脱時の `DisconnectHandler()` 呼び出しのみ自前で入れる

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `MauiProgram.cs` | `ConfigureDebug` | DEBUG 限定で有効化(`GC.Collect()` を多用するため Release では無効) |
| `Template.MobileApp.csproj` | パッケージ / トリミング | 案A: パッケージ + `TrimmerRootAssembly` 追加 |

- [ ] **6-3** 有効化と検出結果の確認

### 6-4 アクセシビリティ(SemanticProperties / AutomationId)

参照: `0_maui-samples/10.0/Apps/DeveloperBalance/Pages/*.xaml`(`SemanticProperties` の付け方)、`0_maui-samples/10.0/UITesting/BasicAppiumNunitSample/MauiApp/MainPage.xaml`(`AutomationId` の付け方)。`SemanticProperties` / `AutomationId` は 0 件。

- [ ] **6-4-0**【判断】全画面に入れるか、代表画面(`Modules/Basic/*`)のみのサンプルに留めるか
- [ ] **6-4** `SemanticProperties.Description` / `Hint` / `HeadingLevel` をアイコンのみのボタンと `InfoCard` の見出しに付け、TalkBack で読み上げ順を実機確認する

### 6-5 画面録画(任意)

参照: `Plugin.Maui.ScreenRecording-main`

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Components/ScreenRecorder.cs` + `.android.cs` | — | 新規。`MediaProjection` + `MediaRecorder` |
| `Platforms/Android/` 前景サービス | — | 新規。テンプレートに前景サービスの実装例は無い |
| `Platforms/Android/AndroidManifest.xml` | 権限 / サービス | `FOREGROUND_SERVICE` / `FOREGROUND_SERVICE_MEDIA_PROJECTION` + `<service>` 宣言 |
| `Modules/Device/DeviceMiscView.xaml` | — | 開始 / 停止 / 保存先表示 |

- [ ] **6-5** 録画の開始 / 停止 / 保存先表示
  - 制約: Android 14 以降、`MediaProjection` は前景サービス(`mediaProjection` タイプ)からの開始が必須
