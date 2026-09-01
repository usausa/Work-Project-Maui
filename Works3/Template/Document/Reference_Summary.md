# 外部リファレンス評価 (要約版)

外部の記事・OSS 51 件を「本サンプルに取り込む価値があるか」で評価した結果。
判断根拠・技術詳細は [Reference_Analysis.md](Reference_Analysis.md) を参照。

改訂: 2026-08-24 (前提条件の見直しに伴い全面改訂 / Shiny Controls を S-51 として追加 / SCP (SSH.NET) をユーザー指示で追加 / **D1〜D18 決定済み。D19+C-14 は後日対応。D20〜D22 は未決**)

**前提**: NuGet 追加は禁止ではないが、既存パッケージや他ライブラリで同等機能が実現できるものは追加しない。
追加先は UI 画面に限らず、MAUI の基本要素サンプルであれば任意のモジュールに入れる。
スコープ外 = 生体認証 / カスタムハンドラ / App Actions / iOS。テーマ・ロケールの根本的な切替機構は不要。

---

## 1. 最大の発見

**導入済みパッケージに、一度も使っていない機能が大量にある。** 外部ライブラリを検討する前にここを埋めるのが最も効果が高い。

| パッケージ | 使用中 | 未使用 |
| --- | --- | --- |
| CommunityToolkit.Maui 15.0.0 | MediaElement / DrawingView / CameraView / StatusBarBehavior | **AvatarView / Expander / LazyView / RatingView / SemanticOrderView / DockLayout / UniformItemsLayout / StateContainer / TouchBehavior / AnimationBehavior / IconTintColorBehavior / MaskedBehavior / EventToCommandBehavior / 検証 Behavior 7 種** ほか |
| Syncfusion.Maui.Toolkit 1.0.10 | **SfEffectsView のみ (98 箇所)** | **TabView / BottomSheet / OTP Input / SegmentedControl / Chips / Accordion / Shimmer / Calendar / NumericEntry / Spark・Sunburst・Polar・Funnel・Pyramid Charts / DatePicker / TimePicker / PullToRefresh** ほか約 30 種 |
| SkiaSharp.Extended.UI.Maui 3.0.0 | SKLottieView | **SKConfettiView** |
| Mapsui.Maui 5.1.0 | 素の MapControl + ズーム 3 操作のみ | **ウィジェット 4 種 / Pin+Callout / Polyline・Polygon / GeoJSON / クラスタリング / MBTiles オフライン** |

また、**標準 MAUI コントロールの `Stepper` / `DatePicker` / `TimePicker` / `RadioButton` / `SearchBar` が使用箇所ゼロ**。

これにより、当初「自作が必要」と見ていた項目 (Avatar / Rating / Expander / PinBox / タブ / ボトムシート / DockLayout / UniformGrid / 任意ビューへの Command 添付) は**すべて既存パッケージで充足**する結論に変わった。

---

## 2. 結論

51 件中、取り込み候補は **42 件** (うち B-20 の SCP はユーザー指示による追加)。新規 NuGet の追加は **2 件**（`Microsoft.AspNetCore.Components.WebView.Maui` / `SSH.NET`）+ 確認待ち 1 件（`BruTile.MbTiles`）。Shiny Controls の参照追加は D13 で判断 (推奨: 参照しない)。

| 区分 | 件数 |
| --- | --- |
| 優先度 A (即着手) | 12 |
| 優先度 B (設計判断を伴う) | 20 |
| 優先度 C (小物 / ドキュメント) | 12 |
| 不採用 (1) ライブラリ・資料としては有用 | 22 |
| 不採用 (2) サンプル側が優れており参考自体が不要 | 16 |

---

## 3. 判断項目の決定 (D1〜D12)

