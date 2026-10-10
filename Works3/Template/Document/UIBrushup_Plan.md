# 🎨UI のブラッシュアップの計画

UI のブラッシュアップの残作業はこの計画で管理する(`Task_Checklist.md` には載せない)。ここが終わってから `Task_Checklist.md` の項目に戻る。済んだ項目はこの計画から消し、内容は `Change_Summary.md` に書く。

| 部分 | 番号 | 内容 |
| --- | --- | --- |
| 🖼️UI 1 / UI 2 と View の部品の画面 | 10-x | UI 1 / UI 2 の画面を、App のミニアプリと同じ水準(画面の中で完結する操作、下に余白を残さない配置、色と描画で華やかに、一覧で選んだ項目が詳細に出る)へ寄せる。View の部品の画面(Toolkit / Custom / Chart / Sf Chart / Bottom Sheet / Drawer)でしか使っていない部品は、使いどころのある UI の画面へ取り込む |
| 💫アニメーションの強化の案 | — | 既存の画面をアニメーションで強化する案(番号なし)。実施するものは 10-x / 11-x の項目(📝)にしてから進める |

ファイルパスは `Template.MobileApp/` からの相対。各項目の ①〜 は段階または修正の候補。

## 🔖凡例

| 列 | 絵文字 |
| --- | --- |
| 優先 | 🔴 先に行う(不具合・機能の欠け・見劣りが大きい)/ 🟡 作り込み / 🟢 小さな手直し・追加 / ⚖️ 実施の前に決めること |
| 変更量 | 📦📦📦 大(新しい画面・部品・モデル、作り直し、複数の画面や共有のスタイル)/ 📦📦 中(1 画面の組み直し、ViewModel・コンバーター・アイコンの追加も)/ 📦 小(1 画面の XAML・スタイルの手直し) |
| 状態 | 📝 案のまま(内容は未確認。実施の前に確認する。⑤📝 はその候補だけ)/ 👀 実施済み・確認待ち(①👀 はその候補だけ。確認待ちの候補と一覧の内容は取り消し線)/ 空欄 = 実施待ち |

## 📋一覧

### 🖼️UI 1 / UI 2 と View の部品の画面

| 番号 | 対象 | 優先 | 変更量 | 状態 | 内容 |
| --- | --- | --- | --- | --- | --- |
| 10-3 | UI 1 Money | 🟡 | 📦📦📦 | 👀 | ~~最近の取引、下のタブで中身を切り替え、支払いのシート(バーコード・QR)、前月比のバッジ、月の支出の棒と分類~~ |
| 10-4 | UI 1 Super | 🟢 | 📦📦 | 👀 | ~~クーポンの獲得、サービスのアイコンの反応、近くのお店~~ |
| 10-9 | UI 1 Chat | 🟢 | 📦📦 | 👀 | ~~写真の送信、タップでリアクション~~ |
| 10-11 | UI 1 Kit | 🟡 | 📦📦📦 | 👀 | ~~通知のスワイプ~~ |

### 🚀10-x の進め方

弾ごとにまとめて作り、実機で前後を撮って、まとめて確認を受ける。細部(色・大きさ)は既存の画面の形に合わせ、決めた細部は確認のときに示す。

| 弾 | 項目 |
| --- | --- |
| 第 1 弾 | 10-23 Custom・10-15 Load・10-16 TreeMap・10-13 Dock・10-9 Chat・10-1 Profile・10-4 Super |
| 第 2 弾 | 10-22 Chart → 10-3 Money → 10-11 Kit(10-21 の `SfSegmentedControl`・`SfAccordion`・`ColorPicker` もここ) |
| 第 3 弾 | 10-8 Mail・10-12 Stream・10-10 Timeline・10-14 Graph / Graph2・10-17 Wheel(10-21 の `AvatarView` もここ) |
| 最後 | アニメーションの強化の案から実施するものを選ぶ。無ければこの計画を閉じて `Task_Checklist.md` へ戻る |

## ⚖️決定事項

### 🖼️UI 1 / UI 2 と View の部品の画面

