# 🧱画面のモデルの見直し

画面の処理を、モデル(`Models/App` の Application / Game)と、画面どうしが共有する文脈クラス(`[Scope]`)へ寄せ、VM をメソッドの呼び出しとバインドだけの薄い層にする。あわせて、表示の文言をモデルから外し、1 つの機能に固有のコントロールに機能の名前を付ける。番号は `Task_Checklist.md` の 9 と同じ。

| 番号 | 対象 | 内容 |
| --- | --- | --- |
| 9-1 | Calc | 計算できなかった理由を種類と原因の文字列で返し、文言は VM で作る |
| 9-2 | Timer | `TimerApplication.cs` をストップウォッチとカウントダウンの区切りで分ける |
| 9-3 | Weather | 天気に固有のコントロールの名前に `Weather` を付ける |
| 9-4 | ToDo | 一覧と編集が共有する文脈クラス、行と一覧と編集の入力のモデル、Smart.Mapper |
| 9-5 | News | 一覧と記事が共有する文脈クラス |
| 9-6 | Timer | ラップの行と印、表示の値の計算をモデルへ |
| 9-7 | Sudoku | 数字のキーの行と残りの数をモデルへ |
| 9-8 | Minesweeper | タップと長押しの規則、負けたときに見せるマスをモデルへ |
| 9-9 | 2048 | 入力を受け付けるかと勝ちを見せるかの規則をモデルへ |
| 9-10 | Calc | 式の入力の組み立てをモデルへ |

ファイルパスは `Template.MobileApp/` からの相対。

## ⚖️決定事項

| 項目 | 決定 |
| --- | --- |
| モデル | アプリの状態と規則(並び・グループ分け・集計・入力の解釈・失敗の種類)を持つ。表示の文言は持たない。行のように画面へバインドする型も置いてよい |
| 文脈クラス | 一覧と、そこから開く画面(編集・記事)が共有する情報(画面寄りのモデル)。VM と同じ場所に置き、`[Scope]` のプロパティで受け取る(Navigation > Wizard の `WizardContext` と同じ形)。画面の間の受け渡し(`Parameters` の Model と Pop の結果)の代わりにし、保存などの処理も文脈が持つ。サービス(`DataService` / `TimeProvider` など)は参照してよいが、表示寄りの仕組み(`IDispatcher`、表示を消す・行を動かすまでの時間)と、片方の画面だけの状態(選んだタブ・スクロールの位置)は VM に置く |
| VM | 文脈とモデルのメソッドの呼び出しと、バインドだけ。表示の文言(エラーの文言など)は VM で作る |
| 行の型 | `XxxViewModel` と名付けない(`TodoItem`・`TimerLapItem`・`SudokuDigit` など) |
| 写し替え | 型の間の値の写し替えは Smart.Mapper(`[Mapper]`。使う側のファイルの `// Mapper` 区画) |
| コントロールの名前 | 1 つの機能に固有のものは機能の名前を頭に付ける(`WeatherMoonPhaseView`)。汎用のもの(`ArcMeter`・`RangeBar`・`SegmentBar`・`SeriesSegmentView`・`TabStrip`)は付けない |
| Timer のモデルのファイル | `TimerApplication.cs` の 1 つのまま、区切り(Stopwatch / Countdown)で分ける |