| # | 判断内容 | 決定 |
| --- | --- | --- |
| D1 | NuGet 追加の可否 | **案B** — `Microsoft.AspNetCore.Components.WebView.Maui` のみ許可。`BruTile.MbTiles` は `dotnet list package --include-transitive` で推移依存を確認してから判断 |
| D2 | アプリ風サンプルの置き場所 | **案A** — `Modules/App/` 新設 + トップメニュー `10.App`。D10 により配下は電卓とミニゲームの 2 本 |
| D3 | 電卓の仕様 | **案A (科学電卓)**。ただしモデルが複雑になりすぎる場合は機能を省略する。優先順位は ①四則+%+括弧 → ②単項マイナス・三角関数・LOG・EXP → ③累乗・階乗 |
| D4 | 設定画面サンプルの置き場所 | **案A** — `Modules/Basic/BasicSettingView` 新設 |
| D5 | ミニゲームの題材 | **数独**。後からライフゲーム / 2048 を追加できるよう、盤面モデルを差し替え可能な構造にする |
| D6 | リスト D&D の置き場所 | **案A** — `Modules/View/ViewDragDropView` 新設 |
| D7 | テーマ / ロケールの強化 | **案B** — `BasicLocaleView` のみ強化。テーマ (`AppThemeBinding`) は触らない |
| D8 | Scene のダブルバッファ | **実験的に導入して効果を判定し、本採用の可否を決める** (B-16)。A-3 を先に入れて Release ビルドで実測 → 1 シーンに試験導入 → 有意差があれば本採用 |
| D9 | タブ / ボトムシート | **Syncfusion を本線** (B-15)。実装後に自作 (タブ帯 + ViewSwitcher + LazyView) の価値を再判断 |
| D10 | TimeRecorder からの取り込み | **新規画面は作らない**。既存の `UIScheduleView` + `DayTimetableView` を強化するに留め、UI 要素を部分的に参照するのみ。→ B-9 を差し替え、C-9 / C-11 を取り下げ |
| D11 | CSLA からの取り込み | **非同期検証 (サーバ問い合わせ) は実装しない**。相関検証のみ検討 (B-12 のスコープを限定) |
| D12 | 未使用機能サンプルの構成 | **案B + 案C の併用** — 種別別の新規画面 + 既存画面への追記 |

### D13〜D18 (Shiny Controls)

| # | 判断内容 | 決定 |
| --- | --- | --- |
| D13 | Shiny Controls パッケージの参照可否 | **案A — 参照しない**。純増分は自前実装できる規模。v1.0 は 2026-08 リリースで実績も薄い |
| D14 | ChatView 強化の範囲 | **機能の取り込みは全て見送り** (送信状態 / 入力中 / セッション抽象は不要、リアクションは既存で充足)。`RemainingItemsThreshold` は**チャットでは簡単に該当しない**ため `ViewCollectionView` へ振り替え (B-17) |
| D15 | Scheduler 強化の範囲 | **`ScheduleService` の `IScheduleEventProvider` 化のみ**。日ビューの複数日イベント対応・イベント一覧ビューは対応不要 |
| D16 | Walkthrough の実装可否 | **低優先。実装せず、実装方針のメモのみ残す** (B-18) |
| D17 | 小型入力コントロール群 | **簡単なものだけ採用** — `ColorPicker` / `DurationPicker` は採用、`RangeSlider` / `AutoCompleteEntry` (難度中) は見送り |
| D18 | チャット UI の二重実装 (`Controls/ChatView` と `UIChatView`) | **案A — 現状維持**。用途の違いによる意図的な使い分けとして残す。C-13 (バブル色のバインダブル化) は**検討扱い** |
| — | カレンダーの正 | **`CalendarView2` (Skia 自前描画版) を正とする** |
| D20 | **SSH.NET (SCP) の追加可否** | **未決**。`SSH.NET` 2026.0.0 / MIT / 推移依存に **`BouncyCastle.Cryptography` (新規)**。.NET 標準にも導入済みパッケージにも同等機能は無いため追加は正当。案A: 追加 (推移依存を事前確認) / 案B: 追加しない / 案C: 増分次第で再判断。**推奨は案A**。ただし **Android Release はトリミングされるため実機確認が必須** |
| D21 | SCP サンプルのスコープ | **未決**。案A: SCP のみ (アップロード / ダウンロード + 進捗) / 案B: SCP + SFTP (一覧・削除・リネーム) / 案C: SCP + リモートコマンド実行。**推奨は案A** |
| D22 | 接続情報の保管とホスト鍵検証 | **未決**。保管は案A: ホスト/ポート/ユーザー名を `IPreferences`、パスワード/鍵を `ISecureStorage` (既存 `Settings` に倣う) を推奨。ホスト鍵は案A: **TOFU (初回に指紋を承認して保存、以降は不一致で中断)** を推奨。**無条件受け入れは避ける** |
| D19 | 未使用になった `CalendarView` (旧) の扱い | **【後日対応 / 本セッション対象外】**。旧版は 1,490 行が**どこからも参照されていない**。案A: 削除し `CalendarView2` を `CalendarView` にリネーム / 案B: 削除のみ / 案C: 比較サンプルとして残す。**推奨は案A**（案C にも教材価値あり）。C-14 も同時に扱う |


