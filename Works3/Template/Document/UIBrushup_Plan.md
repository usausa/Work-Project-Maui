# 🎨UI のブラッシュアップの計画

UI のブラッシュアップの残作業はこの計画で管理する(`Task_Checklist.md` には載せない)。ここが終わってから `Task_Checklist.md` の項目に戻る。済んだ項目はこの計画から消し、内容は `Change_Summary.md` に書く。

| 部分 | 番号 | 内容 |
| --- | --- | --- |
| 🖼️UI 1 / UI 2 と View の部品の画面 | 10-x | UI 1 / UI 2 の画面を、App のミニアプリと同じ水準(画面の中で完結する操作、下に余白を残さない配置、色と描画で華やかに、一覧で選んだ項目が詳細に出る)へ寄せる。View の部品の画面(Toolkit / Custom / Chart / Sf Chart / Bottom Sheet / Drawer)でしか使っていない部品は、使いどころのある UI の画面へ取り込む |
| ✨既存の画面の細部 | 11-x | Main / Basic / Navigation / Device / Network / View / Sample の画面を、表示する項目は変えずに、細部(色・形・強弱・余白・状態の見せ方)を App の画面と同じ水準へ寄せる。📝 各項目は案のままで、実施の前に内容を確認する |  |
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
| 10-0 | 共通 | ⚖️ | — |  | 共通の方針(一覧と詳細の受け渡し、規則の置き場所、余白、操作のボタン、行の形、見本のデータ) |
| 10-1 | UI 1 Profile | 🟢 | 📦📦 |  | 下の段のタブ、写真の全画面の表示、フォローで数が変わる |
| 10-3 | UI 1 Money | 🟡 | 📦📦📦 | ⑤〜⑦📝 | 最近の取引、下のタブで中身を切り替え、支払いのシート(バーコード・QR)、前月比のバッジ、月の支出の棒と分類 |
| 10-4 | UI 1 Super | 🟢 | 📦📦 |  | クーポンの獲得、ポイントのカウントアップ、近くのお店 |
| 10-8 | UI 1 Mail | 🟡 | 📦📦📦 |  | メールらしい見本、本文の画面、フォルダーのドロワー、日付の区切り |
| 10-9 | UI 1 Chat | 🟢 | 📦📦 |  | 写真の送信、長押しでリアクション |
| 10-10 | UI 1 Timeline | 🟡 | 📦📦 |  | 今日の日付と今の時刻からの進行、ブックマークとタグの絞り込み |
| 10-11 | UI 1 Kit | 🟡 | 📦📦📦 |  | ダッシュボードのグラフ、通知の行とスワイプ、設定の部品、追跡とはじめにの見た目 |
| 10-12 | UI 1 Stream | 🟡 | 📦📦📦 |  | 選んだ作品の詳細、のぞきと中央の強調のあるトップ、続きを見るの棚 |
| 10-13 | UI 1 Dock | 🟢 | 📦📦 |  | CPU・メモリの実際の値、フォルダーの中、タイマーとミュートの動き |
| 10-14 | UI 2 Graph / Graph2 | 🟡 | 📦📦 |  | Graph の行のタップでコミットの詳細のシート、Graph2 の開いた行に要約 |
| 10-15 | UI 2 Load | 🟢 | 📦 |  | 画面の中の Clear、履歴の目盛りと目安の線 |
| 10-16 | UI 2 TreeMap | 🟢 | 📦📦 |  | 画面の中の撮影と拡大・縮小、色の割合の一覧 |
| 10-17 | UI 2 Wheel | 🟡 | 📦📦 |  | 画面の中の Spin、背景、結果のカードと履歴、項目の編集 |
| 10-18 | UI 2 Character | 🟡 | 📦 |  | Detail の絵の切り抜きを直す |
| 10-21 | View Toolkit | — | — |  | 部品を UI の画面へ取り込む(Toolkit は残す) |
| 10-22 | View Chart | 🟡 | 📦📦📦 | ③📝 | 自作のグラフの軸・凡例・値の表示・動き、UI で使える形に、棒のグラデーションと溝 |
| 10-23 | View Custom | 🟢 | 📦 |  | TreeView の見本のデータ |