| 項目 | 決定 |
| --- | --- |
| Toolkit の画面 | 残す(ライブラリの部品の一覧) |
| 下から出るシート | 自作の `BottomSheetView` を優先する(SfBottomSheet ではなく) |
| 10-2 Login | 対象外 |
| Character の Detail | 全身の絵の下半身だけを切り抜いて並べるのは意図したもの。変えない |
| 一覧と詳細 | 一覧から開く画面(Stream → 詳細、Mail → 本文)は、Shop・News と同じく `[Scope]` の文脈クラスで選んだ項目を共有する。どれを押しても同じ詳細が開く形をなくす |
| 規則の置き場所 | 集計・ポイントのような画面の規則は文脈クラスかモデルに置き、VM は呼び出しとバインドだけにする。値段・日時は文字列ではなく値で持ち、書式は XAML で付ける |
| 配置 | 画面の下に大きな余白を残さない。余る画面は、その画面らしい情報(履歴・グラフ・一覧)で埋める |
| 操作のボタン | F キーのまま(画面の中にボタンを足さない) |
| 一覧の行 | 角丸のカードの行(Kit の通知)は、フラットな行と区切り線にし、中身を色付きのバッジと絵文字で示す |
| 見本のデータ | 日時は今日からの相対にする(固定の日付や、予定の無い曜日を作らない)。文面は実在しそうな内容にする(登場人物はそのまま) |
| 部品 | 部品の画面(View の Toolkit・Custom・Chart・Bottom Sheet・Drawer)でしか使っていない部品は、使いどころのある UI の画面で使う |
| 遷移先の無いボタン | 今のまま(押しても何も出さない)。画面の中で完結する操作(割引・チップの選択・お気に入りなど)だけを動かす |
| 画像 | 既存の画像(商品・ポスター・人物・写真)を使う。足りないところは絵文字とアイコン |

### ✨既存の画面の細部

| 項目 | 決定 |
| --- | --- |
| 言葉 | 英語と日本語の混在はそのまま(日本語にしたのは UI 2 の Habit・Home)。題・メニュー・F キー、機材・計器風の英語、方位、英語のバッジは英語のまま。曜日は端末の言語、確認のダイアログの Cancel は MauiComponents の既定のまま |
| スペース | 文章の中は、日本語と英語・数字の間と括弧の前に半角スペースを入れない。パラメータ的な所(部品の名前と括弧の補足・値と単位・ID・ファイル名)、「:」の後、記号と「/」の区切りの前後、角括弧の前、リストの点の絵文字の後は入れる。メニュー・倍率・日付と時刻(「10/06 (火)」)・フォントの見本は今のまま。英単語を複数並べるときは半角スペースで区切る。そろえるのは対象の画面の文字だけ |
| 補足の文 | 実装の説明や断り書きは書かない。「⇔」は使わず、状態の移り変わりは書かない。書くのは何をするかだけ |
| F キーだけの操作 | そのまま(画面の中にボタンを足さない) |
| メニュー | 大きさ(文字・すき間)と色は変えない。角丸は使わない |
| 進め方 | 番号ごとに、修正の前後を確認しながら進める。指示に無い点は、決める前に確認する |
| 見出しのアイコンの色 | 鮮やかな色(XxxDefault。琥珀は AmberDarken2)を画面ローカルのスタイルで付ける |
| 名前と値の行 | 名前 14 の灰・値 16 の濃い色・行の間に 1px の線。値は左寄せ(2:3 の 2 列)。ID・パス・日時は等幅の 14(共有の `CardNameValueGrid` / `CardNameLabel` / `CardValueLabel` / `CardMonoValueLabel`) |
| 状態の見せ方 | 淡い地のチップ(`StatusChip` の `Tone`)。緑 = 正常・接続済み、琥珀 = 途中・注意、赤 = エラー・切断、灰 = 未使用・停止(まだつないでいない・止めた)。表の値のチップはアイコンなし、カードの先頭のチップはアイコン付き。絵文字は使わない |
| エラーの文 | 淡い赤の帯にアイコン(共有の `CardErrorBorder` / `CardErrorIconLabel` / `CardErrorLabel`) |
| 空・読み込み中・失敗 | 一覧は `EmptyView` に `BasicEmptyStack`、読み込み中は `BasicLoadingIndicator`。通信できないときは空の表示を出さない |
| ボタン | 主な操作は塗りの `BasicFilledButton`、ほかはアイコン付きの `BasicIconOutlinedButton`、消す操作は赤の枠の `BasicOutlinedCancelButton`。押す部品の高さは 44 以上 |
| 配置 | 中央のカード 1 枚の画面(QR Display・Bluetooth・BLE Host・Audio・View State)は、主役を空きの真ん中に、操作を下にそろえる。下が大きく空く画面は、その画面の情報で埋める |
| カメラの画面 | Camera・QR Scan は、OCR と同じく黒地にし、プレビューに重ねた丸いボタンと状態のチップでそろえる |
| 一覧の行 | 角丸のカードの行(View Drag & Drop・Device NFC の履歴)は、フラットな行と区切り線にする |
| 決まりに合わない値 | 許可値以外の文字の大きさ(13・15・56)と押す部品の高さ(32・36・40)は、その画面を直すときに許可値へ寄せる。要素に直接書いた見た目の属性(見本の内容のものを除く)も画面ローカルのスタイルへ移す |

