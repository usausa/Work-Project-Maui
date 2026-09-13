# 残作業チェックリスト

残作業(実機確認 / 実テスト / 保留)のマスターチェックリスト。経緯・実装内容・ナレッジ・開発ポリシーは `Change_Summary.md`(付録含む)を参照。
優先順 = 2 節 → 3 節。以降は 1 節。

## サマリ

| Category | Feature | 章 |
| --- | --- | --- |
| Network | SCP の実機確認と転送実テスト(要 SSH サーバ) | 1 |
| Device | WiFi manager | 2-1 |
| Device | Biometric(生体認証) | 2-2 |
| UI | Bottom sheet / Navigation drawer(SfNavigationDrawer or 自作) | 2-3 |
| Device | Local notification / Push(FCM) | 2-4 |
| Other | Cognitive service / Chat AI(Azure / AI サービス利用部分の実装) | 2-5 |
| Control | Control メニューの新設(Toolkit / Custom / Sf Chart の移動) | 3-1 |
| Control | Grid(ClamGrid) | 3-2 |
| Control | Card list(CollectionView の一覧パターン) | 3-3 |
| Diagnostics | メモリ監視オーバーレイ | 3-4 |
| Diagnostics | リーク検出(DEBUG 限定) | 3-5 |
| Device | 画面録画(任意) | 3-6 |

## 運用ルール

- 作業はこの番号で指示・進行する(例:「2-1 を実施」)。完了した項目は本書から削除し、内容は `Change_Summary.md` に記録する
- **【判断】印の項目はユーザーが決定**(勝手に進めない)。デザイン判断を伴う差分は 1 項目ずつ指示を受けて実施
- 実装・変更を行なう場合の完了条件 = **ビルド警告ゼロ** + `Change_Summary.md` への記録(開発ポリシーは同 付録A)
- コミットはユーザーが実施(グループ単位を推奨)
- `README.md` の TODO 表は本書のサマリ表(2〜3 節)と同期させる(項目の追加・削除・完了時に両方を更新。TODO 表に本書の番号は書かない)
- 描画・性能の計測は **Release ビルド + 実機**(手順は `Development.md` の「Releaseビルドでの検証と計測」)

## 前提(環境)

- **環境制約 (不具合ではない)**: ①地図タイルは Google Maps API キー未設定だと非表示 (ピン・カメラ移動は動作) ②SampleCvNet 系は AI エンドポイント未設定だと画面に入れない ③CommunityToolkit CameraView の `CaptureAsync` がまれに未完了になり Function キーが無反応化 (再起動で回復)
- 現在実機に入っているのは **Debug ビルド**(2026-09-13 デプロイ。性能・描画の確認時は Release へ入れ替える)

---

## 1. SCP の実機確認と転送実テスト(要 SSH サーバ)

- [ ] **1-1** Network メニュー: 「SCP」が追加され遷移できる(未設定時は接続先が「未設定 (設定画面の QR で投入)」でボタン無効)
- [ ] **1-2** Main > Setting: 項目の**ラベルと現在値が横並び**で表示される。SCP セクション(Host/User/Password)があり、QR(`ScpHost=...` 形式)を読むと反映される
- [ ] **1-3** Network > SCP: QR 投入後、「アップロード」でファイル選択 → 進捗バー → 完了ログ。「ダウンロード」で同ファイルがキャッシュへ取得される。転送中「キャンセル」で中断。接続後にサーバのホスト鍵指紋が参考表示される(照合は行わない)

---

## 2.【優先】`tmpl-plan-maui.md` からの移管課題

`D:\GitHubTemplate\tmpl-plan-maui.md`(MAUI トラック強化プラン)の未対応項目(同書の番号を併記)。**keyboard / blazor 向けの対応(同書 §4 / §5)と iOS 対応(同書 3-13。保留継続)は対象外**。
参照サンプルは `C:\Users\machi\Desktop\Maui`(残置 18 件)。ファイルパスは `Template.MobileApp/` からの相対。

### 機能実装(同書 §3)

2-1 / 2-2 は画面・`ViewId`・メニューボタンが配置済み(`DeviceMenuView.xaml` の該当ボタンが `IsEnabled="False"`、画面は `Not implemented` 表示、ViewModel は 8 行)。プラットフォーム実装は `Components/Nfc.cs` + `Nfc.android.cs` と同じ構成(共通インターフェース + `*.android.cs`)に揃える。

#### 2-1 WiFi manager(同書 3-1)

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