---

## 4. 優先度 A — 即着手

| # | やること | 実装先 (現在のファイル名) |
| --- | --- | --- |
| A-1 | 単発アニメーション + 完了通知の共通機構 (減速停止 + コールバック) | `Graphics/Drawing/` |
| A-2 | 回転テキスト描画 + 抽選ホイール | `Graphics/Drawing/WheelDrawing.cs` (新規) + `Modules/UI/UIWheelView` (新規) |
| A-3 | Scene の静的レイヤ `SKPicture` キャッシュ | `Graphics/Scene/SceneObject.cs` + 4 Scene |
| A-4 | `SvgView` に `ImageSource` 対応 / キャッシュ / Placeholder / イベント | `Controls/SvgView.cs` |
| A-5 | Google Maps に `Polyline` / `Polygon` / `Circle` | `Modules/Sample/SampleMap1ViewModel.cs` |
| A-6 | Mapsui 強化 (ウィジェット / Callout / 図形 / GeoJSON / クラスタリング) | `Modules/Sample/SampleMap2*` + `Behaviors/MapsuiBind.cs` |
| A-7 | リスト項目のドラッグ&ドロップ | `Modules/View/ViewDragDropView` (新規) |
| A-8 | `ConfigureLifecycleEvents` に実例投入 | `MauiProgram.cs:139` |
| A-9 | 電卓 (純モデル + DSEG7) | `Modules/App/AppCalcView` (新規) |
| A-10 | 設定画面サンプル (未使用の標準入力コントロールを網羅) | `Modules/Basic/BasicSettingView` (新規) |
| A-11 | **導入済みパッケージの未使用機能をサンプル化** (第1章の一覧) | 第7章の割り付けに従う |
| A-12 | 標準 `ToolTipProperties` のサンプル化 (使用箇所ゼロ / 最低コスト) | `BasicSettingView` + `Modules/View/` |

---

## 5. 優先度 B

| # | やること | 実装先 |
| --- | --- | --- |
| B-1 | `CircularLayout` と **`StaggeredGrid`** の自前実装 (`ILayoutManager`) | `Layouts/` (新規) |
| B-2 | `ILayoutManagerFactory` でレイアウトマネージャを差し替えるデモ | `Layouts/` |
| B-3 | `ChartKind` 追加 (Stacked / Scatter / Heat) + 要素ごとディレイ出現 | `Graphics/Drawing/ChartDrawing.cs` |
| B-4 | Scene のヒットテスト (タップで対象選択) | `Graphics/Scene/` |
| B-5 | Scene のオブジェクトプール | `Graphics/Scene/` |
| B-6 | Scene の論理解像度固定 + 動的スケール | `Graphics/Scene/SceneObject.cs` |
| B-7 | スクロール連動アニメーション (正規化 progress) | `Behaviors/AnimationOption.cs` |
| B-8 | `Marquee` / `TreeView` の自作 (未充足はこの 2 つのみ) | `Controls/` |
| B-9 | **日次スケジュール表示の強化** — イベントのカード化 / 所要時間と日合計 / 空き時間ハイライト、**`IScheduleEventProvider` 化** (D15) | `Modules/UI/UIScheduleView*` + `Controls/DayTimetableView.cs` + `Services/ScheduleService.cs` |
| B-10 | ミニゲーム (数独。盤面モデルは差し替え可能に) | `Modules/App/AppGameView` (新規) |
| B-11 | Blazor Hybrid サンプル (`BlazorWebView`) | `Modules/Sample/SampleBlazorView` (新規) |
| B-12 | 相関検証 (非同期検証は対象外) | `Modules/ValidationHelper.cs` + `BasicValidationView` |
| B-13 | `DrawingObject` の画像出力経路 + `DrawingControl` のインタラクション | `Graphics/Drawing/` |
| B-14 | Mapsui のオフライン地図 (MBTiles) — 推移依存の確認が先 | `Modules/Sample/SampleMap2*` |
| B-15 | タブ / ボトムシート (`SfTabView` / `SfBottomSheet`) | `Modules/View/ViewToolkitView` (新規) |
| B-16 | **Scene のダブルバッファ (試験導入 → 効果判定 → 採否)** | `Graphics/Scene/SceneObject.cs` |
| B-17 | **`CollectionView.RemainingItemsThreshold` による追加読み込み** (無限スクロール) — D14 により ChatView から振り替え | `Modules/View/ViewCollectionView.xaml` |
| B-18 | **【低優先・メモのみ / 実装しない】Walkthrough (要素スポットライト型コーチマーク)** — コスト中 (D16) | (未定) |
| ~~B-19~~ | `StaggeredGrid` は **B-1 に統合** | `Layouts/` |
| B-20 | **SCP 転送サンプル** — 空スタブの `NetworkScpView` を実装しメニューに結線。接続 / アップロード / ダウンロード / 進捗 / キャンセル / ホスト鍵確認。`FilePicker` (使用箇所ゼロ) のサンプル化も兼ねる (D20〜D22 が前提) | `Modules/Network/NetworkScp*` + `Services/ScpService.cs` (新規) + `State/Settings.cs` |

