# 外部リファレンス評価 (詳細版 / AI向け知識ベース)

外部の記事・OSS・ライブラリを本サンプル (Template.MobileApp) に取り込む価値があるかを評価した記録。
判断根拠・技術詳細・実装時の具体的な着地点まで残すことを目的とし、冗長さを許容する。

人間が確認するための要約版は [Reference_Summary.md](Reference_Summary.md)。本書はその上位集合であり、両者の判定は常に一致させること。

改訂: 2026-08-24 (前提条件の見直しに伴い全面改訂。第1版の「ライブラリ追加禁止」「記録のみ」区分は廃止)

---

## 0. 前提

| 項目 | 内容 |
| --- | --- |
| NuGet 追加 | **禁止ではない**。ただし**既存パッケージまたは他ライブラリで同等機能が実現できるものは追加しない**。追加が必要な候補は内容を提示してユーザーが可否を判断する (D1) |
| 追加先 | UI 画面に限らない。**MAUI としての基本要素のサンプル**であれば Basic / View / Device / Sample / Navigation など任意のモジュールに入れる |
| テーマ / ロケール | **根本的な切替機構の実装は不要**。既存の簡易機構 (`AppThemeBinding`・`.resx`) の見せ方強化に限り検討 (D7) |
| スコープ外 | 生体認証 / カスタムハンドラ / App Actions / iOS 対応 |
| 「記録のみ」区分 | **廃止**。すべての項目を「採用候補」か「不採用」に振り分ける。知見はドキュメント追記タスク (C-10) として実作業化する |
| UI 画面数 | メニューは 2 画面構成にすることも可。**画面数の制約は考慮しない** |
| 実施単位 | 番号ベースで 1 項目ずつ。デザイン判断を伴う統合・置換を一括実施しない |

---

## 1. ベースライン (本サンプルの現状インベントリ)

評価の妥当性はこのインベントリに依存する。参照時は実ファイルで再確認すること。

### 1.1 導入済みパッケージ (UI・描画関連)

`Template.MobileApp/Template.MobileApp.csproj`

- `CommunityToolkit.Maui` 15.0.0 / `CommunityToolkit.Maui.Camera` 6.1.0 / `CommunityToolkit.Maui.MediaElement` 10.0.0
- `Syncfusion.Maui.Toolkit` 1.0.10 (**これが最新版**)
- `SkiaSharp` 4.151.1 / `SkiaSharp.Views.Maui.Controls` / `SkiaSharp.Extended` / `SkiaSharp.Extended.UI.Maui` 3.0.0
- `Svg.Skia` 5.2.1
- `Mapsui.Maui` 5.1.0 / `Microsoft.Maui.Controls.Maps`
- `Indiko.Maui.Controls.Markdown` / `Vitvov.Maui.PDFView` / `Plugin.Maui.Audio` / `QRCoder` / `BarcodeScanning.Native.Maui`
- `Microsoft.ML` / `Microsoft.ML.OnnxRuntime` / `Azure.AI.Vision.*` / `OllamaSharp`
- `Usa.Smart.*` (Mvvm / Navigation / Resolver / Reactive / Data ほか)

### 1.2 【最重要】導入済みパッケージの未使用機能インベントリ

**評価の結論を大きく左右する事実**: 導入済みパッケージには、本サンプルで一度も使われていない機能が大量にある。
外部ライブラリを検討する前に、まずここを埋めるのが最も費用対効果が高い。

#### CommunityToolkit.Maui 15.0.0

| 分類 | 機能 | 使用状況 |
| --- | --- | --- |
| Views | `MediaElement` | **使用中** (5 箇所) |
| Views | `DrawingView` | **使用中** (2 箇所) |
| Views | `CameraView` | **使用中** (Camera パッケージ) |
| Views | `Popup` / `PopupService` | 既定設定のみ (`MauiProgram.ConfigureMauiCommunityToolkit`) |
| Views | **`AvatarView`** | 未使用 |
| Views | **`Expander`** | 未使用 |
| Views | **`LazyView`** | 未使用 |
| Views | **`RatingView`** | 未使用 |
| Views | **`SemanticOrderView`** | 未使用 |
| Layouts | **`DockLayout`** | 未使用 |
| Layouts | **`UniformItemsLayout`** | 未使用 |
| Layouts | **`StateContainer`** | 未使用 |
| Behaviors | `StatusBarBehavior` | **使用中** (1 箇所) |
| Behaviors | **`TouchBehavior`** | 未使用 |
| Behaviors | **`AnimationBehavior`** | 未使用 |
| Behaviors | **`IconTintColorBehavior`** | 未使用 |
| Behaviors | **`ImageTouchBehavior`** | 未使用 |
| Behaviors | **`MaskedBehavior`** | 未使用 |
| Behaviors | **`ProgressBarAnimationBehavior`** | 未使用 |
| Behaviors | **`EventToCommandBehavior`** | 未使用 |
| Behaviors | **`UserStoppedTypingBehavior`** | 未使用 |
| Behaviors | 検証系 7 種 (`Characters` / `Email` / `MultiValidation` / `Numeric` / `RequiredString` / `Text` / `Uri`) | 未使用 |
| Behaviors | `SelectAllTextBehavior` / `SetFocusWhenEntryCompletedBehavior` / `MaximumLengthReachedBehavior` | 未使用 |

#### Syncfusion.Maui.Toolkit 1.0.10

`MauiProgram.cs:59` で `.ConfigureSyncfusionToolkit()` を呼び、**`SfEffectsView` を 98 箇所で使用中**。それ以外は全て未使用。

| 分類 | コントロール | 使用状況 |
| --- | --- | --- |
| Misc | `SfEffectsView` | **使用中 (98 箇所)** |
| Charts | Cartesian / Circular / Funnel / Polar / Pyramid / **Spark** / **Sunburst** | 未使用 |
| Calendars | `Calendar` (月/年/十年/世紀ビュー、複数選択) | 未使用 |
| Editors | DatePicker / DateTimePicker / **NumericEntry** / NumericUpDown / **OTP Input** / Picker / TimePicker | 未使用 |
| Navigation | **Bottom Sheet** / NavigationDrawer / **Tab View** | 未使用 |
| Layout | **Accordion** / Cards / Carousel / **Expander** / Popup / TextInputLayout | 未使用 |
| Buttons | Button / **Chips** / **Segmented Control** | 未使用 |
| Notification | Circular ProgressBar / Linear ProgressBar / PullToRefresh | 未使用 |
| Misc | **Shimmer** | 未使用 |

**注意**: Syncfusion.Maui.Toolkit (無償) には**ゲージ・Rating・Badge・Avatar は含まれない**。
Syncfusion のブログ記事に出てくる `SfRadialGauge` は有償パッケージ側。本サンプルは自前の `Gauge` / `SpeedGauge` / `NoiseGauge` / `SocialNotificationBadge` を持っており、Rating / Avatar は CommunityToolkit.Maui 側にある。

#### SkiaSharp.Extended.UI.Maui 3.0.0

| コントロール | 使用状況 |
| --- | --- |
| `SKLottieView` | **使用中** (`ViewLottieView.xaml`、`Progress` バインド済み) |
| **`SKConfettiView`** | 未使用 |

#### Mapsui.Maui 5.1.0

`SampleMap2View.xaml` は `<mapsui:MapControl behaviors:MapsuiBind.Controller="{Binding Controller}" />` のみ。
VM (`SampleMap2ViewModel`) はズームイン / ズームアウト / 初期位置復帰の 3 操作だけ。以下は全て未使用。

- レイヤ種別: Tile / Memory / Raster / Vector / WMS / **MBTiles (オフライン、`BruTile.MbTiles` 依存)**
- スタイル: Symbol / Label / Vector / **Callout**
- ウィジェット: **ScaleBar / Zoom / MapInfo / 北矢印 / Button**
- データ: NTS による Point / Line / Polygon、**GeoJSON**、Shapefile
- 機能: **クラスタリング**、アニメーション、静止画出力、OGC (WMS / WFS / WMTS)

### 1.3 描画基盤は 2 系統

| 系統 | 名前空間 | ビュー | 基底 | 用途 |
| --- | --- | --- | --- | --- |
| Drawing | `Template.MobileApp.Graphics.Drawing` | `DrawingControl : GraphicsView` | `DrawingObject : IDrawable` | データ駆動の静的 / 短時間アニメ描画 |
| Scene | `Template.MobileApp.Graphics.Scene` | `SceneControl : SKCanvasView` | `SceneObject : ISceneObject` | 自走アニメーション (HUD 等)。`PeriodicTimer` で 60fps 無限ループを内包、`Start()`/`Stop()` を VM が制御 |

実装済み Drawing: `ActivityDrawing` / `BarcodeDrawing` / `ChartDrawing` / `ColorTreeMapDrawing` / `DetectDrawing` / `LoadDrawing` / `SensorDrawing` / `ShapeDrawing`
実装済み Scene: `EnergyFlowScene` / `FlightHudScene` / `MechHudScene` / `TelemetryScene`

**既知の欠落**:
- Scene 側に**描画キャッシュが無い** (`SceneObject` の `BlurCache` は `SKMaskFilter` の使い回しのみ)。毎フレーム全要素を再描画。
- **「1 回だけ走って完了を通知する」アニメーション基盤が無い**。`ChartDrawing` が `System.Timers.Timer` で 600ms 線形アニメを自前実装しているだけで、完了コールバックは共通化されていない。
- Scene 系は**入力 (ヒットテスト) を受け付けない**。
- `DrawingObject` は画面描画専用。**同じ描画コードで画像ファイルを出力する経路が無い**。

### 1.4 Controls

`AiChatTemplateSelector` / `CalendarView` / `CalendarView2` / `CameraOverlayView` / `ChatMessageTemplateSelector` / `ChatView` / **`DayTimetableView`** / `DeckButtonTemplateSelector` / `EasingCurveView` / `Gauge` / `GraphRowSurface` / `InfoCard` / `JoyStickView` / `MixerEqualizer` / `MixerKnob` / `MixerSlider` / `NoiseGauge` / `RadarScreen` / `SocialControls` / `SocialNotificationBadge` / `SpeedGauge` / `StatControl` / `StatusChip` / `StepIndicator` / `SvgView` / `TextToggle`

- `DayTimetableView` は 8:00-20:00 の 1 日タイムテーブルを `SKCanvasView` で描画。`HourHeight` / `Events` バインダブル。
- `SvgView` は `SKCanvasView` + `Svg.Skia`。`SKSvg` を直接バインドする形で、`ViewSvgViewModel` が VM 側で `new SKSvg()` + `Load(stream)` している。

### 1.5 Behaviors (添付プロパティ群)

`AnimationOption` / `BarcodeBind` / `Border` / `ButtonOption` / `CameraBind` / `DrawingBind` / `EntryBind` / `EntryOption` / `Focus` / `LabelOption` / `MapBind` / `MapsuiBind` / `MediaBind` / `Scroll` / `Select` / `SliderOption` / `WebViewBind`

`AnimationOption` の演出: `Pulse` / `Bounce` / `FadeIn` / `Wave` (+`WaveDelay`) / `Flash` / `Highlight` (+`HighlightColor`) / `ProgressTo` / `EnterAnimation` (+`EnterDelay` / `EnterTrigger`)

### 1.6 画面構成

トップメニュー (`Modules/Main/MenuView.xaml`) は 9 項目:
`1.Basic` / `2.Navigation` / `3.Device` / `4.Data` / `5.Network` / `6.Sample` / `7.View` / `8.UI` / `9.Setting`

| モジュール | 画面 |
| --- | --- |
| `Modules/Main/` | `MenuView` / **`SettingView`** (アプリ実設定。`Settings` 経由で `IPreferences`/`ISecureStorage` に保存、バーコードから設定投入) |
| `Modules/Basic/` | Typography / Style / Font / Converter / Behavior / Locale / Dialog / Validation (+ Menu) |
| `Modules/Navigation/` | EditList / Stack / Wizard / Shared / Navigate ほか |
| `Modules/Device/` | Activity / Audio / BLE / Bluetooth / Camera / Communication / Location / Misc / NFC / Sensor ほか |
| `Modules/Sample/` | Chart / Chat / CV系 8 画面 / Map1 / Map2 / Markdown / Media / Pdf / WebApp / WebBasic |
| `Modules/View/` | Animation / Border / Carousel / Collection / Drawing / Easing / Effect / GraphicsView / Lottie / Refresh / Shadow / Svg |
| `Modules/UI/` | 30 画面 (`UILogin` … `UIEnergy`)。`UIMenuView` は 10 行 × 3 列 = 30 スロット |

### 1.7 標準 MAUI コントロールの使用状況 (XAML ファイル数)

| コントロール | ファイル数 | コントロール | ファイル数 |
| --- | --- | --- | --- |
| `Slider` | 8 | **`Stepper`** | **0** |
| `FlexLayout` | 7 | **`DatePicker`** | **0** |
| `ActivityIndicator` | 5 | **`TimePicker`** | **0** |
| `Switch` | 3 | **`RadioButton`** | **0** |
| `ProgressBar` | 3 | **`SearchBar`** | **0** |
| `IndicatorView` | 3 | `TableView` | 0 (非推奨寄り) |
| `Picker` | 2 | `Frame` | 0 (`Border` 移行済み) |
| `CheckBox` | 2 | `TitleBar` | 0 (デスクトップ向け) |
| `SwipeView` | 2 | `Editor` | 1 |
| `AbsoluteLayout` | 2 | `RefreshView` | 1 |