## 🙅見直さない画面

| 画面 | 理由 |
| --- | --- |
| UI 1 Grid / Visit | 業務アプリの一覧の見本として作り込み済み |
| UI 1 Weather / News | App のミニアプリと同じ水準で作成済み |
| UI 2 Gauge / Meter / Mixer / Radar / Monster | 部品の見本として揃っている |
| Basic Behavior | 欄・枠・注記がほかの画面とそろっている |
| Navigation Stack | 段ごとの色・番号・進み具合が中央のカードにまとまっている |
| Navigation Initialize / Cancel | 初期化中のスケルトンと完了のカード、確認のカードで意図が伝わる |
| Device OCR | 黒地・四隅のかぎと格子・角丸の結果のパネルで整っている |
| Navigation のメニューの項目の絵文字 | メニューの間の意図的な差異(`Change_Summary.md` の付録B) |
| View Svg | 大きな枠・ファイル名・選んだものを塗るチップがそろっている |
| Sample Web App | 中は Vite + React の雛形のページで、読み込み中の覆いもそろっている |
| 各メニュー(Device / Sample ほか) | 大きさと色は変えない(決定事項) |

## 🖼️UI 1

### 💰10-3 Money(🟡📦📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UIMoneyView.xaml` + `UIMoneyViewModel.cs` | 決済アプリのホーム(残高・ランク・機能のアイコン・利用可能額・下のタブ) |

今: 下のタブは色が変わるだけで中身は変わらない。利用可能額の下が大きく空く。中央の「支払い」は何もしない。

- ~~① 空きに最近の取引(日付の区切り、入金は緑・支払いは赤、店の種類の絵文字、金額)~~
- ~~② 下のタブで中身を切り替える(ホーム以外は、取引の検索・お知らせの一覧・アカウントの簡単な画面)~~
- ~~③ 「支払い」で、バーコードと QR を出すシート(自作の `BottomSheetView`。QR は Device > QR Display と同じ `QrImageSourceConverter`。残り時間で作り直す)~~
- ~~④ 「詳細」で残高の推移の折れ線(10-22 のグラフ)~~
- ~~⑤ 残高のカードに前月比のバッジ(↑ +2.4% は緑、↓ は赤)~~
- ~~⑥ 月の支出のカード: 月の切り替え(▼ で月の一覧のシート)、合計と週の平均、週ごとの棒(10-22 ③ の形。タップで選んだ週を強調)~~
- ~~⑦ 支出の分類: 丸い色のアイコン・金額・割合を横に並べ、「›」ですべてを出す~~

結果(👀 確認待ち): ① ホームの下に最近の取引 5 件(今日・昨日・「10/06 (月)」の区切り、店の種類の絵文字、入金は緑で +、支払いは赤で −)。② 下のタブで中身を切り替える(検索 = 店の名前で探す、お知らせ = 8 件・タップで既読、アカウント = 名前・ランクの進み具合・本人確認の未完了など)。③ 支払いで下からシート(Code 128 のバーコード・番号・QR、30 秒で作り直す残り時間の棒、残高)。④ 詳細で残高の推移のシート(最近 30 日の折れ線、タップで日付と残高)。⑤ 残高の横に前月比のバッジ(1 日あたりの平均残高。↑ 緑・↓ 赤)。⑥ 月の支出のカード(▼ で 6 か月の一覧のシート、合計と週の平均、週ごとの棒・タップで強調。棒は見本の画像と同じ青緑から青のグラデーション)。⑦ 支出の分類(丸い色のアイコン・金額・割合を横に並べ、› で分類の一覧のシート)。取引の見本は半年前から今日までを曜日と日付で作り、残高が 8,000 円を下回るとオートチャージする。開くまで約 0.6 秒・検索のタブは約 0.15 秒(検索は見えている行だけを作り、お知らせの一覧・アカウントのタブ・4 つのシートは初めて開いたときに作る)

