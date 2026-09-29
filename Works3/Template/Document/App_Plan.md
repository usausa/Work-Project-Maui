# 🧩App のミニアプリの実装計画(タイマー・ToDo・2048・マインスイーパー)

App のメニューに、アプリ・画面・モデルの実装の見本になる小さなアプリ(タイマー・ToDo・2048・マインスイーパー)を足す。あわせて、ダミーのデータで既存の分類を強化する天気(UI)とニュース(Control のタブ)を検討する。番号は `Task_Checklist.md` の 7 と同じ。

| 番号 | 段階 | 内容 |
| --- | --- | --- |
| 7-1 | タイマー | ストップウォッチとカウントダウン。背面やアプリの終了をまたいでも合う時間、終了の通知 |
| 7-2 | ToDo | 一覧と、Push / Pop で開くダイアログ的な編集。SQLite(既存の `data.db`)への保存 |
| 7-3 | 2048 | スワイプでの操作、モデルの結果から作るタイルのアニメーション、途中の盤面の保存 |
| 7-4 | マインスイーパー | 描画で作る盤面とタップ位置からのセルの特定、長押しで旗、最初の 1 手を安全にする配置 |
| 7-5 | 天気(UI) | 採用は 7-5-0 の判断。ダミーのデータで UI に天気の画面を足す |
| 7-6 | ニュース(Control のタブ) | 採用は 7-6-0 の判断。ダミーのニュースで Control にタブ(`SfTabView` と自作)を足す |

ファイルパスは `Template.MobileApp/` からの相対。

## ⚖️決定事項

| 項目 | 決定 |
| --- | --- |
| 置き場所 | 画面は `Modules/App/`、モデルは `Models/App/`(`SudokuGame` / `ExpressionCalculator` と同じく MAUI に依存しない)。VM はモデルの状態を画面に写し、操作をモデルに渡す |
| 画面の数 | 1 画面。ToDo だけ一覧と編集の 2 画面にする(編集の開き方は 7-2) |
| 表示 | VM は状態(列挙・数値・時間)を持ち、文言・色・表示の切り替えは XAML のコンバーターとトリガーで行う |
| 時刻 | `TimeProvider` を DI に登録し(`TimeProvider.System`)、VM が現在時刻をモデルに渡す |
| 乱数 | `RandomNumberGenerator.GetInt32`(CA5394 の抑制が要らない)。モデルは乱数を `Func<int, int>`(0 以上 n 未満)で受け取り、決まった値を渡して確かめられる形にする |
| 保存 | 小さな状態(タイマー、2048 の途中の盤面とベストスコア、マインスイーパーのベストタイム)は `Settings`(Preferences)。まとまった状態は JSON のソース生成(`Models/App/AppJsonContext.cs`)で文字列にする。ToDo は SQLite(既存の `data.db`) |
| メニュー | App のメニューの 3〜6 行目(`Grid.Row` 2〜5)に Timer / ToDo / 2048 / Minesweeper。アイコンは `AppIcons` に Timer / Checklist / Grid4x4 / Flag を足す |
| ファンクションキー | F1 = Back は共通。F2〜F4 は各節 |

## ⏱️7-1 タイマー

| 項目 | 内容 |
| --- | --- |
| 画面 | F2 = ストップウォッチ / F3 = タイマーで切り替える。ストップウォッチは経過時間(1/100 秒)、開始 / 一時停止 / 再開、ラップ(番号・ラップ・通算の一覧。最速と最遅を色分け)、リセット。タイマーは残り時間と進み具合、よく使う時間(1 / 3 / 5 / 10 分)と任意の分(数値入力のポップアップ `InputNumberAsync`)、開始 / 一時停止 / 再開 / リセット |
| 更新 | 表示の更新は画面を表示していて前面の間だけ(`IDispatcher.CreateTimer` + `TickAsObservable`、50 ms)。時間は刻みを数えず、開始時刻と止めた時点までの時間から毎回計算する |
| 背面・終了 | 状態(開始時刻・止めた時点までの時間・ラップ・目標の時間)を操作のたびに `Settings` に保存し、アプリを終了して開き直しても続きから |
| 通知 | カウントダウンの開始・再開で、終わる時刻に通知を予約し(`INotificationService.Schedule`)、一時停止・リセットで取り消す。前面で終わったら振動と表示で知らせ、予約の通知は取り消す |
| 画面の消灯 | 計測中に画面を表示している間は消灯しない(`IScreen.KeepScreenOn`) |
| モデル | `LapStopwatch`(停止 / 計測中 / 一時停止、ラップ、最速と最遅)と `CountdownTimer`(停止 / 計測中 / 一時停止 / 終了、残り時間)。どちらも現在時刻を引数で受け取り、状態の書き出しと読み込みを持つ |

