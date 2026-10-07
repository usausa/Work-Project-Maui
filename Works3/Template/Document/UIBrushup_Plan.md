# 🎨UI のブラッシュアップの計画

UI のブラッシュアップの残作業はこの計画で管理する(`Task_Checklist.md` には載せない)。ここが終わってから `Task_Checklist.md` の項目に戻る。済んだ項目はこの計画から消し、内容は `Change_Summary.md` に書く。

| 部分 | 番号 | 内容 |
| --- | --- | --- |
| 🖼️UI 1 / UI 2 と View の部品の画面 | 10-x | UI 1 / UI 2 の画面を、App のミニアプリと同じ水準(画面の中で完結する操作、下に余白を残さない配置、色と描画で華やかに、一覧で選んだ項目が詳細に出る)へ寄せる。View の部品の画面(Toolkit / Custom / Chart / Sf Chart / Bottom Sheet / Drawer)でしか使っていない部品は、使いどころのある UI の画面へ取り込む。UI 2 のメニューの空き(8 段)には新しい画面を置く(10-29 / 10-30) |
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
| 10-26 | UI 2 Flight | 🔴 | 📦📦📦 | 👀 | ~~同じ内容のまま、空と地面のある姿勢の表示・目標の向きのあるレーダー・機体の上面図の兵装の画面に作り直す~~ |
| 10-27 | UI 2 Tactical | 🔴 | 📦📦📦 | 👀 | ~~同じ内容のまま、陰影のある地形図・脅威の範囲と一覧・機体の損傷の図のある画面に作り直す~~ |
| 10-28 | UI 2 Telemetry | 🟡 | 📦📦 | 👀 | ~~下の 6 項目を横長に、回転計・ブースト・ギアの行を大きく~~ |
| 10-29 | UI 2 Habit | 🟡 | 📦📦📦 | 👀 | ~~新しい画面。習慣の記録(光るグラデーションの輪・分類のチップ・連続日数)~~ |
| 10-30 | UI 2 Home | 🟡 | 📦📦📦 | 👀 | ~~新しい画面。スマートホーム(温度のダイヤル・機器のタイル・電池・ぼかした写真と半透明のカード)~~ |

### ✨既存の画面の細部

📝 11-x の項目は案のまま(まだ確認していない)。番号ごとに、実施の前に内容を確認し、修正の前後を見て確認する。

| 番号 | 対象 | 優先 | 変更量 | 状態 | 内容 |
| --- | --- | --- | --- | --- | --- |
| 11-0 | 共通 | — | — | 📝 | 共通の方針(スペース、状態の見せ方、ボタン、空・読み込み中、名前と値の行、配置)。決めたことは決定事項の表 |
| 11-4 | Main Diagnostics | 🟡 | 📦📦 | 👀 | ~~名前と値の行を詰める、ID・パスは等幅、状態はバッジ~~ |
| 11-5 | Main Setting | 🟡 | 📦 | 👀 | ~~節の見出しのアイコンの色~~ |
| 11-7 | Basic Style | 🟡 | 📦 | 📝 | Action 系のボタンの形 |
| 11-9 | Basic Converter | 🟢 | 📦 | 👀 | ~~名前と値の文字の大きさ~~ |
| 11-11 | Basic Dialog | 🟢 | 📦 | 👀 | ~~見出しの色~~ |
| 11-13 | Basic Setting | 🟡 | 📦📦 | 📝 | SearchBar の枠、日付・時刻の欄、Stepper |
| 11-16 | Navigation Dialog(数値入力) | 🟡 | 📦📦 | 📝 | 角丸、数字の帯、✓ と ✕ の区別、AC / C の色 |
| 11-17 | Device Info | 🟢 | 📦📦 | 👀 | ~~名前と値の強弱、カードごとの色~~ |
| 11-18 | Device Status | 🟡 | 📦📦 | 👀 | ~~値を 1 回だけ状態のチップで、列挙の名前を短い言葉に~~ |
| 11-19 | Device Sensor | 🟢 | 📦📦 | 📝 | 中央から左右に伸びるバー |
| 11-22 | Device QR Scan | 🔴 | 📦📦 | 👀 | ~~結果の帯、状態のチップ、プレビューに重ねた丸いボタン、読み取りの枠~~ |
| 11-23 | Device Camera | 🔴 | 📦📦 | 👀 | ~~黒地と状態のチップ、プレビューに重ねた丸いボタン~~ |
| 11-26 | Device Bluetooth | 🟡 | 📦📦 | 📝 | 段階の表示 |
| 11-27 | Device BLE Scan | 🟢 | 📦 | 📝 | 行の角丸 |
| 11-28 | Device BLE Host | 🟡 | 📦📦 | 📝 | 広告中の色と動き |
| 11-29 | Device NFC | 🔴 | 📦📦 | 👀 | ~~IC カード風のカード、履歴をフラットな行に~~ |
| 11-31 | Device Activity | 🟡 | 📦📦 | 📝 | 歩数の強調と輪のグラデーション、3 つのタイル、活動時間の書式 |
| 11-32 | Device Biometric | 🟡 | 📦📦 | 👀 | ~~状態の色のバッジ~~ |
| 11-33 | Device Communication | 🟡 | 📦📦 | 📝 | 連絡先の画面の形、外のアプリを開く印 |
| 11-34 | Device Misc | 🟢 | 📦 | 👀 | ~~見出しの色~~ |
| 11-36 | Network HTTP (Auth) | 🟡 | 📦📦 | 👀 | ~~ログインの状態の表示~~ |
| 11-38 | Network Realtime | 🟡 | 📦📦 | 📝 ②👀 | グラフの枠、~~状態のチップ~~ |
| 11-39 | Network gRPC | 🟡 | 📦📦 | 👀 | ~~接続の状態~~ |
| 11-40 | Network SFTP | 🟢 | 📦📦 | 📝 | 接続先と指紋、転送を Storage と同じ形、ログを下まで |
| 11-47 | View Custom | 🟢 | 📦📦 | 📝 | TreeView の印とアイコン |
| 11-53 | View Drag & Drop | ⚖️ | 📦 |  | かんばんのカードの角丸 |
| 11-59 | Sample Web Basic | 🔴 | 📦📦 | 👀 | ~~中のページの見た目~~ |
| 11-65 | Sample CV Local | 🟡 | 📦 | 📝 | 推論中の表示と撮影のフラッシュ |
| 11-67 | 文字のスペース | 🟢 | 📦📦📦 | 📝 | 対象の画面の文字のスペースを決まりにそろえる(日本語と英語・数字の間、括弧の前は詰める) |