- [ ] **2-1** 表示: SSID / BSSID / IP アドレス / 信号強度 / リンク速度 / 周波数帯 / 接続状態
  - 制約: Android 10 以降、`WifiInfo.SSID` は位置情報権限が無いと `<unknown ssid>`。既存の `Permissions.RequestLocationAsync()` を `OnNavigatedToAsync` で要求する
  - 制約: `getScanResults` は Android 9 以降スロットリング対象。スキャン一覧を出す場合は結果キャッシュを再利用する
  - 制約: Android 10 以降、任意 SSID への直接接続は不可(`WifiNetworkSuggestion` / `WifiNetworkSpecifier` 経由のみ)

#### 2-2 生体認証(同書 3-2)

参照: `Bio`(`Maui.Biometric-main` / `MauiBiometricPluginSample-main` / `NET-MAUI-FingerPrint-main`)

- [ ] **2-2-0**【判断】実装方式 — 案A `Components/Biometric.cs` + `.android.cs` を自作(`Xamarin.AndroidX.Biometric` を追加)/ 案B `Maui.Biometric` パッケージを参照

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

- [ ] **2-2** 可用性は「ハードウェア無し / 未登録 / 一時利用不可」を区別して表示する。範囲は認証成否の表示まで(鍵の解錠に使う `CryptoObject` は対象外)
  - 制約: `androidx.biometric` は `androidx.fragment` に依存する。csproj は `Xamarin.AndroidX.Fragment.Ktx` をピン止めしているため、追加後に `dotnet list package --include-transitive` で競合を確認する
  - `BiometricPrompt` が要求する `FragmentActivity` は `MainActivity`(`MauiAppCompatActivity` 派生)で満たしている。基底クラスの変更は不要

#### 2-3 Bottom sheet / Navigation drawer(同書 3-3)

View > Toolkit の `SfBottomSheet`(`Modules/View/ViewToolkitView.xaml`)とは別に、Syncfusion に依存しない自作版を検討する。ドロワーも同じ枠で扱い、`Syncfusion.Maui.Toolkit` の `SfNavigationDrawer`(参照 0 件・追加パッケージ不要)と自作版をあわせて検討する。