| ファイル | 変更 |
| --- | --- |
| `Models/App/LapStopwatch.cs` / `CountdownTimer.cs` / `AppJsonContext.cs` | 新規 |
| `Modules/App/AppTimerView.xaml` + `AppTimerViewModel.cs` | 新規 |
| `State/Settings.cs` | タイマーの状態 |
| `State/Session.cs` | 前面・背面の変化の通知(表示の更新を背面で止める) |
| `Modules/ViewId.cs` / `Modules/App/AppMenuView.xaml` / `Markup/AppIcons.cs` | 画面の ID、メニュー(3 行目)、アイコン |
| `MauiProgram.cs` | `TimeProvider` の登録 |

確認: 背面に回して戻っても時間が合う。アプリを終了して開き直すと続きから。背面で終わると通知が出て、一時停止・リセットで予約が消える。前面で終わると画面で知らせ、通知は残らない。ラップの最速・最遅の色分け。

## ✅7-2 ToDo

| 項目 | 内容 |
| --- | --- |
| 一覧 | 期限で分けたグループ(期限切れ / 今日 / 明日 / 以降 / 期限なし)の一覧。行はチェック(タップで完了の切り替え)・件名・期限のバッジ(期限切れは赤)。F3 = 完了した行の表示 / 非表示。一覧の上の入力欄で件名だけの追加。行を左へスワイプ(`SwipeView`)で削除し、一覧の下に「元に戻す」の帯を数秒出す |
| 編集 | 件名(必須、100 文字)、メモ(複数行)、期限(日付と「期限なし」のスイッチ)、完了。`[DialogView]` を付けた画面(Navigation > Effect の Dialog と同じ開き方と閉じ方)で、行のタップと F4(新規)から `PushAsync` で開く。F4 = 保存は入力の結果を付けて `PopAsync`、Back は結果なしの `PopAsync`(UI > Grid の列の設定と同じ形)。件名が空なら保存できない |
| 保存 | 編集の画面は DB に触れず、戻り(`IsRestore()`)で結果を受け取った一覧が DB に保存して行を差し替える。削除はすぐに DB から消し、元に戻すで同じ行を入れ直す |
| DB | 既存の `data.db` に表 `Todo` を足す(`DataAccessor` / `DataService` に操作を足す。Navigation > Edit の `Work` と同じ形)。起動時の作り直しで、期限の区分けがそろう見本の行(期限切れ / 今日 / 明日 / 以降 / 期限なし / 完了)を入れる |
| モデル | 期限の区分け(期限切れ / 今日 / 明日 / 以降 / 期限なし)を日付から決める関数 |

| ファイル | 変更 |
| --- | --- |
| `Models/Entity/TodoEntity.cs` | 新規。`[Name("Todo")]`(Id / 件名 / メモ / 期限 / 完了 / 作成日時 / 更新日時) |
| `Services/DataAccessor.cs` + `Services/Sql/DataAccessor.*.sql` | 表の作成(`CreateTables.sql`)に `Todo` を足す。一覧、追加、更新、完了の切り替え、削除 |
| `Services/DataService.cs` | ToDo の操作。作り直しで見本の行を入れる |
| `Models/App/TodoDue.cs` | 新規。期限の区分け |
| `Models/Input/TodoInput.cs` | 新規。編集に渡す値と戻りの結果 |
| `Modules/App/AppTodoView.xaml` + `AppTodoViewModel.cs` | 新規。一覧 |
| `Modules/App/AppTodoEditView.xaml(.cs)` + `AppTodoEditViewModel.cs` | 新規。編集(`[DialogView]`) |
| `Modules/Parameters.cs` | 編集に渡す値と、戻りで受け取る結果 |
| `Modules/ViewId.cs` / `Modules/App/AppMenuView.xaml` / `Markup/AppIcons.cs` | 画面の ID(2 つ)、メニュー(4 行目)、アイコン |

確認: 追加・編集・完了・削除が一覧と DB に反映される。元に戻すで行が戻る。編集の Back では一覧が変わらない。期限のグループと期限切れの色。件名が空だと保存できない。

