# 🎨UI のブラッシュアップの計画

UI のブラッシュアップの残作業はこの計画で管理する(`Task_Checklist.md` には載せない)。ここが終わってから `Task_Checklist.md` の項目に戻る。済んだ項目はこの計画から消し、内容は `Change_Summary.md` に書く。

| 部分 | 番号 | 内容 |
| --- | --- | --- |
| 🖼️UI 1 / UI 2 と View の部品の画面 | 10-x | UI 1 / UI 2 の画面を、App のミニアプリと同じ水準(画面の中で完結する操作、下に余白を残さない配置、色と描画で華やかに、一覧で選んだ項目が詳細に出る)へ寄せる。View の部品の画面(Toolkit / Custom / Chart / Sf Chart / Bottom Sheet / Drawer)でしか使っていない部品は、使いどころのある UI の画面へ取り込む |
| 💫アニメーションの強化 | 10-35 | 動きの部品を足し、画面ごとの案の画面に当てる |
| 🔤英語への統一 | 10-36 | UI と App 以外の画面の日本語の文字を英語にする |

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
| 10-35 | アニメーションの強化 | 🟡 | 📦 | ②👀 ③📝 | ~~② Character の星を弾む動きに、Kit・Schedule の動きを元に戻す~~、③ 案のまま残すもの |
| 10-36 | 英語への統一(UI と App 以外) | 🟢 | 📦 | 👀 | ~~NFC の待ち受けの文を消す~~ |
| 10-37 | View のメニューの並び | 🟢 | 📦 | 👀 | ~~Bottom Sheet・Drawer を Toolkit・Custom の前の段へ~~ |
| 10-38 | Minesweeper の結果の表示 | 🟢 | 📦 | 👀 | ~~クリア・ゲームオーバーのカードが出ても盤面を動かさない~~ |

### 🚀10-x の進め方

弾ごとにまとめて作り、実機で前後を撮って、まとめて確認を受ける。細部(色・大きさ)は既存の画面の形に合わせ、決めた細部は確認のときに示す。

| 弾 | 項目 |
| --- | --- |
| 第 5 弾 | 10-35 ②・10-36・10-37・10-38 の確認、10-35 ③ の案の判断 |
| 最後 | この計画を閉じて `Task_Checklist.md` へ戻る。`Biometric_Plan.md`・`Other_App_Candidates.md`・`Telemetry_Study.md` も、要らない部分を消して `Task_Checklist.md` にまとめる |

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
| Syncfusion の部品 | UI の画面では使わず、自前の部品にする。Sf の部品は Sf の見本の画面(View の Toolkit・Sf Chart・Bottom Sheet・Drawer)だけに残す |
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
| Sample Web App | 中は Vite + React の雛形のページ |
| 各メニュー(Device / Sample ほか) | 大きさと色は変えない(決定事項) |

## 🧰部品の画面(View)

### 🔍部品の画面と UI / App での使用

| 画面 | 現在のファイル名 | 見せている機能 | UI / App での使用 | 判定 |
| --- | --- | --- | --- | --- |
| Toolkit | `Modules/View/ViewToolkitView.xaml` + `ViewToolkitViewModel.cs` | `SfTabView`・`SfOtpInput`・`SfSegmentedControl`・`SfChipGroup`・`AvatarView`・`RatingView`・`Expander`・`SfAccordion` | `RatingView` = UI Shop の商品。ほかは使われていない(UI の画面のチップ・区切り・開閉は自前の `ChipGroup`・`SegmentedView`・`AccordionView`) | 残す(ライブラリの部品の一覧) |
| Custom | `Modules/View/ViewCustomView.xaml` + `ViewCustomViewModel.cs` | 自作の `MarqueeLabel`・`TreeView`・`ColorPicker`・`DurationPicker`・`AvatarGroup` | `MarqueeLabel` = UI News、`AvatarGroup` = UI Stream の詳細、`ColorPicker` = UI Kit の設定。ほかは使われていない | 残す(自作の部品の一覧) |
| Chart | `Modules/View/ViewChartView.xaml` + `ViewChartViewModel.cs` | 自作の描画のグラフ 7 種(`Graphics/Drawing/ChartDrawing`) | UI Money(残高の推移・週の支出)と UI Kit(歩数・心拍) | 残す(グラフの種類の一覧) |
| Sf Chart | `Modules/View/ViewSfChartView.xaml` + `ViewSfChartViewModel.cs` | Syncfusion のグラフ(縦棒・ドーナツ・極座標・ファネル / ピラミッド・スパーク・サンバースト) | 使われていない | 残す(ライブラリのグラフの一覧) |
| Bottom Sheet | `Modules/View/ViewBottomSheetView.xaml` + `ViewBottomSheetViewModel.cs` | `SfBottomSheet` と自作(`Controls/BottomSheetView`)の比較 | 自作は UI Shop(絞り込み)・Schedule(予定の詳細)・Money(支払いなど)・Graph(コミットの詳細)で使う | 残す(比較) |
| Drawer | `Modules/View/ViewDrawerView.xaml` + `ViewDrawerViewModel.cs` | `SfNavigationDrawer` と自作(`Controls/SideDrawer`)の比較 | 自作は UI Mail(フォルダー)で使う | 残す(比較) |

