# 🎨UI のブラッシュアップの計画

UI のブラッシュアップの残作業はこの計画で管理する(`Task_Checklist.md` には載せない)。ここが終わってから `Task_Checklist.md` の項目に戻る。済んだ項目はこの計画から消し、内容は `Change_Summary.md` に書く。

| 部分 | 番号 | 内容 |
| --- | --- | --- |
| 🖼️UI 1 / UI 2 と View の部品の画面 | 10-x | UI 1 / UI 2 の画面を、App のミニアプリと同じ水準(画面の中で完結する操作、下に余白を残さない配置、色と描画で華やかに、一覧で選んだ項目が詳細に出る)へ寄せる。View の部品の画面(Toolkit / Custom / Chart / Sf Chart / Bottom Sheet / Drawer)でしか使っていない部品は、使いどころのある UI の画面へ取り込む |
| ✨既存の画面の細部 | 11-x | Main / Basic / Navigation / Device / Network / View / Sample の画面を、表示する項目は変えずに、細部(色・形・強弱・余白・状態の見せ方)を App の画面と同じ水準へ寄せる。📝 各項目は案のままで、実施の前に内容を確認する |  |

ファイルパスは `Template.MobileApp/` からの相対。各項目の ①〜 は段階または修正の候補。

## 🔖凡例

| 列 | 絵文字 |
| --- | --- |
| 優先 | 🔴 先に行う(不具合・機能の欠け・見劣りが大きい)/ 🟡 作り込み / 🟢 小さな手直し・追加 / ⚖️ 実施の前に決めること |
| 変更量 | 📦📦📦 大(新しい画面・部品・モデル、作り直し、複数の画面や共有のスタイル)/ 📦📦 中(1 画面の組み直し、ViewModel・コンバーター・アイコンの追加も)/ 📦 小(1 画面の XAML・スタイルの手直し) |
| 状態 | 📝 案のまま(内容は未確認。実施の前に確認する)/ 👀 実施済み・確認待ち(①👀 はその候補だけ)/ 空欄 = 実施待ち |

## 📋一覧

### 🖼️UI 1 / UI 2 と View の部品の画面

| 番号 | 対象 | 優先 | 変更量 | 状態 | 内容 |
| --- | --- | --- | --- | --- | --- |
| 10-0 | 共通 | ⚖️ | — |  | 共通の方針(一覧と詳細の受け渡し、規則の置き場所、余白、操作のボタン、行の形、見本のデータ) |
| 10-1 | UI 1 Profile | 🟢 | 📦📦 |  | 下の段のタブ、写真の全画面の表示、フォローで数が変わる |
| 10-3 | UI 1 Money | 🟡 | 📦📦📦 |  | 最近の取引、下のタブで中身を切り替え、支払いのシート(バーコード・QR) |
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
| 10-22 | View Chart | 🟡 | 📦📦📦 |  | 自作のグラフの軸・凡例・値の表示・動き、UI で使える形に |
| 10-23 | View Custom | 🟢 | 📦 |  | TreeView の見本のデータ |
| 10-26 | UI 2 Flight | 🔴 | 📦📦📦 | 👀 | 同じ内容のまま、空と地面のある姿勢の表示・目標の向きのあるレーダー・機体の上面図の兵装の画面に作り直す |
| 10-27 | UI 2 Tactical | 🔴 | 📦📦📦 | 👀 | 同じ内容のまま、陰影のある地形図・脅威の範囲と一覧・機体の損傷の図のある画面に作り直す |
| 10-28 | UI 2 Telemetry | 🟡 | 📦📦 | 👀 | 下の 6 項目を横長に、回転計・ブースト・ギアの行を大きく |

### ✨既存の画面の細部

📝 11-x の項目は案のまま(まだ確認していない)。番号ごとに、実施の前に内容を確認し、修正の前後を見て確認する。

