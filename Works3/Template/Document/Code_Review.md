# Template (MAUI テンプレートプロジェクト) コードレビュー

- **対象**: `Works3/Template` (Template.MobileApp)
- **レビュー日**: 2026-08-05
- **規模**: C# 477ファイル / 約43,500行、XAML 131ファイル / 約22,900行、画面(ViewModel) 119個
- **ターゲット**: .NET 10 / MAUI 10.0.80、現在は `net10.0-android` のみ (minSdk 30)
- **修正プラン**: 本レビューに基づく対応計画は [Fix_Plan.md](Fix_Plan.md) を参照

---

## 1. プロジェクト概要

業務向けMAUIアプリケーションの標準構成を示すテンプレート兼機能サンプル集。自作のSmartライブラリ群 (Smart.Navigation / Smart.Resolver / Smart.Mvvm / Smart.Data.Mapper / Rester) を基盤とし、以下の層構造をとる。

```
App / MainPage (単一ContentPage = 自前シェル: タイトルバー + F1〜F4ボタン)
  └ Modules/<分類>/<画面>View + ViewModel   … enum ViewId によるIDベース遷移
       ↓
     Usecase (NetworkUsecase / NetworkOperator / CognitiveUsecase)
       ↓
     Services (DataService=SQLite / HttpService=REST)
     State (Settings / Session / DeviceState)
     Components (NFC / BLE / OCR / 騒音計 等 — partial による OS 分岐)
```

設計方針は [Development.md](Development.md) に約1,400行にわたり文書化されており、レイヤ責務・非同期・例外・ログ設計の指針が明確。**テンプレートとしての設計思想の質は高い**。

---

## 2. 総評

| 観点 | 評価 | 補足 |
|---|:---:|---|
| アーキテクチャ設計・文書化 | **A** | 層構造が明確。Development.md の指針は実務水準 |
| 静的解析・ビルド規律 | **A-** | AnalysisMode=All + StyleCop + nullable警告エラー化。ただし `#pragma` 抑止75箇所と NU1903 抑止が方針と乖離 |
| XAML品質 (compiled binding) | **A** | DataTemplate の `x:DataType` 100%、FontSize許可値違反 0件 |
| デバイス連携コードの堅牢性 | **C** | リソースリーク・権限処理・スレッド安全性に問題が集中 |
| データ・通信層 | **B-** | SQLインジェクション0件は良好。非同期誤用・TZ不整合・CancellationToken欠落あり |
| セキュリティ | **C+** | QR無検証受け入れ、APIキー平文保存、脆弱性警告の抑止 |
| アクセシビリティ | **F** | SemanticProperties / AutomationId が全ソースで 0件 |
| ローカライズ | **D** | resx は実質スケルトン。XAML内に日本語リテラル168箇所 |

**結論**: MVVM基盤・ナビゲーション・DI・スタイル統制といった「テンプレートの骨格」は非常に良くできている。一方で、**デバイス連携層(Components)に確定バグとリークが集中**しており、また「テンプレート=コピーされる見本」であるがゆえに、例外処理・権限・セキュリティの省略パターンがそのまま実案件へ波及するリスクが大きい。下記 High 群(11件)は優先的な修正を推奨する。

---

## 3. 良い点