---

## 6. 優先度 C

| # | やること | 実装先 |
| --- | --- | --- |
| C-1 | `PulseRingDrawing` (波紋リング) | `Graphics/Drawing/` |
| C-2 | ボタン背面の進捗リング | `Behaviors/AnimationOption.cs` |
| C-3 | ニューモーフィズム風の影 (`Border` 2 枚重ね) | `Modules/View/ViewShadowView.xaml` |
| C-4 | グラデーション線 (線分ごとにシェーダ差し替え) | `Graphics/Drawing/ChartDrawing.cs` |
| C-5 | 地図上への SkiaSharp オーバーレイ | `Modules/Sample/SampleMap2*` |
| C-6 | 画像トリミング画面の自作 | `Modules/Sample/SampleCropView` (新規) |
| C-7 | 音声フロー UI (録音→文字起こし→プレビュー→承認) | `Modules/Sample/SampleChat*` |
| C-8 | リストの複数選択 + 一括操作 | `Modules/Navigation/Edit/EditListView*` |
| C-10 | ドキュメント追記 (リスト選定基準 / Release 計測手順と B-16 の結果 / SfEffectsView 方針) | `Document/Development.md` |
| C-12 | 小型入力の自作 — **`ColorPicker` / `DurationPicker` のみ** (D17) | `Controls/` + `Modules/View/ViewInputView` (新規) |
| C-13 | **【検討 / 未確定】** `Controls/ChatView` に見た目のバインダブルプロパティ (バブル色など) を追加 | `Controls/ChatView.xaml*` |
| C-14 | **【後日対応 / D19 と同時】** `CalendarView2` を正とする方針の反映 — `UICalendarView.xaml:57-59` のコメント更新。API 差分の是正は不要 | `Modules/UI/UICalendarView.xaml` |

**取り下げ**: C-9 (NavigationRail 風の縦アイコンナビ) / C-11 (月次集計レポート) — D10 の決定 (新規画面を作らない) による。

---

## 7. 画面 (Module) 単位の追加要素一覧

「現在のファイル名」が (新規) の行は新規作成。