### ✨既存の画面の細部

📝 11-x の項目は案のまま(まだ確認していない)。番号ごとに、実施の前に内容を確認し、修正の前後を見て確認する。

| 番号 | 対象 | 優先 | 変更量 | 状態 | 内容 |
| --- | --- | --- | --- | --- | --- |
| 11-0 | 共通 | — | — | 📝 | 共通の方針(スペース、状態の見せ方、ボタン、空・読み込み中、名前と値の行、配置)。決めたことは決定事項の表 |

## ⚖️決定事項

### 🖼️UI 1 / UI 2 と View の部品の画面

| 項目 | 決定 |
| --- | --- |
| 10-21 Toolkit | 残す(ライブラリの部品の一覧) |
| 下から出るシート | 自作の `BottomSheetView` を優先する(SfBottomSheet ではなく) |
| 10-2 Login | 対象外 |

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

## 🧭共通の方針

### ⚖️10-0 UI 1 / UI 2 の共通の方針(案)

| 項目 | 案 |
| --- | --- |
| 一覧と詳細 | 一覧から開く画面(Shop → Item → Cart、Stream → 詳細、Mail → 本文)は、`[Scope]` の文脈クラスで選んだ項目を共有する(ToDo・News と同じ形)。どれを押しても同じ詳細が開く形をなくす |
| 規則の置き場所 | 合計・割引・カートの数のような画面の規則は文脈クラスかモデルに置き、VM は呼び出しとバインドだけにする。値段・日時は文字列ではなく値で持ち、書式は XAML で付ける |
| 配置 | 画面の下に大きな余白を残さない。余る画面は、その画面らしい情報(履歴・グラフ・一覧)で埋める |
| 操作のボタン | 画面の主な操作(Wheel の Spin・Load の Clear・TreeMap の撮影)は画面の中のボタンにする。業務アプリの形の Grid / Visit と、計器の画面の Telemetry は F キーのまま |
| 一覧の行 | 角丸のカードの行(Kit の通知・Cart・Timeline)は、フラットな行と区切り線にし、中身を色付きのバッジと絵文字で示す |
| 見本のデータ | 日時は今日からの相対にする(固定の日付や、予定の無い曜日を作らない)。文面は実在しそうな内容にする(登場人物はそのまま) |
| 部品 | 部品の画面(View の Toolkit・Custom・Chart・Bottom Sheet・Drawer)でしか使っていない部品は、使いどころのある UI の画面で使う(10-21) |
| 遷移先の無いボタン | 今のまま(押しても何も出さない)。画面の中で完結する操作(割引・チップの選択・お気に入りなど)だけを動かす |
| 画像 | 既存の画像(商品・ポスター・人物・写真)を使う。足りないところは絵文字とアイコン |

### 📝11-0 既存の画面の共通の方針(案)

画面ごとの候補で使う形。項目ごとに、前後を見て確認する。

| 項目 | 案 |
| --- | --- |
| スペース | 文章の中は日本語と英語・数字の間、括弧の前を詰め、パラメータ的な所は半角スペースを入れる |
| 状態の見せ方 | 「状態: …」の文字や、True / False・列挙の名前・x-1 のような生の値を出さない。`StatusChip` か、絵文字付きの短い言葉(WiFi・Biometric の `MapToTextConverter` と同じ形)にする。色は緑 = 正常・接続済み、琥珀 = 途中・注意、赤 = 停止・エラー、灰 = 未使用。VM は状態を列挙で持ち、文言と色は XAML で決める |
| エラーの文 | 赤の小さな文字ではなく、淡い赤の帯にアイコン付きで出す |
| 空・読み込み中・失敗 | 一覧は `EmptyView` に `BasicEmptyStack`、読み込み中は `BasicLoadingIndicator`。通信できないときは空の表示を出さない |
| ボタン | 主な操作は塗りの `BasicFilledButton`、ほかはアイコン付きの `BasicIconOutlinedButton`、消す操作は赤の枠の `BasicOutlinedCancelButton`。押す部品の高さは 44 以上 |
| 名前と値の行 | 名前は小さい灰色、値は濃い色にする(太字の名前の方が値より目立つ画面がある)。カードの中の名前と値の行に使う文字 16 のスタイルと 1px の区切り線を `Styles.xaml` の Card の区画に足し、Diagnostics・Converter・Locale・Info・Status・Location で使う(`NameLabel` / `ValueLabel` は変えない) |
| 見出しのアイコンの色 | `InfoCard` の見出しのアイコンは全部同じ灰青。カードの意味ごとに色を付ける(画面ローカルのスタイル) |
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