| 番号 | 対象 | 優先 | 変更量 | 状態 | 内容 |
| --- | --- | --- | --- | --- | --- |
| 11-0 | 共通 | — | — | 📝 | 共通の方針(スペース、状態の見せ方、ボタン、空・読み込み中、名前と値の行、配置)。決めたことは決定事項の表 |
| 11-3 | Main Data | 🟡 | 📦📦 | 📝 | ボタンのアイコンと削除の赤、件数のカードを残りの高さに |
| 11-4 | Main Diagnostics | 🟡 | 📦📦 | 📝 | 名前と値の行を詰める、ID・パスは等幅、状態はバッジ |
| 11-5 | Main Setting | 🟡 | 📦 | 📝 | 行の高さをそろえる、文字 13 → 14 |
| 11-6 | Basic Typography | 🟢 | 📦 | 📝 | 色の帯の見本の内側の余白 |
| 11-7 | Basic Style | 🟡 | 📦 | 📝 | 選択の枠と文字、Action 系のボタンの形 |
| 11-8 | Basic Font | 🟢 | 📦 | 📝 | 見本の文字の色、Default の見本の折り返し |
| 11-9 | Basic Converter | 🟢 | 📦 | 📝 | 値の色をパレットに、入力と結果の区切り |
| 11-10 | Basic Locale | 🟢 | 📦 | 📝 | Current culture の書き方をそろえる、キーの塊の区切り |
| 11-11 | Basic Dialog | 🟢 | 📦 | 📝 | ボタンのアイコン、Information のボタンを全幅、見出しの色 |
| 11-12 | Basic Validation | 🟢 | 📦 | 📝 | 欄名と説明を分ける、エラーの間は枠を赤 |
| 11-13 | Basic Setting | 🟡 | 📦📦 | 📝 | SearchBar の枠、日付・時刻の欄、Stepper とボタンの高さ、Summary |
| 11-14 | Navigation Edit | 🔴 | 📦📦 | 📝 | 行をフラットに、編集・削除のアイコンのボタン、番号のバッジ、選んだ行 |
| 11-15 | Navigation Wizard / Shared | 🟢 | 📦 | 📝 | プレースホルダーを入力例に、Shared2 の番号の丸の色 |
| 11-16 | Navigation Dialog(数値入力) | 🟡 | 📦📦 | 📝 | 角丸、数字の帯、✓ と ✕ の区別、AC / C の色 |
| 11-17 | Device Info | 🟢 | 📦📦 | 📝 | 機種名の見出し、名前と値の強弱、カードごとの色 |
| 11-18 | Device Status | 🟡 | 📦📦 | 📝 | 値を 1 回だけ状態のチップで、列挙の名前を短い言葉に、電池の輪 |
| 11-19 | Device Sensor | 🟢 | 📦📦 | 📝 | 中央から左右に伸びるバー |
| 11-20 | Device Location | 🟢 | 📦 | 📝 | 時刻の行の高さ、緯度・経度を 2 列、Motion をタイルに |
| 11-21 | Device QR Display | 🟢 | 📦 | 📝 | QR を大きく中央に、二重の枠、下の文字を 1 行 |
| 11-22 | Device QR Scan | 🔴 | 📦📦 | 📝 | 結果の帯、状態のチップ、プレビューに重ねた丸いボタン、読み取りの枠 |
| 11-23 | Device Camera | 🔴 | 📦📦 | 📝 | 黒地と状態のチップ、プレビューに重ねた丸いボタン |
| 11-25 | Device WiFi | 🟢 | 📦 | 📝 | 文字の大きさを許可値に |
| 11-26 | Device Bluetooth | 🟡 | 📦📦 | 📝 | 状態に合わせた色、段階の表示、Print を下に |
| 11-27 | Device BLE Scan | 🟢 | 📦 | 📝 | 7 セグの消えたセグメント、灰色の地 |
| 11-28 | Device BLE Host | 🟡 | 📦📦 | 📝 | UserId を 1 行、広告中の色と動き |
| 11-29 | Device NFC | 🔴 | 📦📦 | 📝 | 読み取る前の空の箱、IC カード風のカード、履歴をフラットな行に |
| 11-30 | Device Audio | 🟡 | 📦📦 | 📝 | ジャケット風の絵、音量のアイコン |
| 11-31 | Device Activity | 🟡 | 📦📦 | 📝 | 歩数の強調と輪のグラデーション、3 つのタイル、活動時間の書式 |
| 11-32 | Device Biometric | 🟡 | 📦📦 | 📝 | 状態の色のバッジ、押せないボタンのアイコン、「—」の色 |
| 11-33 | Device Communication | 🟡 | 📦📦 | 📝 | 連絡先の画面の形、外のアプリを開く印 |
| 11-34 | Device Misc | 🟢 | 📦 | 📝 | 対の操作をセグメントに、見出しの色 |
| 11-35 | Network HTTP (Data) | 🟡 | 📦📦 | 📝 | 空・読み込み中・失敗の表示、選んだ行、主な操作のボタン |
| 11-36 | Network HTTP (Auth) | 🟡 | 📦📦 | 📝 | ログインの状態の表示、ボタンの強弱、入力欄の枠 |
| 11-37 | Network Storage | 🟡 | 📦📦 | 📝 | 空の表示、パスと「上へ」、転送の進捗とボタン |
| 11-38 | Network Realtime | 🟡 | 📦📦 | 📝 | グラフの枠、状態のチップ、通知の空の文 |
| 11-39 | Network gRPC | 🟡 | 📦📦 | 📝 | 接続の状態、チャットの空の表示と行、入力欄と送信 |
| 11-40 | Network SFTP | 🟢 | 📦📦 | 📝 | 接続先と指紋、転送を Storage と同じ形、ログを下まで |
| 11-41 | Network Telemetry | 🟢 | 📦 | 📝 | アイコンの色、Custom value の行 |
| 11-42 | View Layout | 🟢 | 📦 | 📝 | セルの色分け、左右の端 |
| 11-43 | View State | 🟢 | 📦 | 📝 | 状態の切り替えのボタン、状態の表示を中央に |
| 11-44 | View Border | 🔴 | 📦📦 | 📝 | InfoCard の形、色の丸 |
| 11-45 | View Shadow | 🔴 | 📦📦 | 📝 | ニューモーフィズムの影が切れる、InfoCard の形、色の丸 |
| 11-46 | View Toolkit | 🟢 | 📦 | 📝 | SegmentedControl の色、OtpInput の位置、Expander の印 |
| 11-47 | View Custom | 🟢 | 📦📦 | 📝 | TreeView の印とアイコン、ColorPicker のスライダーの色 |
| 11-48 | View Bottom Sheet | 🟢 | 📦 | 📝 | シートの行のアイコンと区切り、結果の表示 |
| 11-49 | View Drawer | 🟢 | 📦 | 📝 | SegmentedControl の色、選んでいる行、選択中の表示 |
| 11-50 | View Animation | 🟡 | 📦📦 | 📝 | タイルを 2 × 2 に、効果のアイコン、ボタンの形 |
| 11-51 | View Easing | 🟡 | 📦📦 | 📝 | 丸を曲線に沿って動かす、曲線の色 |
| 11-52 | View Effect | 🟢 | 📦 | 📝 | Replay の位置、増減のアイコン |
| 11-53 | View Drag & Drop | 🟡 | 📦📦 | 📝 | 並べ替えの行をフラットに、取っ手、TODO / DONE のバッジ |
| 11-54 | View Lottie | 🟡 | 📦 | 📝 | 開いた直後の絵、帯の文字の切れ、端をそろえる |
| 11-55 | View Graphics | 🟢 | 📦 | 📝 | 図形の色、ボタンのアイコン |
| 11-56 | View Drawing | 🟡 | 📦 | 📝 | 案内の色 |
| 11-57 | View Chart | 🟡 | 📦 | 📝 | グラフの枠、チップのアイコン |
| 11-58 | View Sf Chart | 🟢 | 📦 | 📝 | ドーナツの色と凡例、ラベル、格子線と棒の角 |
| 11-59 | Sample Web Basic | 🔴 | 📦📦 | 📝 | 中のページの見た目、状態の帯の文字の大きさ |
| 11-60 | Sample Map | 🟢 | 📦 | 📝 | 切り替えのボタンに状態 |
| 11-61 | Sample Map2 | 🔴 | 📦 | 👀 | パネルがズームのボタンを隠す、パネルの行、右下のボタン |
| 11-62 | Sample Media | 🟢 | 📦 | 📝 | 開いた直後の操作のバー、失敗の表示 |
| 11-63 | Sample Markdown | 🔴 | 📦 | 👀 | 左右の余白、見出しの強弱、本文の色 |
| 11-64 | Sample PDF | 🟢 | 📦 | 📝 | F キーの表示、スライダーとページ数 |
| 11-65 | Sample CV Local | 🟡 | 📦 | 📝 | 推論中の表示と撮影のフラッシュ、結果の後の案内 |
| 11-66 | Sample Crop | 🟢 | 📦 | 📝 | プレビューの空の表示、ボタンの高さ |
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

### ✨既存の画面の細部

| 項目 | 決定 |
| --- | --- |
| 言葉 | 英語と日本語の混在はそのまま(そろえない) |
| スペース | 日本語と英語・数字の間、括弧の前には半角スペースを入れない。英単語を複数並べるときは半角スペースで区切る。そろえるのは対象の画面の文字だけ(11-67) |
| F キーだけの操作 | そのまま(画面の中にボタンを足さない) |
| メニュー | 大きさ(文字・すき間)と色は変えない。角丸は使わない |
| 11-61 Map2 | 6 つの切り替えは地図の上に出し続けない(場所をとりすぎる)。下から出るシート(自作の `BottomSheetView`、開いている間は地図が暗くなる)に入れ、右下のボタンの列の一番上のボタンで開く |
| 進め方 | 番号ごとに、修正の前後を確認しながら進める。指示に無い点は、決める前に確認する |

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
| 地の色 | 白地の画面(View Border・Shadow・Chart、Device BLE Scan)は、ほかと同じ灰色の地に白い枠の形にする |
| カメラの画面 | Camera・QR Scan は、OCR と同じく黒地にし、プレビューに重ねた丸いボタンと状態のチップでそろえる |
| 一覧の行 | 角丸のカードの行(Navigation Edit・View Drag & Drop・Device NFC の履歴)は、フラットな行と区切り線にする |
| 決まりに合わない値 | 許可値以外の文字の大きさ(13・15・56)と押す部品の高さ(32・36・40)は、その画面を直すときに許可値へ寄せる。要素に直接書いた見た目の属性(見本の内容のものを除く)も画面ローカルのスタイルへ移す |

## 🐞不具合(既存の画面)

見た目の候補とは別に、先に直せる。

| 画面 | 内容 | 候補 | 状態 |
| --- | --- | --- | --- |
| Device NFC | 読み取る前も、上に空の緑の箱と「¥ 0」が出る | 11-29 ① |  |
| Device Biometric | 押せないボタンのアイコンが濃いまま | 11-32 ② |  |
| Network Realtime | 通知の空の文(前面ならトースト)が、今の動き(いつもローカル通知)と違う | 11-38 ③ |  |
| View Shadow | ニューモーフィズムの影が、タイルを並べた枠の端で切れる | 11-45 ② |  |
| View Easing | 丸が、描いた曲線と違う向き・道筋で動く | 11-51 ① |  |
| View Lottie | スクロール連動の帯の文字が右端で切れる | 11-54 ② |  |
| Sample Map2 | 切り替えのパネルが Mapsui のズームのボタンを隠す | 11-61 ① | 👀 |

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

- ① 見出し: モードのチップ、システムの状態、燃料の残りの棒、G と推力
- ② 姿勢の表示(PFD): 機体の傾きとピッチで回る空と地面、ロールの目盛りの内側だけのピッチの目盛り、上の方位の帯と読み取りの枠、速度と高度の帯と内側を指す読み取りの枠、6 秒後の速度の見込みの矢印、機首の印と飛んでいく向きの印
- ③ レーダー: 距離の輪と方位の目盛り、走査の帯、目標の印と 10 秒後の位置への線、選んだ目標のロックの枠と方位・距離
- ④ 兵装: 機体の上面図に 6 本のミサイル(次に撃つものを点滅の枠)、機銃の残弾の棒、FOX 2 の帯、MASTER ARM のチップ
- ⑤ 下の状態の行はチップに。変わらない部分は画像にして写すだけにする(描画は 1 フレーム平均 13〜14 ms)