## 🧮9-1 Calc のエラー

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Models/App/CalcApplication.cs` | 電卓のモデル | `CalcErrorType`(空・数値が不正・未知の名前・未知の文字・括弧の対応・不完全・不正・0 で割る・負数の平方根・階乗の範囲・未知の演算子・未知の関数・計算できない)と `CalcError`(Smart.Results の `Error` の派生。種類 `Type` と原因の文字列 `Text`)。字句・逆ポーランド記法への変換・評価の各段は例外を投げずに `Result` で返す。`CalcException` は削除。階乗の上限は `CalcEngine.MaxFactorial` |
| `Modules/App/AppCalcViewModel.cs` | 電卓の画面 | `CalcError` の種類と原因から文言を作る |

完了(2026-10-02)。結果は `Change_Summary.md` の区間 17。

## ⏱️9-2 Timer の区切り

`Models/App/TimerApplication.cs` を、保存する状態(`TimerSnapshot`)の後で Stopwatch と Countdown の区切りに分ける。

完了(2026-10-02)。結果は `Change_Summary.md` の区間 17。

## 🌤️9-3 Weather のコントロールの名前

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Controls/WeatherCompassDial.cs` | 風の羅針盤 | `CompassDial` から改名 |
| `Controls/WeatherSunArcView.cs` | 日の出から日の入りまでの弧 | `SunArcView` から改名 |
| `Controls/WeatherMoonPhaseView.cs` | 月の満ち欠け | `MoonPhaseView` から改名 |
| `Modules/UI/UIWeatherView.xaml` | 天気の画面 | 新しい名前で使う |

完了(2026-10-02)。結果は `Change_Summary.md` の区間 17。

## ✅9-4 ToDo

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Models/App/TodoApplication.cs` | ToDo のモデル | 行 `TodoItem`(`AppTodoViewModel.cs` の `TodoItemViewModel` から)、グループの行のまとまり `TodoSection`(同 `TodoGroupViewModel` から)、一覧 `TodoList`(表示するグループ、完了の表示の切り替え、件数と完了の割合、行を置く・外す・動かす、並びの比べ方。VM の Helper から)、編集の入力 `TodoDraft`(期限の区分けとチップ、保存できるか、前後の空白を除く) |
| `Modules/App/AppTodoContext.cs`(新規) | 一覧と編集が共有する文脈 | 一覧(`TodoList`)、今日、編集の入力(`TodoDraft`)、消した行(元に戻せる間)。読み込み、完了と重要の切り替え、削除と元に戻す、編集の開始と保存(`DataService` / `TimeProvider`)。`TodoEntity` と `TodoItem`、`TodoItem` と `TodoDraft` の写し替えは Smart.Mapper(`AppTodoContextMapper`) |
| `Models/Input/TodoInput.cs` | 編集の受け渡し | 削除(`TodoInput` / `TodoEditAction` / `TodoEditResult`) |
| `Modules/App/AppTodoViewModel.cs` | 一覧 | `[Scope]` の文脈のメソッドの呼び出しとバインド。表示の動き(完了の後に動きを見せてから行を動かす、元に戻すの帯を時間で閉じる)は VM(`IDispatcher` / タイマー)。編集は文脈で始めてから Push |
| `Modules/App/AppTodoEditViewModel.cs` | 編集 | 文脈の編集の入力にバインドし、保存・削除は文脈のメソッドを呼んでから Pop。保存のボタンは `TodoDraft.CanSave` で有効にする |
| `Modules/App/AppTodoView.xaml` / `AppTodoEditView.xaml` | 画面 | バインドのパスと `x:DataType` |

確認: 一覧の表示(グループと件数)、追加・編集・削除と元に戻す、完了と重要の切り替え、完了の表示の切り替え、編集から戻ったときの一覧の位置。

完了(2026-10-02)。結果は `Change_Summary.md` の区間 17。

## 📰9-5 News

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Modules/UI/UINewsContext.cs`(新規) | 一覧と記事が共有する文脈 | ページ `UINewsPage` と行 `UINewsItem`(`UINewsViewModel.cs` から)、速報、ニュースの見本(`NewsFeed`)、開いている記事と関連記事。ページの読み込み・引っ張って更新・続きの読み込み、記事を開く・関連記事へ切り替える(一覧の同じ記事を既読に)、保存の切り替え(保存した記事のページへすぐに反映) |
| `Modules/UI/UINewsViewModel.cs` | 一覧 | 選んだタブとページの位置(画面だけの状態)、選んだページの読み込みと、増えた数を時間で消す(`IDispatcher`)。ほかは文脈のメソッドの呼び出し |
| `Modules/UI/UINewsDetailViewModel.cs` | 記事 | 文字の大きさと読んだ量(画面の状態)と、文脈のメソッドの呼び出しだけ。`UINewsDetailInput` / `UINewsDetailResult` は削除 |
| `Modules/UI/UINewsView.xaml` / `UINewsDetailView.xaml` | 画面 | バインドのパスと `x:DataType` |

