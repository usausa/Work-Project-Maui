# UI 変更・確認チェックリスト(uibase → 現在)

git タグ **`uibase`(2026-05-16、UI ブラッシュアップ着手前のベースライン)から現在(2026-07-12)まで**の変更を**画面単位**でまとめた確認用ドキュメント。第1弾ブラッシュアップ+UISample 取り込み+第2弾ブラッシュアップ+改名+スタイル切り出しの全 UI 作業を含む。**全変更コミット済み**(ビルド 0 エラー・新規警告ゼロ=既知の CS8785×1 / XA4301×7 のみ)。

- このファイルの役割 = **人が実機で見て確認する**ためのチェックリスト。確認できた画面は `[ ]` にチェックを付け、問題があれば行末にメモを追記する。
- 経緯・技術詳細・決定事項 = `UI_Development_Log.md`(本書と Log の2本立て運用)。
- **残作業** = 末尾「画像アセットのコード反映」のみ(素材待ち)。
- **環境制約(不具合ではない)**: ①地図タイルは Google Maps API キー未設定だと非表示(ピン・カメラ移動は動作)②SampleCvNet 系は AI エンドポイント未設定だと画面に入れない ③CommunityToolkit CameraView の `CaptureAsync` がまれに未完了になり Function キーが無反応化(再起動で回復)。

---

## 0. 共通基盤・新規部品(画面横断)

新規追加または改修した基盤。個別画面はこれらに依存する。

### 新規コントロール(`Controls/`)
- `InfoCard` — 見出しアイコン+タイトル+区切り線付きの角丸カード(ControlTemplate 方式)
- `StatusChip` — アイコン+テキストのピル型チップ(DataTrigger で状態色分け)
- `StepIndicator` — ●●● のステップ表示(現在=ピル型強調+「n / N」)
- `EasingCurveView` — Easing 曲線を背景描画する GraphicsView
- `Gauge` / `CalendarView` / `CalendarView2` / `CameraOverlayView` / `DayTimetableView` / `GraphRowSurface` ほか(UISample 取り込み・第1弾で新設)

### 新規ビヘイビア/コンバータ/マークアップ(`Behaviors/` `Converters/` `Markup/`)
- `AnimationOption` — 入場/常時/フィードバック演出の添付プロパティ群。**第2弾で `ProgressTo` を新設**(ProgressBar を 800ms CubicOut で伸長)
- `MapBind`(+`MapController.MoveTo`)/ `MediaBind` / `SliderOption`(DragCompletedCommand)を追加
- `Converters` 多数追加(`CenteredRatioConverter` / `BadgeCountConverter` / `CompassDirectionConverter` ほか)
- `Markup/FontIconExtensions` — アイコン用マークアップ拡張(`Material`/`Fluent`/用途別 `MenuIcon`/`MoneyIcon`)

### 描画基盤(`Graphics/`)
- **`Graphics.Drawing`**(IDrawable・データ駆動: `DrawingObject`/`DrawingControl`/各 `*Drawing`)と **`Graphics.Scene`**(SKCanvas・自走: `SceneObject`/`SceneControl`/各 `*Scene`)へ再編(旧 `GraphicsObject`/`GraphicsControl`/`AnimatedSkiaView` から改名・移設)
- `SensorDrawing`(方位ダイヤル+気泡水準器)追加、`ShapeDrawing` に Circle 追加

### その他基盤
- `MauiProgram` に **NotoSerifJP** フォント登録追加(BasicFont の明朝見本用)
- `Resources/Styles/Styles.xaml` にリソース/派生スタイル追加(共有スタイルの既存定義は不変更=ポリシー遵守)
- `Models/Sample/*` 多数追加(Calendar/Chat/Graph 系のモデル)、`PhotoItem` を ObservableObject 化(+IsCurrent)

- [ ] 起動〜各メニュー遷移がこれまで通り動作する(基盤変更による退行がない)

---

## 1. メニュー(空セル統一・並び替え)

- 空セルの扱いを「**可視の無効ボタン**」に全メニュー統一(SampleMenu=IsVisible 除去7箇所 / SampleCvNetMenu=同4箇所 / UIMenu=空きセルに無効ボタン追加)
- UIMenu: Timeline→Graph2 改名に伴う並び替え、Profile2・Cockpit 廃止で **11行→10行×3列=30ボタン・空セルなし**
- その他メニュー間の差異(番号プレフィックス/絵文字/列数/アイコン有無)は**意図的に維持**(統一しない)

- [ ] UI メニュー: 10行×3列の30ボタンで全セルが埋まっている(Profile2・Cockpit が無い。グループ順=EC→ツール→プロフィール→SNS→日付→可視化→計器→HUD)
- [ ] Sample / SampleCvNet メニュー: 空セルが「薄い無効ボタン」として見える(欠けセルがない)
- [ ] 他メニュー(Basic/Device/Navigation/Network/View)は従来どおり(退行確認のみ)