1. **IDベースナビゲーションのゼロコンフィグ登録** — `enum ViewId` + Source Generator (`[ViewSource]`) で View登録の書き忘れが構造的に発生しない ([MauiProgram.cs:285,339-343](../Template.MobileApp/MauiProgram.cs))
2. **シェルと画面の完全分離** — 画面側は添付プロパティ (`ShellProperty`) を宣言するだけで、シェル実装を知らない
3. **compiled binding の徹底** — `MauiStrictXamlCompilation` + `MauiXamlInflator=SourceGen` 有効、DataTemplate 46箇所すべてに `x:DataType`
4. **Rx購読ライフサイクルの統一** — ほぼ全VMが `Disposables.Add(...ObserveOnCurrentContext().Subscribe(...))` パターン
5. **プラットフォーム分岐の partial 方式** — 共通側でポリシー(ループ/キャンセル/イベント)、Android側で素朴なI/Oのみという責務分割 (`NoiseMonitor` が模範例)
6. **通信エラー処理の一元化** — リトライ/エラー表示ポリシーが `NetworkOperator` に集約され各VMに散らない
7. **Converterの色・文言外部注入ルールの徹底** — 決め打ちせずXAMLから注入 (Development.md の方針どおり)
8. **数値統制** — FontSize 許可値リスト(6〜160の20段階)に対し**違反 0件**
9. **SQLインジェクション対策** — 全クエリがバインドパラメータ。文字列組み立てによるSQLは 0件
10. **シークレットの外部注入設計** — `.EmbeddedProperty.props` (gitignore済) からビルド時注入、リポジトリ内の既定値は空
11. **DEBUG限定の診断パネル** (FPS/CPU/Mem/GC) や物理キー制御など、業務端末向けの実務的な作り込み

---

## 4. 指摘事項 — High (実バグ・クラッシュ・セキュリティ)

### H-1. `RegisterReceiver` の Exported フラグ未指定 → Android 14+ で即クラッシュ
[Components/BluetoothSerial.android.cs:84,110](../Template.MobileApp/Components/BluetoothSerial.android.cs)。targetSdk 34+ では `RECEIVER_EXPORTED / NOT_EXPORTED` 未指定の動的Receiver登録は `SecurityException`。net10.0-android のtargetSdkでは確実に踏む。

### H-2. TextAlignment → Gravity 変換の左右反転
[Behaviors/Extensions.android.cs:12-13](../Template.MobileApp/Behaviors/Extensions.android.cs)
```csharp
Microsoft.Maui.TextAlignment.Start => GravityFlags.Right,   // 逆
Microsoft.Maui.TextAlignment.End   => GravityFlags.Left,    // 逆
```
`ButtonOption.EnableTextAlignment` 経由で `Start` 指定の文字が右寄せになる。RTL対応も考慮するなら `GravityFlags.Start/End` を使うべき。

### H-3. Dispose済み `SKBitmap` を ImageSource にバインド
[Modules/Sample/SampleCvLocalViewModel.cs:73-74](../Template.MobileApp/Modules/Sample/SampleCvLocalViewModel.cs) ほか `SampleCvNetObjectViewModel.cs:74-75`、`SampleCvNetTagViewModel.cs:71-72` の計3箇所。
```csharp
using var bitmap = ImageHelper.ToNormalizeBitmap(input);
Image.Bitmap = bitmap;      // メソッド終了時に Dispose される
```
逆に `UITreeMapViewModel.cs:80,89` は `using` なしで解放漏れ。同一処理で真逆の扱いになっており、描画タイミング次第で解放済みビットマップの描画になる。

### H-4. 同期 `Using` に async デリゲートを渡している (接続Dispose後のawait)
[Services/DataService.cs:130-135](../Template.MobileApp/Services/DataService.cs)。この2メソッドだけ `UsingAsync` ではなく同期版 `Using` を使用しており、`ValueTask` を返した直後に接続が Dispose される。Microsoft.Data.Sqlite が内部同期実行のため偶然動いているだけの脆い状態で、`EditListViewModel.cs:32` の実利用パスに乗っている。

### H-5. QRコードから通信先・APIキーを無検証で受け入れ
[Modules/Main/SettingViewModel.cs:35-62](../Template.MobileApp/Modules/Main/SettingViewModel.cs)。スキーム検証(https強制)・ユーザー確認なしで `ApiEndPoint` / `AIServiceKey` を書き換え可能。悪意あるQRによる通信先すり替えの典型経路。さらに `UriFormatException` 時も `settings.ApiEndPoint` は**例外前に保存済み**で、不正値が永続化されたまま無通知 (`catch { /* Do nothing */ }`)。