### 👤10-1 Profile(🟢📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UIProfileView.xaml` + `UIProfileViewModel.cs` | SNS のプロフィール(表紙・アイコン・数・フォロー / いいね / お気に入り・自己紹介・興味・写真) |

今: 写真は並べるだけで、押しても何も起きない。下の段は自己紹介・興味・写真の固定の並び。

- ① 興味の下をタブ(投稿 / 写真 / いいね)で切り替える(`Controls/TabStrip`)
- ② 写真のタップで全画面の表示(左右に送る)
- ③ フォローの切り替えで「フォロワー」の数が増減する(カウントアップ)

### 💰10-3 Money(🟡📦📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UIMoneyView.xaml` + `UIMoneyViewModel.cs` | 決済アプリのホーム(残高・ランク・機能のアイコン・利用可能額・下のタブ) |

今: 下のタブは色が変わるだけで中身は変わらない。利用可能額の下が大きく空く。中央の「支払い」は何もしない。

- ① 空きに最近の取引(日付の区切り、入金は緑・支払いは赤、店の種類の絵文字、金額)
- ② 下のタブで中身を切り替える(ホーム以外は、取引の検索・お知らせの一覧・アカウントの簡単な画面)
- ③ 「支払い」で、バーコードと QR を出すシート(自作の `BottomSheetView`。QR は Device > QR Display と同じ `QrImageSourceConverter`。残り時間で作り直す)
- ④ 「詳細」で残高の推移の折れ線(10-22 のグラフ)
- ⑤ 残高のカードに前月比のバッジ(↑ +2.4% は緑、↓ は赤)
- ⑥ 月の支出のカード: 月の切り替え(▼ で月の一覧のシート)、合計と週の平均、週ごとの棒(10-22 ③ の形。タップで選んだ週を強調)
- ⑦ 支出の分類: 丸い色のアイコン・金額・割合を横に並べ、「›」ですべてを出す

見本の画像(⑤〜⑦): `__Image/Mockup/Finance.jpg`。

使える部品(⑤〜⑦): 前月比のバッジは無い(形は `Controls/StatusChip.xaml` と Cart のバッジ、アイコンは `MaterialIcons.Trending_up`)。月の切り替え(▼)も無い。合計と平均は Schedule の `SummaryBorder`、分類の横の一覧は Super の丸い色のアイコンと Stream の「›」の見出しと横の一覧。

### 🏬10-4 Super(🟢📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UISuperView.xaml` + `UISuperViewModel.cs` | スーパーアプリのホーム(検索・ポイント・自動で送るバナー・サービス・クーポン) |

今: バナーの自動の送り以外の操作が無い。

- ① クーポンの「獲得」の切り替え(獲得済みの印と、押したときの弾む動き)
- ② ポイントのカウントアップと、サービスのアイコンの押したときの反応
- ③ クーポンの下に「近くのお店」の横の一覧(距離・混み具合のバッジ)

### ✉️10-8 Mail(🟡📦📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UIMailView.xaml` + `UIMailViewModel.cs` | メールの受信トレイ(優先 / その他・フィルター・スワイプで削除とアーカイブ・新規の丸いボタン・下のタブ) |