---

## 2. UI モジュール(決済・EC)

### UILogin(第1弾+アイコン微調整)
- 背景/レイアウト/メッセージ/入力/Keep login/Login/パスワード表示トグル/Forget password の各セクションを整備(第1弾)
- **ユーザー名・パスワードの左アイコンを Size 30→28 に変更**(2026-07-12・非標準サイズ解消)
- [ ] レイアウトが崩れず、パスワード表示トグルが効く
- [ ] 左アイコン2つ(人物/鍵)が大きすぎず整って見える

### UIMoney(第1弾)
- Background/Header/Rank/Menu/Detail/Bottom select+バッジ の構成でカード型に整備、エントランス演出(AnimationOption)を全面適用
- メニューアイコンは `MoneyIcon` マークアップ拡張へ集約(既定 28)
- [ ] ランク表示・メニュー・明細・下部セレクト+バッジが崩れず表示される
- ※Money メニューの個別アイコンは遷移先未結線=**仕様(メッセージ表示は不要)**

### UISuper(第1弾)
- 検索バー+ポイント残高 / バナーカルーセル / ミニアプリ / クーポン の縦構成
- FontSize を大きい側の許可値へ拡大(第2弾)
- [ ] バナーカルーセルが送れる、各セクションが崩れない
- ※検索・ミニアプリの遷移は未結線=**仕様**

### UIPos(第1弾)
- POS 明細・小計まわりを整備、増減ボタンの参照実装(UIItem/UICart の手本)
- [ ] 明細と合計まわりが崩れず表示される

### UIShop(第2弾・タスク2-1)
- 商品カード(Popular 横スクロール+All Items 2列グリッド)を **SfEffectsView Ripple** 化
- ヘッダ→検索→見出し→リストの **FadeUp 段差入場**
- **検索 Entry を実結線**(All Items をタイトル部分一致で絞り込み。`SearchText`+NotifyAlso 計算プロパティ)
- [ ] カードタップでリップルが出て Item 画面へ遷移する
- [ ] 入場時にセクションが順に現れる
- [ ] 検索欄に文字を入れると All Items が絞り込まれる(例「se」→ Aqua Serum のみ)、空で全件復帰
- ※Filter ボタンは未結線=**仕様**

### UIItem(第2弾・タスク2-1)
- 数量ステッパー結線(1..99)
- **サイズタグ 30/50/100ml のタップ切替**(選択=ピンク。DataTrigger 活性化)
- 商品画像+皿の **Pop 入場**
- ※数量変更時に Add to Cart が弾む演出は「別ボタンへのエフェクト不要+Bounce 中に角丸が崩れる」ため**削除済み**
- [ ] サイズタップで選択が移る(初期 50ml)
- [ ] 数量 +/− が動き、1 未満にならない

### UICart(第2弾・タスク2-1)
- `UICartItem` を ObservableObject 化し **ステッパー結線+合計再計算**(ItemCount/Subtotal/Discount10%/Total を連動、数量1..99)
- Total は **CountUp(450ms)+Bounce**、明細行 FadeUp 段差
- **Checkout 結線**(件数と合計入りの完了ダイアログ)
- [ ] +/− で数量・小計・割引・合計・ヘッダ件数がすべて連動する
- [ ] 合計変更時に数値が滑らかに変わり弾む
- [ ] Checkout で「Checkout completed」ダイアログが出る

---

## 3. UI モジュール(ツール・プロフィール・キャラクター)

### UIDock(第1弾・小改修)
- ドックのアイコン配置/演出を微調整
- [ ] 従来どおり表示・遷移する

### UIProfile(第2弾・タスク2-0/Profile 統合)
- 旧 Profile2 のカード型デザインを **UIProfile へ統合**(UIProfile2 は View/VM/ViewId/メニューボタンとも削除)
- 名前=**「山奥 うさぎ, 29」**、**パララックスヘッダー移植**(カバーが内容よりゆっくり動く・透けない)
- **SNSアクション3トグル**: フォロー(人物→人物✓・青)/ いいね(♡・ピンク+Bounce)/ お気に入り(★・琥珀)。全て「OFF=白地+色アイコン / ON=色地+白アイコン」で統一、PressEffect 付き
- 初期状態: ♡(ピンク地)★(琥珀地)は ON、フォローは OFF
- 写真6枚グリッド/色付きタグ/区切り線統計/Bio は旧 Profile2 の内容を維持
- [ ] メニュー「Profile」からカード型が開き、名前が「山奥 うさぎ, 29」
- [ ] スクロールでカバーがパララックスし、下の内容に透けない
- [ ] 初期表示で ♡ と ★ が ON、フォローが OFF
- [ ] フォロータップで青地白の「人物✓」に反転、♡/★はタップで outline に戻り再タップで色地(♡は弾む)
- [ ] Back で UI メニューへ戻る(Profile2 が無い)