実施(2026-10-04)、確認待ち。結果は `Change_Summary.md` の区間 17。

### 🗺️10-27 Tactical(🔴📦📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UITacticalView.xaml` + `UITacticalViewModel.cs` | 画面(全面の `SceneControl`) |
| `Graphics/Scene/MechHudScene.cs` | 地上部隊の戦術の画面の描画 |

今: 黒地に緑の線だけの画面。通信の枠の SIG の強さの記号がフォントに無く、四角(豆腐)で出る。小隊 D2 / D3 の名前が動く隊員の点と重なる。文字が小さい(8〜10)。

表示する内容(部隊・モード・経過時間・時計・データリンク・損傷、通信のチャンネル・暗号鍵・波形・送受信・信号、戦術の地図の地形・格子・味方・敵・目標・自機、状態の行)はそのままに、デザインを作り直す。

- ① 見出し: 部隊の記章、モードのチップ、データリンクの点滅、損傷の状態のチップ
- ② 通信: 選んだチャンネルの帯、暗号鍵のチップ、オシロスコープの波形(送信中は大きく揺れる)、信号の強さの棒
- ③ 機体: 部位ごとの損傷を色で示す機体の図(いちばん傷んだ部位の枠が点滅)、全体の耐久と最も傷んだ部位。値はモデルの部位の耐久をそのまま使う
- ④ 地図: 標高を色と陰影で塗った地形と等高線、格子、縮尺と北、センサーの走査、自機の視界の扇、敵の足跡と脅威の範囲、目標の輪と経路と距離・方位、近い順の脅威の一覧(右下に半透明)
- ⑤ 下の状態の行はチップに。変わらない部分は画像にして写すだけにする(描画は 1 フレーム平均 12〜13 ms)

実施(2026-10-04)、確認待ち。結果は `Change_Summary.md` の区間 17。

### 🏎️10-28 Telemetry(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/UI/UITelemetryView.xaml` + `UITelemetryViewModel.cs` | 画面(F2 で描き方の切り替え) |
| `Graphics/Scene/TelemetryScene.cs` | 車のテレメトリの描画 |

今: 下の 6 項目が正方形の枠で、半円の計器の上に空きがある。

- ① 下の 6 項目を横長の枠にし、半円の計器を枠の下寄りに置く
- ② 空いた高さを、回転計とブースト(半径 60 → 70)、ギアの行に回す
- ③ ギアの行は、ギア(幅 112・高さ 150)を ERS と G-FORCE(半径 44)より大きくし、ギアの枠には角のラインを付けない(下の 6 項目の枠には残す)
- 表示項目は変えない

実施(2026-10-04)、確認待ち。結果は `Change_Summary.md` の区間 17。

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
- UI 1 / UI 2 / App の画面とドキュメントは対象外

## 🏠Main

### 🗄️11-3 Data(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Data/DataView.xaml` + `DataViewModel.cs` | SQLite の CRUD と一括の処理の見本 |

今: 文字だけの同じ枠のボタンが並び、削除の Delete / DeleteAll もほかと同じ見た目。件数は JetBrainsMono の中に点のある 0 で、記号のように見える。画面の下の 4 割が空く。

- ① ボタンにアイコンを付ける(`BasicIconOutlinedButton` と `AppIcons` の Small 系。無いものは足す)
- ② Delete / DeleteAll は赤の枠のボタン(`BasicOutlinedCancelButton`)にする
- ③ Bulk のカードを残りの高さいっぱいに広げ、件数を中央に既定のフォントの太字で大きく置き、ボタンをカードの下端にそろえる

### 🩺11-4 Diagnostics(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Main/DiagnosticsView.xaml` + `DiagnosticsViewModel.cs` | 端末・起動・DB・通信・テレメトリ・ログの診断 |

今: 名前 18 の太字と値 18 の 2 列のカードが 8 枚続いて縦に長く、Path だけ 12 で大きさがそろわない。状態(Sending / Not sent / Failed)は色の付いた文字だけで、目に入りにくい。

- ① 名前と値の行を 11-0 の形(16・1px の区切り線)にして縦を詰める
- ② ID・Path・日時は JetBrainsMono の 14 にそろえる
- ③ Status / Last send / Crash は白文字の色付きのバッジにする
- ④ Delete files / Clear は赤の枠のボタンにする

### ⚙️11-5 Setting(🟡📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Main/SettingView.xaml` + `SettingViewModel.cs` | 設定の QR の読み取りと、今の設定値の表示 |

今: 文字の行(高さ約 25)に対してスイッチの行(約 50)が倍の高さで、Telemetry / Push の所だけ間延びする。文字 13 は許可値の外。

- ① Telemetry と Push を 1 行に左右で並べるか、全部の行の最小の高さを 40 にして間隔をそろえる
- ③ 13 の 3 か所(`SettingCaptionLabel` / `SettingValueLabel` / `SettingEmptyLabel`)を 14 にする
- ④ 節の見出しのアイコンに節ごとの色を付ける

## 🧱Basic

### 🔤11-6 Typography(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Basic/BasicTypographyView.xaml` + `BasicTypographyViewModel.cs` | 共有の文字のスタイルと Label のクラスの見本 |

今: 色の帯の見本に内側の余白が無く、左寄せ・右寄せの文字が帯の端に接して窮屈に見える。

- ① 見本の Label に左右 8 の Padding を足す画面ローカルのスタイルを作り、StyleClass と併せて付ける(共有のクラスは変えない)

### 🖌️11-7 Style(🟡📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Basic/BasicStyleView.xaml` + `BasicStyleViewModel.cs` | 共有のボタンのスタイルと、選択のボタンの見本 |

今: Action / Information のボタンは角 0・文字 24 の板で、ほかの画面の角丸 8・文字 14 のボタンと比べて古い。選択のボタンは黒字で、角の無い表のように見える。

- ② 選択の 3 つを角丸 8 の枠でくるみ、文字を 14 の BlueGray にする(画面ローカルのスタイル)
- ③ ⚖️ Action 系の共有のスタイル(`ActionButtonBase` と Primary〜Error。`BasicFilledButton` / `BasicFilledSecondaryButton` の基底でもある)を角丸 8・文字 18 にするか

### 🅰️11-8 Font(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Basic/BasicFontView.xaml` + `BasicFontViewModel.cs` | 同梱のフォントとアイコンのフォントの見本 |

今: 見本の文字が既定の灰色(SecondaryTextColor)で、字形が薄く見える。Default の 24 の行は最後の「歴」だけが次の行に落ちる。

- ① 見本の行に PrimaryTextColor の画面ローカルのスタイルを付ける(FontFamily / FontSize は見本の内容として属性のまま)
- ② Default の見本の文字列を、24 でも 1 行に収まる長さにする

### 🔁11-9 Converter(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Basic/BasicConverterView.xaml` + `BasicConverterViewModel.cs` | Bool / Multi / Text のコンバーターの見本 |

今: BoolTo / EmptyTo の値だけ素の Black と Red で、ほかの灰色の値やパレットの赤から浮く。同じカードにチェックボックスの名前 14 と名前 18 の太字が混ざり、入力と結果の境目が分かりにくい。

- ① 色をパレットの値にする(True は `RedDefault`、False は `PrimaryTextColor` など)
- ② チェックボックスの行と結果の行の間に `CardDivider` を入れる
- ③ 文字の大きさを 11-0 の名前と値の行でそろえる

### 🌐11-10 Locale(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Basic/BasicLocaleView.xaml` + `BasicLocaleViewModel.cs` | カルチャとローカライズのリソースの見本 |

今: Current culture のカードだけ名前 18 の太字と値 18 で、下の 2 枚(等幅の小さいタグと値)と書き方がそろわない。Localized resources は 2 つ目のキーの前の間隔が行の間隔と同じで、塊の区切りが分かりにくい。