見本の画像(⑤〜⑦): `__Image/Mockup/Finance.jpg`。

使える部品(⑤〜⑦): 前月比のバッジは無い(形は `Controls/StatusChip.xaml` と Cart のバッジ、アイコンは `MaterialIcons.Trending_up`)。月の切り替え(▼)も無い。合計と平均は Schedule の `SummaryBorder`、分類の横の一覧は Super の丸い色のアイコンと Stream の「›」の見出しと横の一覧。

### 🏬10-4 Super(🟢📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UISuperView.xaml` + `UISuperViewModel.cs` | スーパーアプリのホーム(検索・ポイント・自動で送るバナー・サービス・クーポン) |

今: バナーの自動の送り以外の操作が無い。

- ~~① クーポンの「獲得」の切り替え(獲得済みの印と、押したときの弾む動き)~~
- ~~② サービスのアイコンの押したときの反応~~
- ~~③ クーポンの下に「近くのお店」の横の一覧(距離・混み具合のバッジ)~~

結果(👀 確認待ち): ① クーポンの左下に「獲得する」。押すと「✓ 獲得済み」(半透明の白)になって弾み、もう一度押すと戻る。② サービスのアイコンは押すと縮む(何も開かない)。③ 「近くのお店」の横の一覧 6 店(色の丸いアイコン・店名・種類、📍 距離、混み具合のバッジ 空いています 緑・やや混雑 橙・混雑 赤)。カードは白地に灰の枠・角丸 12(クーポンに合わせた)、バッジの角は 2。クーポンとお店の横の一覧は止まる位置をそろえない(最後の項目まで送れる)

### 💬10-9 Chat(🟢📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UIChatView.xaml` + `UIChatViewModel.cs` | チャット(スタンプ・リアクション・既読・入力欄) |

今: 入力欄の左のカメラのボタンは何もしない(コマンドが空)。

- ~~① カメラのボタンで写真を選び、画像のメッセージとして送る(`MediaPicker`)~~
- ~~② 相手のメッセージのタップでリアクションを付ける(よく使う絵文字の帯)~~

結果(👀 確認待ち): ① カメラのボタンで端末の写真の選択を開き、選んだ写真を画像のメッセージ(200 × 150・角丸 12)で送る(実機は選択の画面が開くところまで)。② 相手のメッセージをタップすると、その行を琥珀の地にして、メッセージの下(リアクションの下)に絵文字の帯(👍 ❤️ 😂 😮 😢 🙏。白地に灰の枠)を出す。絵文字を押すとメッセージの下に加わり(同じ絵文字は数が増える)、帯を閉じる。同じメッセージをもう一度押す・戻るキー・スタンプの帯・送信でも閉じる。自分のメッセージには付けない(長押しと入力欄の下の帯はやめた)

### 🧩10-11 Kit(🟡📦📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UIKitNotifyView.xaml` + `UIKitNotifyViewModel.cs` | 通知の一覧 |

- ~~② 通知のスワイプ: 端まで引くとそのまま実行し、引いたところは色とアイコンだけ~~

結果(👀 確認待ち): ② 右へ引くと緑の地に ✓、左へ引くと赤の地にごみ箱のアイコン(文字は出さない)。端まで引いて離すとそのまま既読・削除

## 🧰部品の画面(View)

### 🔍部品の画面と UI / App での使用