→ **Stepper / DatePicker / TimePicker / RadioButton / SearchBar が未使用**。設定画面サンプル (A-10) の題材として最適。

### 1.8 その他の現状

| 領域 | 現状 |
| --- | --- |
| テーマ | `App.xaml.cs` で `Current!.UserAppTheme = AppTheme.Light` 固定。`DynamicResource` / `AppThemeBinding` の使用箇所 0 件 |
| ローカライズ | `Resources/Strings/` に `Messages.resx` / `.ja.resx` / `Names.resx` / `.ja.resx`。`BasicLocaleViewModel` は `CultureInfo.CurrentCulture.Name` の**表示のみ** |
| 検証 | `Modules/ValidationHelper.cs` が `DataAnnotations` の `Validator.TryValidateProperty` を使用 (**同期のみ・単一プロパティのみ**)。`BasicValidationView` は `s:ValidationProperty.ClearErrorOnFocus` / `.ValidateOnUnfocused` と `s:ValidationFocusBehavior` を使用 |
| 設定永続化 | `State/Settings.cs` が `IPreferences` / `ISecureStorage` を DI で受けて使用。**既にカバー済み** |
| ライフサイクル | `App.CreateWindow` / `App.OnStart` 実装済み。`MainPageViewModel` に `OnActivated` / `OnDeactivated` / `OnResumed`。**`MauiProgram.ConfigureLifecycleEvents` は空実装 `{ }`** |
| リスト | `CollectionView` / `BindableLayout` / `ListView` を併用。選定基準は未文書化 |
| 地図 (Google) | `maps:Map` + `ItemTemplate` による Pin 表示 + `MapBind.Controller`。**`MapElements` 未使用** |
| 地図 (Mapsui) | 1.2 参照。ほぼ素の `MapControl` |
| Web | `HybridWebView` (MAUI 標準) を `SampleWebAppView` で使用。**`BlazorWebView` は未導入** |
| D&D | `DragGestureRecognizer` / `DropGestureRecognizer` **使用箇所ゼロ** |
| FAB | 専用コントロールは無く、`SampleMap1View.xaml` が `ImageButton` + `MapFabButton` スタイルで実現。**これで足りるため対応不要** |
| フォント | `DSEG7Classic-Regular.ttf` (7セグ) / `Oxanium` / `JetBrainsMono` / `851Gkktt` / `NotoSerifJP` / `MaterialIcons` / `FluentSystemIcons` 登録済み。DSEG7 の活用先は限定的 |

### 1.9 確定済みポリシー

| ポリシー | 内容 |
| --- | --- |
| 共有スタイル変更禁止 | `Resources/Styles/Styles.xaml` の共有スタイルは変更せず `BasedOn` 派生を新設 |
| Code-Behind 不使用 | View の code-behind は使わず Behavior / コントローラパターン / Trigger / VM で実装 |
| OverScroll 全体抑止 | Android の OverScroll は `Scroll` クラスのハンドラで既定抑止 (opt-out 方式) |
| アイコン | `Markup/FontIconExtensions.cs` の用途別マークアップ拡張 (`MenuIcon` / `MoneyIcon` ほか) |
| フォントサイズ | 伝統的サイズ (Office 系) に統一。Android は WPF 比で小さく見えるため大きい側へ丸める |
| 命名 / 整形 | `.editorconfig` 準拠、メンバ変数に `_` 接頭辞を付けない、新規テキストファイルは CRLF、ビルド警告ゼロ |

---

## 2. 評価軸

1. **既存充足の確認** — 導入済みパッケージ (1.2) / 自前実装 (1.3〜1.5) で実現できないか
2. **新規性** — 本サンプルに同等の実装が無いか
3. **基盤適合** — 既存の Drawing / Scene / Behaviors / Controls / Layouts にそのまま乗るか
4. **サンプルとしての価値** — MAUI の基本要素の欠落を埋めるか、「見て分かる」ものになるか
5. **ポリシー整合** — 1.9 のポリシーと衝突しないか

---

## 3. 判断が必要な項目

すべての判断項目は 2026-08-24 に決定済み。各項目の末尾に **【決定】** を記載する。

| # | 判断内容 | 決定 |
| --- | --- | --- |
| D1 | NuGet 追加の可否 | 案B — `Microsoft.AspNetCore.Components.WebView.Maui` のみ許可。`BruTile.MbTiles` は推移依存を確認後 |
| D2 | アプリ風サンプルの置き場所 | 案A — `Modules/App/` 新設 + トップメニュー `10.App` |
| D3 | 電卓の仕様 | 案A (科学電卓)。ただしモデルが複雑になりすぎる場合は機能省略も検討 |
| D4 | 設定画面サンプルの置き場所 | 案A — `Modules/Basic/BasicSettingView` 新設 |
| D5 | ミニゲームの題材 | 数独。後からライフゲーム / 2048 の追加も検討 |
| D6 | リスト D&D の置き場所 | 案A — `Modules/View/ViewDragDropView` 新設 |
| D7 | テーマ / ロケールの強化 | 案B — ロケールのみ強化。テーマは触らない |
| D8 | Scene のダブルバッファ | 実験的に導入し、効果を判定して本採用の可否を決める |
| D9 | タブ / ボトムシート | Syncfusion を本線。学習用途として自作案も候補に残す |
| D10 | TimeRecorder からの取り込み | **新規画面は作らない**。既存の日次スケジュール表示を強化するに留める |
| D11 | CSLA からの取り込み | **非同期検証は実装しない**。相関検証のみ検討 |
| D12 | 未使用機能サンプルの構成 | 案B + 案C の併用 |


### D1. NuGet 追加の可否

既存パッケージで代替できないものだけを候補として残した。**候補は 2 つのみ**。

| 候補 | パッケージ | 用途 | 代替可能性 | 備考 |
| --- | --- | --- | --- | --- |
| (a) | `Microsoft.AspNetCore.Components.WebView.Maui` | Blazor Hybrid サンプル (B-11) | 代替不可。`HybridWebView` は JS↔C# 連携で、Razor コンポーネント共有とは別の機構 | Microsoft 公式。MAUI の基本要素の中で最大の欠落 |
| (b) | `BruTile.MbTiles` | Mapsui のオフライン地図 (B-14) | 代替不可 (MBTiles 読み込みに必須) | `Mapsui.Maui` が既に推移依存を持つ可能性あり。着手前に `dotnet list package --include-transitive` で要確認 |

- **案A**: (a)(b) 両方を許可
- **案B**: (a) のみ許可、(b) は推移依存の確認結果次第
- **案C**: 追加なし (B-11 / B-14 を不採用にする)

**推奨: 案B**。(a) は MAUI の主要機能で、サンプルとして持つ価値が明確。(b) は「既存の推移依存で足りるなら追加ゼロ」の可能性があるため確認を先に行う。

**【決定】案B**。(a) を許可する。(b) は着手前に `dotnet list package --include-transitive` で確認し、未解決なら改めて可否を判断する。

### D2. アプリ風サンプル (電卓 / ミニゲーム / 工数タイマー) の置き場所

既存カテゴリは「UI = 見た目の再現」「Sample = 外部連携・機能の実演」「View = コントロール単位のデモ」であり、**完結した小アプリ**はどこにも合わない。

- **案A**: 新規モジュール `Modules/App/` を作り、トップメニューに `10.App` を追加
- **案B**: `Modules/Sample/` に追加 (`SampleCalcView` など)
- **案C**: `Modules/UI/` に追加し、`UIMenuView` を 2 画面構成にする

**推奨: 案A**。電卓・ミニゲーム・工数タイマーはいずれも「純モデル + MVVM の分離を見せる」という共通の狙いを持つため、1 カテゴリにまとめると意図が伝わる。トップメニューは現在 9 項目で余裕がある。

**【決定】案A**。`Modules/App/` を新設し、トップメニューに `10.App` を追加する。なお D10 の決定により工数タイマーは対象外となったため、配下は**電卓とミニゲームの 2 本**。

### D3. 電卓の仕様レベル

コアは純モデル (`Models/` 配下、MAUI 非依存) として実装し、表示は DSEG7。参考実装の比較:

| 参考 | UI | コア | 採るべき点 |
| --- | --- | --- | --- |
| naweed/MauiScientificCalculator | 10 行 × 5 列、2 段の背景帯 (`#262D37` / `#2C3240`) を `BoxView` + `Grid.RowSpan` で塗り分け、入力欄は `FormattedString` の `Span` に `"│"` を挟んでカーソル表現 | `NCalcSync` に依存 (外部評価器) | **UI 構成とスタイル分類**。`Style` + `Command`/`CommandParameter` 方式は本サンプルと同型 |
| davidortinau/SimpleCalculator | 四則 + `%` + `+/-`、全 code-behind | `Calculator.Calculate(v1, v2, op)` の switch のみ | **`ToTrimmedString`** (末尾ゼロと小数点の除去) だけ |

- **案A**: 科学電卓 (四則 + `%` + 括弧 + `SIN`/`COS`/`TAN`/`LOG`/`EXP`/累乗/階乗)。トークナイザ → 操車場アルゴリズム → RPN 評価器を自前実装
- **案B**: 四則 + `%` のみ。状態機械 (入力中 / 演算子待ち / 結果表示) を純モデルで表現
- **案C**: 案B を先に実装し、後から科学関数を追加する段階実装

**推奨: 案A**。純モデルとして実装する価値 (パーサ + 評価器 + エラー処理) が最も高く、`NCalc` を入れない判断の裏付けにもなる。UI は MauiScientificCalculator を参考にする。

**【決定】案A。ただしモデルが複雑になりすぎる場合は機能を省略することも検討する。**
実装の優先順位を次の通りとし、複雑化した時点で下位から切る。

1. 四則 + `%` + 括弧 (トークナイザ / 操車場アルゴリズム / RPN 評価器の骨格。ここは必須)
2. 単項マイナス、`SIN` / `COS` / `TAN` / `LOG` / `EXP` (1 引数関数。トークン種別を 1 つ増やすだけ)
3. 累乗 (右結合の扱いが必要)、階乗 (後置演算子の扱いが必要)

3 の追加でパーサが読みにくくなるようであれば 3 は省略する。

### D4. 設定画面サンプルの置き場所と役割

既存に 2 つの「設定っぽい画面」がある。役割を分けること。

| 既存 | 現在のファイル名 | 何用か |
| --- | --- | --- |
| アプリ実設定 | `Modules/Main/SettingView.xaml` | 実際の接続先 / API キー設定。バーコードから投入、`Preferences`/`SecureStorage` に保存 |
| 設定画面の見た目デモ | `Modules/UI/UIKitSettingView.xaml` | iOS 風グルーピングリスト。アイコン + タイトル + シェブロンのみで**操作要素は無い** |

新設するのは「**操作できる設定 UI の型**」。未使用の標準コントロール (Stepper / DatePicker / TimePicker / RadioButton / SearchBar) と `Switch` / `Slider` / `Picker` / `Entry` を網羅し、値を VM に双方向バインドする。

- **案A**: `Modules/Basic/BasicSettingView` を新設 (`1.Basic` 配下)
- **案B**: `Modules/View/ViewInputView` として「標準入力コントロール一覧」に寄せる (`7.View` 配下)
- **案C**: `UIKitSettingView` を機能化する

**推奨: 案A**。狙いが「MAUI 標準コントロールの網羅」なので Basic が適切。案C は既存の見た目デモを壊す。

**【決定】案A**。`Modules/Basic/BasicSettingView` を新設する。

### D5. ミニゲームの題材

純モデル (盤面 / ルール / 判定) と MVVM を分離して見せる。

- **案A**: 数独 (9×9)。制約充足が純ロジックに閉じる。`UniformItemsLayout` の題材にもなる
- **案B**: ライフゲーム。`Graphics/Drawing` または `Scene` の題材を兼ねられる
- **案C**: 2048。スライドアニメーションの題材になる

**推奨: 案A** (ユーザー指定)。次点は案B — Scene 強化 (B-4〜B-6) の実演を兼ねられる。

**【決定】数独。後からライフゲーム / 2048 を追加することも検討する。**
そのため盤面モデルは数独専用にせず、「盤面 + ルール + 判定」を差し替えられる構造にしておく (`IBoardGame` 相当の抽象を置き、View 側は盤面サイズとセル描画だけを見る)。

### D6. リスト項目のドラッグ&ドロップの置き場所

「UI でも Navigation でもない」という指定に従う。

- **案A**: `Modules/View/ViewDragDropView` を新設
- **案B**: `Modules/View/ViewCollectionView` に並べ替えセクションを追加
- **案C**: `Modules/Basic/BasicBehaviorView` に追加

**推奨: 案A**。`DragGestureRecognizer` / `DropGestureRecognizer` の使い方 (並べ替え / 別リストへの移動 / ゴミ箱へのドロップ) を 1 画面で見せられる。`ViewCollectionView` は既に 4 種のスタイル + `SwipeView` で密度が高い。

**【決定】案A**。`Modules/View/ViewDragDropView` を新設する。

### D7. テーマ / ロケールの簡易機構強化

根本的な切替機構は不要という前提。既存機構の**見せ方**の強化に限る。