### H-6. APIキーを平文 `Preferences` に保存
[State/Settings.cs:42-46](../Template.MobileApp/State/Settings.cs)。`AIServiceKey` が Android では平文 SharedPreferences に入る。`SecureStorage` はプロジェクト全体で未使用。キー類は `SecureStorage` へ。

### H-7. `NfcReader` の解放処理が皆無 (Activityリーク + タグ接続リーク)
[Components/Nfc.android.cs:47-72,110-113](../Template.MobileApp/Components/Nfc.android.cs)。DIシングルトンが `RegisterActivityLifecycleCallbacks` したまま `Dispose` オーバーライドなし。加えて `AndroidNfcF` は `Connect()` するのに `Close()` を一度も呼ばず、タグ検出のたびに接続が残る。

### H-8. `NoiseMonitor` の Stop/Start 競合 → ArrayPool 二重返却
[Components/NoiseMonitor.cs:45-59](../Template.MobileApp/Components/NoiseMonitor.cs)。`Stop()` がループ完了を待たないため、直後の `Start()` で新インスタンス初期化後に**前ループの `CleanupMeasure()` が新しいバッファを返却**し得る (プール破壊)。CTS の Cancel/Dispose 順序にも `ObjectDisposedException` の窓。シングルトン登録のため複数画面からの操作で顕在化しやすい。

### H-9. ActivityRecognition 権限の要求結果を待たずにリスナー登録
[Components/ActivityRecognizer.android.cs:39-44](../Template.MobileApp/Components/ActivityRecognizer.android.cs)。`RequestPermissions` 直後に同期で `RegisterListener`。`OnRequestPermissionsResult` の受け口も存在せず (MainActivity は空)、拒否時は黙って動かない。`CheckSelfPermission` による事前判定もなく毎回ダイアログ要求。

### H-10. 起動初期化の競合 — `async void` の DB 再構築と画面遷移
[ApplicationInitializer.cs:12,43](../Template.MobileApp/ApplicationInitializer.cs) が `async void` のため、`RebuildAsync()`(DB全削除→再作成) の完了前に [App.xaml.cs:31-44](../Template.MobileApp/App.xaml.cs) の `OnStart` が画面遷移し得る。例外発生時は捕捉不能でクラッシュ直行。初期化完了の待機点(Task保持 or 初期画面側でawait)が必要。

### H-11. Style の TargetType 不一致 (実バグ)
[Modules/View/ViewCollectionView.xaml:26-30](../Template.MobileApp/Modules/View/ViewCollectionView.xaml)。`x:Key="AddressHeaderGrid"` が `TargetType="Label"` のまま **Grid に適用** (:133)。`Padding` は効かず、環境によっては実行時例外。

---

## 5. 指摘事項 — Medium

### 例外処理・非同期

| # | 内容 | 場所 |
|---|---|---|
| M-1 | **119 VM中、catch を持つのは4つのみ**。センサー非搭載端末では `barometer.Start()` 等の `FeatureNotSupportedException` で画面遷移ごと失敗 | `Modules/Device/DeviceSensorViewModel.cs:105-110` |
| M-2 | 失敗時に状態復帰しないステートマシン — IO例外で `State=Printing` / `IsBusy=true` のままコマンド永久無効化 | `Modules/Device/DeviceBluetoothViewModel.cs:52-60` |
| M-3 | バックキーが fire-and-forget (`NotifyAsync` のTask破棄、例外未観測) | `MainPage.xaml.cs:16` |
| M-4 | BLEスキャンの Rx `OnError` 未処理 (OnErrorはアプリを落とす) | `Modules/Device/DeviceBleScanViewModel.cs:18-22` |
| M-5 | OCR失敗の完全握りつぶし (`OnFailure => TrySetResult(null)`、ログなし、「文字なし」と区別不能) | `Components/OcrReader.android.cs:46-49` |
| M-6 | BluetoothSerial: `await tcs.Task` の永久待ち (タイムアウト実装がコメントアウトのまま)、Receiver解除が try/finally 外 | `BluetoothSerial.android.cs:84-128` |
| M-7 | コンストラクタからの無限ループ起動 — 画面スタック退避後も60fps/250msループが回り続ける | `UIMeterViewModel.cs:74`、`DeviceAudioViewModel.cs:48-50` |
| M-8 | `CognitiveUsecase` 初期化の競合 (`initialized` フラグに同期なし、二重初期化でInferenceSessionリーク)。ArrayPool返却も例外セーフでない | `Usecase/CognitiveUsecase.cs:41-75` |