今: 件名・本文が仮の文字(「タイトルだよもん…」「ああああ…!!!!」)。行のタップは何もしない。左上のアイコン・フィルター・検索・通知・新規は動かない。

- ① 件名・本文をメールらしい内容にする(会議の案内・請求・配送の連絡・お知らせ。差出人はそのまま)。添付の印とスター
- ② 行のタップで本文の画面(新規 `UIMailDetailView`。既読にする、返信・削除のボタン。一覧と文脈で共有)
- ③ 左上のアイコンで自作のドロワー(`SideDrawer`): 受信トレイ・スター付き・送信済み・下書き・ゴミ箱と未読の数
- ④ 日付の区切り(今日・昨日・今週・それ以前)
- ⑤ 絵の無い差出人は頭文字の丸いアイコン(`AvatarView`)

### 💬10-9 Chat(🟢📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UIChatView.xaml` + `UIChatViewModel.cs` | チャット(スタンプ・リアクション・既読・入力欄) |

今: 入力欄の左のカメラのボタンは何もしない(コマンドが空)。

- ① カメラのボタンで写真を選び、画像のメッセージとして送る(`MediaPicker`)
- ② メッセージの長押しでリアクションを付ける(よく使う絵文字の帯)

### 🗓️10-10 Timeline(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UITimelineView.xaml` + `UITimelineViewModel.cs` | 1 日のセッションの流れ(時刻・点・カード・タグ) |

今: 日付が固定(2025/06/25)で、終わった・進行中の印も固定。行は角丸のカード。

- ① 日付を今日にし、セッションの時刻を今の前後に置いて、終わった・進行中を今の時刻から決める(1 分ごとに更新、今の時刻の線)
- ② セッションのブックマーク(☆)と、タグのチップでの絞り込み

### 🧩10-11 Kit(🟡📦📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UIKitDashView.xaml` + `UIKitDashViewModel.cs` | ダッシュボード(心拍・歩数・カロリー・睡眠の数、注文・はじめにの入口、通知・設定のボタン) |
| `Modules/UI/UIKitNotifyView.xaml` + `UIKitNotifyViewModel.cs` | 通知の一覧(タップで既読) |
| `Modules/UI/UIKitSettingView.xaml` + `UIKitSettingViewModel.cs` | 設定(アカウント・環境設定のスイッチ) |
| `Modules/UI/UIKitTrackingView.xaml` + `UIKitTrackingViewModel.cs` | 配送の追跡(段階の線) |
| `Modules/UI/UIKitOnboardView.xaml` + `UIKitOnboardViewModel.cs` | はじめに(3 ページ) |

今: ダッシュボードは数のタイルだけ。どの画面も下が大きく空く。通知は角丸のカードの行。はじめにの絵の背景の四角が画面から浮いて見える。

- ① ダッシュボード: 歩数の輪(目標に対する割合)、週の歩数の棒と心拍の折れ線(10-22 のグラフ)、日 / 週 / 月の切り替え(`SfSegmentedControl`)、睡眠の段の帯
- ② 通知: フラットな行、スワイプで既読・削除、「すべて既読」、今日・昨日の区切り
- ③ 設定: 文字の大きさ(スライダー)、テーマの色(`ColorPicker`)、よくある質問(`SfAccordion`)、ログアウト(赤)、バージョン
- ④ 追跡: 配達員のカード、到着までの残り時間、段階の線が順に伸びる動き
- ⑤ はじめに: ページごとに背景の色を変えて絵となじませる、最後のページだけ「始める」

### 🎬10-12 Stream(🟡📦📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UIStreamView.xaml` + `UIStreamViewModel.cs` | 動画配信のホーム(大きな作品・棚) |
| `Modules/UI/UIStreamDetailView.xaml` + `UIStreamDetailViewModel.cs` | 作品の詳細(予告・タブ・友だちのアイコン・関連) |

今: どのポスターを押しても同じ詳細が開く。