| 画面 | 現在のファイル名 | 見せている機能 | UI / App での使用 | 判定 |
| --- | --- | --- | --- | --- |
| Toolkit | `Modules/View/ViewToolkitView.xaml` + `ViewToolkitViewModel.cs` | `SfTabView`・`SfOtpInput`・`SfSegmentedControl`・`SfChipGroup`・`AvatarView`・`RatingView`・`Expander`・`SfAccordion` | `SfChipGroup` = UI Shop・Timeline、`RatingView` = UI Shop の商品、`SfAccordion` = UI Shop の商品・Kit の設定、`SfSegmentedControl` = UI Kit のダッシュボード。ほかは使われていない | 残す(ライブラリの部品の一覧) |
| Custom | `Modules/View/ViewCustomView.xaml` + `ViewCustomViewModel.cs` | 自作の `MarqueeLabel`・`TreeView`・`ColorPicker`・`DurationPicker`・`AvatarGroup` | `MarqueeLabel` = UI News、`AvatarGroup` = UI Stream の詳細、`ColorPicker` = UI Kit の設定。ほかは使われていない | 残す(自作の部品の一覧) |
| Chart | `Modules/View/ViewChartView.xaml` + `ViewChartViewModel.cs` | 自作の描画のグラフ 7 種(`Graphics/Drawing/ChartDrawing`) | UI Money(残高の推移・週の支出)と UI Kit(歩数・心拍) | 残す(グラフの種類の一覧) |
| Sf Chart | `Modules/View/ViewSfChartView.xaml` + `ViewSfChartViewModel.cs` | Syncfusion のグラフ(縦棒・ドーナツ・極座標・ファネル / ピラミッド・スパーク・サンバースト) | 使われていない | 残す(ライブラリのグラフの一覧) |
| Bottom Sheet | `Modules/View/ViewBottomSheetView.xaml` + `ViewBottomSheetViewModel.cs` | `SfBottomSheet` と自作(`Controls/BottomSheetView`)の比較 | 自作は UI Shop(絞り込み)・Schedule(予定の詳細)・Money(支払いなど)・Graph(コミットの詳細)で使う | 残す(比較) |
| Drawer | `Modules/View/ViewDrawerView.xaml` + `ViewDrawerViewModel.cs` | `SfNavigationDrawer` と自作(`Controls/SideDrawer`)の比較 | 自作は UI Mail(フォルダー)で使う | 残す(比較) |

判定の考え方: 標準のコントロールの単体の見本で、機能が UI / App の画面ですでに使われているものは削除する。複数の部品を並べた一覧と、ライブラリと自作の比較は、UI で使った後も残す。

## 💫アニメーションの強化の案

既存の画面をアニメーションで強化する案(番号なし)。実施するものは 10-x / 11-x の項目(📝)にしてから進める。参照: https://www.telerik.com/blogs/5-heart-animations-using-net-maui(ハートの 5 種の動き。`ScaleTo` / `FadeTo` / `RotateTo` / `TranslateTo` と `GraphicsView` で作る)。

### 🧭今ある動き

| 部品 | 動き | 現在のファイル名 |
| --- | --- | --- |
| `AnimationOption.Pulse` | 拡大と縮小(1.0 → 1.05、3 秒)を繰り返す | `Behaviors/AnimationOption.cs` |
| `AnimationOption.BounceTrigger` / `BounceValue` | 値が変わったときに 1 回弾む(1.0 → 1.15、300 ms)。`BounceValue` を指定すると、その値になったときだけ弾む | `Behaviors/AnimationOption.cs` |
| `AnimationOption.Wave` / `WaveDelay` | 上下の揺れ(5、700 ms)を繰り返す。開始を遅らせて並べると波になる | `Behaviors/AnimationOption.cs` |
| `AnimationOption.FadeInTrigger` / `FlashTrigger` / `HighlightTrigger` | 1 回の表示・光る・背景色 | `Behaviors/AnimationOption.cs` |
| `AnimationOption.ProgressTo` / `HoldCommand` | 進捗のバーの伸び / 長押しの進み | `Behaviors/AnimationOption.cs` |
| `AnimationOption.EnterAnimation` | 表示したときの FadeUp / Pop | `Behaviors/AnimationOption.cs` |
| `LabelOption.CountUpValue` | 数の数え上げ | `Behaviors/LabelOption.cs` |
| Smart.Maui の XAML のアニメーション | Fade / Rotate / Scale / Translate を組み合わせた 1 回の動き | `Modules/View/ViewAnimationView.xaml` |
| `AnimationBase` の派生 | 1 回の動きのクラス(`SpringAnimation` / `EasingDemoAnimation`) | `Animations/` |
| `DrawingObject.AnimateValue` | 描画の値の補間(16 ms ごとに描き直す) | `Graphics/Drawing/DrawingObject.cs` |
| `PulseRingDrawing` | 3 つの輪が広がって消える波紋。開始と停止は ViewModel から呼び、色は固定 | `Graphics/Drawing/PulseRingDrawing.cs` |
| `ChartDrawing` | 折れ線の左からの描き込み、棒の伸び、ドーナツの回り込み(1 回) | `Graphics/Drawing/ChartDrawing.cs` |
| `SceneObject` | 60 fps の描画のループ。点滅(`Blink`)と光る線(`DrawGlowLine`) | `Graphics/Scene/SceneObject.cs` |