| モジュール | 対象 | 現在のファイル名 | 何用か | 追加する要素 | 項目 |
| --- | --- | --- | --- | --- | --- |
| Main | トップメニュー | `Modules/Main/MenuView.xaml` | 9 カテゴリへの入口 | `10.App` を追加 | D2 |
| Main | アプリ設定 | `Modules/Main/SettingView.xaml` | 接続先 / API キーの実設定 | 変更なし | — |
| Basic | Basic メニュー | `Modules/Basic/BasicMenuView.xaml` | Basic 配下への入口 | 新規画面の項目 | — |
| Basic | 設定 UI の型 | (新規) `BasicSettingView` | 標準入力コントロールの網羅 | Stepper / DatePicker / TimePicker / RadioButton / SearchBar / Switch / Slider / Picker / Entry | A-10 |
| Basic | 検証 | `Modules/Basic/BasicValidationView.xaml` | 入力検証のデモ | 相関検証 / CommunityToolkit 検証 Behavior 7 種 | B-12 / A-11 |
| Basic | ロケール | `Modules/Basic/BasicLocaleView.xaml` | カルチャ名の表示のみ | resx 参照結果、カルチャ別の数値/日付/通貨/序数の書式差 | D7 |
| Basic | ビヘイビア | `Modules/Basic/BasicBehaviorView.xaml` | Entry 系 Behavior | MaskedBehavior / UserStoppedTypingBehavior / EventToCommandBehavior | A-11 |
| View | View メニュー | `Modules/View/ViewMenuView.xaml` | View 配下への入口 | 新規画面の項目 | — |
| View | レイアウト | (新規) `ViewLayoutView` | レイアウトの比較デモ | DockLayout / UniformItemsLayout / 自前 CircularLayout / LayoutManager 差し替え | A-11 / B-1 / B-2 |
| View | D&D | (新規) `ViewDragDropView` | ドラッグ&ドロップ | 並べ替え / 別リストへ移動 / ゴミ箱ドロップ | A-7 |
| View | 状態切替 | (新規) `ViewStateView` | 状態に応じた表示切替 | StateContainer / LazyView | A-11 |
| View | 入力/ナビ部品 | (新規) `ViewToolkitView` | 導入済み Toolkit の未使用部品 | SfTabView / SfBottomSheet / SfOtpInput / SfSegmentedControl / SfChips / SfAccordion / AvatarView / Expander / RatingView | A-11 / B-15 |
| View | 演出 | `Modules/View/ViewEffectView.xaml` | エフェクトのデモ | SKConfettiView / TouchBehavior / IconTintColorBehavior | A-11 |
| View | Lottie | `Modules/View/ViewLottieView.xaml` | SKLottieView の Progress 制御 | スクロール連動 / 長押し進行 | B-7 |
| View | 影 | `Modules/View/ViewShadowView.xaml` | 影のデモ | ニューモーフィズム風 | C-3 |
| View | SVG | `Modules/View/ViewSvgView.xaml` | SvgView のデモ | Source 指定 / Placeholder / キャッシュ | A-4 |
| View | GraphicsView | `Modules/View/ViewGraphicsView.xaml` | IDrawable のデモ | インタラクション + 画像出力 | B-13 |
| View | 更新 | `Modules/View/ViewRefreshView.xaml` | Pull to refresh | SfShimmer | A-11 |
| Sample | Sample メニュー | `Modules/Sample/SampleMenuView.xaml` | Sample 配下への入口 | 新規画面の項目 | — |
| Sample | チャート (自前) | `Modules/Sample/SampleChartView.xaml` | ChartDrawing の 4 種 | Stacked / Scatter / Heat、ディレイ出現、グラデーション線 | B-3 / C-4 |
| Sample | チャート (Syncfusion) | (新規) `SampleSfChartView` | 導入済み Syncfusion Charts | Spark / Sunburst / Polar / Funnel / Pyramid、ダッシュボード構成 | A-11 |
| Sample | 地図 (Google) | `Modules/Sample/SampleMap1View.xaml` | Map + Pin | Polyline / Polygon / Circle | A-5 |
| Sample | 地図 (Mapsui) | `Modules/Sample/SampleMap2View.xaml` | 素の MapControl | ウィジェット / Callout / 図形 / GeoJSON / クラスタリング / MBTiles / Skia オーバーレイ | A-6 / B-14 / C-5 |
| Sample | Blazor Hybrid | (新規) `SampleBlazorView` | Razor コンポーネントのホスト | BlazorWebView と HybridWebView の対比 | B-11 |
| Sample | チャット | `Modules/Sample/SampleChatView.xaml` | Ollama チャット | 音声フロー 4 ステップ UI | C-7 |
| Sample | 画像トリミング | (新規) `SampleCropView` | SkiaSharp による編集 | ジェスチャ + PNG 出力 | C-6 / B-13 |
| App | App メニュー | (新規) `AppMenuView` | App 配下への入口 | — | D2 |
| App | 電卓 | (新規) `AppCalcView` | 純モデル + MVVM の見本 | トークナイザ / 操車場アルゴリズム / RPN 評価器、DSEG7 | A-9 |
| App | ミニゲーム | (新規) `AppGameView` | 盤面ロジックの純モデル化 | 数独 (差し替え可能な盤面モデル) | B-10 |
| UI | UI メニュー | `Modules/UI/UIMenuView.xaml` | 30 画面への入口 | 2 画面構成化 | — |
| UI | 抽選ホイール | (新規) `UIWheelView` | 単発アニメ + 回転テキスト | WheelDrawing | A-1 / A-2 |
| UI | スケジュール | `Modules/UI/UIScheduleView.xaml` | 日チップ + 日次タイムテーブル | イベントのカード化 / 所要時間と日合計 / 空き時間ハイライト | B-9 |
| View | 自作入力部品 | (新規) `ViewInputView` | 両 Toolkit にも無い小型入力 | ColorPicker / DurationPicker | C-12 |
| View | Collection | `Modules/View/ViewCollectionView.xaml` | グループ + SwipeView | `RemainingItemsThreshold` による追加読み込み | B-17 |
| Network | Network メニュー | `Modules/Network/NetworkMenuView.xaml` | Network 配下への入口 (空きスロット 2 つ) | `Grid.Row="7"` を SCP に結線 | B-20 |
| Network | SCP 転送 | `Modules/Network/NetworkScpView.xaml` | **現在は空スタブ・メニュー未結線で到達不可** | 接続 / アップロード / ダウンロード / 進捗 / キャンセル / ホスト鍵の指紋確認 | B-20 |
| 基盤 | SCP サービス | (新規) `Services/ScpService.cs` | `ScpClient` のラッパ | DI 登録 / 進捗イベント中継 / キャンセル対応 | B-20 |
| 基盤 | 設定 | `State/Settings.cs` | `IPreferences` + `ISecureStorage` の使い分け | SCP のホスト / 認証情報 / ホスト鍵指紋 | B-20 |
| 基盤 | チャットコントロール | `Controls/ChatView.xaml` | AI チャット用の吹き出し | 見た目のバインダブルプロパティ | C-13 |
| 基盤 | カレンダーコントロール | `Controls/CalendarView2.xaml` (正) / `CalendarView.xaml` (旧・未参照 1,490 行) | 月カレンダー描画 | **後日対応** — コメント反映と旧版の削除/リネーム | C-14 / D19 |
| 基盤 | スケジュール供給 | `Services/ScheduleService.cs` | 月/日ビュー共有のイベント供給 | `IScheduleEventProvider` 化 | B-9 |
| UI | 設定 (見た目) | `Modules/UI/UIKitSettingView.xaml` | iOS 風グルーピングリスト | 変更なし (A-10 と役割を分ける) | — |
| 基盤 | Drawing 基底 | `Graphics/Drawing/DrawingObject.cs` | IDrawable 基底 | 単発アニメ + 完了通知、画像出力経路 | A-1 / B-13 |
| 基盤 | Drawing ビュー | `Graphics/Drawing/DrawingControl.cs` | GraphicsView ラッパ | インタラクション (論理座標変換) | B-13 |
| 基盤 | チャート | `Graphics/Drawing/ChartDrawing.cs` | 4 種のチャート | Stacked / Scatter / Heat ほか | B-3 / C-4 |
| 基盤 | Scene 基底 | `Graphics/Scene/SceneObject.cs` | 60fps ループ + 描画基底 | SKPicture キャッシュ / ヒットテスト / プール / 論理解像度 / ダブルバッファ(試験) | A-3 / B-4 / B-5 / B-6 / B-16 |
| 基盤 | タイムテーブル | `Controls/DayTimetableView.cs` | 日次タイムテーブル描画 | イベントのカード化 / 所要時間 / 空き時間 | B-9 |
| 基盤 | SVG | `Controls/SvgView.cs` | SVG 描画 | ImageSource / キャッシュ / Placeholder | A-4 |
| 基盤 | 新規コントロール | (新規) `Controls/` | 未実装コントロール | Marquee / TreeView | B-8 |
| 基盤 | レイアウト | (新規) `Layouts/` | カスタムレイアウト | CircularLayout / LayoutManager 差し替え | B-1 / B-2 |
| 基盤 | アニメーション | `Behaviors/AnimationOption.cs` | 演出の添付プロパティ | スクロール連動 progress、進捗リング | B-7 / C-2 |
| 基盤 | 地図バインド | `Behaviors/MapBind.cs` / `MapsuiBind.cs` | コントローラ結線 | MapElements 操作、機能グループ別マネージャ分割 | A-5 / A-6 |
| 基盤 | 検証 | `Modules/ValidationHelper.cs` | DataAnnotations 検証 | 相関検証 | B-12 |
| 基盤 | 起動 | `MauiProgram.cs` | DI / 初期化 | ConfigureLifecycleEvents の実例 | A-8 |
| 基盤 | ドキュメント | `Document/Development.md` | 開発方針 | リスト選定基準 / Release 計測手順 / SfEffectsView 方針 | C-10 |