### データ・通信

| # | 内容 | 場所 |
|---|---|---|
| M-9 | **DateTime/TZの不整合**: DBは `Ticks` のみ保存(Kind喪失)、JSONは `ToUniversalTime()` — Unspecified値をローカルとみなすため **DB↔API往復で端末TZ分ずれる**。`DateTime.Parse` も `AssumeUniversal` 未指定 | `Helpers/Data/DateTimeTypeHandler.cs:7-16`、`Helpers/Json/DateTimeConverter.cs:18,28` |
| M-10 | **`SqlHelper` の NOT NULL 判定がコンパイラ内部属性 (`System.Runtime.CompilerServices.NullableAttribute`) 依存** — `int?`/`DateTime?` は誤って NOT NULL になり、別アセンブリのエンティティでは常に NOT NULL。現エンティティにnullable列がないため潜在化しているだけ | `Helpers/Data/SqlHelper.cs:47-52` |
| M-11 | HTTP API 全メソッドに `CancellationToken` なし。タイムアウトは一律30秒でファイル転送(10万行)にも適用 | `Services/HttpService.cs` 全体、`AppHostBuilderExtensions.cs:13` |
| M-12 | 401ハンドリング・トークンリフレッシュなし。`ApiContext` は排他なしの可変シングルトンで書き込み元が3箇所に散在 | `Services/ApiContext.cs`、`ApiDelegatingHandler.cs:16-20` |
| M-13 | リトライは「ダイアログで人力」のみ。`while(true)` に回数上限なし。進捗付き版にはリトライ自体がなく非対称 | `Usecase/NetworkOperator.cs:36-94,169-210` |
| M-14 | ダウンロード進捗: `Content-Length` 不明時に `NaN` となり進捗が一切更新されない | `HttpService.cs:63,81,100,119` |
| M-15 | `SELECT MAX(Id)`→`INSERT` の非アトミック採番 | `DataService.cs:148-153` |
| M-16 | マイグレーション機構なし (毎起動でDB物理削除・再作成)。`user_version` 未使用、WAL/busy_timeout 未設定。サンプルとしては意図的だが、この構造のまま実案件に流用される危険 | `DataService.cs:30-43` |

### Android 権限・Manifest

| # | 内容 | 場所 |
|---|---|---|
| M-17 | **過剰権限**: `BATTERY_STATS`(通常アプリには付与されない), `ACCESS_BACKGROUND_LOCATION`(機能なし・審査高リスク), `WRITE/READ_EXTERNAL_STORAGE`(minSdk30では無効/不要), `CHANGE_WIFI_STATE`(未実装), `USE_FINGERPRINT`(非推奨・重複), `FLASHLIGHT`(不要) | `Platforms/Android/AndroidManifest.xml:6-10,14,17,20` |
| M-18 | **不足**: minSdk 30 のため Android 11 端末ではレガシー `BLUETOOTH`/`BLUETOOTH_ADMIN` (maxSdkVersion=30) が必要 — ないと `StartDiscovery()`/`CreateBond()` が SecurityException | 同上 |
| M-19 | 起動時に全権限一括要求 + `LocationAlways`(バックグラウンド位置)を無条件要求。機能実態は `LocationWhenInUse` で足りる。UX/ストア審査両面で不利 | `App.xaml.cs:37-39`、`Permissions.cs:17-21` |
| M-20 | NoiseMonitor/OcrReader/CameraBind に権限チェックなし (起動時一括要求への暗黙依存)。唯一 `BluetoothSerial` だけが Check→Request の正しい流れ | `NoiseMonitor.android.cs:22` 等 |