- **案A**: 何もしない
- **案B**: `BasicLocaleView` を強化 — `.resx` 参照結果の一覧と、カルチャ別の数値 / 日付 / 通貨 / 序数の書式差を並べて表示する (切替機構は作らない)
- **案C**: 案B に加えて `BasicStyleView` に `AppThemeBinding` の記述例セクションを 1 つ追加 (`UserAppTheme = Light` 固定のまま、書き方だけ示す)

**推奨: 案B**。テーマ側はダークモード保留の判断と共有スタイル変更禁止ポリシーに触れるため触らない。ロケール側は現状「カルチャ名を表示するだけ」で情報量が乏しく、強化の余地が明確。

**【決定】案B**。`BasicLocaleView` のみ強化する。`BasicStyleView` への `AppThemeBinding` 追加は行わない。

### D8. Scene のダブルバッファ

- **案A**: 採用しない (A-3 の `SKPicture` 静的レイヤキャッシュのみ)
- **案B**: 重い初期化を伴う Scene に限定して採用
- **案C**: 全 Scene に導入

**推奨: 案A**。DrawnUI の `ImageDoubleBuffered` は「**重いコンテンツ構築 (記事本文・バナー画像のロード) をバックグラウンドで行いつつ、前回のキャッシュを描く**」ための機構。現行 4 シーンの課題は毎フレームの描画コストであり、初期化コストではないため直接効かない。

**【決定】実験的に導入し、効果を判定して本採用の可否を決める。** 手順:

1. A-3 (`SKPicture` 静的レイヤキャッシュ) を先に入れ、**Release ビルドで**フレーム時間を実測する
2. 最も描画が重い 1 シーン (`TelemetryScene` または `MechHudScene`) にダブルバッファを試験導入して再計測する
3. 有意な差が出れば `SceneObject` の基盤機能として本採用、出なければ試験実装を破棄する

計測結果は `Document/Development.md` (C-10) に残す。この項目を **B-16** として第 4 章に追加する。

### D9. アプリ固有タブ / ボトムシート (低優先)

- **案A**: Syncfusion の `SfTabView` / `SfBottomSheet` を使うサンプル画面を作る (実装はほぼ配置のみ)
- **案B**: 自作する (タブ帯 + `ViewSwitcher` + `LazyView` の分離設計)
- **案C**: 対応しない

**推奨: 案A**。導入済みパッケージに両方あり、追加コストがほぼゼロ。Sharpnado.Tabs の「タブ帯とコンテンツ切替を分離する」設計思想は `SfTabView` が既に内包している。

**【決定】案A を本線とする。ただし学習用途として自作案 (案B) も候補に残す。**
まず `SfTabView` / `SfBottomSheet` のサンプルを作り (B-15)、その後で「タブ帯 + `ViewSwitcher` + `LazyView` の分離設計」を自作する価値があるかを改めて判断する。

### D10. TimeRecorder からの取り込み範囲

WPF アプリだが、UI 要素と機構に参考価値がある。
構成: DDD + オニオン、`Entity` / `ValueObject` / `Identity` (型付き ID) / `IRepository` / **`Specification`** / **`Command`** / `NotificationDomainModel`、`MonthlyReportBuilder`、Domain のユニットテストあり。
UI: `NavigationRailResource.xaml` (縦アイコンナビ) / `WorkUnitRecorderView` + `TimelineWorkingTimeCardResource` (日次タイムライン + 作業カード) / `TaskCardResource` / `DateTimePickerView` / `ChipBorderStyle` / `ExpanderStyle` / Todo / Exporter / ArchiveManager。

- **案A**: 工数タイマー小アプリ — Start/Stop で作業時間を記録し、日次タイムラインに積む。純モデルとして `TimePeriod` 値オブジェクトと集計 (`Specification` 相当) を実装
- **案B**: `NavigationRail` 風の縦アイコンナビ UI のみ
- **案C**: 両方

**推奨: 案A**。既存の `DayTimetableView` と `UIScheduleView` が受け皿として活きる。

**【決定】新規画面は作らない。既存の日次スケジュール表示と突き合わせ、UI 要素として部分的に参照して強化するに留める。**

対象の現状 — `Modules/UI/UIScheduleView.xaml`: 日チップの横並び (`DayChipBorder` + `DataTrigger` で選択表示) + `Controls/DayTimetableView` (8:00-20:00 の日次タイムテーブルを `SKCanvasView` で描画、1 分毎に更新される現在時刻ライン)。モデルは `TimetableDay` / `TimetableEvent`、供給は `Services/ScheduleService`。

| TimeRecorder の UI 要素 | 本サンプルへの反映 |
| --- | --- |
| `TaskCardResource` / `TimelineWorkingTimeCardResource` (作業カード) | `DayTimetableView` のイベント描画をカード風に強化 (色分け / タイトル + 補足の二段表示) |
| 作業時間の表示 | 各イベントの所要時間ラベル、および日合計時間の表示 |
| 未入力時間帯の可視化 | タイムテーブルの空き時間帯をハイライトする |
| `ChipBorderStyle` | 既存 `DayChipBorder` で充足。**対応不要** |
| `DateTimePickerView` | `SfDateTimePicker` (導入済み) で充足。A-11 の範囲 |
| `NavigationRailResource` (縦アイコンナビ) | 新規画面になるため**対象外**。C-9 を取り下げる |

これに伴い **B-9 (工数タイマー小アプリ) は「日次スケジュール表示の強化」に差し替え**、**C-9 (NavigationRail 風 UI) と C-11 (月次集計レポート) は取り下げ**る。`Modules/App/` 配下は電卓とミニゲームの 2 本になる。
アーキテクチャ (DDD / オニオン) 自体は本サンプルが `Smart.*` + DI で確立済みのため取り込まない。

### D11. CSLA からの取り込み範囲

CSLA が提供するのはビジネスオブジェクト基底 / ルールエンジン / 検証 / 認可 / データポータル / モバイルオブジェクト。
本サンプルの検証は `DataAnnotations` + `Smart.Mvvm` の `ValidationProperty` / `ErrorInfo` で確立済みで、`CommunityToolkit.Maui` にも検証 Behavior 7 種がある。**フレームワーク導入は不要**。
ただし現行 `ValidationHelper` は `Validator.TryValidateProperty` の**同期・単一プロパティ**のみで、CSLA が持つ以下は欠落している。

- **案A**: `BasicValidationView` に「**非同期検証**」(サーバ問い合わせを模したチェック) と「**相関検証**」(複数プロパティに跨る条件) のセクションを追加
- **案B**: 案A に加えて「プロパティ単位の認可」(条件により読み取り専用 / 非表示) のデモを追加
- **案C**: 対応しない

**推奨: 案A**。非同期検証と相関検証はどのアプリでも必要になる要素で、既存画面への追記で完結する。

**【決定】非同期検証 (サーバ問い合わせ) は実装しない。相関検証 (複数プロパティに跨る条件) のみ検討する。** B-12 のスコープを相関検証に限定する。

### D12. 未使用機能サンプル (1.2) の構成

導入済みパッケージの未使用機能は数十個ある。どう画面に割り付けるか。

- **案A**: パッケージ別に画面を作る (`ViewToolkitView` = CommunityToolkit、`ViewSyncfusionView` = Syncfusion)。Syncfusion は約 30 個あるため 1 画面では収まらない
- **案B**: **種別別に画面を作る** — 入力系 / レイアウト系 / ナビゲーション系 / 演出系。パッケージの境界を意識せず「やりたいこと」で引ける
- **案C**: 既存画面に分散して追記する (`ViewEffectView` に Confetti、`BasicValidationView` に検証 Behavior など)

**推奨: 案B + 案C の併用**。単独で 1 セクション成立するもの (レイアウト 3 種、タブ / ボトムシート、入力コントロール群) は種別別の新規画面に、既存画面のテーマに収まるもの (Confetti → `ViewEffectView`、検証 Behavior → `BasicValidationView`、Shimmer → `ViewRefreshView`) は追記する。

**【決定】案B + 案C の併用**。第 5 章の画面別一覧がこの方針で割り付け済み。

### D13. Shiny Controls パッケージの参照可否

MIT / 無償 / OSS (`Shiny.Maui.Controls` / `Shiny.Blazor.Controls`)。70 種超のコントロールを持ち、MAUI と Blazor の 2 レンダラが Material 3 のトークン契約を共有する。
ただし v1.0 は 2026-08 リリース直後で、GitHub のスター 14 / コミット 219 と実績が薄い。第 9 章の全機能照合の結果、**大半は既存 + CommunityToolkit + Syncfusion で充足**する。

- **案A**: 参照しない。既存に無い機能のみ自前実装で強化する
- **案B**: `Walkthrough` など既存で埋まらないものだけのために参照する
- **案C**: 判断を保留し、v1.1 以降の成熟を待つ

**推奨: 案A**。「既存もしくは他ライブラリで同等の機能を実現していることまで増やさない」という前提に照らすと、参照して得られる純増分は `Walkthrough` / `StaggeredGrid` / 小型入力コントロール数種に限られ、いずれも自前実装できる規模のため。

**【決定】案A — Shiny Controls パッケージは参照しない。**

### D14. ChatView 強化の範囲

Shiny の `ChatView` は `IChatSessionProvider` → `IChatSession` というセッション抽象で、ライブイベント購読・送信状態・ページングをコントロール側が持つ。
既存 (`Controls/ChatView.xaml` + `Modules/UI/UIChatView` + `Models/Sample/Chat/`) はリアクション / 既読 / スタンプ / 画像を実装済みだが、送信状態・入力中・ページングが無い。

- **案A**: 送信状態 (送信中 → 成功 / 失敗、失敗と拒否の区別、再送) と入力中インジケータのみ
- **案B**: 案A + カーソルベースのページング (`RemainingItemsThreshold` で過去を追加読み込みし、ライブ挿入でスクロール位置が崩れない)
- **案C**: 案B + セッションプロバイダ抽象 (`IChatSession` 相当) までモデル側を作り替える

**推奨: 案B**。案A の 2 つは `ChatMessage` に列挙型を 1 つ足す程度で済み、ページングは `CollectionView` の未使用機能 (`RemainingItemsThreshold`) のサンプル化を兼ねる。**案C はコスト高** — 既存 VM の構造を作り替えることになり、得られるものは抽象化の見本のみ。

**【決定】機能面の取り込みはすべて見送る。** 送信状態 / 入力中インジケータ / セッションプロバイダ抽象は対応不要。リアクションは既存で充足。

**`RemainingItemsThreshold` は ChatView では「簡単」に該当しないため、別画面へ振り替える。** 理由 — `UIChatView.xaml:403` の `CollectionView` は
`ItemsUpdatingScrollMode="KeepLastItemInView"` + `behaviors:Scroll.ShowOnAwayFromLastTarget` + VM の `ScrollToLast()` で
**「最新が末尾・末尾に追従する」作り**になっている。`RemainingItemsThresholdReached` は*末尾*到達で発火するため、
チャットで必要な「先頭方向へスクロールして過去を読む」用途には合わず、コレクションの逆順化や `RotationX` 反転が必要になり既存の 3 つの仕組みと衝突する。
→ **B-17 は「`RemainingItemsThreshold` による追加読み込みを `Modules/View/ViewCollectionView.xaml` に入れる」に変更**。こちらは下方向スクロールの素直な無限読み込みで、属性 2 つと VM のコマンド 1 本で済む。

### D15. Scheduler 強化の範囲

Shiny の `Scheduler` は `ISchedulerEventProvider` 1 本で月カレンダー / 日アジェンダ / イベント一覧の 3 ビューを賄い、複数日イベントを全モードで帯表示する。
**既存はこの考え方を 2/3 まで実装済み** — `Services/ScheduleService.cs` の `GetEvents(DateOnly start, DateOnly end)` を `UICalendarViewModel` (月) と `UIScheduleViewModel` (日) が共有している。`ScheduleEvent` は `Span` を持ち、月ビューは複数日を帯で描いている。

- **案A**: 日ビューの複数日イベント対応のみ (現状 `GetEvents(day.Date, day.Date)` で単日しか拾えない)
- **案B**: 案A + 3 つ目の「イベント一覧 (アジェンダ)」ビューを既存画面にタブ / セグメントで追加
- **案C**: 案B + `ScheduleService` を `IScheduleEventProvider` としてインターフェース化し、供給元を差し替え可能にする

**推奨: 案C**。案A・案B は既存の欠落の穴埋め。案C も具象クラス 1 本にインターフェースを被せるだけで**コストは低く**、「1 データ供給元 × 複数ビュー」という設計を明示できる。D10 の決定 (新規画面を作らない) は維持し、すべて既存 2 画面の中で完結させる。

**【決定】`ScheduleService` 相当 (= `IScheduleEventProvider` 化) のみ実施。** 日ビューの複数日イベント対応と 3 つ目のイベント一覧ビューは**対応不要**。

### D16. Walkthrough (要素スポットライト型コーチマーク) の実装可否

Shiny の `Walkthrough` は、対象要素を `Target="{x:Reference SearchBox}"` で指定してスポットライト表示し、宣言順にツアーを進める。`RememberRunKey` で初回のみ自動実行する。
既存 `UIKitOnboardView` はページ送り型のオンボーディングで、**要素を指すコーチマークは無い**。