## 🔢7-3 2048

| 項目 | 内容 |
| --- | --- |
| 画面 | スコアとベストスコア、4×4 の盤面、詰みと 2048 の到達の表示(到達の後も続けられる)。F4 = 新しいゲーム |
| 操作 | 盤面の上下左右のスワイプ(`SwipeGestureRecognizer`) |
| アニメーション | タイルは Id を持ち、行・列が変わった前後の位置の差を `TranslationX/Y` で打ち消してから 0 へ動かす(`Behaviors/AnimationOption` に添付プロパティを足す)。合体したタイルは動いた後に弾ませ(`AnimationOption.BounceTrigger`)、新しいタイルは拡大して出す |
| 保存 | 1 手ごとに盤面とスコアを `Settings` に保存し、開き直すと続きから。ベストスコア |
| モデル | `Game2048`: 盤面(タイルの Id・値・位置)、1 手の結果(動いたタイル・合体・新しいタイル・加点)、動けない方向では何もしない、詰みと 2048 の到達、新しいタイル(9 割が 2、1 割が 4)。状態の書き出しと読み込み |

| ファイル | 変更 |
| --- | --- |
| `Models/App/Game2048.cs` | 新規 |
| `Modules/App/App2048View.xaml` + `App2048ViewModel.cs` | 新規 |
| `Behaviors/AnimationOption.cs` | 位置の変化を動かす添付プロパティ |
| `State/Settings.cs` / `Models/App/AppJsonContext.cs` | 盤面・スコア・ベストスコア |
| `Modules/ViewId.cs` / `Modules/App/AppMenuView.xaml` / `Markup/AppIcons.cs` | 画面の ID、メニュー(5 行目)、アイコン |

確認: 4 方向で動く。1 列の合体は 1 回ずつ(`2 2 2 2` → `4 4`)。動けない方向では新しいタイルが出ない。詰みの表示。アプリを終了しても続きから。アニメーションの途中で続けてスワイプしても表示が崩れない。

## 💣7-4 マインスイーパー

| 項目 | 内容 |
| --- | --- |
| 画面 | 残りの地雷の数、経過時間、状態の顔(😀 / 😵 / 😎)、盤面。F2 = 難易度(初級 9×9・10 個 / 中級 16×16・40 個)、F3 = 旗のモード(タップで旗)、F4 = 新しいゲーム |
| 盤面 | セルごとの要素を作らず、1 つのコントロールで描く(`Controls/MineBoard`: `GraphicsView` の派生。`Controls/MixerKnob` と同じく `StartInteraction` / `EndInteraction` でタップと長押しを見分け、行と列をコマンドで渡す)。VM が操作のたびに版の番号を上げて描き直させる |
| 操作 | タップで開く、長押しで旗、開いた数字のタップで周りを開く(周りの旗の数が数字と同じとき) |
| 保存 | 難易度ごとのベストタイム(`Settings`) |
| モデル | `MinesweeperGame`: 最初に開いたマスとその周りを避けて地雷を置く、0 のマスが続く範囲をまとめて開く(幅優先)、旗、勝ち(地雷以外をすべて開く)と負け(すべての地雷を見せる)、残りの数。経過時間は最初の 1 手からで、時刻は引数で受け取る |

| ファイル | 変更 |
| --- | --- |
| `Models/App/MinesweeperGame.cs` | 新規 |
| `Controls/MineBoard.cs` | 新規。盤面の描画と入力 |
| `Modules/App/AppMinesweeperView.xaml` + `AppMinesweeperViewModel.cs` | 新規 |
| `State/Settings.cs` | ベストタイム |
| `Modules/ViewId.cs` / `Modules/App/AppMenuView.xaml` / `Markup/AppIcons.cs` | 画面の ID、メニュー(6 行目)、アイコン |

確認: 最初の 1 手で地雷を踏まない。0 の連鎖。長押しと旗のモードで旗が立つ。周りを開く。勝ち・負けの表示。中級の盤面でも操作が遅れない。ベストタイム。

## 🌤️7-5 天気(UI)

採用は 7-5-0 で判断する。UI 2 のメニューの 9 行目(拡張用の空き。`Grid.Row="8" Grid.Column="0"`)に、ダミーのデータで天気の画面を足す。