### 設計・保守性

| # | 内容 | 場所 |
|---|---|---|
| M-21 | **`OnNotifyFunction1() => OnNotifyBackAsync();` が116ファイルに完全重複** — 基底クラスの既定実装にすべき | `Modules/AppViewModelBase.cs:71` |
| M-22 | シェル情報 (11プロパティ) が `ShellProperty` / `IShellControl` / `MainPageViewModel` の3箇所で手書き重複。F5追加には4ファイル修正が必要 | `Shell/ShellProperty.cs:143-173` 等 |
| M-23 | Usecase層が `IDialog` に依存し「業務ロジック」ではなく「UIシナリオ」になっている。UIなしのテスト・再利用が不可能 | `Usecase/NetworkOperator.cs:16,44,60,76` |
| M-24 | `Validate` メソッドが基底2クラスでほぼコピー (コピー先に同一行 `validationResults ??= [];` が2回混入)。`NetworkOperator.Execute<T>`/`Execute`、`HttpService` の転送4メソッド、CvNet系5VMも同様の重複 | `AppDialogViewModelBase.cs:33,40` 等 |
| M-25 | `BusyOverlay` 判定が常に真 (boxed bool は null にならない) — `False` 指定でもBehavior付与、変更のたび remove→add | `Shell/ShellProperty.cs:199-210` |
| M-26 | 診断パネルの CPU% 計算バグ (`stopwatch` をRestartせず累積時間で除算→時間経過で0に収束)。`Process`/イベントの解放もなし | `Shell/DiagnosticPanel.xaml.cs:152,17,77` |
| M-27 | `UIDockViewModel` の CPU/Memory ボタンの `Parameter` が両方 `"VolumeDown"` (コピペ誤り) | `UIDockViewModel.cs:240-258` |
| M-28 | Service層のDIバイパス (`ScheduleService`/`HolidayService` をVMが `new`)。かつ実態は静的サンプルデータ生成器で `Services` の名にそぐわない | `UICalendarViewModel.cs:16-17` |
| M-29 | コンポーネント層にUIスレッドへのマーシャリングが一切なく、VM側の `ObserveOnCurrentContext()` に暗黙依存。イベント直接購読者は容易にクラッシュ | `Nfc.android.cs:113`、`NoiseMonitor.cs:72` |
| M-30 | NU1903 (既知の高severity脆弱性パッケージ警告) を `NoWarn` で抑止したまま `<!-- TODO -->` | `Template.MobileApp.csproj:14-15` |

### UI/XAML

| # | 内容 | 場所 |
|---|---|---|
| M-31 | **アクセシビリティ全欠落** — `SemanticProperties`/`AutomationId` 0件。アイコンのみのImageButtonは無名ボタンとして読み上げられ、アイコンフォントのPUA文字がTextのボタンは意味不明な音声に。テンプレートとして「見本がない」こと自体が波及リスク | `UIMoneyView.xaml:501-547`、`UIMailView.xaml:246-256` 等 |
| M-32 | ScrollView内の縦CollectionView (スクロール競合+仮想化無効化)、BindableLayout→CollectionView三重ネスト (非仮想化のため全Section即時実体化) | `UIProfileView.xaml:255,406-411`、`UIStreamView.xaml:225,278` |
| M-33 | グローバルStyleキーのシャドーイング10種/16ファイル — `UIMailView` の `MenuGrid`/`MenuButton` はグローバル版と全く別の見た目で同名再定義 | `UIMailView.xaml:222-235` 等 |
| M-34 | ダークモード未対応は意図的 (Light固定) だが、`White` 直書き100箇所超+生hex122箇所のため将来の移行コストが極めて高い。セマンティック色トークンは4色のみ | `App.xaml.cs:16-17`、`Styles.xaml:16-23,110` |
| M-35 | RelativeSourceバインディング17件中10件が `x:DataType` 未指定 (DataTemplate内のためリフレクション経路が行リサイクルごとに走る) | `UIMailView.xaml:289,300` 等 |