- **案A**: 実装する。`Modules/View/` に新規画面を作り、自画面の要素を順に案内する
- **案B**: 実装する。既存 `UIKitOnboardView` にモード切替として追加する
- **案C**: 実装しない

**推奨: 案A**。既存に無い演出で、`Grid` 全面オーバーレイ + `Border` のくり抜き + 対象要素の `GetBoundsRelativeTo` 相当の座標取得で自作できる。**コストは中** — 対象要素の絶対座標取得と、`ScrollView` 内にある要素へのスクロール追従が要注意点。`RememberRunKey` に相当する初回判定は既存 `State/Settings.cs` (`IPreferences`) で賄える。

**【決定】低優先。実装は行わず、実装方針のメモだけ残す (B-18)。**

### D17. 小型入力コントロール群の扱い

Shiny にあり既存にも両 Toolkit にも無い小型入力: `ColorPicker` / `RangeSlider` / `DurationPicker` / `AutoCompleteEntry`。

- **案A**: A-10 (`BasicSettingView`) に混ぜる
- **案B**: 自作コントロールとして `Controls/` に置き、`Modules/View/` の入力系画面で見せる
- **案C**: 作らない

**推奨: 案B**。A-10 は「標準コントロールの網羅」が狙いなので、自作コントロールを混ぜると趣旨がぼやける。

**【決定】案B を採り、対象は「簡単に実装できるもの」に限定する。**

| 部品 | 実装方法 | 難度 | 採否 |
| --- | --- | --- | --- |
| `ColorPicker` | RGBA スライダ 4 本 + プレビュー。`BindableProperty` で `Color` を公開 | 低 | **採用** |
| `DurationPicker` | 時 / 分の `Picker` 2 個を `TimeSpan` にまとめる | 低 | **採用** |
| `RangeSlider` | `GraphicsView` + つまみ 2 個。ドラッグのヒットテストとつまみの交差処理が必要 | 中 | 見送り |
| `AutoCompleteEntry` | `Entry` + 候補オーバーレイ。フォーカス制御とオーバーレイの配置が必要 | 中 | 見送り |

### D18. チャット UI の二重実装

`Controls/ChatView.xaml` (汎用コントロール。`Messages` / `InputText` / `SendCommand` の 3 プロパティのみ) と
`Modules/UI/UIChatView.xaml` (画面に直書き。リアクション / 既読 / スタンプトレイ / 最新へジャンプまで持つ) で、チャット UI が 2 つ存在する。
カレンダー側が「コントロール化 → 画面から使う」で統一されているのに対し、チャット側だけ構成が揃っていない。

- **案A**: 現状維持。用途が違う (`ChatView` = AI チャット用の素朴な吹き出し、`UIChatView` = LINE 風 UI の見本) ため、意図的な使い分けとして残す
- **案B**: `Controls/ChatView` に寄せ、`UIChatView` はそれを使う形に統合する
- **案C**: `UIChatView` の作りを `Controls/` へ抽出し、`ChatView` を置き換える

**推奨: 案A + C-13**。役割が異なるため統合の利は小さい。ただし `Controls/ChatView` が見た目を一切外に出していない点だけは改善余地があるので、
バブル色などのバインダブルプロパティ追加 (C-13) に留める。

**【決定】案A — 現状維持。** 二重実装は用途の違いによる意図的な使い分けとして残す。バブル色のバインダブル化 (C-13) は**検討扱い (未確定)**。

### D19. 未使用になった `CalendarView` (旧) の扱い【後日対応 / 本セッションでは扱わない】

**【前提・決定済み】`CalendarView2` (Skia 自前描画版) を正とする。**

現状を確認したところ、`Controls/CalendarView.xaml` (旧・標準コントロール構成版) は **`Modules/UI/UICalendarView.xaml:60` が `controls:CalendarView2` を使っているため、どこからも参照されていない**。
コメント (`UICalendarView.xaml:57-59`) にも「どちらを採用するかは未決定」と書かれており、今回の決定で実態と食い違う。

| ファイル | 行数 | 状態 |
| --- | --- | --- |
| `Controls/CalendarView.xaml` | 73 | 未参照 |
| `Controls/CalendarView.xaml.cs` | 1,417 | 未参照 |
| `Controls/CalendarView2.xaml` | 64 | 使用中 (正) |
| `Controls/CalendarView2.xaml.cs` | 1,153 | 使用中 (正) |

計 **1,490 行が未使用**のまま残っている。

- **案A**: `CalendarView` (旧) を削除し、`CalendarView2` を `CalendarView` にリネームして正の名前を与える
- **案B**: `CalendarView` (旧) を削除するが、`CalendarView2` の名前はそのまま残す
- **案C**: 「標準コントロール構成 vs Skia 自前描画」の比較サンプルとして旧版も残し、コメントを実態に合わせて更新するだけに留める

**推奨: 案A**。1,490 行の未使用コードは保守負債になる。`2` という接尾辞も、正の実装であることが名前から読み取れないため望ましくない。
ただし**案C にも意味がある** — 同一 UI を「標準コントロールの組み合わせ」と「Skia 全面描画」の 2 通りで実装した対比は、本サンプルの性格上ほかに無い教材価値を持つ。その場合はコメントを「比較用に残している。正は `CalendarView2`」へ書き換える。

**【後日対応】旧 `CalendarView` の扱いは本セッションの対象外。** 別途あらためて判断する。
**C-14 (コメントの実態合わせ) も D19 と同時に扱う** — 案A を採る場合はリネームでコメントごと書き直しになるため、先にコメントだけ直しても手戻りになる。

### D20. SSH.NET (SCP) パッケージの追加可否

**ユーザー指示 (2026-08-24) により SCP 対応を計画に追加する。**

現状確認 — **`Modules/Network/NetworkScpView.xaml` は空のスタブとして既に存在する**。
`ViewId.NetworkScp` は `Modules/ViewId.cs:76` に登録済み、`[View(ViewId.NetworkScp)]` も付いているが、
本体は `<!-- Menu -->` のみで、`NetworkScpViewModel` も戻る処理しか持たない。さらに `NetworkMenuView.xaml` から結線されていないため**到達できない**。
一方でメニューには `Grid.Row="7"` / `Grid.Row="8"` の**空きスロットが 2 つ** (`IsEnabled="False"` / `Text=""`) 空いている。

| 項目 | 内容 |
| --- | --- |
| パッケージ | `SSH.NET` (Renci.SshNet) **2026.0.0** (2026-08-09 リリース) |
| ライセンス | MIT |
| ターゲット | .NET 8.0 以上 / .NET Standard 2.0 / .NET Framework 4.6.2 以上 |
| 推移依存 | **`BouncyCastle.Cryptography` ≥ 2.7.0 (新規)** / `Microsoft.Extensions.Logging.Abstractions` ≥ 8.0.3 (既存の `Microsoft.Extensions.Logging.Debug` 経由で導入済み) |
| 代替の有無 | **無い**。.NET 標準にも導入済みパッケージにも SSH / SCP の同等機能は存在しない |

前提の「既存もしくは他ライブラリで同等の機能を実現しているものは追加しない」に照らすと、**SSH/SCP は例外に当たらず追加が正当化される**。

- **案A**: 追加する。事前に `dotnet list package --include-transitive` で推移依存の実際の増分を確認する
- **案B**: 追加しない (SCP は見送り、スタブも削除)
- **案C**: 追加するが、`BouncyCastle` の増分が許容できない場合は再判断

**推奨: 案A**。ただし**注意点が 1 つある** — Android の Release ビルドは既定でトリミングされる。
本プロジェクトの csproj / Directory.Build.* にトリミング設定は無く、既定に任せている状態。
SSH.NET と BouncyCastle は暗号アルゴリズムの解決にリフレクションを使う箇所があるため、**Debug で動いても Release で失敗しうる**。
**必ず Release ビルドで実機確認**し、失敗する場合は `TrimmerRootAssembly` で退避する。この確認は C-10 の「Release ビルドで測る」手順と同じ位置づけ。

### D21. SCP サンプルのスコープ

`SSH.NET` は `ScpClient` (SCP) / `SftpClient` (SFTP) / `SshClient` (コマンド実行) を持つ。

- **案A**: **SCP のみ** — アップロード / ダウンロード + 進捗表示
- **案B**: SCP + SFTP — `SftpClient` でディレクトリ一覧 / 削除 / リネームまで
- **案C**: SCP + `SshClient.RunCommand` によるリモートコマンド実行

**推奨: 案A**。指示が SCP であること、`ScpClient` と `SftpClient` は API が別物で 1 画面に詰めると散らかることが理由。
案B に広げるなら画面を分ける (`NetworkScp` / `NetworkSftp`) 方が良く、メニューの空きスロットは 2 つあるので枠は足りる。

### D22. 接続情報の保管とホスト鍵検証

**接続情報の保管** — `State/Settings.cs` が `IPreferences` (平文) と `ISecureStorage` (鍵類) を使い分ける前例を持つ (`GetAIServiceKeyAsync` / `SetAIServiceKeyAsync` は旧 `Preferences` からの移行処理付き)。

- **案A**: ホスト / ポート / ユーザー名は `IPreferences`、パスワード / 秘密鍵は `ISecureStorage`。既存 `Settings` に倣う
- **案B**: 画面内の入力のみで保存しない

**推奨: 案A**。既存の作法をそのまま適用でき、`ISecureStorage` の使い方サンプルとしても機能する。

**ホスト鍵検証** — `SshClient` / `ScpClient` は `HostKeyReceived` イベントで指紋を検証できる。**サンプルが無条件受け入れを書くと誤った作法を広めることになる**ため、扱いを決めておく。

- **案A**: TOFU (Trust On First Use) — 初回接続時に指紋を表示してユーザーが承認し、`ISecureStorage` に保存。次回以降は不一致なら接続を中断する
- **案B**: 指紋を画面に表示するが、接続は常に許可する
- **案C**: 検証しない (`e.CanTrust = true` 固定)

**推奨: 案A**。実装量は「指紋の保存と比較」だけで小さく、セキュリティの作法を示せる。**案C は避ける**。

---

## 4. 採用候補

### 優先度 A — 即着手できる (既存基盤の素直な拡張 / 明確な欠落の穴埋め)

| # | 内容 | 出典 | 実装先 | 得られるもの |
| --- | --- | --- | --- | --- |
| A-1 | **単発アニメーション + 完了通知**の共通機構。`Microsoft.Maui.Controls.Animation` + `Easing` で減速停止し、完了コールバックで結果を確定する | S-03 | `Graphics/Drawing/` に共通化 | 描画基盤に欠けていた 3 つ目のアニメ型 |
| A-2 | **回転テキスト描画** (`SaveState`/`Rotate`/`RestoreState`) + 抽選ホイール。読み取り専用 `Command` は `BindableProperty.CreateReadOnly` で公開 | S-03 | `Graphics/Drawing/WheelDrawing.cs` + 画面 1 枚 | A-1 の実演。扇形は `ChartDrawing` の Donut を流用 |
| A-3 | **Scene: 静的レイヤの `SKPicture` キャッシュ**。`SKPictureRecorder` で背景グリッド / 目盛りを 1 回記録し、毎フレームは `DrawPicture` | S-04 / S-05 | `Graphics/Scene/SceneObject.cs` + 4 Scene | 毎フレーム全描画の解消。2 出典が同じ結論に到達 |
| A-4 | **`SvgView` の機能拡張** — `ImageSource` (File/Uri/Stream) 対応、メモリキャッシュ、Placeholder / ErrorPlaceholder、`Loading`/`Ready`/`Error` イベント | S-12 | `Controls/SvgView.cs` + `Modules/View/ViewSvg*` | XAML から `Source="xxx.svg"` で使えるようになる |
| A-5 | **Google Maps に `MapElements` を追加** — `Polyline` (経路) / `Polygon` (範囲) / `Circle` (半径)。`Geopath` に `Location` を並べ、`StrokeColor`/`StrokeWidth`/`FillColor` を設定 | S-24 | `Modules/Sample/SampleMap1*` + `Behaviors/MapBind.cs` | 純粋な機能欠落の穴埋め |
| A-6 | **Mapsui (フリー地図) の強化** — ウィジェット (ScaleBar / Zoom / MapInfo / 北矢印)、Pin + Callout、Polyline / Polygon (NTS)、GeoJSON 読み込み、クラスタリング。コントローラは MPowerKit/GoogleMaps に倣い**機能グループ別マネージャ + Mapper 辞書**に分割 | S-50 / S-26 | `Modules/Sample/SampleMap2*` + `Behaviors/MapsuiBind.cs` | API キー不要のフリー地図で機能を厚くできる |
| A-7 | **リスト項目のドラッグ&ドロップ** — `DragGestureRecognizer` / `DropGestureRecognizer`、`DragStartingCommandParameter` で対象を VM へ、並べ替え / 別リストへの移動 / ゴミ箱ドロップ | S-30 | `Modules/View/ViewDragDropView` (D6 案A) | 標準機能で使用箇所ゼロの領域 |
| A-8 | **`ConfigureLifecycleEvents` に実例投入** — Android の `OnBackPressed` など | S-46 | `MauiProgram.cs:139` | 空実装の解消 |
| A-9 | **電卓** — コアは純モデル (トークナイザ → 操車場アルゴリズム → RPN 評価器)、表示は DSEG7、UI は MauiScientificCalculator を参考、末尾ゼロ除去は SimpleCalculator の `ToTrimmedString` | S-01 / S-02 | `Modules/App/AppCalcView` (D2 案A) | 純モデル + MVVM 分離の見本。DSEG7 の活用先。**複雑化時は D3 の優先順位で機能を切る** |
| A-10 | **設定画面サンプル** — 未使用の `Stepper` / `DatePicker` / `TimePicker` / `RadioButton` / `SearchBar` と `Switch` / `Slider` / `Picker` / `Entry` を網羅し VM に双方向バインド。セクション見出し + グループ枠は `UIKitSettingView` の見た目を流用 | S-18 | `Modules/Basic/BasicSettingView` (D4 案A) | 標準コントロールの欠落を埋める |
| A-11 | **導入済みパッケージの未使用機能をサンプル化** — 1.2 の一覧を D12 の方針で画面に割り付ける。CommunityToolkit の `DockLayout` / `UniformItemsLayout` / `StateContainer` / `AvatarView` / `Expander` / `LazyView` / `RatingView` / `TouchBehavior` / 検証 Behavior、Syncfusion の `SfTabView` / `SfBottomSheet` / `SfOtpInput` / `SfSegmentedControl` / `SfChips` / `SfAccordion` / `SfShimmer` / `SfSparkChart` / `SfSunburstChart` / `SfCalendar` / `SfNumericEntry`、SkiaSharp.Extended の `SKConfettiView` | 1.2 | D12 案B+C (第 5 章で割り付け済み) | **追加コストが最も低く、効果が最も大きい**。新規ライブラリ 0 個 |
| A-12 | **標準 `ToolTipProperties` のサンプル化** — 使用箇所ゼロ。`ToolTipProperties.Text` を付けるだけで長押し / ホバーで説明が出る | S-51 | `Modules/Basic/BasicSettingView` + `Modules/View/` | Shiny の `Tooltip` に相当する機能が標準にある。**最低コスト** |