| 項目 | 内容 |
| --- | --- |
| 現在 | 大きな気温、天気のアイコン、体感・最高・最低。天気と時間帯で変わる背景のグラデーション |
| 時間ごと | 24 時間の横スクロールの一覧に、項目をまたいでつながる気温の折れ線(各項目が前後の値を受け取り、自分の区間を描く)と降水確率 |
| 週間 | 7 日の一覧に、最低・最高の幅を週全体の範囲に合わせたバー |
| 詳細 | 湿度、風(向きの矢印を角度で回す)、UV の段階、日の出・日の入りの弧(今の位置) |
| 更新 | 引っ張って更新で、ダミーのデータを作り直す |
| 既存の強化 | 一覧の項目をまたぐ描画と、全体の範囲に合わせたバーは既存の UI に無い |

| ファイル | 変更 |
| --- | --- |
| `Models/Sample/WeatherInfo.cs` | 新規。現在・時間ごと・週間の値と、ダミーのデータの作成 |
| `Controls/SeriesSegmentView.cs` / `SunArcView.cs` | 新規。折れ線の区間と、日の出・日の入りの弧(`GraphicsView` の描画) |
| `Modules/UI/UIWeatherView.xaml` + `UIWeatherViewModel.cs` | 新規 |
| `Modules/ViewId.cs` / `Modules/UI/UIMenu2View.xaml` / `Markup/AppIcons.cs` | 画面の ID、メニュー、アイコン(WbSunny) |

## 📰7-6 ニュース(Control のタブ)

採用は 7-6-0 で判断する。Control のメニューの Refresh の隣(`Grid.Row="1" Grid.Column="1"`)に、ダミーのニュースでタブの画面を足す。

| 項目 | 内容 |
| --- | --- |
| タブ | カテゴリ(総合 / 経済 / 技術 / スポーツ / エンタメ)。見出しのタップと左右のスワイプで切り替える |
| 比べ方 | `SfTabView`(Syncfusion.Maui.Toolkit)と、自作(横に並ぶ見出し + 選択の下線の移動 + `CarouselView` のページ)を F2 = Sf / F3 = Custom で切り替える(Bottom Sheet と同じ比べ方) |
| ページ | 引っ張って更新する記事の一覧(`RefreshView`)。記事のタップで詳細を `PushAsync` で開き、`PopAsync` で戻ったときに選んでいたタブと一覧の位置がそのまま残る |
| 既存の強化 | 上部のタブと左右のスワイプでページを切り替える部品は Control に無い(Shell のタブを使わない構成で要る) |

| ファイル | 変更 |
| --- | --- |
| `Models/Sample/NewsItem.cs` | カテゴリと本文を足す(Refresh の一覧と共用) |
| `Controls/TabStrip.cs` | 新規。自作の見出し(選択の下線の移動) |
| `Modules/Control/ControlTabView.xaml` + `ControlTabViewModel.cs` | 新規 |
| `Modules/Control/ControlTabDetailView.xaml` + `ControlTabDetailViewModel.cs` | 新規。記事の詳細 |
| `Modules/ViewId.cs` / `Modules/Control/ControlMenuView.xaml` / `Markup/AppIcons.cs` | 画面の ID(2 つ)、メニュー、アイコン(Tab) |

## ⚠️制約

- タイマー: 正確なアラームの許可(`SCHEDULE_EXACT_ALARM`。Android 14 以降は既定で不許可)が無いと、終了の通知が遅れることがある。時間は端末の時計(UTC)から計算するので、計測中に端末の時刻を変えるとずれる
- ToDo: `data.db` は起動のたびに作り直すので、ToDo の行もアプリの起動で見本の行に戻る。`IDialog.Snackbar` はボタンを持てないので、元に戻すは画面の中の帯で出す
- 2048: 縦の `ScrollView` の中ではスワイプがスクロールと競合するので、盤面は `ScrollView` の外に置く
- マインスイーパー: 中級(16×16)はセルが小さい(幅 360dp の画面で約 20dp)

## 🚫対象外

| 項目 | 内容 |
| --- | --- |
| タイマー | 複数のタイマーの同時実行、時刻を指定するアラーム |
| ToDo | 繰り返し、期限の通知、ドラッグでの並べ替え、サーバーとの同期 |
| 2048 | 取り消し、盤面の大きさの変更 |
| マインスイーパー | 上級(16×30)、盤面の拡大・縮小 |
| 天気・ニュース | 実際のサービスからの取得(データはダミー) |