## ⚖️決定事項

### 🖼️UI 1 / UI 2 と View の部品の画面

| 項目 | 決定 |
| --- | --- |
| 10-21 Toolkit | 残す(ライブラリの部品の一覧) |
| 10-26 Flight / 10-27 Tactical | 同じ内容のまま、デザインを作り直す |
| 10-28 Telemetry | 表示項目は変えない。下の 6 項目は横長の枠にし、空いた高さを回転計・ブーストとギアの行に回す。ギアは ERS・G-FORCE より大きくし、ギアの枠には角のラインを付けない |
| 下から出るシート | 自作の `BottomSheetView` を優先する(SfBottomSheet ではなく) |
| 10-2 Login | 対象外 |
| 10-29 Habit / 10-30 Home | UI 2 のメニューの空き(8 段)に新しい画面として作る。ヘッダーと F キーの帯は隠す。文字は日本語。Habit の強調(琥珀と 🔥)は今日 100% の習慣だけ。Home の下のタブは部屋の切り替え。Home だけで使う部品は 1 つのファイル(`Controls/HomeControls.cs`)にまとめる |

### ✨既存の画面の細部

| 項目 | 決定 |
| --- | --- |
| 言葉 | 英語と日本語の混在はそのまま(日本語にしたのは UI 2 の Habit・Home)。題・メニュー・F キー、機材・計器風の英語、方位、英語のバッジは英語のまま。曜日は端末の言語、確認のダイアログの Cancel は MauiComponents の既定のまま |
| スペース | 日本語と英語・数字の間、括弧の前には半角スペースを入れない。英単語を複数並べるときは半角スペースで区切る。そろえるのは対象の画面の文字だけ(11-67) |
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
| スペース | 日本語と英語・数字の間、括弧の前は詰める。英単語を複数並べるときは半角スペース。直すのは 11-67 |
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

### ✈️10-26 Flight(🔴📦📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UIFlightView.xaml` + `UIFlightViewModel.cs` | 画面(全面の `SceneControl`) |
| `Graphics/Scene/FlightHudScene.cs` | 戦闘機の HUD の描画とレーダーのタップ |

今: 黒地に緑の線だけの HUD で、姿勢の表示とレーダーの間に大きな空きがある。文字が小さい(8〜9)。機体が傾くと、ピッチの目盛りの数字が切れる。

表示する内容(モード・燃料・G・推力、方位、姿勢、速度・マッハ・迎え角、高度・昇降率・気圧、レーダーと目標の選択、機銃と 6 本のミサイル、状態の行)はそのままに、デザインを作り直す。

- ~~① 見出し: モードのチップ、システムの状態、燃料の残りの棒、G と推力~~
- ~~② 姿勢の表示(PFD): 機体の傾きとピッチで回る空と地面、ロールの目盛りの内側だけのピッチの目盛り、上の方位の帯と読み取りの枠、速度と高度の帯と内側を指す読み取りの枠、6 秒後の速度の見込みの矢印、機首の印と飛んでいく向きの印~~
- ~~③ レーダー: 距離の輪と方位の目盛り、走査の帯、目標の印と 10 秒後の位置への線、選んだ目標のロックの枠と方位・距離~~
- ~~④ 兵装: 機体の上面図に 6 本のミサイル(次に撃つものを点滅の枠)、機銃の残弾の棒、FOX 2 の帯、MASTER ARM のチップ~~
- ~~⑤ 下の状態の行はチップに。変わらない部分は画像にして写すだけにする(描画は 1 フレーム平均 13〜14 ms)~~

実施(2026-10-04)、確認待ち。結果は `Change_Summary.md` の区間 17。

### 🗺️10-27 Tactical(🔴📦📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UITacticalView.xaml` + `UITacticalViewModel.cs` | 画面(全面の `SceneControl`) |
| `Graphics/Scene/MechHudScene.cs` | 地上部隊の戦術の画面の描画 |

今: 黒地に緑の線だけの画面。通信の枠の SIG の強さの記号がフォントに無く、四角(豆腐)で出る。小隊 D2 / D3 の名前が動く隊員の点と重なる。文字が小さい(8〜10)。

表示する内容(部隊・モード・経過時間・時計・データリンク・損傷、通信のチャンネル・暗号鍵・波形・送受信・信号、戦術の地図の地形・格子・味方・敵・目標・自機、状態の行)はそのままに、デザインを作り直す。

- ~~① 見出し: 部隊の記章、モードのチップ、データリンクの点滅、損傷の状態のチップ~~
- ~~② 通信: 選んだチャンネルの帯、暗号鍵のチップ、オシロスコープの波形(送信中は大きく揺れる)、信号の強さの棒~~
- ~~③ 機体: 部位ごとの損傷を色で示す機体の図(いちばん傷んだ部位の枠が点滅)、全体の耐久と最も傷んだ部位。値はモデルの部位の耐久をそのまま使う~~
- ~~④ 地図: 標高を色と陰影で塗った地形と等高線、格子、縮尺と北、センサーの走査、自機の視界の扇、敵の足跡と脅威の範囲、目標の輪と経路と距離・方位、近い順の脅威の一覧(右下に半透明)~~
- ~~⑤ 下の状態の行はチップに。変わらない部分は画像にして写すだけにする(描画は 1 フレーム平均 12〜13 ms)~~