- ① 文脈クラス `UIStreamContext`(`[Scope]`)で選んだ作品を詳細に出す(作品ごとの題・年・時間・説明・評価・関連)
- ② 上の大きな作品を、のぞき(左右の作品が少し見える)と中央の強調のある `CarouselView` にし、自動で送る(10-6 の Shop の人気の商品と同じ作り)
- ③ 「続きを見る」の棚(進み具合のバー)

### 🎛️10-13 Dock(🟢📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UIDockView.xaml` + `UIDockViewModel.cs` | ボタンを並べた操作盤(色・フォルダー・音・音量・タイマー・ロック・設定・終了・CPU・メモリ) |

今: CPU・メモリは乱数。終了以外のボタンは名前のダイアログを出すだけ。

- ① CPU・メモリをアプリの実際の値にする(診断のパネル `Shell/DiagnosticSampler` と同じ取り方)
- ② フォルダーで中のボタンの面に切り替える(戻るのボタン)
- ③ タイマーのボタンで残り時間を減らし、ミュートの切り替えでアイコンを変える

## 🖼️UI 2

### 🌳10-14 Graph / Graph2(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UIGraphView.xaml` + `UIGraphViewModel.cs` | Git のコミットのグラフ(線・ブランチ・タグ) |
| `Modules/UI/UIGraph2View.xaml` + `UIGraph2ViewModel.cs` | 同じデータを曲線で |

今: Graph は行のタップで何もしない。Graph2 は行のタップで作者と日時を行の下に開く(要約は出さない)。

- ① Graph の行のタップでコミットの詳細のシート(自作の `BottomSheetView`。作者・日時・要約・ブランチとタグ)。データ(`repository.json`)に作者と要約がある
- ② Graph2 の開いた行に要約を足す

### 🔊10-15 Load(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UILoadView.xaml` + `UILoadViewModel.cs` | 騒音計(マイクの音量の針・最小・平均・最大・履歴の棒) |

今: Clear は F4。履歴の棒に目盛りが無い。

- ① Clear を画面の中のボタンにする
- ② 履歴の棒に dB の目盛りと、静か・普通・うるさいの目安の線

### 🎨10-16 TreeMap(🟢📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UITreeMapView.xaml` + `UITreeMapViewModel.cs` | カメラで撮った絵の色の割合のツリーマップ |

今: 縮小・拡大・撮影・やり直しは F2〜F4。

- ① 画面の中のシャッターのボタンと拡大・縮小
- ② 撮影の後に色の見本と割合の一覧

### 🎡10-17 Wheel(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UIWheelView.xaml` + `UIWheelViewModel.cs` | ルーレット(項目・止まったときの演出) |

今: Spin は F4。白い背景で下が空き、結果は文字だけ。

- ① 画面の中の大きな Spin のボタン
- ② 背景のグラデーションと、結果のカード(当たりは今の紙吹雪)と履歴(直近 5 回)
- ③ 項目の追加・削除(チップ)

### 🧝10-18 Character(🟡📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UICharacterView.xaml` + `UICharacterViewModel.cs` | キャラクターの選択(顔・クラス・詳細の絵) |

今: Detail の絵は、全身の絵の中央(腰から下)を切り抜いて並べている。

- ① Detail を、選んだキャラクターの全身の絵(またはバストアップ)と名前・クラス・お気に入りにする

## 🧰部品の画面(View)

### 🔍部品の画面と UI / App での使用