参照: `maui-toolkit-samples-master/NavigationDrawer/GettingStarted`(`DrawerSettings` に `DrawerHeaderView` / `DrawerContentView` / `DrawerFooterView`、`DrawerWidth` / `Position` / `Transition`、開閉は `IsOpen` または `ToggleDrawer()`)、`SyncFusion`(ソース)

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Modules/View/ViewToolkitView.xaml` + `ViewToolkitViewModel.cs` | Syncfusion / CommunityToolkit の未使用機能の実演(6 種掲載) | `SfNavigationDrawer` を採用する場合にカードを追加(`IsOpen` を VM にバインド) |
| `Controls/` | — | 自作の Bottom sheet / Drawer(`Layout` / `ContentView` 派生)。実演画面は Control メニュー(3-1)に置く |

- [ ] **2-3-0**【判断】Bottom sheet 自作版の要否と範囲(ドラッグで開閉するシート + 背景の暗転)
- [ ] **2-3-1**【判断】Drawer の実装方式 — `SfNavigationDrawer` の追加 / 自作ドロワー(端からのスワイプで開閉 + 背景の暗転)/ 両方
- [ ] **2-3** 決定した方式の実装
  - 制約: Syncfusion の URL 形式 xmlns で解決できない場合は `clr-namespace` 指定(MAUIG1001)

#### 2-4 通知(同書 3-4)

参照: `Plugin.LocalNotification-master`(`Sample/Direct` / `Sample/Maui`)、`0_maui-samples/10.0/PlatformIntegration/LocalNotificationsDemo`、`0_MauiSamples/MauiNotifications`、FCM は `0_maui-samples/10.0/WebServices/PushNotificationsDemo`。`NotificationManager` / `NotificationCompat` の参照は 0 件。

- [ ] **2-4-0**【判断】ローカル通知の実装方式 — 案A `Components/Notification.cs` + `.android.cs` を自作 / 案B `Plugin.LocalNotification` を参照
- [ ] **2-4-1**【判断】画面の置き場 — Device メニューは 9 行 × 2 列で満杯。案A `DeviceMiscView` に `InfoCard` を追加(現在 Screen / Feedback / Light / Speech の 4 枚)/ 案B 新規画面 + Device メニュー 10 行目(9 段規約から外れる)
- [ ] **2-4-2**【判断】プッシュ通知(FCM)の要否 — Firebase プロジェクトと `google-services.json` が前提。採用する場合、トークン表示と受信ログをローカル通知の画面に追記する

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

- [ ] **2-4** ローカル通知(即時 / スケジュール / アクションボタン / タップ時ペイロード)
  - 制約: Android 8 以降、通知チャンネルの事前作成が必須。チャンネル ID は `Components/Notification.cs` に定数化
  - 制約: 通知タップからの画面遷移は `StartupState` の初期遷移完了を待ってから行う(`Change_Summary.md` 区間8)
  - 制約: スケジュール通知で `AlarmManager` の完全一致指定を使う場合、Android 12 以降は `SCHEDULE_EXACT_ALARM` が必要

### Azure / AI サービス利用部分の実装(README TODO の Cognitive service / Chat AI)

`Azure.AI.Vision.ImageAnalysis` 1.0.0 / `Azure.AI.Vision.Face` 1.0.0-beta.2 / `OllamaSharp` 5.4.30 は csproj で参照済みだがコードから未使用。接続先は Setting 画面の QR で投入する `Settings.AIServiceEndPoint` + `GetAIServiceKeyAsync()`(`SampleCvNetMenu` は未設定時に遷移を止める)。

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Modules/Sample/SampleCvNetObjectViewModel.cs` / `SampleCvNetTagViewModel.cs` / `SampleCvNetPeopleViewModel.cs` / `SampleCvNetOcrViewModel.cs` | 撮影して静止画を表示するところまで(撮影後が `// TODO`。`DetectDrawing` はコメントアウト) | `ImageAnalysisClient` で物体 / タグ / 人物 / 文字を解析し結果を描画 |
| `Modules/Sample/SampleCvNetFaceViewModel.cs` | 同上 | `FaceClient` で顔検出し結果を描画 |
| `Modules/Sample/SampleCvNet*View.xaml` | カメラ + 静止画 | 結果描画用の `GraphicsView`(`SampleCvLocalView` と同構成)を追加 |
| `Usecase/CognitiveUsecase.cs` | ローカル ONNX の物体検出(`DetectAsync`)のみ | Azure 呼び出しを追加するか、別ユースケースに分ける |
| `Modules/Sample/SampleChatViewModel.cs` | 応答は固定文の疑似ストリーミング、音声入力は `MockTranscript` | `OllamaSharp` で実応答(ストリーミング)。音声入力は `Components/Speech` の文字起こしへ |
| `State/Settings.cs` + `Modules/Main/SettingView.xaml` / `SettingViewModel.cs` | AI Service の EndPoint / Key | Ollama の接続先とモデル名を追加(QR 投入・表示) |
| `MauiProgram.cs` | `ConfigureComponents` | クライアントの DI 登録 |
| `README.md` | TODO 表の Cognitive service / Chat AI | 完了時に Implement 側へ |

- [ ] **2-5** `SampleCvNet*` 5 画面の解析呼び出しと結果描画、`SampleChat` の実応答・文字起こし
  - 制約: `Azure.AI.Vision.Face` はプレビュー版(beta.2)。API 変更に追従できるよう呼び出しはユースケース側に閉じ込める
  - 制約: 未設定時は現状どおり `SampleCvNetMenu` で止める。Ollama 未設定時の `SampleChat` は疑似応答のままにするか、同様に止めるかを実装時に決める

---

## 3. Control メニューの新設と追加サンプル

3-1〜3-3 は一体で扱う(新設する Control メニューに 3-2 / 3-3 と 2-3 の実装先を置く)。3-4〜3-6 は参照サンプル(`C:\Users\machi\Desktop\Maui`)からの追加候補。ファイルパスは `Template.MobileApp/` からの相対。

### 3-1 Control メニューの新設(SF 系などの配置換え)