実施(2026-10-04)、確認待ち。結果は `Change_Summary.md` の区間 17。

### 🏎️10-28 Telemetry(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UITelemetryView.xaml` + `UITelemetryViewModel.cs` | 画面(F2 で描き方の切り替え) |
| `Graphics/Scene/TelemetryScene.cs` | 車のテレメトリの描画 |

今: 下の 6 項目が正方形の枠で、半円の計器の上に空きがある。

- ~~① 下の 6 項目を横長の枠にし、半円の計器を枠の下寄りに置く~~
- ~~② 空いた高さを、回転計とブースト(半径 60 → 70)、ギアの行に回す~~
- ~~③ ギアの行は、ギア(幅 112・高さ 150)を ERS と G-FORCE(半径 44)より大きくし、ギアの枠には角のラインを付けない(下の 6 項目の枠には残す)~~
- 表示項目は変えない

実施(2026-10-04)、確認待ち。結果は `Change_Summary.md` の区間 17。

### 🧱新しい画面の作り方(10-29 / 10-30)

| 項目 | 内容 |
| --- | --- |
| メニュー | UI 2(`Modules/UI/UIMenu2View.xaml`)の 8 段目の 2 つの空き(無効のボタン)に置く。メニューの大きさと色は変えない |
| 画面 ID | `Modules/ViewId.cs` の UI の区画の最後(今は `UIEnergy`)の後に、メニューの順で足す |
| 結線 | コードビハインドに `[View(ViewId.X)]`、メニューのセルに `{markup:ViewId X}`、戻るは `OnNotifyBackAsync` で `ForwardAsync(ViewId.UIMenu2)`(`UIEnergyViewModel.cs` と同じ) |
| 全面の画面 | `ShellProperty.HeaderVisible` / `FunctionVisible` を `False` にすると、ヘッダーと F キーの帯が消える(AppTodo・AppTimer・ゲーム・UISocial・UIDock) |
| 描画 | 使っているぼかしは `SKMaskFilter.CreateBlur`(光と影)と `SKImageFilter.CreateDropShadow` だけで、画像やその下のぼかし(`RenderEffect` など)は無い。全面のグラデーションと `SKMaskFilter` をシーンの CPU のバッファで描くと重いので、変わらない層は画像にして写し、光は半透明の形の重ねで描く。ぼかしは `SKCanvasView` の端で切れるので、ぼかしの半径の 2 倍ほど内側に余白を取る |
| 再利用の候補 | 光る輪 = `Controls/TimerDial.cs`(いちばん近い)/ `Controls/ArcMeter.cs`(グラデーションだけ)。ダイヤルの入力 = `Controls/MixerKnob.cs`(`IInteractiveDrawing` でもドラッグを受けられる。`MixerKnob` は変えずに、Home のダイヤルは新しい部品にする)。電池 = `EnergyFlowScene.DrawBatteryTile`。光 = `TimerDial` / `SceneObject` の光の線 / `Label.Shadow` / `AnimationOption.Pulse` |

### 🔥10-29 Habit(🟡📦📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UIHabitView.xaml` + `UIHabitViewModel.cs`(新規) | 習慣の記録(暗い色。今日の進み・習慣ごとの輪・分類・連続日数) |
| `Modules/UI/UIMenu2View.xaml` / `Modules/ViewId.cs` | UI 2 のメニュー(8 段の空き)/ 画面 ID |

見本の画像: `__Image/Mockup/Habit.jpg`。

今: 習慣の記録の画面は無い。光るグラデーションの輪は App > Timer の `TimerDial`(表示だけ。12 時の位置から始まり、目盛りとつまみが付く)、色の輪は Weather の `ArcMeter`、チップは ToDo・Profile・Monster にある。連続日数の表示は無い。タスクの一覧は App > ToDo と、アイコンだけの下のタブは Money と重なるので入れない。

- ~~① 見出し(丸いアイコンのボタン・日付・大きな題)、今日の進み(18 中 12 と光るグラデーションの輪)、習慣のカード(分類の色の輪と割合、分類のチップ、名前、目標、連続日数)。分類の色は青緑・紫・橙・青~~
- ~~② 習慣のタップで進みを足し、100% で ✓ と弾む動き。今日の進みの輪も伸びる~~
- ~~③ 連続日数の強調(7 日以上は 🔥 と琥珀)と、週の 7 日の達成の点~~
- ~~④ 光る輪を部品にする(`TimerDial` の描き方を切り出す)。10-11 ① の歩数の輪と 11-31 の Activity の輪でも使う~~