### UICharacter(第1弾)
- Character / Class / Detail のカード構成に整備
- [ ] キャラクター情報・クラス・詳細が崩れず表示される

### UIPet(第2弾・タスク1)
- ステータスバー4本を ProgressBar 化し **0→実値の伸長アニメ**(`ProgressTo` 新設)+数値 CountUp、シート全体 FadeUp
- **Heart ボタン結線**(HP+5/上限400、バー・数値連動+Bounce)
- **Add to Party トグル**(✓ In Party+緑)
- [ ] 表示時にバーが伸び、数値が数え上がる
- [ ] Heart 連打で HP バーが伸び 400 で止まる
- [ ] Add to Party ↔ ✓ In Party がトグルする

### UIFeel(第2弾・タスク1)
- **7つの hex セルにタップ選択**(選択セルに自系統色の枠3px+チェックバッジが移動、バッジは表示時 Pop)
- ハニカム全体が中央→外周へ Pop 開花入場
- [ ] 入場時に中央→外周へ順にポップする
- [ ] 任意セルをタップすると枠とバッジがそのセルへ移る(初期は Happy)

---

## 4. UI モジュール(通信・SNS・配信)

### UIMail(第1弾)
- Header / Messages / Empty state / Indicator / Floating Button / Tab の構成に整備
- [ ] メッセージ一覧・タブ・空状態が崩れず表示される
- ※通知・検索・FAB は未結線=**仕様**

### UIChat(第1弾・新規)
- Send / Receive / System のバブル+リアクションピル(4列2行折返し)
- メッセージバブルの FontSize を読みやすく拡大(第2弾)
- [ ] 送受信バブル・システムメッセージ・リアクションが崩れず表示される
- [ ] 本文が読みやすい

### UISocial(第1弾)
- Shared/背景/Icon/Episode/Counter/Alert の構成、作戦中止の Back 結線(SfEffectsView)
- [ ] エピソード一覧・カウンター・アラートが崩れず表示される
- ※タイルの遷移は未結線=**仕様**

### UIStream(第2弾・タスク2-2)
- **「+ My List」トグル結線**(✓ In My List+赤背景)
- TopBar/Hero の FadeUp+レーティング Pop、セクション棚の FadeUp 段差、ポスター Ripple、各ボタン PressEffect
- [ ] My List がトグルする
- [ ] ポスタータップでリップル→詳細へ遷移
- ※検索・通知・See all は未結線=**仕様**

### UIStreamDetail(第2弾・タスク2-2/親子フロー)
- **タブ切替(Trailers ⇔ More Like This)でコンテンツが FadeIn**
- **Favorite トグル**(outline→赤 filled+Bounce)、**Download トグル**(→緑チェック)
- Trailer 行・Related タイル Ripple
- [ ] タブ切替の度にリスト面がふわっと入れ替わる
- [ ] Favorite/Download が押す度にトグルする
- ※Play/Resume/Share は未結線=**仕様**

---

## 5. UI モジュール(日付・予定 / データ可視化 / 計器 / HUD)

### UISchedule(第1弾)
- 7日チップ+タイムテーブルの構成、FontSize 17→18
- [ ] 週チップとタイムテーブルが崩れず表示される

### UICalendar(新規・カレンダー接続)
- `CalendarView2` を用いた月表示(日付/曜日/年の FontSize 既定 13→14)
- **日付セルのタップで日付(yyyy/MM/dd)、イベントのタップで予定タイトルがトースト表示**(旧 Debug 出力から変更、MakeAsyncCommand 化)
- [ ] 日付数字・曜日ヘッダ・年表示が読みやすく、イベントピルと重ならない
- [ ] 日付タップで日付トースト、イベントタップでタイトルトーストが出る

### UITimeline(第2弾・タスク2-2 / 旧 TimelineSample)
- 進行中イベントのドット(halo)Pulse+ヘッダ/リストの FadeUp 入場(行個別入場はリサイクル制約で不採用)
- **UITimelineSample→UITimeline に改名**(ViewId/View/VM/ファイル名/メニュー/タイトル)
- [ ] 「現在」のイベントのドットだけが脈動している
- [ ] スクロールしても脈動が別の行に移らない(リサイクル確認)
- [ ] メニュー名・タイトルが「Timeline」

### UIGraph / UIGraph2(新規/改名)
- UIGraph=Git グラフ表現の可視化画面(新規)
- UIGraph2=旧 Timeline を **Graph2 に改名**(同一 Git グラフの別表現として両立)+リスト全体の FadeUp 入場
- [ ] Graph / Graph2 が崩れず描画され、Graph2 は表示時にリストが FadeUp する