- ① Current culture も `ResourceTagLabel` + `CardPrimaryValueLabel` の形にする
- ② キーの塊の間に `CardDivider` か 16 の間隔を入れる

### 💬11-11 Dialog(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Basic/BasicDialogView.xaml` + `BasicDialogViewModel.cs` | 通知・確認・進捗・フィードバックのダイアログの見本 |

今: 文字だけの枠のボタンが 4 枚のカードに並ぶだけで単調。Notification のカードは Information のボタン 1 つが左半分にだけあり、右半分が空いて見える。

- ① 各ボタンにアイコンを付ける(Validation の Error / Clear と同じ形。無い Small のアイコンは `AppIcons` に足す)
- ② Information のボタンを全幅にする
- ③ 見出しのアイコンに種類ごとの色を付ける(11-0)

### ☑️11-12 Validation(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Basic/BasicValidationView.xaml` + `BasicValidationViewModel.cs` | 入力の検証とエラーの表示の見本 |

今: 長い説明がそのまま欄名になっていて(Confirm は 2 行)、欄名と説明の区別が無い。エラーは赤い文と、枠の背景の一瞬の点滅だけで、エラーの間も枠は普段の灰色のまま。

- ① 欄名は短く(Text1 / Confirm など)し、説明は欄の下に 12 の注記(`BasicCaptionLabel`)で分ける
- ② エラーがある間は枠を赤(`RedDefault`)にする

### 🎚️11-13 Setting(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Basic/BasicSettingView.xaml` + `BasicSettingViewModel.cs` | 設定の画面でよく使う入力の部品の見本 |

今: SearchBar だけ枠が無く大きく字下げされ、上の Entry の枠と形がそろわない。Stepper の - / + は Android の標準の灰色の四角で浮いて見える。日付・時刻は未設定だと短い下線だけが出て崩れて見え、開く / クリアのボタンは高さ 32 で押しにくい。

- ① SearchBar も `CardFieldBorder` に入れて下線を消す(`EntryOption.NoBorder` の対象に SearchBar を足す)
- ② 日付・時刻のピッカーを幅をそろえた枠の欄に入れ、未設定でも空の欄に見せる
- ③ 開く / クリアは高さ 44 にし、Stepper のボタンは白地・枠・角丸に寄せる(ハンドラーでの見た目の調整が要る)
- ④ Summary は名前と値の 2 列にして値を太字にする

## 🧭Navigation

### ✏️11-14 Edit(🔴📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Navigation/Edit/EditListView.xaml` + `EditListViewModel.cs` | 一覧(追加・編集・削除) |
| `Modules/Navigation/Edit/EditDetailView.xaml` + `EditDetailViewModel.cs` | 編集の画面 |