結果(👀 確認待ち): ① 暗い地の全面の画面(上の帯も #121212)。丸い戻る・メニューのボタンと日付「10月7日(水)」、題「今日の習慣」、今日の達成率のカード(「12/18完了」・光る輪・週の 7 日の点)、習慣のカード 4 枚(分類の色の輪と割合、分類のチップ 運動・健康・学習・仕事、名前、目標、連続日数)。② カードのタップで 1 回分進み、100% で ✓ と弾み、連続日数が 1 増える。③ 琥珀と 🔥 は今日 100% の習慣だけ(7 日以上の条件はやめた)。週の点は今日の全部が終わると塗る。④ 光る輪は `Controls/GlowRing.cs`(Timer の文字盤も同じ光る弧の描き方を使う)。メニューのアイコンは炎、見本のデータはメモリーの中だけ(画面を出ると戻る)

使える部品:

| 要素 | 既存の画面・部品 |
| --- | --- |
| 見出し(丸いボタン・日付・大きな題) | App > ToDo(丸いボタン・日付)、App > Timer(暗い色の上の帯) |
| 光るグラデーションの輪 | `Controls/TimerDial.cs` |
| 習慣ごとの色の輪 | `Controls/ArcMeter.cs`(Weather の湿度の輪)、`Graphics/Drawing/ActivityDrawing.cs`(明るい色) |
| 分類のチップ | App > ToDo の期限のチップ、Profile・Monster・Timeline のタグ |
| 小さな灰色の大文字の見出し | Gauge の `GaugePanelTitle`、Kit の Setting |
| 連続日数と 🔥 | 無い(🔥 の文字だけ Visit・Sudoku にある) |

### 🏠10-30 Home(🟡📦📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UIHomeView.xaml` + `UIHomeViewModel.cs`(新規) | スマートホームの操作(暗い色と半透明のカード。部屋の状態・温度のダイヤル・機器のタイル・電池) |
| `Controls/`(新規) | 温度のダイヤル、電池の残量、ぼかした背景の写真 |
| `Modules/UI/UIMenu2View.xaml` / `Modules/ViewId.cs` | UI 2 のメニュー(8 段の空き)/ 画面 ID |

見本の画像: `__Image/Mockup/Home.jpg`。

今: スマートホームの画面は無い。近いのは Energy(工場の電力の監視。設備のタイル・電池の描画・状態の点)、Weather(半透明のカード。ぼかしは無い)、Mixer(ドラッグで回すつまみの `MixerKnob`。光は無い)、Timer(光る輪)。画像をぼかす部品と、部屋の写真は無い。

- ~~① 見出しのパネル(部屋の名前・接続の状態・室温・シーン)、温度のダイヤル(ドラッグで設定、−/+ のボタン、橙の光る弧とつまみ)、機器のタイル 3 × 2(押すと点いてアイコンが光る、明るさ・開き具合のスライダー、状態の点)、凡例の点(緑・琥珀・赤)。暗い地に、Weather と同じ半透明のカード~~
- ~~② 電池の残量の行(Energy の `DrawBatteryTile` を部品にする。残量で緑・琥珀・赤)~~
- ~~③ 背景の写真をぼかして暗く描く部品(`SKImageFilter.CreateBlur` で 1 回描いて写す)。写真は既存のもの(`Profile/gallery05.jpg` の窓辺の部屋、`gallery06.jpg` の夜の街)を使う~~
- ~~④ 下のタブ(半透明と光る印)で部屋(リビング・寝室・キッチン)を切り替える~~

結果(👀 確認待ち): ① 全面の画面(上の帯は #101418)。見出しのパネル(戻る・「スマートホーム」・部屋の名前・「オンライン | 室温21°C | 夕暮れのシーン」・人と設定のアイコン)、空調のダイヤル(輪のドラッグと −/+ で 0.5 度刻み、16〜30°C。運転のスイッチを切ると弧が灰色で「停止」)、機器のタイル 3 × 2(タップでオン・オフ。照明とランプは明るさのスライダーで、消すと 0%・点けると消す前の明るさ。ブラインドは開・閉と開度、鍵は施錠・解錠と安全・要確認、ファンは風量、エアコンは冷房の温度。消えているときは灰色)、凡例の点(正常 緑・動作中 琥珀・異常 赤)。② 電池の行は Energy の `DrawBatteryTile` を使わず新しい部品にした(60% 以上 緑・30% 以上 琥珀・それ未満 赤、言葉は 十分・良好・普通・少ない)。③ 部屋ごとの写真をぼかして暗く敷く(リビング = `gallery05.jpg`・寝室 = `gallery06.jpg`・キッチン = `gallery02.jpg`)。④ 下のタブで部屋を切り替える(選んだ部屋は水色と光る線。機器・電池・空調・背景が入れ替わる)。Home だけの部品は `Controls/HomeControls.cs` にまとめた。メニューのアイコンは家、見本のデータはメモリーの中だけ

使える部品:

| 要素 | 既存の画面・部品 |
| --- | --- |
| 見出しのパネル(状態の行) | Weather の見出しと半透明のピル、`Graphics/Scene/EnergyFlowScene.cs` の点滅する状態の点と NORMAL のピル |
| 温度のダイヤル | `Controls/MixerKnob.cs`(ドラッグで回す。光は無い)、`Controls/TimerDial.cs`(光る弧とつまみ。表示だけ)、`Controls/ArcMeter.cs`。両方を合わせた部品は無い |
| −/+ の丸いボタン | Cart の `StepperButton`、Item、POS |
| 光るスイッチ | 標準の `Switch`(光は無い。光は `Shadow` で付けられる: Meter) |
| 凡例の点 | `EnergyFlowScene.cs` のイベントの点、Kit の Tracking の段階の点 |
| 機器のタイル | Weather の半透明のタイル 3 列、`EnergyFlowScene.cs` の設備のタイルと角の LED、Dock の暗い色のタイル。点く・消える・光るタイルは無い |
| 電池の残量 | `EnergyFlowScene.cs` の `DrawBatteryTile`(外形・端子・量のグラデーション・20% 未満で警告の色。部品ではない) |
| 半透明のカード | Weather の `WeatherCardBorder`(`#26FFFFFF` の地・`#38FFFFFF` の枠・角丸 20)、App > Timer。写真に暗い覆いは Stream・Social。ぼかしは無い |
| 半透明の下のタブ | 無い(明るい色の Money・Mail だけ) |

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

## 🧭既存の画面の共通の手直し

### 🧹11-67 文字のスペース(🟢📦📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| 対象の画面の XAML(`Text` / `Title` / `Placeholder` など)と ViewModel の文字列 | 画面に出る文字 |

今: 日本語と英語・数字の間や括弧の前に、半角スペースがある所と無い所が混ざる(「管理画面の Devices から」「0 回 (10 秒ごと)」「エントランス(FadeUp 時間差)」など)。

- ① 日本語と英語・数字の間、括弧の前の半角スペースを詰める。英単語を複数並べる所は半角スペースのまま
- 直す前に、対象の文字の一覧(今と直した後)を作って確認する。記号(: / → など)の前後や、英語だけの文字の括弧(HTTP (Data) など)の扱いも、そのときに確認する
- UI 1 / UI 2 / App の画面とドキュメントは対象外(日付の括弧の前のスペースも今のまま)

## 🏠Main

### 🩺11-4 Diagnostics(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Main/DiagnosticsView.xaml` + `DiagnosticsViewModel.cs` | 端末・起動・DB・通信・テレメトリ・ログの診断 |

今: 名前 18 の太字と値 18 の 2 列のカードが 8 枚続いて縦に長く、Path だけ 12 で大きさがそろわない。状態(Sending / Not sent / Failed)は色の付いた文字だけで、目に入りにくい。

- ~~① 名前と値の行を 11-0 の形(16・1px の区切り線)にして縦を詰める~~
- ~~② ID・Path・日時は JetBrainsMono の 14 にそろえる~~
- ~~③ Status / Last send / Crash は白文字の色付きのバッジにする~~

結果(👀 確認待ち): ①② Device・Application・Startup・Database・Connection・Telemetry の表を、名前と値の行の形(名前 14 の灰・値 16 の濃い色・行の間に 1px の線)にし、ID・Path・日時は等幅の 14 に。③ Status(Sending 緑 / Stopped 灰)・Last send(Succeeded 緑 / Failed 赤 + 時刻、理由はその下)・Crash(Not sent 琥珀 / None 灰)を淡い地のチップに(バッジの形は I の決定でチップにした)。

### ⚙️11-5 Setting(🟡📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Main/SettingView.xaml` + `SettingViewModel.cs` | 設定の QR の読み取りと、今の設定値の表示 |

今: 節の見出しのアイコン(3 つ)が全部同じ灰青(`BlueGrayDarken2`)。

- ~~④ 節の見出しのアイコンに節ごとの色を付ける~~

結果(👀 確認待ち): ④ Network = 青・AI Service = 紫・SSH = 緑。

## 🧱Basic

### 🖌️11-7 Style(🟡📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Basic/BasicStyleView.xaml` + `BasicStyleViewModel.cs` | 共有のボタンのスタイルと、選択のボタンの見本 |

今: Action / Information のボタンは角 0・文字 24 の板で、ほかの画面の角丸 8・文字 14 のボタンと比べて古い。

- ③ ⚖️ Action 系の共有のスタイル(`ActionButtonBase` と Primary〜Error。`BasicFilledButton` / `BasicFilledSecondaryButton` の基底でもある)を角丸 8・文字 18 にするか

### 🔁11-9 Converter(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Basic/BasicConverterView.xaml` + `BasicConverterViewModel.cs` | Bool / Multi / Text のコンバーターの見本 |

今: 同じカードにチェックボックスの名前 14 と名前 18 の太字が混ざる。

- ~~③ 文字の大きさを 11-0 の名前と値の行でそろえる~~

結果(👀 確認待ち): ③ 3 枚の表を名前と値の行の形に。

### 💬11-11 Dialog(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Basic/BasicDialogView.xaml` + `BasicDialogViewModel.cs` | 通知・確認・進捗・フィードバックのダイアログの見本 |

今: 4 枚のカードの見出しのアイコンが全部同じ灰青。

- ~~③ 見出しのアイコンに種類ごとの色を付ける(11-0)~~

結果(👀 確認待ち): ③ Notification = 青・Confirm / Select / Input = 緑・Progress = 琥珀・Feedback = 紫。

### 🎚️11-13 Setting(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Basic/BasicSettingView.xaml` + `BasicSettingViewModel.cs` | 設定の画面でよく使う入力の部品の見本 |

今: SearchBar だけ枠が無く大きく字下げされ、上の Entry の枠と形がそろわない。Stepper の - / + は Android の標準の灰色の四角で浮いて見える。日付・時刻は未設定だと短い下線だけが出て崩れて見える。

- ① SearchBar も `CardFieldBorder` に入れて下線を消す(`EntryOption.NoBorder` の対象に SearchBar を足す)
- ② 日付・時刻のピッカーを幅をそろえた枠の欄に入れ、未設定でも空の欄に見せる
- ③ Stepper のボタンは白地・枠・角丸に寄せる(ハンドラーでの見た目の調整が要る)

## 🧭Navigation

### 🔢11-16 Dialog(数値入力)(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Navigation/Modal/InputNumberView.xaml` + `InputNumberViewModel.cs` | 数値入力のポップアップ(App の Timer の分の入力でも使う)。スタイルは `Styles.xaml` の Dialog の区画 |

今: 角の無い箱に、題の帯と数字の帯が重なる。✕ と ✓ が同じ青で、決定と取り消しの区別が無い。AC / C の琥珀の塗りも強く、Timer から開くと画面の質の差が目立つ。

- ① 角丸 16 にし(`MauiProgram` で Popup の既定の Shape を null にしているのを変える。Popup はこの 1 つだけ)、題は 20 の太字にする
- ② 数字の帯は淡い色の面に濃い文字にする
- ③ ✓ は青の塗り、✕ は白地に灰色の文字にする
- ④ AC / C は淡い琥珀の面に濃い琥珀の文字にする(押すと縮む効果は付けない)

## 📱Device

### ℹ️11-17 Info(🟢📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceInfoView.xaml` + `DeviceInfoViewModel.cs` | 端末・アプリ・画面の情報 |

今: 名前と値の表 3 枚だけで、太字の名前の方が灰色の値より目立つ。アイコンは 3 枚とも同じ灰色で、Emulator は「False」のまま。下の 4 分の 1 が空く。

- ~~② 値を濃い色、名前を小さい灰色にして強弱を入れ替える(11-0)~~
- ~~③ カードごとにアイコンの色を付け(端末 = 青、アプリ = 緑、画面 = 紫)、パッケージ名は等幅にする~~

結果(👀 確認待ち): ② 3 枚の表を名前と値の行の形に。③ Device = 青・Application = 緑・Display = 紫、パッケージ名は等幅の 14。

### 🔋11-18 Status(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceStatusView.xaml` + `DeviceStatusViewModel.cs` | 電池と通信の今の状態 |

今: Network はチップの列と下の表が同じ 3 つの値を二重に出し、値は列挙の名前のまま(ConnectedHighSpeed / Usb など)。充電中でもチップは灰色。電池のバーは細い既定の ProgressBar。下の 4 割が空く。

- ~~① 表の値の側を状態の色のチップにして、上のチップの列はやめる(各値を 1 回だけ出す)~~
- ~~② 列挙の値を絵文字付きの短い言葉にし(コンバーター。例 ⚡ 充電中 / 🔌 USB / 📶 高速)、充電中は緑にする~~

結果(👀 確認待ち): ① Network の上のチップの列をやめ、表の Access と State をチップに(Profile は種類なので文字のまま)。② 電池は 充電中・満充電(緑)/ 放電中・充電していない・電池なし・不明(灰)、電源は AC電源・USB・ワイヤレス・電池・不明(灰)、Access は インターネット(緑)・制限付き・ローカル(琥珀)・なし(赤)・不明(灰)、State は 高速(緑)・接続(琥珀)・切断(赤)。絵文字ではなく色で示す。

### 🧲11-19 Sensor(🟢📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceSensorView.xaml` + `DeviceSensorViewModel.cs` | 加速度・ジャイロ・磁気・姿勢・方位・水準器・気圧 |

今: 軸の色のバッジと等幅の値で整っているが、符号付きの値のバーが左端から伸びるので、0 でも中央まで塗られ、負の値は短い棒に見える(バーは細い既定の ProgressBar)。

- ① 中央の目盛りから左右に伸びるバーにする(`RangeBar`。Low と High に、0 と値の小さい方・大きい方)
- ② バーを太さ 8 前後の角丸にし、溝を軸の色の淡い色にする

### 🔍11-22 QR Scan(🔴📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceQrScanView.xaml` + `DeviceQrScanViewModel.cs` | カメラでのバーコード・QR の読み取り |

今: プレビューの下の結果と状態の文字が左端に付いて余白が無く、「Invert: False」「Zoom: x-1」と生の値が出る。Torch / Aim / ZoomOut / ZoomIn は水色の文字のボタンの帯(`SubMenuButton`。ZoomOut は文字が枠いっぱい)で、F キーと合わせて原色の帯が 2 段になる。

- ~~① 結果を余白のある帯にする(QR のアイコンと値、読み取る前は案内)~~
- ~~② 状態を ON / OFF のチップにする(ON は色付き、ズームは値が無ければ「-」)~~
- ~~③ Torch / Aim / Zoom をプレビューに重ねた丸いアイコンのボタン(半透明の黒地)にして、水色の帯をやめる~~
- ~~④ プレビューの中央に読み取りの枠(四隅のかぎ)を置く~~

結果(👀 確認待ち): 黒地(`DarkRootGrid`)に。① 下に OCR と同じ白い帯(上の角丸 16)を置き、青の QR のアイコン(28)と値(16・2 行まで)、読む前は灰色の「Scan a code...」。② プレビューの上に半透明の黒のチップ(12 の白)を 4 つ。Camera Back / Zoom x1.0(値が無ければ「Zoom -」)/ Invert ON・OFF / Vibe ON・OFF で、ON は青の地。③ プレビューの下に半透明の黒の丸いボタン(52)を 4 つ(ライト・照準・縮小・拡大)。ライトの ON は琥珀の地とアイコンの切り替え、照準の ON は青の地。④ 中央に 260 の枠の四隅のかぎ(`CameraOverlayView` の格子なし)。文字は今の英語のまま

### 📸11-23 Camera(🔴📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceCameraView.xaml` + `DeviceCameraViewModel.cs` | カメラのプレビューと撮影 |

今: プレビューの上下の黒い帯の下に白い状態の帯があって境目がきつく、状態は左端に付いた「Torch: False」などの文字。Torch / Flash / ZoomOut / ZoomIn は水色の文字のボタンの帯(`SubMenuButton`)。

- ~~① 画面を黒地(`DarkRootGrid`)にし、状態はプレビューの上の半透明のチップにする(🔦 OFF / ⚡ Off / 🔍 x1)~~
- ~~② Torch / Flash / Zoom を、プレビューに重ねた丸いアイコンのボタン(半透明の黒地)にして、水色の帯をやめる(切り替え・撮影は F キーのまま)~~
- ~~11-22 と 11-23 で水色の帯をやめると、`SubMenuButton` を使う画面が無くなる(削除する)~~

結果(👀 確認待ち): ① 黒地にし、状態は QR Scan と同じ半透明のチップ(カメラの名前 / Zoom x1.0 / Flash Off・On・Auto / Torch ON・OFF。絵文字は使わない決定に合わせて文字に)。Flash On とライトの ON は琥珀、Flash Auto は青の地。② QR Scan と同じ丸いボタン(ライト・フラッシュ・縮小・拡大。フラッシュのアイコンは Off・On・Auto で切り替え)。この端末はプレビューの上下に黒い余白が出るので、チップとボタンはその黒の上に乗る。共有の `SubMenuGrid` / `SubMenuButton` は削除

### 🖨️11-26 Bluetooth(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceBluetoothView.xaml` + `DeviceBluetoothViewModel.cs` | Bluetooth のシリアルでの試し印刷 |

今: 状態はアイコンの円の色とチップの言葉で示し、段階(接続 → 送信 → 完了)の表示は無い。

- ② 段階(接続 → 送信 → 完了)を `StepIndicator` で見せる

### 🌡️11-27 BLE Scan(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceBleScanView.xaml` + `DeviceBleScanViewModel.cs` | SwitchBot の温湿度計・CO2 の値の受信 |

今: 液晶を真似たパネルで個性がある。行は角丸 12 の枠で、11-0 の一覧の行(フラット)の案とは形が違う。

- ③ ⚖️ 行の角丸は、液晶のパネルの見立てとして残す(案)

### 📡11-28 BLE Host(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceBleHostView.xaml` + `DeviceBleHostViewModel.cs` | BLE のペリフェラル(広告)の開始と停止 |

今: 中央のカードだけで上下が空き、円は広告中も停止中も青。

- ② 広告中は円を緑にして、電波の輪が広がる動きにする

### 💳11-29 NFC(🔴📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceNfcView.xaml` + `DeviceNfcViewModel.cs` | Suica の残高と利用履歴の読み取り |

今: Metro 風の濃い灰色と原色の箱で古く見える。履歴は 1 件ごとに日時 3 段・処理・残高の大きな箱で、1 画面に 7 件ほどしか入らない。

- ~~① 読み取った後の上の帯を IC カード風のカードにする(グラデーション、IDm は等幅、残高は大きくカウントアップ)~~
- ~~② 履歴をフラットな行にする(左に処理の色の細い帯、日時は「05/12 11:24」の 1 行、端末と処理は絵文字付きのバッジ、残高は右寄せ)~~

結果(👀 確認待ち): 暗い地はそのまま。① 角丸 16 の緑のグラデーション(GreenDarken4 → GreenDarken1 → LightGreenLighten1)のカードに、Suica の文字と非接触のアイコン、残高(40 の太字・数え上げ)、IDm(等幅 16)。② 行は左に処理の色の帯(4)、日時(等幅 14。物販のときだけ時刻)、端末(灰)と処理(処理の色)のバッジ、右に残高(20)、行の間に 1px の線。バッジの絵文字は、絵文字は使わない決定に合わせて付けず、処理の色で分けた。カードがあるときの見た目は、一時的に見本のデータを入れて確かめた(Suica は手元に無い)

### 🚶11-31 Activity(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceActivityView.xaml` + `DeviceActivityViewModel.cs` | 歩数の輪と、消費カロリー・距離・活動時間 |

今: 輪と歩数で主役ははっきりしているが、歩数は細い灰色の文字で、下の表は名前・値・単位がすべて 24 で同じ強さ。活動時間は「0.00 時間」。下の 4 分の 1 が空く。

- ① 歩数を太字の濃い色にし、輪の進みを Timer の文字盤のようなグラデーションにする
- ② 下の 3 項目を横並びの 3 つのタイル(絵文字、太字の大きな値、小さな単位)にし、活動時間は「0:00」(時:分)にする

### 👆11-32 Biometric(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceBiometricView.xaml` + `DeviceBiometricViewModel.cs` | 生体認証の可否・本人確認・鍵での署名と検証 |

今: 状態の値がすべて青の文字なので、使える / 使えないが色で分からず、3 列のタイルでは「一時的に使えない」が途中で折り返す。Authenticate の Result だけ名前と値の 1 行で、ほかのタイルと形が違う。

- ~~① 状態の値を状態の色のバッジにし(使える = 緑、未登録 = 琥珀、一時的に使えない = 灰、センサー無し = 赤)、文言は 1 行に収まる長さにする(例 ⏳ 一時不可)~~

結果(👀 確認待ち): ① 使える(緑)・未登録(琥珀)・一時不可(灰)・センサー無し(赤)のチップに(絵文字はやめた)。

### ☎️11-33 Communication(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceCommunicationView.xaml` + `DeviceCommunicationViewModel.cs` | 電話・SMS・メールのアプリを開く |

今: 3 つの操作は角丸のカードの行で、下の 3 分の 2 が空く。行の右の「>」はアプリの中の次の画面へ進む印に見えるが、実際には外のアプリが開く。

- ① 上に宛先(番号とアドレス)を置き、その下に丸い 3 つの操作のボタン(電話 = 緑、SMS = 青、メール = 赤)を並べた連絡先の画面の形にする
- ② 行の形のまま残すなら、1 枚の白い面の中のフラットな行にし、「>」を外のアプリを開く印(`Open_in_new`)にする

### 🧰11-34 Misc(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceMiscView.xaml` + `DeviceMiscViewModel.cs` | 画面・振動・ライト・音声・通知の操作の見本 |

今: 節ごとのカードで整っているが、ボタンがすべて同じ灰色の枠なので、対の操作(Keep on / off、Portrait / Landscape、Light on / off)が対に見えない。見出しのアイコンも全部同じ灰色。

- ~~② 節の見出しのアイコンに色を付ける(画面 = 青、振動 = 紫、ライト = 琥珀、音声 = 緑、通知 = 赤)~~

結果(👀 確認待ち): ② 画面 = 青・振動 = 紫・ライト = 琥珀・音声 = 緑・通知 = 赤。

## 🌐Network

### 🔑11-36 HTTP (Auth)(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Network/NetworkAuthView.xaml` + `NetworkAuthViewModel.cs` | ID だけのログイン(JWT)と、JWT が要る API の呼び出し |

今: 状態とトークンの期限がボタンの下の本文 2 行で目立たない。カード 2 枚で終わり、画面の下の約 4 割が空く。ボタンはすべて同じ枠のボタン。

- ~~① ログインのカードの先頭に状態の表示を置く(鍵のアイコンの丸。未ログインは `BasicEmptyIconBorder` の灰、ログイン済みは `BasicSuccessIconBorder` の緑。`StatusChip` を並べ、期限は名前と値の行)~~

結果(👀 確認待ち): ① カードの先頭に丸い鍵のアイコン(未ログインは灰の Lock、ログイン済みは緑の Lock_open)と状態のチップ(未ログイン 灰 / ログイン済み 緑 + ID)、その下にトークンの期限の行(等幅)。説明・入力欄・ボタンはその下。

### 🔄11-38 Realtime(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Network/NetworkRealtimeView.xaml` + `NetworkRealtimeViewModel.cs` | SignalR の常時接続で、サーバーの状態のグラフと通知を受ける |
| `Controls/StatControl.cs` | グラフ(この画面だけで使う) |

今: グラフは白い枠(余白 8)の中に角の無い色の四角を置いた二重の枠。名前が上端と左端にほぼ接していて、データが無いとただの色の塊に見える。状態は「状態: 接続中...」の文字で、エラーは赤の小さな文字。

- ① グラフを角の丸い色の面にして二重の枠をやめる。名前は 14 の太字で余白 12 にし、データが無くても薄い目盛りの横線を出す
- ~~② 状態は `StatusChip` にする(緑 = 接続済み / 琥珀 = 接続中・再接続中 / 赤 = 停止・エラー)。接続 ID・サーバー時刻・送信の回数は名前と値の 2 列(`CardInfoGrid`)にし、エラーは淡い赤の帯で出す~~

結果(👀 確認待ち): ② 状態をアイコン付きのチップ(接続済み 緑・接続中・再接続中 琥珀・エラー 赤・停止 灰)に、接続ID・サーバー時刻・状態の送信を名前と値の行に、エラーを淡い赤の帯に(停止は I の決定で灰)。

### 🗨️11-39 gRPC(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Network/NetworkGrpcView.xaml` + `NetworkGrpcViewModel.cs` | gRPC の双方向ストリーミングのチャットと単項 RPC |

今: チャットの枠は、空のとき左上に小さな説明の文があるだけの大きな白い箱。入力欄は下線で、送信は小さな枠のボタンなので、上の枠と端がそろわない。サーバー時刻の結果「-」がボタンの横に浮いている。

- ~~① 接続のカードを 11-38 と同じ形にする(状態のチップ、アドレスは等幅、エラーは淡い赤の帯)。サーバー時刻の結果はボタンの下の名前と値の行に出す~~

結果(👀 確認待ち): ① 11-38 と同じ形(接続済み 緑・接続中・再接続中 琥珀・切断 灰)。アドレスは等幅、未配送の送信を行にして説明をその下に、サーバー時刻はボタンの下の行に。

### 🔐11-40 SFTP(🟢📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Network/NetworkSftpView.xaml` + `NetworkSftpViewModel.cs` | SFTP でのアップロード・ダウンロードと、サーバーのホスト鍵の表示 |

今: 接続先はホストの本文 1 行。転送は下線の入力欄・細い進捗・枠のボタン 3 つで、11-37 と同じく強弱が無い。ログのカードは、転送する前は「まだ転送していません」の 1 行だけ。

- ① ホストを等幅にしてサーバーのアイコンの行に置き、指紋は淡い灰色の箱(等幅)に入れる
- ② 転送を 11-37 と同じ形にする(割合付きの進捗、アイコン付きのボタン、見出し付きの枠の入力欄)
- ③ ログのカードを残りの高さいっぱいに広げる(11-39 のチャットと同じ組み方)
- 実機の見た目は、SSH を設定した端末で確かめる

## 🖼️View

### 🛠️11-47 Custom(🟢📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/View/ViewCustomView.xaml` + `ViewCustomViewModel.cs`(部品は `Controls/TreeView.cs` / `ColorPicker.cs`) | 自作の部品(MarqueeLabel / TreeView / ColorPicker / DurationPicker / AvatarGroup)の一覧 |

今: TreeView の開閉の印(▸)が大きさ 14 の薄い灰色で見えにくく、フォルダーとファイルの区別も無い。

- ① TreeView の印を MaterialIcons の Chevron(20・濃い灰)にし、行の頭にフォルダー 📁 とファイル 📄 の絵文字を付ける
- 見本のデータは 10-23

### 🖐️11-53 Drag & Drop(⚖️📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/View/ViewDragDropView.xaml` + `ViewDragDropViewModel.cs` | 長押しのドラッグによる並べ替え・リスト間の移動・ゴミ箱 |

今: TODO / DONE の列の中のカードは角丸のカード(並べ替えの一覧はフラットな行)。

- ④ ⚖️ TODO / DONE の列の中のカードは、かんばんのカードとして角丸のまま残す(案)

## 🧪Sample

### 🕸️11-59 Web Basic(🔴📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Sample/SampleWebBasicView.xaml` + `.xaml.cs` / `SampleWebBasicViewModel.cs`(ページは `Resources/Raw/web-basic/`) | HybridWebView と C# の相互の呼び出し |

今: 中のページがブラウザの既定の見た目のまま(灰色の小さなボタン、「Log:」と小さな入力欄)で、下が大きく空く。受信のハイライトは code-behind で行っている。

- ~~① `index.html` / `other.html` に CSS を足す。余白 16・文字 16、ボタンは幅いっぱい・高さ 44・角 8 で `BasicOutlinedButton` と同じ色の枠、ログは等幅の角丸の枠にして下まで広げる~~
- ~~③ 受信のハイライトを、code-behind から `AnimationOption.HighlightTrigger` / `HighlightColor` へ移す~~

結果(👀 確認待ち): ① 2 つのページで同じ `style.css`(新規)を読む。地はアプリと同じ #FAFAFA、余白 16・文字 16、ボタンは幅いっぱいで縦に並べ、高さ 44・角 8・白地に灰の枠と `BlueGrayDarken2` の文字、ログは白地の角丸の枠(等幅 14)で下まで広げる。リンクは青。③ 下の帯の光は `AnimationOption.HighlightTrigger` で、code-behind の処理は削除(最初の Initialized では光らない)。文字は今の英語のまま

### 👁️11-65 CV Local(🟡📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Sample/SampleCvLocalView.xaml` + `SampleCvLocalViewModel.cs` | 端末の中(ONNX)での物体検出 |

今: Detect を押してから結果が出るまでの表示と、撮影の瞬間の表示が無い。

- ① 推論中は `BasicLightLoadingIndicator` を出す(VM の IsProcessing)。撮影の瞬間は白いフラッシュを出す(CV Net の `ShutterFlashBoxView` と `FlashTrigger`)

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