### 優先度 B — 価値は高いが設計判断を伴う

| # | 内容 | 出典 | 実装先 | 備考 |
| --- | --- | --- | --- | --- |
| B-1 | **`CircularLayout` と `StaggeredGrid` の自前実装** (B-19 を統合) (`ILayoutManager` の `Measure` / `ArrangeChildren`)。子要素を円周上に配置、`Angle` 添付プロパティで個別指定 | S-13 / S-14 | `Layouts/` を新設 | `DockLayout` / `UniformItemsLayout` は CommunityToolkit で充足するため**残る欠落は円形配置と Pinterest 型の 2 つ**。`StaggeredGrid` (高さの異なるカードを列ごとに詰める) は S-51 由来で、CommunityToolkit にも Syncfusion にも無い。ラジアルメニューとカードウォールの題材 |
| B-2 | **`ILayoutManagerFactory` で既存レイアウトのマネージャを差し替える**デモ | S-14 | `Layouts/` | 応用範囲が広い。B-1 と同一画面で見せられる |
| B-3 | **`ChartKind` 追加** — Stacked Bar / Scatter (Bubble) / Heat。加えて**要素ごとにインデックス比例のディレイを与えて出現させる** (`AnimationOption.Wave` の考え方を Canvas 描画へ持ち込む) | S-10 | `Graphics/Drawing/ChartDrawing.cs` | Syncfusion Charts と自前 Drawing の使い分けも同時に示せる |
| B-4 | **Scene のヒットテスト** — タップ座標を論理座標へ変換し、対象プリミティブを選択する | S-04 | `Graphics/Scene/` | Scene 系が入力を受けられるようになる |
| B-5 | **Scene のオブジェクトプール** — `ReusableSpritePool<T>` 相当で GC ポーズを排除 | S-05 | `Graphics/Scene/` | パーティクル / 軌跡を扱う際に必要 |
| B-6 | **Scene の論理解像度固定 + 動的スケール** — 論理サイズを固定し実画面に対してスケールする | S-05 | `Graphics/Scene/SceneObject.cs` | 解像度非依存化 |
| B-7 | **スクロール連動アニメーション** — スクロール位置を 0〜1 に正規化して任意の progress へ流す添付プロパティ。`SKLottieView.Progress` / `AnimationOption.ProgressTo` / Drawing の progress すべてに使える | S-28 | `Behaviors/AnimationOption.cs` (`Behaviors/Scroll.cs` と連携) | 長押しで進行、`CarouselView` オフセット連動も同じ機構 |
| B-8 | **`Marquee` と `TreeView` の自作** | S-11 | `Controls/` | TemplateMAUI 27 種のうち、既存 + CommunityToolkit + Syncfusion で埋まらないのはこの 2 つのみ (`PinBox`→`SfOtpInput`、`ExpanderView`→両 Toolkit、`AvatarView`/`Rate`→CommunityToolkit、`StepBar`→`StepIndicator`、`SegmentedControl`→Syncfusion、`SnackBar`→CommunityToolkit) |
| B-9 | **日次スケジュール表示の強化** (D10 により工数タイマー小アプリから差し替え) — イベント描画のカード化 (色分け / 二段表示)、所要時間ラベルと日合計、空き時間帯のハイライト | S-35 / S-51 | `Modules/UI/UIScheduleView*` + `Controls/DayTimetableView.cs` + `Services/ScheduleService.cs` | **新規画面は作らない**。TimeRecorder の UI 要素を部分的に参照するのみ。**D15 により Shiny 由来は `ScheduleService` の `IScheduleEventProvider` 化のみ**。日ビューの複数日イベント対応とイベント一覧ビューの追加は対応不要。**既に月/日の 2 ビューが同一供給元を共有しており、Shiny の設計思想は実質実装済み** |
| B-10 | **ミニゲーム (数独)** — 純モデル (盤面 / ルール / 判定) と MVVM を分離。盤面モデルは差し替え可能な構造にし、後からライフゲーム / 2048 を追加できるようにする | S-41 | `Modules/App/AppGameView` (D2 案A / D5) | 低優先 |
| B-11 | **Blazor Hybrid サンプル** — `BlazorWebView` で Razor コンポーネントをホストし、`HybridWebView` との違いを見せる | S-36 | `Modules/Sample/SampleBlazorView` | MAUI 基本要素の最大の欠落。**D1 で `Microsoft.AspNetCore.Components.WebView.Maui` の追加は許可済み** |
| B-12 | **相関検証** — 複数プロパティに跨る条件の検証。`ValidationHelper` を拡張し `BasicValidationView` にセクション追加 | S-37 | `Modules/ValidationHelper.cs` + `Modules/Basic/BasicValidationView*` | **D11 により非同期検証 (サーバ問い合わせ) は対象外** |
| B-13 | **`DrawingObject` の画像出力経路** (`SKBitmap` + `SkiaCanvas` に同じ `Render()` を流し PNG 出力) と **`DrawingControl` のインタラクション** (`StartInteraction`/`DragInteraction`/`EndInteraction` を論理座標へ変換して渡す) | S-07 | `Graphics/Drawing/DrawingObject.cs` / `DrawingControl.cs` | Drawing 基盤が「表示専用」から「編集可能 + 出力可能」に広がる。Undo は `List` 末尾削除で足りる |
| B-14 | **Mapsui のオフライン地図 (MBTiles)** | S-50 | `Modules/Sample/SampleMap2*` | **`BruTile.MbTiles` の推移依存を `dotnet list package --include-transitive` で確認するのが先** (D1)。未解決なら可否を再判断 |
| B-15 | **タブ / ボトムシート** — `SfTabView` / `SfBottomSheet` を使うサンプル | S-20 / S-38 | `Modules/View/ViewToolkitView` | 低優先。**D9 により Syncfusion が本線。実装後に自作 (タブ帯 + ViewSwitcher + LazyView) の価値を再判断する** |
| B-16 | **Scene のダブルバッファ (試験導入)** — A-3 の実測後、最も重い 1 シーンに試験導入して再計測し、有意差があれば基盤機能として本採用、無ければ破棄 | S-04 | `Graphics/Scene/SceneObject.cs` | **D8。必ず Release ビルドで測る。結果は `Document/Development.md` へ** |
| B-17 | **`CollectionView.RemainingItemsThreshold` による追加読み込み** — 下方向スクロールで次ページを読む無限スクロール。`RemainingItemsThreshold` + `RemainingItemsThresholdReachedCommand` の 2 属性と VM のコマンド 1 本 | S-51 / 1.2 | `Modules/View/ViewCollectionView.xaml` | **D14 により ChatView から振り替え**。チャットは「最新が末尾・末尾追従」の作りのため、末尾到達で発火する本機能は過去読み込みに使えない |
| B-18 | **【低優先・メモのみ / 実装しない】Walkthrough (要素スポットライト型コーチマーク)** — 対象要素を指してツアーを進め、初回のみ自動実行。実装方針: `Grid` 全面オーバーレイ + `Border` のくり抜き + 対象要素の絶対座標取得、初回判定は `State/Settings.cs` (`IPreferences`) | S-51 | (未定) | **D16 により実装しない。** コストは中 — 対象要素の絶対座標取得と `ScrollView` 内要素への追従が要注意 |
| B-20 | **SCP 転送サンプル** — 空スタブの `Modules/Network/NetworkScpView` を実装し、`NetworkMenuView.xaml` の空きスロット (`Grid.Row="7"`) に結線する。`Services/ScpService.cs` に `Renci.SshNet.ScpClient` のラッパを置いて DI 登録し、接続 / アップロード / ダウンロード / キャンセルを VM から操作する。進捗は `ScpClient.Uploading` / `Downloading` イベント (`Uploaded` / `Size`) を既存 `AnimationOption.ProgressTo` へ流す。アップロード元の選択に **`FilePicker` (現在プロジェクトで使用箇所ゼロ)** を使い、未使用 API のサンプル化も兼ねる。保存先は `FileSystem.CacheDirectory` | ユーザー指示 (2026-08-24) | `Modules/Network/NetworkScp*` + `Services/ScpService.cs` (新規) + `State/Settings.cs` + `Modules/Network/NetworkMenuView.xaml` | **D20〜D22 が前提**。決まれば実装は素直。**Release (トリミング有効) での動作確認が必須** |

### 優先度 C — 小物 / ドキュメント作業

| # | 内容 | 出典 | 実装先 |
| --- | --- | --- | --- |
| C-1 | `PulseRingDrawing` — アイコン周囲に波紋リングを広げる描画 (既存 `AnimationOption.Pulse` は Scale 変化のみ) | S-09 | `Graphics/Drawing/` |
| C-2 | ボタン背面の進捗リング (カウントダウンボタン)。`endAngle = 90 - Progress * 360` | S-08 | `Behaviors/AnimationOption.cs` |
| C-3 | ニューモーフィズム風の影 — `Border` 2 枚を重ねて明暗 2 方向の影を出すスタイル | S-29 | `Modules/View/ViewShadowView.xaml` |
| C-4 | グラデーション線 — 線分ごとに `SKShader.CreateLinearGradient` を差し替える。継ぎ目は丸キャップで緩和 | S-25 | `Graphics/Drawing/ChartDrawing.cs` |
| C-5 | 地図上への SkiaSharp オーバーレイ — メルカトル図法で緯度経度→画面座標を変換し、透過キャンバスに描く。パン時は再描画 | S-25 | `Modules/Sample/SampleMap2*` |
| C-6 | 画像トリミング画面の自作 (SkiaSharp + ジェスチャ + PNG 出力)。B-13 の実演を兼ねる | S-44 | `Modules/Sample/` または `Modules/Device/` |
| C-7 | 音声フロー UI — 「録音 → 文字起こし表示 → 抽出結果プレビュー → 承認」の 4 ステップ。既存 `StepIndicator` と組み合わせる。外部 AI 依存部はローカル / モック化 | S-40 | `Modules/Sample/SampleChat*` 周辺 |
| C-8 | リストの複数選択 + 一括操作 (`SelectionMode="Multiple"`) | S-34 | `Modules/Navigation/Edit/EditListView*` |
| C-10 | **ドキュメント追記** — (1) リスト手段の選定基準 (`CollectionView` / `ListView` / `BindableLayout` / 手動、「Flat is faster than fat」、明示サイズ)、(2) 描画性能は Release ビルドで測る手順と B-16 の計測結果、(3) `SfEffectsView` 98 箇所の使用方針 | S-15 / S-06 | `Document/Development.md` |
| C-12 | **小型入力コントロールの自作 (簡単なもののみ)** — `ColorPicker` (RGBA スライダ 4 本 + プレビュー) と `DurationPicker` (`Picker` 2 個 → `TimeSpan`)。**`RangeSlider` / `AutoCompleteEntry` は難度中のため D17 で見送り** | S-51 | `Controls/` + `Modules/View/ViewInputView` (D17 案B) |
| C-14 | **【後日対応 / 本セッション対象外・D19 と同時に扱う】`CalendarView2` を正とする方針の反映** — `UICalendarView.xaml:57-59` のコメント「どちらを採用するかは未決定」を実態 (`CalendarView2` が正) に合わせて更新する。**API 差分 (`SelectedDayBackground` など `CalendarView2` にのみ存在) の是正は不要** — 旧版が追いついていないだけで、正は `CalendarView2` 側。旧版の削除可否は D19 | 9.6 | `Modules/UI/UICalendarView.xaml` (+ D19 の結果次第で `Controls/CalendarView*`) |
| C-13 | **【検討 / 未確定】`Controls/ChatView` に見た目のバインダブルプロパティを追加** — 自分 / 相手のバブル色など。現状は `Messages` / `InputText` / `SendCommand` の 3 つだけで見た目を外に出していない | S-51 | `Controls/ChatView.xaml` + `.xaml.cs` (D18) |