足りない動き: 回り続ける、透明度の点滅、着地でつぶれるジャンプ、心拍の 2 段の鼓動、バインドで動かす要素の後ろの波紋、値が流れる線。

`Pulse` / `Bounce` / `Pop` と `ButtonOption.PressEffect` はどれも Scale を、`Wave` と `FadeUp` は TranslationY を動かす。同じ要素に重ねると打ち消し合う。

### 💡足す動きの案

| 案 | 動き | 形 | 使い道 |
| --- | --- | --- | --- |
| 鼓動 | 1.0 → 1.18 → 1.0 → 1.1 → 1.0 の 2 段の拡大の後に休む。間隔は毎分の回数から決める(72 回で 833 ms) | `AnimationOption.Heartbeat`(bool)+ `HeartbeatRate` | 心拍の表示、残り時間が少ないときの警告(回数を上げて速くする) |
| 点滅 | 透明度 1 → 0.3 → 1 を繰り返す | `AnimationOption.Blink`(bool) | LIVE・録音中・再接続の待ち・一時停止中の表示 |
| 回転 | 回り続ける(1 周 1.2 秒、Linear)。止めると 0 度に戻す | `AnimationOption.Spin`(bool) | 接続中・同期中・検索中のアイコン、読み込み中 |
| ジャンプ | 上へ跳ねて落ち、着地で縦につぶれて戻る(TranslationY と ScaleY) | `AnimationOption.JumpTrigger` + `JumpValue` | お気に入りに入れたとき、獲得・完了のとき、空の表示の絵文字 |
| 波紋 | 要素の後ろで輪が広がって消える | `PulseRingDrawing` を、色と動かすかどうかをバインドできるビューにする(表示中だけ動く) | 待ち受け(マイク・検索・NFC・位置)、広告中、接続済み、今の位置の点 |
| 流れる線 | 受けた値の線を右から左へ滑らかに流し、先頭に点を置いて後ろを薄くする | `DrawingObject` の新しい描画(`AnimateValue` のループか、`ChartDrawing` と同じ 60 fps のタイマー) | リアルタイムのグラフ、騒音の履歴、心拍のカード |
| 描き込み | 線・経路・弧を始点から伸ばして描く | `ChartDrawing` の描き込みと同じ切り抜きを、ほかの描画にも使う | 配送の段階の線、地図の経路、日の出と日の入りの弧 |

- 同じ要素に Scale の動きを 2 つ付けない。鼓動とジャンプは中のアイコンに、押したときの縮み(`PressEffect`)は外側のボタンに付けるように、要素を分ける
- 続く動きは表示中だけ動かす(`Pulse` と同じく、表示で始めて非表示で止める)

### 🗺️画面ごとの案