---

## 8. 不採用 (1) — 今回のサンプルとしては不要だが、ライブラリ / ツール / 資料としては有用

| 出典 | 理由 |
| --- | --- |
| LiveCharts2 | 自前 ChartDrawing + 導入済み Syncfusion Charts で足りる |
| Sharpnado.Tabs | `SfTabView` (導入済み) で足りる。カスタマイズ性は上 |
| Redth/Maui.VirtualListView | ネイティブ仮想化。本サンプルのデータ規模では過剰 |
| MPowerKit/VirtualizeListView | 同上 |
| AiForms.Maui.SettingsView | セル網羅度は上。狙いは A-10 の自作で達成できる |
| MPowerKit/GoogleMaps | API キー必須。フリー地図 (Mapsui) 強化の方針と合わない。**API 設計は A-6 に反映** |
| Esri ArcGIS samples | 商用 SDK 前提 |
| Maui.Nuke | iOS 向け。スコープ外 |
| ImageCropper.Maui | ネイティブラッパ。**自作案は C-6 に反映** |
| Evergine (3D) | 方針外。カスタムハンドラもスコープ外 |
| DrawnUI (全面採用) | 著者自身が実験的と明言。**部分技法は A-3 / B-4〜B-6 / B-16 に反映** |
| Grial UI Kit の FluentEmoji | CDN 依存 |
| CSLA | 業務フレームワーク。検証は既存 + CommunityToolkit で足りる。**相関検証のアイデアのみ B-12 に反映** |
| LocalizationResourceManager.Maui | 根本的な切替機構は不要という前提 |
| AlohaKit.Layouts | DockLayout / UniformGrid は CommunityToolkit、WrapLayout は FlexLayout で充足。**CircularLayout のみ B-1** |
| TemplateMAUI | 27 種中 25 種が充足。**Marquee / TreeView のみ B-8** |
| Plugin.Maui.SegmentedControl | `SfSegmentedControl` (導入済み) で充足 |
| brminnick/GitTrends | Xamarin.Forms 製。テスト / GraphQL 構成は範囲外 |
| WeatherTwentyOne | App Actions / システムトレイ / 通知はスコープ外 |
| dotnet-maui-showcase | サンプルカタログ。今後のネタ探し資料 (特に MAUIsland) |
| dotnet-maui-templates / MauiAppAccelerator | 開発ツール |
| Shiny Controls の `DataGrid` / `VirtualizedGrid` / `TableView` / `FrostedGlassView` / Mermaid / テーマトークン | **いずれも取り込みコストが高い**。DataGrid 系は別プロジェクト規模、`FrostedGlassView` は Android `RenderEffect` が API31+、Mermaid はパーサ + レイアウトエンジン自作。テーマは D7 により対象外 |
| Shiny Controls の Tray Icon / Docking / Blazor レンダラ | デスクトップ / Web 向け。本サンプルは Android 中心 |