メインメニューは 9 段 × 2 列で Row 4 の `View` だけが 2 列分。View メニューは 18 枠が満杯(17 画面 + 空き 1)、Sample は空き 5、UI 2 は空き 2。

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Modules/Main/MenuView.xaml` | メインメニュー | Row 4 を `View` / `Control` の 2 列に |
| `Modules/ViewId.cs` | 画面 ID | `ControlMenu` と各画面の ID を追加。移動する画面は `View*` / `Sample*` → `Control*` に改名 |
| `Modules/Control/ControlMenuView.xaml` + `ControlMenuViewModel.cs` | — | 新規(9 段 × 2 列。グループごとに行分け、空きは可視の無効ボタン) |
| `Modules/View/ViewToolkitView.xaml` + VM | Syncfusion / CommunityToolkit の実演(SfTabView / SfBottomSheet / SfOtpInput / SfSegmentedControl / SfChipGroup / SfAccordion / AvatarView / Expander / RatingView) | `Modules/Control/ControlToolkitView` へ移動 |
| `Modules/View/ViewCustomView.xaml` + VM | 自作コントロールの実演(MarqueeLabel / TreeView / DurationPicker / ColorPicker / AvatarGroup) | `Modules/Control/ControlCustomView` へ移動 |
| `Modules/Sample/SampleSfChartView.xaml` + VM | Syncfusion チャート(Cartesian / Polar / Sunburst / Spark) | `Modules/Control/ControlSfChartView` へ移動 |
| `Modules/View/ViewMenuView.xaml` / `Modules/Sample/SampleMenuView.xaml` | 各メニュー | 移動分の枠を空き(可視の無効ボタン)に |
| `README.md` の Implement 表 / `Document/Sample_SfChart.png` | 一覧・画像 | Control 行を追加、移動画面の画像名を `Control_*.png` に |

- [ ] **3-1-0**【判断】新設の可否 — 案A Control メニューを新設(推奨。View は満杯で 3-2 / 3-3 を置く枠が無い)/ 案B 既存メニューの空きに分散(Sample 5 枠・UI 2 の 2 枠)
- [ ] **3-1-1**【判断】移動対象 — Toolkit / Custom / Sf Chart の 3 画面(+ 2-3 の Bottom sheet / Drawer の実装先)。Sample > Chart(自作 `DrawingControl`)は Sample に残す
- [ ] **3-1** メニュー新設と移動。戻り先(`OnNotifyBackAsync`)と `Document/*.png` / README の Implement 表を合わせる

### 3-2 Grid(ClamGrid)

参照: `https://github.com/usausa/clam-grid`(NuGet `ClamGrid` 1.0.0。SkiaSharp 4.151.2 依存で本プロジェクトと同版、`UseSkiaSharp()` は呼び出し済み)。Example の `Modules/Grid`(一覧 / 条件付き着色 / 列設定 / 入力)と `Modules/Ticket`(業務画面)を参考にするが、見た目は Example より今風にする。

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Template.MobileApp.csproj` | パッケージ | `ClamGrid` 1.0.0 を追加 |
| `Modules/Control/ControlGridView.xaml` + `ControlGridViewModel.cs` | — | 新規。列は XAML の `GridColumn` + 静的 `GridValueAccessorCollection<T>`、`GridDataView<T>` にソート / 複数選択 / ブール編集、`FrozenColumnCount`、`CellColors` / `RowBackground` で状態色。見出しタップでソート、長押しで列設定 |
| `Modules/Control/ControlGridColumnView.xaml` + VM | — | 新規。列の表示 / 順序の設定画面(`GridColumnConfigurationEventArgs.CreateEditSession()` → 戻りで `ColumnOrders` に反映) |
| `Models/Control/GridRow.cs` | — | 新規。行モデルとダミーデータ(数千行) |
| 画面側 ResourceDictionary | — | `GridStyle` を定義(共有 `Styles.xaml` は変更しない) |

- [ ] **3-2-0**【判断】画面構成 — 一覧 + 列設定の 2 画面(推奨)/ 一覧のみ / Ticket 相当の業務画面風も追加
- [ ] **3-2** 実装。デザインは `GridStyle` で今風に: 縦罫線なし(`ShowVerticalLines=False`)・行ヘッダなし・白地に太字の見出し・薄いグレーの横罫線・淡いブルーの選択・アプリのフォント(`FontFamily`)・状態列だけ色付け
  - 制約: セルは文字列とブールのみ(アイコン / チップは描けない。絵文字は `GridFonts` のフォールバックで可)
  - 制約: グリッドは自前でスクロールするため `Grid` の `*` 行など確定サイズで置く

### 3-3 Card list(CollectionView の一覧パターン。`D:\Project\KDH\Sample` から移植)

`KDH.Sample.MobileApp` は FlexGrid の一覧を `CollectionView` のカード表示に置き換えた検討用サンプル。行タップで選択(単一 / 複数、選択 > 状態色の背景)、カード右端の展開ボタンで残り項目を開閉、ツールバーの並替パネル(複数キー、順位表示、昇降反転)/ 全展開 / 再読込、F3 一括選択、F4 確定ダイアログ。

| 移植元(`KDH.Sample.MobileApp/`) | 何用か | 移植先 |
| --- | --- | --- |
| `Models/Cards/CardItem.cs` / `CardCollection.cs`(346 行、`ObservableCollection` 派生)/ `Sorting.cs` / `CardText.cs` | 選択・展開状態、選択件数、複数キーソート | `Models/Control/` |
| `Modules/List/Parts/CardListToolbar.xaml` + `CardListPart.cs` | 並替 / 昇降 / 全展開 / 再読込のツールバーとソートパネル | `Controls/CardListToolbar` |
| `Modules/List/Parts/Inspection1CardList.xaml` ほか(エントリ種別ごとの DataTemplate) | カード表示 | 1 種別に絞り `Modules/Control/ControlCardListView.xaml` に内包 |
| `Modules/List/CardListViewModelBase.cs` / `Inspection1ListViewModelBase.cs` | 一覧共通ロジック(ソートパネル、選択件数、全展開、F キー) | `ControlCardListViewModel.cs` に統合 |
| `Resources/Styles/Cards.xaml` | 背景色 / アイコンのコンバータとラベルスタイル | 画面側 ResourceDictionary(共有 `Styles.xaml` は変更しない) |
| `Modules/List/Sample*Data.cs` / `Models/*.cs` / `Domain/*` | 既存アプリ由来のモデルとダミーデータ | 持ち込まず、汎用のダミー(訪問先一覧など)に置き換え |

- [ ] **3-3-0**【判断】置き換えか新規か — 案A 新規 `Control > Card List`(推奨。View > Collection はグループ化 + SwipeView の基本サンプルとして残す)/ 案B View > Collection を置き換え
- [ ] **3-3-1**【判断】移植範囲 — 1 種別(複数選択 + 展開 + 並替パネル + ツールバー + 状態色 + F キー)/ 単一選択の種別も加える
- [ ] **3-3** 実装(ダミーデータは汎用の内容に差し替え、既存アプリの項目名は持ち込まない)

### 3-4 メモリ監視オーバーレイ

参照: `maude-main`。計測は `[SceneStats]` の `Android.Util.Log` 出力のみで、アプリ内の可視化は無い。

- [ ] **3-4-0**【判断】全画面共通のオーバーレイ(`MainPage` の `AbsoluteLayout` 上)にするか、専用画面にするか

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Graphics/Drawing/ChartDrawing.cs` | `ChartKind` = Line / Bar / Donut / Candle / Stacked / Scatter / Heat | `Line` をメモリ推移の描画に流用(新規チャートは作らない) |
| `Modules/Device/DeviceSensorViewModel.cs` | `IAccelerometer` を注入済み | 起動トリガは `IAccelerometer.ShakeDetected` を使う(シェイク判定の自作は不要) |
| 新規オーバーレイ View / ViewModel | — | `GC.GetTotalMemory` / `Process.WorkingSet64` の定期サンプリング + ライブチャート |

- [ ] **3-4** サンプリングとライブチャート

### 3-5 リーク検出(DEBUG 限定)

参照: `MemoryToolkit.Maui-main`。`DisconnectHandler()` の呼び出しは 0 件。

- [ ] **3-5-0**【判断】案A `MemoryToolkit.Maui` を参照(`TrimmerRootAssembly` 追加)/ 案B ページ離脱時の `DisconnectHandler()` 呼び出しのみ自前で入れる

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `MauiProgram.cs` | `ConfigureDebug` | DEBUG 限定で有効化(`GC.Collect()` を多用するため Release では無効) |
| `Template.MobileApp.csproj` | パッケージ / トリミング | 案A: パッケージ + `TrimmerRootAssembly` 追加 |

- [ ] **3-5** 有効化と検出結果の確認

### 3-6 画面録画(任意)

参照: `Plugin.Maui.ScreenRecording-main`

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Components/ScreenRecorder.cs` + `.android.cs` | — | 新規。`MediaProjection` + `MediaRecorder` |
| `Platforms/Android/` 前景サービス | — | 新規。テンプレートに前景サービスの実装例は無い |
| `Platforms/Android/AndroidManifest.xml` | 権限 / サービス | `FOREGROUND_SERVICE` / `FOREGROUND_SERVICE_MEDIA_PROJECTION` + `<service>` 宣言 |
| `Modules/Device/DeviceMiscView.xaml` | — | 開始 / 停止 / 保存先表示 |

- [ ] **3-6** 録画の開始 / 停止 / 保存先表示
  - 制約: Android 14 以降、`MediaProjection` は前景サービス(`mediaProjection` タイプ)からの開始が必須