| 現在のファイル名 | 何用か | 案 |
| --- | --- | --- |
| `Modules/UI/UIKitDashView.xaml` | ダッシュボード(平均心拍数のカード・心拍数のタイル・通知のベル) | 平均心拍数のカードに、ハートの鼓動(毎分 72 回)と流れる線(10-11 ① の心拍の折れ線と合わせる)。未読の点に波紋か点滅 |
| `MainPage.xaml` | ヘッダーのプッシュの接続の状態(↔️ / 🔄 / ⛔) | 接続中は回転、再接続の待ちは点滅、接続済みは波紋 |
| `Modules/Network/NetworkRealtimeView.xaml` + `Controls/StatControl.cs` | SignalR の接続の状態と値のグラフ | 接続中は同期のアイコンを回転。1 秒ごとに跳ぶグラフを流れる線に |
| `Modules/UI/UIStreamView.xaml` | 動画配信のホーム(LIVE / NEW のバッジ) | LIVE のバッジを赤い点の点滅に |
| `Modules/UI/UIProfileView.xaml` / `UIStreamDetailView.xaml` / `UIShopView.xaml` / `UIItemView.xaml` / `UICharacterView.xaml` / `UIMonsterView.xaml` | いいね・お気に入り・ハートのボタン | 入れたときだけ鼓動かジャンプにする(今は外したときも弾む) |
| `Modules/Device/DeviceBleHostView.xaml` | BLE の広告 | 広告中の `Pulse` を波紋に |
| `Controls/ChatView.xaml` / `Modules/Device/DeviceMiscView.xaml` / `DeviceBleScanView.xaml` / `DeviceNfcView.xaml` / `DeviceLocationView.xaml` | 聞き取り中のマイク、検索中・待ち受けの空の表示 | `Pulse` を波紋に |
| `Modules/Device/DeviceWiFiView.xaml` / `DeviceBluetoothView.xaml` / `Modules/Network/NetworkStorageView.xaml` / `NetworkSftpView.xaml` | 検索・印刷・同期 | 処理中はアイコンを回転 |
| `Modules/UI/UILoadView.xaml` + `Graphics/Drawing/LoadDrawing.cs` | 騒音計の履歴 | 履歴を流れる線に。録音中の点の点滅、最大を更新したときの点滅 |
| `Modules/Device/DeviceActivityView.xaml` + `Graphics/Drawing/ActivityDrawing.cs` | 歩数の輪 | 歩くたびに 👟 がジャンプ、輪が値まで伸びる(11-31 と合わせる) |
| `Modules/UI/UIKitTrackingView.xaml` | 配送の追跡 | 今の段階の点に波紋、段階の線の描き込み(10-11 ④)、トラックの上下の揺れ |
| `Controls/DayTimetableView.cs`(`Modules/UI/UIScheduleView.xaml`) | 1 日の予定と今の時刻の線 | 今の時刻の点に波紋 |
| `Modules/App/AppTimerView.xaml` | タイマー | 残り 10 秒から鼓動を速める、一時停止中は数字の点滅、終わったらベルのジャンプ |
| `Modules/UI/UIWeatherView.xaml` | 天気(時間ごとの線・日の出と日の入りの弧) | 今の時刻の点に波紋、弧と線の描き込み |
| `Modules/Sample/SampleMap2View.xaml` + `Messaging/MapsuiController.cs` | 地図の経路 | 経路を出したときの描き込み |
| `Modules/View/ViewEffectView.xaml` | 効果の見本 | 足した動き(鼓動・点滅・回転・ジャンプ・波紋)の見本を並べる |
| `Modules/UI/UICartView.xaml` / `UIShopView.xaml` / `UIMailView.xaml` / `UINewsView.xaml` / `Modules/App/AppTodoView.xaml` ほか | 一覧が空のときの表示 | 絵をゆっくり上下に揺らす。完了の 🎉 はジャンプ |

## 🙈見ていない画面

| 画面 | 理由 |
| --- | --- |
| Sample CV Net / Chat | AI の接続先を設定していない端末では画面に入れない(設定した端末で見る) |

## 🔧見た目以外

| 画面 | 内容 |
| --- | --- |
| Sample Web App | 読み込み中の覆いを消す処理(決まった時間の後に消す)が code-behind にある |

## ⚠️制約

- 新しい画像は作らない(既存の画像と、絵文字・アイコン)
- 既存の画面の細部(11-x)は、表示する項目を変えない(並び・形・色・強弱・余白・状態の見せ方・操作の位置を変える)

## 🚫対象外

| 項目 | 内容 |
| --- | --- |
| 10-2 Login | 入力の検証、ログイン中・失敗・成功の表示、2 段階認証のコードの入力、指紋でのログイン |
| 10-7 ③ | Calendar の日のタップで、その日の Schedule を開く |
| 10-16 TreeMap | 撮影の後の色の丸と割合の一覧(ツリーマップの区画に色と割合が出ている) |
| 10-4 ② | Super のポイントの数え上げ(開いたときから値を出す) |