**取り下げ**: C-9 (NavigationRail 風の縦アイコンナビ) と C-11 (月次集計レポート) は D10 の決定 (新規画面を作らない) により取り下げ。


---

## 5. 画面 (Module) 単位の追加要素一覧

「現在のファイル名」が空欄 (新規) の行は新規作成。項目番号は第 4 章に対応。

### 5.1 Modules/Main

| 対象 | 現在のファイル名 | 何用か | 追加する要素 | 項目 |
| --- | --- | --- | --- | --- |
| トップメニュー | `Modules/Main/MenuView.xaml` | 9 カテゴリへの入口 | `10.App` を追加 | D2 |
| アプリ設定 | `Modules/Main/SettingView.xaml` | 接続先 / API キーの実設定 | 変更なし | — |

### 5.2 Modules/Basic

| 対象 | 現在のファイル名 | 何用か | 追加する要素 | 項目 |
| --- | --- | --- | --- | --- |
| Basic メニュー | `Modules/Basic/BasicMenuView.xaml` | Basic 配下への入口 | 新規画面の項目を追加 | — |
| 設定 UI の型 | (新規) `BasicSettingView` | 標準入力コントロールの網羅 | `Stepper` / `DatePicker` / `TimePicker` / `RadioButton` / `SearchBar` / `Switch` / `Slider` / `Picker` / `Entry`、セクション + グループ枠、`ToolTipProperties` | A-10 / A-12 |
| 検証 | `Modules/Basic/BasicValidationView.xaml` | 入力検証のデモ | **相関検証**セクション、CommunityToolkit の検証 Behavior 7 種 (非同期検証は D11 により対象外) | B-12 / A-11 |
| ロケール | `Modules/Basic/BasicLocaleView.xaml` | 現在のカルチャ名表示のみ | `.resx` 参照結果の一覧、カルチャ別の数値 / 日付 / 通貨 / 序数の書式差 | D7 |
| ビヘイビア | `Modules/Basic/BasicBehaviorView.xaml` | Entry 系 Behavior | `MaskedBehavior` / `UserStoppedTypingBehavior` / `EventToCommandBehavior` | A-11 |

### 5.3 Modules/View

| 対象 | 現在のファイル名 | 何用か | 追加する要素 | 項目 |
| --- | --- | --- | --- | --- |
| View メニュー | `Modules/View/ViewMenuView.xaml` | View 配下への入口 | 新規画面の項目を追加 | — |
| レイアウト | (新規) `ViewLayoutView` | レイアウトの比較デモ | `DockLayout` / `UniformItemsLayout` (CommunityToolkit)、自前 `CircularLayout`、自前 `StaggeredGrid` (Pinterest 型)、`ILayoutManagerFactory` 差し替え | A-11 / B-1 / B-2 / B-19 |
| D&D | (新規) `ViewDragDropView` | ドラッグ&ドロップ | 並べ替え / 別リストへの移動 / ゴミ箱ドロップ | A-7 |
| 自作入力部品 | (新規) `ViewInputView` | 両 Toolkit にも無い小型入力 | `ColorPicker` / `DurationPicker` (自作。`RangeSlider` / `AutoCompleteEntry` は D17 で見送り) | C-12 |
| 状態切替 | (新規) `ViewStateView` | 状態に応じた表示切替 | `StateContainer` (Loading / Error / Empty / Success)、`LazyView` | A-11 |
| 入力/ナビ部品 | (新規) `ViewToolkitView` | 導入済み Toolkit の未使用部品 | `SfTabView` / `SfBottomSheet` / `SfOtpInput` / `SfSegmentedControl` / `SfChips` / `SfAccordion` / `AvatarView` / `Expander` / `RatingView` | A-11 / B-15 |
| 演出 | `Modules/View/ViewEffectView.xaml` | エフェクトのデモ | `SKConfettiView`、`TouchBehavior`、`IconTintColorBehavior` | A-11 |
| Lottie | `Modules/View/ViewLottieView.xaml` | `SKLottieView` の Progress 制御 | スクロール連動 / 長押し進行 | B-7 |
| 影 | `Modules/View/ViewShadowView.xaml` | 影のデモ | ニューモーフィズム風 (`Border` 2 枚重ね) | C-3 |
| SVG | `Modules/View/ViewSvgView.xaml` | `SvgView` のデモ | `Source` 指定 / Placeholder / キャッシュの受け皿 | A-4 |
| GraphicsView | `Modules/View/ViewGraphicsView.xaml` | `IDrawable` のデモ | インタラクション + 画像出力の受け皿 | B-13 |
| 更新 | `Modules/View/ViewRefreshView.xaml` | Pull to refresh | `SfShimmer` (ローディング表現) | A-11 |
| Collection | `Modules/View/ViewCollectionView.xaml` | グループ + SwipeView | `RemainingItemsThreshold` による追加読み込み (無限スクロール) | B-17 |
| 描画 | `Modules/View/ViewDrawingView.xaml` | `DrawingView` (CommunityToolkit) | 変更なし | — |

### 5.4 Modules/Sample

| 対象 | 現在のファイル名 | 何用か | 追加する要素 | 項目 |
| --- | --- | --- | --- | --- |
| Sample メニュー | `Modules/Sample/SampleMenuView.xaml` | Sample 配下への入口 | 新規画面の項目を追加 | — |
| チャート (自前) | `Modules/Sample/SampleChartView.xaml` | `ChartDrawing` の 4 種 | Stacked / Scatter / Heat、要素ごとディレイ出現、グラデーション線 | B-3 / C-4 |
| チャート (Syncfusion) | (新規) `SampleSfChartView` | 導入済み Syncfusion Charts | `SfSparkChart` / `SfSunburstChart` / Polar / Funnel / Pyramid、ダッシュボード構成 | A-11 / S-39 |
| 地図 (Google) | `Modules/Sample/SampleMap1View.xaml` | `maps:Map` + Pin | `Polyline` / `Polygon` / `Circle` | A-5 |
| 地図 (Mapsui) | `Modules/Sample/SampleMap2View.xaml` | 素の `MapControl` | ウィジェット 4 種、Pin + Callout、Polyline / Polygon、GeoJSON、クラスタリング、MBTiles、SkiaSharp オーバーレイ | A-6 / B-14 / C-5 |
| Blazor Hybrid | (新規) `SampleBlazorView` | Razor コンポーネントのホスト | `BlazorWebView` + `HybridWebView` との対比 | B-11 |
| チャット | `Modules/Sample/SampleChatView.xaml` | Ollama チャット | 音声フロー 4 ステップ UI | C-7 |
| 画像トリミング | (新規) `SampleCropView` | SkiaSharp による編集 | ジェスチャ + PNG 出力 | C-6 / B-13 |

### 5.5 Modules/App (新規)

| 対象 | 現在のファイル名 | 何用か | 追加する要素 | 項目 |
| --- | --- | --- | --- | --- |
| App メニュー | (新規) `AppMenuView` | App 配下への入口 | — | D2 |
| 電卓 | (新規) `AppCalcView` | 純モデル + MVVM の見本 | トークナイザ / 操車場アルゴリズム / RPN 評価器、DSEG7 表示 | A-9 |
| ミニゲーム | (新規) `AppGameView` | 盤面ロジックの純モデル化 | 数独 (盤面モデルは差し替え可能に) | B-10 |

### 5.6 Modules/UI

| 対象 | 現在のファイル名 | 何用か | 追加する要素 | 項目 |
| --- | --- | --- | --- | --- |
| UI メニュー | `Modules/UI/UIMenuView.xaml` | 30 画面への入口 (10 行 × 3 列) | 2 画面構成化 (画面数の制約なし) | — |
| 抽選ホイール | (新規) `UIWheelView` | 単発アニメ + 回転テキスト | `WheelDrawing` | A-1 / A-2 |
| スケジュール | `Modules/UI/UIScheduleView.xaml` | 日チップ + 日次タイムテーブル (現在時刻ライン付き) | イベント描画のカード化、所要時間ラベルと日合計、空き時間帯のハイライト、**`IScheduleEventProvider` 化** (複数日イベント / 一覧ビューは対応不要) | B-9 |
| チャット | `Modules/UI/UIChatView.xaml` | 吹き出し / リアクション / 既読 / スタンプ | 変更なし (D14 により機能追加は見送り) | — |
| 設定 (見た目) | `Modules/UI/UIKitSettingView.xaml` | iOS 風グルーピングリスト | 変更なし (A-10 とは役割を分ける) | — |

### 5.7 Modules/Network

| 対象 | 現在のファイル名 | 何用か | 追加する要素 | 項目 |
| --- | --- | --- | --- | --- |
| Network メニュー | `Modules/Network/NetworkMenuView.xaml` | Network 配下への入口 (空きスロット 2 つ) | `Grid.Row="7"` を SCP に結線 | B-20 |
| SCP 転送 | `Modules/Network/NetworkScpView.xaml` | **現在は空スタブ (`<!-- Menu -->` のみ・メニュー未結線で到達不可)** | 接続 / アップロード / ダウンロード / 進捗 / キャンセル / ホスト鍵の指紋確認 | B-20 |
| SCP サービス | (新規) `Services/ScpService.cs` | `ScpClient` のラッパ (DI 登録) | 接続情報の受け渡し、進捗イベントの中継、`CancellationToken` 対応 | B-20 |
| 設定 | `State/Settings.cs` | `IPreferences` + `ISecureStorage` の使い分け | SCP のホスト / ポート / ユーザー名 (Preferences)、パスワード / 秘密鍵 / ホスト鍵指紋 (SecureStorage) | B-20 / D22 |

### 5.8 共通基盤 (画面外)

| 対象 | 現在のファイル名 | 何用か | 追加する要素 | 項目 |
| --- | --- | --- | --- | --- |
| Drawing 基底 | `Graphics/Drawing/DrawingObject.cs` | `IDrawable` 基底 | 単発アニメ + 完了通知、画像出力経路 | A-1 / B-13 |
| Drawing ビュー | `Graphics/Drawing/DrawingControl.cs` | `GraphicsView` ラッパ | インタラクション (論理座標変換) | B-13 |
| ホイール | (新規) `Graphics/Drawing/WheelDrawing.cs` | 抽選ホイール | 扇形 + 回転テキスト + 減速停止 | A-2 |
| 波紋 | (新規) `Graphics/Drawing/PulseRingDrawing.cs` | 波紋リング演出 | — | C-1 |
| チャート | `Graphics/Drawing/ChartDrawing.cs` | 4 種のチャート | Stacked / Scatter / Heat、ディレイ出現、グラデーション線 | B-3 / C-4 |
| Scene 基底 | `Graphics/Scene/SceneObject.cs` | 60fps ループ + 描画基底 | `SKPicture` キャッシュ、ヒットテスト、オブジェクトプール、論理解像度固定、ダブルバッファ (試験) | A-3 / B-4 / B-5 / B-6 / B-16 |
| タイムテーブル | `Controls/DayTimetableView.cs` | 日次タイムテーブル描画 | イベントのカード化、所要時間 / 合計、空き時間ハイライト | B-9 |
| SVG | `Controls/SvgView.cs` | SVG 描画 | `ImageSource` 対応 / キャッシュ / Placeholder / イベント | A-4 |
| Marquee / TreeView | (新規) `Controls/` | 未実装コントロール | — | B-8 |
| レイアウト | (新規) `Layouts/` | カスタムレイアウト | `CircularLayout`、`ILayoutManagerFactory` 差し替え | B-1 / B-2 |
| アニメーション | `Behaviors/AnimationOption.cs` | 演出の添付プロパティ | スクロール連動 progress、進捗リング | B-7 / C-2 |
| 地図バインド | `Behaviors/MapBind.cs` / `MapsuiBind.cs` | コントローラ結線 | `MapElements` 操作、機能グループ別マネージャ分割 | A-5 / A-6 |
| 検証 | `Modules/ValidationHelper.cs` | DataAnnotations 検証 | 相関検証 | B-12 |
| 起動 | `MauiProgram.cs` | DI / 初期化 | `ConfigureLifecycleEvents` の実例 | A-8 |
| ドキュメント | `Document/Development.md` | 開発方針 | リスト選定基準、Release 計測手順、`SfEffectsView` 使用方針 | C-10 |

---

## 6. 不採用

### 6.1 今回のサンプルとしては不要だが、ライブラリ / ツール / 資料としては有用