---

## 6. 指摘事項 — Low

| # | 内容 | 場所 |
|---|---|---|
| L-1 | ファイル名とクラス名の不一致: `BasicLocalViewModel.cs` 内は `BasicLocaleViewModel` | `Modules/Basic/BasicLocalViewModel.cs:3` |
| L-2 | デッドコード: `ParameterBuilder`(URLエンコードなし・未使用)、`Session`/`Domain/Length`(空+TODO、Sessionは DI登録済み)、`ReactiveSignalR`(実装済みだが参照0)、`RepositoryData.Empty` | `Services/ParameterBuilder.cs`、`State/Session.cs` 等 |
| L-3 | `CalendarView` / `CalendarView2` がほぼ同一機能で並存 (計約2,570行、現用は2のみ) | `Controls/CalendarView*.xaml.cs` |
| L-4 | 証明書検証バイパスのコードがコメントで残置 (誤って有効化のリスク、削除か `#if DEBUG` 化推奨) | `Services/AppHostBuilderExtensions.cs:24-26` |
| L-5 | 署名キーストア+パスワードのコミット (`example` なので実害はないが、直書きパターンが実案件へコピーされる危険。`.EmbeddedProperty.props` 方式へ統一推奨) | `Template.MobileApp.csproj:42-48` |
| L-6 | ローカライズがスケルトン (resx実エントリ各1件、`{x:Static}` は起動時解決のみ)。XAML内日本語リテラル168箇所、`Title` 直書き118画面 | `Resources/Strings/`、`UIMoneyView.xaml:436` 等 |
| L-7 | 自己規約違反の絵文字/生Unicode 36箇所 (「アイコンは markup:Material/Fluent」ルールに対し `🐰`/`➡️`/`◀` 等) | `UIMoneyView.xaml:434,487` 等 |
| L-8 | gRPC / SignalR / Ollama はパッケージ参照のみで実装なし (.proto 0件、`OllamaSharp` 使用0行、チャットは固定文字列モック、リアルタイム画面はRandom生成)。参照だけ残すとテンプレート利用者を誤解させるため、実装するか参照を削除すべき | `csproj:126-146`、`SampleChatViewModel.cs:9-15` |
| L-9 | `catch { throw new FormatException(); }` — 元例外を InnerException に保持せず情報破棄 | `Helpers/Json/DateTimeConverter.cs:20-23` |
| L-10 | `#pragma warning disable` 75箇所 (CA1819×9、CA5394×8、CA2012×6、CA2000×5、CA2213等) — AGENTS.md の「抑止は事前相談」方針と乖離。特にリーク系 (CA2213/CA1001/CA2000) は棚卸し推奨 | プロジェクト全体 |
| L-11 | `EntryOption` のイベント多重購読 (Mapper再実行のたび `+=`)、`Focus` のアニメーション未中断+16msごとの `SolidColorBrush` 生成 | `EntryOption.android.cs:95-105`、`Focus.cs:102-141` |
| L-12 | 非搭載端末での `!` 起因NRE: NFC/Bluetoothアダプタ・外部ストレージの null免除多用 (`BluetoothSerialFactory` はDI解決の瞬間に落ちる) | `Nfc.android.cs:57-58`、`BluetoothSerial.android.cs:20-21` |
| L-13 | `Async` サフィックス不統一 (`OnNotifyBackAsync` vs `OnNotifyFunction1`)、`MailDateTimeStringConverter` のみ非sealed+`NotImplementedException` | `AppViewModelBase.cs:69-77` 等 |
| L-14 | uses-feature 宣言不足 (nfc/camera/bluetooth/microphone に `required="false"` なし) — 非搭載端末がPlayストアで除外される | `AndroidManifest.xml` |