## 9. 不採用 (2) — 本サンプル側が優れた / 同等の実装を持ち、参考にすること自体が不要

| 出典 | 本サンプル側の該当実装 |
| --- | --- |
| AlohaKit.Controls | 15 中 13 が既存で充足 (ChartDrawing / Gauge / SpeedGauge / NoiseGauge / MixerSlider / ButtonOption / LoadDrawing / TextToggle / ProgressTo)。Avatar・Rating は CommunityToolkit。設計思想も DrawingObject/DrawingControl として実装済み。**PulseIcon のみ C-1** |
| Grial の SvgImage | `Controls/SvgView.cs` が同一構成。**差分機能は A-4** |
| The49.Maui.Toolkit ViewClickListener | CommunityToolkit `TouchBehavior` の下位集合 |
| AiForms.Effects AddCommandPlatformEffect | `TouchBehavior` + 既存 `ButtonOption` + 標準 Command の IsEnabled 連動で充足 |
| AiForms.Maui.Effects の FAB ハンドラ | `ImageButton` + `MapFabButton` スタイルで実現済み。**FAB は対応不要** |
| slideshare JXUGC #26 | 4 デモは UISocial / UIProfile / UITimeline / UIStream として実装済み。Easing 一覧も ViewEasingView にある |
| SimpleCalculator | 全 code-behind (ポリシー違反)、コアは switch のみ。**`ToTrimmedString` のみ A-9** |
| dev.to 数独記事 | コンパイル済みバインディングは全 View で徹底済み、AppThemeBinding は方針外。**題材のみ B-10** |
| MauiScientificCalculator の csproj | net6.0 世代、依存 2 つのみ。**UI 構成のみ A-9** |
| TimeRecorder のアーキテクチャ | DDD + オニオン。本サンプルは Smart.* + DI で確立済み。**UI 要素のみ B-9 (既存スケジュール画面の強化) に反映。NavigationRail / 月次集計は新規画面になるため対象外** |
| PhotoAlbum のバックエンド構成 | 層分割は範囲外、Gateway は Rester + ApiContext で確立済み。**クライアント要素 (Razor 共有) のみ B-11** |
| All the Lists in .NET MAUI | 3 手段を既に使い分け済み。**明文化のみ C-10** |
| c-sharpcorner ライフサイクル記事 | CreateWindow / OnStart / OnActivated 等は実装済み。**空実装の解消のみ A-8** |
| Doom.Mobile | 移植エンジン依存。**Release 計測の知見のみ C-10** |
| Breakout | GameLoop は `SceneObject.Update(t, dt)` として実装済み (dt もクランプ済み)。**キャッシュ / プール / スケールは A-3 / B-5 / B-6** |