| 出典 | 理由 |
| --- | --- |
| S-10 beto-rodriguez/LiveCharts2 | チャート表現力は高いが、自前 `ChartDrawing` (B-3 で拡張) と導入済み Syncfusion Charts で足りる。パッケージを重ねる意味が薄い |
| S-20 Sharpnado.Tabs | カスタマイズ性は `SfTabView` より上だが、タブ帯 / コンテンツ分離という設計思想は `SfTabView` が内包している。導入済みで足りる |
| S-16 Redth/Maui.VirtualListView | ネイティブ (`RecyclerView`/`UICollectionView`) リサイクルとアダプタパターン。本サンプルのデータ規模では過剰 |
| S-17 MPowerKit/VirtualizeListView | `Translation` による仮想化。同上 |
| S-18 muak/AiForms.Maui.SettingsView | 13 種のセルは網羅度が高いが、A-10 の自作サンプルで狙い (標準コントロールの網羅) は達成できる |
| S-26 MPowerKit/GoogleMaps | 機能は豊富だが Google Maps (API キー必須) 前提で、フリー地図 (Mapsui) 強化の方針と合わない。**API 設計 (機能グループ別マネージャ + Mapper 辞書) は A-6 に反映済み** |
| S-27 Esri/arcgis-maps-sdk-dotnet-samples | 商用 SDK 前提 |
| S-43 roubachof/Maui.Nuke | iOS の画像キャッシュ (メモリ約 90% 削減)。iOS はスコープ外 |
| S-44 jmbowman1107/ImageCropper.Maui | Android/iOS ネイティブライブラリのラッパ。**自作案は C-6 に反映済み** |
| S-45 Evergine (3D) | 3D エンジン導入は方針外。カスタムハンドラもスコープ外 |
| S-04 DrawnUI (全面採用) | 著者自身が「商用アプリには実験的」と明言。**部分技法は A-3 / B-4〜B-6 に反映済み** |
| S-12 Grial UI Kit の `FluentEmoji` | CDN からの絵文字取得はネットワーク依存でサンプルに不適 |
| S-37 MarimerLLC/csla | ルールエンジン / 認可 / データポータル / モバイルオブジェクトを持つ業務フレームワーク。本サンプルの検証は `DataAnnotations` + `Smart.Mvvm` + CommunityToolkit 検証 Behavior で確立済み。**相関検証のアイデアのみ B-12 に反映済み (非同期検証は D11 により対象外)** |
| S-42 SirJohnK/LocalizationResourceManager.Maui | ランタイムのカルチャ切替機構。根本的な切替は不要という前提のため対象外 |
| S-13 jsuarezruiz/AlohaKit.Layouts | `DockLayout` / `UniformGrid` は CommunityToolkit で充足、`WrapLayout` は標準 `FlexLayout` で代替。**`CircularLayout` のみ自前実装 (B-1)** |
| S-11 jsuarezruiz/TemplateMAUI | 27 種のうち 25 種が既存 + CommunityToolkit + Syncfusion で充足。**`Marquee` / `TreeView` のみ自作 (B-8)** |
| S-19 yurkinh/Plugin.Maui.SegmentedControl | `SfSegmentedControl` (導入済み) で充足 |
| S-32 brminnick/GitTrends | Xamarin.Forms 製。UITests / UnitTests / GraphQL の構成は参考になるが、本サンプルの範囲外 |
| S-31 davidortinau/WeatherTwentyOne | App Actions / システムトレイ / 通知はいずれもスコープ外 |
| S-33 jsuarezruiz/dotnet-maui-showcase | サンプルアプリのカタログ。今後の UI ネタ探しの一次資料として有用 (特に MAUIsland) |
| S-51 Shiny Controls の `DataGrid` / `VirtualizedGrid` / `TableView` / `FrostedGlassView` / Mermaid ダイアグラム / テーマトークン | **いずれも取り込みコストが高い**。DataGrid 系は列固定 + 仮想化 + ソート + セル編集を自作すると別プロジェクト規模。`FrostedGlassView` は Android の `RenderEffect` が API 31+ で下地取得も厄介。Mermaid はパーサ + レイアウトエンジンの自作が必要。テーマトークンは D7 (テーマは触らない) により対象外 |
| S-51 Shiny Controls の Tray Icon / Docking / Blazor レンダラ | デスクトップ / Web 向け。本サンプルは Android 中心 |
| S-48 egvijayanand/dotnet-maui-templates | 開発ツール (テンプレート)。本サンプルは `Smart.*` + `[ViewSource]` ソースジェネレータで定型化済み |
| S-49 mrlacey/MauiAppAccelerator | 開発ツール (VS 拡張) |

### 6.2 本サンプルの方が優れた / 同等の実装を持つため、参考にすること自体が不要

| 出典 | 本サンプル側の該当実装 |
| --- | --- |
| S-09 jsuarezruiz/AlohaKit.Controls | 15 コントロール中 13 が既存で充足 — BarChart/LineChart → `ChartDrawing`、LinearGauge → `Gauge`/`SpeedGauge`/`NoiseGauge`、Slider → `MixerSlider`/`MixerKnob`、Button → `ButtonOption`、BusyIndicator → `LoadDrawing`、CheckBox/ToggleSwitch → `TextToggle` + 標準、ProgressBar → `AnimationOption.ProgressTo`。Avatar / Rating は CommunityToolkit にあり。**設計思想 (Drawable を View から分離) も `DrawingObject`/`DrawingControl` として実装済み**。PulseIcon のみ C-1 に反映 |
| S-12 Grial の `SvgImage` | `Controls/SvgView.cs` が `SKCanvasView` + `Svg.Skia` で同一構成。差分の機能追加は A-4。Skia 上での絵文字描画は `CalendarView2.xaml.cs:290` で `SKTypeface` 明示取得済み |
| S-23 The49.Maui.Toolkit `ViewClickListener` | `Action` を `IOnClickListener` に橋渡しするだけの薄いラッパ。CommunityToolkit `TouchBehavior` の下位集合 |
| S-21 muak/AiForms.Effects `AddCommandPlatformEffect` | `TouchBehavior` (タップ / ロングタップ Command、押下時の見た目変化) + 既存 `ButtonOption` (Ripple / PressEffect) で充足。`SyncCanExecute` に相当する「`CanExecute` に応じて見た目を変える」挙動は、標準 `Button`/`ImageButton` の `Command` バインドが `IsEnabled` として既に行う |
| S-22 muak/AiForms.Maui.Effects の FAB ハンドラ | `SampleMap1View.xaml` の `ImageButton` + `MapFabButton` スタイルで実現済み。**FAB は既存で足りるため対応不要**。カスタムハンドラはスコープ外 |
| S-47 slideshare「Xamarin.Forms の標準UIでUIを作る」 | 提示される 4 デモ (Twitter フォロワー一覧 / プロフィールカード / タイムライン / Instagram 風フィード) は `UISocial` / `UIProfile` / `UITimeline` / `UIStream` として実装済み。Easing 一覧も `ViewEasingView` にある |
| S-02 davidortinau/SimpleCalculator | 全 code-behind (Code-Behind 不使用ポリシーに反する)、コアは 4 演算の `switch` のみ。テーマ切替は方針外。**`ToTrimmedString` (末尾ゼロ / 小数点の除去) のみ A-9 に反映** |
| S-41 dev.to 数独記事 | 要点 3 つのうち、コンパイル済みバインディング (`x:DataType`) は全 View で徹底済み、`AppThemeBinding` は方針外、MVVM は確立済み。**題材のみ B-10 に反映** |
| S-01 MauiScientificCalculator の csproj | net6.0 世代。依存は `CommunityToolkit.Mvvm` と `NCalcSync` の 2 つのみ。**UI 構成のみ A-9 に反映** |
| S-35 TimeRecorder のアーキテクチャ | DDD + オニオン + Reactive Extensions。本サンプルは `Smart.Mvvm` + `Smart.Navigation` + DI で確立済み。**UI 要素のみ B-9 (既存スケジュール画面の強化) に反映。NavigationRail / 月次集計は新規画面になるため対象外** |
| S-36 PhotoAlbum のバックエンド構成 | クリーンアーキテクチャの層分割 (`Backend.BusinessObjects` / `Backend.Core` / `Backend.IoC` / `MemoryRepositories` / `WebApi`) は本サンプルの範囲外。Gateway パターンは `Rester` + `ApiContext` で確立済み。**クライアント要素 (Razor コンポーネント共有) のみ B-11 に反映** |
| S-15 dev.to「All the Lists in .NET MAUI」 | `CollectionView` / `ListView` / `BindableLayout` を既に使い分けている。**基準の明文化のみ C-10 に反映** |
| S-46 c-sharpcorner ライフサイクル記事 | `CreateWindow` / `OnStart` / `OnActivated` / `OnDeactivated` / `OnResumed` は実装済み。**`ConfigureLifecycleEvents` の空実装のみ A-8 に反映** |
| S-06 taublast/Doom.Mobile | 移植エンジン (`ManagedDoom`) 依存。**「Release ビルドで性能を測る」という知見のみ C-10 に反映** |
| S-05 taublast/Breakout | ゲームループ `GameLoop(deltaSeconds)` は `SceneObject.Update(t, dt)` として実装済み (`dt` は `Math.Clamp(t - lastTime, 0f, 0.1f)` でクランプ済み)。**キャッシュ / プール / スケール / 状態機械は A-3 / B-5 / B-6 / A-11 に反映** |

| S-51 Shiny Controls の既存充足分 | `Wizard` → `Modules/Navigation/Wizard/`、Parallax → `Behaviors/Scroll.cs` + `UIProfileView`、`SignaturePad` → CommunityToolkit `DrawingView` (使用中)、`Toast`/`Dialogs` → CommunityToolkit (`BasicDialogView`)、`SkeletonView` → `SfShimmer` (A-11)、`BadgeView`/`PillView` → `SocialNotificationBadge`/`StatusChip`、`SecurityPin` → `SfOtpInput` (A-11)、`TreeView` → B-8 で計画済み、`ImageEditor` → C-6 で計画済み、`CameraView` + フレーム解析 → `CameraOverlayView` + BarcodeScanning + OCR + ONNX (**既存の方が厚い**)、`Markdown` → Indiko、`MediaElement` → CommunityToolkit、Barcode/QR → QRCoder + BarcodeScanning、`Fab` → `MapFabButton` スタイル、`StateView` → `StateContainer` (A-11)、`SheetView`/`Toolbar`/`TabBar` → Syncfusion (B-15)、Keyframe/Motion → `Animations/` + `AnimationOption` + Lottie |

---

## 7. 制約

1. **テーマ**: `UserAppTheme = AppTheme.Light` 固定、`DynamicResource` / `AppThemeBinding` 0 件。根本的な切替は不要という前提のため D7 案B に留める。
2. **共有スタイル変更禁止**: `Styles.xaml` は変更せず `BasedOn` 派生を新設する。
3. **Code-Behind 不使用**: 参照実装の code-behind 実装 (S-02 / S-07 / S-08 など) はそのまま移植できない。VM / Behavior / コントローラパターンへ読み替える。
4. **スコープ外**: 生体認証 / カスタムハンドラ / App Actions / iOS 対応。
5. **FAB**: 既存の `ImageButton` + `MapFabButton` スタイルで足りるため対応不要。
6. **新規テキストファイルは CRLF**、`.editorconfig` 準拠、メンバ変数に `_` 接頭辞なし、ビルド警告ゼロ。警告抑制が必要な場合は事前確認。
7. **実施は番号ベースで 1 項目ずつ**。デザイン判断を伴う統合・置換を一括実施しない。

---

## 8. 出典一覧 (S 番号)