今: 行が角丸 10 の枠のカードで、一覧はフラットな行にする決まりと違う。編集・削除は蛍光の緑と赤の四角(約 26)で重く、押しにくい。番号のバッジ(#1 など)に色が無い。

- ① 行をフラット(余白 0・枠なし)と 1px の区切り線にする(UI Visit の `RowSeparator` と同じ)
- ② 編集・削除は透明の 44 のアイコンのボタン(青のペン / 赤のゴミ箱)にする
- ③ 番号は色付きのバッジにし、選んだ行は青地に白文字で示す

### 🧙11-15 Wizard / Shared(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Navigation/Wizard/WizardInput1View.xaml` / `WizardInput2View.xaml` / `WizardResultView.xaml` + 各 ViewModel | 段階を追って入力するウィザード |
| `Modules/Navigation/Shared/SharedInputView.xaml` / `SharedMain1View.xaml` / `SharedMain2View.xaml` + 各 ViewModel | 2 つの画面から共有する入力の画面 |

今: 欄名とプレースホルダーが同じ文言で重なる(「Data1 (required)」と「Data1」、「No」と「No」)。Shared2 から戻った先の SharedMain2View は、番号の丸だけ藍(`IndigoDefault`)で、チップと No の値のティールとそろわない。

- ① プレースホルダーを欄名の繰り返しではなく入力例にする
- ② SharedMain2View の番号の丸(`NumberCircleBorder`)を `TealDefault` にする

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

- ① Device のカードを見出しの形にする(機種名を大きく、OS の版と実機 / エミュレーターをチップで)
- ② 値を濃い色、名前を小さい灰色にして強弱を入れ替える(11-0)
- ③ カードごとにアイコンの色を付け(端末 = 青、アプリ = 緑、画面 = 紫)、パッケージ名は等幅にする

### 🔋11-18 Status(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceStatusView.xaml` + `DeviceStatusViewModel.cs` | 電池と通信の今の状態 |

今: Network はチップの列と下の表が同じ 3 つの値を二重に出し、値は列挙の名前のまま(ConnectedHighSpeed / Usb など)。充電中でもチップは灰色。電池のバーは細い既定の ProgressBar で、Network の見出しのアイコンは塗りの扇形で読み取りにくい。下の 4 割が空く。

- ① 表の値の側を状態の色のチップにして、上のチップの列はやめる(各値を 1 回だけ出す)
- ② 列挙の値を絵文字付きの短い言葉にし(コンバーター。例 ⚡ 充電中 / 🔌 USB / 📶 高速)、充電中は緑にする
- ③ 電池の残量を `ArcMeter` の輪にして % を中に置く(20% 以下は赤)
- ④ Network の見出しのアイコンを線の形(`Wifi` など)にする

### 🧲11-19 Sensor(🟢📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceSensorView.xaml` + `DeviceSensorViewModel.cs` | 加速度・ジャイロ・磁気・姿勢・方位・水準器・気圧 |

今: 軸の色のバッジと等幅の値で整っているが、符号付きの値のバーが左端から伸びるので、0 でも中央まで塗られ、負の値は短い棒に見える(バーは細い既定の ProgressBar)。

- ① 中央の目盛りから左右に伸びるバーにする(`RangeBar`。Low と High に、0 と値の小さい方・大きい方)
- ② バーを太さ 8 前後の角丸にし、溝を軸の色の淡い色にする

### 📍11-20 Location(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceLocationView.xaml` + `DeviceLocationViewModel.cs` | 地図と現在地の座標・高度・速度など |

今: 時刻の行で、時計のアイコン(縦の中央)と文字(上寄せ)の高さがずれる。緯度・経度は縦に積んで長く、Motion だけ名前と値の表で書式が違う。

- ① 時刻の文字も縦の中央にそろえる(画面ローカルのスタイル)
- ② 緯度・経度を 2 列に並べる
- ③ Motion の 4 項目も 2 × 2 のタイル(見出しの下に値、単位は小さく)にし、Position と書式をそろえる

### 🔳11-21 QR Display(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceQrDisplayView.xaml` + `DeviceQrDisplayViewModel.cs` | 入力した文字の QR コード |

今: 中央のカードの中に QR の枠が入れ子になっていて線が二重。QR は 240 と小さめで、カードの上下に大きな空きがある。QR の下の文字は入力欄と同じ値で、長い文字だと折り返してカードが伸びる。

- ① QR を 280〜300 に大きくして空きの真ん中に置き、入力欄は画面の下にそろえる
- ② 内側の QR の枠線をやめて白い余白だけにする
- ③ QR の下の文字は 1 行の省略にする

### 🔍11-22 QR Scan(🔴📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceQrScanView.xaml` + `DeviceQrScanViewModel.cs` | カメラでのバーコード・QR の読み取り |

今: プレビューの下の結果と状態の文字が左端に付いて余白が無く、「Invert: False」「Zoom: x-1」と生の値が出る。Torch / Aim / ZoomOut / ZoomIn は水色の文字のボタンの帯(`SubMenuButton`。ZoomOut は文字が枠いっぱい)で、F キーと合わせて原色の帯が 2 段になる。

- ① 結果を余白のある帯にする(QR のアイコンと値、読み取る前は案内)
- ② 状態を ON / OFF のチップにする(ON は色付き、ズームは値が無ければ「-」)
- ③ Torch / Aim / Zoom をプレビューに重ねた丸いアイコンのボタン(半透明の黒地)にして、水色の帯をやめる
- ④ プレビューの中央に読み取りの枠(四隅のかぎ)を置く

### 📸11-23 Camera(🔴📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceCameraView.xaml` + `DeviceCameraViewModel.cs` | カメラのプレビューと撮影 |

今: プレビューの上下の黒い帯の下に白い状態の帯があって境目がきつく、状態は左端に付いた「Torch: False」などの文字。Torch / Flash / ZoomOut / ZoomIn は水色の文字のボタンの帯(`SubMenuButton`)。

- ① 画面を黒地(`DarkRootGrid`)にし、状態はプレビューの上の半透明のチップにする(🔦 OFF / ⚡ Off / 🔍 x1)
- ② Torch / Flash / Zoom を、プレビューに重ねた丸いアイコンのボタン(半透明の黒地)にして、水色の帯をやめる(切り替え・撮影は F キーのまま)
- 11-22 と 11-23 で水色の帯をやめると、`SubMenuButton` を使う画面が無くなる(削除する)

### 📶11-25 WiFi(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceWiFiView.xaml` + `DeviceWiFiViewModel.cs` | 接続中の Wi-Fi と周りのアクセスポイント |

今: フラットな行・色付きのバッジ・絵文字で整っている。文字の大きさに許可値以外が 2 つ残る(情報の行が 13、接続中の電波のアイコンが 56)。

- ① 13 → 14、56 → 48 にそろえる

### 🖨️11-26 Bluetooth(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceBluetoothView.xaml` + `DeviceBluetoothViewModel.cs` | Bluetooth のシリアルでの試し印刷 |

今: 中央のカードだけで、上下が 3 割ずつ空く。アイコンの円は状態によらず青で、状態のチップは列挙の名前(Idle)。Print は幅 200 の中央のボタン。

- ① アイコンの円の色を状態に合わせる(接続中は琥珀、完了は緑、失敗は赤)
- ② 段階(接続 → 送信 → 完了)を `StepIndicator` で見せる
- ③ アイコンと状態を空きの真ん中に置き、Print は画面の下の幅いっぱいのボタン(高さ 48)にする

### 🌡️11-27 BLE Scan(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceBleScanView.xaml` + `DeviceBleScanViewModel.cs` | SwitchBot の温湿度計・CO2 の値の受信 |

今: 液晶を真似たパネルで個性はあるが、背景がほかの Device の画面の灰色ではなく白。7 セグの数字に消えたセグメントが無く、時刻は 1 の字の幅で「15:3 1:05」と間が空いて見える。

- ① Timer と同じく、消えたセグメント(8)を薄く重ねる(1 の字の空きも液晶らしく見える)
- ② 背景を `RootGrid` と同じ灰色にし、パネルの間を 12 にする
- ③ ⚖️ 行の角丸は、液晶のパネルの見立てとして残す(案)

### 📡11-28 BLE Host(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceBleHostView.xaml` + `DeviceBleHostViewModel.cs` | BLE のペリフェラル(広告)の開始と停止 |

今: 中央のカードだけで上下が空き、UserId(36 文字の GUID)が決まっていない位置で 2 行に折れる。円は広告中も停止中も青。

- ① UserId を等幅の 14 にして 1 行に収める
- ② 広告中は円を緑にして、電波の輪が広がる動きにする

### 💳11-29 NFC(🔴📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceNfcView.xaml` + `DeviceNfcViewModel.cs` | Suica の残高と利用履歴の読み取り |

今: Metro 風の濃い灰色と原色の箱で古く見え、読み取る前も上に空の緑の箱と「¥ 0」が出る。履歴は 1 件ごとに日時 3 段・処理・残高の大きな箱で、1 画面に 7 件ほどしか入らない。

- ① 読み取る前は上の帯を隠し、読み取った後は IC カード風のカードにする(グラデーション、IDm は等幅、残高は大きくカウントアップ)
- ② 履歴をフラットな行にする(左に処理の色の細い帯、日時は「05/12 11:24」の 1 行、端末と処理は絵文字付きのバッジ、残高は右寄せ)

### 🎵11-30 Audio(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceAudioView.xaml` + `DeviceAudioViewModel.cs` | 音声ファイルの再生(シーク・音量) |

今: 中央のカードの再生の画面で、操作はそろっている。上下に空きがあり、音符の円は再生中も止まっているときと同じ。

- ② 音符の円を大きなグラデーションの四角(ジャケットの見立て)にして空きの真ん中に置き、シークと操作の行は下にそろえる
- ③ 音量のアイコンを値に合わせて切り替える(0 は `Volume_off`、小さいときは `Volume_down`)

### 🚶11-31 Activity(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceActivityView.xaml` + `DeviceActivityViewModel.cs` | 歩数の輪と、消費カロリー・距離・活動時間 |

今: 輪と歩数で主役ははっきりしているが、歩数は細い灰色の文字で、下の表は名前・値・単位がすべて 24 で同じ強さ。⏱ だけ白黒の絵文字(異体字セレクタなし)で、活動時間は「0.00 時間」。下の 4 分の 1 が空く。

- ① 歩数を太字の濃い色にし、輪の進みを Timer の文字盤のようなグラデーションにする
- ② 下の 3 項目を横並びの 3 つのタイル(絵文字、太字の大きな値、小さな単位)にし、活動時間は「0:00」(時:分)にする
- ③ ⏱ を色の付く「⏱️」にする

### 👆11-32 Biometric(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Device/DeviceBiometricView.xaml` + `DeviceBiometricViewModel.cs` | 生体認証の可否・本人確認・鍵での署名と検証 |

今: 状態の値がすべて青の文字なので、使える / 使えないが色で分からず、3 列のタイルでは「一時的に使えない」が途中で折り返す。押せないボタンは文字だけ薄くなり、アイコンは濃いまま。値の無い「—」は青と灰色が混ざる。

- ① 状態の値を状態の色のバッジにし(使える = 緑、未登録 = 琥珀、一時的に使えない = 灰、センサー無し = 赤)、文言は 1 行に収まる長さにする(例 ⏳ 一時不可)
- ② 押せないボタンはアイコンも薄くする(画面ローカルの派生のスタイルで、押せないときの Opacity)
- ③ 「—」を灰色にそろえ、Authenticate の Result も見出しの下に値を置くタイルにする

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

- ① 対の操作を、1 つの枠を 2 つに分けたセグメントの形にする
- ② 節の見出しのアイコンに色を付ける(画面 = 青、振動 = 紫、ライト = 琥珀、音声 = 緑、通知 = 赤)

## 🌐Network

### 🗃️11-35 HTTP (Data)(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Network/NetworkHttpView.xaml` + `NetworkHttpViewModel.cs` | Web API の CRUD・追加読み込み・テスト API(エラー・遅延・キャンセル) |

今: 通信できないと、高さ 240 の空きの上に「データなし (作成で追加)」の小さな文字だけが出て、空と失敗の区別が無い。選んだ行は淡い青で目立たない。ボタンは 9 つとも同じ枠のボタンで、主な操作が分からない。保存のボタンに「(Navigation > Edit で表示)」の括弧書きが付いている。

- ① 一覧の `EmptyView` に `BasicEmptyStack` を置く(Navigation の Edit と同じ形)。読み込み中は `BasicLoadingIndicator`、通信できないときは空の表示を出さない。件数は説明の文から出して「全 N 件」のバッジにする
- ② 選んだ行は青地に白文字にする(UI Visit と同じ)。Id は「#12」の淡色のバッジにする
- ③ 作成は塗りのボタン、ほかのボタンとテスト API の 4 つはアイコン付きの枠のボタン(`BasicIconOutlinedButton`)にする。保存のボタンの括弧書きはボタンの下の説明へ移す

### 🔑11-36 HTTP (Auth)(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Network/NetworkAuthView.xaml` + `NetworkAuthViewModel.cs` | ID だけのログイン(JWT)と、JWT が要る API の呼び出し |

今: 状態とトークンの期限がボタンの下の本文 2 行で目立たない。カード 2 枚で終わり、画面の下の約 4 割が空く。ボタンはすべて同じ枠のボタン。

- ① ログインのカードの先頭に状態の表示を置く(鍵のアイコンの丸。未ログインは `BasicEmptyIconBorder` の灰、ログイン済みは `BasicSuccessIconBorder` の緑。`StatusChip` を並べ、期限は名前と値の行)
- ② ログインは塗りのボタン、Secure を呼ぶはアイコン付きにする。ログアウトと無効化は今の赤の枠のまま
- ③ ログイン ID の入力欄に見出しを付けて、枠付き(`CardFieldBorder` + `BasicFieldEntry`)にする

### 📁11-37 Storage(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Network/NetworkStorageView.xaml` + `NetworkStorageViewModel.cs` | サーバーのストレージの一覧・アップロード・ダウンロード・削除 |

今: 空のときは高さ 280 の空きの上に「空のディレクトリ」の小さな文字だけが出て、通信できないときも同じ。「上へ」は文字だけのボタンで、説明はカードの下にある(HTTP は上)。転送の 5 つのボタンは強弱もアイコンも無く、進捗は細いバーだけ。

- ① 空の表示を 11-35 と同じ形にする
- ② パスの行にフォルダーのアイコンを付け、「上へ」は矢印のアイコン付きにする。ディレクトリの行の右に「›」を付け、サイズは淡色のバッジにする
- ③ 転送の進捗に割合の文字を添えて `AnimationOption.ProgressTo` で伸ばす。5 つのボタンにアイコンを付け、説明は見出しのすぐ下へ移す

### 🔄11-38 Realtime(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Network/NetworkRealtimeView.xaml` + `NetworkRealtimeViewModel.cs` | SignalR の常時接続で、サーバーの状態のグラフと通知を受ける |
| `Controls/StatControl.cs` | グラフ(この画面だけで使う) |

今: グラフは白い枠(余白 8)の中に角の無い色の四角を置いた二重の枠。名前が上端と左端にほぼ接していて、データが無いとただの色の塊に見える。Connections が「0.0」と小数で出る。状態は「状態: 接続中...」の文字で、エラーは赤の小さな文字。通知の空の文に「前面ならトースト」とあり、今の動き(いつもローカル通知)と違う。

- ① グラフを角の丸い色の面にして二重の枠をやめる。名前は 14 の太字で余白 12、Connections は整数にし、データが無くても薄い目盛りの横線を出す
- ② 状態は `StatusChip` にする(緑 = 接続済み / 琥珀 = 接続中・再接続中 / 赤 = 停止・エラー)。接続 ID・サーバー時刻・送信の回数は名前と値の 2 列(`CardInfoGrid`)にし、エラーは淡い赤の帯で出す
- ③ 通知の空の文を今の動きに合わせる

### 🗨️11-39 gRPC(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Network/NetworkGrpcView.xaml` + `NetworkGrpcViewModel.cs` | gRPC の双方向ストリーミングのチャットと単項 RPC |

今: チャットの枠は、空のとき左上に小さな説明の文があるだけの大きな白い箱。入力欄は下線で、送信は小さな枠のボタンなので、上の枠と端がそろわない。サーバー時刻の結果「-」がボタンの横に浮いている。

- ① 接続のカードを 11-38 と同じ形にする(状態のチップ、アドレスは等幅、エラーは淡い赤の帯)。サーバー時刻の結果はボタンの下の名前と値の行に出す
- ② チャットの空の表示を枠の中央(アイコンと文)に置く。メッセージは名前を色付きのバッジ、時刻を右寄せの灰色にして、行の間に区切り線を入れる
- ③ 入力欄を枠付き(`CardFieldBorder` + `BasicFieldEntry`)にし、送信は丸い塗りのアイコンのボタンにして、上の枠と左右の端をそろえる

### 🔐11-40 SFTP(🟢📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Network/NetworkSftpView.xaml` + `NetworkSftpViewModel.cs` | SFTP でのアップロード・ダウンロードと、サーバーのホスト鍵の表示 |

今: 接続先はホストの本文 1 行。転送は下線の入力欄・細い進捗・枠のボタン 3 つで、11-37 と同じく強弱が無い。ログのカードは、転送する前は「まだ転送していません」の 1 行だけ。「サーバ」の表記がほかの画面(サーバー)と違う。

- ① ホストを等幅にしてサーバーのアイコンの行に置き、指紋は淡い灰色の箱(等幅)に入れる。「サーバ」は「サーバー」にそろえる
- ② 転送を 11-37 と同じ形にする(割合付きの進捗、アイコン付きのボタン、見出し付きの枠の入力欄)
- ③ ログのカードを残りの高さいっぱいに広げる(11-39 のチャットと同じ組み方)
- 実機の見た目は、SSH を設定した端末で確かめる

### 📈11-41 Telemetry(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Network/NetworkTelemetryView.xaml` + `NetworkTelemetryViewModel.cs` | テレメトリの送信の試験(ログ・スパン・通信・Flush・クラッシュ)と任意の値 |

今: 整っているが、6 つのボタンのアイコンがすべて同じ灰色で、Warning・Error・Crash の重さの差が見えない。Custom value は細い行の並びで、画面の下の約 3 割が空く。

- ① アイコンに種類の色を付ける(Warning = 琥珀、Error・Crash = 赤。`AppIcons` に色違いを足す)。Crash は文字も赤にする(画面ローカルのスタイル)
- ② Custom value の行を 2 段にする(上に名前と大きな値、下に幅いっぱいのスライダー)。行ごとに色(青・緑・橙・紫)を分ける

## 🖼️View

### 📐11-42 Layout(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/View/ViewLayoutView.xaml` + `ViewLayoutViewModel.cs` | CommunityToolkit と自作のレイアウトの見本 |

今: 上の UniformItems と Circular のセルが水色 1 色で、色分けした下の Staggered / VariableSize / Honeycomb より地味。UniformItems のセルは外側に余白 3 があるので、左右の端がほかの段より内側にずれる。

- ① UniformItems・Circular(曜日・弧・扇・軌道)のセルを Staggered と同じ淡い色分けにし、曜日は土を青・日を赤にする
- ② UniformItemsLayout を余白 3 の分だけ外へ広げ、左右の端を上の DockLayout とそろえる

### 🚦11-43 State(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/View/ViewStateView.xaml` + `ViewStateViewModel.cs`(+ `ViewStatePanelView.xaml`) | StateContainer(状態ごとの表示)と LazyView(遅延生成)の見本 |

今: 4 つのボタンのどれが今の状態か分からない。状態の表示が高さ 180 の枠の上に寄って枠の下が空き、画面の下半分も空いている。

- ① 4 つのボタンを、選んでいるものを塗る切り替え(App の Timer・Sudoku の形)にし、Loading は青・Empty は灰・Error は赤・Success は緑にする
- ② 状態の表示を枠の上下中央に置き、アイコンを共有の丸い地のアイコン(`BasicEmptyIconBorder` / `BasicSuccessIconBorder` など)にそろえる
- ③ LazyView の読み込みのボタン(高さ 40・文字 12)を `BasicOutlinedButton`(44・14)に寄せる

### 🔲11-44 Border(🔴📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/View/ViewBorderView.xaml` + `ViewBorderViewModel.cs` | Border の形・色・線・角丸を、値を変えて試す |

今: 白地に太字 18 の名前と Picker / Slider を並べただけで、灰色の地に InfoCard を置くほかの画面より古く見える。

- ② プレビューと設定を InfoCard(プレビュー / 形と色 / 線 / 角丸)に分け、名前を左・値を右(等幅)に置く 1 行の形にする
- ③ 色の Picker の横に選んだ色の丸を出す(「RedDefault」のような名前だけの表示を補う)

### 🌗11-45 Shadow(🔴📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/View/ViewShadowView.xaml` + `ViewShadowViewModel.cs` | Shadow の値を変えて試す画面と、ニューモーフィズムの見本 |

今: 11-44 と同じく、白地に設定を縦に並べただけで古く見える。ニューモーフィズムの影が、タイルを並べた枠の端で四角く切れ、角の外に白い三角や、直線で切れた帯が出る。

- ① 11-44 と同じく、灰色の地に InfoCard(プレビュー / 影の設定 / ニューモーフィズム)を置く形に分け、値は右に等幅で出す
- ② タイルを並べた `NeumorphRowStack` に影の分(20 程度)の Padding を取り、影が切れないようにする
- ③ Border / Color の Picker の横に、選んだ色の丸を出す

### 🧩11-46 Toolkit(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/View/ViewToolkitView.xaml` + `ViewToolkitViewModel.cs` | Syncfusion / CommunityToolkit の部品の一覧(入力 / 表示のタブ) |

今: `SfSegmentedControl` の選択が既定の紫(#6750A4)で、青で統一したタブの下線やチップと合わない。`SfOtpInput` だけ左寄せで、Expander の見出しの「▼」はただの文字なので、開いても向きが変わらない。

- ① SegmentedControl の選択の色(`SelectionIndicatorSettings`)を `BlueDefault` にする
- ② OtpInput を中央に寄せる
- ③ Expander の見出しの「▼」を、開閉で向きが変わる表示(共有の `ExpandGlyphConverter`)にする
- 部品の UI の画面への取り込みは 10-21

### 🛠️11-47 Custom(🟢📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/View/ViewCustomView.xaml` + `ViewCustomViewModel.cs`(部品は `Controls/TreeView.cs` / `ColorPicker.cs`) | 自作の部品(MarqueeLabel / TreeView / ColorPicker / DurationPicker / AvatarGroup)の一覧 |

今: TreeView の開閉の印(▸)が大きさ 14 の薄い灰色で見えにくく、フォルダーとファイルの区別も無い。ColorPicker の R / G / B / A のスライダーが全部同じ青で、どれがどの色か分からない。

- ① TreeView の印を MaterialIcons の Chevron(20・濃い灰)にし、行の頭にフォルダー 📁 とファイル 📄 の絵文字を付ける
- ② ColorPicker のスライダーを R は赤・G は緑・B は青・A は灰に色分けし、16 進の値を等幅の小さなチップにする
- 見本のデータは 10-23

### 📤11-48 Bottom Sheet(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/View/ViewBottomSheetView.xaml` + `ViewBottomSheetViewModel.cs` | SfBottomSheet と自作の BottomSheetView の比較 |

今: シートの 4 行(共有 / リンクをコピー / お気に入りに追加 / レポート)がどれも同じ Chevron のアイコンで、行の区切りも無い(`SheetSeparator` は定義だけで使っていない)。行の文字は許可値に無い 15。結果のカードは小さな 1 行だけで、画面の下半分が空く。

- ① 行ごとのアイコンを XAML のコンバーターで出す(共有 = Share・リンク = Link・お気に入り = 琥珀の Star・レポート = 赤の Flag)
- ② 行の間に `SheetSeparator` の 1px の線を入れ、文字を 16 にする
- ③ 結果のカードを、選んだ操作のアイコンと名前を並べた大きめの行にする

### 📑11-49 Drawer(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/View/ViewDrawerView.xaml` + `ViewDrawerViewModel.cs` | SfNavigationDrawer と自作の SideDrawer の比較 |

今: 切り替えの `SfSegmentedControl` が既定の紫で、ほかの青と合わない。選択中のカードは小さな青文字 1 行だけで、画面の下半分が空く。ドロワーの行では選んでいる項目が分からず、文字も許可値に無い 15。

- ① SegmentedControl の選択の色を `BlueDefault` にする
- ② ドロワーで選んでいる行を、淡い青の地に青のアイコンと文字で示し、文字を 16 にする
- ③ 選択中のカードを、項目のアイコンと名前を並べた大きめの行にする

### 🎞️11-50 Animation(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/View/ViewAnimationView.xaml` + `ViewAnimationViewModel.cs` | ボタンを押したときと、タイルをタップしたときのアニメーション |

今: タップ用のタイル 4 枚が、列の中央・左下・中央とばらばらに置かれ、白い空きが不規則に残る。ボタンは角の丸みが小さい既定の形。

- ① タイル 4 枚を同じ大きさの 2 × 2 にそろえ、Sequence の動く量を枠の中に収める
- ② タイルに効果ごとのアイコン(回転・連続・ばね)と淡いグラデーションを付ける
- ③ ボタンを `BasicFilledButton`(角 8)に寄せ、効果ごとのアイコンを付ける

### 〰️11-51 Easing(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/View/ViewEasingView.xaml` + `ViewEasingViewModel.cs`(動きは `Animations/EasingDemoAnimation.cs`、曲線は `Controls/EasingCurveView.cs`) | Easing 11 種の曲線と動きの比較 |

今: 丸が左上から右下へまっすぐ斜めに動き、描いた曲線(左下から右上)と向きも道筋も合わない。曲線は薄い水色の細い線で目立たない。

- ① 丸を曲線の始点(左下)に置き、横を時間・縦を Easing の値で動かして曲線をなぞらせる
- ② 曲線を系統ごとの濃い色(Sin は青・Cubic は緑・Bounce は橙・Spring は紫)にし、0 と 1 の高さに薄い補助線を引く

### ✨11-52 Effect(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/View/ViewEffectView.xaml` + `ViewEffectViewModel.cs` | 出現・バッジ・カウントアップ・フォーカス・押下などの演出の一覧 |

今: 白いカードと影で整っている。Replay のボタンが 1 枚目のカードに接していて、バッジを増減するボタンが細いハイフンの「-」「+」。

- ① Replay を見出しの行の上下中央に置き、カードとの間を空ける
- ② 「-」「+」を MaterialIcons の Remove / Add にする
- 文字のスペースは 11-67

### 🖐️11-53 Drag & Drop(🟡📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/View/ViewDragDropView.xaml` + `ViewDragDropViewModel.cs` | 長押しのドラッグによる並べ替え・リスト間の移動・ゴミ箱 |

今: 並べ替えの行が角丸の枠で囲んだカードで、一覧はフラットな行にする決まりと違う。左の色の帯は角丸で削れて細い曲線に見え、ドラッグできることを示す取っ手も無い。

- ① 並べ替えの一覧をフラットな行と 1px の区切り線にし、左の色の帯を行の高さいっぱいにする
- ② 行の右に取っ手のアイコン(`Drag_indicator`)を付ける
- ③ TODO / DONE の見出しを色付きのバッジ(TODO は青、DONE は緑と ✅)にする
- ④ ⚖️ TODO / DONE の列の中のカードは、かんばんのカードとして角丸のまま残す(案)

### 🎬11-54 Lottie(🟡📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/View/ViewLottieView.xaml` + `ViewLottieViewModel.cs` | Lottie の再生・シーク・スクロール連動・長押しでの進行 |

今: 開いた直後は大きな白い枠の真ん中に小さな点が 1 つあるだけで、空の画面に見える。スクロール連動の帯の文字が右端で「→ → → スクロ」と切れ、スライダーと時刻の左右の端もそろっていない。

- ① 枠の地を淡いグラデーションにし、開いた直後に絵が分かるコマを出す(少し進めた位置にするか、自動で再生する)
- ② 帯の文字を左寄せにして、最初から読めるようにする
- ③ スライダーと時刻の左右の端をそろえる

### 🖍️11-55 Graphics(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/View/ViewGraphicsView.xaml` + `ViewGraphicsViewModel.cs` | DrawingControl の図形・手描きと PNG 出力・波紋・カウントダウン |

今: 最初に出る図形が原色の青(#0000FF)と赤(#FF0000)で、ほかの画面の色より粗く見える。ボタンのアイコンは Clear にしか付いていない。

- ① 最初に出る図形の色を、アプリのパレット(`BlueDefault` / `RedDefault` など)にする
- ② ボタンを `BasicIconOutlinedButton` で、全部アイコン付き(線・丸・四角・消す・元に戻す・出力)にそろえる

### ✍️11-56 Drawing(🟡📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/View/ViewDrawingView.xaml` + `ViewDrawingViewModel.cs` | DrawingView での手描きと、保存した絵のプレビュー |

今: プレビューの案内は薄い灰色。

- ② 案内の文字色を `BasicCenterHintLabel` と同じ濃さにする(今は GrayLighten1)

### 📊11-57 Chart(🟡📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/View/ViewChartView.xaml` + `ViewChartViewModel.cs`(描画は `Graphics/Drawing/ChartDrawing.cs`) | 自作の描画によるグラフ 7 種の切り替え |

今: グラフは白地のまま残りの高さいっぱいに縦長に伸び、切り替えのチップは 4 + 3 の並びで 2 段目の右が空く。

- ② グラフを、灰色の地の上の白い枠に入れる
- ③ チップに種類のアイコン(Show_chart / Bar_chart / Donut_large など)を付け、2 段目を中央にそろえる
- 軸・凡例・値の表示は 10-22

### 🍩11-58 Sf Chart(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/View/ViewSfChartView.xaml` + `ViewSfChartViewModel.cs` | Syncfusion のグラフの一覧 |

今: ドーナツが Syncfusion の既定の色のままで、濃い灰色の区画が浮いて見え、数字しか出ないので何の区画か分からない。小さい区画の「9」だけが外の吹き出しになる。縦棒のグラフは縦の格子線が多く、単色の角ばった棒が並ぶ。

- ① ドーナツの色をアプリのパレット(`PaletteBrushes`)にし、凡例(食品 / 日用品 / 衣料 / 家電 / その他)を出す
- ② 値のラベルの置き方をそろえる(全部を内側か、全部を線付きの外側)
- ③ 縦棒の縦の格子線を消し(`ShowMajorGridLines`)、棒の上の角を丸める(`CornerRadius`)

## 🧪Sample

### 🕸️11-59 Web Basic(🔴📦📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Sample/SampleWebBasicView.xaml` + `.xaml.cs` / `SampleWebBasicViewModel.cs`(ページは `Resources/Raw/web-basic/`) | HybridWebView と C# の相互の呼び出し |

今: 中のページがブラウザの既定の見た目のまま(灰色の小さなボタン、「Log:」と小さな入力欄)で、下が大きく空く。受信のハイライトは code-behind で行っている。

- ① `index.html` / `other.html` に CSS を足す。余白 16・文字 16、ボタンは幅いっぱい・高さ 44・角 8 で `BasicOutlinedButton` と同じ色の枠、ログは等幅の角丸の枠にして下まで広げる
- ③ 受信のハイライトを、code-behind から `AnimationOption.HighlightTrigger` / `HighlightColor` へ移す
- ④ 状態の帯の文字を 18 → 14 にする

### 🗺️11-60 Map(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Sample/SampleMap1View.xaml` + `SampleMap1ViewModel.cs` | MAUI の Map(Google)のピン・経路・範囲・円 |

今: 右上の丸いボタンは整っているが、経路・範囲・円・地図の種類を切り替えても、今どれが出ているかがボタンの見た目で分からない。

- ① 切り替えのボタンに状態を出す(表示中は色の地に白のアイコン、非表示は今の白地に色のアイコン。VM の RouteVisible / AreaVisible / CircleVisible / CurrentMapType を DataTrigger で見る。`AppIcons` に白の版を足す)

### 🌍11-61 Map2(🔴📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Sample/SampleMap2View.xaml` + `SampleMap2ViewModel.cs`(地図の部品は `Messaging/MapsuiMapManagers.cs`) | Mapsui(OpenStreetMap)の機能ごとの表示の切り替え |

今: 左上の切り替えのパネルが、同じ左上にある Mapsui のズームのボタン(Widget の見本)を隠していて、パネルの左上に灰色の端がはみ出す。パネルの行は高さ 32・文字 12 で押しにくい。右下のホームのボタンが「© OpenStreetMap contributors」の表記に接している。

- ① Mapsui のズームのボタン(`ZoomInOutWidget`)を、パネルと重ならない右上へ移す
- ② 6 つの切り替えを、右下のボタンの列の一番上のボタンで開く下のシートに入れる。行は高さ 44・文字 14 にし、名前の前に絵文字を付ける(🧭 Widget / 📍 Spot / 🔷 Shape / 🗾 GeoJSON / 🔵 Cluster / 🌈 Overlay)
- ③ 右下のボタンの列の下の余白を広げて、表記から離す

結果: ① は右上へ。② は地図の上のパネルをやめ、下のシート(開くボタンはレイヤーのアイコン)に高さ 44・文字 14・絵文字の行。③ は下の余白 16 → 40。

実施(2026-10-05)、確認待ち。結果は `Change_Summary.md` の区間 17。

### 🎥11-62 Media(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Sample/SampleMediaView.xaml` + `SampleMediaViewModel.cs` | MediaElement での動画の再生と、自作の操作のバー |

今: 開いた直後は黒い画面に読み込みのくるくるだけで、タップで操作のバーが出ることが分からない。読み込みに失敗するとくるくるが消えて黒いままになる。バー(半透明の黒の角丸、再生・時間・シーク)は整っている。

- ① 開いた直後もバーを出して、3 秒で隠す
- ② 失敗したとき(`MediaElementState.Failed`)は、中央にアイコンと「再生できません」を出す

### 📝11-63 Markdown(🔴📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Sample/SampleMarkdownView.xaml` + `SampleMarkdownViewModel.cs` | Markdown の表示(見出し・箇条書き・引用・コード・表・リンク) |

今: 本文・箇条書き・引用とコードの箱が、画面の左右の端に付いている。見出しは 3 段とも青系の細い文字で、大きさの差も小さい(24 / 20 / 18)。本文は純黒。

- ① ScrollView に左右 16・上下 12 の余白を付ける(画面ローカルのスタイル)
- ② 見出しを太字にして差を付ける(H1〜H3 の大きさ 28 / 22 / 18、色は濃い青灰を基本に H1 だけアクセントの青)。本文は `GrayDarken3` にする
- ③ 引用とコードの箱の枠の色を淡くする

結果: ① は左右 16・上下 12。② は H1 を 28 の太字の `BlueDarken2`、H2 を 22 の太字の `BlueGrayDarken3`、H3 を 18 の太字の `BlueGrayDarken2`、本文を `GrayDarken3` に。③ は引用の枠を `OrangeDarken4` → `OrangeLighten2`、コードの枠を `GrayDarken1` → `GrayLighten2` に。

実施(2026-10-05)、確認待ち。結果は `Change_Summary.md` の区間 17。

### 📄11-64 PDF(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Sample/SamplePdfView.xaml` + `SamplePdfViewModel.cs` | PDF の表示とページの移動 |

今: F3 / F4 の ◀️ ▶️ が絵文字の橙の四角で、緑・橙のキーの色とぶつかる。スライダーの溝(GrayLighten2)は白地でほとんど見えず、ページ数(18 の灰色)は別の行に離れている。

- ① F3 / F4 を文字(Prev / Next)にする
- ② スライダーとページ数を 1 行にまとめ(右に `BasicMonoLabel` で「1 / 6」)、溝の色を濃くする(GrayLighten1 など)

### 👁️11-65 CV Local(🟡📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Sample/SampleCvLocalView.xaml` + `SampleCvLocalViewModel.cs` | 端末の中(ONNX)での物体検出 |

今: Detect を押してから結果が出るまでの表示も、結果の後の案内も無く、同じ形の CV Net より簡素。

- ① 推論中は `BasicLightLoadingIndicator` を出す(VM の IsProcessing)。撮影の瞬間は白いフラッシュを出す(CV Net の `ShutterFlashBoxView` と `FlashTrigger`)
- ② 結果の表示中は、CV Net と同じ「Retry でプレビューを再開」の帯を下に出す

### ✂️11-66 Crop(🟢📦)

| 現在のファイル名 | 何用か |
| --- | --- |
| `Modules/Sample/SampleCropView.xaml` + `SampleCropViewModel.cs` | 画像のトリミング(枠の移動・四隅での拡大縮小)と書き出し |

今: 暗い編集の面、角丸の白いパネル、丸いボタンで整っている。書き出す前のプレビューが灰色の四角だけで、ボタンの高さが 40。

- ① 書き出す前のプレビューに、薄い切り抜きのアイコンを出す
- ② ボタンの高さを 44 にする

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
