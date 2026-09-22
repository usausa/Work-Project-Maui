# 📝変更内容まとめ (uibase → fix3)

`Works3/Template` 配下の変更を、git タグの区間ごとに **画面単位** でまとめたドキュメント。
**Git の差分を確認しながら「何を行なったのか」を確認するための参考資料**とすることを目的とする。
各区間は「A. 画面単位の変更」→「B. 画面以外の変更」(+「C. この区間のナレッジ」) の順で記載し、区間に紐付かない恒常情報(ポリシー / 意図的差異 / 資産レシピ / 決定アーカイブ)は末尾の**付録**に置く。

## 🗺️区間の概要

| 区間 | 期間 | コミット | 変更ファイル | 変更行 | 性格 |
|---|---|---|---|---|---|
| [🎨1. uibase → uibase2](#1-uibase--uibase2--ui-ブラッシュアップ) | 2026-05-16 → 07-21 | 35 | 338 | +30,254 / -2,474 | **UI ブラッシュアップ**(新規20画面・全画面の作り込み・描画基盤再編) |
| [🔍2. uibase2 → uibase3](#2-uibase2--uibase3--コードレビュー対応) | 2026-07-21 → 08-15 | 10 | 122 | +2,472 / -1,020 | **コードレビュー対応**(不具合修正・堅牢化・権限/DB/通信の見直し) |
| [📚3. uibase3 → uibase4](#3-uibase3--uibase4--外部リファレンス評価とライブラリ追従) | 2026-08-15 → 09-01 | 1 | 10 | +1,273 / -26 | **外部リファレンス評価**の追加とライブラリ API 追従 |
| [🧹4. uibase4 → fix1](#4-uibase4--fix1--アナライザ設定の全面見直し) | 2026-09-01 | 1 | 42 | +308 / -172 | **アナライザ設定の全面見直し**と機械的追従 |
| [🔨5. fix1 → plus1](#5-fix1--plus1--外部リファレンス評価の実装フェーズ110) | 2026-09-01 → 09-03 | 3 | 130 | +9,967 / -1,781 | **外部リファレンス評価の実装**(新規12画面・App モジュール新設・SCP・描画基盤拡張) |
| [🧩6. plus1 → baseup1](#6-plus1--baseup1--基盤刷新di-移行白画面対策メニュー再編) | 2026-09-03 → 09-05 | 9 | 135 | +2,129 / -1,693 | **基盤刷新**(DI コンテナ移行・BACK/白画面対策・メニュー再編・ドキュメント統合) |
| [🐛7. baseup1 → fix2](#7-baseup1--fix2--resharper-全件対応と-scene-描画の重大バグ修正) | 2026-09-05 | 1 | 66 | +459 / -333 | **ReSharper 全件対応**(254 件)と **Scene 描画の重大バグ修正**(かくつき・ANR・SIGSEGV) |
| [🔙8. fix2 → back](#8-fix2--back--back初期化方式の刷新白画面対策-b-1-の方式変更) | 2026-09-05 → 09-06 | 4 | 15 | +276 / -233 | **BACK/初期化方式の刷新**(白画面対策 B-1 を StartupState 方式へ・ApplicationInitializer 廃止) |
| [📅9. back → fix3](#9-back--fix3--calendar--location-の手直し) | 2026-09-06 | 2 | 4 | +15 / -36 | **Calendar / Location の手直し**(Debug 計測撤去・未取得表示の空状態化) |

- 関連ドキュメント: 残作業(SCP 実テスト / 移管課題 / 参照サンプルからの追加機能)は `Task_Checklist.md`(**2026-09-03 に `UI_Verification_Checklist.md` + `Implementation_Checklist.md` + 旧 `UI_Task_Checklist.md` + `Image_Asset_Expansion_Plan.md` を統合**)。
  - `Fix_Checklist.md`(区間2で作成)と `Reference_Summary.md` / `Reference_Analysis.md`(区間3で作成)は**区間5で削除**され、内容は本書と上記へ統合された。
  - `UI_Development_Log.md`(区間1で新設した経緯・ナレッジの記録)は **2026-09-03 に本書へ統合して削除**(ナレッジ=各区間の C 節、恒常情報=付録)。

---

# 🎨1. uibase → uibase2 — UI ブラッシュアップ

第1弾ブラッシュアップ + 別リポジトリ `Work-Project-MauiUI` からの UISample 取り込み + 第2弾ブラッシュアップ + 画面改名 + スタイル切り出しをまとめた区間。**UI 系はこの区間がほぼすべて**。

## 🖼️A. 画面単位の変更

### 📋A-1. メニュー画面

| 画面 | 変更内容 |
|---|---|
| UIMenu | Timeline→Graph2 改名に伴う並び替え。UIProfile2 / UICockpit 廃止で **11行 → 10行×3列=30ボタン・空セルなし** に縮小 |
| SampleMenu | 空セル 7 箇所の `IsVisible` 除去 → 「可視の無効ボタン」に統一 |
| SampleCvNetMenu | 同上(4 箇所) |
| ViewMenu | 空セルを可視の無効タイル(灰色)に変更 |
| Main/Menu・BasicMenu・DeviceMenu・NavigationMenu・NetworkMenu | 空セルの扱い以外の差異(番号プレフィックス / 絵文字 / 列数 / アイコン有無)は**意図的に維持**(統一しない方針) |

### 🎨A-2. UI モジュール — 新規追加(20 画面)

いずれも View / View.xaml.cs / ViewModel の 3 ファイル一式を新設し、`ViewId` とメニューへ登録。

| 画面 | 種別 | 内容 |
|---|---|---|
| UIShop | EC | 商品カード(Popular 横スクロール + All Items 2列グリッド)。SfEffectsView Ripple、FadeUp 段差入場、**検索 Entry を実結線**(タイトル部分一致で絞り込み) |
| UIItem | EC | 商品詳細。数量ステッパー(1..99)、サイズタグ 30/50/100ml のタップ切替、画像 Pop 入場 |
| UICart | EC | カート。`UICartItem` を ObservableObject 化しステッパー結線 + 合計再計算(件数 / 小計 / 割引10% / 合計連動)、Total は CountUp + Bounce、Checkout ダイアログ結線 |
| UIStream | 配信 | 「+ My List」トグル結線、TopBar/Hero の FadeUp + レーティング Pop、ポスター Ripple |
| UIStreamDetail | 配信 | タブ切替(Trailers ⇔ More Like This)で FadeIn、Favorite / Download トグル、Trailer 行・Related タイル Ripple |
| UIChat | 通信 | Send / Receive / System のバブル + リアクションピル(4列2行折返し)、スタンプトレイ |
| UICalendar | 日付 | `CalendarView2` を用いた月表示。日付タップ / イベントタップでトースト表示 |
| UITimeline | 日付 | 旧 `UITimelineSample`。進行中イベントのドット Pulse + ヘッダ/リストの FadeUp 入場 |
| UIGraph | 可視化 | Git グラフ表現の可視化画面(`Resources/Raw/Graph/repository.json` を読み込み) |
| UIGraph2 | 可視化 | 旧 `UITimeline`(Git グラフ別表現)を改名。リスト全体の FadeUp 入場 |
| UIFeel(2026-09-12 廃止。hex 配置は `Controls/HoneycombLayout` として ViewLayout へ) | キャラ | 7つの hex セルのタップ選択(自系統色の枠3px + チェックバッジ移動)、中央→外周の Pop 開花入場 |
| UIMonster(旧 UIPet) | キャラ | ステータスバー4本を ProgressBar 化 + `ProgressTo` による伸長アニメ、数値 CountUp、Heart ボタン結線(HP+5 / 上限400)、Add to Party トグル |
| UIKitOnboard | Kit | Skip / Get Started 結線(→ Kit Dashboard)、ページスワイプでテキスト FadeIn |
| UIKitSetting | Kit | Switch を ObservableProperty 化して実バインド、グループ FadeUp 段差 |
| UIKitDash | Kit | メトリクス4カード Pop 段差、Heart Rate カード FadeUp、ベル未読ドット Pulse、リンク2カード Ripple |
| UIKitNotify | Kit | 未読/既読の視覚差(左アクセントバー + 青背景 + 太字 + 未読ドット)、行タップで既読化 |
| UIKitTracking | Kit | ステップ3状態(完了=緑チェック / 進行中=青ドット Pulse / 未来=灰)、完了区間の縦線を緑に塗り分け |
| UIFlight | HUD | 旧 `UIFlightHud`。SkiaSharp 自走描画(`FlightHudScene`) |
| UITactical | HUD | 旧 `UIMechHud`。SkiaSharp 自走描画(`MechHudScene`) |
| UIEnergy | HUD | **UICockpit を廃止して差し替え**(`UICockpitView.xaml` をベースに再構成)。`EnergyFlowScene` |
| UITelemetry | HUD | SkiaSharp 自走描画(`TelemetryScene`) |

### 🎨A-3. UI モジュール — 既存画面の改修(18 画面)

| 画面 | 変更内容 |
|---|---|
| UILogin | 背景 / レイアウト / メッセージ / 入力 / Keep login / パスワード表示トグル / Forget password の各セクションを整備。左アイコンを Size 30→28 |
| UIMoney | Background / Header / Rank / Menu / Detail / Bottom select + バッジのカード構成に整備、エントランス演出を全面適用。メニューアイコンを `MoneyIcon` マークアップ拡張(既定28)へ集約 |
| UISuper | 検索バー + ポイント残高 / バナーカルーセル / ミニアプリ / クーポンの縦構成。FontSize を許可値の大きい側へ拡大 |
| UIPos | POS 明細・小計まわりを整備(増減ボタンの参照実装) |
| UIProfile | **旧 UIProfile2 のカード型デザインを統合**(UIProfile2 は View/VM/ViewId/メニューとも削除)。パララックスヘッダー、SNSアクション3トグル(フォロー/いいね/お気に入り)、写真6枚グリッド、色付きタグ、区切り線統計、Bio |
| UICharacter | Character / Class / Detail のカード構成に整備 |
| UIDock | ドックのアイコン配置 / 演出を微調整 |
| UIMail | Header / Messages / Empty state / Indicator / Floating Button / Tab の構成に整備 |
| UISocial | Shared / 背景 / Icon / Episode / Counter / Alert の構成、作戦中止の Back 結線(SfEffectsView) |
| UISchedule | 7日チップ + タイムテーブルの構成、FontSize 17→18 |
| UITreeMap | 撮影瞬間のシャッターフラッシュ演出 |
| UIRadar | 飾りステータス追加(Random 使用のため CA5394 をファイル先頭 pragma で抑止) |
| UIGauge | 6種サンプル(Pressure / Humidity / Temperature / Wind / Speed / Tachometer)を整備 |
| UIMeter | 四隅ビネット追加 |
| UIMixer | Knob / dB scale / Slider / Channel / Equalizer / Frequency を整備(クラス名 **Mixier → Mixer** にスペル統一) |
| UILoad | 微調整 |
| UITimeline(旧 Timeline) | 内容を旧 TimelineSample に差し替え(旧内容は UIGraph2 へ) |
| UICockpit | **廃止**(View.xaml.cs / ViewModel 削除、View.xaml は UIEnergyView.xaml へ転用。`CockpitControls` も削除) |

### 🖼️A-4. View モジュール

| 画面 | 変更内容 |
|---|---|
| ViewEffect | **新規**。演出資産のカタログ画面。常時アニメ(Wave×3 時間差 + Pulse + ON/OFF Switch)と変化フィードバック(Fire で Bounce + Highlight + Flash 同時発火) |
| ViewAnimation | 2 InfoCard 化(Click 系4ボタン=**Shake 新設** + Tap 系タイル)、空セル解消 |
| ViewBorder | コメントアウトされていた `StrokeLineJoin` / `StrokeLineCap` の Picker を復活 |
| ViewCarousel | 中央カード強調(非中央=Scale 0.92 + Opacity 0.55)、白カード影、画像角丸クリップ、背景グレー |
| ViewCollection | ▼▲ を Expand_less/more アイコン化、スワイプ項目をアイコン+文字の縦積みに |
| ViewDrawing | 色6チップ + 線幅 Slider のパレット、プレビュー額装 + 未描画時の空状態 |
| ViewEasing | 4×3 均等グリッドへ再設計、背景に Easing 曲線(`EasingCurveView` 新設)、12セル目=実行状態セル(Run 中 Pulse + Function4 無効化) |
| ViewGraphics | Add line/circle/rect(乱数色)/ Clear ボタン + 図形数チップ(`ShapeDrawing` に Circle 追加) |
| ViewLottie | プレイヤー UI(シーク Slider + mm:ss.f 等幅表示 + 円形 Play/Pause・Reset、白カード額装) |
| ViewRefresh | 初回ロード(1秒)中に EmptyView のスケルトン4行、ヘッダに Newspaper アイコン + 件数ピル、行カード白 + 枠線化 |
| ViewSvg | 3 SVG 切替(dotnet_bot / vite / react)チップボタン + ファイル名表示 + 額装 |

### 📱A-5. Device モジュール(17 画面)

| 画面 | 変更内容 |
|---|---|
| DeviceInfo | 3 InfoCard 化 + FadeUp 段差入場 |
| DeviceSensor | Vector3 / Quaternion を VM で軸分解(RGB=XYZ 軸バッジ + 中央ティック付きバー)。Compass カード=実描画ダイヤル(30°目盛 + N/E/S/W + 赤針)、Level カード=気泡水準器を新設(`SensorDrawing`) |
| DeviceStatus | 開発用バッテリーアイコン列挙を削除 → 動的バッテリーアイコン + 残量ゲージ(20%以下=赤 / 充電中=緑)+ Network 状態チップ(緑/琥珀/赤) |
| DeviceAudio | プレイヤーカード UI(250ms ポーリング、シーク Slider + DragCompleted、円形トランスポート3ボタン、再生中アイコン Pulse、音量%表示) |
| DeviceQrDisplay | Entry ライブ編集 → QR 再生成 + 額装 + 空状態表示 |
| DeviceQrScan | 結果空時「Scan a code...」プレースホルダ + 検出時にカメラ面へ白 Flash |
| DeviceLocation | 画面上部に 240px の地図(IsShowingUser)を常設、初期=東京駅→測位毎に現在地へ移動(`MapController.MoveTo`)。測位待ち空状態 + Position/Motion カード + タイムスタンプ Highlight |
| DeviceOcr | カメラにガイド枠 + **結果は画面内パネル表示**(ダイアログ廃止)+ 実行中オーバーレイ |
| DeviceBluetooth | 状態を**インライン状態チップ**(Idle/Connecting/Printing/Completed/Failed)で表示(ダイアログ廃止、State enum + IsBusy) |
| DeviceBleScan | 未検出時 EmptyView(Bluetooth_searching Pulse + 「Scanning...」)+ 温湿度/CO2 値の更新時 Bounce |
| DeviceBleHost | Podcasts アイコン円(Advertising 中 Pulse)+ 状態チップの中央カード化 |
| DeviceNfc | 履歴 EmptyView「Suica をかざしてください」(Contactless Pulse・ダークテーマ)+ 残高 CountUp(600ms) |
| DeviceMisc | 15 アクションを 4 InfoCard(Screen / Feedback / Light / Speech)に分類、Material アイコン付き ActionButton 化、音声認識中は赤マイク Pulse |
| DeviceCommunication | タップ行カード×3(色分けアイコン円 + Ripple) |
| DeviceActivity | 歩数(96pt)を CountUp 化 |
| DeviceWiFi / DeviceBiometric | 「Not implemented」空状態パネル(暫定) |

### 🧭A-6. Navigation モジュール(14 画面)

| 画面 | 変更内容 |
|---|---|
| Wizard Input1 / Input2 | 入力フィールド定型(キャプション + Border + フォーカスで青枠)、`StepIndicator`(1/3・2/3)、ステップ説明カード + ヒント。**Next(▶️)は入力があるまで無効**(`WizardContext` を ObservableObject 化)。Input2 の Placeholder 誤記(Data1→Data2)修正 |
| Wizard Result | 入力サマリカード(Data1/Data2)+ 完了カード、Function4 表示を ✔️ に |
| Stack 1 / 2 / 3 | 本文が空だった3画面を中央カード化(レベル別アクセント 1=Blue / 2=Teal / 3=DeepPurple、Looks_one〜3 アイコン円 + StepIndicator + キー操作ヒント + Pop 入場) |
| Shared Input / Main1 / Main2 | SharedInput に「Return to Shared1/2」チップ(遷移元により Indigo/Teal)+ 入力フィールド定型。Main1/Main2 は系統色の数字円 + チップ + No 大型表示 + FadeUp |
| Edit List / Detail | List=一覧行を白カード化(Id=等幅 #n ピル、Edit/Delete に PressEffect)+ 空時 EmptyView。Detail=入力フィールド定型 + インラインエラー表示 |
| Navigate Cancel | パターン説明カード常設(Amber Help + Yes/No の挙動説明) |
| Navigate Initialize | 初期化3秒間スケルトン + Hourglass Pulse → 完了で緑 Task_alt カードが FadeIn |
| InputNumber(モーダル) | 全キーにローカル派生スタイル(共有 Input*Button + PressEffect)、✔/❌ を Material の Check/Close グリフに変更 |

### 🧩A-7. Basic / Main / Data / Network モジュール

| 画面 | 変更内容 |
|---|---|
| BasicValidation | **ルートの `ContentPage.Behaviors` 誤記を `ContentView.Behaviors` に修正**(バリデーションが実際に動くようになった)。エラーで枠が赤 Highlight + エラーラベル FadeIn、Error/Clear/Focus をアイコン付きボタン化 |
| BasicBehavior | 2 Entry を Border + フォーカス枠(青/緑)で色分け + イベント値の Highlight カード |
| BasicConverter | タイポ修正(Upper/**Lowwer→Lower**)+ 未使用 `ToChecked` 削除、CheckBox + ラベル横並び化、変換結果に Highlight |
| BasicStyle | 全 Action/Information ボタンを ActionCommand で結線し「Last action」カードに押したボタン名を Highlight。SelectButton は隣接セグメント配置 |
| BasicFont | ScrollView 化(実機で全フォント見える)+ フォント毎 InfoCard 10枚 + JetBrainsMono / NotoSerifJP 見本追加 |
| BasicTypography | ScrollView 化 + 4 セクションカード |
| BasicLocale | 現在カルチャカード + リソースキー2件の一覧 |
| BasicDialog | 11 ボタンを 4 カテゴリの InfoCard に整理 |
| Setting(Main) | スキャンガイドピル + 設定値パネル、値変更時 Highlight、未設定時「(not set)」 |
| Data | CRUD / Bulk の 2 カード化 + BulkDataCount に CountUp |
| NetworkRealtime | TODO 解消=鋸波 → **ランダムウォーク擬似データ**(CPU ゆらぎ+スパイク / Memory ドリフト / Network バースト)、StatControl 3枚(青/緑/橙)、500ms 更新 |
| NetworkMenu | VM の未使用 `RealtimeCommand`(デッドコード)を削除 |

### 🧪A-8. Sample モジュール

多くは第1弾で整備済み。この区間の主な変更は FontSize 拡大 / アイコンのマークアップ拡張化 / 軽微な整形。

| 画面 | 変更内容 |
|---|---|
| SampleChart | チャート表示整備 + FontSize 13→14 |
| SampleChat | チャット UI 微調整(共通コントロール `ChatView` 側で整備) |
| SampleMedia / SamplePdf / SampleMap1 / SampleMap2 / SampleMarkdown / SampleWebApp / SampleWebBasic | 表示・操作を整備(Map 系は `MapBind` / `MapController` 経由へ) |
| SampleCvLocal / SampleCvNet(Face/Object/Ocr/People/Tag) | CV 系の表示整備 |

### 🔀A-9. 画面横断の変更

| 項目 | 内容 |
|---|---|
| FontSize 統一 | XAML/C# の FontSize を許可値の大きい側へ統一(13→14 / 15→16 / 17→18 ほか)。Skia 自走描画(Scene)は 8.5→9 / 9.5→10 / 7.5→8 / 7→8 / 13→14 / 15→16 / 17→18、Telemetry の GEAR 桁のみ 56→72。before/after 比較画像を `Document/FontSize_{FlightHud,MechHud,Telemetry,Energy}_{Before,After}.png` に追加 |
| アイコン共通化 | 生 Unicode・絵文字を `markup:Material` / `Fluent` / `MenuIcon` / `MoneyIcon` へ置換(72箇所)。メニューアイコン 24→28。動的色の 11 箇所のみ `FontImageSource` 残置 |
| スタイル切り出し | 36 画面の直書き視覚属性を画面ローカル `ResourceDictionary` の Style へ切り出し(**値は不変更=見た目は同一**) |
| 画面改名 | UITimelineSample→**UITimeline** / UIFlightHud→**UIFlight** / UIMechHud→**UITactical** / Timeline→**Graph2**、UICockpit→廃止 |

A-9 の補足:

- **FontSize の規則**: 許可値(付録A参照)への統一。Scene(Skia 自走描画)は情報量の観点から Excel 標準値(6/8/9/10 等)の小フォントも許可し、**Android では WPF 版より小さく見えるため大きい側へ丸める**
- **Profile 統合の採用基準**: 基本要素(写真グリッド / 色付きタグ / 統計 / Bio)は旧 Profile2 を採用し、旧 Profile 優位の**パララックスヘッダーのみ移植**(`Scroll.ParallaxTarget`)。SNS アクション 3 トグル(フォロー / いいね / お気に入り)は「OFF=白地+色アイコン / ON=色地+白アイコン」で統一
- **スタイル切り出しの規約**(36 画面で統一):
  - 定型スタイルキー: `RootGrid` / `RootScroll` / `PageStack` / `CenterCardBorder` / `CardStack` / `IconCircleBorder(+Label)` / `CenterTitleLabel` / `CardDivider` / `CenterHintLabel` / `Empty*` 系。色違いの繰り返しは BasedOn 派生
  - 定義順: コンバータ → ルート要素(`Root*`) → `PageStack` → 本文の出現順(コンテナ→中身、BasedOn 派生は基底の直後)
  - スタイル化せず残す直指定 = 動作パラメータ(`FocusedStroke` / `HighlightColor` / `ProgressColor` / `AccentColor` / DataTrigger の状態色 / セグメント重ね `Margin=-1` / 行 Grid の `ColumnSpacing`)と `FontImageSource`(Style 不可の既知制約)のみ
  - 対象外 = 直書きが僅少な 10 画面(Navigation 4 / Device 4 / Basic 2)と、バリエーション自体がコンテンツの画面(BasicFont / ViewEffect / DeviceSensor / UIFeel / UIMeter / メニュー系)

## 🧱B. 画面以外の変更

### 🧰B-1. 新規コントロール(`Controls/`)

| ファイル | 用途 |
|---|---|
| `InfoCard.xaml(.cs)` | 見出しアイコン + タイトル + 区切り線付きの角丸カード(ControlTemplate 方式) |
| `StatusChip.xaml(.cs)` | アイコン + テキストのピル型チップ(DataTrigger で状態色分け) |
| `StepIndicator.cs` | ●●● のステップ表示(現在=ピル型強調 + 「n / N」) |
| `EasingCurveView.cs` | Easing 曲線を背景描画する GraphicsView |
| `Gauge.cs` | 汎用ゲージ(SKCanvasView) |
| `CalendarView.xaml(.cs)` / `CalendarView2.xaml(.cs)` | 月カレンダー(MAUI 部品版 / Skia 自前描画版) |
| `DayTimetableView.cs` | 日次タイムテーブル描画 |
| `GraphRowSurface.cs` | Git グラフの行描画 |
| `CameraOverlayView.cs` | カメラのガイド枠オーバーレイ |
| `AiChatTemplateSelector.cs` / `DeckButtonTemplateSelector.cs` | テンプレートセレクタ |
| `MixerEqualizer.cs` / `MixerKnob.cs` / `MixerSlider.cs` | 旧 `Mixier*` から**スペル修正のリネーム** |
| `TimelineCell.cs` | **削除**(Graph 系の再構成に伴う) |

### 🔧B-2. Behaviors / Converters / Markup

| ファイル | 内容 |
|---|---|
| `Behaviors/AnimationOption.cs` | **新規**。入場/常時/フィードバック演出の添付プロパティ群(FadeUp / Pop / EnterDelay / EnterTrigger / Pulse / Wave / Bounce / FadeIn / Flash / Highlight / Shake / **ProgressTo**) |
| `Behaviors/MapBind.cs` + `Messaging/MapController.cs` | **新規**。Map をコントローラパターンで操作(`MoveTo`) |
| `Behaviors/MediaBind.cs` + `Messaging/MediaController.cs` | **新規**。MediaElement をコントローラパターンで操作 |
| `Behaviors/SliderOption.cs` | **新規**。`DragCompletedCommand` |
| `Behaviors/ButtonOption.cs` / `LabelOption.cs` / `Focus.cs` / `Scroll.cs` | 拡張(PressEffect / CountUp / フォーカス枠 / OverScroll 抑止) |
| `Converters/*` | 9 件新規(`AlternateRowBackground` / `BadgeCount` / `CenteredRatio` / `ChatTime` / `CollectionNotEmpty` / `CompassDirection` / `DecibelToColor` / `DurationToSeconds` / `RefKindBrush`) |
| `Markup/FontIconExtensions.cs` | **新規**。アイコン用マークアップ拡張(基底 `Material` / `Fluent` + 用途別 `MenuIcon` / `MoneyIcon`) |

### 🖌️B-3. 描画基盤(`Graphics/`)の 2 名前空間分離

用途で分離し、対称形の命名({概念}Object / Control / Xxx{概念})に統一。

- **`Graphics.Drawing`**(IDrawable・データ駆動): `DrawingObject`(旧 `GraphicsObject`)/ `DrawingControl`(旧 `GraphicsControl`)/ `ActivityDrawing` / `BarcodeDrawing` / `DetectDrawing` / `LoadDrawing` / `ShapeDrawing`(旧 `*Graphics` からリネーム)+ 新規 `ChartDrawing` / `ColorTreeMapDrawing` / `SensorDrawing`
- **`Graphics.Scene`**(SKCanvas・自走アニメ): `SceneObject` / `SceneControl`(旧 `AnimatedSkiaView` 後継)+ `EnergyFlowScene` / `FlightHudScene` / `MechHudScene` / `TelemetryScene`

### 🗄️B-4. モデル・サービス

| ファイル | 内容 |
|---|---|
| `Models/Sample/Calendar/*` | **新規 11 ファイル**(`MonthViewBuilder` / `ScheduleEvent` / `TimetableDay` / `Stamp` / `DayKind` ほか) |
| `Models/Sample/Chat/*` | **新規 4 ファイル**(`ChatMessage` / `AiChatMessage` / `MessageReaction` / `MessageType`。旧 `Models/Sample/ChatMessage.cs` は削除) |
| `Models/Sample/Graph/*` | **新規 4 ファイル**(`GraphBuilder` / `GraphLayout` / `GraphModels` / `TimelineRow`。旧 `Models/Sample/TimelineRow.cs` は削除) |
| `Models/Sample/{MapSpot,RadarTarget,SocialNotificationInfo,SocialUnit,SuperItems}.cs` | 新規 |
| `Models/Sample/PhotoItem.cs` | ObservableObject 化(+ `IsCurrent`) |
| `Services/ScheduleService.cs` / `HolidayService.cs` | **新規**。スケジュール / 祝日のサンプルデータ供給 |

### 🏗️B-5. リソース・ビルド設定

| 項目 | 内容 |
|---|---|
| フォント | `JetBrainsMono-Regular.ttf` 追加。`MauiProgram` に **JetBrainsMono / NotoSerifJP** を登録 |
| 画像 | `ic_camera.svg` / `ic_send.svg` / `ic_sticker.svg` / `stamp01〜08.png` を追加 |
| データ | `Resources/Raw/Graph/repository.json` を追加(UIGraph / UIGraph2 用) |
| `Styles.xaml` | リソース・派生スタイルを追加(**共有スタイルの既存定義は不変更**=ポリシー遵守) |
| `Settings.XamlStyler` | 属性並び順に `HorizontalOptions, VerticalOptions` を追加 |
| `MauiProgram.cs` | BLE ホスティングの登録方法を変更(`AddBleHostedCharacteristic<UserCharacteristic>` → `AddSingleton<UserCharacteristic>`) |
| パッケージ更新 | SkiaSharp 3.119.2→**4.148.0**、Smart.Navigation 系 2.20.0→**3.1.0**、Shiny 系 4.0.1→**5.1.1**、CommunityToolkit.Maui 14.1.1→14.2.0、MAUI 10.0.60→10.0.80、Svg.Skia 4.9.0→5.1.1 ほか多数 |
| `NoWarn` | `NU1903` を **TODO 付きで暫定追加**(次区間で解消) |
| ドキュメント | `Document/UI_Development_Log.md` / `Document/UI_Verification_Checklist.md` を新設 |

## 💡C. この区間のナレッジ

- **正確な API 名**(発明注意): CountUp=`LabelOption.CountUpValue/CountUpFormat/CountUpDuration`、フォーカス枠=`Focus.FocusedStroke/FocusedThickness`(親 Border 必須)、Make* コマンドヘルパは canExecute 自動再評価(`.Observe()` は存在しない)、`s:NullToText` は Null/NonNull 置換のみ(値パススルー不可 → 空状態は 2 ラベル + `NullToBoolConverter`)
- `Vector3` / `Quaternion` の X/Y/Z は**フィールド**のためバインド不可 → VM で NotifyAlso 連動の計算プロパティに分解
- MAUI `Button.CornerRadius` は**全周一括**(左右個別の角丸は不可)
- `PathF` は IDisposable(CA2000 → `using var`)。CA5394(Random)はファイル先頭 `#pragma` の前例(UIRadarViewModel)。算術式は括弧明示(IDE0048/SA1407)
- **CollectionView 行の入場アニメは不成立**(リサイクルで Loaded が再発火する)。BindableLayout の行はリサイクルされないため可
- SkiaSharp: `SKPath` 直接構築は CS0618 → `SKPathBuilder`+`Detach()`。`DrawText` は SKTextAlign 付き / `DrawBitmap` は SKSamplingOptions 付きオーバーロードを使用。日本語は `SocialFonts.NotoSerifJP`(`SKTypeface.Default` は豆腐化)、絵文字は `SKFontManager.MatchCharacter`+異体字セレクタ U+FE0F 除去
- **FontImageSource は Style 不可・マークアップ拡張はバインド不可**(検証済)→ 動的色の 11 箇所だけ FontImageSource 直書きが残る理由
- ControlTemplate+ContentPresenter+`TemplateBinding` に Converter 指定可(InfoCard / StatusChip で実証)。CarouselView の中央強調は項目モデルの IsCurrent+`CurrentItemChangedCommand`+DataTrigger で code-behind 不要
- 画面定型: GrayLighten5 背景+Padding 12+InfoCard+FadeUp 段差(0/80/160…)、単一機能画面は中央カード+円形アイコン(96)+説明+ヒント
- ビルド警告ベースライン=8 件(CS8785×1=Smart 生成器、XA4301×7=ネイティブ lib 重複)。Visual Studio 併用時は obj のファイルロックで CLI ビルドが失敗することがある(リトライか該当 VS を閉じる)

---

# 🔍2. uibase2 → uibase3 — コードレビュー対応

コードレビュー(2026-08-05)に基づく Phase 0〜9 の修正コミット `2f533e0f` と、その後の追補・差し戻しからなる区間。**UI の見た目を変える変更はほぼ無く、不具合修正・堅牢化・設計是正が中心**。

## 🖼️A. 画面単位の変更

### 🐛A-1. 不具合の修正

| 画面 | 変更内容 |
|---|---|
| Edit List | **編集/削除ボタンでクラッシュしていた問題を修正**。`Button` の要素レベル `x:DataType` により `CommandParameter="{Binding}"` が `TypedBinding<EditListViewModel, EditListViewModel>` としてコンパイルされ、実 BindingContext(`WorkEntity`)と不一致で null になっていた(uibase2 以前から存在した不具合) |
| Device Status | **クラッシュの修正**。Manifest から `BATTERY_STATS` を削除したことで `IBattery` が `PermissionException` を投げていた(`Permissions.EnsureDeclared` は付与ではなく**宣言の有無**を見る)。宣言を復活 |
| Device QrScan / Setting(Main) | **カメラ権限の Check→Request を追加**。起動時の一括要求を廃止した際の漏れで、新規インストール時にスキャンが動作しなかった |
| UIDock | CPU/Memory ボタンの `Parameter` が両方 `"VolumeDown"` だった誤りを `"Cpu"` / `"Memory"` に修正 |
| UIGraph / UIGraph2 | JSON デシリアライズ結果の `null!` を `?? RepositoryData.Empty` に統一 |
| Network(サーバ時刻表示) | `.ToLocalTime()` を追加。DateTime の UTC 統一の副作用で API 経路の表示が 9 時間ずれていた |
| Data | 保存を `DateTime.Now` → `DateTime.UtcNow`、表示側で `ToLocalTime()` |

### ♻️A-2. ライフサイクル・リソース解放

| 画面 | 変更内容 |
|---|---|
| UIMeter | **二重起動ガード**(`if (loopTask is not null) return;`)+ `await loopTask` の try/finally 化 |
| Device Audio | 購読前に `polling?.Dispose()`(二重購読の防止) |
| Device Bluetooth | `finally` で状態復帰。想定外の例外で `State=Printing` のまま `IsBusy` が固着し印刷ボタンが永久に無効化されていた |
| Device BleScan | 外側タイマ購読にも `onError` を追加 |
| Device Nfc | **Rx シーケンス死亡経路を解消**。`ConvertResult` を `ParseTag` に分離して解析例外(`ArgumentException` / `OverflowException` / `IndexOutOfRangeException`)を null 化 + WARN ログ、`Subscribe` に `onError` 追加 |
| UITreeMap / ViewDrawing | `Dispose` で `ImageHelper.ReplaceBitmap(Image, null)`、代入も `ReplaceBitmap` 経由に統一(SKBitmap の所有権を一元化) |
| UIMail | `SKBitmap` を `Disposables.Add` して画面破棄時に解放(アンマネージドメモリのリーク) |
| SampleCvLocal | **再入防止ガード**。`await DetectAsync(bitmap)` 中に再実行されると `ReplaceBitmap` が推論中のビットマップを破棄していた(use-after-dispose) |

### 🔐A-3. 権限フローの画面側への移動

起動時の一括権限要求を廃止し、各画面の `OnNavigatedToAsync` で Check→Request するよう変更。

| 画面 | 要求する権限 |
|---|---|
| Device Camera / Device QrScan / Device Ocr / UITreeMap / Setting(Main) | カメラ |
| Device Location | 位置情報(`LocationAlways` → **`LocationWhenInUse`** に緩和) |
| Device Activity | ActivityRecognition(未許可時はダイアログ表示) |
| UILoad | マイク |

### 🚫A-4. 非搭載ハードウェアへの対応

| 画面 | 変更内容 |
|---|---|
| Device Sensor | 各センサーの `IsSupported` を見て開始、`IsMonitoring` を見て停止(非搭載センサーの `FeatureNotSupportedException` を回避) |
| Device Bluetooth / Device Nfc | アダプタ未搭載時に null を返す `IsSupported` を追加(下記 B-2 参照) |

### 🧩A-5. DI・共通化

| 画面 | 変更内容 |
|---|---|
| UICalendar / UISchedule | `ScheduleService` / `HolidayService` を VM 内 `new` から **DI 注入**へ変更 |
| Device Misc | `speech.RecognizeCancel()` → `await speech.RecognizeCancelAsync()` |
| SampleCvNet(Face/Object/Ocr/People/Tag) | Phase 7-3 で基底クラス `SampleCvNetViewModelBase` に共通化 → **同区間内で差し戻し**、5画面を独立 VM に戻して基底クラスを削除(サンプルは1画面で完結して読める方が良いという判断)。ビットマップ所有権とカメラ権限チェックは維持 |

### 🏷️A-6. XAML の是正(見た目の変更なし)

| 画面 | 変更内容 |
|---|---|
| UICharacter / UIChat / UIGraph2 ほか | `RelativeSource` バインドに `x:DataType` を付与(意図の記述。生成コードは従来どおり) |
| UIProfile | 写真一覧を `CollectionView` → **`FlexLayout` + `BindableLayout`** に変更(件数固定・縦 ScrollView 内で仮想化が効かないため)。`Basis="33.33%"` で 3列×2行 |
| UIMoney / UIMail / UIPos / UILoad / UIShop / UIItem / UIGraph / DeviceBleScan / ViewRefresh ほか | **Style キーのリネーム**(共有スタイルを隠蔽していたキーの解消)。衝突した1〜2キーだけでなく、同じ画面の同じ役割グループ全体を同じプレフィックス規則に統一(計 40 キー・98 箇所) |
| ViewCollection / DeviceBleScan | 軽微な整形 |

## 🧱B. 画面以外の変更

### 🚀B-1. 起動シーケンス・アプリ基盤

| ファイル | 内容 |
|---|---|
| `ApplicationInitializer.cs` | `async void Initialize` を廃止し **`StartupTask` を公開**。DB 初期化の失敗(`IOException` / `UnauthorizedAccessException` / `SqliteException`)を捕捉して `InitializeError` に保持 |
| `App.xaml.cs` | 起動時の一括権限要求を削除。`StartupTask` の完了を待ってから遷移し、**DB 初期化失敗時は原因を提示して `Quit()`**(従来は無言でクラッシュ→次回起動でも同じ所で落ちるループになっていた) |
| `MauiProgram.cs` | `ApplicationInitializer` を単体登録し `IMauiInitializeService` はファクトリ経由に。`ScheduleService` / `HolidayService` / `INetworkInteraction` を DI 登録 |
| `MainPage.xaml(.cs)` / `MainPageViewModel.cs` | Function ボタンの状態を **`FunctionState` に集約**(Title / HeaderVisible / FunctionVisible も `NotificationValue<T>` 化)。`HeaderVisible` の既定を `true`→**`false`**(初回ナビゲーションまで空タイトルのヘッダーが出ていた)。Back の fire-and-forget に例外ログを追加 |
| `Modules/ValidationHelper.cs` | **新規**。`AppViewModelBase` / `AppDialogViewModelBase` に重複していた検証処理を集約(バッファは `[ThreadStatic]`) |
| `Shell/IShellControl.cs` / `ShellProperty.cs` / `DiagnosticPanel.xaml.cs` | Shell 状態を `FunctionState` に統合。DiagnosticPanel は `HandlerChanged` → **`Loaded`/`Unloaded`** に変更し、**世代番号でタイマーの多重起動を防止**(`StopMonitor` はフラグを倒すだけで次 tick までタイマーが生存するため) |

### 📱B-2. Components(デバイス層)の堅牢化

| ファイル | 内容 |
|---|---|
| `BluetoothSerial.android.cs` | アダプタ未搭載を許容(`IsSupported`)。`RegisterReceiver` を `ContextCompat.RegisterReceiver(..., ReceiverNotExported)` に変更、**Discovery 30秒 / Bond 60秒のタイムアウト**を追加、解除処理を `finally` に集約、`socket.Dispose()` を追加 |
| `Nfc.android.cs` / `Nfc.cs` | アダプタ解決を遅延化 + `IsSupported`。**タグはイベントハンドラ内でのみ有効**という契約にし、検出ごとに `Close`/`Dispose`(接続リーク防止)。`TagLostException` を含む `Java.IO.IOException` を握って購読側へ伝播させない。`Dispose` で ReaderMode を解除 |
| `NoiseMonitor.android.cs` / `NoiseMonitor.cs` | 停止を **`StopAsync`** 化(stop/start レースの解消) |
| `OcrReader.*` | ログ出力とキャンセル対応 |
| `ActivityRecognizer.*` | コンポーネント内での権限要求を削除(呼び出し側に移動)。イベントが**UI スレッド以外から発火する**契約を XML コメントで明記 |
| `NfcExtensions.cs` | `SubArray` の負サイズをガード |

### 🌐B-3. 通信・データアクセス

| ファイル | 内容 |
|---|---|
| `Usecase/NetworkInteraction.cs` | **新規**。`INetworkInteraction` / `DialogNetworkInteraction` で `NetworkOperator` を `IDialog` から分離(UI なしテスト・再利用が可能に) |
| `Usecase/NetworkOperator.cs` | エラー分類(`NetworkErrorKind`)による統一とリトライ上限の導入(219行の全面書き換え) |
| `Services/HttpService.cs` | 全メソッドに `CancellationToken` を追加。`IHttpClientFactory` のクライアントを `using` しない(ハンドラはプール管理)。転送系を **`ApiNames.Transfer`** クライアントへ分離、進捗コールバックを共通化し **Content-Length 不明時は通知しない** |
| `Services/AppHostBuilderExtensions.cs` | 転送用 HttpClient を追加(**Timeout 10分**。無限にすると中断手段が無くなるため上限を設ける) |
| `Services/ApiContext.cs` | `BaseAddress` / `Token` を **volatile** 化(UI スレッドの書き込みを通信スレッドが読むため) |
| `Services/ParameterBuilder.cs` | クエリのキー・値を **`Uri.EscapeDataString`** でエンコード |
| `Services/DataService.cs` | `busy_timeout` を **接続文字列 `Default Timeout=3`** に変更(PRAGMA は接続単位で他接続に効かない)。**WAL 有効化**と `-wal`/`-shm` の削除漏れ修正。Work の採番を `INSERT ... (SELECT COALESCE(MAX(Id),0)+1 ...)` の単文にして **SELECT MAX→INSERT の非アトミック競合を解消**。`Using`→`UsingAsync` の誤用を修正 |
| `Usecase/CognitiveUsecase.cs` | 初期化を `SemaphoreSlim` で保護(並行初期化による `InferenceSession` リーク防止)、`ArrayPool` の返却を `finally` に移動(出力読み取り完了前に返していた)、リサイズ後の `SKBitmap` を `using` |
| `Helpers/ReactiveSignalR.cs` / `Helpers/Data/*` / `Helpers/Json/*` | async void の除去、DateTime の UTC 正規化(`DateTimeTypeHandler` / `DateTimeConverter`) |
| `State/Settings.cs` | AI サービスキーを **Preferences → `SecureStorage`** へ移行(旧値の自動移行付き)。空文字は `Remove` として扱う |

### 🔐B-4. 権限・Manifest

| 項目 | 内容 |
|---|---|
| `Permissions.cs` | `CheckStatusAsync` → 未許可なら `RequestAsync` の共通実装に統一。`ActivityRecognition` 権限クラスを追加。位置は `LocationAlways` → `LocationWhenInUse` |
| `AndroidManifest.xml` | 不要権限を削除(`CHANGE_WIFI_STATE` / `READ/WRITE_EXTERNAL_STORAGE` / `FLASHLIGHT` / `ACCESS_BACKGROUND_LOCATION` / `USE_BIOMETRIC` / `USE_FINGERPRINT`)。`BATTERY_STATS` は**宣言が必須のため復活**。Android 11 向けレガシー Bluetooth(`maxSdkVersion="30"`)を追加、`BLUETOOTH_SCAN` に `neverForLocation` を付与。`uses-feature`(nfc / camera / bluetooth / microphone、いずれも `required="false"`)を追加 |

### 🏗️B-5. ビルド・アナライザ・署名

| 項目 | 内容 |
|---|---|
| `NoWarn` | `NU1608;NU1903`(TODO 付き暫定)→ **`XA4301` のみ**(理由コメント付き)。NU1903 は `SQLitePCLRaw` の版数対応で解消 |
| Release 署名 | csproj 直書きの `example.keystore` 設定を廃止し、**gitignore 対象の `.Signing.props`** から注入する方式に変更(未配置時は debug 署名)。`example.keystore` を削除 |
| `#pragma warning disable` | 発火し得ない抑止 9 行 / 5 ファイル(`SpeedGauge` CA1001 / `NewsItem` CA1056 / `DeviceInfoViewModel` SA1135 / `LabelOption.android` CA1416×2 / `SocialControls` CA1822)を削除。Phase 9-2 で追記された理由コメント 27 件も削除 |
| `Styles.xaml` | 参照 0 のスタイル 4 件を削除(`FillHorizontalStack` / `InputEntry` / `ItemCollectionLabel` / `SideFlexLayout`) |
| パッケージ更新 | CommunityToolkit.Maui 14.2.0→**15.0.0**、BarcodeScanning 3.0.4→3.1.0、Grpc 2.80/2.81→2.83.0、MAUI 10.0.80→10.0.90、Smart 系各種 |
| `.gitignore` / `AGENTS.md` | `.Signing.props` を追加 / 改行コードのルール(既存は変更しない・新規は CRLF)を追記 |
| ドキュメント | `Document/Development.md` に**実案件適用時の注意**(DB 初期化・シークレット・Release 署名・開発用 HTTPS 証明書・コンポーネントのスレッド契約)を追記。`Code_Review.md` / `Fix_Plan.md` / `Implementation_Plan.md` を作成後 **`Fix_Checklist.md` へ集約して削除**。`README.md` / `README-ja.md` に Android 専用である旨を追記 |

---

# 📚3. uibase3 → uibase4 — 外部リファレンス評価とライブラリ追従

10 ファイルのみの小さな区間。**画面の見た目・動作を変える変更は無く**、ドキュメント追加とライブラリ API 追従が中心。

## 🖼️A. 画面単位の変更

なし。

## 🧱B. 画面以外の変更

| 項目 | 内容 |
|---|---|
| `Document/Reference_Analysis.md`(新規 973行) | 外部の記事・OSS 51 件の詳細評価。**導入済みパッケージの未使用機能が大量にある**ことが最大の発見(Syncfusion は SfEffectsView のみ使用・約30種が未使用、CommunityToolkit.Maui も多数未使用、標準の `Stepper` / `DatePicker` / `TimePicker` / `RadioButton` / `SearchBar` は使用箇所ゼロ) |
| `Document/Reference_Summary.md`(新規 278行) | 上記の人間向け要約。取り込み候補 41 件、新規 NuGet 追加は 1 件のみ、判断項目 D1〜D19 の決定内容 |
| `Modules/ValidationHelper.cs` | `AccessorRegistry.FindAccessor` → **`AccessorProvider.FindAccessor`**(ライブラリの API 変更に追従) |
| `Resources/Styles/Styles.xaml` | 前区間で削除した 4 スタイル(`FillHorizontalStack` / `InputEntry` / `ItemCollectionLabel` / `SideFlexLayout`)を**復活**。加えて `NoErrorColor` / `GroupSpan` / `ItemCollectionGrid` を追加 |
| `.editorconfig` | `dotnet_style_operator_placement_when_wrapping` を `beginning_of_line` → **`end_of_line`** |
| `README.md` / `README-ja.md` | 「Android 専用」の記述を削除 |
| `Template.MobileApp/example.keystore` | 復活(署名自体は `.Signing.props` 方式のまま) |
| `Modules/Basic/BasicLocalViewModel.cs` | 旧ファイル名のまま `BasicLocaleViewModel` クラスが**重複追加**(次区間で削除) |

---

# 🧹4. uibase4 → fix1 — アナライザ設定の全面見直し

コミット `af18c6a4`「Fix1」1本のみ。**アナライザ / エディタ設定の全面見直しと、それに伴う機械的なコード追従**。画面の見た目・動作を変える変更は無い。

## 🖼️A. 画面単位の変更

| 画面 | 変更内容 |
|---|---|
| Basic Locale | 前区間で重複追加された `Modules/Basic/BasicLocalViewModel.cs` を**削除**(正は `BasicLocaleViewModel.cs`) |
| Basic Style | `BasicStyleViewModel` に CA1002 の pragma を付与 |
| Navigation Shared Input | `SharedInputView.xaml.cs` の namespace に CA1716 の pragma を付与 |
| Sample Chat / UIChat | `string.IsNullOrWhiteSpace` → `String.IsNullOrWhiteSpace` |

## 🧱B. 画面以外の変更

### 🧹B-1. アナライザ設定(この区間の主目的)

| ファイル | 内容 |
|---|---|
| `Analyzers.ruleset` | ルール名前空間を `Microsoft.CodeQuality.Analyzers` → **`Microsoft.CodeAnalysis.NetAnalyzers`** に修正(**従来の CA 抑止が効いていなかった**)。Hidden 指定を整理し、**CA1002 / CA1305 / CA1416 / CA1716 / CA1721 / CA1724 / CA1873 / CA2007 などの一括抑止を廃止**。StyleCop 側は SA1009 / SA1025 / SA1111 / SA1118 / SA1606・1607・1614・1616・1622・1623・1626・1629 / SA1642・1643 / SA1652 を Hidden に追加 |
| `GlobalSuppressions.cs` | 全体で許容するものを assembly 属性へ移動(CA1305 / CA1721 / CA1873 / CA2007) |
| 各ファイルの `#pragma` | ruleset から外したルールを**必要な箇所だけ** pragma で抑止(CA1724=`App` / `Extensions` / `Result` / `Parameters` / `Permissions`、CA1716=`Select` / `SharedInputView`、CA1002=`ShapeDrawing` / `ColorExtractor` / `DataService` / `BasicStyleViewModel`) |
| `.editorconfig` | 大幅な見直し(198 行)。`[*.slnx]` / `[*.{razor,cshtml}]` / `[*.{xaml,axaml}]` のセクションを追加。多くのルールの重大度を `warning` / `silent` → **`none` または重大度なし**へ変更(括弧・using 整理・式形式メンバーなど)。`csharp_style_expression_bodied_local_functions` / `_operators` は `when_on_single_line:warning` に変更 |

### 🧹B-2. アナライザ有効化に伴うコード追従

| 項目 | 対象 |
|---|---|
| **括弧の明示**(`always_for_clarity`) | `CalendarView.xaml.cs` / `CalendarView2.xaml.cs` / `MixerSlider.cs` / `GraphRowSurface.cs` / `BarcodeDrawing.cs` / `MonthViewBuilder.cs` / `HolidayService.cs` / `ScheduleService.cs` / `DeviceState.cs` / `AlternateRowBackgroundConverter.cs` / `BadgeCountConverter.cs` / `NumericInputModel.cs` ほか |
| **BCL 型名の使用**(`string`→`String` 等) | `LabelOption.cs` / `Gauge.cs` / `DayTimetableView.cs`(`float`→`Single`)/ `GraphRowSurface.cs`(`float`→`Single`・`double`→`Double`)/ `SampleChatViewModel.cs` / `UIChatViewModel.cs` |
| null 許容の整理 | `Gauge.cs`(`Unit!` → `Unit`)/ `DrawingControl.cs`(`Drawable = null!` → `null`) |
| BOM 除去 | `Directory.Build.targets` / `Template.MobileApp.csproj` / `AndroidManifest.xml` |
| `*.Designer.cs` | 自動生成物の追従 |

### 📦B-3. パッケージ更新

`Microsoft.Maui.*` 10.0.90→**10.0.100**、`CommunityToolkit.Maui` 15.0.0→15.0.1、`Usa.Smart.Resolver`(+ DI 拡張)2.15.0→**3.0.0**、`Usa.Smart.Core` 2.16.0→2.19.0、`Usa.Smart.Mvvm` 2.8.0→2.10.0、`Usa.Smart.Converter` 2.15.0→2.16.0。

### 📄B-4. ドキュメント

`Reference_Summary.md` / `Reference_Analysis.md` を改訂。ユーザー指示により **SCP(SSH.NET)を B-20 として追加**し、判断項目 **D20〜D22(SSH.NET 追加可否 / SCP サンプルのスコープ / 接続情報の保管とホスト鍵検証)を未決として追記**。

---

# 🔨5. fix1 → plus1 — 外部リファレンス評価の実装(フェーズ1〜10)

区間3で作成した外部リファレンス評価(記事・OSS 51 件)の**採用項目を実装した区間**。コミットは3本。

| コミット | 日付 | 内容 |
|---|---|---|
| `8ad7b772` Update plus p1 | 2026-09-01 | フェーズ1(基盤・低コストの穴埋め) |
| `51aa581b` Update plus p2 | 2026-09-01 | フェーズ2(抽選ホイール) |
| `3f213e4b` Update plux p x | 2026-09-03 | フェーズ3〜9 + Release トリミング修正 + D8 本採用 + SCP + ドキュメント整理 |

**新規 12 画面 + `Modules/App` および `Layouts/` の新設**。新規 NuGet は `SSH.NET` の 1 件のみで、他はすべて**導入済みパッケージの未使用機能のサンプル化**または自作。

## 🖼️A. 画面単位の変更

### 📋A-1. メニュー画面(空きセルの結線)

| 画面 | 変更内容 |
|---|---|
| Main/Menu | 9行 → **10行**。`10.App` を追加(`Modules/App` への入口) |
| BasicMenu | Row8 の空きボタンを **Setting**(`BasicSetting`)に結線 |
| ViewMenu | 空きセル5つを **Layout / DragDrop / State / Toolkit / Custom** に結線 |
| SampleMenu | 空きセル2つを **Sf Chart**(`SampleSfChart`)/ **Crop**(`SampleCrop`、Material `Crop` アイコン)に結線 |
| NetworkMenu | `Grid.Row="7"` の空きボタンを **SCP** に結線(それまで到達不能だった `NetworkScpView` が開けるようになった) |
| UIMenu | 10行 → **11行**。「描画デモ」グループを追加し **Wheel**(Material `Attractions`)を配置。残り2セルは規約どおり可視の無効ボタン |

### 🖼️A-2. 新規画面(12 画面)

#### 📲App モジュール(新設)

| 画面 | 内容 |
|---|---|
| AppMenu | `Modules/App` の入口メニュー |
| AppCalc | 科学電卓。コアは純モデル `Models/App/ExpressionCalculator`(トークナイザ → **操車場アルゴリズム**で中置→RPN → RPN 評価器の3段構成、NuGet 不使用)。四則 / %(百分率)/ 括弧 / 単項マイナス / 三角関数(DEG)/ log / ln / exp / √ / 累乗(右結合)/ 階乗 / π / e / **暗黙の乗算**(2π, 3(1+2))に対応。5列ダークレイアウト、結果表示は DSEG7、入力行末尾の「│」カーソルは `FormattedString` の Span 表現。「=」直後は演算子入力で結果から継続 |
| AppGame | 数独。盤面ロジックは純モデル `Models/App/SudokuGame`(バックトラッキングで完全解を生成し 36 マスを残して問題化 / 入力 / 行・列・ボックスの矛盾判定 / 完成判定)。盤面は `UniformItemsLayout`(9列)+ `BindableLayout`、3x3 区切りはセル VM の `Margin` で表現。矛盾は赤字、COMPLETE! バナー + Bounce。**ライフゲーム / 2048 は純モデルの差し替えで追加できる方針**をコメントで明記 |

#### 🖼️View モジュール(5 画面)

| 画面 | 内容 |
|---|---|
| ViewLayout | CommunityToolkit の `DockLayout`(上下左右ドック+残り充填)/ `UniformItemsLayout`(MaxColumns=4)。自作レイアウト3種(CircularLayout=曜日リング / StaggeredGrid=カードウォール / Cascade=MDI ウィンドウ風)も同画面へ追記 |
| ViewState | `StateContainer`(Loading / Empty / Error / 既定=Success の切替)+ `LazyView`(`x:TypeArguments` で `ViewStatePanelView` を遅延生成)。`Behaviors/LazyViewOption.cs` を新設し、`LoadViewAsync()` の呼び出しを VM のフラグから起動(code-behind 回避) |
| ViewToolkit | ルートを `SfBottomSheet` にし Content=`SfTabView`(タブ3枚)。入力タブ=`SfOtpInput` / `SfSegmentedControl` / `SfChipGroup`、表示タブ=`AvatarView` / `RatingView` / `Expander` / `SfAccordion`、シートタブ=ボタンで `IsOpen` |
| ViewDragDrop | 標準 `DragGestureRecognizer` / `DropGestureRecognizer` のデモ。①同一リスト内の並べ替え ②TODO⇔DONE のリスト間移動(列の空き領域へのドロップは末尾追加)③ゴミ箱ドロップで削除(`DragOverCommand` / `DragLeaveCommand` でハイライト)。3リスト共用の `DataTemplate` 1本で実装 |
| ViewCustom | 自作コントロール4種のカタログ(計画の `ViewInputView` と**統合して1画面に変更**)。`MarqueeLabel` / `TreeView`(+`TreeNode`)/ `ColorPicker`(RGBA スライダ4本 + ARGB hex)/ `DurationPicker`(時0-23・分5分刻み → `TimeSpan`) |

#### 🖼️Basic / Sample / UI モジュール(4 画面)

| 画面 | 内容 |
|---|---|
| BasicSetting | 使用ゼロだった標準コントロール `Stepper` / `DatePicker` / `TimePicker` / `RadioButton`(`RadioButtonGroup.GroupName` + `SelectedValue`)/ `SearchBar`(`SearchCommand`)を `Switch` / `Slider` / `Picker` / `Entry` と共に網羅。**各コントロールに `ToolTipProperties.Text` を付与**(長押しで表示)。Summary カードで双方向バインドを確認 |
| SampleSfChart | Syncfusion チャートのダッシュボード。Cartesian Column / Doughnut / **Polar(レーダー)** / Funnel + Pyramid / **SparkLine・SparkColumn・SparkWinLoss** / **Sunburst(2階層)** |
| SampleCrop | 画像切り抜き。`CropDrawing`(`IInteractiveDrawing` + `ExportPng` 共用構成)で枠移動 + 四隅ハンドルリサイズ(ヒット半径28・最小64・画像内クランプ)、三分割線 + 減光。書き出しは exporting フラグで `OnDraw` を出力モードに切替 |
| UIWheel | 抽選ホイール。`WheelDrawing`(扇形 + 回転テキスト + ハブ/リム/上部ポインタ)。`Spin(extra, length, completed)` が CubicOut で減速停止し**完走時のみ**当選項目を通知、離脱時は `CancelSpin()` で中断(通知なし)。ホイールタップ / SPIN ボタンの両方で回転、結果は `HasResult` の DataTrigger で「？」⇔当選名を切替 + Bounce。code-behind なし |

### 🖼️A-3. 既存画面の強化

| 画面 | 変更内容 |
|---|---|
| NetworkScp | **空スタブを実装**(フェーズ6)。接続先カード / 転送カード(**`FilePicker`** — 使用ゼロ API のサンプル化、リモートファイル名 Entry、ProgressBar、アップロード / ダウンロード / キャンセル)/ ログカード(最新20件)。ダウンロード先は `FileSystem.CacheDirectory` |
| Setting(Main) | SCP セクション(Host / User / Password)を追加し、**設定投入は設定画面の QR に統一**(`SettingViewModel.DetectCommand` に `ScpHost` / `ScpPort` / `ScpUser` / `ScpPassword` の4キーを追加)。項目増に伴い**ラベルと現在値を横並び**に変更(`SettingRowGrid` + キャプション幅108固定) |
| EditList | **複数選択 + 一括操作**。Function3=Select トグルで `SelectionMode` を None⇔Multiple、`SelectedItems` は `ObservableCollection<object>` にバインド。行は VSM Selected で青ハイライト、選択モード中は行ボタンを DataTrigger で非表示。下部バー=件数 + 全選択 + 一括削除(確認ダイアログ) |
| BasicBehavior | `MaskedBehavior`(電話番号)/ `UserStoppedTypingBehavior`(800ms)/ `EventToCommandBehavior`(Switch.Toggled)を追記 |
| BasicValidation | **相関検証**を追加。`Confirm` に `[Compare(nameof(Password))]`、Password 変更時は `PropertyChanged` 購読で Confirm を `ClearErrors`→`Validate` 再検証。CT 検証 Behavior は `EmailValidationBehavior` / `NumericValidationBehavior`(`Flags=ValidateOnValueChanged` + Invalid/ValidStyle で文字色切替) |
| BasicLocale | `ResourceManager.GetResourceSet` で resx キーを列挙し **neutral / ja / current の3値を一覧表示** + カルチャ別書式カード(current / en-US / de-DE / ja-JP の N2 / C / D / t) |
| SampleMap1 | **Google Maps `MapElements`**。右上 FAB 3個(Route / Pentagon / Circle)で経路(Polyline=スポット巡回)/ 範囲(Polygon=皇居周辺)/ 円(Circle=東京駅1.5km)をトグル |
| SampleMap2 | **Mapsui 強化**。画面左上のトグルパネルで機能グループ別マネージャを個別に有効化(ウィジェット / スポット+コールアウト / 図形 / GeoJSON / クラスタリング)+ **SkiaSharp オーバーレイ**のグラデーション経路 |
| SampleChart | `ChartKind` に **Stacked(積み上げ棒)/ Scatter(散布図)/ Heat(ヒートマップ)** を追加。**要素ごとのディレイ出現**(Stacked=棒ごと / Scatter=点ごと / Heat=行ごと。ディレイ系は全体1000ms)。Line の折れ線は**値の高さで色補間し線分ごとに塗り分け**(低=青→高=赤) |
| SampleChat | **音声フロー4ステップ**(モック)。マイク FAB + オーバーレイ(`StepIndicator` 4段)で ①録音(タイマー秒数 + Pulse 脈動 + 赤 Stop 化)②文字起こし(1.5秒待ち→固定文)③抽出プレビュー(`VoiceExtractItem` 4件)④承認で `InputText` へ反映。× / 画面離脱でリセット |
| UISchedule / UICalendar | `IScheduleEventProvider` への依存に変更(下記 B-5)。`DayTimetableView` がイベントを**カード描画**(白地 + ドロップシャドウ + 枠線 + アクセントバー)、幅110以上のカード右上に所要時間、空き時間帯を薄緑 + 「空き xh」表示。VM の日合計(予定 n 件 / 合計 / 空き)を追加 |
| UITelemetry | **Function2 で DIRECT⇔BUFFER を切替**(ダブルバッファの比較デモ。画面左上に MODE 表示)。滞在中のみフレーム統計 `[SceneStats]` を出力 |
| ViewCollection | `RemainingItemsThreshold="3"` + `RemainingItemsThresholdReachedCommand` で**無限スクロール**(最大16グループ)。Footer に読み込み済み件数 |
| ViewEffect | **`SKConfettiView`**(Celebrate トグル)+ `TouchBehavior`(PressedScale / 長押し Command)+ `IconTintColorBehavior`(画像の色替え)を追記 |
| ViewRefresh | 自作スケルトンを **`SfShimmer` の `CustomView`** に入れ、形はそのままで波アニメーションを追加 |
| ViewGraphics | `SketchDrawing`(フリーハンド描画・ストローク色自動循環・Undo/Clear)+ **PNG 出力**結果の表示、`PulseRingDrawing`(波紋リング)、`ProgressArcDrawing`(カウントダウンボタンの残量リング)を追加 |
| ViewLottie | **スクロール連動 / 長押し進行**。横スクロール帯(幅900のグラデーション Border)と長押しボタンを追加し、共通の `ScrubCommand` で Lottie の Progress を駆動 |
| ViewShadow | 末尾に**ニューモーフィズム**を追加。同色タイル(#E0E5EC)の Border 2枚重ねで暗影(#A3B1C6, +8+8)と明影(White, -8-8)を合成、凹は影の向きを反転。ページ全体を ScrollView 化 |
| ViewSvg | `SvgView` 拡張に伴い VM から SKSvg ロード処理が消え、**パス切替だけの VM** になった(`IFileSystem` 依存も除去) |
| UIFlight / UITactical / UIEnergy / UITelemetry | 静的レイヤの `SKPicture` キャッシュ適用(下記 B-1)。Flight は**レーダーブリップのタップ選択**(最近傍14単位以内、再タップで解除、選択リング + TGT 情報行)を追加 |

## 🧱B. 画面以外の変更

### 🖌️B-1. Graphics 基盤の拡張

| 対象 | 内容 |
|---|---|
| `DrawingObject` | **単発アニメーション + 完了通知** `AnimateValue(name, start, end, length, easing, frame, completed)` を新設。`IAnimatable` の `Animate` 拡張で減速停止し、**完走時のみ** completed を呼ぶ(中断時は呼ばない)。`AbortAnimation` / `AnimationIsRunning` も公開 |
| `IInteractiveDrawing` / `DrawingControl` | タッチ(Start / Drag / End)を Drawing へ転送。`DrawingObject.ExportPng(stream, w, h)` は**表示と同じ `OnDraw`** を `PlatformBitmapExportContext` へ流して画像化 |
| 新規 Drawing | `WheelDrawing`(抽選ホイール)/ `SketchDrawing`(フリーハンド)/ `CropDrawing`(切り抜き)/ `PulseRingDrawing`(波紋)/ `ProgressArcDrawing`(残量リング) |
| `ChartDrawing` | Stacked / Scatter / Heat の3種追加、要素ディレイ出現、値連動のグラデーション線 |
| `SceneObject` | **静的レイヤの `SKPicture` キャッシュ** `DrawCachedLayer(canvas, key, w, h, draw)`(サイズ変化時のみ再記録)。**ヒットテスト** `Touch` + `SceneControl.EnableTouchEvents`。**論理解像度固定** `VirtualSize`(opt-in・uniform scale + レターボックス + タッチ座標逆変換)。**ダブルバッファ** `UseDoubleBuffer`(ループスレッドでオフスクリーン `SKSurface` に描画 → `Snapshot()` をロック付き交換、UI スレッドは転写のみ)+ フレーム統計 `[SceneStats]` |
| `ScenePool<T>` | **新規**。短命オブジェクトの再利用プール(Rent / Return + CreatedCount) |
| 各 Scene | キャッシュ適用: Energy=ドット背景(約500円/フレームの描画を排除)/ Flight=レーダー盤面 + ロール目盛 / Tactical=マップ静的層(パネル枠 + 等高線ストローク + グリッド + 固定ラベル)/ Telemetry=タコメーター盤面 + Gフォース盤面(破線円の `SKPathEffect` 毎フレーム生成も解消) |

**D8(ダブルバッファ)の Release 実測 → 本採用**

| モード | 描画1回の avg | max | フレーム数/3秒 | 実効フレームレート |
|---|---|---|---|---|
| DIRECT(UI スレッド描画) | 16.5〜17.8ms | 19〜44ms | 90〜97 | 約 30fps |
| BUFFER(ループスレッド描画+転写) | **14.0〜14.5ms** | 17〜37ms | **171〜186** | **約 60fps** |

フレームレートがほぼ倍増したため **`UseDoubleBuffer` の既定を ON**(4シーン全て)に変更。Telemetry の Function2 トグルは比較デモとして残置。
※ 実演として入れた Energy の火花は「わかりづらい」ためユーザー判断で削除(`ScenePool<T>` は基盤として残置)。

### 🧰B-2. `Layouts/` の新設(第3の拡張ポイント)

| ファイル | 内容 |
|---|---|
| `CircularLayout.cs` | 子要素を円周に均等配置(真上開始・時計回り)。`Radius` 未指定は領域から自動、添付プロパティ `Angle` で個別角度も可 |
| `StaggeredGrid.cs` | 高さの異なるカードを**最も低い列へ詰める** Pinterest 型 |
| `AppLayoutManagerFactory.cs` | `ILayoutManagerFactory` のデモ。DI 登録したファクトリが `CascadeStackLayout` のときだけカスケード配置のマネージャを返し、他は null(=既定)。**サブクラス側を変更せずに配置アルゴリズムを差し替えられる**ことを示す |

### 🧰B-3. コントロール / ビヘイビア

| ファイル | 内容 |
|---|---|
| `Controls/SvgView.cs` | **拡張**。`Source`(アプリパッケージ内パス)/ `Placeholder` / `ErrorPlaceholder` と `Loading` / `Ready` / `Error` イベント、`SKSvg` の共有キャッシュ。従来の `Svg`(SKSvg 直接バインド)は互換維持で `Source` が優先 |
| `Controls/MarqueeLabel.cs` | **新規**。クリップした Grid 内の Label を無限スクロール。Loaded / Unloaded / SizeChanged で開始・停止・再計算 |
| `Controls/TreeView.cs` | **新規**。展開中ノードをフラット化して並べ直す簡易ツリー。▸/▾ 切替・行選択(`SelectedNode` TwoWay) |
| `Controls/ColorPicker.cs` | **新規**。RGBA スライダ4本 + プレビュー + ARGB hex(`SelectedColor` TwoWay・再入ガード) |
| `Controls/DurationPicker.cs` | **新規**。時 / 分の Picker 2個 → `TimeSpan`(TwoWay) |
| `Controls/DayTimetableView.cs` | イベントのカード描画・空き時間帯表示に対応 |
| `Behaviors/LazyViewOption.cs` | **新規**。`LoadViewAsync()` を VM のフラグから起動する添付プロパティ(code-behind 回避) |
| `Behaviors/Scroll.cs` | `RatioCommand` を新設(ScrollView のスクロール量を 0-1 に正規化して ICommand へ) |
| `Behaviors/AnimationOption.cs` | `HoldCommand` / `HoldDuration` を新設(Button の Pressed/Released で 0-1 を進め、途中離しは 250ms で巻き戻し、完走後の再押下は先頭から) |

### 🗺️B-4. 地図基盤

| ファイル | 内容 |
|---|---|
| `Messaging/MapController.cs` | `SetRoute`(Polyline)/ `SetArea`(Polygon)/ `SetCircle`(Circle)を追加 |
| `Messaging/MapsuiMapManagers.cs` | **新規**。`IMapsuiMapManager`(Attach / Detach)を `MapsuiController` の辞書に登録し個別に有効化する**機能グループ別マネージャ構成**。`MapsuiWidgetManager`(ScaleBar + ZoomInOut)/ `MapsuiSpotManager`(`PointFeature` + `SymbolStyle` + **`CalloutStyle`**、`Map.Tapped` → `GetMapInfo` でトグル)/ `MapsuiShapeManager`(NTS の LineString / Polygon)/ `MapsuiGeoJsonManager`(`Resources/Raw/Map/tokyo.geojson` を EPSG:4326 のまま読み `ICoordinateFilter` で球面メルカトルへ再投影)/ `MapsuiClusterManager`(固定シード240点を解像度連動のグリッドクラスタリング、`ViewportChanged` でズーム変化時のみ再計算) |
| `Behaviors/MapsuiBind.cs` | `Overlay` 添付プロパティで `SKCanvasView`(InputTransparent)を同じコントローラに結線。`Viewport.WorldToScreen` で経度緯度→画面座標へ変換し、**線分ごとに `SKShader.CreateLinearGradient` を差し替えるグラデーション経路**を白ハロー付きで描画 |
| `Resources/Raw/Map/tokyo.geojson` | **新規**。GeoJSON サンプルデータ |

### 🗄️B-5. モデル / サービス

| ファイル | 内容 |
|---|---|
| `Models/App/ExpressionCalculator.cs` | **新規**。式評価エンジン(AppCalc のコア。UI 非依存の純モデル) |
| `Models/App/SudokuGame.cs` | **新規**。数独の生成 / 入力 / 矛盾判定 / 完成判定(AppGame のコア) |
| `Models/Sample/Calendar/TimetableCalculator.cs` | **新規**。区間マージ / 空き算出 / 所要時間表記を集約し、描画と VM の日合計で共用 |
| `Services/IScheduleEventProvider.cs` | **新規**。`ScheduleService` が実装し、`UICalendar` / `UISchedule` 両 VM はインターフェース依存へ(`BindSingleton<IScheduleEventProvider, ScheduleService>()`) |
| `Services/ScpService.cs` | **新規**。`ScpClient` ラッパ(DI 登録)。`ConnectionInfo` + `RemotePathTransformation.ShellQuote`(旧形式 ctor は CS0618=コマンドインジェクション注意のため不使用)。`Uploading` / `Downloading` イベントを `IProgress<double>` へ中継、キャンセルは `CancellationToken.Register(client.Disconnect)`(転送 API が同期のため) |
| `State/Settings.cs` | SCP 設定を追加。`ScpHost` / `ScpPort`(既定22)/ `ScpUser` は `IPreferences`、`ScpPassword` は `SecureStorage`。**ホスト鍵指紋の設定は D22-b の変更で撤去**(QR 照合は行わず、サーバ指紋の参考表示のみ) |
| `MauiProgram.cs` | `ConfigureLifecycleEvents` に Android の Create / Start / Resume / Pause / Stop / Destroy フックを実装(挙動は変えずログのみ。`adb logcat -s AppLifecycle`)。`ConfigureCustomLayouts`(`ILayoutManagerFactory` の DI 登録)を追加。`ScpService` / `IScheduleEventProvider` を登録 |

### 🏗️B-6. Release ビルドのトリミング対応(フェーズ8の計測で発覚)

初の Release 実行で**既存の潜在問題**が表面化(この区間の実装変更とは無関係)。

- **症状**: Release(トリミング有効)で起動時に `TargetInvocationException` → アプリ落ち。Debug は正常
- **原因**: トリマーが `PageContextStorage`(Smart.Navigation)のコンストラクタを削除し、Smart.Resolver の `StandardProvider.CreateFactory` が「No constructor available」で失敗(リフレクションでコンストラクタを解決するためトリミングと相性が悪い)
- **修正**: csproj に **`TrimmerRootAssembly` を追加**(`Template.MobileApp` + Smart 系 + MauiComponents 系 + `Renci.SshNet` / `BouncyCastle.Cryptography` の計21アセンブリ)
- **副産物(恒久コード)**: `ApplicationInitializer.Initialize` に**起動失敗時の完全な例外連鎖ログ**(`StartupError` タグ)を追加。トリミング時は例外メッセージがリソースキー化されるため、これが無いと原因が追えない。また `Console.WriteLine` は Release の Android では logcat に出ないと判明したため、診断 / 計測ログは `Android.Util.Log` 直接出力へ変更

### 📦B-7. パッケージ

`SSH.NET` 2026.0.0 を追加(この区間で追加した唯一の NuGet)。推移依存の増分は **`BouncyCastle.Cryptography` 2.7.0 のみ**。

### 📄B-8. ドキュメントの整理(ユーザー指示)

| 対象 | 内容 |
|---|---|
| `Reference_Analysis.md` / `Reference_Summary.md` | **削除**。採用項目は実装完了したため、残る価値(決定 D1〜D22 と不採用理由)はアーカイブへ移設(現在は**本書の付録D**。当時は `UI_Development_Log.md` 末尾) |
| `Fix_Checklist.md` | **削除**。第1部の残課題は `Implementation_Checklist.md` へ統合、第2部の完了記録は削除 |
| `Implementation_Checklist.md` | **新規**。残項目のみに再構成(実機確認 + SCP 実テスト + 保留・対象外 + 他案件の残課題)。**2026-09-03 に `Task_Checklist.md` へ統合** |
| `UI_Verification_Checklist.md` | 残確認のみに再構成(0〜12章の実装済み画面別変更記録を削除)。**2026-09-03 に `Task_Checklist.md` へ統合** |

**対応不要 / 取りやめの確定**

- **Blazor Hybrid**: 別リポジトリ `template-maui-blazor` が BlazorWebView + Routes/Layout/Pages の完全な Hybrid 構成で充足しているため**対応不要で確定**
- **MBTiles(`BruTile.MbTiles`)**: SQLitePCLRaw 3.0 系との衝突リスク + デモ用アセット未保有により**取りやめ確定**(パッケージ未追加のため削除対象なし)

## 💡C. この区間のナレッジ

### 🏷️MAUI / XAML

- **MAUI 10 で `Page.OnBackButtonPressed` は素の `ContentPage` では呼ばれない**。`MauiAppCompatActivity` の `OnBackPressed()` override が廃止され、AndroidX `OnBackPressedDispatcher` のコールバック 1 本になったため。有効判定は `Window.CanConsumeBackNavigation` で、Shell / NavigationPage / FlyoutPage / MultiPage 以外は常に false → **自前で `OnBackPressedCallback` を登録するしかない**。`android:enableOnBackInvokedCallback="false"` の退避策も効かない (dispatcher に有効なコールバックが無いだけなので結局システム既定の finish になる)。関連: dotnet/maui#31266 (OnBackButtonPressed の見直し提案)
- **`App.OnStart` はプロセスに 1 回しか呼ばれない**(`Application.SendStart()` の `_isStarted` ガード)。Android では Activity 再生成のたびに `CreateWindow` は呼ばれるが `OnStart` は呼ばれないため、**画面構築を `OnStart` に依存させると再生成で白画面になる**。公式ドキュメント (App lifecycle) も標準は `Window` のイベント (`Created` = Android の `OnPostCreate`) で、`Application.OnStart` は登場しない。関連: dotnet/maui#18845 (Verified / Backlog・未修正)
- Activity 再生成は BACK 以外でも起きる。`ConfigurationChanges` に `FontScale` / `Locale` が無いため、**端末のフォントサイズ・言語変更で必ず再生成**される (`adb shell settings put system font_scale 1.30` で再現可能)
- `launchMode` (`singleTop` / `singleTask` / `singleInstance`) は「既存インスタンスの再利用方法」の設定なので、**finish 済みで再利用対象が無いケースには効かない**。BACK で Task / ActivityRecord は消滅し、プロセスだけが `oom_score_adj` 900 の空プロセスとして残る
- **Syncfusion の URL 名前空間は Charts / SparkCharts / SunburstChart を解決できない**(MAUIG1001 の不可解な ElementNode エラー)→ チャート系は `clr-namespace` で参照。チャートの `Fill`/`Stroke` は Brush 型のためリテラル色を指定
- Smart.Mvvm の `[ObservableProperty]` に CommunityToolkit 流の `OnXxxChanged` partial フックは無い(CS0759)→ `PropertyChanged` 購読が本プロジェクトの定型
- `SKConfettiView` は `Systems` を明示定義(xmlns の assembly は `SkiaSharp.Extended.UI`)。`SfShimmer.CustomView` は既存スケルトンに波アニメだけ足せる
- カスタムレイアウトは `Layout` 継承 + `CreateLayoutManager()` が最小構成。`ArrangeChildren` の bounds は **Padding 込み**。`ILayoutManagerFactory` は **null 返却で既定マネージャへフォールバック**するため対象型以外へ影響しない
- C# コントロールの自己バインドは typed `SetBinding(..., static (T v) => v.Prop, source: this)` が使える。TwoWay の合成コントロールは「updating フラグで再入ガード」が定石
- `CollectionView.SelectedItems` は `ObservableCollection<object>` をバインドすると**双方向**に機能する(選択変更で中身が更新され、コード側の Add も UI に反映)
- 相関検証は `[Compare(nameof(X))]` + `PropertyChanged` 購読で相手側を `ClearErrors`→`Validate` 再検証

### 🖌️SkiaSharp / Graphics

- ICanvas(MAUI Graphics)の**ストロークにグラデーションは使えない** → 区間分割 + 色補間で代替(Line チャートのグラデーション線)
- Drawing の PNG 出力は `Microsoft.Maui.Graphics.Platform.PlatformBitmapExportContext` が**追加パッケージなし**で使える
- SkiaSharp 4 系: `SKCanvas.DrawImage` は `SKSamplingOptions` 付きオーバーロードを使う
- `SKPictureRecorder` は共有ペイントを使う描画でも記録可(描画呼び出し時点のペイントを記録)。キャッシュ再生は呼び出し時点のキャンバス変換の中で行われるため**仮想座標系のまま記録**して良い
- `PathF.AddArc` の角度は**反時計回りが正**(0°=3時方向)/ `ICanvas.Rotate` は**時計回りが正**。回転後に `(cx+r, cy)` へ右寄せ描画すると半径方向の外向きテキストになる
- `Animation` を直接 new すると CA2000 → `IAnimatable.Animate` 拡張なら生成が MAUI 側に隠れ警告なし。async メソッド内の `Dispatcher.Dispatch` は CA1849 → `DispatchAsync`
- `System.Threading.Lock`(net9+)は `lock` 文でそのまま使える

### 🗺️Mapsui / 地図

- **`Mapsui.Styles.Color.FromString` は 3/6 桁 hex のみ対応。8 桁(アルファ付き)は実行時 ArgumentException でクラッシュ** → 半透明は `new Color(r, g, b, a)`(MAUI の `Color.FromArgb` は 8 桁可、という差異に注意)
- `Map.Widgets` は `ConcurrentQueue` のため取り外し不可 → トグルは `widget.Enabled` で行う。タップ判定は v5 では `Map.Tapped` + `e.GetMapInfo([layer])`(`IsMapInfoLayer` は廃止)
- GeoJSON は `GeoJsonProvider`(ファイルパス前提)より **GeoJSON4STJ 直接デシリアライズ + `ICoordinateFilter` 再投影**がアセット運用に合う(追加パッケージ不要)。`SymbolStyle` は `SymbolType`+`Fill`/`Outline` で図形シンボル可
- Mapsui のビューポートは論理座標系 → `SKCanvasView` の物理ピクセルとは `e.Info.Width / view.Width` でスケールを合わせる

### 🌐SSH / SCP

- SSH.NET の `ScpClient(string, ...)` ctor は**旧形式(CS0618)** — パス未エスケープでコマンドインジェクションの恐れ → `ConnectionInfo` + `RemotePathTransformation.ShellQuote`
- `ScpClient` の転送 API は同期のみ → `Task.Run` + `CancellationToken.Register(client.Disconnect)` でキャンセル対応
- `SettingParser` は改行区切りのため **1 値に改行を含められない**(PEM 秘密鍵は QR に載せられない)。QR ペイロード例(指紋設定は撤去済み):

```
ScpHost=192.168.1.10
ScpPort=22
ScpUser=deploy
ScpPassword=********
```

### 🧹アナライザ / .NET

- IDisposable の所有は**フィールドでなく get-only プロパティ + 宣言時初期化**にする(CA2000/CA2213 は `Disposables.Add` を所有移転と認識しない)。`CancellationTokenSource` フィールドは `Dispose(bool)` オーバーライドで明示 Dispose
- 構造体の CA1815 は `readonly record struct` 化が最小修正 / private 例外クラスは CA1064 → public + 標準 3 コンストラクタ / `Random.Shuffle`(.NET 8+)で Fisher-Yates が 1 行
- `IImage` は `Microsoft.Maui.IImage` と `Microsoft.Maui.Graphics.IImage` で衝突(CS0104)→ `using` エイリアスで解決
- インターフェースのパラメータ名 `end` は CA1716(VB 予約語)→ `startDate`/`endDate`。公開 static メソッドの `List<T>` 戻り値は CA1002 → `IReadOnlyList<T>`
- ctor 内で自プロパティを参照するラムダは、コマンド代入**前**に `PropertyChanged` を購読すると CS8602 → 購読を代入後へ移動。`await` 跨ぎの状態ガードは CA1508(常に true 扱い)に注意

### 🏗️Release / 計測

- **`Console.WriteLine` は Release の Android では logcat に出ない**(stdout 転送は Debug のみ)→ 診断 / 計測ログは `Android.Util.Log` 直接出力(`StartupError` / `SceneStats` タグ)
- トリミング時は例外メッセージがリソースキー化されるため、**起動失敗時の完全な例外連鎖ログが無いと原因が追えない**。調査手順: `adb logcat -d -b crash` でスタック → メッセージがキーのみなら例外連鎖ログを仕込んで再現 → 内部例外で特定
- 新規作成したテキストファイルは **LF になっていることがある** → CRLF 規約のため作成後に改行コードを確認して変換する(パイプ処理は python の `os.walk` が確実)

---

# 🧩6. plus1 → baseup1 — 基盤刷新(DI 移行・白画面対策・メニュー再編)

フェーズ10 の記録類に続けて、**DI コンテナ移行・BACK/白画面対策・メニュー再編・ドキュメント統合**を行なった区間(2026-09-03 → 09-05)。コミットは 9 本。

## 🖼️A. 画面単位の変更

### 📋A-1. メニュー画面の再編

| 画面 | 変更 |
|---|---|
| メインメニュー(`Modules/Main/MenuView.xaml`) | **番号プレフィックス廃止**・並び替え(View → Sample → UI → App、**Setting を最後**)→ **9 段×2 列へ再構成**(Data\|Network / Sample\|App / UI 1\|UI 2 をペア行に・Setting 最終行・余り 1 行は可視の無効ボタン)+**全ボタンに Material アイコン追加**(`MenuIconButton` 化。Widgets/Navigation/Devices/Storage/Cloud/Layers/Science/Apps/Palette/Insights/Settings) |
| UI メニュー(`Modules/UI/UIMenu1*` / `UIMenu2*`) | **旧 UIMenu を UIMenu1(アプリ系 18 画面)/ UIMenu2(可視化・計器・HUD 系 13 画面)へ分離**。グループ毎に行を分け、余りセルは可視の無効ボタン。**F4 で相互遷移**、31 画面の戻り先を所属メニューへ振り分け。旧 UIMenuView/VM は削除。**各 3 列×9 段**(メニュー規約を 9 段基本へ改定。2 列化も検討したが UI 1 の 18 ボタンは 2 列×9 段=18 セルちょうどでグループ行分けが成立せず、**列数は UI 1/UI 2 で統一する方針=両方 3 列**を維持して拡張行を追加) |
| `Modules/Main/MenuViewModel.cs` | ルート画面の BACK に `AndroidHelper.MoveTaskToBack()` を結線。戻り先が無いルートでは終了せずバックグラウンドへ送る(Android の作法)。Activity が生き残るので再生成経路も踏まない |

## 🧱B. 画面以外の変更

### 🧩B-1. DI コンテナ移行(Usa.Smart.Resolver → BunnyTail.DependencyInjection 0.4.0)

`template-maui2` の 69ba9a41 と同様の変更(2026-09-03)。

- csproj: Smart.Resolver 系 2+Navigation.Resolver+MauiComponents.Resolver 参照を削除、Smart.Navigation 3.4→**3.8** / Mvvm 2.11 / BunnyTail 系整合、TrimmerRootAssembly から Resolver 系 4 行削除
- `MauiProgram`: `GeneratedServiceProviderFactory`+`IServiceCollection` 化。View/ViewModel/Context は `[ComponentRegistration]` のソース生成 `AddViews`/`AddViewModels`/`AddContexts`、HttpClient 登録も ConfigureContainer へ統合し `Services/AppHostBuilderExtensions.cs` 削除
- **`GeneratedFactory.cs` 新設**: ライブラリ内部登録型のファクトリ明示生成(Shiny 4 型+MauiComponents 8 型+App+PopupFocusPlugin+CT PopupService)
- `WizardContext`: IInitializable/IDisposable → **`IScopeLifecycle`**。`ApplicationInitializer` に DEBUG 時のフォールバック報告出力

### 🐛B-2. 移行で表面化した不具合の修正(`Shell/ShellProperty.cs` / `ShellUpdateBehavior.cs`)

退場ビューのバインディング解除が ShellProperty 変更を発火し、遷移直後のタイトル/F キー状態を旧値で上書き(Smart.Navigation 3.8 で解除順が変化)→ **現在ビューのみ反映する CurrentView ガード**を追加。

### 🚀B-3. BACK キーと Activity 再生成(白画面)対策(2026-09-04)

| ファイル | 内容 |
|---|---|
| `Platforms/Android/MainActivity.cs` | **BACK キーの受け取りを自前化**。MAUI 10 の `MauiAppCompatActivity` は `OnBackPressed()` の override を廃止し、AndroidX `OnBackPressedDispatcher` へ登録した `MauiOnBackPressedCallback` のみで BACK を処理する。その `Enabled` は `Window.CanConsumeBackNavigation`(Shell/NavigationPage/FlyoutPage/MultiPage のみ true)で決まるため、**素の ContentPage では `Page.OnBackButtonPressed` が一切呼ばれない**。`base.OnCreate` の後に自前の `OnBackPressedCallback`(`Enabled=true`)を追加して `Page.SendBackButtonPressed()` へ流す(後勝ちで先に呼ばれる)。未処理時は自身を一時無効化して `OnBackPressedDispatcher.OnBackPressed()` へフォールバック |
| `State/StartupState.cs` / `App.xaml.cs` / `MainPageViewModel.cs` / `MauiProgram.cs` | **Activity 再生成時の初期画面復帰** (2026-09-06 に方式変更)。`Application.SendStart()` は `_isStarted` ガードでプロセス内 1 回のみのため、プロセス生存のまま Activity が作り直されると `App.OnStart()` が再実行されず初回遷移が走らない → 新しい `MainPage` のコンテナが空で**白画面**。**初期画面への遷移を `MainPageViewModel.OnCreated()` へ移動**した(`MainPage.xaml` の `s:AppLifecycleBehavior` が `Window.Created` を購読するため **Activity 生成のたびに必ず走り**、復帰時の `Resumed` では走らない)。`OnCreated` を `async void` にして 「`await startup.Completed` → `Navigator.Exit()` → `ForwardAsync(ViewId.Menu)`」を実行する。起動時の初期化(DB再構築・クラッシュレポート)は `App.OnStart` に残し、完了を **`State/StartupState.cs`** へ通知する(`TaskCompletionSource` を隠蔽し `Completed` / `NotifyCompleted()` のみ公開。**完了後に待ち始めても即座に返る**ため作り直し後の ViewModel でも取りこぼさない。単発の `IReactiveMessenger` は `Subject<T>` でリプレイしないため不可)。`OnDestroying` の `destroying` フラグは初期化中に作り直された場合の二重遷移防止。旧方式(`CreateWindow` で 2 回目以降の `Window.Created` を拾う `windowCreated` / `RestoreInitialViewAsync` / 専用ログ 2 件)は撤去し `App` は元の姿へ |
| (他テンプレートへ横展開) | A-1 (BACK の受け取り) は `template-maui` / `template-maui2` / `template-maui-keyboard` / `template-maui-blazor` へ反映済み。**新方式の B-1 (`StartupState` + `OnCreated`) は `template-maui-keyboard` のみ**。`template-maui-blazor` は `INavigator` の参照が 1 箇所も無く UI が `BlazorWebView` として XAML 宣言済みのため **B-1 は対象外**で、代わりに DB 初期化のエラー処理 (`InitializeDataAsync` + ダイアログ + `Quit()`) を揃えた。`template-maui` への反映は 2026-09-06 のユーザー判断で**不要**。全プロジェクト 0 エラー・自コード由来の警告 0 |

**検討して不採用にした案**(旧 `Task_Checklist.md` 0 節の調査記録より):

| 案 | 内容 | 不採用の理由 |
|---|---|---|
| A-2 | `MainPage` を `NavigationPage` 等でラップして `CanConsumeBackNavigation` を true にする | シェル構造(ヘッダー/ファンクション/コンテナ)の作り直しが必要で影響が大きい |
| A-3 | MAUI 側の修正待ち | dotnet/maui#31266 は提案段階(.NET 10 SR11 マイルストーン)で時期未定 |
| B-2 | コンテナ再接続(表示中 View を新コンテナへ付け替え) | 破棄済み Activity / MauiContext のハンドラを持つ View の付け替えになりリスク高 |
| B-3 | Window/MainPage の再利用 | `Window.Destroying()` で `RemoveWindow` + `Handler.DisconnectHandler()` が走るため非推奨 |
| — | `launchMode` / `alwaysRetainTaskState` 等の manifest 設定 | **実測で否定**。BACK で Task / ActivityRecord は完全に消滅し(`Task #83` → 消滅 → 再起動で `Task #84`)プロセスだけが空プロセス(`oom_score_adj` 0→900)として残る。launchMode は「既存インスタンスの再利用方法」の設定なので finish 済みでは効かない |

**実測メモ**: ①原因B は **BACK と無関係に再現する** — 表示中に端末のフォントサイズを変更すると `ConfigurationChanges` に `FontScale`/`Locale` が無いため Activity が再生成され(pid 不変)画面が完全に空になる(uiautomator でテキスト 0 件)。通常操作で踏める不具合のため原因A を直しても対処必須だった ②Android 16 / targetSdk 36 では `onBackPressed` も `KEYCODE_BACK` も配送されず、`android:enableOnBackInvokedCallback="false"` の退避策も効かない(dispatcher に戻るだけで有効なコールバックが無い) ③確認は 8 経路(コールド起動 / ホーム→再開 / サブ画面 BACK / ルート BACK / BACK 終了→即再起動 / `am start` 直接起動 / フォントサイズ変更 / 他アプリ切替・プロセス kill)で全て Menu 表示を確認(2026-09-04・Release)

### 🖼️B-4. リソース — Images の用途別階層化(画像アセット拡充の前準備)

`Resources/Images/` を**用途別 10 フォルダへ階層化**(Banner/Character/Chat/Common/Login/Onboard/Pet/Profile/Shop/Stream=Raw と同じ PascalCase。`MauiImage` glob を `Resources\Images\**` へ変更、参照はファイル名のまま)+**プレースホルダ 42 枚を配置**(現在スロットで使用中の既存画像のコピー。実素材は同名上書きで反映)。

### 📄B-5. ドキュメントの統合(2026-09-03)

| ファイル | 内容 |
|---|---|
| `Document/Change_Summary.md` | **本書**。タグ区間×画面単位の変更まとめとして新設(`UI_Development_Log.md` を統合して削除。ナレッジ=各区間の C 節、恒常情報=付録) |
| `Document/Task_Checklist.md` | **新規**。残作業の統合マスター(`UI_Verification_Checklist.md`+`Implementation_Checklist.md`+旧 `UI_Task_Checklist.md`+`Image_Asset_Expansion_Plan.md` を統合し、4 本とも削除) |
| `Document/Development.md`(+39行) | フェーズ10。「リスト表示」「タッチフィードバック」「Release ビルドでの検証と計測」の 3 節を追記 |

### 📌B-6. その他

- `.editorconfig` の軽微な調整
- `MauiProgram` に `BusyState.Default` の Singleton 登録を追加

## 💡C. この区間のナレッジ

### 🧩DI コンテナ移行(BunnyTail.DependencyInjection)

- 運用: DEBUG 起動時に `DescribeRuntimeFallbacks` の出力(そのまま貼れる属性行)を `GeneratedFactory.cs` へ貼り、ライブラリ内部で登録される型(`AddComponentsXxx` / `UseShiny` 等)のファクトリを明示生成する。自コードの `AddSingleton<T>` 等はジェネレータが自動生成する
- `Shiny.AndroidPlatform` は属性を書いてもファクトリ生成されない(生成不能な ctor)ため**リフレクションフォールバックのまま残置**(従来も Smart.Resolver のリフレクション生成であり同等)
- **退場ビューのバインディング解除が ShellProperty(バインドされた Function4Enabled 等)の変更を発火し、遷移直後のシェル状態を旧値で上書きする**(Smart.Navigation 3.8 で BindingContext 解除順が変化)→ `ShellProperty` に「現在ビューのみ反映」の CurrentView ガードを追加(症状=タイトルが 1 画面遅れる。Wizard / Lottie / Edit List などバインドを持つ画面の離脱で再現)
- ページスコープは Navigation 3.8 の DI 拡張が担う: Context を DI に Transient 登録+`IScopeLifecycle`(OnScopeInitialize/OnScopeTerminate)。`[Scope]` プロパティ注入・複数画面での共有・離脱時破棄まで従来どおり(実機で確認済み)
- `SudokuCellViewModel(int,int)` のような手動 new 前提の型も "ViewModel$" パターンで DI 登録される(解決されなければ無害。ValidateOnBuild は無効)
- 検証中に **BACK 終了→即再起動で白画面**になる事象を確認(コールド起動 / ホーム→再開は正常)。プロセス生存中の再起動で `App.OnStart` の初回ナビゲーションが走らない構造によるもので **DI 移行とは独立** → 本区間の B-3 で対策済み
- adb での Entry 入力は日本語 IME の未確定に注意: `input text` の後 KEYCODE_ENTER(66) で確定し、BACK(4) でキーボードを閉じてから画面下の F キーをタップする

---

# 🐛7. baseup1 → fix2 — ReSharper 全件対応と Scene 描画の重大バグ修正

`jb inspectcode` の指摘 **254 件を全数分類して対応**し、その過程で発覚した **Scene 描画基盤の重大バグ(かくつき・ANR・SIGSEGV)を修正**した区間(2026-09-05)。コミットは 1 本。

## 🖼️A. 画面単位の変更

### 📍A-1. DeviceLocationView / ViewCustomView — 未取得値の「-」表示

- `FallbackValue='-'` を 8 バインドへ追加(位置**未取得**=`Location` が null でパス不成立のとき「-」表示)
- Motion 4 項目(Altitude/Course/Speed/Accuracy)は **`TargetNullValue='-'` を併用**(取得済みでも Course/Speed 等の**末端プロパティが null** のとき「-」表示。従来は単位だけが残っていた)。実機確認済み
- `AnimationOption.HighlightTrigger` の 1 バインドのみ**意図的に未適用**(FallbackValue を付けると初回位置取得時にハイライトが発火する挙動変化が出るため)

### 🧹A-2. XAML 横断(ReSharper 対応)

- **方針決定: Grid の `RowDefinitions`/`ColumnDefinitions` は Style の Setter で定義せず Grid 側に個別記述**(12 スタイル→55 Grid へインライン化。Basic 4+Data+Device 4+ViewEasing の 10 画面。値は同一のため表示不変)= `Xaml.IndexOutOfGridDefinition` 誤検知 99 件解消
- `x:Reference`/`RelativeSource` バインドの誤検知 25 件は `ReSharper disable/restore Xaml.BindingWithContextNotResolved` コメントで範囲抑止(14 箇所)
- XAML `x:Name` リネーム(`_self`→`Self`×2・`indicators`→`Indicators`)、冗長 xmlns 削除 2

## 🧱B. 画面以外の変更

### 🐛B-1. 【重大バグ修正】`Graphics/Scene/SceneObject.cs` — ダブルバッファ描画がメインスレッドで実行されていた

区間5(D8)で導入したダブルバッファの `Loop` は、`await WaitForNextTickAsync` に `ConfigureAwait(false)` が無く、`Start()`(main)からの継続が main の SynchronizationContext へ戻るため、**RenderToBuffer(フルスクリーン CPU 描画+Snapshot)+転写の 2 重描画を毎フレーム main で実行**していた。

- 症状: Scene 画面(Flight 等)で main 飽和(実測 Release 112%/Debug は 1 フレーム 3.25 秒)=**全 UI のかくつき・タップ不応答(ANR)**、退場時の Stop/Dispose 競合で **SIGSEGV**(fault addr 0x48。9/5 に 4 連発)
- 修正: ①`ConfigureAwait(false)` でループをスレッドプール化 ②`Stop()` がループ Task の完了を待ってから CTS を破棄(進行中フレームと Dispose の競合防止) ③ループ稼働中は main の直接描画フォールバックを無効化(共有 SKPaint の 2 スレッド同時使用を根絶)
- **実機検証済み(Debug/Release 両方)**: Flight 表示中 main 128%→28〜36%(描画はワーカー)・Back 即応答・退場後全スレッドアイドル・Scene 4 画面連続入退場でクラッシュ/ANR ゼロ

### 🧹B-2. ReSharper inspectcode 対応(C# 側)

- **機械修正 68 件**: 末尾カンマ削除 41 / `async`→Task 直返し 11(`HttpService` 全 API+`NetworkScpViewModel`)/ 冗長な既定値引数 4 / 空 `default: break;` 3 / 冗長 using 2(`MediaController`=CT.Maui 15 で `MediaElementState` が Core へ移動済み・`MapsuiMapManagers`)/ partial の重複基底型 1(`CalendarView.xaml.cs`)/ 空行 1 ほか
- **個別判断分(ステップバイステップ・都度ユーザー確認)**: `field` キーワード化(`UICalendarViewModel`=`#pragma IDE0032` 撤去)/ null 免罪符→**`ReSharper disable once` へ変更**(`DrawingControl`=Roslyn IDE0370 との板挟み解消。`ImageHelper` は `bitmap!`)/ `MixerEqualizer` の冗長条件 `(peak > 0)` 削除 / `BluetoothSerial` の引数 `adapter`→`bluetoothAdapter` / StyleCop SA1500(field 初期化子構文の誤検知)を `#pragma` 局所抑止
- 未使用代入 17 件は **Debug 計測(`[Conditional]` の `Debug.WriteLine`)でのみ使用のため現状維持で確定**(ユーザー決定)
- **最終残 18 件=全て確定済みの許容**(Debug 計測 17+HighlightTrigger 1)。※この 2 件は区間 9 でさらに方針変更(計測は撤去/抑止・Location は空状態表示へ)したため、残数は次回 inspectcode 実行時に再集計する

## 💡C. この区間のナレッジ

- inspectcode の再実行コマンド(**Bash 系シェルで実行**する。PowerShell は `--properties:` がコロンで分割され「Specify only one solution file」で失敗)。`.sln.DotSettings`(旧 .sln 名)は .slnx 解析にも適用される

  ```bash
  jb inspectcode Template.MobileApp.slnx -f=xml -o=results.xml --no-build --no-swea --properties:Configuration=Release
  ```

- **Release 解析では `[Conditional("DEBUG")]` の `Debug.WriteLine` でのみ使う変数が RedundantAssignment 誤検知**になる(削除すると Debug ビルドが壊れる)
- **ReSharper と Roslyn の nullable 解釈が食い違うことがある**: null 代入を R# だけが指摘し、`!` を付けると Roslyn が IDE0370「抑制は不要」→ 素の null+`// ReSharper disable once` で両立。**StyleCop SA1500 は C# 14 の field 初期化子構文 `} = 値;` を誤検知** → `#pragma` 局所抑止
- `FallbackValue` は「パス不成立(親が null)」のみに効き、**末端プロパティ自体の null には `TargetNullValue`** が必要(どちらも `StringFormat` を通らず素の値が表示される)
- 稀に inspectcode のソースジェネレータ実行が COR_E_APPLICATION 例外で失敗し、CSharpErrors 数百件の不良 run になる → そのまま再実行すれば正常化する
- Scene バグの調査手法: bugreport の ANR trace(main が libSkiaSharp 内)+tombstone 一覧(`am_crash`/`am_anr` は `logcat -b events`)+`top -b -n 1 -H -p` のスレッド別 CPU+「メニューのみ=アイドル / Flight 入場で発症」の二分探索。**Mono アプリは ART 系ダンプ(`am profile` / `kill -3`)が効かない**

---

# 🔙8. fix2 → back — BACK/初期化方式の刷新(白画面対策 B-1 の方式変更)

白画面対策の B-1(Activity 再生成時の初期画面復帰)を、**旧方式(`App.CreateWindow` での復帰)から `StartupState` 方式へ作り直した**区間(2026-09-05 → 09-06)。コミットは 4 本(サブモジュール参照更新 1 本を含む)。原因の詳細・不採用案・実測メモは**区間 6 の B-3** にまとめてある。

## 🖼️A. 画面単位の変更

なし(シェル基盤のみ)。

## 🧱B. 画面以外の変更

### 🚀B-1. 初期画面遷移の `MainPageViewModel` への移設(`StartupState` 方式)

- **初期画面への遷移を `App.OnStart` から `MainPageViewModel.OnCreated` へ移す**。`MainPage` は `Window` 生成のたびに作り直され、`MainPage.xaml` の `s:AppLifecycleBehavior` が `Window.Created` で `IAppLifecycle.OnCreated()` を呼ぶため、**Activity の作り直しのたびに必ず走る**(復帰時は `Resumed` なので呼ばれない)
- 起動時の初期化(DB 再構築・クラッシュレポート表示)は `App` に残し、完了を **`State/StartupState.cs`(新規)** へ通知。`TaskCompletionSource` を隠して `Completed` / `NotifyCompleted()` だけを公開し、**完了後に待ち始めても即座に返る**ため、作り直しで生成し直された ViewModel でも取りこぼさない
- 旧方式(`App.CreateWindow` で 2 回目以降の `Window.Created` を拾い `RestoreInitialViewAsync`)は撤去。`windowCreated` フラグ・`CurrentViewId` ガード・専用ログ 2 件が不要になり、`App.CreateWindow` は素の実装へ戻った
- `OnDestroying` で立てる `destroying` フラグで、初期化が終わる前に作り直された場合に新旧 ViewModel が二重に遷移するのを防止
- 新方式の他テンプレートへの展開状況は区間 6 B-3 の表を参照(`template-maui` は反映不要)

### 🚀B-2. `ApplicationInitializer` の廃止(初期化の `App` への集約)

`ApplicationInitializer`(`IMauiInitializeService`・103 行)を削除し、DB 初期化は `App.OnStart` 内の `InitializeDataAsync()` へ移動。`MauiProgram` に `StartupState` の Singleton 登録を追加。

### 🧹B-3. 細かな警告整理

アナライザ抑止の追加(`BasicStyleViewModel`=IDE0028 / `CollectionGroup`=CA1000 等)とコレクション式化(`[.. items]`)など少量の機械的整理(`LineReaderWriter` / `TreeMapNode` / `DeviceNfcViewModel` / `SampleCvLocalViewModel` / `ViewCollectionViewModel`)。

## 💡C. この区間のナレッジ

- `Application.SendStart()` は `_isStarted` ガードで**プロセス内 1 回のみ**。プロセス生存中の Activity 再生成では `App.OnStart()` が再実行されないため、「作り直しのたびに走ってほしい初期遷移」は `Window.Created` 起点(`AppLifecycleBehavior` → `OnCreated`)へ置く
- 単発のイベントバス(`IReactiveMessenger`)は `Subject<T>` 実装で**リプレイしない**ため、再生成後に購読しても通知が来ず白画面に戻る → 完了状態の受け渡しは `TaskCompletionSource` を包んだ状態クラス(`StartupState`)で行う
- 既知の副作用: `Navigator.Exit()` は `Controller` を経由せず `provider.CloseView` を直接呼ぶため `plugin.OnClose` が走らない(= `ScopePlugin` の参照カウントが減らない)。本アプリで `[Scope]` を使うのは Navigation > Wizard の 3 画面のみで、影響は「作り直し後に Wizard の入力値が残る」程度

---

# 📅9. back → fix3 — Calendar / Location の手直し

## 🖼️A. 画面単位の変更

### 📍A-1. DeviceLocationView — 未取得表示を「空状態パネル」へ変更

区間 7 で入れた `FallbackValue='-'` / `TargetNullValue='-'` 方式を撤回し、**測位待ちは専用の空状態表示(Acquiring location... パネル+Pulse アニメーション)へデザイン変更**。`Location` の null 判定(`NullToBoolConverter`)で空状態と測位結果を切り替える。

### 📅A-2. UICalendar — Debug 計測の撤去

- `MonthViewBuilder.cs` / `UICalendarViewModel.cs`: `Stopwatch`+`Debug.WriteLine` の計測コードを**撤去**(区間 7 の 6-3 で「現状維持」とした判断を変更)
- `CalendarView.xaml.cs`(旧 XAML 版): 計測は残し `// ReSharper disable RedundantAssignment` コメントで抑止(※この旧版は区間 10 で削除)
- あわせて `UICalendarViewModel` の SA1500 `#pragma` を `FirstDayOfWeek` プロパティ全体を囲む位置へ調整

## 🧱B. 画面以外の変更

なし。

---

# 🚧10. fix3 以降(次のタグまでの変更)

### 🗄️`Usa.Smart.Data.Accessor` への移行(2026-09-07)

`[DataAccessor]` 付き partial class の partial メソッドをソースジェネレータが ADO.NET コードへ展開する方式(3.0.0-beta11)へ移行し、`Usa.Smart.Data.Mapper` / `.Builders` を撤去した。

| 対象 | 内容 |
|---|---|
| `Services/DataAccessor.cs`(新規) | 17 メソッド。CRUD は Builder 属性(`[Insert]` / `[Update]` / `[Delete]` / `[SelectSingle]` / `[Count]`。テーブル名はエンティティクラスの `[Name]` から解決)、それ以外は `Services/Sql/DataAccessor.{メソッド名}.sql`(6 本: PRAGMA 3 文と CREATE TABLE 3 文は `;` 区切りで各 1 ファイル、ORDER BY 付き SELECT 2 本、採番付き INSERT、UPDATE)。初期化(`ExecutePragmaAsync` / `CreateTablesAsync`)は `DbConnection` 引数、トランザクション系(`InsertBulkData` / `InsertWorkAsync`)は `DbTransaction` 引数で呼び出し側の接続を使う |
| `Services/DataService.cs` | 公開シグネチャは維持し、DI から受け取る `IDbProvider` / `DataAccessor` に委譲するラッパに変更。`SQLITE_CONSTRAINT` の判定と `UsingTx` / `UsingTxAsync` によるトランザクションはこちらに残す。`ReplaceWorkEnumerableAsync`(全件削除 + 追加を 1 トランザクションで行う洗い替え)を追加。`QueryAllBulkDataList` の戻り値は `IReadOnlyList<T>` |
| `Models/Entity/*.cs` | クラスに `[Name("Data")]` / `[Name("BulkData")]` / `[Name("Work")]`(Builder のテーブル名)。`[PrimaryKey]` → `[Key]`(複合キーは `[Key(n)]`)。`DataEntity.CreateAt` に `[TypeHandler(typeof(DateTimeTicksConverter))]` |
| `Helpers/Data/DateTimeTicksConverter.cs`(新規) | `IValueConverter<long, DateTime>`(UTC ticks で保存。旧 `DateTimeTypeHandler` と同じ変換) |
| 削除 | `Helpers/Data/SqlHelper.cs`(リフレクションによる DDL 生成)、`DateTimeTypeHandler.cs`、`GuidTypeHandler.cs`(Guid プロパティを持つエンティティが無く未使用) |
| `MauiProgram.cs` / csproj | `IDbProvider`(`DelegateDbProvider`。DB のパスは `IStorageManager` から組み立てて接続文字列にする。`DataServiceOptions` は廃止し、`DataService.RebuildAsync` が削除するファイルのパスは `IDbProvider.CreateConnection()` の `DataSource` から得る)を DI 登録。`DataAccessor` の登録は `[DataAccessorRegistration]` を付けた partial メソッド `AddDataAccessors`(3.0.0-beta11 のソースジェネレータが `AddSingleton<DataAccessor>(services, static p => new DataAccessor(p.GetRequiredService<IDbProvider>()))` の実装を生成)で行い、`services.AddDataAccessors()` を呼ぶ。`SqlMapperConfig` の TypeHandler 登録を削除。`Usa.Smart.Data.Mapper` / `.Builders` の `PackageReference` と `TrimmerRootAssembly` を削除。`Usa.Smart.Data.Accessor.Extensions.DependencyInjection` は使用しない |

**実装上の注意(ライブラリの動作)**
- SQL ファイル名は `{クラス名}.{メソッド名}.sql`(無い場合は SDA0401)。3.0.0-beta9 からは `Async` サフィックスを除いた名前でも一致する(beta8 までは完全一致)。`**/Sql/*.sql` はパッケージの targets が `AdditionalFiles` として自動登録するため csproj の変更は不要
- `[Sql]` によるインライン SQL は使わず、必ず `.sql` ファイルにする
- 1 ファイル内の `;` 区切りの複数文はそのまま 1 つの `CommandText` になり、`Microsoft.Data.Sqlite` が順に実行する(`PRAGMA journal_mode=WAL` の戻り行は `ExecuteNonQuery` で無視される)
- `[Select]` Builder は ORDER BY を付けない(必要なら SQL ファイル)。`[Delete]` Builder は引数なしだと WHERE なしの全件削除になる。Builder のテーブル名は `Table = "..."` → エンティティクラスの `[Name]`(3.0.0-beta10 から) → クラス名(`Entity` サフィックスは除去されない)の順で決まる
- `[TypeHandler]` はエンティティのプロパティ単位で、Builder の INSERT(`ToDb`)と行マッピング(`FromDb`)の両方に適用される
- 生成コードは `-p:EmitCompilerGeneratedFiles=true` で `obj/Debug/net10.0-android/generated/Smart.Data.Accessor.Generator/` に出力して確認できる(通常ビルドでは削除される)

- ビルド 0 エラー 0 警告。実機(Pixel 9a)で確認: 起動時の DB 再構築(PRAGMA / DDL の複数文実行)、Data 画面の Insert(重複時は Key duplicate)・Query(`CreateAt` の UTC ticks 往復)・Update・Delete・BulkInsert 10,000 件(461ms)・QueryAll・DeleteAll、Navigation > Edit の一覧・新規(採番 #5)・更新・削除。端末から取り出した DB ファイルを直接読んで行の状態も確認
- `[Name]` への置き換えと `DataServiceOptions` 廃止後(3.0.0-beta10)も実機で再確認: 生成 SQL のテーブル名が `"Data"` / `"BulkData"` / `"Work"` になること、起動時の DB 再構築(旧ファイルの削除と再作成、Work に Sample-1〜4)、Data 画面の Insert・Query・Update・Delete・BulkInsert・DeleteAll、Edit 画面の一覧
- `[DataAccessorRegistration]` の生成メソッドへ切り替え後(3.0.0-beta11)も実機で再確認: 生成された `MauiProgram.Registration.g.cs` の登録内容、BunnyTail の生成ファクトリ(`DataService`)が `DataAccessor` を依存として解決すること、起動時の DB 再構築、Data 画面の Insert・Query・BulkInsert・DeleteAll、Edit 画面の一覧

### 📦`Usa.Smart.Mapper` の採用(2026-09-07)

`[Mapper]` 付き `static partial` メソッドをソースジェネレータが展開する方式(1.0.0-beta8)。マッパーは `Models/ObjectMapper.cs` に集約する。

| 対象 | 内容 |
|---|---|
| `Models/ObjectMapper.cs` | `Map(SwitchBotTemperature source, SwitchBotTemperature destination)`(既存インスタンスへの上書き)と `ToWorkEntity(DataListResponseEntry source)`(生成。Id は int→long の暗黙変換)。属性指定なしの同名自動マッピングのみ |
| `Models/Sample/SwitchBotTemperature.cs` / `Modules/Device/DeviceBleScanViewModel.cs` | 手書きの `CopyTo` 拡張メソッドを削除し `ObjectMapper.Map(data, current)` へ |
| `Usecase/NetworkUsecase.cs` | `GetDataListAsync` で取得した一覧を `ToWorkEntity` で変換し、Work テーブルを洗い替え(全件削除 + 追加)してから件数をダイアログ表示(`DataService` を注入) |
| `Services/DataService.cs` | `ReplaceWorkEnumerableAsync`(全件削除 + 追加を 1 トランザクションで実行) |

- ジェネレータは `this` 付きの拡張メソッド形には対応していない(通常の static メソッドとして呼ぶ)
- 生成コードは `-p:EmitCompilerGeneratedFiles=true` で `obj/.../generated/` に出力して確認(インクリメンタルビルドでは出力されないためソースの更新が必要)。`Map` は 6 プロパティの代入、`ToWorkEntity` は `new WorkEntity()` + Id/Name の代入
- ビルド 0 エラー 0 警告。実機(Pixel 9a)で `WorkMauiServer` をローカル起動(`dotnet run --no-launch-profile --urls http://127.0.0.1:5000`)+ `adb reverse tcp:5000 tcp:5000` + 端末の `ApiEndPoint` を `http://localhost:5000/` にして Network > Data list を実行 → 「count=[10] Saved to Work table.」→ Navigation > Edit の一覧に Data-1〜Data-10 を確認。`Map` は生成コードが旧 `CopyTo` と同一であることで確認

### ✨遷移効果(Effect)デモの追加(2026-09-07)

`Usa.Smart.Navigation.Maui` の Effect 機構(`IMauiNavigationEffect` + `NavigationParameter.WithEffect`)によるアニメーション付き画面遷移を Navigation 配下の新規 3 画面で示す。既存画面は変更しない。

| 対象 | 内容 |
|---|---|
| `Modules/Navigation/Effect/EffectMenuView(+ViewModel)` | 効果選択メニュー(9 段×2 列)。標準 6 効果(Forward/Back/Push/Pop/Fade/None)とアプリ定義 4 効果(Zoom/Drop/Flip/Rotate)は `ForwardAsync(Demo, WithEffect(key))`、Push (Stack)/Fade (Stack) は `PushAsync`、Dialog (Plugin) は効果未指定の `ForwardAsync` |
| `Modules/Navigation/Effect/EffectDemoView(+ViewModel)` | 再生した効果・戻りの効果・遷移種別(Forward / Push (stacked))を表示。Back は対になる効果(`AppEffect.Reverse`: Forward↔Back / Push↔Pop / 他は同一)で `PopAsync` または `ForwardAsync`、Replay は同じ効果で自分自身へ Forward |
| `Modules/Navigation/Effect/EffectDialogView(+ViewModel)` | `[DialogView]` 属性付きのダイアログ風画面。呼び出し側は効果を指定せず、Plugin が付与した効果名を表示 |
| `Extender/Effects/AppEffect.cs` | 効果キー定数(Zoom/Drop/Flip/Rotate/DialogOpen/DialogClose)+ `Reverse()` |
| `Extender/Effects/{Zoom,Drop,Flip,Rotate,Dialog}Effect.cs` | `IMauiNavigationEffect` 実装。Zoom=拡大+フェード、Drop=上から落下(`Easing.BounceOut`)/上へ抜け、Flip=`RotationY` の 3D 回転、Rotate=回転+フェード、DialogEffect=開き側/閉じ側の片側効果(`open` 引数)。いずれも 4 フェーズ(Open/Close/Activate/Deactivate)対応 |
| `Extender/Effects/NavigationProviderOptionsExtensions.cs` | `RegisterAppEffects()` でアプリ定義 6 効果を登録 |
| `Extender/DialogEffectPlugin.cs` / `Extender/DialogViewAttribute.cs` | `PluginBase.OnPrepareParameter` で効果未指定時に `[DialogView]` 付き ViewId への遷移→DialogOpen、からの遷移→DialogClose を自動付与。判定用の ViewId 集合は `ViewSource()` から属性で抽出 |
| `MauiProgram.cs` | `UseMauiNavigationProvider(static options => options.RegisterAppEffects())`、`AddPlugin(new DialogEffectPlugin(ViewSource()))` |
| `MainPage.xaml` | コンテナの `AbsoluteLayout` に `IsClippedToBounds="True"`(効果再生中のビューをヘッダ/フッタへ重ねない。MAUI の Layout は既定でクリップしない) |
| `Modules/ViewId.cs` / `NavigationMenuView.xaml` | `NavigationEffectMenu/Demo/Dialog` を追加、Navigation メニュー 6 行目に「🍒 Effect」 |

**実装上の注意(ライブラリの動作)**
- `ForwardStrategy.UpdateStackAsync` は Open(新)と Close(旧)を `Task.WhenAll` で同時再生し、新ビューは `Children.Add` で常に手前になる。閉じる側だけが動く効果(DialogClose)は再生前に `ZIndex = 1` へ上げる
- 表示値の反映は `OnNavigatingToAsync` で行う(`OnNavigatedToAsync` はアニメーション完了後に呼ばれる)。`Navigator` は `CreateView` 時に注入済み
- スタック上にあるかの判定は `context.Attribute.IsStacked() || Navigator.StackedCount > 1`(Push で積まれる場合と、積まれた状態からの Forward(Replay)の両方)。Replay 後の Back も `PopAsync` になりスタックにビューが残らない
- 連打による二重遷移は起きない。footer ボタンは `MakeAsyncCommand` の BusyState 連動で遷移中は BusyOverlay がタップを吸収、ハードウェア Back は `BusyState.IsBusy` で抑止、アニメ中のビューは Provider が `InputTransparent` にする
- 標準の Slide 系効果(Forward/Back/Push/Pop)は `Usa.Smart.Navigation.Maui` 3.9.0 では Open/Close のみ対応(Push (Stack) では遷移元が即座に消え、Pop では遷移先が即座に現れる)。`Smart-Net-Navigation` 側で MAUI/WPF/Avalonia の Slide 効果を 4 フェーズ化し、3.10.0 へ更新した実機で Pop 時に遷移先が上からスライドインすることを確認済み

- ビルド 0 エラー 0 警告。実機(Pixel 9a)で 13 ボタン全経路(標準 6 / 独自 4 / Stack 2 / Plugin 1)+ Replay(通常・スタック時)+ card Back / footer Back / ハードウェア Back を確認。4 フェーズ化版の Slide は修正版 DLL の直接参照で Pop 時に復帰側が上からスライドインすることを確認済み

### 📦`Usa.Smart.Results` の採用(2026-09-06)

**参照だけあって未使用だった `Usa.Smart.Results` 2.2.0 を実際に使うようにし、自前の劣化版を撤去した**。

| 対象 | 変更 |
|---|---|
| `Models/Result.cs` | **削除**。自前の `IResult<T>` + `Result.Success/Failed`(失敗理由を持てない実装)を撤去 |
| `GlobalUsing.cs` | `global using Smart.Results;` を追加(`Smart.Reactive` と `Smart.Text` の間) |
| `Usecase/NetworkOperator.cs` | 戻り値を `IResult<T>` → `Result<T>` へ。**`sealed record NetworkError(NetworkErrorKind Kind, HttpStatusCode Status) : Error(...)` を新設**し、従来 `Result.Failed<T>()` で**捨てていた失敗理由(種別・ステータス)を呼び出し側へ伝えるようにした**。応答自体が無い場合(ネットワーク未接続)は `Result.Failure<T>("Network is unavailable.")` で string→Error の暗黙変換を利用 |
| `Usecase/NetworkUsecase.cs` | `ValueTask<IResult<object>>` → `ValueTask<Result<object>>`(`IsSuccess`/`Value` の呼び出しは不変) |
| `Models/App/ExpressionCalculator.cs` | `Evaluate` の戻り値を `Result<double>` へ。`readonly record struct CalculationResult(bool, double, string)` は削除。失敗は `Result.Failure<double>("式が空です")` のようにメッセージのみで返す |
| `Modules/App/AppCalcViewModel.cs` | `result.Success`/`result.Error` → `result.IsSuccess`/`result.Error.Message`。`[MemberNotNullWhen(false)]` により else 節で `Error` が非 null 確定になるため `!` は不要 |
| `Graphics/Drawing/CropDrawing.cs` / `Modules/Sample/SampleCropViewModel.cs` | `(int Width, int Height)` の `(0,0)` センチネル返却を `Result<(int Width, int Height)>` へ。消費側は `TryGetValue(out var size)` でマジックナンバー判定(`if (width == 0)`)を排除 |

- **名前衝突の解消が前提だった**: `global using Template.MobileApp.Models;` があるため、自前の `Result` を残したまま `global using Smart.Results;` を足すと CS0104 になる。`Models/Result.cs` の削除とセットで実施
- 対象外として確定: `SettingParser.TryGetXxx`(BCL 慣習)/ `LineReaderWriter.TryReadLine`(`ref` ホットパス)/ 成否を含まない多値タプル / `ExpressionCalculator` 内部の例外(境界で結果型に変換済み)
- `Services/ScpService.cs` の `ScpTransferResult` は対応不要(`ServerFingerprint` を成功・失敗の両方で返す構造のため `Result<T>` に嵌まらない)。現状の record のまま維持する
- ビルド 0 エラー 0 警告。**実機確認済み**: 電卓の成功(`2+3×4`→`14`)/ 失敗(「式が不完全です」)、Crop 書き出し(`143 x 134 px`)、Network の失敗経路(タイムアウト → エラーダイアログ)と成功経路(ローカル起動した `WorkMauiServer` + `adb reverse` で「Get success. time=[...]」。2026-09-07 確認)

### 📅旧 CalendarView の廃止と CalendarView2 のリネーム(C-14+D19 の実施。2026-09-06)

- **旧 XAML 版 `Controls/CalendarView.xaml(.cs)`(未参照 1,490 行)を削除**し、**Skia 自前描画版 `CalendarView2` を `CalendarView` へリネーム**(git mv。クラス名 / `x:Class` / `typeof` 参照など 70 箇所を置換)
- `UICalendarView.xaml`: タグを `controls:CalendarView` へ変更し、「タグ名を変えるだけで従来版へ切り替えられる(未決定)」の切替コメントを実態(一本化済み)へ合わせた
- `CalendarSelectionMode` 等の共有型は独立ファイルのため影響なし。ビルド警告ゼロ・実機で表示 / 月送り / イベント / 選択モードバーの動作確認済み

### 🎨Grid の Spacing を Style へ集約(2026-09-07)

XAML の `Grid` に直接書いていた `RowSpacing` / `ColumnSpacing` を全廃し、画面ローカルの `Style`(`TargetType="Grid"`)の `Setter` へ移した。**27 ファイル・39 箇所**。

| 対象 | 追加したスタイルキー |
|---|---|
| `Controls/` | `InfoCard.xaml` = `HeaderGrid` / `CalendarView.xaml` = `CalendarRootGrid`(いずれもローカルの `ResourceDictionary` を新設) |
| `Modules/Basic/` | `BasicBehaviorView` = `SwitchRowGrid` / `BasicStyleView` = `SegmentRowGrid` |
| `Modules/Sample/` | `SampleChartView` = `SegmentGrid` / `SampleChatView` = `ExtractItemGrid` / `SampleCropView` = `ExportResultGrid` / `SampleMediaView` = `ControlBarGrid` |
| `Modules/View/` | `ViewCarouselView` = `CarouselCardGrid` / `ViewCustomView` = `ColorPreviewGrid` / `ViewDragDropView` = `BoardGrid` / `ViewEasingView` = `RootGrid` / `ViewEffectView` = `EffectRowGrid`(5 箇所で共用) / `ViewStateView` = `StateButtonGrid` |
| `Modules/Device/` | `DeviceSensorView` = `SensorRowGrid`(Compass / Level の 2 箇所で共用) |
| `Modules/Network/` | `NetworkScpView` = `TransferButtonGrid` |
| `Modules/UI/` | `UICalendarView` = `RootGrid` / `ModeBarGrid`、`UIGraphView` = `GraphRowGrid`、`UIGraph2View` = `CommitInfoGrid`、`UIKitNotifyView` = `NotifyRowGrid`、`UISuperView` = `SearchRowGrid`、`UIKitDashView` = 既存 `KitDashHeaderGrid` に Setter を追加 |
| `Modules/Navigation/` | `EditListView` = `WorkRowGrid` / `SelectionBarGrid`、`EffectDemoView` = `InfoGrid` / `ButtonRowGrid`、`EffectDialogView` = `InfoGrid` |
| `Modules/Toolkit/` | `ToolkitChartView` = `FunnelGrid` / `SparkGrid` |

- `RowDefinitions` / `ColumnDefinitions` は従来どおり要素側にインライン記述する。`Margin` / `Padding` / `BackgroundColor` / `HeightRequest` も移していない
- 共有スタイル(`Resources/Styles/Styles.xaml`)は変更しない。同じ値を複数箇所で使う画面は 1 キーを共用する
- ビルド 0 エラー 0 警告。実機(Pixel 9a)で対象画面を巡回して表示崩れと `StaticResource` 解決エラーが無いことを確認

### 🧰`ILayoutManagerFactory` の撤去(2026-09-07)

`CascadeStackLayout` を `Layout` 派生に変更し、`CreateLayoutManager()` の override でカスケード配置を返すようにした(`CircularLayout` / `StaggeredGrid` と同じ形)。

| 対象 | 内容 |
|---|---|
| `Controls/CascadeStackLayout.cs`(新規) | `Layout` 派生。`CreateLayoutManager()` で入れ子の `CascadeLayoutManager` を返す。ずらす量は `Offset` 依存プロパティ(既定 20) |
| `Layouts/AppLayoutManagerFactory.cs`(削除) | `ILayoutManagerFactory` 実装と `VerticalStackLayout` 派生の `CascadeStackLayout`、`CascadeLayoutManager` |
| `MauiProgram.cs` | `UseCustomLayouts()` とその中の `AddSingleton<ILayoutManagerFactory, AppLayoutManagerFactory>()` を削除 |
| `Controls/CircularLayout.cs` / `Controls/StaggeredGrid.cs`(移動) | `Layouts/` を廃止し `Controls/` へ統合。名前空間は `Template.MobileApp.Controls` |
| `Modules/View/ViewLayoutView.xaml` | カードのタイトルと説明を新しい仕組みに合わせ、`layouts:` 名前空間を廃止して既存の `controls:` に統一 |

- `ILayoutManagerFactory` は「自分で継承できない型(標準の `Grid` / `StackLayout` や他社ライブラリのレイアウト)のマネージャを差し替える」ためのフックで、自作レイアウトには不要。MAUI の `Layout.LayoutManager` は `GetLayoutManagerFromFactory(this) ?? CreateLayoutManager()` の順で解決する
- ビルド 0 エラー 0 警告。実機(Pixel 9a)で View > Layout の CircularLayout / StaggeredGrid / CascadeStackLayout の 3 カードを確認

### 🔣アイコンの定義集約とプリロード(2026-09-08 / 2026-09-09 再構成)

初回表示の画面でボタンのアイコンが遅れて入り、文字位置が動いて見える問題への対処。あわせて XAML の指定と温める対象の二重管理をやめる。

| 対象 | 内容 |
|---|---|
| `Markup/AppIcons.cs`(新規) | XAML から `x:Static` で参照する `FontImageSource` を 1 つずつ生成して保持する。生成時に内部の一覧へ登録し、`WarmStartupAsync` / `WarmAllAsync` はその一覧をそのまま温める。`WarmTypefaces` は `IFontManager.GetTypeface` で Material / FluentUI の Typeface を生成する。色は `Application.Current.Resources` から解決する |
| `Markup/FontIconExtensions.cs`(削除) | `MenuIcon` / `MoneyIcon` / `Material` / `Fluent` のマークアップ拡張。参照が 0 になったため削除 |
| `App.xaml.cs` | DB 初期化の前に `WarmTypefaces` と `WarmStartupAsync`、`StartupState.NotifyCompleted()` の後に `WarmAllAsync` を呼ぶ。所要時間を Debug ログへ出力 |
| `Log.cs` | `DebugFontWarmup` を追加 |
| XAML 21 ファイル | `{markup:MenuIcon ...}` 等 178 箇所を `{x:Static markup:AppIcons.Xxx}` へ置換。`fonts:` の参照が無くなったファイルは `xmlns:fonts` も削除 |

対象は XAML がアイコンとして参照している全 162 個。全て MaterialIcons のグリフで、サイズと色の組み合わせ毎に別インスタンスにする(ビットマップのキャッシュキーにサイズと色が含まれるため)。

| 指定 | 個数 | 温めるタイミング |
|---|---|---|
| Material / 24 / White のうち MenuView 分 | 11 | 初期表示前 |
| Material / 24 / White の残り | 109 | 初期表示後 |
| Material / 18 / BlueGrayDarken1 | 17 | 初期表示後 |
| Material / 28 / White | 12 | 初期表示後 |
| Material / 24 / GrayDarken3 | 4 | 初期表示後 |
| Material / 36 / White | 2 | 初期表示後 |
| Material / 18 と 24 の個別色(Red / Green / Blue / Gray / LightBlue) | 7 | 初期表示後 |

- 命名は 24/White がグリフ名そのまま、28/White が `Money`、18/BlueGrayDarken1 が `Small`、36/White が `Large` の接頭辞。個別色のものは用途名(`ValidationError`、`MapHome`、`LoginVisibility` 等)
- フィールド名にアンダースコアは使えない(CA1707 / SA1310)。`MaterialIcons` は `// <auto-generated />` のため対象外になっている
- `Button.ImageSource`(`FontImageSource`)は `ImageSourcePartLoader.UpdateImageSourceAsync` が await を挟むため**最初のレイアウトに間に合わない**。アイコンが後から入ると `ContentLayout="Top"` の構成が変わり、文字が下へ動く
- **グリフのビットマップはキャッシュされる**。同じ集合を続けて 2 回読むと 2 回目は 134ms → 11ms になる。キャッシュのキーはインスタンスではないので、インスタンス共有による速度上の効果は無い
- Button 側は `ImageSourcePartExtensions.UpdateSourceAsync` から `GetDrawableAsync(imageSource, context)` を呼ぶ。温め側も同じ経路を使う
- 読み込み状態(`ImageSourceServiceResultManager`)は Button のハンドラ毎に持つため、1 つの `FontImageSource` を複数のボタンや複数の画面で共有しても競合しない
- FluentUI は `Label.Text` でのみ使っており `ImageSource` には無いため、温めるグリフは Material だけ。Typeface は両方生成する
- グリフの要求は `Task.WhenAll` で並列に投げ、初期表示後の分は 16 件毎に区切って UI スレッドを長く占有しないようにする
- 実測(Pixel 9a、3 回)

| 段階 | 内容 | 実測 |
|---|---|---|
| Typeface | 2 フォント + `AppIcons` の静的初期化 | 19ms(起動をブロック) |
| glyph(startup) | 11 個 | 113〜117ms(起動をブロック) |
| glyph(rest) | 162 個(うち 11 はキャッシュ済み) | 249〜258ms(初期表示の後ろで実行) |

- glyph(startup) の大半は経路構築の固定費で、個数にはあまり依存しない。ただし画像のディスクキャッシュが溜まると伸びる(同じ 11 個で 116ms → 170ms まで観測。`pm clear` 後は 116ms に戻る)
- Menu 表示直後にタップしても遷移は遅くならない(tap→Navigated 225 / 262 / 265ms。ブロックしない場合の基準値は 278〜290ms)
- 根治する場合は `Button.ImageSource` をやめ、アイコン用とテキスト用の `Label` を並べる構成にする(未実施)
- ビルド 0 エラー 0 警告。実機で Menu / Device > Misc(同じインスタンスを 2 ボタンで使用・再訪問)/ Basic > Validation / UI Login(`Value` セッターでの差し替え)/ UI Money / Sample > Map の表示を確認

### 📋メニュー画面のアイコン追加(2026-09-09)

アイコンの無かったメニュー画面にアイコンを付け、`Button.ImageSource` の指定を全メニューで揃える。

| 画面 | 追加数 |
|---|---|
| `Modules/Device/DeviceMenuView.xaml` | 18 |
| `Modules/View/ViewMenuView.xaml` | 17 |
| `Modules/Network/NetworkMenuView.xaml` | 11 |
| `Modules/Basic/BasicMenuView.xaml` | 9 |
| `Modules/App/AppMenuView.xaml` | 2 |
| `Modules/Sample/SampleMenuView.xaml` | 1(Sf Chart のみ未設定だった) |

- スタイルは `MenuButton` から `MenuIconButton` へ変更。アイコンの指定は Material / 24 / White
- 可視の無効ボタン(空セル)は対象外。`DeviceMenu` の WiFi / Biometric は項目名を持つ無効ボタンなのでアイコンを付ける
- `Modules/Navigation/NavigationMenuView.xaml` はラベル先頭の果物の絵文字をそのまま使うため対象外
- 追加分もウォームアップ対象に含める(Material / 24 / White が 68 → 120 個)
- ビルド 0 エラー 0 警告。実機で Basic / Device / Network / View / App の表示を確認

### 👆フッターボタンのタッチフィードバックの終端(2026-09-08)

UI 1 と UI 2 を相互に行き来したとき、2 画面目の表示が遅く見える問題への対処。

| 対象 | 内容 |
|---|---|
| `Extender/NavigationFeedbackPlugin.cs`(新規) | `OnNavigatedTo` でページのプラットフォームビューへ `JumpDrawablesToCurrentState()` を呼び、実行中のタッチフィードバックを終端する(`ViewGroup` は子孫へ伝播する) |
| `MauiProgram.cs` | `AddPlugin<NavigationFeedbackPlugin>()` を追加 |

- フッターのボタンは `MainPage.xaml` にあり、ページ差し替えを跨いで生存する。ページ内のボタンは遷移で破棄されるためリップルもそこで止まるが、フッターボタンは**新しい画面の上で再生が続いていた**。UI 1 と UI 2 の相互遷移はフッター経由しか無いため、2 画面目だけが遅く見えていた。`Menu` は `FunctionVisible="False"` でフッターごと消えるため元から発生しない
- 遷移完了までの時間は変わらない。ページ生成で UI スレッドが約 200ms ブロックされ、タッチフィードバックの開始もその分遅れる(遷移のたびに遷移先を作り直しており、UI メニュー 1 画面あたり `MaterialButton` を 27 個生成する)
- 実測(Pixel 9a、UI 1 → UI 2): リップルの描画が tap+196ms〜1032ms から tap+168ms〜352ms になり、ページ内ボタンのタップ(tap+197ms〜385ms)と同じ長さになった

タップから最後のフレームまでの時間。

| 遷移 | 変更前 | 変更後 |
|---|---|---|
| Menu → UI 1 | 292ms | 341ms |
| Menu → UI 2 | 268ms | 244ms |
| UI 1 → UI 2 | 899ms | 439ms |
| UI 2 → UI 1 | 905ms | 431ms |
| UI 1 → Menu | 242ms | 200ms |
| UI 2 → Menu | 256ms | 197ms |

- ビルド 0 エラー 0 警告。実機で Menu / UI 1 / UI 2 / UILogin / BasicMenu の遷移を確認

### 🧰外部リファレンス評価 第2弾 N1 — 自作レイアウト / コントロールの追加(2026-09-10)

`Document/Reference_Nova_Nalu.md`(Nova.Avalonia.UI / Nalu の評価)の N1-1〜N1-4。Nova.Avalonia.UI のパネル / コントロールを MAUI の `Layout` + `ILayoutManager` / `ContentView` で自作した。新規 NuGet なし。

| 対象 | 内容 |
|---|---|
| `Controls/OverlapPanel.cs`(新規) | 子を `OffsetX` / `OffsetY` でずらして重ねる `Layout`。`ReverseZIndex=true` で先頭の子が最前面(子の `ZIndex` を `OnAdd` / `OnInsert` / `OnRemove` / `OnUpdate` で振り直す)。負のオフセットは逆方向へずらす |
| `Controls/VariableSizeWrapPanel.cs`(新規) | 列数固定のタイルグリッド。添付 `ColumnSpan` / `RowSpan`、`Columns` / `RowHeight`(NaN で行毎に子の高さから自動)/ `Spacing`。空きセルは先頭から埋め戻す |
| `Controls/CircularLayout.cs` | `StartAngle`(既定 -90)/ `SweepAngle`(既定 360)/ `DistributeEvenly`(既定 true)/ `FitToArc`(既定 false。`Radius` 指定時のみ有効)を追加。既定値では従来の全周配置と同じ |
| `Controls/AvatarGroup.cs`(新規) | `OverlapPanel` ベースの重ねアバター。`ItemsSource`(画像名 / `ImageSource`)/ `ItemTemplate` / `MaxDisplayed` / `Overlap` / `ShowCount` / `AvatarSize` / `StrokeColor` / `CountBackgroundColor` / `CountTextColor`。超過分は「+N」、`INotifyCollectionChanged` の増減に追従 |
| `Controls/CompareSlider.cs`(新規) | `BeforeContent` / `AfterContent` を仕切りのドラッグ(Pan)またはタップで見比べる。`Position`(0〜1、TwoWay)/ `Orientation` / `HandleColor` / `HandleSize`。After は `RectangleGeometry` でクリップ |
| `Modules/UI/UIStreamDetailView.xaml` / VM | 「Friends watching」の重ねアバター(負の `Spacing` + 固定の「+3」)を `controls:AvatarGroup` に置き換え。VM に `Friends`(6 件)。スタイル 5 件を `FriendsAvatars` 1 件に集約 |
| `Modules/UI/UIKitDashView.xaml` / VM | ハートカードと 2×2 のメトリクス(`Metrics[0..3]` の固定インデックス)を `VariableSizeWrapPanel`(Columns=2)1 つに統合。VM は `Tiles`(`UIKitDashHero` + `UIKitDashMetric`×4。`EnterDelay` はモデル側)を `BindableLayout` + `UIKitDashTileTemplateSelector` で流し込む。ハートカードは `ColumnSpan=2`。ヘッダへの -30 の重なりはパネル側の `Margin`(ハート→メトリクス間は 16→12) |
| `Modules/Sample/SampleCvLocalView.xaml` | 撮影後の原画 `Image` + `DetectDrawing` を `CompareSlider`(Before=原画 / After=原画+検出枠)に変更。`DrawingControl` は `InputTransparent` |
| `Modules/View/ViewLayoutView.xaml` | CircularLayout カードに半円(`StartAngle=180` / `SweepAngle=180` / `FitToArc`)の例、`VariableSizeWrapPanel`(Columns=3 で span 混在)/ `OverlapPanel`(カスケードと `ReverseZIndex` の円)のカードを追加(7 カード) |
| `Modules/View/ViewCustomView.xaml` / VM | `AvatarGroup`(Add / Remove で「+N」の変化)/ `CompareSlider`(色フィルタの前後)のカードを追加(6 カード)。VM に `Avatars`(`ObservableCollection`)/ `ComparePosition` / `AddAvatarCommand` / `RemoveAvatarCommand` |

- ビルド 0 エラー 0 警告(Debug)。実機確認は 2026-09-13 に完了。CompareSlider は 2026-09-13 に撤去(後述)

### 🖼️画像アセットの生成と反映(2026-09-11〜12)

Microsoft Foundry の `gpt-image-2` で画像を生成し、`Resources/Images/` の用途別フォルダへ配置してコードの参照・文言・配色を合わせた。画像ごとの一覧と一括確認ギャラリーは `Document/Image_Generation_List.md`、全プロンプトは `Document/Image_Generation_Prompts.md`、非採用画像は `Document/ImageCandidates/`、サムネイルは `Document/Thumbnails/`。

| 対象 | 内容 |
|---|---|
| `Resources/Images/Shop/` | `product_device01〜03.jpg`(900×1200)/ `product_gear01〜06.jpg`(800×800)。PC・ガジェット(ノート PC / モニター / デスクトップ / キーボード / マウス / ヘッドセット / Web カメラ / SSD / USB-C ドック)。旧 `product_apparel` / `product_beauty` から改名 |
| `Resources/Images/Stream/` | `poster01〜06.jpg`(600×900。作品ごとに配色を変えたジャンル別ポスター)/ `stream_hero.jpg`(1600×900)/ `stream_clip01〜03.jpg`(1280×720) |
| `Resources/Images/Profile/` | `profile_cover.jpg`(1536×1024。マイクラ風の夕暮れの街)/ `gallery01〜06.jpg`(1000×1000。旅行 / 料理 / 街並み / 自然 / カフェ / 夜景のイラスト)。`avatar_user.jpg` は現行のまま |
| `Resources/Images/Onboard/` / `Banner/` | `onboard01〜03.jpg`(1080×1080。人物なし)/ `banner01〜03.jpg`(1200×600。モールの宣伝・人物なし) |
| `Resources/Images/Chat/` | `avatar_person01〜05.jpg`(256×256。アニメ少女 / メカヘッド / スライム / マイクラ風 usa7 / 銀髪の少年) |
| `Resources/Images/Character/` | `usa1〜8_full.jpg` をリデザイン版(1024×1024)、`usa1〜8_face.jpg` をマイクラ風(256×256)に差し替え |
| `Resources/Images/Common/social_background.png` | 構図はそのままのブラッシュアップ版(1024×1536) |
| `Resources/Images/Monster/monster01.jpg` | ゼリー猫のモンスター(1000×1000)。使わない候補は `Resources` に含めず `ImageCandidates/monster_c01〜08_*.jpg` に保存。旧 `Pet/pet01〜03.jpg` は削除 |
| 生成対象外 | `login_hero.png`(UILogin は `profile.jpg` を使用)/ `Raw/Social/player.jpg` / `Raw/Avatar/mofusand.jpg` / `avatar_user.jpg` |

| 画面 | 内容 |
|---|---|
| UIShop | 商品 9 件を PC 機器の名称・円価格(`¥179,800` 等)へ。「こんにちは、アンナさん / デスク周りをアップグレード」。商品画像は `AspectFit` + Margin の余白付き中央表示(`PopularImage` / `ItemImage`)。アクセント色 `PinkAccent2` → `BlueDefault`、`PinkLighten5` → `BlueLighten5` |
| UIItem | メカニカルキーボード / ¥19,800 / キーボード / 説明文。サイズタグを「スイッチ 赤軸 / 茶軸 / 青軸」(`SelectedSwitch` / `SwitchCommand`、スタイル `Variant*`)へ。「数量」「カートに入れる」。アクセント色をブルーへ |
| UICart | 明細 3 件(キーボード / マウス / ヘッドセット)。単価は円の整数(`¥{0:N0}`。割引は円未満切り捨て)。ショッピングカート / カートに N 点 / スプリングセール −10% / 適用済み / 小計 / 割引 / 合計 / レジに進む、完了ダイアログも日本語。アクセント色をブルーへ |
| UIMonster(旧 UIPet) | View / VM / `ViewId` / 画像フォルダを Pet → Monster に改名。名前「ぷるにゃ」/ 種族「ゼリーキャット」/ 説明文 / タグ(水タイプ / のんびり / 甘えん坊)/「パーティーに加える」「✓ パーティー参加中」。カードの Padding を Border から内側の VerticalStackLayout へ移し、Heart ボタンの Bounce が左端で切れないようにした |
| UISuper | バナー = オータムセール開催中 / 新作ギフト入荷 / ポイントキャンペーン |
| UIStream / UIStreamDetail | ヒーロー「君の知らない空の果てで」(2024 · SF · 2h 18m / 本日の高評価)。ポスター = 星海のリング / 紅の残響 / 屋上の約束 / キッチン三人組 / 山の記憶(詳細の関連作品に浮遊城の魔導士)、セクション = 高評価 / オリジナル / 急上昇 / アクション & アドベンチャー。詳細のあらすじ・出演・予告編 3 件・フレンド 5 件(`avatar_person01〜05`)を日本語化 |
| UIChat | 送信者 = M･I･O / 日本酒飲郎 / 悪いスライム / †聖天使†、自分。アバターは `avatar_person01〜05` |
| UIProfile | カバー `profile_cover.jpg`、ギャラリー `gallery01〜06.jpg` |
| UIKitDash / UIKitNotify / UIKitSetting / UIKitOnboard / UIKitTracking | 本文を日本語化(おはようございます / うさうさうさん、平均心拍数・歩数・心拍数・消費カロリー・睡眠、注文 #2412 / はじめに、通知 5 件、アカウント / 環境設定、ようこそ / いつでもつながる / さあ、始めよう、スキップ / 始める、注文番号 / 到着予定 / 注文受付〜配達完了)。タイトルは Dashboard / Notifications / Settings / Onboarding / Tracking。Onboarding は `onboard01〜03.jpg` |
| `Document/UI_*.png` | Shop / Item / Cart / Monster(旧 `UI_Pet.png`)/ Super / Stream / Chat / Profile / Onboard / Character / Social / KitDash / KitNotify / KitSetting / KitTracking を実機で撮り直し(1080×2424 RGB PNG) |
| `README.md` | TODO を表形式に(Grid control / WiFi manager / Bottom sheet 等)。Image の一覧に `UI_Monster.png` |

- 生成は `images/generations`(JSON)と `images/edits`(参照画像付き)。目標サイズは生成後に切り出し・縮小して JPEG 品質 80 で保存
- ビルド 0 エラー 0 警告(Debug)。実機で全画面の表示を確認

### 📋UI 1 / UI 2 メニューの再配置と UIFeel の廃止(2026-09-12)

| 対象 | 内容 |
|---|---|
| `Modules/UI/UIMenu1View.xaml` / `UIMenu2View.xaml` | 各 3 列×9 段 → **2 列×9 段**。UI 1 = Profile \| Login / Money \| Super / POS \| Shop / Schedule \| Calendar / Timeline \| − / Mail \| Chat / Kit \| − / Graph \| Graph2 / TreeMap \| −。UI 2 = Stream \| Dock / Load \| Gauge / Meter \| Mixer / Monster \| Wheel / Character \| Social / Radar \| − / Flight \| Tactical / Telemetry \| Energy / − \| −。余りセルは可視の無効ボタン |
| 戻り先 | Graph / Graph2 / TreeMap → UI 1。Dock(Back / Exit)/ Stream / Character / Monster / Social → UI 2 |
| UIFeel | 廃止(View / VM / `ViewId.UIFeel` / `Document/UI_Feel.png` を削除)。hex の花形配置は `Controls/HoneycombLayout` として View > Layout へ |
| `Controls/HoneycombLayout.cs`(新規) | 列を交互に半セルずらし、上のセルから順に埋める `Layout`(`Columns` / `Spacing` / `StaggerEvenColumns`。セルサイズは子の DesiredSize の最大値)。3 列に 7 個で中央 1 + 周囲 6 の花形 |
| `Modules/View/ViewLayoutView.xaml` / VM | 「HoneycombLayout (自作)」カードを CircularLayout の次に追加(8 カード)。VM の `Hexes`(`ViewLayoutHex`: Label / Icon / Fill / Accent / Delay / IsSelected)を `BindableLayout` で表示し、タップで選択(枠 3px + チェックバッジ、中央→外周の Pop 入場)。以降のカードの入場遅延を 1 段ずつ後ろへ |
| `README.md` | Implement の「UI」「UI (Visualization)」行を「UI 1」「UI 2」に変更(メニューと同じ内訳。Feel を除去) |

- ビルド 0 エラー 0 警告(Debug)。実機でメニュー・戻り先・Layout の表示とタップ選択を確認

### 🧹ReSharper 指摘の解消(2026-09-13)

| 対象 | 内容 |
|---|---|
| `Controls/AvatarGroup.cs` / `Controls/CompareSlider.cs` | `BindableProperty.Create` の既定値 `null`(引数の既定値と同じ)を削除(4 件)。`ItemTemplate is not null && ItemTemplate.CreateContent() is View` を `ItemTemplate?.CreateContent() is View view` に統合 |
| `Controls/CircularLayout.cs` | `ComputeExtent` の引数を `double[]` → `IEnumerable<double>` |

- inspectcode(Release)0 件、Debug ビルド 0 警告。Release ビルドに残る 10 件の警告は Android SDK 側(`BluetoothGattServerCallback.OnServiceAdded` の動的登録 ×8、registered dynamically ×2)

### 🐛CompareSlider の撤去と `ReplaceBitmap` の修正(2026-09-13)

| 対象 | 内容 |
|---|---|
| `Controls/CompareSlider.cs` | 削除 |
| `Modules/View/ViewCustomView.xaml` / VM | CompareSlider のカードと `CompareBorder` / `CompareTint` スタイル、VM の `ComparePosition` を削除(5 カード) |
| `Modules/Sample/SampleCvLocalView.xaml` | 撮影後の表示を原画 `Image` + `DetectDrawing` の重ね(N1 以前の形)に戻した |
| `Helpers/ImageHelper.cs` | `ReplaceBitmap` の `old.Dispose()` を `old?.Dispose()` に修正。`SKBitmapImageSource.Bitmap` の初期値は null のため、初回のキャプチャ / 読み込みで `NullReferenceException` になっていた(呼び出し元 8 箇所: Custom Vision(Local)/ CvNet 5 画面 / UI TreeMap の撮影 / View Drawing の保存プレビュー) |

- ビルド 0 エラー 0 警告(Debug)。実機(Pixel 9a)で Custom Vision の Detect → Retry → Detect、TreeMap の Count 2 回、Drawing の Save 2 回を確認(いずれも落ちず、2 回目で表示が更新される)

### 🧰CircularLayout の回転 / リング(第2弾 N3-6。2026-09-13)

| 対象 | 内容 |
|---|---|
| `Controls/CircularLayout.cs` | `RotateItems`(子を接線方向へ回転。子の上端が円の外側を向く。false に戻すと回転を 0 に戻す)/ `ItemAngle`(回転に加える角度)/ `OrbitSpacing`(既定 48)/ 添付 `Orbit`(0 = 中心、1 = 基本の円 = 既定、2 以降 = `Radius + (k - 1) × OrbitSpacing` のリング)を追加。リング毎に `StartAngle` / `SweepAngle` を等分し、自動半径は最外周のリングが領域に収まる大きさにする。`Angle` / `Orbit` の変更で親を再レイアウトする。既定値では従来と同じ配置 |
| `Modules/View/ViewLayoutView.xaml` | 「CircularLayout 回転 / リング」カードを追加(8 カード)。左 = `RotateItems` の扇(5 ピル、`StartAngle=200` / `SweepAngle=140` / `FitToArc`)、右 = 中心 1 + リング 3 + リング 6 の同心円 |

- ビルド 0 エラー 0 警告(Debug)。実機(Pixel 9a)で View > Layout の表示を確認

### 📱ステータスバーの画面追従(2026-09-13)

`MainPage.xaml` の `toolkit:StatusBarBehavior` は起動時に `BlueDefault` + `LightContent` を 1 回適用するだけだったため、ヘッダの無い画面でも青い帯が残っていた。画面が `Title` / `Function` と同じ要領でステータスバーの色とアイコン色を宣言できるようにした。

| 対象 | 内容 |
|---|---|
| `Shell/ShellProperty.cs` | 添付 `StatusBarColor`(`Color?`。null = `MainPage` の既定)/ `StatusBarStyle`(`Default` = `MainPage` の既定、`LightContent` = 白アイコン、`DarkContent` = 黒アイコン)。`UpdateShellControl` で `IShellControl` へ反映(Exited 時は既定へ) |
| `Shell/IShellControl.cs` / `MainPageViewModel.cs` | `NotificationValue<Color?> StatusBarColor` / `NotificationValue<StatusBarStyle> StatusBarStyle` |
| `Shell/ShellUpdateBehavior.cs` | `IShellControl` の 2 値の変更を購読し、ページに付いている `StatusBarBehavior` の `StatusBarColor` / `StatusBarStyle` へ書き込む(既定値は初回に `StatusBarBehavior` の宣言値を取り込む)。Toolkit 側は値の変更で `StatusBar.SetColor` / `SetStyle` を再適用する |
| `MainPage.xaml` | `StatusBarBehavior` の宣言は従来どおり(既定値の置き場) |
| `Modules/UI/UIDockView.xaml` / `UISocialView.xaml` | ヘッダ非表示の 2 画面に `shell:ShellProperty.StatusBarColor`(Dock = `GrayDarken4`、Social = `Black`)を指定 |

- Toolkit の `StatusBarBehavior`(`BasePlatformBehavior`)は付けたページの `BindingContext` を引き継がないため、`StatusBarColor="{Binding ...}"` は解決されず既定の `Transparent` になる(ステータスバーが白地 + 白アイコンで読めなくなる)。値の受け渡しは Behavior 間で行う
- Android 15 以降は `Window.SetStatusBarColor` が効かないため、Toolkit は DecorView の最上部にステータスバーの高さの View を重ねて色を出す(`Transparent` のときだけ `LayoutNoLimits` + `SetDecorFitsSystemWindows(false)` で edge-to-edge)。`IScreen.SetFullscreen(true)` の画面(Dock)でもこの View は残るので、色を背景に合わせる
- ビルド 0 エラー 0 警告(Debug)。実機(Pixel 9a)で Menu = 青、Dock = `#212121`、Social = 黒、Stream = 青(ヘッダあり)を確認し、各画面から戻ると青に復帰する

### 📱Edge-to-Edge / キーボードの確認(2026-09-13)

`MainPage` の `SafeAreaEdges="Default"` とキーボード(ウィンドウ既定の `adjust=pan`)を実機(Pixel 9a / Android 17 / Debug)で確認し、いずれも**現状維持で確定**(付録D D26)。コード変更なし。

| 確認点 | 結果 |
|---|---|
| フッタとナビゲーション領域 | ページの下 Padding = ジェスチャー 63px / 3 ボタン 126px。フッタ(下端 2361 / 2298)はナビ領域と重ならず、余白も出ない |
| `mct:Popup` / `SfBottomSheet` | Popup は中央配置の別ウィンドウ、BottomSheet はページコンテナ内。インセットの影響なし |
| ディスプレイカットアウト | 152px のステータス帯の中。ヘッダ(152〜278)に影響なし |
| キーボード(現状 `adjust=pan`) | ウィンドウ全体がパンし(Validation 852px / Login 73px)、フォーカス中の `Entry` はキーボードの直上に保たれる。ヘッダは画面外、フッタはキーボードの下 |
| `adjustResize` + `SafeAreaEdges="SoftInput"`(画面ルート) | `ScrollView` ルートはキーボードとの重なり分(880px)の下 Padding が付き全コンテンツをキーボード上へスクロールできるが、フォーカス中の `Entry` へは自動スクロールしない。固定 `Grid` ルート(Login)は内容が潰れる |
| `InputNumberView` | 自前テンキーで `Entry` を持たず IME は出ない(対象外) |

### 🔐SecureStorage の復旧と通信の中断(2026-09-13)

| 対象 | 内容 |
|---|---|
| `State/Settings.cs` | `SecureStorage` の Get / Set / Remove を `GetSecureValueAsync` / `SetSecureValueAsync` / `RemoveSecureValue` に集約し、`Java.Lang.Throwable`(復号失敗 = `Java.Security.GeneralSecurityException` 等)を捕捉。捕捉時は `ResetSecureStorage()` で暗号化層を通さずに実体の `SharedPreferences`(`{package}.microsoft.maui.essentials.preferences`)を `Clear()` し、Get は `null`(未設定)、Set は初期化後に保存し直す |
| `Usecase/NetworkOperator.cs` | デリゲート型を `Func<HttpService, CancellationToken, …>`(進捗版は `Func<HttpService, IProgress, CancellationToken, …>`)に変更し、各 `Execute*` に `CancellationToken cancellationToken = default` を追加。呼び出し側の中断は `NetworkOperationResult.Canceled`(新設)を返し、警告ログ・通知・再試行確認を行わない |
| `Usecase/NetworkUsecase.cs` | 全メソッドに `CancellationToken cancellationToken = default` を追加して `HttpService` まで転送(ダミーファイル作成の `WriteAllLinesAsync` にも渡す) |

- 実機確認: ①`shared_prefs/template.mobileapp.microsoft.maui.essentials.preferences.xml` の keyset を `0800`(空の Keyset として解釈される値)に書き換えて Main > Setting を開くと、変更前は `Java.Security.GeneralSecurityException: empty keyset` でクラッシュ、変更後は画面が開いて値は未設定表示・保存領域は再生成される ②ローカルの WorkServer(`api/test/delay/5000`)に対し 2 秒で中断するトークンを渡すと、インジケータが約 2 秒で閉じ、「Canceled. Retry ?」も出ない(通常の Get server time は成功)
- ビルド 0 エラー 0 警告(Debug)

### 🎮Sudoku の盤面線と Calculator のボタン(2026-09-13)

| 対象 | 内容 |
|---|---|
| `Modules/App/AppGameView.xaml` / VM | 盤面を `mct:UniformItemsLayout` + セル Margin から **`Grid`** へ変更。細線は `RowSpacing` / `ColumnSpacing` = 1、太線は 3x3 境界に挟んだ幅 2 のスペーサ行・列(`Auto,Auto,Auto,2,…` / `*,*,*,2,…`)+ Spacing で 4。セルは `Grid.Row` / `Grid.Column` を VM の `GridRow` / `GridColumn`(= 行列 + ブロック数)にバインド(`Margin` は廃止) |
| `Modules/App/AppCalcView.xaml` | ボタン行を 44 / 60 → **56 / 76** に拡大(`RowDefinitions="*,56,56,76,76,76,76,76"`)、関数ボタンの FontSize 14 → 16 |

- 実機確認: 盤面の横線・縦線とも細線 6 本(2〜3px)+太線 2 本(10〜11px)が全て描画される(変更前は行 1-2 / 4-5 の横線が消え、太線も 6px)。セルのタップ選択と数字入力は従来どおり。Calculator は表示部の余白が減りボタンが一回り大きい
- ビルド 0 エラー 0 警告(Debug)

### 🎮Sudoku の自動モードと Wheel の描画・演出(2026-09-13)

| 対象 | 内容 |
|---|---|
| `Modules/App/AppGameView.xaml` / VM / `Models/App/SudokuGame.cs` | 「新しい問題」ボタンをフッタ **F4「New」**へ。**F3「Auto」= 押すたびに 1 マス**、未確定(空きまたは誤入力)のマスをランダムに 1 つ選んで正解を入れ、そのマスを選択表示(自動で進行はしない)。モデルに `GetSolution(row, col)` を追加 |
| `Graphics/Drawing/WheelDrawing.cs` | 項目を `WheelItem(Label, Effect)` に。描画を作り込み: 扇形は放射グラデーション(`RadialGradientPaint`)、外輪は影付きの暗い環 + 金縁 + 電球 24 個(回転中は流れ、演出中は点滅)、金属調のハブ、影付きの赤いポインタ、太字ラベル(左半分は 180 度回して読める向き)。停止後の演出を追加: 当選セグメントの明滅 + **Sparkle**(ポインタ周辺のきらめき)/ **Confetti**(画面上端から舞い落ちる紙吹雪 110 枚・3.2 秒)。演出の配置は seed 由来の疑似乱数で毎フレーム決定的に計算 |
| `Modules/UI/UIWheelView.xaml` / VM | SPIN ボタンを廃止し **F4「Spin」**へ(ホイールのタップでも回転)。寿司 / 焼肉を当たり枠(Confetti)にし、結果の見出しを「JACKPOT!」(琥珀・太字)に切り替え |
| `State/Settings.cs` / `Controls/CircularLayout.cs` / `Helpers/ImageHelper.cs` | inspectcode の指摘を解消(単純 await の `async` 除去 / `GetValueOrDefault` / 注釈と実態が異なる null 条件アクセスは理由付きで抑止) |

- 実機確認: Sudoku は Auto を押すたびに 1 マスずつ埋まり(放置しても進まない)、New で再開。Wheel は Spin から停止・演出・結果表示まで確認(寿司 / 焼肉で紙吹雪、その他はきらめき)
- ビルド 0 エラー 0 警告(Debug)、inspectcode(Release)0 件

### 🔧MaskedBehavior と相関検証の修正(2026-09-13)

| 対象 | 内容 |
|---|---|
| `Modules/Basic/BasicBehaviorView.xaml` | `mct:MaskedBehavior` に `UnmaskedCharacter="0"` を追加。既定の入力位置文字は `X` のため、`Mask="000-0000-0000"` だけでは全て固定文字扱いになり入力が一切受け付けられなかった |
| `Modules/Basic/BasicValidationViewModel.cs` / View | Confirm の相関検証(`[Compare]`)を入力の度に実行(`PropertyChanged` で Confirm 自身の変更時もエラーを消して再検証。Password の変更時は Confirm 入力済みの場合のみ)。View の `ValidateOnUnfocused` は不要になったため除去 |

- 実機確認: 電話番号の入力が `090-1234-5678` に整形される。Confirm をフォーカスしたまま `ab` → エラー表示、`abc` → 消える、`abcd` → 再表示
- ビルド 0 エラー 0 警告(Debug)

### 👆DragDrop のドロップ先表示(2026-09-13)

| 対象 | 内容 |
|---|---|
| `Modules/View/ViewDragDropViewModel.cs` | 並べ替えの挿入位置を方向で分ける(同じリスト内で下へ動かすときはドロップ先の後ろ、上へ動かすときは前。隣の行へ落として入れ替わらない問題の修正)。`DragTask` を `ObservableObject` にして `IsSource`(ドラッグ元)/ `IsOver`(重なり中)/ `IsOverAbove` / `IsOverBelow`(挿入線の位置)を追加。VM に `IsDragging` / `TodoOver` / `DoneOver` と `DropCompletedCommand` / `ItemOverCommand` / `ItemLeaveCommand` / `ListOverCommand` / `ListLeaveCommand`。強調の解除は `EndDrag`(DropCompleted と各ドロップ処理の末尾) |
| `Modules/View/ViewDragDropView.xaml` | ドラッグ中は受け入れ先を全て表示(行=青の破線枠、TODO / DONE 列=青の破線枠+薄青、ゴミ箱=赤の破線枠+薄赤)。重ねている先は濃く(行=挿入位置の線 `InsertLine`(上へ動かすときは上端、下へ動かすときは下端)+ 薄青 + 実線、列=濃い青、ゴミ箱=従来の赤)。ドラッグ元の行は Opacity 0.35。`DropGestureRecognizer` の `DragOver` / `DragLeave` を行と列にも配線 |

- 実機確認: 並べ替え(4 → 5 で入れ替わる / 5 → 2 で 2 の前に入る)/ TODO → DONE の移動 / ゴミ箱削除 / 対象外への中断のいずれでも、ドラッグ中の表示と終了後の解除が正しい
- ビルド 0 エラー 0 警告(Debug)

### 🖼️UISocial 背景の専用化(2026-09-13)

| 対象 | 内容 |
|---|---|
| `Resources/Images/Social/social_background.png` | 新規(1024×1536 / 2:3、PNG)。従来の構図・衣装・舞台のまま、女の子を中央・膝上までの範囲で生成し直したもの(髪のボリュームを少し増やした版)。旧 `Resources/Images/Common/social_background.png` は削除 |
| `Modules/UI/UISocialView.xaml` | 背景 `Source="social_background.png"` |
| `Document/*.png` | 50 枚すべてを実機で撮り直し(1080×2424 RGB PNG。`UI_TreeMap` はぬいぐるみを写して解析した状態、`Device_BLE` は SwitchBot センサー 3 台を検出した状態、`Device_NFC` は FeliCa カードの読み取り結果、`Device_Activity` は 1,017 歩を計測した状態、`Sample_CV` は CV Local でレゴブロック 3 個を検出した状態、`UI_Meter` は 122 km/h でスティックを左に倒した状態、`UI_Load` はレベル履歴のバーが幅いっぱいまで溜まった状態) |

- Pixel 9a(表示領域 1080×2228)では AspectFill で左右が約 13% ずつ切れる(女の子が中央のため顔と上半身は収まる)
- ビルド 0 エラー 0 警告(Debug)

### 🔧MailDateTimeStringConverter の ConvertBack(2026-09-13)

| 対象 | 内容 |
|---|---|
| `Converters/MailDateTimeStringConverter.cs` | `ConvertBack` を `NotSupportedException` の throw から `Binding.DoNothing` の返却へ(表示専用。TwoWay で使われても例外にならず、バインドの更新も起こさない) |

- 他の一方向コンバーター 18 件は `NotSupportedException` のまま
- ビルド 0 エラー 0 警告(Debug)。実機で UIMail の日時表示を確認

### 📋Control メニューの新設(2026-09-13)

部品・一覧系の画面を Main > Control に集め、View は表現技法(レイアウト / 装飾 / アニメーション / 描画)の画面だけにした。

| 対象 | 内容 |
|---|---|
| `Modules/Main/MenuView.xaml` | Row 4 を `View` / `Control` の 2 列に。Control のアイコンは `AppIcons.ViewModule`(`Markup/AppIcons.cs` に追加、起動時プリロード対象) |
| `Modules/Control/ControlMenuView.xaml` + `ControlMenuViewModel.cs` | 新規(9 段 × 2 列)。Collection \| Carousel / Refresh \| − / Toolkit \| Custom / Chart \| Sf Chart / 残り 5 段は無効ボタン |
| `Modules/Control/ControlCollectionView` / `ControlCarouselView` / `ControlRefreshView` / `ControlToolkitView` / `ControlCustomView` | `Modules/View/View*` から移動・改名(`ViewId` も `Control*`)。戻り先は `ControlMenu` |
| `Modules/Control/ControlChartView` / `ControlSfChartView` | `Modules/Sample/SampleChartView` / `SampleSfChartView` から移動・改名(チャートを Control に集約) |
| `Modules/View/ViewMenuView.xaml` | Layout \| − / Border \| Shadow / Animation \| Easing / Lottie \| Svg / Graphics \| − / Drawing \| DragDrop / Effect \| State / 残り 2 段は無効ボタン |
| `Modules/Sample/SampleMenuView.xaml` | Chart / Sf Chart を外し、Media \| − の行に |
| `Document/Control_Collection.png` / `Control_Carousel.png` / `Control_Refresh.png` / `Control_Chart.png` / `Control_SfChart.png` | `UI_Collection` / `UI_Carousel` / `UI_Refresh` / `Sample_Chart` / `Sample_SfChart` から改名(README の Image 節は `Control_Chart.png` に追従) |
| `README.md` | Implement 表に Control 行(Collection / Carousel / Refresh / Toolkit / Custom / Chart / SfChart)、View / Sample 行から除去。TODO 表から Control menu を削除 |

- View / ViewModel の DI 登録は `Modules` 名前空間配下の名前で自動登録されるため、名前空間の移動に伴う登録変更は無い
- ビルド 0 エラー 0 警告(Debug)。実機で Main > Control > 7 画面の遷移と Back、View / Sample メニューの表示を確認

### 📶WiFi manager(2026-09-14)

| 対象 | 内容 |
|---|---|
| MauiComponents `WiFi.cs` / `WiFi.WiFiManager.cs`(2026-09-19 に `Components/WiFiManager.cs` から移動。`AddComponentsWiFi` で登録) | `IWiFiManager`(`IsSupported` / `IsRadioOn` / `Connection` / `AccessPoints` / `Enabled` / `StartScan` / `OpenSettings` / `StateChanged`)、`WiFiConnection`(SSID / BSSID / RSSI / 信号レベル / リンク速度(Rx / Tx)/ 周波数 / 規格 / IP / ゲートウェイ / DNS)、`WiFiAccessPoint`(SSID / BSSID / RSSI / 信号レベル / 周波数 / チャネル / 帯域幅 / セキュリティ / 規格 / 検出時刻)。`Nfc` と同じ共通 + `*.android.cs` の partial 構成 |
| MauiComponents `WiFi.WiFiManager.android.cs`(+ `.ios.cs` は `IsSupported=false` で設定画面を開くだけ) | `ConnectivityManager.NetworkCallback`(Android 12 以降は `IncludeLocationInfo`)で Wi-Fi の能力(`WifiInfo`)とリンク情報(`LinkProperties`)を受け取り合成。無線のオン / オフは `WIFI_STATE_CHANGED`、スキャン結果は `SCAN_RESULTS_AVAILABLE` の `BroadcastReceiver` で追従(`ScanResults` は電波の強い順、`Capabilities` からセキュリティ、周波数からチャネルを算出)。`StartScan` はシステムの回数制限で拒否されると false。信号レベルは `WifiManager.CalculateSignalLevel`、Settings は `ACTION_WIFI_SETTINGS` |
| `Modules/Device/DeviceWiFiView.xaml` + `DeviceWiFiViewModel.cs` | 上部 = 接続中のブロック(信号アイコン / SSID / 状態バッジ / 📶 dBm・⚡ リンク速度・📡 帯域・🌐 IP の絵文字行。タップで BSSID / 周波数 / 規格 / Rx・Tx / ゲートウェイ / DNS を展開)。下部 = 検出したアクセスポイントの `CollectionView`(件数と帯域別の内訳、更新時刻、スキャンボタン。行 = 信号アイコン + SSID + 接続中 / 帯域 / 🔒 セキュリティ / 規格のバッジ + 📶 dBm・📡 チャネル・↔ 帯域幅、タップで BSSID と検出時刻を展開。接続中を先頭に電波の強い順)。行は角丸のカードにせず区切り線で仕切る。表示中だけ監視し、変化はイベントで反映。F2 = スキャン(制限中はキャッシュ表示の旨)、F4 = Wi-Fi 設定。権限が無い場合は SSID とスキャン結果が取れない旨を表示。VM は `WiFiConnection?` と `WiFiAccessPointItem.AccessPoint`(`WiFiAccessPoint`)をそのまま公開し、XAML が `Connection.Ssid` / `AccessPoint.Rssi` のように直接バインド(2026-09-19)。帯域は `WiFiBandConverter`、セキュリティ(🔓 / 🔒 + 名称)は `s:MapToTextConverter`、信号色は `s:MapToColorConverter`、非公開 SSID は `s:NullToParameterConverter`、チャネル / 帯域幅 / 検出時刻は `MultiBinding` / `StringFormat` |
| `Converters/WiFiSignalIconConverter.cs` / `WiFiBandConverter.cs` | 信号レベル → アイコン(未接続 / 0〜4 本)/ 周波数 → 帯域 |
| `Modules/Device/DeviceMenuView.xaml` / `MauiProgram.cs` / `Extensions.cs` / `Permissions.cs` / `Platforms/Android/AndroidManifest.xml` | WiFi ボタンを有効化、DI 登録、`StateChangedAsObservable`、`NearbyWifiDevices` 権限(Android 13 以降のみ必須)と `RequestNearbyWifiDevicesAsync`、マニフェストに `CHANGE_WIFI_STATE`(スキャン要求)/ `NEARBY_WIFI_DEVICES` を追加 |
| `Document/Device_WiFi.png` / `README.md` | 画像を追加(Image 節)、Implement の Device 行に WiFi、TODO から削除 |

- ビルド 0 エラー 0 警告(Debug)。実機で接続情報とスキャン結果(2.4 GHz / 5 GHz の 2 AP)の表示、`svc wifi disable / enable` による オフ → 未接続 → 接続中 の追従、F2 のスキャン、F4 で設定画面が開くことを確認

### 🧮Grid(ClamGrid)と Card list の追加(2026-09-13)

Control メニューに一覧系の 2 画面を追加した。Grid は `ClamGrid` 1.0.0(SkiaSharp 描画のグリッド)、Card list は標準の `CollectionView` によるカード一覧。

| 対象 | 内容 |
|---|---|
| `Template.MobileApp.csproj` | `ClamGrid` 1.0.0 を追加(SkiaSharp 4.151.2 依存で既存と同版) |
| `Modules/Control/ControlGridView.xaml` + `ControlGridViewModel.cs` | 受注一覧 2,000 行。列は XAML の `GridColumn` で宣言し値は `OrderInfo.Accessors` から Key で解決。`GridDataView<OrderInfo>` が行・選択・ソートを持ち、`ColumnOrders` / `SortOrders` は TwoWay(グリッドが正規化した値を書き戻す)。見出しタップでソート(3 段階、順位表示)、見出し長押しで列設定、行タップで選択、行長押しで未処理の一括選択 / 全解除、確認列はチェックで編集。先頭 2 列固定。F2 選択解除 / F3 列設定とソートを既定へ / F4 再読込、確定(ダイアログ)/ 状態更新(`Suspend` / `Resume` でまとめて反映し `UpdateSelection` で選び直す) |
| `Modules/Control/ControlGridColumnView.xaml` + `ControlGridColumnViewModel.cs` | 列設定。`GridColumnEditSession` を `PushAsync` の引数で受け取り、チェック(表示)と行ヘッダのドラッグ(順序、`GridRowMover`)で編集、Apply で `PopAsync` の引数に `Export()` を返す |
| `Modules/Control/ControlGridStyles.cs` | `GridStyle`: 横罫線のみ・行ヘッダなし・白地の見出し・`sans-serif`・淡いブルーの選択・状態列は `CellColors` で状態ごとの色、納期は期限切れを赤字。列設定用は行ヘッダをドラッグの取っ手として表示。`ListStyle` / `SettingsStyle` の static プロパティを XAML から `x:Static` で参照(列キーは `nameof`) |
| `Models/Control/OrderInfo.cs`(`OrderInfo` / `OrderStatus` / `OrderChannel` / `OrderMarks` / `OrderSamples`。2026-09-19 に 1 ファイルへ統合、`OrderRow` → `OrderInfo`)/ `ColumnOptionAccessors.cs` | 行モデル(変更通知)、状態、受付経路、目印、ダミーデータ、列設定行のアクセサ |
| `Modules/Control/ControlCardListView.xaml` + `ControlCardListViewModel.cs` / `Converters/InitialConverter.cs` | 訪問先一覧 40 件。カードのタップで選択(青背景 + 白文字)、右端で展開(電話 / 前回訪問 / メモ)、状態(未訪問 / 訪問済 / 再訪問 / 不在)で左のストリップと背景色、重点 / 区分のバッジ(角 2px)。状態 / 区分 / 担当の文言と色は `s:MapToTextConverter` / `s:MapToColorConverter`(Style の Setter にバインド。選択は DataTrigger で上書き)、日付は `StringFormat`、並替パネルにクリア(コード順)。行は角丸のカードにせず区切り線で仕切る。ツールバー = 並替パネル(キーをタップで第 1 キー、再タップで昇降反転、最大 3 キー、順位バッジ)/ 昇降 / 全展開 / 再読込。F2 並替 / F3 未訪問の一括選択(全選択済みなら解除)/ F4 確定(ダイアログ) |
| `Models/Control/VisitInfo.cs` | `VisitInfo` / `VisitStatus` / `VisitCategory` / `VisitSamples`(汎用のダミー) |
| `Modules/Parameters.cs` | 列設定セッションと列順序の受け渡し |
| `Modules/Control/ControlMenuView.xaml` / `Modules/ViewId.cs` / `Markup/AppIcons.cs` | Row 4 = Grid \| Card List(`TableChart` / `ViewAgenda`) |
| `Document/Control_Grid.png` / `Control_CardList.png` / `README.md` | 画像を追加(README の Image 節に 1 行)、Implement 表の Control 行に Grid(ClamGrid)/ Card list、TODO から削除 |

- ビルド 0 エラー 0 警告(Debug)。ReSharper inspectcode 0 件(`ControlCardListView.xaml` の RelativeSource / `x:DataType` 指定バインドは `Xaml.BindingWithContextNotResolved` の誤検知として既存画面と同じコメントで抑止。`Helpers/ImageHelper.cs` の `old?.Dispose()` には理由付きの `ReSharper disable once` を再付与)。実機で Grid のソート / 選択 / 列設定(表示切替・ドラッグ・Apply・Reset)/ 横スクロール、Card list の選択 / 展開 / 並替パネル / 昇降 / 全展開 / F3 / F4 を確認

### 📑Bottom sheet と Drawer の追加(2026-09-14)

Control メニューに Bottom sheet と Drawer の 2 画面を追加した。Syncfusion(`SfBottomSheet` / `SfNavigationDrawer`)と自作(`Controls/BottomSheetView.cs` / `Controls/SideDrawer.cs`)を同じ内容で並べて比べる。

| 対象 | 内容 |
|---|---|
| `Controls/BottomSheetView.cs` | 自作のボトムシート(`Grid` 派生、依存なし)。`IsOpen`(TwoWay)/ `SheetContent` / `HalfExpandedRatio`(既定 0.5)/ `ExpandedRatio`(0.92)/ `CornerRadius` / `SheetBackgroundColor`。背景(タップで閉じる)と上角丸の `Border`(グラバー + 内容)を重ね、開くと半開まで上がる。ドラッグで 半開 ⇔ 全開 ⇔ 閉じる(離した位置に近い状態へ)、背景の暗さはシートの位置に連動。高さが未確定(初回表示)のときの開閉は `SizeChanged` 後に行う。ジェスチャは内側の `Grid` に付ける。2026-09-19: 高さは内容に合わせる(上限 `ExpandedRatio`、内容が半開に収まるなら全開は無い)。ドラッグの終了は離したときの速さ(100ms 止まっていれば無し)か最後に動かした方向で決め(下 = 半開より上なら半開、それ以外は閉じる / 上 = 半開より下なら半開、それ以外は全開)、方向が明確でなければ離した位置に近い状態へ。ドラッグの続きは離した速さから減速(SinOut、80〜400ms)、ボタン / 背景からは 250ms。閉じる途中のドラッグは無視 |
| `Controls/SideDrawer.cs` + `SideDrawer.android.cs` | 自作のドロワー(`Grid` 派生、依存なし)。`IsOpen`(TwoWay)/ `DrawerContent` / `DrawerWidth`(280)/ `EdgeSwipeEnabled` / `EdgeWidth`(24)/ `DrawerBackgroundColor`。左端の帯(スワイプで開く)+ 背景(タップとスワイプで閉じる)+ 影付きのパネル(ドラッグの終了はボトムシートと同じ判定 = 離したときの速さ(100ms 止まっていれば無し)か最後に動かした方向(16dp / 0.2dp/ms)、どちらでもなければ半分の位置で開閉を決め、ドラッグの続きは離した速さから減速(SinOut、80〜400ms)。ボタン / 背景からの開閉は 150ms)。閉じているときは帯以外はタッチを通す(`InputTransparent` + `CascadeInputTransparent=False`)。Android は帯の上下中央 200dp を `SystemGestureExclusionRects` でシステムの戻るジェスチャから除外 |
| `Modules/Control/ControlBottomSheetView.xaml` + `ControlBottomSheetViewModel.cs` | 新規。`SfBottomSheet`(ページを包む。`HalfExpandedRatio=0.45` / `IsModal`)と `BottomSheetView` をルートの `Grid` に兄弟で置き、同じ内容(4 項目 + 閉じる)を表示。F2 = Sf / F3 = 自作(同じシートなら閉じる、別のシートが開いていれば閉じ終わってから開く)。開く / 閉じるボタンは他画面と同じ高さ 44 / 角丸 8。選択した項目を「結果」カードに表示して閉じる |
| `Modules/Control/ControlDrawerView.xaml` + `ControlDrawerViewModel.cs` | 新規。`SfNavigationDrawer`(`Position=Left` / `Transition=SlideOnTop` / 幅 280 / ヘッダ 96 / フッタ 44)と `SideDrawer` を `SfSegmentedControl`(`VisibleSegmentsCount=2` で 2 分割し文言を全表示)で切り替え(端のスワイプは選択中の側だけ有効)。ヘッダ(アバター / 名前 / メール)+ 5 項目(Material アイコン)+ フッタを共通の Style / `DataTemplate` で構成。F2 = 開閉。項目タップで「選択中」に反映して閉じる |
| `Modules/Control/ControlToolkitView.xaml` + `ControlToolkitViewModel.cs` | `SfBottomSheet` の「シート」を Control > Bottom Sheet へ移し、ルートを `SfTabView` に。`SfSegmentedControl` の `SelectedIndex` は `Mode=TwoWay` を明示(Drawer 画面も同じ) |
| `Modules/Control/ControlMenuView.xaml` / `Modules/ViewId.cs` / `Markup/AppIcons.cs` | Row 5 = Bottom Sheet \| Drawer(`VerticalAlignBottom` / `MenuOpen`)。空きは 3 段 |
| `Document/Control_BottomSheet.png` / `Control_Drawer.png` / `README.md` | 画像を追加(Image 節に 1 行)、Implement 表の Control 行に Bottom sheet / Drawer、TODO から削除 |

- ジェスチャナビゲーションでは画面の左端からのスワイプはシステムの「戻る」が優先される。`SfNavigationDrawer` の `EnableSwipeGesture` は効かず、自作は除外した帯(上下中央 200dp)から始めたときだけ開く(付録B)
- ビルド 0 エラー 0 警告(Debug)。ReSharper inspectcode 0 件(`DeviceWiFiView.xaml` の RelativeSource バインドの誤検知を既存画面と同じコメントで抑止、`ControlToolkitView.xaml` の未使用 xmlns を削除、`WiFiManager.android.cs` の整数除算と冗長な `?.` を修正)。実機で Sf / 自作のシート(開く・上へドラッグで全開・下へドラッグで半開と閉じる・背景タップで閉じる・項目選択)、Sf / 自作のドロワー(開く・項目選択・背景タップ・パネルのドラッグで閉じる・自作は帯の中央からの端スワイプで開く)、Toolkit の `SfSegmentedControl` の選択が VM に反映されることを確認

### 📶WiFi のアクセスポイント一覧の定期更新と未検出の猶予(2026-09-14)

| 対象 | 内容 |
|---|---|
| `Modules/Device/DeviceWiFiViewModel.cs` | 表示中は「最後のスキャン要求または結果から 30 秒」経ったときだけ自前でスキャンする(他者のスキャン結果が届いていれば延期。前面アプリの制限 2 分に 4 回。拒否されたときは次回に回す)。手動スキャン(ヘッダの更新ボタンと F2)は廃止し自動のみ。新しいスキャン結果は行を差し替えず、BSSID で既存の行をその場で更新して並べ替え(`ObservableCollection.Move`)、含まれなかった行は「未検出」にして 60 秒後に消す(5 秒ごとに確認)。同じ結果の再通知(接続の変化など)では未検出の判定をしない。並びは 接続中 → 検出中を電波の強い順 → 未検出 |
| `Modules/Device/DeviceWiFiView.xaml` | 未検出の行は半透明(Opacity 0.45)にして「未検出」バッジを付ける。ヘッダの更新ボタンと F2 = Scan を削除(F4 = 設定のみ) |
| MauiComponents `WiFi.WiFiManager.android.cs` | スキャン結果の受信を `SCAN_RESULTS_AVAILABLE` ブロードキャストから `WifiManager.registerScanResultsCallback`(API 30。ライブラリの最小 API 27 に合わせ、30 未満はブロードキャストと `CalculateSignalLevel(rssi, 5)` にフォールバック)に変更(自アプリ以外が要求したスキャンの完了も受ける)。`WIFI_STATE_CHANGED` でもスキャン結果を読み直す(無線オフで空になり、一覧が未検出 → 削除の流れに乗る) |
| `Document/Device_WiFi.png` | 撮り直し |

- ビルド 0 エラー 0 警告(Debug)。実機で `svc wifi disable` → 全行が未検出(半透明)→ 60 秒後に消える → `svc wifi enable` → 再検出を確認。`dumpsys wifiscanner` で自アプリのスキャン要求が結果の 30 秒後(約 34 秒間隔)に出ること、`cmd wifi start-scan` の外部スキャンの結果をコールバックで受けて次の自前スキャンがその 30 秒後に延びることを確認

### 🧮Card list と Grid の絵文字・色付きバッジ化(2026-09-14)

| 対象 | 内容 |
|---|---|
| `Modules/Control/ControlCardListView.xaml` + `ControlCardListViewModel.cs` | 行 = 担当者のアバター(先頭 1 文字、担当ごとの色)+ コード + 名前 / 状態(⏳ 未訪問 / ✅ 訪問済 / 🔁 再訪問 / 🚫 不在、白文字の色付き)・🔥 重点・📌 今日・🆕 初回・区分(🗓 定期 / ✨ 新規 / 🔧 点検 / 💰 集金、区分ごとの淡色)のバッジ(折り返し)/ 📍 住所・👤 担当・🕒 予定の絵文字行。展開部は 📞 電話・📅 前回・📝 メモ。上部の件数は状態ごとの色付きバッジ(⏳ ✅ 🔁 🚫)。行の形(フラット + 区切り線)と選択(青地 + 白文字)は据え置き |
| `Models/Control/OrderInfo.cs`(当時は `OrderRow.cs` / `OrderChannel.cs` / `OrderSamples.cs`) | 状態を絵文字付きに(⏳ 未処理 / 🔄 処理中 / ✋ 保留 / ✅ 完了)。フラグ列(❗ 期限切れ / 🔥 期限 3 日以内 / 💰 30 万円以上 / 📦 数量 20 以上 / 🆕 3 日以内の更新)、顧客ランク列(⭐〜⭐⭐⭐、顧客ごとに固定)、受付列(🏪 店頭 / 🌐 Web / 📞 電話 / 📠 FAX)を追加。状態更新でフラグも再評価 |
| `Modules/Control/ControlGridView.xaml` + `ControlGridViewModel.cs` | 列を 状態 / 受注番号 / フラグ / 顧客 / ランク / 商品 / 数量 / 金額 / 納期 / 受付 / 確認 / 担当 / 更新 に。ランクと受付はソート可、フラグはソート不可 |
| `Modules/Control/ControlGridStyles.cs` | フラグ列の背景(期限切れ = 淡い赤 / 期限間近 = 淡い橙)、ランク 3 の背景(淡い黄)、高額の金額と大口の数量の文字色、受付ごとの文字色、納期の期限切れ(赤)/ 期限間近(橙) |
| `Document/Control_CardList.png` / `Control_Grid.png` | 撮り直し |

- ClamGrid は `SKFontManager.MatchCharacter` のフォールバックで絵文字をカラーで描く。文字は絵文字既定のもの(⏳ ✅ ❗ ✋ など)を使い、VS16 が要るテキスト既定の記号(⚠ ⏸)は使わない
- ビルド 0 エラー 0 警告(Debug)。実機で Card list のバッジ・アバター・件数、選択 / 展開、Grid の絵文字列(状態 / フラグ / ランク / 受付)と色を確認

### 📈診断パネルのメモリ推移とリーク検出(2026-09-14)

Task_Checklist 3-1(メモリ監視オーバーレイ)と 3-2(リーク検出)を、既存の診断パネルとナビゲーションプラグインへの局所的な追加で実装した。

| 対象 | 内容 |
|---|---|
| `Shell/DiagnosticPanel.xaml` + `.xaml.cs` | 📈 で出る既存の診断オーバーレイ(DEBUG 限定、全画面共通)に、ワーキングセットの直近 60 秒のスパークライン(`GraphicsView` + パネル内の `IDrawable`)を追加。最新値を右端に置き、最小〜最大で自動スケール(1 MB 未満の変動は平ら)。線の色は Memory の値の色(安全 / 警告 / 危険)に連動。表示開始で履歴をリセット |
| `Extender/LeakDetectionPlugin.cs` | 新規。`PluginBase.OnClose` で閉じたビューと ViewModel を `WeakReference` で保持し、5 秒後に GC(マネージド → Java → マネージド)を回して残っていれば `WarnLeakSuspected`、回収されていれば `DebugClosedObjectCollected` を出力 |
| `MauiProgram.cs` / `Log.cs` | `#if DEBUG` でプラグインを登録。ログメッセージ 2 件を追加 |
| `Document/Task_Checklist.md` / `README.md` | 3-1 / 3-2 を削除(3-3 画面録画 → 3-1)。TODO の Memory monitor overlay / Leak detection を削除 |

- ビルド 0 エラー 0 警告(Debug)。実機で 📈 のパネルにスパークラインが描かれることを確認。Control(Card List / Grid / Bottom Sheet / Drawer)、Device(WiFi)、UI 1(Profile)を開いて戻る操作で、閉じた 36 件(ビュー 18 + ViewModel 18)がすべて回収され、リーク疑いは 0 件

### 🤖Azure AI Vision / Ollama チャット / 音声入力の実装(2026-09-14)

Task_Checklist 2-3(Azure / AI サービス利用部分)を実装した。接続先とキーは設定画面(QR)で投入した値を使う。

| 対象 | 内容 |
|---|---|
| `Usecase/AzureVisionUsecase.cs` | 新規。`ImageAnalysisClient`(Image Analysis 4.0)で物体 / 人物 / タグ(日本語)/ 文字、`FaceClient` で顔を検出し、正規化した矩形 + ラベル + 信頼度(`DetectResult`)またはタグ(`TagResult`)に変換。物体と人物は信頼度 0.5 未満を除外。接続先とキー(`Settings.AIServiceEndPoint` / `GetAIServiceKeyAsync`)は呼び出しのたびに取得し、未設定は `InvalidOperationException` |
| `Modules/Sample/SampleCvNetObjectViewModel.cs` / `People` / `Ocr` / `Face` / `Tag` + 各 View | 撮影 → 静止画表示 → `BusyState` の間に解析 → `DetectDrawing` で枠を描く(Tag は左上のパネルに 🏷 名前と %)。`RequestFailedException` / `HttpRequestException` / 未設定はダイアログで表示。解析中の再入は `IsProcessing` でガード |
| `Graphics/Drawing/DetectDrawing.cs` | ラベルがあれば「ラベル 信頼度」を描く(ローカル検出もラベル付きに) |
| `Modules/Sample/SampleChatViewModel.cs` + `SampleChatView.xaml` | 設定の Ollama(接続先 + モデル)があれば `OllamaSharp` の `Chat` でストリーミング応答(未設定は従来の疑似応答。冒頭のあいさつに動作モードを表示)。音声入力は `ISpeechService` の音声認識で文字起こし(無音で自動停止。認識できなければその旨を表示)、抽出は Ollama に「項目: 値」の 4 行で答えさせて読み取る(未設定 / 音声なしは固定の例) |
| `State/Settings.cs` / `Modules/Main/SettingViewModel.cs` / `SettingView.xaml` | `OllamaEndPoint` / `OllamaModel` を追加(QR のキー名も同じ)。AI Service の節に Ollama / Model の行 |
| `MauiProgram.cs` | `AzureVisionUsecase` の DI 登録 |
| `Document/Sample_CvNet_Tag.png` / `README.md` | 画像を追加、Implement の Sample 行に Object / Tag / People / OCR / Face(Azure AI Vision)/ Chat(Ollama)、TODO から Cognitive service / Chat AI を削除 |

- ビルド 0 エラー 0 警告(Debug)。実機(Foundry の AI Services リソース + PC の Ollama gemma2 に `adb reverse tcp:11434` で接続)で、Tag = 暗い画面から「霧 95% / 黒 90% / ぼかし 88% / 灰色 88%」、Object / People / OCR = 例外なく解析(暗い被写体のため枠なし)、Face = このリソースは Face API を持たないため 401 のダイアログ、Chat = 質問への応答がストリーミングで表示、音声 = 無音で自動停止し「(音声を認識できませんでした)」→ 抽出は固定の例、Setting = Ollama / Model の行を確認

### 🔔ローカル通知(2026-09-14)

Task_Checklist 2-2 のローカル通知を自作した(ライブラリなし。FCM は 2-2 に残す)。

| 対象 | 内容 |
|---|---|
| `Components/NotificationService.cs` | 新規。`INotificationService`(`Show` = 即時 + アクションボタン + ペイロード / `Schedule` = 指定時間後 / `Cancel` / `CanScheduleExact` / `OpenExactAlarmSettings` / `Tapped` イベント / `TakePendingTap`)。`Nfc` と同じ共通 + `*.android.cs` の partial 構成。チャンネル ID は `ChannelId` 定数 |
| `Components/NotificationService.android.cs` | `NotificationCompat.Builder`(チャンネルは表示のたびに作成 = 既存なら no-op)。本体とボタンの `PendingIntent` は `MainActivity` を開く Activity Intent(extras に id / action / payload、requestCode は id とボタン番号で分ける)。スケジュールは `AlarmManager`(正確なアラームが許可されていれば `SetExactAndAllowWhileIdle`、未許可は `SetAndAllowWhileIdle` で前後を許容)+ `NotificationAlarmReceiver`(manifest 登録はアトリビュートから生成)。ボタンのタップは通知を消す |
| `Platforms/Android/MainActivity.cs` | `OnCreate` / `OnNewIntent`(`LaunchMode = SingleInstance`)で `NotificationService.HandleIntent` に渡す。`Context.NotificationService` 定数と名前が衝突するため `Components.` を付けて参照 |
| `MainPageViewModel.cs` | 初期遷移の完了後に `Tapped` を購読し、どの画面でもトーストで表示。起動前に届いたタップは `TakePendingTap` で拾う |
| `Modules/Device/DeviceMiscView.xaml` + `DeviceMiscViewModel.cs` | Notification カード(Notify = 承認 / 却下ボタン付き、Schedule = 10 秒後、Cancel、Exact alarm = 許可設定を開く)。タップ結果をカードに表示 |
| `Permissions.cs` / `Platforms/Android/AndroidManifest.xml` / `Extensions.cs` / `MauiProgram.cs` / `Markup/AppIcons.cs` | `RequestNotificationsAsync`(`PostNotifications`)、`POST_NOTIFICATIONS` / `SCHEDULE_EXACT_ALARM`、`TappedAsObservable`、DI 登録、Small アイコン 4 件 |
| `Document/Task_Checklist.md` / `README.md` | 2-2 は Push(FCM)の判断項目だけに。Implement の Device 行に Local notification |

- ビルド 0 エラー 0 警告(Debug)。実機で 承認依頼(承認 / 却下)の通知 → シェードの「却下」で `ボタン [reject]: SO-2026-000123`、本体タップで `タップ: SO-2026-000123`、Schedule → 正確なアラーム未許可の旨を表示し 14 秒以内にリマインダーが届く、Cancel で消えることを確認

### 🌐ネットワーク実装(template-maui-server 対向)(2026-09-15)

対向サーバーを template-maui-server(`D:\GitHubTemplate\template-maui-server`、8080 = Web / API、9090 = gRPC)にし、Web API / ストレージ / SignalR / gRPC を実装した。サーバー側の変更も同時に行なった(同リポジトリの README に反映)。

| 対象 | 内容 |
|---|---|
| `Modules/Network/NetworkMenuView.xaml` | Download \| Upload の行を HTTP \| Storage に、Realtime \| gRPC を 2 列に(単発ボタンは残置) |
| `Modules/Network/NetworkHttpView.xaml` + `NetworkHttpViewModel.cs` | Data の CRUD(一覧は 20 件ずつ追加読み込み、行選択で詳細取得)、ログイン状態(Id / 有効期限)、10 秒待つ API のキャンセル、ログ |
| `Modules/Network/NetworkStorageView.xaml` + `NetworkStorageViewModel.cs`(新規) | ストレージの一覧(階層移動)、ファイル / 写真のアップロード(進捗 + キャンセル)、ダウンロード(公開フォルダ)、削除 |
| `Modules/Network/NetworkRealtimeView.xaml` + `NetworkRealtimeViewModel.cs` | 乱数のダミーを SignalR の実データに。接続は `MonitorConnection.Connect()` の購読(遷移時に購読、離脱時に破棄 = 切断、再接続は購読し直し)、受信は `ServerStatus` / `Notifications` の購読。サーバーの CPU / メモリ / 接続数のグラフ、端末の状態を 10 秒ごとに送信、受信した通知の一覧(前面はトースト、バックグラウンドはローカル通知) |
| `Modules/Network/NetworkGrpcView.xaml` + `NetworkGrpcViewModel.cs` | gRPC チャット(管理画面 `/chat` と相互、切断中の送信は再接続後に配送)と単項 RPC |
| `Helpers/ReactiveSignalR.cs`(Rx ベースで作り直し) | 汎用の SignalR 接続維持: `Connect()` = 購読で接続を始め破棄で切断する `IObservable<HubStatus>`(初回接続の失敗は `RetryWhen` でバックオフ再試行、ネットワーク復帰(`resume`)で待ちを打ち切り(`Amb`)、接続後の切断は `WithAutomaticReconnect` の自動再接続、`Closed` になったら `Repeat` で初回接続からやり直し)、`On<T>()` = 受信メッセージの `IObservable<T>`、`CreateRetryPolicy` = 諦めないバックオフ |
| `Services/MonitorConnection.cs`(新規) | MonitorHub 固有(認証なし): `HubConnection` の構築(`/hubs/monitor`、KeepAlive 15 秒 / ServerTimeout 30 秒)、`Connect(baseAddress)`(`ReactiveSignalR.Connect` + 状態のログ)、`ServerStatus` / `Notifications` の `IObservable`、`ReportDeviceStatusAsync` |
| `Services/Chat/`(新規) | サーバーの WPF サンプル `Chat/` の移植(`ChatRoomClient` = 指数バックオフ再接続 + 送信キュー)と `chat.proto` / `server.proto` のコピー |
| `Services/HttpService.cs` / `ApiContext.cs` / `Log.cs`(新規) | Data CRUD / ストレージ一覧・削除(PUT / DELETE は `HttpClient` を `RestResponse` に包む)、`LoginId` / `TokenExpires`、通信系ログの分離 |
| `Usecase/NetworkOperator.cs` / `NetworkUsecase.cs` | 401 で保存した Id により再ログインして 1 回だけ再送、`ExecuteTransfer`(インジケーターなし)、gRPC チャット用の `EnsureLoginAsync` / `GetTokenAsync` |
| `Models/Api/` | `DataListResponse`(`Id` long / `Total`)、`DataResponse` / `DataCreateRequest` / `DataCreateResponse` / `DataUpdateRequest` / `StorageListResponse` / `MonitorMessages` |
| `State/Settings.cs` / `Modules/Main/SettingView.xaml` + `SettingViewModel.cs` | `MonitorEndPoint` → `GrpcEndPoint`、Setting の gRPC 行 |
| `State/Session.cs` / `App.xaml.cs` | `IsForeground`(Window の Resumed / Stopped) |
| `MauiProgram.cs` / `Helpers/JwtHelper.cs` / `Extensions.cs` / `Modules/ViewId.cs` / `Markup/AppIcons.cs` / `Template.MobileApp.csproj` | Rester の JSON を PascalCase + 大文字小文字を区別しない設定に、JWT の exp 取り出し、`ConnectivityChangedAsObservable`、`NetworkStorage`、アイコン、`<Protobuf Include="Services\Chat\*.proto">` |
| `Modules/Network/NetworkScpViewModel.cs` | 転送を BusyState(オーバーレイ)の外で実行し、キャンセルボタンが押せるように |
| `Document/Development.md` | 「サーバー処理」を対向サーバーの起動 / ポート / `adb reverse` / QR の手順に更新 |
| template-maui-server | `/qr` に全キー(`GrpcEndPoint` / Ollama / SCP、接続先以外は `Client` セクションの初期値)、`GET /api/data/list?offset=&size=`(`Total`)、SignalR `MonitorHub`(`/hubs/monitor`、認証なし)+ `DeviceRegistry`(接続の `Abort` を保持)+ `ServerStatusWorker`(1 秒)+ `NotificationRelayWorker`、管理画面 Devices(端末一覧 / 通知送信 / 切断)と Home の接続数、gRPC `info.ServerInfo/GetServerTime`、開発環境の JWT 有効期限 5 分、テスト 11 件追加(35 件) |

- ビルド 0 エラー 0 警告(アプリ Debug / Release、サーバー Debug / Release)、inspectcode 0 件(両方)、サーバーのテスト 33 件成功。実機(Pixel 9a、`adb reverse tcp:8080` / `tcp:9090`)で確認: HTTP = 未ログインの作成は 401 → ログイン(有効期限表示)→ 作成 → 重複 409 → 45 件を 20 / 40 / 45 と追加読み込み → 行選択で詳細 → 更新 → 削除 → 10 秒 API を 2 秒でキャンセル → 有効期限切れ後の作成が 401 → 再ログイン → 成功。Storage = 3 MB ファイルと写真のアップロード → 一覧 / 下階層 / 上へ → ダウンロード(公開フォルダ)→ 削除。Realtime = 接続済み(サーバーログの接続 ID と一致)→ グラフとサーバー時刻 → 管理画面 Devices に端末が表示 → 通知送信(前面 = 一覧に追加、HOME 中 = ローカル通知)→ サーバー停止で再接続中 → 再起動で新 ID で接続済み → 管理画面の「切断」(`Closed`)で 100 ms 後に新 ID で接続済み → サーバー停止中の再接続は接続中...(0 / 2 / 5 秒のバックオフ)→ 再起動で接続済み → 離脱で切断(サーバーログの `Monitor disconnected`)。gRPC = 単項 RPC のサーバー時刻 → 端末 ⇔ `/chat` の相互送受信 → サーバー停止中の送信(未配送 1)→ 再起動で再接続(1 → 2 → 5 → 10 秒のバックオフ)と配送

### 🧩.NET 10 API の適用と IChatClient 抽象化(2026-09-15)

| 対象 | 内容 |
|---|---|
| `Modules/Basic/BasicSettingView.xaml` | `SearchBar.SearchIconColor` / `ReturnType="Search"`、`Switch.OffColor` |
| `Modules/Device/DeviceMiscViewModel.cs` | `IVibration.IsSupported` / `IHapticFeedback.IsSupported` が false の端末では Vibrate / Feedback のボタンを無効に |
| `Modules/Device/DeviceLocationViewModel.cs` + `DeviceLocationView.xaml` | `IGeolocation.IsEnabled` が false のとき測位待ちの空状態に「Location service is disabled」を表示 |
| `Modules/Sample/SampleChatViewModel.cs` | チャットの依存を `OllamaSharp` の `Chat` から `Microsoft.Extensions.AI.IChatClient` に変更(`OllamaApiClient` を設定の Ollama から直接生成)。会話の履歴は VM が `List<ChatMessage>` で保持し `GetStreamingResponseAsync(history)` で送る |
| `Document/Development.md` | `dotnet run --project … -f net10.0-android --device <シリアル>` の手順(端末が複数あるときは `--device` 必須) |
| `Document/Other_App_Candidates.md`(新規) | 本サンプルでは対象外だが別アプリケーションで導入を検討する項目の一覧(ディープリンクを移動) |
| `Document/Telemetry_Study.md`(新規) | クラッシュレポート / テレメトリ基盤の検討資料(現状、要件と論点、DeviceManager 型 / OpenTelemetry / 外部サービスの候補と比較、アプリ側の組み込み設計、Aspire の位置付け) |
| `Document/Task_Checklist.md` / `README.md` | 3-1(→ Other_App_Candidates)/ 3-7(完了)/ 3-8・3-9(→ Telemetry_Study)を削除、3-2 は残りの項目に、3-5 は StyleClass の活用計画に。TODO 表を同期 |

- ビルド 0 エラー 0 警告。実機で Sample > Chat が Ollama(gemma2)で応答し、2 回目の質問が 1 回目の内容を踏まえること(履歴の送信)、Basic > Setting の検索アイコンが青、Device > Misc の Vibrate / Cancel が有効、`dotnet run --device 4A071JEBF16992` で配置と起動を確認

### 🧮ClamGrid 1.1.0 の Converter / OrderInfo / ボトムシート / WiFi のレコードバインド(2026-09-19)

| 対象 | 内容 |
|---|---|
| Other-ClamGrid `GridColumn.cs` / `Rendering/GridRenderer.cs` / `Document/API.md` / `README.md` / `Directory.Build.props`(別リポジトリ、未コミット・未公開) | `GridColumn.Converter`(`IValueConverter?`)を追加。文字セルの描画と自動幅の計測で `Format` の前に適用(真偽セル / ソート / 編集は生値)。1.0.0 → 1.1.0。テスト `ConverterRunsBeforeTheFormatString` を追加 |
| `Template.MobileApp.csproj` | `ClamGrid` 1.1.0(nuget.org に公開済み)へ更新 |
| `Models/Control/OrderInfo.cs` | `OrderRow.cs` / `OrderStatus.cs` / `OrderChannel.cs` / `OrderSamples.cs` を 1 ファイルに統合し `OrderRow` → `OrderInfo`。`StatusText` / `Flags`(文字列)/ `RankText` / `ChannelText` を削除し、アクセサは `Status` / `Marks`(`[Flags] OrderMarks`)/ `Rank` / `Channel` の値を返す |
| `Modules/Control/ControlGridView.xaml` / `Converters/FlagsToTextConverter.cs` | 状態 / 受付は `s:MapToTextConverter`、ランクは `s:MapToTextConverter`(`{s:Int32 n}` → ⭐)、目印は `FlagsToTextConverter`(`[Flags]` の立っているビットの文言を `Entries` の順に連結)を列の `Converter` に指定 |
| `Modules/Control/ControlGridViewModel.cs` | 状態更新は `Rows.Suspend()` → 更新 → `Resume()` → `UpdateSelection` で選び直す(行ごとの変更通知で全行を並べ替え直さない) |
| `Controls/BottomSheetView.cs` | 高さを内容に合わせる(上限 `ExpandedRatio`)。ドラッグの終了は離した速さ(100ms 止まっていれば無し)と最後に動かした方向で決める(戻したドラッグは閉じない。速さは逆方向に転じたら平均せず置き換える)。ドラッグの続きは離した速さから減速(SinOut、80〜400ms)。閉じる途中のドラッグは無視 |
| `Controls/SideDrawer.cs` | 開閉 250 → 150ms。ドラッグの終了判定と続きの減速をボトムシートと同じに(`Settle` / `FlingDuration`、`flingDuration` を `IsOpen` の変更に渡す)。背景にも `PanGestureRecognizer` を付け、開いているときは画面のどこからでもスワイプで閉じる(閉じているときの開く操作は帯 24dp × ジェスチャナビでは上下中央 200dp のまま = 付録B) |
| `Modules/Control/ControlBottomSheetViewModel.cs` | F2 / F3 は同じシートなら閉じる、別のシートが開いていれば閉じ終わってから(300ms)開く |
| `State/Settings.cs` / `Modules/Main/SettingViewModel.cs` / `SettingView.xaml` | `OtelEndPoint`(QR キー同名)を追加。設定画面は設定値のパネルを内容の高さ(`Auto`)にしてカメラが残りを全て使う構成にし、行は 1 行表示(`MiddleTruncation`)、行間 2 / 高さ 22 / 文字 13 / 見出し 15 / 見出し列 84 に詰めた |
| (server) `appsettings.json`(`Kestrel:Endpoints`)/ `launchSettings.json` / README / ChatClient の既定値 | ポート構成を 8080 = Web / API(HTTP/1.1)、9090 = gRPC(HTTP/2 h2c)へ変更(旧 8081 / 8084)。QR の gRPC 接続先は `Kestrel:Endpoints:Grpc:Url` から導出されるため追従 |
| (server) `Setting` テーブル + `SettingService` / `SettingAccessor`、`ClientSettingKeys.cs`、`QrPage` | QR の値(AI / Ollama / SCP)を DB の Key / Value で管理し `/qr` で編集・保存(空欄は削除)。接続先 API / gRPC / OTEL はサーバーの URL から決めて保存しない。`appsettings.json` の `Client` セクションと `ClientSetting.cs` は削除 |
| `Modules/Device/DeviceWiFiViewModel.cs` + `DeviceWiFiView.xaml` / `Converters/WiFiBandConverter.cs` | 個別の `[ObservableProperty]` 13 個と `WiFiAccessPointItem` の透過 / 文言プロパティを削除し、`WiFiConnection?` / `WiFiAccessPoint` をそのままバインド。文言と色はコンバーター(帯域 / セキュリティ / 信号色 / 非公開 SSID)と `MultiBinding` / `StringFormat`。信号色の `DataTrigger` 9 個は `s:MapToColorConverter` 1 個に |

- ビルド 0 エラー 0 警告(Debug。Release は既存の Android BLE の警告のみ)。ClamGrid は Release 0 警告、テスト 141 / 142(`GridLayoutTests.BoundariesUnderTheFrozenColumnsAreNotGrabbable` は 1.0.0 でも失敗する既存)。実機: Grid の各列の絵文字と色、未処理 662 行を一括選択しての状態更新(固まらず、選択とスクロール位置が残る)、シートの F2 / F3 切替・短いフリック・戻したドラッグ・内容に合う高さ、ドロワーの開閉(戻したドラッグは閉じない / 開かない、ゆっくり動かして止めてから離すと距離で決まる、短いフリック、帯のスワイプ、背景のスワイプ / ドラッグで閉じる)、WiFi の接続カード / 一覧 / 展開

### 🤖Face(顔検出)の削除と Windows 検証コンソール(2026-09-19)

Face API は Image Analysis と別の専用リソース(Face リソース、または Limited Access 承認済みのサブスクリプション)が必要なため、顔検出のサンプルを削除した。

| 対象 | 内容 |
|---|---|
| `Modules/Sample/SampleCvNetFaceView.xaml(.cs)` / `SampleCvNetFaceViewModel.cs` | 削除 |
| `Modules/Sample/SampleCvNetMenuView.xaml` / `Modules/ViewId.cs` | Face のボタンを空の無効ボタンに、`SampleCvNetFace` を削除 |
| `Usecase/AzureVisionUsecase.cs` / `Template.MobileApp.csproj` | `DetectFacesAsync` と `Azure.AI.Vision.Face` の参照を削除(Image Analysis 4.0 の物体 / 人物 / タグ / 文字のみ) |
| `README.md` / `Document/Task_Checklist.md` | Implement の Sample 行から Face、TODO / サマリ / 3 節から Face 識別(旧 3-6)を削除。0-8 に `Works3/AiSample` の確認項目 |
| `Works3/AiSample`(別フォルダ) | Sample > CV Net / Chat と同じ処理を Windows のコンソールで実行する検証用ソリューション(`AzureVisionUsecase.cs` は無変更のコピー)。構成 / 設定 / Azure の設定 / 確認結果は同フォルダの README |
| `Platforms/Android/AndroidManifest.xml` | `android:usesCleartextTraffic="true"`。Wi-Fi 経由の Ollama(`http://<PC の IP>:<port>`)など localhost 以外への平文 HTTP は既定では `Connection failure` になる(`OllamaSharp` の `HttpClient` は Android のネイティブハンドラ) |

- ビルド 0 エラー 0 警告(Debug)。CV Net メニューは Object / Tag / People / Ocr の 4 つ。実機の Sample > Chat が Wi-Fi 経由(`OllamaEndPoint=http://192.168.100.9:12321`、`adb reverse` なし)で応答

### 🤖音声入力の整理 / カメラ撮影の打ち切り / 設定判定の集約(2026-09-20)

| 対象 | 内容 |
|---|---|
| `Controls/ChatView.xaml(.cs)` | 入力バーにマイクボタン(`VoiceCommand` / `IsListening`。コマンド未設定なら非表示、認識中は赤い停止ボタン + パルス、プレースホルダー「話しかけてください」)。コード片のテンプレート(`CodeTemplate` / `IsCode`)は削除 |
| `Modules/Sample/SampleChatView.xaml` + `SampleChatViewModel.cs` / `Models/Sample/Chat/AiChatMessage.cs` | 音声入力を 4 ステップのオーバーレイ(録音 → 文字起こし → 抽出 → 承認)からマイクボタンに変更。途中結果を入力欄へ流し込み、最終結果(無音で自動停止 / 停止ボタン)で認識中を終える。固定文の疑似応答と抽出(`VoiceExtractItem` / `MockExtractItems`)は削除し、`OllamaApiClient` を `IChatClient` として直接生成 |
| `Modules/Sample/SampleMenuViewModel.cs` + `SampleMenuView.xaml` | Chat は `ChatCommand`(Ollama 未設定なら「Ollama end point is not configured.」を出して遷移しない) |
| `Services/AiChatClientFactory.cs` | 削除 |
| `Modules/Device/DeviceMiscViewModel.cs` + `DeviceMiscView.xaml` | 音声認識の `IsListening` を最終結果で終える(`RecognizeAsync` は認識の開始で戻る)。ボタンは Recognize / Stop のトグル |
| `Messaging/CameraController.cs` / `Modules/Sample/SampleCvNet*ViewModel.cs` / `Modules/Device/DeviceCameraViewModel.cs` / `DeviceOcrViewModel.cs` | `CaptureWithTimeoutAsync`(5 秒で打ち切り)。撮影できなければ「撮影できませんでした。もう一度お試しください。」を出してプレビューのまま続行 |
| `Modules/Sample/SampleCvNet*View.xaml` + `ViewModel.cs` / `Modules/Sample/CaptureState.cs` / `Converters/MapToBoolConverter.cs`(新規) | VM は状態 `CaptureState`(Preview / Capturing / Analyzing / Result)だけを持ち、F2〜F4 の文言・有効とカメラ / 画像の表示は XAML のコンバーターで決める(`MapToBoolConverter` = `Smart.Maui` の `MapToObjectConverter<bool>` 派生、`s:MapToTextConverter`、`s:CompareToText`、`EqualsConverter`)。撮影中 / 解析中は F2〜F4 が空で無効、解析が終わってから Retry |
| `State/Settings.cs` | 通信系の設定が投入済みかの判定を拡張メソッドに集約(`IsApiConfigured` / `IsGrpcConfigured` / `IsAIServiceConfiguredAsync` / `IsOllamaConfigured` / `IsScpConfigured`)。Network メニュー / Realtime / gRPC / SCP / CV Net メニュー / Sample メニュー / `MauiProgram` が使う |
| `Usecase/ScpUsecase.cs`(新規)/ `Services/ScpService.cs`(削除)/ `Modules/Network/NetworkScpViewModel.cs` | SCP はファイル選択(FilePicker)/ 保存先(公開フォルダ)/ 接続情報(設定)の取り出しと SSH.NET の転送を `ScpUsecase` に。結果は `ScpUploadResult` / `ScpDownloadResult`(ファイル名 / サイズ / 転送結果 = 成否・例外メッセージ・指紋)で、文言は VM |
| `Usecase/OnnxVisionUsecase.cs`(旧 `CognitiveUsecase.cs`)/ `Usecase/DetectResult.cs` | `AzureVisionUsecase` と対の名前に変更。共有の `DetectResult` は独立ファイル |
| `Services/ChatRoomClient.cs`(旧 `Services/Chat/ChatClient.cs` + `ChatConnectionState.cs` / `ChatMessageEntry.cs` / `ChatMessageEventArgs.cs` / `ChatStateEventArgs.cs`)/ `Services/Protos/`(`chat.proto` / `server.proto`) | gRPC チャットのクライアントを `ChatRoomClient` に改名(`Microsoft.Extensions.AI.IChatClient` との区別)し、関連型を 1 ファイルにまとめて `Services` 直下へ。proto は `Services/Protos/`(csproj の `Protobuf` を追従) |
| `Usecase/AzureVisionUsecase.cs` / `Modules/Sample/SampleCvNet*ViewModel.cs` | 例外(`RequestFailedException` / `HttpRequestException` / `InvalidOperationException`)は Usecase の中で処理し `Result<T>`(`Error` = 例外のメッセージ)で返す。VM は `IsSuccess` で分岐して失敗をダイアログ表示 |
| `Modules/Main/SettingViewModel.cs` | QR を 1 回読んだら 3 秒間は検出を止める(同じ QR が映り続けても繰り返し読まない) |
| `Usecase/ScpUsecase.cs` / `Modules/Network/NetworkScpView.xaml` | キャンセルは切断ではなく転送中のストリーム操作(`CancellationStream` の Read / Write)で例外にする(`ScpClient` の同期 API は切断しても抜けないことがある)。失敗・キャンセルしたダウンロードは途中までのファイルを削除。保存先は公開フォルダ(説明文も) |
| `Services/Calendar/SampleDataBoundary.cs` | カレンダー系だけが使うため `Services/Calendar/` へ |
| `Helpers/ReactiveSignalR.cs` / `Services/MonitorConnection.cs` | `ReactiveSignalR` をクラスにして `HubConnection` の生成(`HubConnectionBuilder` / 再接続ポリシー / KeepAlive・ServerTimeout)・維持・破棄・`On<T>`(接続を作り直しても購読は続く)・`InvokeAsync`(未接続は false)・`IsConnected` を持たせ、`CreateRetryPolicy` などの公開をやめた。`MonitorConnection` はハブのパスと待ち時間、型付きの受信ストリーム、報告、ログだけ |
| `Modules/Basic/BasicValidationViewModel.cs` / `Modules/Network/NetworkGrpcViewModel.cs` / `NetworkHttpViewModel.cs` / `NetworkScpViewModel.cs` / `NetworkStorageViewModel.cs` / `Modules/Sample/SampleChatViewModel.cs` / `SampleMap2ViewModel.cs` / `Modules/UI/UIChatViewModel.cs` / `UIScheduleViewModel.cs` / `Modules/View/ViewBorderViewModel.cs` | `PropertyChanged += ...` の分岐を、`[ObservableProperty]` が生成する `SubscribeXxx(action)`(`ObserveXxx()` の購読を `Disposables` に登録)へ |
| `Modules/Sample/SampleChatViewModel.cs` / `SampleCvNet*ViewModel.cs` / `SampleCvLocalViewModel.cs` / `Modules/UI/UITreeMapViewModel.cs` / `Modules/View/ViewDrawingViewModel.cs` / `Modules/Device/DeviceAudioViewModel.cs` / `DeviceWiFiViewModel.cs` / `Modules/Network/NetworkRealtimeViewModel.cs` / `NetworkHttpViewModel.cs` / `NetworkScpViewModel.cs` / `NetworkStorageViewModel.cs` | `Dispose(bool)` の上書きをやめ `Disposables` へ(画像の解放は `DelegateDisposable`、`IChatClient` は `Disposables` に登録、購読の入れ物は `SerialDisposable` のプロパティ、転送のキャンセルは `CancellationTokenSource` を `using` にして `Action? cancel` だけ持つ)。`UIMeterViewModel` は `PeriodicTimer` / `CancellationTokenSource` を画面表示中だけ生成するため上書きのまま |
| `Helpers/CancellationStream.cs`(`ScpUsecase` の内部クラスから) | 同期の転送 API をキャンセルするためのストリーム |
| `Smart.Maui/Maui/Data/MapToObjectConverter.cs`(`Works3/Smart.Maui`) | `MapToBoolEntry` / `MapToBoolConverter` を追加(アプリの `Converters/MapToBoolConverter.cs` は削除、CV Net の XAML は `s:` に) |
| `MauiComponents/Speech.SpeechService.cs`(`Works3/MauiComponents`) | Android の CommunityToolkit `SpeechToText` は無音で自動停止しても状態が `Stopped` に戻らず 2 回目の `StartListenAsync` が何もしない、停止(`StopListenAsync`)は認識器を破棄するため最終結果が届かない。開始前 / 最終結果後に `StopListenAsync` で状態を戻し、停止 / 取り消しでは途中結果を最終結果として通知する(Sample > Chat の 2 回目の音声入力と停止、Device > Misc の Stop) |
| `Template.MobileApp.csproj` | `Microsoft.Maui.Controls` / `Controls.Maps` / `Essentials` 10.0.101(Smart.Maui 2.26.0 に合わせる) |
| `Modules/Sample/SampleCvNetView.xaml(.cs)` + `SampleCvNetViewModel.cs` / `VisionFeature.cs`(新規)、`SampleCvNetMenu*` / `SampleCvNetObject*` / `SampleCvNetTag*` / `SampleCvNetPeople*` / `SampleCvNetOcr*`(削除)/ `Modules/ViewId.cs` / `Modules/Sample/SampleMenuView.xaml` + `SampleMenuViewModel.cs` | CV Net の 4 画面とサブメニューを 1 画面に統合。解析の種類(`VisionFeature`)は画面下(機能キーの上)の選択ボタンで切り替え、撮影済み(`Result`)ならプレビューに戻って撮影からやり直す(タグのパネルも消す)。選択中の色は `s:CompareToColorConverter`、撮影中 / 解析中は無効。Sample メニューは `CvNetCommand`(AI 未設定の案内)で直接遷移。機能キーは撮影中 / 解析中も Out / In / Detect の文言のまま無効にし、解析が終わって `Result` になったら Out / In を空、F4 を Retry にする(`s:CompareToText` で `Result` と比較) |
| `Behaviors/CameraBind.cs` + `CameraBind.android.cs`(新規) | カメラのズーム(Device > Camera の ZoomIn / ZoomOut、CV Net の In / Out)が効かなかった修正。CommunityToolkit の `CameraInfo.MaximumZoomFactor` は Android で常に 1(`ZoomState` の取り出しが `as` キャストで失敗)で、`CameraView.ZoomFactor` もその値で丸められる。`CameraBind.CorrectCameraInfo`(Android の partial)が CameraX の `ProcessCameraProvider` から同じカメラの `ZoomState`(`JavaCast<IZoomState>`)を取り、正しい範囲の `CameraInfo` を作り直す。`CameraBind` は一覧を補正して返し、プレビュー開始時に接続時の選択(補正前)を補正済みに入れ替える(同じ DeviceId は等しいと判定されるため、いったん null を入れてから差し替え) |
| `Services/Calendar/`(`HolidayService.cs` / `IScheduleEventProvider.cs` / `ScheduleService.cs`) | カレンダー系のサービスをサブフォルダへ(名前空間 `Services.Calendar`) |
| `Document/Task_Checklist.md` / `Change_Summary.md` / `Telemetry_Study.md` / `Other_App_Candidates.md` | 章題(全階層)と項目(`【判断】`印 = ⚖️、区間の概要表のリンク)に内容を表す絵文字を付けた。絵文字は見出し文字列の直前に空白なしで置く(GitHub の見出しアンカーが変わらない。概要表からリンクする区間見出しは異体字セレクタ無しの絵文字)。`Development.md` は `#` 階層に付いていた既存のまま |
| `Document/Task_Checklist.md` / `README.md` | 確認済みの項目(旧 0-1 / 0-4 / 0-6 / 0-8、SCP / 設定画面 / Network メニュー / QR / `OtelEndPoint` の確認)を削除。ネットワーク系の確認を機能ごと(0-1 共通 / 0-2 REST / 0-3 UL・DL / 0-4 SignalR / 0-5 gRPC)に分割。旧 1 節(`tmpl-plan-maui.md` からの移管課題)は解体し、1 節 = 取り込み候補(リンク集 1-1〜1-6 に番号を詰め、小さな追加項目 = 起動状態と再試行画面 1-7 を案A(`MainPage` のオーバーレイ)/ 案B(専用画面)の判断付きで追加)、2 節 = アプリ構成(機能プロファイル / 診断画面 / カタログ / 遅延初期化)、3 節 = OpenTelemetry(旧 0-5 → 3-1、組み込みの判断 = 3-2)、4 節 = プッシュ通知(旧 0-2 → 4-1、FCM → 4-2)、5 節 = 生体認証、6 節 = オフライン同期の章立てに。低優先 = 3-2 / 4-2 / 5 節 / 6 節(その中では 3-2 を最初)。README の TODO は同じ順に並べ替え、OpenTelemetry を追加 |
| `Controls/ChatView.xaml(.cs)` / `Modules/Sample/SampleChatView.xaml` + `SampleChatViewModel.cs` | 応答中は送信ボタンが赤い中断ボタン(`IsResponding` / `CancelCommand`)に入れ替わり、ストリーミングを `CancellationTokenSource` で打ち切る(途中までの応答は残して「(中断)」、何も届いていなければ「中断しました。」。画面を離れるときも中断)。送信は BusyState の外(`MakeDelegateCommand` + fire-and-forget)で実行する(BusyState のオーバーレイが中断ボタンのタップを塞ぐため)。ストリームの読み取りと破棄は `Task.Run`(`ConfigureAwait(false)`)で行ない、中断後に出る `WebException`(Socket closed)は中断として扱う。送信ボタンは無効(入力が空)のとき灰色(`SendButton` の VisualState) |
| `Modules/Main/SettingViewModel.cs` | 設定値は全て `OnNavigatingToAsync` でまとめて取得(SecureStorage の 2 つも同じタイミング)。初期値は null にして表示前の反映ではハイライトしない(ハイライトは QR で値が変わったときだけ) |
| `Modules/Control/ControlChartViewModel.cs` / `ControlCollectionViewModel.cs` / `Navigation/Edit/EditListViewModel.cs` / `Network/NetworkGrpcViewModel.cs` / `NetworkHttpViewModel.cs` / `NetworkRealtimeViewModel.cs` / `NetworkScpViewModel.cs` / `Sample/SampleChatViewModel.cs` / `SampleCropViewModel.cs` / `SampleMap2ViewModel.cs` / `UI/UICalendarViewModel.cs` / `UIChatViewModel.cs` / `UIGraphViewModel.cs` / `UIGraph2ViewModel.cs` / `UIScheduleViewModel.cs` / `View/ViewEffectViewModel.cs` | ナビゲーションイベントの使い分けを揃えた(付録A): 表示前に済ませる処理(表示値・一覧・パラメータ)は `OnNavigatingToAsync`、表示が要る処理(権限 / 接続 / タイマー / スクロール / 表示後のアニメーション)は `OnNavigatedToAsync`。初回表示だけの処理は `Count == 0` やフラグでなく `context.Attribute.IsRestore()` で判定 |
| `Modules/Network/NetworkGrpcViewModel.cs` / `Modules/Sample/SampleChatViewModel.cs` / `Modules/UI/UIMeterViewModel.cs` / `Controls/RadarScreen.cs` / `Graphics/Scene/SceneObject.cs` | `MainThread.BeginInvokeOnMainThread` を `IDispatcher`(VM は DI 注入、コントロールは `Dispatcher`)に統一。async メソッド内は `await DispatchAsync`、`SceneObject` の描画ループは `Stop` が UI スレッドで待つため待たない `Post`(`Dispatch`)にした |
| `Modules/Main/DiagnosticsView.xaml(.cs)` + `DiagnosticsViewModel.cs`(新規)/ `Modules/ViewId.cs` / `Modules/Main/MenuView.xaml` / `Markup/AppIcons.cs` | 診断画面(メインメニューの Setting の上)。`InfoCard` の縦積み: Runtime(ワーキングセット / マネージドヒープ / スレッド数 / GC 回数 / プロセッサ数。先頭)、Application(名前 / バージョン / ビルド / パッケージ / flavor / 端末 / OS)、Startup(プロセス開始時刻 / 稼働時間 / 初期化時間)、Connection(API / gRPC / OTEL / AI / Ollama / SCP の設定有無、ログイン ID と有効期限、ネットワーク / 電池 = `DeviceState` を直接バインド)、Database(パス / サイズ(-wal 込み)/ 更新日時 / 各テーブルの件数)、Log(ファイル一覧、Warning 以上の直近 50 件、Share files / Clear recent)、Crash report(前回のレポート、Clear)。値は `OnNavigatingToAsync` で取得、F2 = Refresh。実行時情報(`Process.StartTime` / ワーキングセット / GC)とログ(ファイル一覧は `FileLoggerOptions.Directory`、共有は `IShare` の `ShareMultipleFilesRequest`、直近ログは `DiagnosticLogProvider`)は VM が直接扱う(ユースケースは置かない)。無効なボタンは文字と枠を薄くする(VisualState) |
| `Components/DiagnosticLogProvider.cs`(新規)/ `MauiProgram.cs` | Warning 以上の直近 50 件を固定長のリングバッファーに保持する `ILoggerProvider`(`DiagnosticLogEntry` も同じファイル。カテゴリは末尾の型名だけ、例外は型名とメッセージを付ける。取り出しは新しい順の配列)。`ConfigureLogging` で singleton + `ILoggerProvider` として登録 |
| `Services/DataService.cs` / `DataAccessor.cs` / `Services/DatabaseInfo.cs`(新規) | `DatabasePath`(`RebuildAsync` もこれを使う)、`GetDatabaseInfoAsync`(パス / サイズ(-wal 込み)/ 更新日時 / 各テーブルの件数 = `DatabaseInfo`)、`[Count]` の `CountDataAsync` / `CountWorkAsync` |
| `Helpers/CrashReport.cs` | `GetReport` → `GetLastReport`(起動時に表示済みの `crash.old.log` を優先)、`ClearReport` を追加 |
| `State/Settings.cs` / `State/StartupState.cs` | `IsOtelConfigured`、`CompletedAt`(初期化完了時刻) |
| `Converters/ByteSizeConverter.cs` / `DurationConverter.cs`(新規) | バイト数を B / KB / MB / GB に、経過時間を 1 分未満は秒・1 日未満は時分秒・以上は日数付きに |
| `Document/ClamCalendar_Plan.md`(新規。置換完了に伴い 2026-09-21 に削除) | カレンダー(月表示 / 日表示)を独立ライブラリ `ClamCalendar` 0.1.0(`D:\GitHub\Other-ClamCalendar`、ClamGrid と同じ構成)へ分離する実装プラン。リポジトリ構成 / ライブラリのファイル構成 / 公開 API(既存・変更・追加)/ `CalendarStyle` / テスト / Example / Template 側の置換 / 作業順 / 判断 P-1〜P-5。Task_Checklist 6 節 |
| `Controls/DayTimetableView.cs` / `Models/Sample/Calendar/TimetableCalculator.cs` / `Modules/UI/UIScheduleView.xaml` + `UIScheduleViewModel.cs` | 日表示に `StartTime` / `EndTime`(高さは範囲から決める)、`TimeSlotInterval`(罫線の間隔。時刻の文字は正時だけ)、`FreeSlotHighlightVisible` / `MinimumFreeSlotForLabel`、`EventTappedCommand`(後のレーンを優先するヒットテスト)を追加。範囲外のイベントは範囲へクランプ、カード内の文字はクリップして所要時間ラベルの分だけタイトルを詰め、2 行入らない高さのカードでは時刻行を省く。`TimetableCalculator.GetBusyTotal` を追加し、UISchedule の空き時間はこれで計算。イベントのタップでトースト |
| `Works3/AiSample` | ChatConsole を同じ構成に(`OllamaApiClient` を直接生成、未設定は終了、疑似応答と 4 ステップを削除し `VoiceInput` に)。`AiChatClientFactory.cs` のコピーは削除 |

- 診断画面はエミュレーター(Android 15 x64)で確認: 全カードの値、Network > Get server time の失敗が Log の Recent に Warning として出る、Clear recent、Share files で共有シート、Refresh、無効ボタンの薄色。実機は未確認(Task_Checklist 0-5)
- ビルド 0 エラー 0 警告(Debug)、AiSample 0 警告。実機: Chat = マイク → 「話しかけてください」→ 認識した文章が入力欄に入る → 送信で Ollama(Wi-Fi 経由 12321)がストリーミング応答、無音は自動で停止。CV Net Object = Detect 中は F4 が空、解析後に Retry。Device > Misc = Recognize → Stop → 無音で戻る。音声入力 = 連続 3 回の認識、途中の停止(途中結果が入力欄に残る)、無音の停止、停止直後の再開。CV Net = Detect(Object)→ Tag に切り替えるとプレビューに戻る → Detect でタグ(霧 91% ...)→ Retry でプレビュー。Network > Realtime = 接続 / 離脱で切断 / 再入で新しい接続 ID。HTTP の遅延キャンセル / Storage / WiFi / Audio の表示。Chat の中断 = 応答前(中断しました。)/ ストリーミング中(途中までの応答 + (中断))/ 中断せず完了 → 次の質問が文脈を引き継ぐ、いずれもクラッシュレポートなし。Setting = 画面を開いたときに ServiceKey / Password の行がハイライトしない。**SCP(sshd = 192.168.100.99、Task_Checklist 旧 1 節)**: QR で投入 → 接続先 `root@192.168.100.99:22` → アップロード 300 KB / 40 MB / 200 MB 完了(進捗バー、指紋 SHA256 表示)→ ダウンロード 300 KB / 200 MB 完了(公開フォルダ、md5 一致)→ 200 MB の転送中キャンセル(アップロード / ダウンロード)が「キャンセルしました」で戻り途中ファイルなし → 存在しないリモート名は「失敗: scp: ...: No such file or directory」。旧 1 節(1-1〜1-3)は削除し、2 節 → 1 節、3 節 → 2 節に繰り上げ

### 📡SignalR の接続維持の後勝ちと Dispose ガード(2026-09-21)

| 対象 | 内容 |
|---|---|
| `Helpers/ReactiveHubConnection.cs`(旧 `Helpers/ReactiveSignalR.cs`。`HubStatus` / `HubStatusKind` は同じファイル) | `ReactiveSignalR` を `ReactiveHubConnection` に改名。一度 `Connect()` を購読したら、初回接続の失敗でも接続後の切断でも、購読が破棄されるまで自動で接続し直し続ける汎用の接続維持部品(ハブ固有のパス / メッセージ名 / 型は持たない)。**後勝ち**: 新しく `Connect()` を購読すると前のセッション(`HubConnection`)を止めて破棄し、前の購読者は `OnCompleted` で終える(`On<T>` / `InvokeAsync` / `IsConnected` は新しい接続に追従、置き換えられた古い購読を後から Dispose しても何もしない)。新しい接続は前の接続の `StopAsync` / `DisposeAsync` の完了を待ってから始める(同時に 2 本の `HubConnection` を作らない)。**Dispose ガード**: `Dispose()` は動作中のセッションを止めて(購読者に `OnCompleted`)から内部の Subject を破棄し、その後の古い購読の Dispose は無視、`Connect()` / `On<T>()` / `InvokeAsync()` は `ObjectDisposedException`(`IsConnected` は false)。`Connect(Uri url, Action<HttpConnectionOptions>? configure = null, IObservable<Unit>? resume = null)` で `WithUrl(url, configure)` に `HttpConnectionOptions`(AccessTokenProvider / Headers / Transports など)を渡せる。`InvokeAsync(methodName, cancellationToken)` で引数なしのハブメソッドを(`[null]` を送らずに)呼べる(`InvokeAsync(methodName, argument, cancellationToken)` はそのまま)。初回接続の再試行と自動再接続の間隔は ctor の `retryDelays` で指定する(自動再接続の `IRetryPolicy` はこれから作る)。セッションの置き換えと Dispose は `Lock` で直列化(状態 / 受信の通知はバックグラウンドスレッドのまま) |
| `Services/MonitorConnection.cs` | `ReactiveHubConnection` に追従(`Connect(url, resume: resume)`)。ハブ固有の部分は変更なし |

- ビルド 0 エラー 0 警告(Debug)、inspectcode 0 件。実機(Pixel 9a、`adb reverse tcp:8080` / `tcp:9090`): Network > Realtime = 接続済み(接続 ID / サーバー時刻 / Connections 1)→ 再接続 5 回(続けて 2 回を含む)は毎回「Monitor stopped → 60〜100 ms 後に新 ID で connected」で Connections は 1 のまま(グラフに 2 の跳ねなし)→ 接続中に経路を落とす(`adb reverse --remove tcp:8080` + `adb reconnect`)と再接続中... + 'The remote party closed the WebSocket connection without completing the close handshake.' → 経路を戻して 11 秒後に新 ID で接続済み(自動再接続)→ 経路なしで画面を開くと接続中... + 'Connection failure'(0 / 2 / 5 秒のバックオフ)→ 経路を戻して 12 秒後に新 ID で接続済み → 離脱(Monitor stopped、VM の Dispose とリーク検出の回収)→ 再入で新 ID で接続済み → 前の購読を残したまま `Connect()` を再購読(後勝ち)5 回(続けて 2 回を含む)= 毎回、前の購読者が `OnCompleted`(Monitor stopped)→ 50〜115 ms 後に新 ID で接続済み、Connections は 1 のまま

### 📅月表示カレンダーの ClamCalendar 0.1.0 への置換(2026-09-21)

UI 1 > Calendar の月表示を自作の `Controls/CalendarView` から NuGet の `ClamCalendar` 0.1.0(`D:\\GitHub\\Other-ClamCalendar`、SkiaSharp 描画の月カレンダー)へ置き換えた。日表示(`Controls/DayTimetableView`)は Template のまま。

| 対象 | 内容 |
|---|---|
| `Template.MobileApp.csproj` | `ClamCalendar` 0.1.0 を追加 |
| `Controls/CalendarView.xaml(.cs)` / `Models/Sample/Calendar/Views.cs` / `DayKind.cs` / `CalendarSelectionMode.cs` / `ScheduleEvent.cs` / `ScheduleStyle.cs` / `EventPlacement.cs` / `Stamp.cs` / `StampPosition.cs` / `MonthViewBuilder.cs` | 削除(ライブラリの `ClamCalendarView` / `CalendarEvent` / `CalendarStamp` / `CalendarSelectionMode` / `CalendarMonthBuilder` へ)。`Models/Sample/Calendar/` に残るのは日表示の `TimetableCalculator` / `TimetableDay` / `TimetableEvent` |
| `Services/CalendarService.cs`(新規。`ICalendarService` + `CalendarService`) | 旧 `Services/Calendar/`(`IScheduleEventProvider` / `ScheduleService` / `HolidayService` / `SampleDataBoundary`)を 1 ファイルに統合(フォルダは削除)。`GetEvents` / `GetStamps` は `CalendarEvent` / `CalendarStamp` を返す(`Id` → `Key`、`Underline` は廃止、スタンプのサイズ / 不透明度は float)、`GetHolidays`、生成の下限判定は private |
| `Modules/UI/UICalendarViewModel.cs` | `MonthViewBuilder` / 年月の保持 / Prev / Next をやめ、`DisplayDate`(TwoWay)と `DisplayDateChangedCommand`(表示範囲のイベント / スタンプ / 祝日を差し替え。ナビゲーション中に呼ばれるので `CommandBehavior.AllowBusyExecution`)に。`GoToTodayCommand` は `DisplayDate` を今日にする。`DayTappedCommand` / `EventTappedCommand` は `CalendarDayEventArgs` / `CalendarEventEventArgs`。`CommandBehavior` は `System.Data` と衝突するので using エイリアス |
| `Modules/UI/UICalendarView.xaml` | `controls:CalendarView` → `clam:ClamCalendarView`(`DisplayDate` / `DisplayDateChangedCommand` / `Events` / `Stamps` / `Holidays` / `MonthIndicatorVisible` / 選択のバインド)。選択モードは `SingleDate` / `MultipleDates` / `DateRange`。ヘッダは年月 1 行(既定スタイル) |
| `Modules/UI/UIScheduleViewModel.cs` | `ICalendarService.GetEvents` の `CalendarEvent`(`Key` / `CalendarEventStyle`)から日表示のイベントを組み立てる |
| `MauiProgram.cs` | `AddSingleton<ICalendarService, CalendarService>()`(旧 2 登録を置換) |
| `Document/UI_Calendar.png` / `README.md` / `Document/Task_Checklist.md` / `Document/ClamCalendar_Plan.md` | 画像を撮り直し、Implement の Control 行に Calendar(ClamCalendar)、Libraries に ClamCalendar、TODO と 6 節を削除、プラン文書は削除(ライブラリの仕様は `Other-ClamCalendar/Document/API.md`) |

- ビルド 0 エラー 0 警告(Debug)、inspectcode 0 件。実機(Pixel 9a)UI 1 > Calendar: 9 月のイベント / スタンプ / 祝日 / 月番号の透かし、Single で日タップのトースト(`2026/09/24`)、イベントタップのトースト(`燃えるゴ`)、Range の帯(24〜26)、▶ で 10 月(イベントが差し替わる)、スワイプで戻る、11 月から Today で 9 月へ。UI 1 > Schedule は変更なし(英会話 / 週間報告 / 買い物)
- 見た目の差: ヘッダが年 + 月の 2 段から `2026年9月` の 1 行(既定の `CalendarStyle`)、フォントが Noto Serif JP から端末の sans-serif

### 🗂️Grid / Visit(旧 Card List)の UI 1 への移動(2026-09-21)

Control の Grid(受注一覧)と Card List(訪問先一覧)は業務画面の見本なので UI 1 へ移し、Card List は Visit に改名した。UI = 画面デザインの見本、Control = 部品の使い方の見本。

| 対象 | 内容 |
|---|---|
| `Modules/UI/UIGridView.xaml` + `UIGridViewModel.cs` / `UIGridColumnView.xaml` + `UIGridColumnViewModel.cs` / `UIGridStyles.cs` | `Modules/Control/ControlGrid*` から移動・改名(`ViewId.UIGrid` / `UIGridColumn`、戻り先は UIMenu1) |
| `Modules/UI/UIVisitView.xaml` + `UIVisitViewModel.cs` | `Modules/Control/ControlCardList*` から移動・改名(`ViewId.UIVisit`、タイトル Visit、戻り先は UIMenu1) |
| `Models/Sample/OrderInfo.cs` / `VisitInfo.cs` / `ColumnOptionAccessors.cs` | `Models/Control/` から移動(名前空間 `Models.Sample`。フォルダは削除) |
| `Modules/UI/UIMenu1View.xaml` | Row 3 = Grid \| Visit(`TableChart` / `ViewAgenda`)。Calendar \| Schedule 以降は 1 行ずつ下げ、9 段のまま(空きは TreeMap の右だけ) |
| `Modules/Control/ControlMenuView.xaml` | Row 4 = Bottom Sheet \| Drawer に詰め、Row 5〜8 は空き |
| `Document/UI_Grid.png` / `UI_Visit.png`(旧 `Control_Grid.png` / `Control_CardList.png`)/ `README.md` | 画像を改名、Implement の UI 1 行に Grid(ClamGrid)/ Visit / Calendar(ClamCalendar)、Control 行から削除 |

- ビルド 0 エラー 0 警告(Debug)、inspectcode 0 件。実機(Pixel 9a): UI 1 の Grid / Visit から各画面へ遷移し Back で UI 1 へ戻る、Grid の見出し長押しで Grid Column(Cancel で戻る)、Control メニューの並び

### 📡SignalR 接続維持の Mofucat.ReactiveHub 0.1.0 への置換(2026-09-21)

`Helpers/ReactiveHubConnection.cs` を NuGet の `Mofucat.ReactiveHub` 0.1.0(`D:\GitHub\Mofucat-ReactiveHub`、Mofucat.SerialIO と同じ構成、ライブラリ側にテスト 17 件と Example)へ置き換えた。

| 対象 | 内容 |
|---|---|
| `Template.MobileApp.csproj` | `Mofucat.ReactiveHub` 0.1.0 を追加(`Microsoft.AspNetCore.SignalR.Client` は `HubException` のため残す) |
| `Helpers/ReactiveHubConnection.cs` | 削除(ライブラリの `ReactiveHubConnection` / `HubStatus` / `HubStatusKind` へ。ライブラリ側の追加 = ctor の `IScheduler`、`Connect` の `build`(`IHubConnectionBuilder`)、`Status`(最新値を再生するホット。`HubStatusKind.Disconnected` を追加)、`ConnectionId`、`TrySendAsync` / `TryInvokeAsync`(引数 0 / 1 / 配列、未接続は false)、`InvokeAsync<TResult>`(未接続は `InvalidOperationException`)、`IAsyncDisposable`) |
| `Services/MonitorConnection.cs` | `using Mofucat.ReactiveHub`、`InvokeAsync` → `TryInvokeAsync`。ハブ固有の部分は変更なし |
| `Modules/Network/NetworkRealtimeViewModel.cs` | `using Mofucat.ReactiveHub`(`HubStatus` / `HubStatusKind`)。`Disconnected` は既定の「停止」表示 |
| `README.md` / `Document/Task_Checklist.md` | Libraries に Mofucat.ReactiveHub、0-3 の行を置換後の構成に |

- ビルド 0 エラー 0 警告(Debug)、inspectcode 0 件。実機(Pixel 9a、`adb reverse tcp:8080` / `tcp:9090`)Network > Realtime: 接続済み(接続 ID / サーバー時刻 / 10 秒ごとの報告)、再接続 3 回は毎回新しい接続 ID で Connections は 1 のまま、経路を落とす(`adb reverse --remove tcp:8080` + `adb reconnect`)と「再接続中...」+ 'The remote party closed the WebSocket connection' → 経路を戻して 3 秒で接続済み、経路なしで開くと「接続中...」+ 'Connection failure' のバックオフ → 経路を戻して 7 秒で接続済み、離脱 / 再入で新しい接続 ID。クラッシュなし

### 🧩.NET 10 の未適用 API の反映(2026-09-21)

| 対象 | 内容 |
|---|---|
| `Modules/Basic/BasicSettingView.xaml` + `BasicSettingViewModel.cs` | `DatePicker.Date` / `TimePicker.Time` を nullable(`DateTime?` / `TimeSpan?`、初期値 null = 未設定。Summary は `TargetNullValue` で「未設定」)にし、各行に「開く」(`IsOpen` を TwoWay でバインドし true にすると選択 UI が開く。閉じると false が戻る)と「クリア」(null に戻す。null のときは無効)を追加。言語の `Picker` にも「開く」。`RowButton` スタイル(Disabled は灰色) |
| `Modules/Control/ControlRefreshView.xaml` + `ControlRefreshViewModel.cs` | ヘッダに `Pull` スイッチ(`RefreshView.IsRefreshEnabled`)。オフでは引き下げても更新しない(一覧のスクロールはそのまま) |
| `Modules/Device/DeviceMiscView.xaml` + `DeviceMiscViewModel.cs` / MauiComponents `Speech.cs` + `Speech.SpeechService.cs`(サブモジュール) | `ISpeechService.SpeakAsync` に `rate`(`SpeechOptions.Rate`、0.1〜2.0)を追加し、Speech カードに `Rate` スライダー(0.5〜2.0、既定 1.0) |
| `Messaging/WebViewController.cs` | `WebResourceRequested` / `WebViewInitialized` を転送するイベント、戻り値なしの `InvokeJavaScriptAsync(methodName, paramValues, paramJsonTypeInfos)` |
| `Modules/Sample/SampleWebBasicViewModel.cs` / `Models/Sample/WebLocalInfo.cs` / `SampleWebJsonContext.cs`(`string` / `WebLocalInfo` を追加) | ページからの `local/info.json` に `WebResourceRequested` でアプリ名 / バージョン / 端末 / 時刻の JSON を返す(同期で `SetResponse`)、`WebViewInitialized` で Android の UserAgent を表示、F3 = `Add`(戻り値あり)→ `UpdateStatus`(戻り値なし)→ `ThrowError`(JS の例外は .NET の例外。型 `HybridWebViewInvokeJavaScriptException` は internal なので名前で判定) |
| `Resources/Raw/web-basic/index.html` / `other.html` | ブリッジは MAUI 10 が配信する `_framework/hybridwebview.js` を参照(同梱していた .NET 9 版 `scripts/HybridWebView.js` は `hybridWebViewHost` 未定義で生メッセージが両方向とも届かなかったため削除)。「Fetch local resource」ボタンと `ThrowError` を追加 |

- `WebView` の Android 全画面動画(`allowfullscreen`)/ JavaScript 有効・無効の platform-specific は `WebView` を使う画面が無いため対象外(付録B)
- ビルド 0 エラー 0 警告(Debug)、inspectcode 0 件。実機(Pixel 9a): Setting = 開くで日付 / 時刻 / 言語のダイアログが開き、選択で Summary に反映、クリアで「未設定」/ Refresh = Pull オフで引き下げてもインジケーターが出ず件数が変わらない、オンで更新 / Misc = 2.0x と 0.7x で読み上げ(TTS のディスパッチをログで確認)/ Web Basic = 生メッセージ両方向、`InvokeDotNet`(同期 / 非同期)、`local/info.json` の応答、F3 の 3 段(`Add(1, 2) = 3`、ページのログ更新、`JS error: InvokeJavaScript threw an exception: Error from JS`)、Web App は変化なし

### 📐レイアウト計測を DiagnosticPanel に追加(2026-09-21)

| 対象 | 内容 |
|---|---|
| `Shell/LayoutMetrics.cs`(新規) | `MeterListener` で Meter `Microsoft.Maui` の `maui.layout.measure_count` / `arrange_count`(`Counter<int>`)を購読し、`Take()` で区間の回数を返す。パネル自身の要素(`ClassId` = `Diagnostic`)の計測と、パネルの表示更新直後 100 ms(親のレイアウトが走る)の計測は除く。所要時間(`measure_duration` / `arrange_duration`。`ActivitySource` の購読が要る)は表示しない |
| `Shell/DiagnosticPanel.xaml` + `.xaml.cs` | Measure / Arrange の行(区間の回数。0 = 緑 / ≤ 500 = 橙 / それ以上 = 赤)。監視中だけ `LayoutMetrics.Start` / `Dispose`。生成時に自身と子孫の `ClassId` を `Diagnostic` にする |
| `Template.MobileApp.csproj` | Debug だけ `MetricsSupport=true`(.NET for Android は `Microsoft.Android.Sdk.DefaultProperties.targets` で `Meter.IsSupported` を既定 false にしている) |
| `MauiProgram.cs` | DEBUG で `builder.Services.AddMetrics()`(MAUI の `DiagnosticsManager` は `IMeterFactory` があるときだけ Meter を作る) |

- ビルド 0 エラー 0 警告(Debug)、inspectcode 0 件。実機(Pixel 9a): 静止時は Measure / Arrange とも 0(緑)、メニュー遷移で 61〜217 回が 1 秒だけ出て 0 に戻る。パネルの幅は変わらない(値の列は Memory の値が最長)
- Release は `MetricsSupport` 既定(false)のままで Meter は作られない(パネル自体が DEBUG 限定)

### 🧱Material 3 の評価と見送り / StyleClass / global xmlns の見送り(2026-09-21)

| 対象 | 内容 |
|---|---|
| `Template.MobileApp.csproj` | `<UseMaterial3>true</UseMaterial3>` をコメントアウトで置く(採用時に外す)。有効化して Pixel 9a で確認した差分は `Document/Material3_Setting.png` / `Material3_Controls.png`(左 = 現状、右 = Material 3): Entry は outlined の `TextInputLayout` になり画面側の `Border` と二重枠、Editor は filled で高さ増、SearchBar / Slider / Switch / RadioButton / CheckBox / DatePicker ダイアログは Material 3 の形と既定の紫、Button / メニューは変化なし。採用時は `colorPrimary` の定義と Entry / Editor の枠の統一が要る |
| `StyleClass` | 共有 `Styles.xaml` の Label クラス(`LabelLeft` / `LabelCenter` / `LabelRight`、`LabelMicro` 12 / `LabelSmall` 14 / `LabelMedium` 18 / `LabelLarge` 24 / `LabelExtraLarge` 28 / `LabelHuge` 36、`LabelPrimary` … `LabelError`。Basic > Typography がカタログ)が付録A の方針(サイズ × 配置の直交する組み合わせ)そのものなので、新しいクラス体系は作らない。全画面への適用もしない |
| XAML の global xmlns(`http://schemas.microsoft.com/dotnet/maui/global`) | 見送り(ReSharper が対応するまで)。調査結果: `GlobalXmlns.cs` に `XmlnsDefinition` でアプリの名前空間(`Shell` / `Behaviors` / `Controls` / `Converters` / `Markup` / `Models.*` / `Modules.*` の各サブ名前空間 / `Fonts` など)を登録し 1 画面(`MenuView.xaml`)を切り替えるとビルド(XAML SourceGen)は通る(`{ViewId …}` / `{x:Static AppIcons.…}` / `ShellProperty.Title` / `{x:Type MenuViewModel}` が接頭辞なしで解決)が、ReSharper の XAML 解析は global xmlns を解決できず 1 ファイルで 193 件(`Ambiguous reference` / `Unable to resolve symbol`)。全面適用するときの制約: `Modules` 直下(列挙 `ViewId`)と `Markup`(`ViewIdExtension`)は同じ名前 `ViewId` で解決されるので片方だけを global に入れる、`Graphics.Drawing` の `Line` / `Rectangle` は Shapes と衝突するので接頭辞のまま、`Behaviors.Border` は MAUI の `Border` と衝突するので改名が要る。試行のファイルは元に戻し、`GlobalXmlns.cs` は削除 |

- ビルド 0 エラー 0 警告(Debug)、inspectcode 0 件

### 🚀起動オーバーレイ(2026-09-21)

| 対象 | 内容 |
|---|---|
| `State/StartupState.cs` | `IsCompleted`(`ObservableProperty`)を追加。`NotifyCompleted()` で `Completed` の完了と同時に true にする |
| `MainPage.xaml` | 初期化の完了まで全体を覆う起動オーバーレイ(白背景 + `ActivityIndicator` + 「起動しています」。`Startup.IsCompleted` の反転をバインド)。Grid の最後に置いて最前面にする(view container より前に置くと隠れて見えない) |
| `MainPageViewModel.cs` | `StartupState` を `Startup` プロパティで公開(`OnCreated` の完了待ちは従来どおり) |

- 初期化の失敗時の扱いは従来どおり(ダイアログ + 終了)。再試行・段階の表示は持たない
- 実機(Pixel 9a): 初期化の完了を 3 秒遅らせて表示を確認(スピナー + 文言 → Menu)。通常の起動は初期化が 0.7 秒ほどで、そのうち OnResume 後の glyph の準備(約 0.6 秒)は UI スレッドを占有するため、最初のフレームが出るのは完了の直前になり、オーバーレイはほとんど見えない(初期化が長くなったときの表示)
- ビルド 0 エラー 0 警告(Debug)、inspectcode 0 件

### 🌐通信処理の `NetworkUsecase` への統合と API 契約の整理(2026-09-21)

| 対象 | 内容 |
|---|---|
| `Usecase/NetworkInteraction.cs` / `NetworkOperator.cs` | 削除。共通処理を `NetworkUsecase` に統合 |
| `Usecase/NetworkUsecase.cs` | API ごとの処理 + 共通処理(`ExecuteCoreAsync`: 未接続の確認 / 呼び出し側の中断 / 401 の再ログイン再送 / 404 は結果として返す / エラー種別ごとの通知と再試行の確認)。`ExecuteAsync<T>`(インジケーター付き)/ `ExecuteTransferAsync`(UI なし)の 2 系統で、通知と再試行は verbose、401 の再ログインは `authenticated`(要認証 API = Secure だけが true)の引数で切り替える。型(typed / plain)による分岐はしない。ログインは `LoginCoreAsync` に集約(`EnsureLoginAsync` / 401 の再ログイン)。未使用だった進捗ダイアログ版の Download / Upload(`data.txt`)を削除。分岐の説明コメントは英語 |
| `Services/HttpService.cs` / `Template.MobileApp.csproj` | Rester 2.17.0 の `PutAsync` / `DeleteAsync` で更新 / 削除を呼ぶ(`HttpClient` 直接呼び出しの `SendAsync` と `System.Text.Json` の設定を削除) |
| `MauiProgram.cs` | Rester の JSON を camelCase(`JsonNamingPolicy.CamelCase` + 大文字小文字を区別しない)にし、サーバーの出力と揃える |
| `Services/Log.cs` | `InfoReLogin` / `WarnReLoginFailed` → `InfoLogin` / `WarnLoginFailed`(Id 付き) |
| `Modules/Network/NetworkHttpView.xaml` + `NetworkHttpViewModel.cs` | ログインカード(CRUD の 401 の確認用)を削除(CRUD は匿名になったため。Login / Logout / Secure はメニュー) |
| `MauiProgram.cs` | `INetworkInteraction` / `NetworkOperator` の登録を削除 |
| `Services/ChatRoomClient.cs` + `Protos/chat.proto` / `Modules/Network/NetworkGrpcViewModel.cs` | gRPC チャットの JWT 認証を廃止。`ConnectAsync(address, user)` でユーザー名(端末名 `IDeviceInfo.Name`)を受け取り各 `ChatMessage.User` で送る。`NetworkUsecase` の `EnsureLoginAsync` / `GetTokenAsync` を削除。proto の生成型は `Template.MobileApp.Services` 名前空間 |
| `Models/Api/`(削除)→ `Services/HttpService.cs` / `Services/MonitorConnection.cs` の先頭 | 契約の型は使う処理と同じファイルの先頭に置く(専用フォルダ・1 型 1 ファイルにしない)。REST は `<対象><操作>Request` / `Response` + 一覧の要素 `<対象>ListEntry`、SignalR は `<内容>Message`(`DeviceStatusMessage` / `ServerStatusMessage` / `NotificationMessage`)。サーバー側(`Endpoints/*Endpoints.cs` / `Hubs/MonitorHub.cs` の先頭)と同じ形 |
| (server) template-maui-server | 管理画面の Cookie 認証を削除(Account テーブル / パスワードプロバイダー / ログイン画面 / `Auth` 設定も削除、チャットの名前は入力欄)、Data の CRUD は匿名、要認証 API は `/api/secret/message` だけ、`/api/test/time` を削除(エラー / 遅延は残す)、gRPC チャットも認証なし(`ChatMessage.user` をそのまま使う。WPF サンプルは Server URL / login を撤去)、`Infrastructure/{Chat,Monitor,Notifications}` を `Services/` 直下へ移動(`Infrastructure` はアプリ固有でない機能だけ)、スキーマは `Assets/Data/Schema.sql` を起動時に `GenericAccessor.ExecuteSchemaAsync` で実行(各アクセッサーの `Create()` は削除)、並び順は `DataSort` 列挙 + `desc`(pos サーバーと同じ)、SQL の整形も pos に合わせた、`StorageEntry` は `Infrastructure.Storage` へ、Smart.Data.Accessor は 3.0.0-beta12、API の JSON は camelCase、契約 DTO は使う側のファイルの先頭(`Endpoints/*Endpoints.cs` は Models / Endpoints の区画、`Hubs/MonitorHub.cs`)、gRPC の生成型は `Handlers` 名前空間、proto は `Handlers/Protos/`、`RequestHelper`(並び順の列挙の解釈)、LIKE パターンは `Accessors/AccessorHelper`、サービスの設定は適用先の `*Options`(`Services/MonitorOptions` / `Workers/NotificationWorkerOptions`)、appsettings はパイプライン → サービスの順、`ApiRoutes`(`Prefix` 付き)は `Endpoints/`、`HubRoutes` は `Hubs/`。詳細は同リポジトリの README |

- 実機(Pixel 9a): Get server time = 成功ダイアログ、Error(500)= 再試行の確認 2 回 → 3 回目は通知のみ、Delay = 完了(ダイアログなし)、未ログインの Secure = 401 で再試行の確認、Login → Secure = 成功、期限切れトークンの Secure = 401 → 保存した Id で再ログインして再送 → 成功、HTTP 画面の未ログインの作成 / 取得 / 更新(PUT)/ 削除(DELETE)= 成功、10 秒待つ API のキャンセル = 「遅延 キャンセル」、Storage のダウンロード / 削除 = 成功、gRPC チャット = 未ログインで接続・送信(表示名 `Pixel 9a`)
- ビルド 0 エラー 0 警告(Debug)、inspectcode 0 件。サーバーはビルド 0 警告、単体テスト 37 件成功

### 📦`NetworkUsecase` の戻り値を `NetworkResult` に統一(2026-09-22)

| 対象 | 内容 |
|---|---|
| `Usecase/NetworkUsecase.cs` | 先頭の `// Result` 区画に `NetworkResultType`(Success / Disconnected / Canceled / NotFound / HttpError / Unknown)、`NetworkResult`(`Type` + `StatusCode` + `IsSuccess`)、`NetworkResult<T>`(+ `Value`)を定義。`NetworkOperationResult` / `NetworkErrorKind` / `NetworkError` record(`Smart.Results` の `Error` 派生)は削除。使用側に見せる情報はこの 3 型だけ |
| `Usecase/NetworkUsecase.cs` | 型付き API は `NetworkResult<T>`、本文の無い API(更新 / 削除 / テスト)と転送は `NetworkResult` を返す。`Classify` が `NetworkResultType` を直接返し、未接続は `Disconnected`。内部の分類と外に見せる種別が 1:1 なので内部専用の列挙は置かない |
| `Services/HttpService.cs` | 本文の無い API(`PutDataAsync` / `DeleteDataAsync` / `DeleteStorageAsync` / `GetTestErrorAsync` / `GetTestDelayAsync`)の戻りを `IRestResponse` に(Rester の `SendAsync(HttpMethod.Delete / Get, path)`。型なしの `GetAsync` / `DeleteAsync` は `HttpClient` のインスタンスメソッドに隠れるため Rester には無い) |
| `Modules/Network/NetworkHttpViewModel.cs` / `NetworkStorageViewModel.cs` | `FormatError(NetworkResult)` で `Type` と `StatusCode` を表示文字列にする(`NetworkError` へのキャストや `Error.Message` の参照は無し)。遅延 / 転送の switch も同じ列挙に |

- `Smart.Results` は `ExpressionCalculator` / `CropDrawing` / `AzureVisionUsecase`(値かメッセージ)でそのまま使う
- 実機(Pixel 9a): HTTP = 重複の作成 → 「名前が重複 (409)」、更新成功、サーバー側で削除した行の更新 / 削除 → 「見つからない (404)」、10 秒待つ API のキャンセル → 「遅延 キャンセル」。Storage = ダウンロード成功、サーバー側で削除後のダウンロード → 「転送失敗: 見つからない (404)」、削除 → 「削除失敗: … 見つからない (404)」。メニューの Error(500)= 再試行の確認 2 回 → 3 回目は通知のみ
- ビルド 0 エラー 0 警告(Debug / Mono)、inspectcode 0 件

### 🧭Network メニューの再構成と HTTP (Auth) 画面(2026-09-22)

| 対象 | 内容 |
|---|---|
| `Modules/Network/NetworkMenuView.xaml` + `NetworkMenuViewModel.cs` | メニューは時刻取得(直接呼び出し)と画面遷移(HTTP (Data) / HTTP (Auth) / Storage / Realtime / gRPC / SCP)だけに(1 列 7 行、残り 2 行は無効ボタン)。Data list / Secure / Login / Logout / Error / Delay のコマンドを削除 |
| `Modules/Network/NetworkHttpView.xaml` + `NetworkHttpViewModel.cs` | 題名を HTTP (Data) に。一覧カードに「全件を Work テーブルへ保存」、「遅延」カードを「テスト API」カード(エラー 500 / 遅延 5 秒 / 10 秒待つ API + キャンセル)に。結果はログへ(`エラー 500: HTTP エラー (500)` / `遅延 5 秒 完了`) |
| `Modules/Network/NetworkAuthView.xaml` + `.xaml.cs` + `NetworkAuthViewModel.cs`(新規)/ `Modules/ViewId.cs` | HTTP (Auth) 画面(`ViewId.NetworkAuth`): ログイン ID の入力(既定 `user`)/ ログイン / ログアウト / 状態表示(`ApiContext` のログイン ID とトークン期限)、Secure を呼ぶ / トークンを無効化 |
| `Usecase/NetworkUsecase.cs` | `InvalidateToken()`(ログイン ID は保持したままトークンを不正な値にする。次の認証付き呼び出しで再ログイン再送を確認するためのテスト用)。`DefaultLoginId` は削除(ダミー ID は VM 側) |
| `Markup/AppIcons.cs` | メニューで使わなくなった `ErrorOutline` / `HourglassEmpty` / `ListAlt` / `Logout` を削除 |

- 実機(Pixel 9a): メニュー 7 項目、Server time = 成功ダイアログ。HTTP (Auth) = 未ログインの Secure → 401 の再試行確認、ログイン → 期限表示、無効化 → 期限 `-`、Secure → 再ログインして「Hello user」(期限が更新)、ログアウト → 未ログイン。HTTP (Data) = エラー 500 の再試行確認 2 回 → 通知、遅延 5 秒 完了、全件を Work テーブルへ保存(47 件)
- ビルド 0 エラー 0 警告(Debug / Mono)、inspectcode 0 件

## 💡C. この区間のナレッジ

- **Android の `HttpClient`(`AndroidMessageHandler`)のストリーミング応答を中断するとき**: `await foreach` を UI スレッドで回すと列挙の破棄(ストリームの Close)がメインスレッドで実行され `NetworkOnMainThreadException`(未観測のタスク例外としてクラッシュレポートに残る)。接続待ちの間に中断すると `OperationCanceledException` ではなく `WebException`(Socket closed)。OllamaSharp は中断で例外を出さず列挙が終わることもある。読み取りは `Task.Run` + `ConfigureAwait(false)` で行ない UI 更新だけ `MainThread.BeginInvokeOnMainThread`、中断後の例外は `IsCancellationRequested` で中断扱いにする
- **Debug ビルドの APK は Fast Deployment のためアセンブリを含まない**(`adb install` しても古いコードのまま動く)。CLI からの配置は `dotnet build -t:Install -p:AdbTarget="-s <シリアル>"`。VS が `obj/Debug` をロックしているときは `-p:IntermediateOutputPath=obj\cli\net10.0-android\ -p:OutDir=bin\cli\net10.0-android\` で別ディレクトリにビルドできる(`Restart Manager` API でロック元を特定した)
- **`dotnet run` の Android 実機指定は `--device <シリアル>`**(.NET 10 SDK)。`-p:AdbTarget=-d` は効かず、端末が複数(実機 + エミュレーター)あると候補一覧を出して止まる。起動後は logcat を流し続ける
- **OllamaSharp の `OllamaApiClient` は `Microsoft.Extensions.AI.IChatClient` を実装する**(パッケージは推移参照で入る)。`Chat` ヘルパの代わりに履歴を呼び出し側で持ち `GetStreamingResponseAsync(history)` で送る。`ChatMessage` はアプリの `Models.Sample.Chat.ChatMessage` と衝突するので using エイリアスで避ける(SA1209: エイリアスは名前空間 using の後)
- **Rester の JSON 既定は camelCase(`JsonCamelCaseNamingPolicy`、大文字小文字を区別する)**: 命名が一致しないと配列プロパティが null のまま(`Entries` の NRE)になる。サーバーも camelCase(Minimal API 既定)にし、クライアントは `RestConfig.Default.UseJsonSerializer` で `JsonNamingPolicy.CamelCase` + `PropertyNameCaseInsensitive = true`
- **BusyState のオーバーレイは非同期コマンドの実行中の入力を全て塞ぐ**ため、キャンセルボタン付きの長い処理(転送 / 遅延 API)は `MakeAsyncCommand` にせず、`MakeDelegateCommand` から `_ = RunAsync()` で起動して自前のフラグ(`Transferring` 等)で再入を防ぐ。`MakeDelegateCommand` の既定(`CommandBehavior.None`)は Busy 中の実行を黙って捨てるので、コマンドの有効 / 無効だけでは判断できない
- **常時接続の再接続ループから呼ぶトークン取得に `NetworkOperator`(`IDialog.Indicator`)を通してはいけない**: バックグラウンドスレッドから UI を触って例外になり、`IsConnectionException` に該当しないためループが黙って死ぬ。`EnsureLoginAsync` は `HttpService` を直接呼ぶ
- **`CollectionView` の `EmptyView` を `ScrollView` 内の固定高さ(`HeightRequest`)の CollectionView に置くと `EmptyViewContentView` が `requestLayout` を繰り返し(logcat の `requestLayout() improperly called`)、常時再描画になる**(uiautomator dump も `could not get idle state` で取れない)。空表示は `IsVisible` を束縛した Label に置き換える
- **SignalR の接続維持は Rx で書ける**(`Helpers/ReactiveHubConnection.cs`): 接続試行 `Observable.FromAsync(StartAsync)` を `RetryWhen`(バックオフの `Timer` と復帰シグナルの `Amb`)で繰り返し、`Concat(closed.Take(1))` + `Repeat()` で `Closed` 後に初回接続からやり直す。購読 = 接続の寿命(破棄で `StopAsync`)なので、画面の VM は `OnNavigatedTo` で購読して `OnNavigatingFrom` で破棄するだけになる。`Reconnecting` / `Reconnected` / `Closed` は `Func<T, Task>` のイベントなので `Subject` で橋渡しする。後勝ち(新しい購読で前の購読を終える)は購読ごとの `TakeUntil(stop)` で `OnCompleted` を通常の通知と同じ直列化された経路から流し、前の接続の `StopAsync` / `DisposeAsync` の完了を `Task` で持って次の接続の `StartAsync` の前に待つ(同時に 2 本の接続を作らない)
- **`adb reverse` の経路を消しても確立済みの接続は切れない**(`adb reverse --remove` の後も接続済みのまま送受信が続く)。接続中の切断は、経路を消してから `adb -s <シリアル> reconnect` でホスト側のトランスポートを蹴ると再現できる(端末側の reverse 設定は消えるので `adb reverse` をやり直す)
- **MAUI 10 のレイアウト計測の計器は `Counter<int>` / `Histogram<int>`**(`MeterListener.SetMeasurementEventCallback<long>` / `<double>` では届かない)。Android は `MetricsSupport` が既定 false(`Meter.IsSupported` が false になり MAUI の `ConfigureMauiDiagnostics` が何も登録しない)、Meter は DI に `IMeterFactory`(`AddMetrics()`)があるときだけ作られ、所要時間(`Histogram<int>`、ns)は `ActivitySource` に `ActivityListener` があるときだけ記録される。`Microsoft.Maui.RuntimeFeature.EnableDiagnostics` は XAML 診断向けで無関係
- **SignalR クライアントが `Closed` になる条件**: `WithAutomaticReconnect` を諦めないポリシーにすると、切断はまず `Reconnecting` になり `Closed` は来ない(サーバーのプロセス kill / 再起動はこちら)。`Closed` が来るのはサーバーが再接続不可の Close を送ったとき(`HubCallerContext.Abort()`、`OnConnectedAsync` の例外)。管理画面の「切断」がこれで、`Closed` → 初回接続のやり直しの経路を実機で確認できる
- **SignalR の `DateTime` は Kind を失う**: サーバーの `GetLocalNow().DateTime`(Unspecified)を受けて `ToLocalTime()` すると UTC 扱いで +9 時間ずれる。時刻は `DateTimeOffset` で送り、クライアントは `.LocalDateTime` を使う
- **gRPC(h2c)は Android でも `GrpcChannel.ForAddress("http://…:9090")` だけで繋がる**(`SocketsHttpHandler` の HTTP/2)。API(8080)とポートが違うため接続先は `GrpcEndPoint` として別に持つ。サーバー停止時は `RpcException(Unavailable)`、gzip 圧縮しない生ボディのアップロードは Content-Length が付くので進捗が出る
- **`MediaPicker.PickPhotoAsync` は MAUI 10 で非推奨**(`PickPhotosAsync(new MediaPickerOptions { SelectionLimit = 1 })` を使う)。Android 16 のフォトピッカーは選択後に「完了」が要る
- **ラムダの引数 `(_, e)` の `_` は(1 つだけなら)破棄ではなく引数名**: 中で `_ = SelectAsync();` と書くとその引数への代入になる(ReSharper `AssignmentInsteadOfDiscard`)。引数名を `sender` にする
- **管理画面(Blazor Server)を自動操作する場合**: Cookie 認証のログインフォームはアンチフォージェリ付きのため、内蔵ブラウザで `form` を submit する。Devices / Chat は Interactive Server のためページ内操作にはブラウザが要る(curl 不可)
- **ローカル通知の自作**: Android 13 以降は `POST_NOTIFICATIONS` の実行時許可(MAUI の `Permissions.PostNotifications`)。通知のタップは `PendingIntent.GetActivity` で `MainActivity` を開き、`LaunchMode = SingleInstance` のため起動中は `OnNewIntent` に届く(extras の id は再処理を避けるため取り出したら消す)。`NotificationCompat.Builder` のバインディングは `Set*` の戻り値が nullable なのでメソッドチェーンにせず 1 行ずつ呼ぶ。`Context.NotificationService` 定数が Activity 内で型名 `NotificationService` を隠すので名前空間付きで参照する。正確なアラーム(`SetExactAndAllowWhileIdle`)は Android 14 以降は `SCHEDULE_EXACT_ALARM` が既定で不許可(`CanScheduleExactAlarms` で判定し、未許可は inexact = 数秒〜数分の前後)。Android 16 の通知シェードは同じアプリの通知をまとめる(グループ見出しのタップは展開)。ボタンのタップは通知を自動で消さないのでアプリ側で `Cancel` する
- **Azure AI Vision の SDK**: `ImageAnalysisOptions` は構造体なので省略時は `default` を渡す(`new()` は SA1129)。People は信頼度 0.0x の候補も大量に返すので閾値で切る。タグは `Language = "ja"` で日本語。Foundry の AI Services リソース(`*.services.ai.azure.com`)は Image Analysis 4.0 に応答するが Face API は 401(Face は対応リージョンの Face / AI Services リソースが必要)
- **Ollama を実機から使う**: PC の Ollama(127.0.0.1:11434)へ `adb reverse tcp:11434 tcp:11434` で接続先を `http://localhost:11434` にする。最初の要求はモデルのロードで 1 分近くかかる。`ollama list` は Windows ではサーバを自動起動する
- **設定の注入(実機テスト)**: アプリの `IPreferences` の実体は `shared_prefs/template.mobileapp_preferences.xml`(既定の SharedPreferences)、`SecureStorage` は `…microsoft.maui.essentials.preferences.xml`。`run-as` で前者にキーを平文で書くと、`Settings` の旧バージョン移行パスが起動時に SecureStorage へ移す
- **uiautomator dump が古い画面**: Chat(タイピング表示)も対象。IME が出ているとウィンドウが `adjust=pan` でずれるので、下部のボタンは IME を閉じてから座標でタップする
- **ナビゲーションのリーク確認**: Smart.Navigation の `PluginBase.OnClose` は Forward / Pop でビューが破棄されるときに呼ばれる(Push で積まれたビューは呼ばれない)ので、そこで `WeakReference` を取り、5 秒後に `GC.Collect` → `GC.WaitForPendingFinalizers` → `Java.Lang.JavaSystem.Gc` → `GC.Collect` の順で回してから生存を見る(Java 側の参照が残ると 1 回の GC では回収されない)。プラグインは Smart 側のコンテナが生成するため DI のコンストラクタ注入は使えず、ロガーは `ResolveProvider.Default` から遅延取得する
- **Wi-Fi スキャンの頻度**: 接続中の Android はアプリが何もしなければ定期スキャンをしない(`dumpsys wifiscanner` の要求元ログ: 有効化直後のフレームワーク、Wi-Fi 設定画面、位置情報サービス(動作中は約 2 分間隔)だけ。接続中 + 画面オフでは数時間スキャンが無い)。AP の出現 / 消失のイベントは無く、あるのは「誰かのスキャンが完了した」通知(`SCAN_RESULTS_AVAILABLE` / API 30 の `registerScanResultsCallback`)だけなので、一覧の鮮度を保つには自前の `startScan` が要る。他者のスキャン結果が届いたら自前のタイマーを延期すれば無駄撃ちを避けられる。外部スキャンの再現は `adb shell cmd wifi start-scan`。参考: Plugin.MauiWifiManager(exendahal/maui_wifi_manager)は API 28 以降 `startScan` を呼ばず `ScanResults` のキャッシュを 2 秒ごとに読むだけ(通知の購読なし)
- **ジェスチャナビゲーションと端のスワイプ**: Android 10 以降のジェスチャナビゲーションでは左右端からのスワイプをシステムが「戻る」として横取りし、アプリには届かない(`SfNavigationDrawer` の `EnableSwipeGesture` も同じ)。`View.SystemGestureExclusionRects` で除外できるが片側の上限は 200dp のため、帯の一部(上下中央 200dp)だけを除外する。矩形は View のローカル座標(px)で `Handler.PlatformView` に設定し、`SizeChanged` で更新する。`Android.Graphics.Rect` は `IDisposable` なので `using var` で作る(CA2000)
- **`SfSegmentedControl` の `SelectedIndex`**: バインドの既定は OneWay で、タップの選択は VM に戻らない。`Mode=TwoWay` を明示する
- **表示前の要素の高さ**: `IsVisible=false` の要素は `Height` が確定していない(−1)。表示と同時に高さを使うアニメーション(シートの開閉)は `SizeChanged` まで遅らせる
- **`BottomSheet` という型名は `Google.Android.Material.BottomSheet` 名前空間と衝突する**(CA1724)。`BottomSheetView` のように名前を変える
- **FlexLayout の折り返し**: 子の既定は `Shrink=1` で、幅が足りないと折り返さずに縮めて `TailTruncation` される。折り返したい Label には `FlexLayout.Shrink=0` を付ける
- **Wi-Fi のスキャン**: `getScanResults` は Android 13 以降 `NEARBY_WIFI_DEVICES`(+ 位置情報)が必要。`startScan` は非推奨だが動作し、前面アプリは 2 分に 4 回まで(超えると false、キャッシュ済みの結果は読める)。`ScanResult.ChannelWidth` は int(`CHANNEL_WIDTH_*`)で 320 MHz は Android 13 以降。結果の `Timestamp` は起動からの μs なので `SystemClock.ElapsedRealtime` との差で時刻に直す
- **Wi-Fi の情報取得**: Android 12 以降は `WifiManager.ConnectionInfo` が非推奨で、`NetworkCallback` に `IncludeLocationInfo` を付けて `NetworkCapabilities.TransportInfo`(`WifiInfo`)から取る。SSID / BSSID は位置情報の権限 + このフラグの両方が無いと `<unknown ssid>`。IP / ゲートウェイ / DNS は `OnLinkPropertiesChanged` の `LinkProperties`(IPv6 のリンクローカルが先に来るので IPv4 を選ぶ)。無線のオン / オフだけでは NetworkCallback は呼ばれないため `WIFI_STATE_CHANGED` を別に受ける。API 31 専用のコンストラクタは `[SupportedOSPlatform("android31.0")]` を付けて `OperatingSystem.IsAndroidVersionAtLeast(31)` で分岐(CA1416)
- **ClamGrid**: `GridStyle` の色コールバック(`CellColors` / `RowBackground`)はデリゲートなので XAML リソースにできず、C# の static プロパティ(`ControlGridStyles.ListStyle`)に組み立てて `x:Static` で `GridStyle` に渡す。**static プロパティの初期化子は色のフィールドより後に置く**(前に置くと null の色で組み立てられ、描画が止まって ANR になる)。行ヘッダは `AllowRowDragging` のときライブラリが取っ手を描くので `RowHeaderText` を重ねない。`ColumnOrders` / `SortOrders` は既定で TwoWay。列設定は `GridColumnConfigurationEventArgs.CreateEditSession()` を `PushAsync` の引数で渡し、`PopAsync` の引数で `Export()` を返して一覧側の `OnNavigatingToAsync`(restore)で受け取る
- **ClamGrid の `GridDataView` は行の変更通知ごとに全行を並べ替え直す**: 多数の行をまとめて更新するときは `Suspend()` / `Resume()` で囲む。`Resume` は Reset 扱いで選択が消えるため `UpdateSelection` で選び直す(スクロール位置は保持される)
- **ClamGrid の `GridColumn.Converter`(1.1.0)**: 文字セルの描画と自動幅の計測にだけ効く。ソートは `RegisterSort` の値、真偽セルと編集は生値。`s:MapToTextEntry` の `Key` に数値を書くときは `{s:Int32 n}`(文字列のキーでは `CompareTo` が例外)
- **内容に合わせた高さは `Measure(width, PositiveInfinity)`**: `Grid` の Star 行も無限の高さ制約では内容の高さで測られる。シートの高さ = min(内容の高さ, 上限)
- **ドラッグ終了の判定は合計の移動量だけで決めない**: 戻したドラッグも閉じてしまう。最後に動かした方向(1dp 以上の移動の符号)と離す直前 100ms の停止を見て、速さは逆方向に転じたら平均せず置き換える
- **Style の Setter にコンバーター付きのバインドを書ける**: `<Setter Property="BackgroundColor" Value="{Binding Status, x:DataType={x:Type module:VisitCard}, Converter={StaticResource StatusRowColorConverter}}" />` で値ごとの `DataTrigger` の列挙を 1 行にできる。上書きしたい状態(選択)だけ `DataTrigger` に残す。`s:MapToTextConverter` / `s:MapToColorConverter` の `Entries` は `Key` に列挙値を `x:Static` で書く(`IComparable.CompareTo` で照合するため文字列のキーでは例外)
- **`SfSegmentedControl` の文言切れ**: 既定の `SegmentWidth`(100)で切れる。`VisibleSegmentsCount` を指定すると幅を等分して全表示できる
- **Style 内の DataTrigger と x:DataType**: ページの `x:DataType` が VM のとき、DataTemplate 用 Style の `DataTrigger` の Binding は VM の型で解決されて MAUIG2045(反射バインド)になる。`Binding="{Binding IsSelected, x:DataType={x:Type module:VisitCard}}"` のようにバインド側で型を指定する
- **CollectionView のカード選択**: `SelectionMode=None` にして選択状態は項目側(`IsSelected`)で持ち、`TapGestureRecognizer` から VM のコマンドへ `CommandParameter="{Binding .}"` で渡す。複数条件の背景色(状態 → 選択)は `DataTrigger` を並べ、後に書いたものが勝つ性質で選択を最後に置く
- **Debug ビルドの APK からフォントが消えてアイコンが全て豆腐になる**ことがある(`FontManager: Font asset not found MaterialIcons-Regular.ttf`)。`obj/Debug/net10.0-android/resizetizer/` のフォント出力(`f/*.ttf`)と `assets/*.ttf` が無いのに `mauifont.stamp` が残っている状態で、インクリメンタルビルドがフォント処理を省略している。**`mauifont.stamp` と `resizetizer` フォルダを削除して再ビルド**すると復旧する。Button や Style の問題ではないので、アイコンが豆腐になったらまず APK 内の `assets/*.ttf` を確認する
- **`pm clear` は Debug ビルドのアプリを起動不能にする**。Fast Deployment のアセンブリは `/data/user/0/<pkg>/files/.__override__/<abi>` に置かれるため、データ消去で一緒に消えて `No assemblies found in ...__override__` で abort する(APK 内にはアセンブリが無い)。**再デプロイ(`-t:Install`)で復旧**する。併せて実行時パーミッションも全て取り消されるので `pm grant` で戻す
- **遷移の体感速度は「タップしたボタンが遷移後も生存するか」で変わる**。ページ内のボタンはページごと破棄されるためリップルが遷移と同時に止まるが、シェル側(`MainPage.xaml` のフッター等)のボタンは残るので、遅れて始まったリップルが新しい画面の上で再生され続ける。計測は `atrace --async_start gfx view input res` を取り、RenderThread の `CircleOp` の出現範囲を見る(リップルの描画オペ)。フレームの発生範囲は `dumpsys gfxinfo <pkg> framestats` の `IntendedVsync` / `FrameCompleted` を `/proc/uptime` と突き合わせてタップ基準に変換する
- **インクリメンタルビルドの残骸で起動直後にクラッシュを繰り返す**ことがある(`java.lang.IllegalArgumentException: No view found for id 0x… (template.mobileapp:id/labeled) for fragment NavigationRootManager_ElementBasedFragment`)。マネージドコードに入る前の `FragmentActivity.onStart` で落ちるためログにアプリの出力が残らない。**アンインストール、再インストール、端末再起動では直らず、`obj/Debug` と `bin/Debug` を削除してのクリアビルドで復旧**する。リソース ID の不整合なのでコード側を疑う前にビルド成果物を捨てる
- ソースジェネレータが生成するコンストラクタ(`[DataAccessor]` の `DataAccessor(IDbProvider)` 等)は同じコンパイル内の他のジェネレータ(BunnyTail の生成ファクトリ)からは見えない。`AddSingleton<T>()` の型登録だと CS7036 になる。生成コンストラクタは `[EditorBrowsable(Never)] internal` のためリフレクション系のフォールバック(`ActivatorUtilities` は public ctor のみ)でも解決できない。登録はアクセサ側のジェネレータが生成する `[DataAccessorRegistration]` メソッド(ファクトリ登録)で行う。BunnyTail からは生成された本体が見えないので型登録は生成されず、実行時はファクトリ記述子として扱われ、フォールバック報告にも出ない
- 予測型バック(D25 で現状維持): 自前の `OnBackPressedCallback` が有効なあいだはシステムのアニメーション(back-to-home / cross-activity)は出ない。`OnBackPressedDispatcher`(AndroidX Activity 1.9)が API 34+ で `OnBackAnimationCallback` を登録するため、最上位の有効なコールバックに `HandleOnBackStarted` / `HandleOnBackProgressed(BackEventCompat)` / `HandleOnBackCancelled` が届く。進捗はスワイプ 800px で約 0.7。**ボタン操作(3 ボタンナビ / `KEYCODE_BACK`)でも Android 17 では `Started` が `SwipeEdge = 2`(エッジなし)で来て、直後に `Pressed`、`Progressed` は来ない**
- ジェスチャーの検証: ナビゲーションモードは `adb shell cmd overlay enable-exclusive --category com.android.internal.systemui.navbar.gestural`(戻すときは `...navbar.threebutton`。`settings get secure navigation_mode` で 2 = ジェスチャー / 0 = 3 ボタン)。途中で止める・戻す操作は `input motionevent DOWN 3 y` → `MOVE x y` を刻む → `UP`(`input swipe` は一気に完了する)。縮小量はスクショの要素端の位置から算出できる
- 型引数なしの `AddSingleton(p => new DelegateDbProvider(...))` はラムダの戻り値型(`DelegateDbProvider`)で登録される。インターフェイスで解決させる登録は `AddSingleton<IDbProvider>(p => ...)` と型引数を明示する(漏れると起動時に `Unable to resolve service for type 'Smart.Data.IDbProvider'`)
- 自作 `Layout` の重なり順は Arrange 順では決まらない。子の `ZIndex` を `Layout.OnAdd` / `OnInsert` / `OnRemove` / `OnUpdate` で設定する(`ZIndex` の変更はハンドラ側の並べ替えだけで再レイアウトは起きない)
- `BindableLayout` はレイアウトの子を全て管理するため、静的な子と `ItemsSource` の子は同居できない。種別毎のモデル + `BindableLayout.ItemTemplateSelector` で 1 本にする。テンプレート毎の入場遅延はモデルのプロパティ(`EnterDelay`)にバインドする
- `HeightRequest` を持つ子は `Fill` でもセルいっぱいに広がらない(`ComputeFrame` が明示サイズを優先する)。タイル用のスタイルには `HeightRequest` を持たせない
- `GraphicsView` はタッチを消費する。親のジェスチャで受けたい重ね表示では `InputTransparent="True"` にする
- Avalonia の `Panel`(`MeasureOverride` / `ArrangeOverride` / `StyledProperty` / `AttachedProperty`)は MAUI の `Layout` + `ILayoutManager` / `BindableProperty(.CreateAttached)` に対応する。配置を `DesiredSize` から決定的に再計算する形にすると `Measure` / `ArrangeChildren` で同じ詰め込みを共有できる
- Foundry `gpt-image-2` の `images/generations` は JSON のみ(multipart は 400)、参照画像を渡す `images/edits` は multipart(`image[]`)。生成サイズは 1024x1024 / 1536x1024 / 1024x1536 のみで、目標サイズは生成後に切り出し・縮小する。既存作品に似た語や特徴(pocket monster 等)は安全フィルタで `moderation_blocked` になる
- VS Code の Markdown プレビューはワークスペース外(`../`)の画像を表示しない。ドキュメントから参照する確認用サムネイルは `Document/` 配下に置く
- `Border` の内容は Border の枠ではなく内容要素の枠でクリップされる(Android の `ContentViewGroup`)。内容の端にある要素を Scale で大きくする(Bounce 等)場合は、Padding を Border ではなく内側のレイアウトに持たせる
- `AspectFill` の商品画像はスロットの比率が合わないと被写体が欠ける。白背景の物撮りは `AspectFit` + Margin の余白付き中央表示にする
- `uiautomator dump` は常時アニメーションのある画面(Kit Dashboard / Social 等)で古い階層を返す。実機操作の画面判定は logcat の `Navigated: [from]->[to]` 行で行う。Onboarding の Back はフェード完了まで 2〜3 秒かかる
- `-t:Run` は adb サーバが落ちていると XAFD7000(接続拒否)で失敗する。`adb devices` でサーバを起動してから再実行する
- **ドロップ成功で元の行を作り直すと `DragGestureRecognizer.DropCompleted` は届かない**(BindableLayout がリストの Remove / Insert で行の View を再生成し、`ActionDragEnded` を受け取る元 View が消えるため)。ドラッグ状態の解除は DropCompleted だけに頼らず、各ドロップ処理の末尾でも行う。対象外へ落とした場合(元の View が残る)は DropCompleted が届く
- CommunityToolkit の `MaskedBehavior` は **`UnmaskedCharacter`(既定 `X`)の位置だけが入力欄**。数字を `0` で表したマスク(`000-0000-0000`)は `UnmaskedCharacter="0"` を付けないと全て固定文字になり何も入力できない
- `AppViewModelBase.Validate(name)` は失敗時に `AddError` するだけで成功時に消さない。入力の度に検証するときは `Errors.ClearErrors(name)` → `Validate(name)` の順に呼ぶ(Smart.Maui の `ValidateOnTextChanged` 添付プロパティも同じ理由でそのままでは消えない)
- **Microsoft.Maui.Graphics(Android)で `SetFillPaint` のグラデーションは `FillColor` を設定しても解除されない**(`FillPaintWithAlpha` は色を設定するがシェーダは残るため、以降の塗りが全てグラデーション色になる)。グラデーションで塗る区間は `SaveState` / `RestoreState` で囲む(状態の複製が破棄されるので元の Paint に残らない)
- **MAUI 10 の Android `SecureStorage` は `Remove` / `RemoveAll` も `EncryptedSharedPreferences` の生成を通る**ため、復号できない状態では `GetAsync` と同じ例外になる(`RemoveAll` は復旧手段にならない)。MAUI 側が捕捉するのは `AEADBadTagException`(キー単位)と `InvalidProtocolBufferException`(keyset 破損)だけで、Tink が keyset の復号に失敗して平文として読み直した結果の `GeneralSecurityException`(`empty keyset` 等)は素通りする。復旧は `Application.Context.GetSharedPreferences(alias).Edit().Clear()` で実体を消す
- **`UniformItemsLayout`(CommunityToolkit)はセルサイズを先頭の子の DesiredSize だけで決め、各子を `Measure(セル幅, セル高)` → `Arrange` する**。子ごとに Margin を変えて罫線を作ると、明示 HeightRequest とセル高の食い違いで 1dp の隙間が行によって消える。罫線が要る格子は `Grid` の Spacing とスペーサ行・列で作る(位置は同じ星サイズから決まるため、丸めで隙間が 0 にならない)
- SecureStorage の破損は `run-as <pkg>` で `shared_prefs/<pkg>.microsoft.maui.essentials.preferences.xml` の `__androidx_security_crypto_encrypted_prefs_key_keyset__` / `_value_keyset__` を `0800` にすると再現できる(`120a…` のような不正 protobuf は MAUI が捕捉するため再現にならない)
- MAUI 10.0.100 / Android のウィンドウは既定で `adjust=pan`(`dumpsys window windows` の `sim={adjust=...}`)。`App` のコンストラクタでの `Application.SetWindowSoftInputModeAdjust` は効かず、`MainActivity.OnCreate` の `base.OnCreate` 後の `Window.SetSoftInputMode` で切り替わる。ただし edge-to-edge(`SetDecorFitsSystemWindows(false)`)のため `AdjustResize` でもウィンドウは縮まず、IME の高さは `WindowInsets`(logcat の `WindowInsets changed ... ime:[0,0,0,1065]`)としてしか届かない。受け手が無いとフォーカス中の `Entry` はキーボードに隠れる
- `SafeAreaEdges` のインセット処理(`GlobalWindowInsetListener` / `SafeAreaExtensions.ApplyAdjustedSafeAreaInsetsPx`)は `adjust=pan` 中は `ContentPage`(`Default`)で消費される(`AdjustPan && bottom == 0 → Consumed`)ため、下位の `SafeAreaEdges="SoftInput"` やページの `All` は効かない。`AdjustResize` にすると `SoftInput` を付けた要素に画面上の重なり分だけ Padding が付くが、Padding では `onSizeChanged` が起きないので `ScrollView` はフォーカス要素へスクロールしない(`ScrollToAsync(MakeVisible)` もネイティブの Padding を知らない)。Toolkit の `StatusBarBehavior` が重ねる色 View はパンに追従して画面外へ出る
- `uiautomator dump` は IME ウィンドウの下にあるノードを出力しない(フォーカス中の `EditText` が出なければキーボードに隠れている)。IME の表示状態は `dumpsys input_method` の `mInputShown`
- **CoreCLR(`UseMonoRuntime=false`)では Shiny の `[Export]` ライフサイクルコールバック(`Shiny.Hosting.AndroidLifecycleExecutor.OnResume` / `OnPause`)で起動時にクラッシュする**(`A callback was made on a garbage collected delegate of type '__callback_factory__!callback_delegate__V::Invoke'`)。`Mono.Android.Export` が `[Export]` メソッド用に生成するデリゲートが JNI 登録後にルートされず、最初の `OnResume` までに GC が走ると落ちる(dotnet/android#10996。修正は .NET 11)。Mono では起きない。Android SDK 36.1.69 で 3 / 3 回再現、`[Export]` を使うのはアプリ内では Shiny.Core だけ(`MetadataLoadContext` で全アセンブリを走査)

---

# 📎付録(区間に紐付かない恒常情報)

## 📏付録A. 開発ポリシー(恒常・実装時は常に遵守)

- 共有 `Styles.xaml` は変更しない(**BasedOn 派生 or 新規リソース辞書**で対応)
- **`StyleClass` は文字サイズ × 配置のような直交する属性の組み合わせにだけ使う**(基本は BasedOn 派生。色や余白は Style 側。同じプロパティを Style と StyleClass の両方で指定しない。全面的なユーティリティクラスは採用しない)
- **View の code-behind 不使用**(Behavior / Trigger / VM / コントローラパターンで実装。再利用コントロールは `Controls/` に配置可)
- ビルド**警告ゼロ**(抑制が必要な場合は事前確認。Random の CA5394 のみファイル先頭 pragma の前例=UIRadarViewModel)
- フォントサイズは許可値のみ: `6, 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 32, 36, 48, 72, 96, 160`
- アイコンは `markup:Material` / `markup:Fluent`(生 Unicode・絵文字は使わない)。サイズは Material スケール(18/24/36/48)推奨
- メニューは空セル=「可視の無効ボタン」で統一済み。それ以外のメニュー差異は**意図的なので統一しない**(下表)
- Grid の `RowDefinitions`/`ColumnDefinitions` は **Style の Setter で定義せず Grid 側に個別記述**(2026-09-05 決定。ReSharper が Style 経由の定義を解決できず誤検知するため)
- ReSharper の XAML バインド誤検知(`x:Reference`/`RelativeSource` に `x:DataType` 指定済みで実動作は正常)は `<!-- ReSharper disable/restore Xaml.BindingWithContextNotResolved -->` で該当範囲のみ抑止
- コミットは実機確認後にユーザーが実施
- **設定項目の投入は設定画面の QR に統一**(`SettingParser` の `key=value`)。手入力 Entry は作らない
- **ナビゲーションイベントの使い分け**: 表示前に済ませたい処理(表示値の取得・一覧の準備・パラメータの取り出し)は `OnNavigatingToAsync`、表示後でよい / 表示が要る処理(権限要求・カメラ / センサー / タイマー / 接続の開始・スクロール・表示後のアニメーション)は `OnNavigatedToAsync`。初回表示だけの処理は `context.Attribute.IsRestore()` を明示的に見る(`Count == 0` やフラグで代用しない)
- **UI スレッドへの依頼は `IDispatcher`**(ViewModel は DI で注入、コントロールは `Dispatcher` プロパティ)。`MainThread` は使わない。async メソッド内は `await DispatchAsync`、待てない場所(UI スレッドで完了を待つ `Stop` がある描画ループなど)は同期メソッドに切り出して `Dispatch`
- `Document/*.md` の章題は内容を表す絵文字を先頭に付ける(見出し文字列の直前・空白なし。GitHub のアンカーが変わらない)。`Task_Checklist.md` の `【判断】` 印は `⚖️【判断】`

## ⚖️付録B. 意図的差異・統合しない判断

### 📋メニュー画面の差異一覧(2026-07-07 実態)

メニュー間の差異には**敢えて統一していない面がある**(ユーザー指示)。統一済みは「空セルの扱い」のみ。

| メニュー | ボタンスタイル | アイコン・絵文字 | 段数(行×列) | 空セルの扱い | ガード・その他 |
|---|---|---|---|---|---|
| Main/Menu(ルート) | `MenuButton`×9 | なし(「1.Basic」等の**番号プレフィックス**) | 9×1+フッタ | なし(全セル使用) | フッタに Flavor/Version 表示 |
| BasicMenu | `MenuButton`×9 | なし | 9×1 | 空の無効ボタン1(可視) | — |
| DeviceMenu | `MenuButton`×18 | なし | 9×2 | 無効ボタン2(WiFi/Biometric=**名前付き**・可視) | 遷移先は「Not implemented」パネル |
| NavigationMenu | `MenuButton`×11 | **絵文字プレフィックス**(🍇 Edit 等) | 9×2 | 空の無効ボタン3(可視) | — |
| NetworkMenu | `MenuButton`×12 | なし | 9×2 | 空の無効ボタン2(可視) | BaseAddress 未設定時は機能ボタン無効 |
| SampleMenu | `MenuIconButton`×11 | **Material**(`markup:MenuIcon`) | 9×2 | 空の無効ボタン7(可視)※旧・非表示→統一で可視化 | — |
| SampleCvNetMenu | `MenuIconButton`×5 | **Material**(`markup:MenuIcon`) | 9×1 | 空の無効ボタン4(可視)※旧・非表示→統一で可視化 | AI 未設定時はダイアログ |
| ViewMenu | `MenuButton`×18 | なし | 9×2 | 空の無効ボタン6(可視・灰色タイル) | — |
| UIMenu | `MenuIconButton`×30 | **Material**(`markup:MenuIcon`) | 10×3 | 空セルなし(全セル使用)※Profile2/Cockpit 廃止で 11 行→10 行に縮小 | XAML コメントで比較グルーピング |

- 残る差異(意図的に維持): ①アイコン=Material 3画面・絵文字 1画面・テキストのみ 5画面 ②番号プレフィックス=Main のみ ③列数=1/2/3列混在 ④入場アニメ・PressEffect は全メニュー未使用 ⑤ガード方式(Network=ボタン無効/CvNet=ダイアログ)

※ 表は 2026-07-07 時点の実態。その後の区間5で空セルの多くが結線された(Main=10 行化で App 追加 / Basic=Setting / View=Layout・DragDrop・State・Toolkit・Custom / Sample=Sf Chart・Crop / Network=SCP / UI=11 行化で Wheel)。**差異を統一しない方針自体は不変**。
※ 2026-09-03: メインメニューの**番号プレフィックスを廃止**し並び替え(View → Sample → UI → App → Setting 最後、UI 行のみ 2 列)。**UIMenu は UIMenu1(アプリ系 18)/ UIMenu2(可視化・計器・HUD 系 13)へ分離**(F4 相互遷移)。**メニューは 8 段以上を確保し、グループ毎に行を分けて余りセルを可視の無効ボタンにする形へ統一**(ユーザー指示)。
※ 2026-09-05: **メニュー規約を 9 段基本へ改定**(ユーザー指示)。メインメニュー=9 段×2 列(関連項目 Data\|Network / Sample\|App / UI 1\|UI 2 をペア行に・Setting 最終行・余り行は無効ボタン)+**全ボタンに Material アイコン追加**。UI 1/UI 2=**各 3 列×9 段**。2 列化は UI 1(18 ボタン=2 列×9 段の 18 セルちょうど)でグループ行分けが成立しないため見送り、**UI 1/UI 2 の列数は統一する**(ユーザー決定=片方だけの 2 列化はしない)。
※ 2026-09-12: **UI 1 / UI 2 を 2 列×9 段へ**(UI 1 = Profile \| Login / Money \| Super / POS \| Shop / Schedule \| Calendar / Timeline \| − / Mail \| Chat / Kit \| − / Graph \| Graph2 / TreeMap \| −、UI 2 = Stream \| Dock / Load \| Gauge / Meter \| Mixer / Monster \| Wheel / Character \| Social / Radar \| − / Flight \| Tactical / Telemetry \| Energy / 余り 1 行)。UIPet → UIMonster 改名、UIFeel 廃止(hex 配置は `HoneycombLayout` として View > Layout へ)。全メニューが 1 列または 2 列になった

### 🚫対応しない・保留と確定した項目(旧チェックリストから移設)

- **スコープ外**(外部リファレンス評価の前提): 生体認証 / カスタムハンドラ / App Actions / iOS / テーマ切替(AppThemeBinding) / 非同期検証 / セッションプロバイダ抽象
- **コードレビュー対応(区間2)での除外**: `OnNotifyFunction1` の 116 ファイル重複解消 / SemanticProperties・AutomationId の付与 / gRPC・SignalR・Ollama の実装 / QR コードからの通信先・API キー無検証受け入れ
- **保留**(必要になるまで扱わない): ダークモード対応 / ローカライズ整備 / iOS 対応 / DB マイグレーション機構
- アクセシビリティ(`SemanticProperties` / `AutomationId` の付与、TalkBack 確認)= 対応不要(2026-09-13)
- OAuth2 認可(`WebAuthenticator` + PKCE、旧 Task_Checklist 3-11)= 対応不要(2026-09-15)
- ディープリンク(App Links / カスタムスキーム、旧 Task_Checklist 3-1)= 本サンプル対象外(2026-09-15。別アプリケーションでの導入情報は `Other_App_Candidates.md`)
- SocialControls の TODO 整理 / TimeProvider の MAUI 方式 / Analyzers.ruleset の正典差分(旧 `tmpl-plan-maui.md` 3-9 / 3-10 / 3-12)= 対応不要(2026-09-17)
- 画面録画(`Plugin.Maui.ScreenRecording`)= 対象外(2026-09-19。実装を撤去)
- `WebView` の Android 全画面動画 / JavaScript 有効・無効の platform-specific(.NET 10)= 対象外(2026-09-21。`WebView` を使う画面が無い。Web の画面は `HybridWebView`)
- Material 3(`UseMaterial3`)= 見送り(2026-09-21。csproj にコメントアウトで残置。Entry / Editor の枠と既定色の手当てが要るため)
- XAML の global xmlns(接頭辞の省略)= 保留(2026-09-21。ReSharper が対応したら再開。ビルドは通るが inspectcode が解決できない。名前の衝突と範囲は区間 10 の記録)
- `StyleClass` の新体系(`text-*` など)= 不要(2026-09-21。既存の Label クラスで方針を満たす)
- Face(顔検出 / 顔識別、`Azure.AI.Vision.Face`)= 対象外(2026-09-19。Face API を持つ専用リソースが必要なため実装を撤去)
- Aspire 統合 / クラッシュレポート・テレメトリ基盤(旧 Task_Checklist 3-8 / 3-9)= チェックリストから分離し `Telemetry_Study.md` で検討(2026-09-15)
- ジェスチャナビゲーション時の左端スワイプによるドロワーの開閉 = システムの戻る操作が優先されるため保証しない(自作 `SideDrawer` は帯の上下中央 200dp だけ除外、`SfNavigationDrawer` は不可。ボタン / `IsOpen` で開く。2026-09-14)
- `Controls/ChatView` のバブル色バインダブル化(C-13 / D18)= 対応不要(利用箇所は `SampleChatView` のみ)/ `AnimationOption.ResetEnter` の Scale 固定リセット = 対応不要(静的 Scale と `EnterAnimation` の併用なし。併用が出た場合は `EnterBaseTranslationY` と同じ基準値退避で対処)
- Walkthrough(B-18)= 実装しない(D16)/ NavigationRail・月次集計(C-9/C-11)= 取り下げ(D10)/ Blazor(5-4)= 対応不要 / MBTiles(4-3)= 取りやめ(いずれも詳細は付録D と区間5 B-8)

### 🔀画面統合・類似性分析の結論

- **画面そのものの統合価値が高いのは3組のみ**: ①FlightHud/MechHud/Telemetry/Energy(完全同型)→ **独立維持で確定** ②Profile/Profile2 → **Profile2 ベースで UIProfile へ統合完了(2026-07-07・経緯は完了記録参照)** ③Timeline/Graph(同一 Git グラフの異表現)→ **Graph2 改名で両立(実施済み)**
- その他(EC 3画面・Stream 親子・Chat/Mail・UIKit 5画面・Meter/Radar/Social 等)は**画面マージ非推奨**。部品/スタイル共通化の候補は挙がったが**対応不要で確定**(2026-07-07)
- **Radar/HUD 技術メモ**(D6=別途対応の材料): レーダー描画が2実装ある(`RadarScreen`=MAUI Graphics+外部バインド / `FlightHudScreen` 内蔵=SkiaSharp 自走)。整理するなら SkiaSharp 側(Scene 化)へ寄せるのが自然。両 API を跨ぐ描画共有は不可
- メニュー非掲載の7画面(UIItem/UICart/UIStreamDetail/UIKitNotify/UIKitSetting/UIKitOnboard/UIKitTracking)は**親子フロー**であり統合対象ではない

## 🧰付録C. 資産レシピ表 — 既存資産の正確な名前(適用時のコピー元)

| 資産 | XAML での書き方 | 主な用途 |
|---|---|---|
| エントランス | `behaviors:AnimationOption.EnterAnimation="FadeUp\|Pop"` + `EnterDelay`(ms 段差) + `EnterTrigger`(再実行) | 静的カード/リスト行の入場 |
| 常時アニメ | `behaviors:AnimationOption.Pulse` / `Wave`+`WaveDelay` | 進行中ドット、待受/スキャン中の生感 |
| 変化フィードバック | `behaviors:AnimationOption.BounceTrigger`+`BounceValue` / `FlashTrigger` / `FadeInTrigger` / `HighlightTrigger`+`HighlightColor` | 値更新、追加/削除、タブ切替、CTA 押下 |
| バー伸長 | `behaviors:AnimationOption.ProgressTo`(ProgressBar を 800ms CubicOut で伸長) | ステータスバー・ゲージ |
| カウントアップ | `behaviors:LabelOption.CountUpValue`+`CountUpFormat`(+`CountUpDuration`) | 金額・件数・歩数(Loaded 数え上げ対応) |
| フォーカス枠 | `behaviors:Focus.FocusedStroke`+`FocusedThickness`(**親 Border 必須**) | Entry/Editor の入力体験 |
| 押下 | `behaviors:ButtonOption.PressEffect="True"`(Button/ImageButton)+`HapticFeedback` / `toolkit:SfEffectsView TouchDownEffects="Ripple"`(+`TouchDownCommand`/`TouchDownCommandParameter`) | 全タップ要素 |
| バッジ | `converters:BadgeCountConverter`(0→空、Max 超→「99+」) | 件数バッジ |
| アイコン | `{markup:Material Glyph={x:Static fonts:MaterialIcons.Xxx}, Color=.., Size=..}` / `{markup:Fluent ..}` / `{markup:MenuIcon ..}` | 絵文字・生 Unicode の置換(バインド不可な点に注意) |
| ステータスバー | `shell:ShellProperty.StatusBarColor="{StaticResource ...}"` + `StatusBarStyle="LightContent|DarkContent"`(未指定 = `MainPage.xaml` の既定 `BlueDefault` / `LightContent`) | ヘッダ非表示・全面画像の画面 |
| カード/チップ/ステップ | `controls:InfoCard`(Title/Icon/IconColor+Content)/ `controls:StatusChip`(Text/Icon/ChipColor/IconColor/TextColor)/ `controls:StepIndicator`(CurrentStep/TotalSteps/AccentColor) | 第2弾で新設した共通部品 |
| 空状態 | `CollectionView.EmptyView` / 中央 VStack+円形アイコン(96)+説明の定型 | 0件/未取得/未実装の表示 |
| その他 | `CameraOverlayView`(撮影ガイド枠)/ `MapBind`+`MapController(.MoveTo)` / `EasingCurveView` / `JetBrainsMono`(等幅数値)/ `NotoSerifJP`(Skia 日本語) | — |
| 重ね配置 / 重ねアバター | `controls:OverlapPanel`(OffsetX/OffsetY/ReverseZIndex)/ `controls:AvatarGroup`(ItemsSource/MaxDisplayed/Overlap/AvatarSize/CountBackgroundColor/CountTextColor。超過分は「+N」) | カード束、視聴中フレンド等 |
| 可変タイル / 円弧 | `controls:VariableSizeWrapPanel`(Columns/RowHeight/Spacing + 添付 ColumnSpan/RowSpan)/ `controls:CircularLayout`(Radius/StartAngle/SweepAngle/DistributeEvenly/FitToArc/RotateItems/ItemAngle/OrbitSpacing + 添付 Angle/Orbit) | ダッシュボードのタイル、半円メニュー、扇、同心円 |

## 🗃️付録D. 外部リファレンス評価 決定・不採用アーカイブ(旧 Reference_Analysis.md / Reference_Summary.md より)

51 件 (S-01〜S-51) を評価し、採用分は全て実装完了 (2026-09-01〜02)。QR ペイロード例や実装対象は各完了記録を参照。

### ⚖️決定事項 (D1〜D26)

| # | 決定 |
| --- | --- |
| D1 | NuGet は同等機能が既存に無い場合のみ追加 (実績: SSH.NET のみ。BlazorWebView 用は 5-4 不要化により追加せず) |
| D2 | アプリ風サンプルは `Modules/App/` + トップメニュー `10.App` (電卓 / 数独) |
| D3 | 電卓は科学電卓。複雑化時は ①四則+%+括弧 → ②三角関数等 → ③累乗・階乗 の順で削る |
| D4 | 設定画面サンプルは `Modules/Basic/BasicSettingView` |
| D5 | ミニゲームは数独 (盤面モデル差し替え可能にしてライフゲーム / 2048 に備える) |
| D6 | リスト D&D は `Modules/View/ViewDragDropView` |
| D7 | ロケールのみ強化 (9-3)・テーマ (`AppThemeBinding`) は触らない |
| D8 | ダブルバッファ = 試験導入 → Release 実測 (30fps→60fps) → **本採用・既定 ON** |
| D9 | タブ / ボトムシートは Syncfusion 本線 |
| D10 | TimeRecorder 由来は新規画面を作らず既存スケジュール強化のみ (9-2) |
| D11 | 非同期検証は実装しない (相関検証のみ) |
| D12 | 未使用機能サンプルは種別別新規画面 + 既存画面追記の併用 |
| D13 | Shiny Controls パッケージは参照しない |
| D14 | ChatView 機能追加は全て見送り。`RemainingItemsThreshold` は `ViewCollectionView` へ振り替え |
| D15 | Scheduler は `IScheduleEventProvider` 化のみ (9-2 で実施予定) |
| D16 | Walkthrough は実装しない (方針メモ: `Grid` 全面オーバーレイ + `Border` くり抜き + 対象要素の絶対座標取得 + `ScrollView` 内追従に注意 + 初回判定は `State/Settings.cs`) |
| D17 | 自作入力は `ColorPicker` / `DurationPicker` のみ (RangeSlider / AutoComplete は難度中で見送り) |
| D18 | チャット UI の二重実装 (`Controls/ChatView` ⇔ `UIChatView`) は現状維持。C-13 (バブル色) は**対応不要で確定** (2026-09-13) |
| D19 | 旧 `CalendarView` (未参照 1,490 行) の削除/リネームは**後日対応** (`CalendarView2` が正。C-14 のコメント実態合わせも同時) → **2026-09-06 実施済み**(区間 10) |
| D20 | SSH.NET 2026.0.0 追加 (増分 = BouncyCastle.Cryptography のみ) |
| D21 | SCP のみ (SFTP / コマンド実行は対象外) |
| D22 | 設定投入は設定画面の QR に統一 (全項目)。D22-a = パスワード認証のみ / D22-b = **指紋設定は撤去し参考表示のみ** (2026-09-02 変更。当初の QR 配布指紋照合は撤去) |
| D23 | 第2弾 (`Reference_Nova_Nalu.md`) N1 は OverlapPanel + AvatarGroup / CircularLayout の円弧 / VariableSizeWrapPanel の 3 件を採用確定。**CompareSlider は撤去** (2026-09-13) |
| D24 | 第2弾 N3 は Gravatar / Scratcher / Watermark / SegmentedSlider / TimelinePanel / ResponsivePanel / ToggleTemplate / ExpanderBox / DurationWheel を**不採用** (2026-09-13)。N3-6 は Radial / Orbit を `CircularLayout` の拡張 (RotateItems / Orbit) として採用、Bubble / Loop は不採用 (Hex は `HoneycombLayout` として実装済み)。N3-11 (タッチ横取り抑止 / 色パレット) は不採用。残る N2 (chrome / プラットフォーム 4 件) は 6-1 = 区間 10「ステータスバーの画面追従」/ D25 / D26 で完了、`Reference_Nova_Nalu.md` は削除 |
| D25 | 予測型バック (第2弾 6-4) は**現状維持で確定** (2026-09-13)。エッジスワイプ / BACK ボタン / フッタの Back は同じ経路 (`ShellEvent.Back` → `OnNotifyBackAsync`。フッタは `OnNotifyFunction1` から同じメソッドへ) で遷移し、スワイプ進捗に連動する縮小表現は入れない |
| D26 | Edge-to-Edge (第2弾 6-2) とキーボード (6-3) は**現状維持で確定** (2026-09-13)。`MainPage` は `SafeAreaEdges="Default"`、ウィンドウは既定の `adjust=pan` のまま、`IKeyboardState` は追加しない (確認結果は区間 10「Edge-to-Edge / キーボードの確認」) |

### 🚫不採用 (1) — サンプルとしては不要だが、ライブラリ / ツール / 資料としては有用

LiveCharts2 (自前 ChartDrawing + Syncfusion で充足) / Sharpnado.Tabs (SfTabView で充足) / Maui.VirtualListView・MPowerKit.VirtualizeListView (データ規模的に不要) / AiForms.SettingsView (BasicSettingView で達成) / MPowerKit.GoogleMaps (API キー前提。マネージャ分割設計のみ Mapsui 実装へ反映済み) / ArcGIS (商用) / Maui.Nuke (iOS スコープ外) / ImageCropper.Maui (ネイティブラッパ。自作 = 9-6) / Evergine 3D / DrawnUI 全面採用 (実験的。SKPicture キャッシュ等の部分技法は実装済み) / Grial FluentEmoji (CDN 依存) / CSLA (相関検証のみ 3-6 へ) / LocalizationResourceManager (根本切替は不要) / AlohaKit.Layouts (CircularLayout のみ自作済み) / TemplateMAUI (Marquee/TreeView のみ自作済み) / Plugin.Maui.SegmentedControl (SfSegmentedControl で充足) / GitTrends・WeatherTwentyOne・showcase (資料) / dotnet-maui-templates・MauiAppAccelerator (開発ツール) / Shiny Controls の DataGrid・FrostedGlass・Mermaid・Tray (コスト高 / デスクトップ向け) / Plugin.LocalNotification (thudugala。自作 Components/NotificationService で充足。NotificationRequest の項目構成 = Title / Description / BadgeNumber / Schedule / Android.ChannelId / ReturningData は参考) / MAUIHighSchool の Window.Stopped でローカル通知を予約する記事 (資料)

### 🚫不採用 (2) — 本サンプル側が優れた / 同等の実装を持つため参考自体が不要

AlohaKit.Controls (13/15 既存充足。設計思想も DrawingObject/DrawingControl として実装済み) / Grial SvgImage (SvgView が同構成) / The49 ViewClickListener・AiForms AddCommandEffect (TouchBehavior + ButtonOption で充足) / AiForms FAB (MapFabButton スタイルで充足) / slideshare 標準 UI 論 (UISocial 等で実装済み) / SimpleCalculator (ToTrimmedString のみ反映) / 数独記事 (題材のみ) / MauiScientificCalculator csproj (UI 構成のみ反映) / TimeRecorder アーキテクチャ (UI 要素のみ 9-2 へ) / PhotoAlbum バックエンド構成 / All the Lists (基準の明文化のみ 10-1 へ) / ライフサイクル記事 (実例のみ反映) / Doom.Mobile (Release 計測の知見のみ) / Breakout (プール / 論理解像度 / 状態機械を実装・反映済み) / Shiny の既存充足分 (Wizard / Parallax / SignaturePad / Toast / Shimmer / Badge / OTP / TreeView / CameraView ほか)

### 🚫不採用 (3) — 第2弾 (Nova.Avalonia.UI / Nalu。2026-09-13)

Gravatar・identicon (画像アセットは整備済み) / Scratcher / Watermark / SegmentedSlider (`Slider` + `SfSegmentedControl` で充足) / TimelinePanel (`UITimelineView` の行内描画で充足) / ResponsivePanel (縦画面固定) / ToggleTemplate (`IsVisible` 切替 + `DataTemplateSelector` で充足) / ExpanderBox (`mct:Expander` で充足) / DurationWheel (`DurationPicker` で充足) / HexPanel (`HoneycombLayout` として実装済み) / BubblePanel / LoopPanel (`CarouselView.Loop` で充足) / CompareSlider (実装後に撤去) / 親スクロールへのタッチ伝播停止 / WheelDrawing の色パレット差し替え / Nalu Scaffold 本体 (`Usa.Smart.Navigation` と競合) / Nalu VirtualScroll (商用は別ライセンス。`CollectionView` で充足) / Nalu Magnet (alpha) / Nalu のタブバー・ドロワー・共有要素トランジション / Nova の仮想化パネル 2 種 (`CollectionView` の役割) / Nova CodeViewer