| 画面 | 現在のファイル名 | 見せている機能 | UI / App での使用 | 判定 |
| --- | --- | --- | --- | --- |
| Toolkit | `Modules/View/ViewToolkitView.xaml` + `ViewToolkitViewModel.cs` | `SfTabView`・`SfOtpInput`・`SfSegmentedControl`・`SfChipGroup`・`AvatarView`・`RatingView`・`Expander`・`SfAccordion` | `SfChipGroup` = UI Shop、`RatingView` / `SfAccordion` = UI Shop の商品。ほかは使われていない | 残す(ライブラリの部品の一覧)。10-21 |
| Custom | `Modules/View/ViewCustomView.xaml` + `ViewCustomViewModel.cs` | 自作の `MarqueeLabel`・`TreeView`・`ColorPicker`・`DurationPicker`・`AvatarGroup` | `MarqueeLabel` = UI News、`AvatarGroup` = UI Stream の詳細。ほかは使われていない | 残す(自作の部品の一覧)。10-23 |
| Chart | `Modules/View/ViewChartView.xaml` + `ViewChartViewModel.cs` | 自作の描画のグラフ 7 種(`Graphics/Drawing/ChartDrawing`) | 使われていない | 残す(グラフの種類の一覧)。10-22 |
| Sf Chart | `Modules/View/ViewSfChartView.xaml` + `ViewSfChartViewModel.cs` | Syncfusion のグラフ(縦棒・ドーナツ・極座標・ファネル / ピラミッド・スパーク・サンバースト) | 使われていない | 残す(ライブラリのグラフの一覧) |
| Bottom Sheet | `Modules/View/ViewBottomSheetView.xaml` + `ViewBottomSheetViewModel.cs` | `SfBottomSheet` と自作(`Controls/BottomSheetView`)の比較 | 自作は UI Shop(絞り込み)と UI Schedule(予定の詳細)で使う | 残す(比較)。10-3 / 10-14 でも使う |
| Drawer | `Modules/View/ViewDrawerView.xaml` + `ViewDrawerViewModel.cs` | `SfNavigationDrawer` と自作(`Controls/SideDrawer`)の比較 | 使われていない | 残す(比較)。自作は 10-8 で使う |

判定の考え方: 標準のコントロールの単体の見本で、機能が UI / App の画面ですでに使われているものは削除する。複数の部品を並べた一覧と、ライブラリと自作の比較は、UI で使った後も残す。

### 🧰10-21 Toolkit の部品の取り込み

| 部品 | 取り込み先 |
| --- | --- |
| `SfOtpInput` | なし(10-2 は対象外) |
| `SfAccordion` / `Expander` | 10-11 設定のよくある質問 |
| `SfSegmentedControl` | 10-11 ダッシュボードの日 / 週 / 月 |
| `AvatarView` | 10-8 Mail の絵の無い差出人 |
| `SfTabView` | なし(UI のタブは自作の `TabStrip`) |

Toolkit は取り込んだ後も「ライブラリの部品の一覧」として残す。

### 📊10-22 Chart(🟡📦📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/View/ViewChartView.xaml` + `ViewChartViewModel.cs` | 自作の描画のグラフの切り替え |
| `Graphics/Drawing/ChartDrawing.cs` | グラフの描画(折れ線・棒・ドーナツ・ローソク足・積み上げ・散布図・ヒートマップ) |

今: 軸の目盛り・凡例・値の表示が無い。UI の画面では使われていない。

- ① 軸の目盛りとラベル、凡例、タップした点の値の吹き出し、切り替えのときに伸びる動き
- ② UI の画面で使える形にする(値とラベルを渡す。10-3 Money の残高の推移、10-11 ダッシュボードの週の歩数と心拍)
- ③ 棒の形: 上から下へのグラデーション、最大までの薄い溝、選んだ棒の強調(タップで選ぶ)。10-3 ⑥ の週の支出と、10-11 ① の週の歩数で使う

使える部品(③): 単色の棒は `ChartDrawing`、グラデーションの棒は `Graphics/Drawing/LoadDrawing.cs`、溝とグラデーション(横)は `Controls/RangeBar.cs`。3 つを合わせた形は無い。

### 🌲10-23 Custom(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/View/ViewCustomView.xaml` + `ViewCustomViewModel.cs` | 自作の部品の一覧 |

今: `TreeView` の見本のデータに、今は無いファイル名(`Document/UI_Development_Log.md`)がある。

- ① 見本のデータを今のフォルダーの構成に合わせる(または部署と担当のような一般の木)
- `ColorPicker` は 10-11 の設定で使う

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