判定の考え方: 標準のコントロールの単体の見本で、機能が UI / App の画面ですでに使われているものは削除する。複数の部品を並べた一覧と、ライブラリと自作の比較は、UI で使った後も残す。

## 💫10-35 アニメーションの強化(🟡📦)

既存の画面に動きを足す。参照: https://www.telerik.com/blogs/5-heart-animations-using-net-maui(ハートの 5 種の動き。`ScaleTo` / `FadeTo` / `RotateTo` / `TranslateTo` と `GraphicsView` で作る)。

- ~~② UI > Character の星をジャンプから弾む動き(`BounceTrigger`。入れたときだけ)にする。UI > Kit・Schedule に当てた動きは元に戻す~~
- ③ 案のまま残すもの(下)

結果(👀 確認待ち): ② Character の星は入れたときに 1.15 倍に弾んで戻る(外したときは動かない)。Kit(心拍数のタイルのハート・未読の点・配送の追跡)と Schedule(今の時刻の点)は元の動きに戻した

③ 案のまま残すもの(📝):

- UI > Weather: 時間ごとの線の描き込み
- Sample > Map2: 経路を出したときの描き込み
- Device > Bluetooth: 印刷中のアイコンの動き(円の `Pulse` のまま)

## 🔤10-36 英語への統一(UI と App 以外)(🟢📦)

- ~~② Device > NFC の待ち受けの「Hold Suica near the device」を消す~~

結果(👀 確認待ち): ② 待ち受けは波紋とアイコンだけ(文とスタイル `NfcTitleLabel` を消した)

## 🧭10-37 View のメニューの並び(🟢📦)

- ~~① Bottom Sheet・Drawer を Toolkit・Custom の前の段へ移す~~

結果(👀 確認待ち): ① Layout・State / Border・Shadow / Animation・Easing / Effect・Drag & Drop / Graphics・Drawing / Lottie・SVG / Bottom Sheet・Drawer / Toolkit・Custom / Chart・Sf Chart。ViewId も同じ順に

## 🧩10-38 Minesweeper の結果の表示(🟢📦)

- ~~① クリア・ゲームオーバーのカードが出ても、盤面の位置と大きさを変えない~~

結果(👀 確認待ち): ① 操作の切り替えと結果のカードを同じ下の領域(高さ 120。クリアでベスト更新の 4 行が入る高さ)に置き、どちらも上下の中央。プレイ中・ゲームオーバー・クリアで盤面の位置と大きさが同じ

## ⚠️制約

- 新しい画像は作らない(既存の画像と、絵文字・アイコン)
- UI > Kit・Schedule には動きを足さない
- 既存の画面の細部(11-x)は、表示する項目を変えない(並び・形・色・強弱・余白・状態の見せ方・操作の位置を変える)

## 🚫対象外

| 項目 | 内容 |
| --- | --- |
| 10-2 Login | 入力の検証、ログイン中・失敗・成功の表示、2 段階認証のコードの入力、指紋でのログイン |
| 10-7 ③ | Calendar の日のタップで、その日の Schedule を開く |
| 10-16 TreeMap | 撮影の後の色の丸と割合の一覧(ツリーマップの区画に色と割合が出ている) |
| 10-4 ② | Super のポイントの数え上げ(開いたときから値を出す) |