確認: タブの切り替えと読み込み、引っ張って更新、続きの読み込み、記事と関連記事の既読、保存した記事のページ。

完了(2026-10-02)。結果は `Change_Summary.md` の区間 17。

## ⏱️9-6 Timer のラップと表示の値

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Models/App/TimerApplication.cs` | Timer のモデル | 画面の切り替え `TimerMode`(`AppTimerViewModel.cs` から)。ラップの行 `TimerLapItem`(`TimerLapViewModel` から)とラップの一覧 `TimerLapList`(新しいラップを先頭に足す、最速・最遅の印と一番遅いラップに対する比を付け直す、作り直す)。1 分で 1 周の進み(`TimerStopwatch.GetMinuteProgress`)、残りの割合と警告の判定(`TimerCountdown.GetProgress` / `IsWarning`) |
| `Modules/App/AppTimerViewModel.cs` / `AppTimerView.xaml` | Timer の画面 | モデルの呼び出しとバインドだけ |

確認: ラップの印と比、計測中の表示、カウントダウンの警告と終了。

完了(2026-10-02)。結果は `Change_Summary.md` の区間 17。

## 🧩9-7 Sudoku の数字のキー

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Models/App/SudokuGame.cs` | 数独のモデル | 数字のキーの行 `SudokuDigit`(`AppSudokuViewModel.cs` の `SudokuDigitViewModel` から)と、キーの作成と残りの数の付け直し(`CreateDigits` / `UpdateDigits`) |
| `Modules/App/AppSudokuViewModel.cs` / `AppSudokuView.xaml` | 数独の画面 | モデルの呼び出しとバインドだけ |

確認: 数字を入れる・消す・元に戻す・ヒントでのキーの残りの数。

完了(2026-10-02)。結果は `Change_Summary.md` の区間 17。

## 💣9-8 Minesweeper の操作の規則

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Models/App/MinesweeperGame.cs` | マインスイーパーのモデル | タップ(開く。旗のモードなら旗、開いた数字なら周りを開く)と長押し(旗。開いた数字なら周りを開く)の規則(`Tap` / `LongPress`)と、1 手の結果 `MinesweeperMove`(開いた・旗。負けたときに見せるマスは地雷を近い順) |
| `Modules/App/AppMinesweeperViewModel.cs` | マインスイーパーの画面 | 振動・保存・表示の更新だけ |

確認: タップ・旗のモード・長押し・周りを開く、負けと勝ち。

完了(2026-10-02)。結果は `Change_Summary.md` の区間 17。

## 🔢9-9 2048 の規則

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Models/App/Puzzle2048Game.cs` | 2048 のモデル | 結果を出している間は動かさない規則と、勝ちを見せるか(`IsWinPending`。続けて遊ぶなら見せない) |
| `Modules/App/AppPuzzle2048ViewModel.cs` | 2048 の画面 | モデルの呼び出しとバインドだけ |

確認: スワイプ、勝ちの後に続ける、詰み。

完了(2026-10-02)。結果は `Change_Summary.md` の区間 17。

## 🧮9-10 Calc の式の入力

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Models/App/CalcApplication.cs` | 電卓のモデル | 式の入力 `CalcInput`(計算の後に演算子で続けると前の結果から、ほかは新しい式、1 文字消す、全部消す、直前の結果、値の書き方 `Format`) |
| `Modules/App/AppCalcViewModel.cs` | 電卓の画面 | 表示の値の書式とエラーの文言だけ |

確認: 続けての計算、1 文字消す、全部消す、エラーの表示。

完了(2026-10-02)。結果は `Change_Summary.md` の区間 17。