### UITreeMap / UIRadar(第1弾・小改修)
- TreeMap: 撮影瞬間のシャッターフラッシュ演出
- Radar: 飾りステータス追加(乱数は CA5394 の pragma 前例)
- [ ] TreeMap / Radar が従来どおり描画される

### UIGauge / UIMeter / UILoad / UIMixer(第1弾・計器系)
- Gauge: 6種サンプル(Pressure/Humidity/Temperature/Wind/Speed/Tachometer)を整備
- Meter: 四隅ビネット追加、Mixer: Knob/dB scale/Slider/Channel/Equalizer/Frequency(クラス名 Mixier→**Mixer** に統一)、Load: 微調整
- [ ] 各計器(Gauge6種/Meter/Load/Mixer)が崩れず描画・アニメする

### UIFlight / UITactical / UIEnergy / UITelemetry(第2弾・Scene フォント+改名)
- Skia 描画テキストを許可値へ拡大(8.5→9 / 9.5→10 / 7.5→8 / 7→8 / 13→14 / 15→16 / 17→18、Telemetry の GEAR 桁のみ 56→72)
- **UIFlightHud→UIFlight / UIMechHud→UITactical に改名**(Scene クラス名 FlightHudScene/MechHudScene は内部型として据え置き)
- **UICockpit は廃止**(UIEnergy へ差し替え。CockpitControls も削除)
- before/after 比較画像: `Document/FontSize_{FlightHud|MechHud|Telemetry|Energy}_{Before|After}.png`
- [ ] Flight: SPD 読値/GUN 残弾/下端ステータス行が読みやすく、テープ目盛・読値ボックスに重なりがない
- [ ] Tactical: CH1〜3 リスト/マップのユニットラベル(E1, D1-2 等)が読みやすく、重なりがない
- [ ] Telemetry: TIME/BEST が POS と重ならず、**GEAR の数字が大きくなり(56→72)パネル内に収まる**
- [ ] Energy: KPI カード(DEMAND/PV OUT/STEAM)の値がカード内に収まっている
- [ ] メニュー名・タイトルが Flight / Tactical(旧 FlightHud / MechHud が無い)

---

## 6. UI モジュール(Kit=オンボーディング/設定/ダッシュボード/通知/追跡)

### UIKitOnboard(第2弾・タスク2-2)
- **Skip / Get Started 結線**(→ Kit Dashboard)+ページスワイプでテキストが FadeIn
- [ ] スワイプの度にタイトル/説明がふわっと出る
- [ ] Skip・Get Started のどちらでも Dash へ遷移

### UIKitSetting(第2弾・タスク2-2)
- **Switch 実バインド化**(従来は getter-only で反映されなかったのを ObservableProperty 化)+グループ FadeUp 段差
- [ ] Switch を切り替えると状態が保持される(タップしても戻らない)

### UIKitDash(第2弾・タスク2-2)
- メトリクス4カード Pop 段差+Heart Rate カード FadeUp+ベル未読ドット Pulse+リンク2カード Ripple
- [ ] カードが順にポップ入場し、ベルの赤ドットが脈動する
- [ ] リンクカードタップで Tracking / Onboarding へ遷移(リップル付き)

### UIKitNotify(第2弾・タスク1)
- 未読/既読の視覚差(未読=左アクセントバー+青背景+太字+未読ドット、既読=白+通常)
- **行タップで既読化**(初期は先頭3件が未読)
- [ ] 未読行をタップすると即座に既読表示に変わる(戻らない)

### UIKitTracking(第2弾・タスク1)
- ステップ3状態化=完了(緑+白チェック)/進行中(青ドット Pulse+タイトル青)/未来(灰)、完了区間の縦線を緑に塗り分け、行 FadeUp 段差
- [ ] 「Out for delivery」だけ青く脈動し、完了3行はチェック付き緑
- [ ] 完了区間の線が緑、それ以降が灰

---

## 7. Navigation

### Wizard Input1 / Input2
- 入力フィールド定型(キャプション+Border+フォーカスで青枠)、StepIndicator(1/3・2/3)、ステップ説明カード+ヒント
- **Next(▶️)は入力があるまで無効**(WizardContext を ObservableObject 化)
- Input2 の Placeholder 誤記(Data1→Data2)修正
- [ ] Entry フォーカスで枠が青くなる
- [ ] 未入力だと ▶️ が無効、1文字で有効化、消すと再び無効
- [ ] Input2 のプレースホルダが「Data2」/ StepIndicator が 1/3→2/3 と進む

### Wizard Result
- 入力サマリカード(Data1/Data2)+完了カード、Function4 表示を ✔️ に
- [ ] Input1/2 で入れた値がサマリカードに表示される / ✔️ で完了