---
| Shiny Controls の既存充足分 | `Wizard` → `Modules/Navigation/Wizard/`、Parallax → `Behaviors/Scroll.cs`、`SignaturePad` → CommunityToolkit `DrawingView`、Toast/Dialogs → CommunityToolkit、`SkeletonView` → `SfShimmer`、Badge/Pill → `SocialNotificationBadge`/`StatusChip`、`SecurityPin` → `SfOtpInput`、`TreeView` → B-8、`ImageEditor` → C-6、`CameraView` + フレーム解析 → 既存の方が厚い、Markdown/MediaElement/Barcode/Fab/StateView/SheetView/Tab → すべて既存または導入済み Toolkit |

## 10. 制約

1. テーマは `UserAppTheme = Light` 固定、`DynamicResource` / `AppThemeBinding` 0 件。D7 の決定によりテーマ側は触らない。
2. 共有スタイル変更禁止 — `Styles.xaml` は変更せず `BasedOn` 派生を新設。
3. Code-Behind 不使用 — 参照実装の code-behind は VM / Behavior へ読み替える。
4. スコープ外 — 生体認証 / カスタムハンドラ / App Actions / iOS。
5. FAB は既存で足りるため対応不要。
6. 新規テキストファイルは CRLF、`.editorconfig` 準拠、ビルド警告ゼロ。

---

## 11. 次アクション

判断項目は全て決定済み。優先度 A から番号ベースで 1 項目ずつ着手する。
**A-11 (導入済みパッケージの未使用機能のサンプル化) が最も費用対効果が高く、新規ライブラリは 0 個**。
着手を指示する際は「A-3 を実施」のように番号で指定する。