---

## 7. iOS 対応の現状 (情報)

- `TargetFrameworks` は `net10.0-android` のみ。`Platforms/iOS`、`Info.plist`、`AppDelegate` は存在しない
- `.ios.cs` は Behaviors 5ファイルのみで、すべて空スタブ。Components 側の iOS 実装は **0件**
- `net10.0-ios` をTFMに追加すると partial メソッド未実装で即ビルドエラーになる状態 (対応必要ファイル一覧: `ActivityRecognizer` / `BluetoothSerial` / `Nfc` / `NoiseMonitor` / `OcrReader` / `StorageManager` / `CrashReport` / `ElementHelper` / `Behaviors/Extensions`)
- 実態は Android 専用テンプレート。README/ドキュメントにその旨を明記するのが誠実

---

## 8. 推奨アクション (優先度順)

> 除外項目を反映した実施計画は [Fix_Plan.md](Fix_Plan.md) を参照。

### すぐ直すべき (確定バグ)
1. `RegisterReceiver` に `ReceiverFlags.NotExported` を指定 (H-1)
2. `Extensions.android.cs` の Start/End 反転を修正、`GravityFlags.Start/End` 採用 (H-2)
3. Dispose済みビットマップのバインド3箇所 + `UITreeMapViewModel` の解放漏れを統一 (H-3)
4. `DataService` の `Using` → `UsingAsync` 2箇所 (H-4)
5. `ViewCollectionView.xaml` の TargetType 修正 (H-11)
6. `UIDockViewModel` の Parameter コピペ誤り、`BusyOverlay` 判定、CPU%計算 (M-25〜27)

### 早期に対処すべき (セキュリティ・安定性)
7. QR設定読み取りに https 強制+確認ダイアログ+保存順序修正 (H-5) ※対応不要の指示により Fix_Plan では対象外
8. `AIServiceKey` を `SecureStorage` へ (H-6)
9. `NfcReader` の Dispose 実装とタグ Close (H-7)、`NoiseMonitor` の Stop 待機化 (H-8)
10. ActivityRecognizer の権限フロー修正 — `BluetoothSerial` の Check→Request を横展開 (H-9, M-20)
11. 初期化完了の待機点を用意し `async void` を排す (H-10)
12. Manifest 権限の棚卸し (過剰6件削除+レガシーBluetooth追加) と `LocationAlways`→`WhenInUse` (M-17〜19)
13. NU1903 の原因パッケージ更新と抑止解除 (M-30)

### テンプレート品質として計画的に
14. `OnNotifyFunction1` の基底既定実装化 (M-21) ※対応不要の指示により Fix_Plan では対象外
15. Usecase層から `IDialog` 依存を剥がし Result 返却へ (M-23)
16. `HttpService` に CancellationToken、転送系の別タイムアウト、401時の処理方針 (M-11, M-12)
17. DateTime を UTC正規化 or `DateTimeOffset` へ統一 (M-9)、`SqlHelper` の nullable 判定を `NullabilityInfoContext` へ (M-10)
18. アイコンボタンへの `SemanticProperties.Description` 付与 (M-31) ※対応不要の指示により Fix_Plan では対象外
19. デッドコード削除 (`ParameterBuilder`/`Session`/`Length`/`CalendarView`旧版) (L-2, L-3)
20. `#pragma` 75箇所の棚卸し (L-10)

---

*本レビューは静的解析ベース (ビルド・実機検証は未実施)。High 指摘のうち H-2/H-3/H-4 はコードを直接確認して裏取り済み。*