### Stack 1 / 2 / 3
- 本文空だった3画面を中央カード化(レベル別アクセント 1=Blue / 2=Teal / 3=DeepPurple、Looks_one〜3 アイコン円+StepIndicator+キー操作ヒント+Pop 入場)
- [ ] 各画面が色違いの中央カードで表示され、入場時にポップする
- [ ] StepIndicator が 1/3→2/3→3/3 と進む / 従来のスタック遷移が正常

### Shared Input / Main1 / Main2
- SharedInput に「Return to Shared1/2」チップ(遷移元により Indigo/Teal)+入力フィールド定型
- Main1/Main2 は系統色の数字円+チップ+No 大型表示+FadeUp
- [ ] Shared1 から入ると Indigo 系、Shared2 から入ると Teal 系のチップになる
- [ ] 入力して戻ると元画面に値が反映される

### Edit List / Detail
- List: 一覧行を白カード化(Id=等幅 #n ピル、Edit/Delete に PressEffect)+空時 EmptyView(Inbox+「Press New」)
- Detail: 入力フィールド定型+インラインエラー表示
- [ ] 全件削除すると EmptyView が出る
- [ ] Detail で空のまま確定するとインラインエラーが表示される

### Navigate Cancel / Initialize
- Cancel: パターン説明カード常設(Amber Help+Yes/No の挙動説明)
- Initialize: 初期化3秒間スケルトン+Hourglass Pulse → 完了で緑 Task_alt カードが FadeIn
- [ ] Initialize 直後はスケルトン、約3秒後に緑カードへ切り替わる
- [ ] Cancel の Yes/No が説明どおりに動く

### InputNumber(モーダル)
- 全キーにローカル派生スタイル(共有 Input*Button+PressEffect)、✔/❌ を Material の Check/Close グリフに変更
- [ ] キー押下で縮む(PressEffect)/ 確定・キャンセルが従来どおり動く

---

## 8. Device

### DeviceInfo
- 3 InfoCard 化+FadeUp 段差入場
- [ ] 3カードが順に入場し、既存の情報が全て表示される

### DeviceSensor
- Vector3/Quaternion を VM で軸分解(RGB=XYZ 軸バッジ+中央ティック付きバー)
- **Compass カード=実描画ダイヤル**(30°目盛+N/E/S/W+赤針)、**Level カード=気泡水準器**(新設)
- [ ] 各センサー値が軸バーで動く(生 ToString ではない)
- [ ] 端末を回すと Compass が回転し赤針が北を指し続ける
- [ ] 端末を傾けると Level の気泡が動き、水平で緑になる

### DeviceStatus
- 開発用バッテリーアイコン列挙を削除→動的バッテリーアイコン+残量ゲージ(20%以下=赤/充電中=緑)+Network 状態チップ(緑/琥珀/赤)
- [ ] 実残量でアイコンとゲージが表示される
- [ ] 充電抜き差しで表示が変わる / 機内モードで Network チップが赤系になる

### DeviceAudio
- プレイヤーカード UI(250ms ポーリング、シーク Slider+DragCompleted、円形トランスポート3ボタン、再生中アイコン Pulse、音量%表示)
- [ ] 再生/停止/一時停止が動き、再生中はアイコンが脈動する
- [ ] シークバーを離した位置から再生される

### DeviceQrDisplay
- Entry ライブ編集→QR 再生成+額装+空状態表示
- [ ] 文字を打つたびに QR が更新され、空にすると空状態表示になる

### DeviceQrScan
- 結果空時「Scan a code...」プレースホルダ+検出時にカメラ面へ白 Flash
- [ ] QR を検出した瞬間に白い発光が走り、結果が表示される

### DeviceLocation
- 画面上部に 240px の地図(IsShowingUser)を常設、初期=東京駅→測位毎に現在地へ移動(MapController.MoveTo)
- 測位待ち空状態+Position/Motion カード+タイムスタンプ Highlight
- [ ] 測位開始までは空状態、取得後カードに値が入り更新毎に Timestamp が光る
- [ ] (API キー設定時)地図が現在地に追従する ※未設定ではタイル非表示が正常

### DeviceOcr
- カメラにガイド枠+**結果は画面内パネル表示**(ダイアログ廃止)+実行中オーバーレイ
- [ ] 認識実行中にオーバーレイが出て、結果がダイアログではなく画面内に出る

### DeviceBluetooth
- 状態を **インライン状態チップ**(Idle/Connecting/Printing/Completed/Failed)で表示(ダイアログ廃止、State enum+IsBusy)
- [ ] 印刷フローで状態チップが遷移する(失敗時は Failed 表示)

### DeviceBleScan
- 未検出時 EmptyView(Bluetooth_searching Pulse+「Scanning...」)+温湿度/CO2 値の更新時 Bounce
- [ ] スキャン中(未検出)に EmptyView が脈動する / 値更新でカードの数値が弾む

### DeviceBleHost
- Podcasts アイコン円(Advertising 中 Pulse)+状態チップの中央カード化
- [ ] Advertising 開始でアイコン円が脈動し、チップが状態を示す

### DeviceNfc
- 履歴 EmptyView「Suica をかざしてください」(Contactless Pulse・ダークテーマ)+残高 CountUp(600ms)
- [ ] 履歴なしで EmptyView 表示、読み取りで残高が数え上がる

### DeviceMisc
- 15 アクションを 4 InfoCard(Screen/Feedback/Light/Speech)に分類、Material アイコン付き ActionButton 化、音声認識中は赤マイク Pulse
- [ ] 各ボタン(バイブ/ライト/スピーチ等)が従来どおり動く / 音声認識中に赤マイクが脈動する

### DeviceCommunication
- タップ行カード×3(色分けアイコン円+Ripple)
- [ ] 行タップでリップルが出て、電話/SMS/メールが起動する

### DeviceActivity
- 歩数(96pt)を CountUp 化
- [ ] 画面表示時に歩数が 0 から数え上がる

### DeviceWiFi / DeviceBiometric
- 「Not implemented」空状態パネル(暫定。機能実装/削除は別途=D2)
- [ ] 両画面がクラッシュせず空状態パネルを表示する

---

## 9. Basic / Main / Data / Network

### BasicBehavior
- 2 Entry を Border+フォーカス枠(青/緑)で色分け+イベント値の Highlight カード
- [ ] フォーカス移動で枠色が変わり、イベント発火で値カードが光る

### BasicValidation
- **ルートの `ContentPage.Behaviors` 誤記を `ContentView.Behaviors` に修正(バリデーションが実際に動くようになった)**
- エラーで枠が赤 Highlight+エラーラベル FadeIn、Error/Clear/Focus をアイコン付きボタン化
- [ ] 不正入力でエラー表示が出る(修正前は無反応だった点が最重要)

### BasicConverter
- タイポ修正(Upper/Lowwer→Lower)+未使用 ToChecked 削除、CheckBox+ラベル横並び化、変換結果に Highlight
- [ ] 各コンバータ結果が正しく、値変更で結果欄が光る

### BasicStyle
- 全 Action/Information ボタンを ActionCommand で結線し「Last action」カードに押したボタン名を Highlight
- SelectButton は隣接セグメント配置
- [ ] どのボタンを押しても Last action に名前が出て光る

### BasicFont
- ScrollView 化(実機で全フォント見える)+フォント毎 InfoCard 10枚+JetBrainsMono/NotoSerifJP 見本追加
- [ ] 最後のフォントまでスクロールで見える / NotoSerifJP が明朝で出る(豆腐でない)

### BasicTypography
- ScrollView 化+4 セクションカード
- [ ] 全セクションがスクロールで見える

### BasicLocale
- 現在カルチャカード+リソースキー2件の一覧(カルチャ切替は対応不要=仕様)
- [ ] 現在のカルチャとリソース値が表示される

### BasicDialog
- 11 ボタンを 4 カテゴリの InfoCard に整理
- [ ] 全ダイアログが従来どおり開く

### Setting(Main)
- スキャンガイドピル+設定値パネル、値変更時 Highlight、未設定時「(not set)」
- [ ] QR 読み取りで設定値が入り、変更箇所が光る

### Data
- CRUD/Bulk の 2 カード化+BulkDataCount に CountUp
- [ ] Bulk 実行で件数が数え上がる / CRUD 操作が従来どおり

### NetworkRealtime
- TODO 解消=鋸波→**ランダムウォーク擬似データ**(CPU ゆらぎ+スパイク/Memory ドリフト/Network バースト)、StatControl 3枚(青/緑/橙)、500ms 更新
- [ ] 3 グラフがそれぞれ異なる質感で動き続ける(単調な鋸波でない)

### NetworkMenu
- VM の未使用 RealtimeCommand(デッドコード)削除のみ
- [ ] メニューからの遷移に退行がない

---

## 10. View

### ViewAnimation
- 2 InfoCard 化(Click 系4ボタン=**Shake 新設**+Tap 系タイル)、空セル解消
- [ ] Shake ボタンで震える / 他アニメも従来どおり

### ViewBorder
- コメントアウトされていた StrokeLineJoin・StrokeLineCap の Picker を復活
- [ ] 2 つの Picker で線の角/端の形状が変わる

### ViewCarousel
- 中央カード強調(非中央=Scale 0.92+Opacity 0.55、スナップ確定で切替)+白カード影+画像角丸クリップ+背景グレー
- [ ] スワイプすると中央だけがくっきり・両脇が沈む

### ViewCollection
- ▼▲を Expand_less/more アイコン化、スワイプ項目をアイコン+文字の縦積みに
- [ ] 展開/折りたたみ・スワイプメニューが従来どおり

### ViewDrawing
- 色6チップ+線幅 Slider のパレット(選択チップ枠強調)、プレビュー額装+未描画時の空状態
- [ ] チップで線色・Slider で線幅が変わり、プレビューに反映される

### ViewEasing
- 4×3 均等グリッドへ再設計(白カード+灰トラック+ボール)、**背景に Easing 曲線**(EasingCurveView)、12セル目=実行状態セル(Run 中 Pulse+Function4 無効化)
- [ ] 各カードに曲線が描かれ、実行でボールが曲線どおりに動く
- [ ] 実行中は再実行できず、状態セルが脈動する

### ViewEffect(新規・資産カタログ)
- 常時アニメ(Wave×3 時間差+Pulse+ON/OFF Switch)+変化フィードバック(Fire ボタンで Bounce+Highlight+Flash 同時発火)
- [ ] Switch で Wave/Pulse が止まる・再開する
- [ ] Fire で 3 種のフィードバックが同時に走る

### ViewGraphics
- Add line/circle/rect(乱数色)/Clear ボタン+図形数チップ(ShapeDrawing に Circle 追加)
- [ ] ボタンで図形が増え、チップの数が追従し、Clear で消える

### ViewLottie
- プレイヤー UI(シーク Slider+mm:ss.f 等幅表示+円形 Play/Pause・Reset、白カード額装)
- [ ] 再生/一時停止/リセット/シークが動き、時刻表示が進む

### ViewRefresh
- 初回ロード(1秒)中に EmptyView のスケルトン4行、ヘッダに Newspaper アイコン+件数ピル、行カード白+枠線化
- [ ] 初回表示時に一瞬スケルトンが見える / Pull-to-Refresh 後に件数ピルが更新される

### ViewSvg
- 3 SVG 切替(dotnet_bot/vite/react)チップボタン+ファイル名表示+額装
- [ ] チップで SVG が切り替わる

### ViewMenu
- 空セルを可視の無効タイルに(横断統一の一部)
- [ ] 灰色タイルで欠けセルがない

---

## 11. Sample モジュール(第1弾ブラッシュアップ+第2弾の小変更)

多くは第1弾で整備済み。uibase 以降の主な変更 = メニュー空セル可視化 / FontSize 拡大 / アイコン markup 化 / 軽微な整形。

- **SampleChart** — チャート表示整備+FontSize 13→14(第2弾)
- **SampleChat** — チャット UI 微調整(ChatView 共通コントロール側で整備)
- **SampleMedia / SamplePdf / SampleMap1 / SampleMap2 / SampleMarkdown / SampleWebApp / SampleWebBasic** — 第1弾で表示・操作を整備(Map はタイルに API キー要)
- **SampleCvLocal / SampleCvNet(Menu/Face/Object/Ocr/People/Tag)** — CV 系。SampleCvNetMenu は空セル可視化。AI エンドポイント未設定だと入れない(仕様)
- **SampleMenu** — 空セル7箇所を可視の無効ボタンに統一

- [ ] Sample 各画面が従来どおり表示・遷移する(退行がない)
- [ ] SampleChart の文字が読みやすくなっている
- [ ] Map 系は API キー未設定でもピン・カメラ移動が動く(タイル非表示は正常)

---

## 12. 横断的な変更(全画面に影響)

### FontSize 統一
- XAML/C# の FontSize を大きい側の許可値へ統一(13→14 / 15→16 / 17→18、カレンダーの日付/曜日/年 既定 13→14)
- 許可値 = 6,8,9,10,11,12,14,16,18,20,22,24,26,28,32,36,48,72,96,160
- Scene(Skia 自走)は情報量の観点でルール外の小値も許可(Excel 標準 6/8/9/10 優先)
- [ ] Profile 職業行が1行に収まる / Cart 明細・合計・Checkout の文字が崩れない
- [ ] Schedule/Timeline/Shop/Item/StreamDetail/Super/Graph/Graph2/KitDash/KitTracking/Feel/Pet/ViewEffect/SampleChart の文字が一回り大きく、崩れがない(ざっと確認)

### アイコン共通化
- 生 Unicode・絵文字を `markup:Material`/`Fluent`/`MenuIcon`/`MoneyIcon` へ置換(72箇所)
- **メニューアイコン(MenuIcon)を 24→28**、MoneyIcon 28、Login 左アイコン 28 で確定(非標準 30 は残ゼロ)
- 動的色の11箇所のみ FontImageSource 残置(Style 不可・バインド不可の制約)
- [ ] UI/Sample/SampleCvNet メニュー: アイコンが一回り大きく、ボタン内の文字(Login の g、Shop の p 等)が切れていない

### スタイル切り出しリファクタ(見た目不変)
- 第2弾で追加・改修した36画面の直書き視覚属性を画面ローカル ResourceDictionary の Style へ切り出し(**値は不変更=見た目は完全に同一のはず**)
- [ ] Navigation/Device/Basic/Setting/Data/Realtime/View 系の巡回中に、レイアウト・色・余白の変化を感じる画面がない

### 画面改名 / 基盤再編
- UITimelineSample→**UITimeline** / UIFlightHud→**UIFlight** / UIMechHud→**UITactical**、Timeline→**Graph2**、UICockpit→廃止(UIEnergy)
- Graphics を Drawing / Scene の2名前空間へ分離、Mixier→Mixer にスペル統一
- [ ] メニュー・タイトルの表示名が Timeline / Flight / Tactical / Graph2 になっている(旧名が無い)

---

## 13. 残作業:画像アセットのコード反映【素材が届き次第】

サンプル画面のプレースホルダ画像(profile.jpg の三重使い回し、縦長 social_background.png の 15箇所流用、usa キャラ絵の商品転用)を専用画像に差し替える。**素材の作成手段はスコープ外(別途対応)**。素材が届いたらコード反映のみ行う。

### 作成ファイル一覧(名前は暫定案・連番ゼロ埋め2桁)

```
Resources/Images/   (MauiImage … XAML で <Image Source="ファイル名"> 直接指定)
  ■ プロフィール(UIProfile)★★★
      avatar_user.jpg(512×512)  profile_cover.jpg(1600×800)  gallery01〜06.jpg(1000×1000 ×6)
  ■ ショッピング(UIShop / UIItem / UICart)★★★
      product_apparel01〜03.jpg(900×1200 ×3)  product_beauty01〜06.jpg(800×800 ×6)
  ■ 動画配信(UIStream / UIStreamDetail)★★★
      poster01〜06.jpg(600×900 ×6)  stream_hero.jpg(1600×900)  stream_clip01〜03.jpg(1280×720 ×3)
  ■ オンボーディング(UIKitOnboard)★★   onboard01〜03.jpg(1080×1080 ×3)
  ■ ペット(UIPet)★★                    pet01〜03.jpg(1000×1000 ×3)
  ■ プロモ(UISuper)★★                  banner01〜03.jpg(1200×600 ×3)
  ■ チャット(UIChat)★(任意)            avatar_person01〜05.jpg(256×256 ×5)
  ■ ログイン(UILogin)★(任意)           login_hero.png(512×512 透過)

Resources/Raw/      (MauiAsset … 既存プレースホルダの差し替え)
  ■ Social/player.jpg(256×256)差し替え ★    ■ Avatar/mofusand.jpg(256×256)差し替え ★
```

合計: 新規 42 枚+差し替え 2 枚。**★★★ の3グループ(プロフィール/ショッピング/動画配信=流用が最も目立つ)から着手**。

### 命名・配置・フォーマット
- MauiImage 名は**小文字英数字+`_` のみ**(`-`・大文字・日本語不可)。写真=.jpg(品質80)、透過/図版=.png
- サイズは「最大表示 dp×3」を1枚用意(密度別は自動生成)。**表示スロットのアスペクト比に合わせる**のが最重要
- Raw は `LogicalName` がカテゴリ相対パス(例 `Social/player.jpg`)

### 差し替え時に修正するコード
- プロフィール: `UIProfileViewModel.cs`(usa{n}_full 配列)/`UIProfileView.xaml`(カバー=social_background.png、アバター=profile.jpg)
- ショッピング: `UIShopViewModel.cs`/`UICartViewModel.cs`/`UIItemView.xaml`
- 動画配信: `UIStreamViewModel.cs`・`UIStreamView.xaml`/`UIStreamDetailViewModel.cs`・`UIStreamDetailView.xaml`
- オンボード: `UIKitOnboardViewModel.cs` / ペット: `UIPetView.xaml` / プロモ: `UISuperViewModel.cs`
- ログイン: `UILoginView.xaml` / ソーシャル: `Controls/SocialControls.cs`(player.jpg)/ メール: `UIMailViewModel.cs`

- [ ] (素材受領後)★★★ 3グループのコード差し替え+実機表示確認
- [ ] (素材受領後)残りグループの差し替え+確認

---

## 14. 横断確認(最後に)

- [ ] 各画面の Back/Function キーが従来どおり動作する
- [ ] 回転・再入場(画面を出て入り直す)で入場アニメが再生され、表示が壊れない
- [ ] ダーク寄り画面(Nfc/Stream 系)で文字が読める
- [ ] 未結線ボタン(Money/Mail/Social/Super/Stream/StreamDetail/Login/Shop Filter 等)は押しても無反応=**仕様どおり**(UIShop 検索のみ実機能)