| # | 出典 | 判定 |
| --- | --- | --- |
| S-01 | naweed/MauiScientificCalculator | 採用 (A-9 / UI構成)、csproj は 6.2 |
| S-02 | davidortinau/SimpleCalculator | 6.2 (`ToTrimmedString` のみ A-9) |
| S-03 | blog.bijington「Wheel of names clone」 | 採用 (A-1 / A-2) |
| S-04 | taublast「The Quest To Create Drawn .NET MAUI Apps」 | 採用 (A-3 / B-4 / B-16)、全面採用は 6.1 |
| S-05 | taublast「Breakout」 | 採用 (B-5 / B-6)、記事自体は 6.2 |
| S-06 | taublast/Doom.Mobile | 6.2 (C-10 に反映) |
| S-07 | blog.bijington「Creating a sprite editor」 | 採用 (B-13 / C-6) |
| S-08 | mallibone「Countdown button」 | 採用 (C-2) |
| S-09 | jsuarezruiz/AlohaKit.Controls | 6.2 (C-1 のみ) |
| S-10 | beto-rodriguez/LiveCharts2 | 6.1 (アイデアは B-3) |
| S-11 | jsuarezruiz/TemplateMAUI | 6.1 (B-8 のみ) |
| S-12 | Grial「Emojis in .NET MAUI」 | 採用 (A-4)、SvgImage / FluentEmoji は 6.2 / 6.1 |
| S-13 | jsuarezruiz/AlohaKit.Layouts | 6.1 (B-1 のみ) |
| S-14 | hartez/CustomLayoutExamples | 採用 (B-1 / B-2) |
| S-15 | dev.to「All the Lists in .NET MAUI」 | 6.2 (C-10 に反映) |
| S-16 | Redth/Maui.VirtualListView | 6.1 |
| S-17 | MPowerKit/VirtualizeListView | 6.1 |
| S-18 | muak/AiForms.Maui.SettingsView | 6.1 (A-10 に反映) |
| S-19 | yurkinh/Plugin.Maui.SegmentedControl | 6.1 |
| S-20 | Sharpnado「Full customizable Tabs for MAUI」 | 6.1 (B-15 に反映) |
| S-21 | muak/AiForms.Effects `AddCommandPlatformEffect` | 6.2 |
| S-22 | muak/AiForms.Maui.Effects FAB ハンドラ | 6.2 |
| S-23 | The49.Maui.Toolkit `ViewClickListener` | 6.2 |
| S-24 | devblogs「Drawing Elements on Maps」 | 採用 (A-5) |
| S-25 | Sharpnado「The Run Away! app」 | 採用 (C-4 / C-5) |
| S-26 | MPowerKit/GoogleMaps | 6.1 (設計は A-6 に反映) |
| S-27 | Esri/arcgis-maps-sdk-dotnet-samples | 6.1 |
| S-28 | cayas.de「Lottie + Gestures and Scrolling」 | 採用 (B-7) |
| S-29 | sthewissen/Xamarin.Neumorphism | 採用 (C-3) |
| S-30 | xamgirl「Exploring Drag and Drop」 | 採用 (A-7) |
| S-31 | davidortinau/WeatherTwentyOne | 6.1 |
| S-32 | brminnick/GitTrends | 6.1 |
| S-33 | jsuarezruiz/dotnet-maui-showcase | 6.1 (資料) |
| S-34 | OudomMunint/.NetMAUI-To-Do-List-App | 採用 (C-8)、生体認証はスコープ外 |
| S-35 | ambleside138/TimeRecorder | 採用 (B-9 = 既存スケジュール画面の強化のみ)、アーキテクチャは 6.2 |
| S-36 | drualcman/PhotoAlbum | 採用 (B-11)、バックエンド構成は 6.2 |
| S-37 | MarimerLLC/csla | 6.1 (相関検証のみ B-12 に反映) |
| S-38 | CrossGeeks/xUber | 採用 (B-15) |
| S-39 | Syncfusion「Real-Time Weather Dashboard」 | 採用 (A-11 / ダッシュボード構成) |
| S-40 | devblogs「Multimodal Voice Intelligence」 | 採用 (C-7) |
| S-41 | dev.to「My Journey in Making a Game」(数独) | 6.2 (題材は B-10) |
| S-42 | SirJohnK/LocalizationResourceManager.Maui | 6.1 |
| S-43 | roubachof/Maui.Nuke | 6.1 |
| S-44 | jmbowman1107/ImageCropper.Maui | 6.1 (自作案は C-6) |
| S-45 | devblogs「3D with Evergine」 | 6.1 |
| S-46 | c-sharpcorner「Managing The Application Lifecycle」 | 6.2 (A-8 に反映) |
| S-47 | slideshare「JXUGC #26 Xamarin.Forms UI」 | 6.2 |
| S-48 | egvijayanand/dotnet-maui-templates | 6.1 |
| S-49 | mrlacey/MauiAppAccelerator | 6.1 |
| S-50 | Mapsui/Mapsui (v5 ドキュメント) | 採用 (A-6 / B-14) |
| S-51 | Shiny Controls 1.0 (allanritchie.com / shinylib.net / shinyorg.github.io/controls / github.com/shinyorg/controls) | 部分採用 (A-12 / B-9 / B-17 / B-18 / B-19 / C-12)。パッケージ参照は D13、機能照合は第 9 章 |

---

## 9. 付録: Shiny Controls (S-51) の機能照合

出典: https://allanritchie.com/blog/2026/08/shiny-controls-1-0/ / https://shinylib.net/blog/2026/08/shiny-controls-1-0/ / https://shinyorg.github.io/controls/ / https://github.com/shinyorg/controls

MIT / 無償 / OSS。`Shiny.Maui.Controls` と `Shiny.Blazor.Controls` の 2 パッケージ。70 種超のコントロールが Material 3 のトークン契約を共有する。
v1.0 は 2026-08 リリース。GitHub スター 14 / コミット 219 で、**成熟度は未知数**。

### 9.1 設計思想として価値がある部分

| 思想 | 内容 | 本サンプルへの適用 |
| --- | --- | --- |
| **プロバイダ / セッション抽象** | コレクションを直接バインドせず、`IChatSessionProvider` → セッションスコープの `IChatSession` を介す。ライブイベント購読・送信状態・ページングをコントロール側が持つ | B-17 (範囲は D14)。**フル適用はコスト高** |
| **1 データ供給元 × 複数ビュー** | `ISchedulerEventProvider` 1 本で月 / 日 / 一覧の 3 ビューを賄う | B-9 (D15)。**`ScheduleService` が既に月 / 日で共有されており 2/3 実装済み** |
| **トークン駆動テーマ** | 色役割 / サーフェス / 形状 / 立面 / タイポグラフィ / 密度 / 境界 / 状態 / 余白を契約化し、テーマを丸ごと差し替える | D7 (テーマは触らない) により**対象外** |

### 9.2 ChatView の機能差分

既存: `Controls/ChatView.xaml` + `Modules/UI/UIChatView*` + `Models/Sample/Chat/`

| Shiny の機能 | 既存 | 判定 |
| --- | --- | --- |
| 絵文字リアクション | `MessageReaction` (🍱 / 🙏 / 👀 / 🍣) | 充足 |
| 既読表示 | `ChatMessage.IsRead` + `UIChatView.xaml:304` | 充足 |
| スタンプ / 画像添付 | `StampSource` + `PickStickerCommand` / `PickImageCommand` | 充足 |
| **楽観的送信と送信状態** (送信中 → 成功 / 失敗、失敗と拒否の区別、再送) | `ExecuteSend` が即リストへ追加するのみ。状態を持たない | **欠落 → B-17** |
| **入力中インジケータ** | `AiChatMessage.IsTyping` は AI チャット側のみ。人間チャット側に無い | **欠落 → B-17 (低コスト)** |
| **カーソルベースのページング** | `LoadSampleMessages()` で全件ロード。`RemainingItemsThreshold` 未使用 | **欠落 → B-17** |
| セッションプロバイダ抽象 | VM がコレクションを直接保持 | **コスト高。D14 案C として提示するが非推奨** |

### 9.3 Scheduler の機能差分

既存: `Services/ScheduleService.cs` + `Modules/UI/UICalendarView*` (月) + `Modules/UI/UIScheduleView*` (日) + `Controls/CalendarView2.xaml` + `Controls/DayTimetableView.cs`

| Shiny の機能 | 既存 | 判定 |
| --- | --- | --- |
| 月カレンダービュー | `UICalendarView` + `CalendarView2` (スタンプ / 帯 / 下線スタイル) | 充足 |
| 日アジェンダビュー | `UIScheduleView` + `DayTimetableView` (8-20 時 / 現在時刻ライン) | 充足 |
| 1 データ供給元の共有 | `ScheduleService.GetEvents(DateOnly, DateOnly)` を月 / 日の両 VM が使用 | **2/3 実装済み**。インターフェース化のみ欠落 → B-9 |
| **イベント一覧 (アジェンダ) ビュー** | 無い | **欠落 → B-9** |
| **複数日イベントの全モード帯表示** | 月ビューは `Span` 対応済み (`evEnd = evStart.AddDays(t.Span - 1)`)。**日ビューは `GetEvents(day.Date, day.Date)` で単日しか拾わない** | **欠落 → B-9** |

### 9.4 既存にも両 Toolkit にも無い機能 (自前実装の候補)

| Shiny | 内容 | 判定 | コスト |
| --- | --- | --- | --- |
| **Walkthrough** | 対象要素をスポットライトして宣言順にツアー。`RememberRunKey` で初回のみ | **B-18** | **中** — 絶対座標取得と `ScrollView` 内要素への追従 |
| **StaggeredGrid** | Pinterest 型。高さの異なるカードを列ごとに詰める | **B-19** | 低 (`ILayoutManager`) |
| **Tooltip** | 長押し / ホバーで説明表示 | **A-12**。標準 `ToolTipProperties` が使用箇所ゼロ | **最低** |
| **ColorPicker** | RGBA / HSV の色選択 | **C-12** | 低 (S-07 スプライトエディタの RGBA ピッカーが参考) |
| **RangeSlider** | 下限 / 上限の 2 値スライダ | **C-12** | 低 (`MixerSlider` は単値) |
| **DurationPicker** | 時間量の選択 | **C-12** | 低 |
| **AutoCompleteEntry** | 入力に応じた候補表示 | **C-12** | 低 (`SearchBar` も未使用) |
| **FabMenu** | FAB を押すと放射状にメニューが開く | 低優先。B-1 の `CircularLayout` の応用として実現可 | 低 |
| **FrostedGlassView** | 背景をぼかすすりガラス | **不採用 6.1** | **高** — Android は `RenderEffect` が API 31+、下地の取得も厄介 |
| **Mermaid ダイアグラム** | テキストから図を描画 | **不採用 6.1** | **高** — パーサ + レイアウトエンジンの自作が必要 |
| **DataGrid / VirtualizedGrid / TableView** | 列固定 / 仮想化 / ソート / セル編集 | **不採用 6.1** | **高** — 別プロジェクト規模 |

### 9.5 既存で充足している機能 (対応不要)

`Wizard` (`Modules/Navigation/Wizard/`) / Parallax (`Behaviors/Scroll.cs` + `UIProfileView`) / `SignaturePad` (CommunityToolkit `DrawingView`) / `Toast` / `Dialogs` (CommunityToolkit + `BasicDialogView`) / `SkeletonView` (`SfShimmer` → A-11) / `BadgeView` / `PillView` (`SocialNotificationBadge` / `StatusChip`) / `SecurityPin` (`SfOtpInput` → A-11) / `TreeView` (B-8) / `ImageEditor` (C-6) / `CameraView` + フレーム解析 (`CameraOverlayView` + BarcodeScanning + OCR + ONNX、**既存の方が厚い**) / `Markdown` (Indiko) / `MediaElement` (CommunityToolkit) / Barcode / QR (QRCoder + BarcodeScanning) / `Fab` (`MapFabButton` スタイル) / `StateView` (`StateContainer` → A-11) / `SheetView` / `Toolbar` / `TabBar` (Syncfusion → B-15) / Keyframe / Motion Icons (`Animations/` + `AnimationOption` + Lottie) / `Carousel` (`ViewCarouselView`) / `ProgressBar` (`AnimationOption.ProgressTo`) / `ShinyButton` のスピナー内蔵 (`SfEffectsView` + `ActivityIndicator`)

### 9.6 UI 構成・作り方の観点

機能ではなく「どう組み立てているか」で照合した結果。**Scheduler / ChatView とも、構成面では本サンプル側の方が整っている**箇所が多い。

#### Scheduler

| 観点 | Shiny | 本サンプル | 判定 |
| --- | --- | --- | --- |
| ビューの切り替え方 | 1 コントロール内でモード切替 (月 / 日 / 一覧) | **画面を分ける** (`UICalendarView` / `UIScheduleView`) | 変更不要。「画面ごとに UI サンプルを見せる」という本サンプルの目的には分離の方が合う |
| 実装方式の差し替え | MAUI / Blazor の 2 レンダラが同一 API を共有し、**恒久的に併存する** | `CalendarView` (標準コントロール構成) と `CalendarView2` (Skia 自前描画) が同一の `BindableProperty` API を持ち、XAML のタグ名を書き換えるだけで入れ替わる (`Modules/UI/UICalendarView.xaml:57`) | **併存ではなく移行過程**。`CalendarView2` を正とする決定 (D19) により、2 実装が並ぶのは過渡的な状態で、Shiny の 2 レンダラ設計とは別物。**「API 互換を保ったまま描画方式を丸ごと差し替えられた」という移行手法自体は有効な知見**だが、恒久的な設計として扱わない |
| レイアウト計算の置き場所 | コントロール内部 | **`Models/Sample/Calendar/MonthViewBuilder.cs` がモデル側に分離**。`CreateWeeklyEventCandidates` → `AssignPlacements` → `FindAvailableSlot` → `GetSlotCount` で、複数日イベントの帯レーン割り当てという最も厄介な部分を描画から切り離している | **本サンプルの方が良い**。取り込むものは無い |
| データ供給 | `ISchedulerEventProvider` | `Services/ScheduleService.cs` を月 / 日の両 VM が共有 (具象クラス) | インターフェース化のみ B-9 |

#### ChatView

| 観点 | Shiny | 本サンプル | 判定 |
| --- | --- | --- | --- |
| 見た目の外出し | `MyBubbleColor` / `OtherBubbleColor` をバインダブルにし、利用側が XAML で色を差せる | `Controls/ChatView` は `Messages` / `InputText` / `SendCommand` の 3 つのみ。見た目は内部固定 | **取り込む価値あり → C-13** |
| コントロール化の一貫性 | コントロール 1 本 + `SessionId` バインド | `Controls/ChatView` (汎用) と `Modules/UI/UIChatView` (画面直書き) の**二重実装**。カレンダー側は「コントロール化 → 画面から使う」で統一されているのに、チャット側だけ揃っていない | **D18 で判断**。推奨は現状維持 (用途が違う) |
| スクロール制御 | コントロールがセッションと合わせて内部で持つ | **`behaviors:Scroll.ShowOnAwayFromLastTarget` で「最新へジャンプ」ボタンの表示を添付プロパティに出し、`s:CollectionControlBehavior` の `Controller` 経由で VM からスクロール要求を出す。code-behind ゼロ** | **本サンプルの方が良い**。Code-Behind 不使用ポリシーが効いている |
| 末尾追従 | 記載なし | `ItemsUpdatingScrollMode="KeepLastItemInView"` | 充足 |
