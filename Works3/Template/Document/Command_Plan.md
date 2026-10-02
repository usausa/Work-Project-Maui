# 🚦コマンドの受け付けの見直し

画面の遷移でアクティブ・非アクティブにするタイミング(Smart.Navigation)と、コントロールからの通知のコマンドの種類(`CommandMode.Simple` / 既定の `Standard`)を、あわせて見直す。`Task_Checklist.md` の 8 節から参照する。Simple の一覧と理由は、この見直しが終わるまで、コマンドを変えるたびに更新する。

## 🎯方針

Smart.Navigation 3.15.0 と Smart.Maui 2.31.0 で入れた。Smart.Maui 2.32.0 で、`CommandMode` を `MakeDelegateCommand` / `MakeAsyncCommand` の第 1 引数にし(省略すると既定)、`AcceptsOperation` を `AcceptsCommand` に改名した。

- Smart.Maui: Simple のコマンドもアクティブ(受け付け)を見る。Busy は見ない
- Smart.Navigation: 遷移イベントとアクティブをそろえる。遷移イベントが起きるときは、その画面はアクティブになっている
- Simple がアクティブを見ても、遷移の途中に来るコントロールの通知が捨てられない形にする

## 📖用語

| 用語 | 意味 |
|---|---|
| アクティブ | 画面の VM(`AppViewModelBase`)の `AcceptsCommand` が true。Smart.Navigation の `INavigationLifecycleSupport`(`OnActivated` / `OnDeactivated`)で切り替える |
| Busy | 共有の `BusyState`。立っている間は `MainPage` の全画面のオーバーレイがタップを止める |
| Standard | `MakeXxxCommand` の既定。アクティブで Busy でないときだけ実行し、実行中は Busy を立てる |
| Simple | アクティブのときだけ実行し、Busy は見ず立てもしない(CanExecute の更新の対象にはする)。`MakeDelegateCommand(CommandMode.Simple, execute)` の形で作る |

## ⏱️遷移の順番とアクティブ(Smart.Navigation 3.15.0)

| 順 | 処理 | 旧画面 | 新画面 |
|---|---|---|---|
| 1 | 遷移の確認(`ConfirmNavigation`) | アクティブ | — |
| 2 | `Executing` を true → 旧画面の `OnDeactivated` | 非アクティブ | — |
| 3 | 新画面の解決(新しい画面は VM を作る。`AppViewModelBase` は作るときに非アクティブ) | 非アクティブ | 非アクティブ |
| 4 | 旧画面の `OnNavigatingFrom` / `OnNavigatingFromAsync` | 非アクティブ | 非アクティブ |
| 5 | 新画面の `OnActivated` → `OnNavigatingTo` / `OnNavigatingToAsync` | 非アクティブ | アクティブ |
| 6 | `UpdateStack`(新画面のビューを付ける。コントロールの初回の通知はここで来る) | 閉じる(Push は残す) | アクティブ |
| 7 | 新画面の `OnNavigatedTo` / `OnNavigatedToAsync` | — | アクティブ |
| 8 | `Executing` を false。5 の後で 6 の前に失敗したときは、新画面を非アクティブにし、旧画面をアクティブに戻す | — | アクティブ |

- 画面がアクティブなのは、自分の `OnNavigatingTo` の前から、次の遷移で自分の `OnNavigatingFrom` の前まで。Push で下になる画面は `OnNavigatingFrom` の前に非アクティブになり、Pop で戻る画面は `OnNavigatingTo` の前にアクティブになる
- 遷移の間は Busy: `MainPageViewModel` が `Navigator.ExecutingChanged` で、遷移の間(2〜8)Busy を立てる。コマンド(メニューのボタン・F キー・画面のコマンド)から始めた遷移は、コマンドの Busy(1〜8 とその前後)と重なって `BusyState` の数が 2 になる(`IsBusy` は途中で切れず、変更の通知は最初と最後の 2 回だけ)。新画面の初期化(5〜7)の間の利用者の操作を止める

## ⚠️問題になりうるケースと確認の結果

| 番号 | ケース | 動き | 確認 |
|---|---|---|---|
| P1 | 新画面の初期化(5〜7)の間の利用者の操作 | 遷移の間の Busy(`Navigator.ExecutingChanged`)で止まる | 実機: 戻るキーで UI 1 へ戻る遷移の初期化の間(一時的に 1.5 秒の待ちを入れて確認し、確認後に外した)の F1・メニューのボタン・2 回目の戻るキーは届かない。メニューのボタンのダブルタップ(0.15 秒)の 2 回目も止まる。例外なし |
| P2 | 新画面の初回のコントロールの通知(6) | アクティブなので Simple は受ける。Standard は遷移を始めたコマンドの Busy で捨てる | 実機: Calendar の初回は Simple で実行(アクティブ・Busy)。Carousel の初回は Standard で捨てる(VM が先に印を付けているので変わらない) |
| P3 | 新画面の `OnNavigatingTo` の前に来る通知 | 非アクティブなので捨てる | 実機: 該当なし |
| P4 | 離れる途中の旧画面の通知 | 非アクティブなので捨てる | 実機: Behavior のフォーカスの出入りと Carousel の現在の項目(どちらも Standard、捨てても変わらない)。Simple は該当なし |
| P5 | Push で下になった画面の通知 | 非アクティブなので捨てる。Pop で戻っても送り直されない | 実機: 該当なし(News → 記事) |
| P6 | Pop で戻る画面 | `OnNavigatingTo` の前にアクティブ | 実機: 記事 → News で、News は遷移の終わりの前にアクティブ |
| P7 | 遷移の失敗(例外) | 6 の前の失敗は新画面を非アクティブにして旧画面を戻す。6 の後の失敗は新画面のまま | Smart.Navigation のテスト(`NavigatorActivationTests`) |
| P8 | 新画面の初期化やほかのコマンドの実行の中で、Busy の間に来る通知 | Standard は捨てる。Simple は受ける | 実機: Calendar の初回と「今日へ」、記事の読んだ量(関連記事の切り替え)を Simple で実行 |
| P9 | ポップアップの VM(`AppDialogViewModelBase`)と `MainPageViewModel` | 遷移の対象ではなく、受け付けは常に true | — |

## 📌Simple の基準

コントロールからの通知のうち、次のどれかに当たるものだけ Simple にし、ほかは Standard にする。

| 記号 | 理由 | Standard にすると |
|---|---|---|
| A | 遷移を始めたコマンドの Busy の間(新画面の初期化の途中)に来る | 捨てられ、VM が表示とずれる |
| B | ほかのコマンドの実行の中(そのコマンドの Busy の間)に来る | 捨てられ、VM が表示とずれる |
| C | 1 回の操作の間や画面を開いている間、短い間隔で続けて来る(カメラのフレーム・スクロール・ドラッグ・アニメーション) | 動きは変わらない(Busy で捨てられても次がすぐ来る)。ただし 1 件ごとに Busy が立って下りるので、そのたびに `MainPage` のオーバーレイの表示の切り替えと、画面と `MainPageViewModel` の全コマンドの `CanExecuteChanged` が 2 回ずつ動く |
| D | 同じ操作の一連の通知で、ほかの通知が A・B・C に当たる(DragDrop の開始・重なり・解除・ドロップ・終了) | 動きは変わらない(どれも利用者のドラッグの間だけ来て、Busy と重ならない)。同じ操作の通知を同じ扱いにそろえる |

- Simple もアクティブを見るので、離れる途中や Push で下の画面への通知は Simple でも捨てる。非アクティブの間も受けたい通知が出てきたら、Make 系ではなく素のコマンドにするか、Pop で戻ったときに VM で取り直す
- Simple は Busy を見ないので、Simple のコマンドからは遷移もダイアログも出さない

## 📋Simple のコマンド(19 件)

| 画面 | 現在のファイル名 | コマンド | 作り方 | 何用か | 理由 | 確認したこと(実機) |
|---|---|---|---|---|---|---|
| UI 1 > Calendar | `Modules/UI/UICalendarViewModel.cs` | `DisplayDateChangedCommand` | `MakeDelegateCommand`(同期) | 表示範囲の予定・スタンプ・祝日を読む | A・B | 開く途中の初回はアクティブで Busy(遷移を始めたコマンドの Busy)。「今日へ」(`GoToTodayCommand`)の実行の中に Busy で来る。▶(コントロールの月送り)はアクティブで Busy なし |
| UI 1 > News の記事 | `Modules/UI/UINewsDetailViewModel.cs` | `ProgressCommand` | `MakeDelegateCommand`(同期) | 読んだ量のバー(`Scroll.RatioCommand`) | B・C | 関連記事の切り替え(`RelatedCommand`)の実行の中に Busy で来る。スクロールのたびに来る |
| UI 1 > News | `Modules/UI/UINewsViewModel.cs` | `MoreCommand` | `MakeDelegateCommand`(`_ = LoadMoreAsync(x)` で非同期の処理を始めるだけ) | 続きの読み込み | C | 残りが少ない所では、スクロールのたびに来る |
| Network > HTTP | `Modules/Network/NetworkHttpViewModel.cs` | `LoadMoreCommand` | `MakeDelegateCommand`(`_ = LoadMoreAsync()` で非同期の処理を始めるだけ) | 続きの読み込み | C | 未確認(サーバーが要る)。News・Collection と同じ通知 |
| Control > Collection | `Modules/Control/ControlCollectionViewModel.cs` | `LoadMoreCommand` | `MakeDelegateCommand`(同期) | 続きの読み込み | C | 残りが少ない所では、スクロールのたびに来る |
| View > Lottie | `Modules/View/ViewLottieViewModel.cs` | `ScrubCommand` | `MakeDelegateCommand`(同期) | 横スクロールの割合と長押しの進行で再生位置を動かす | C | 長押しは 16 ms ごと(`AnimationOption.HoldCommand` のアニメーション)、スクロールはスクロールのたび |
| View > DragDrop | `Modules/View/ViewDragDropViewModel.cs` | `ItemOverCommand` / `ListOverCommand` / `TrashOverCommand` | `MakeDelegateCommand`(同期) | ドラッグ中の重なりの表示 | C | 1〜8 ms ごと(1 回のドラッグで 250 回ほど) |
| View > DragDrop | `Modules/View/ViewDragDropViewModel.cs` | `DragStartingCommand` / `ItemLeaveCommand` / `DropOnItemCommand` / `ListLeaveCommand` / `DropOnListCommand` / `TrashLeaveCommand` / `DropOnTrashCommand` / `DropCompletedCommand` | `MakeDelegateCommand`(同期) | ドラッグの開始・終了、重なりの解除、ドロップでの移動と削除 | D | 開始・重なりの解除・ドロップは 1 回ずつで、アクティブで Busy なし。ドロップの処理はドラッグの状態を自分で消す(項目を移したときは終了の通知が来ない)。Simple にした後、リスト間の移動・並べ替え・ゴミ箱を確認 |
| Main > Setting | `Modules/Main/SettingViewModel.cs` | `DetectCommand` | `MakeDelegateCommand`(`_ = DetectAsync(x)` で非同期の処理を始めるだけ) | 設定の QR を読む | C | 約 50 ms ごと(見つからないときも空の集合で来る)。遷移が終わった後から、アクティブで Busy なしで来る |
| Device > QR Scan | `Modules/Device/DeviceQrScanViewModel.cs` | `DetectCommand` | `MakeDelegateCommand`(同期) | 読み取りの表示 | C | 約 50 ms ごと |

## 📋Standard のコントロールの通知(13 件)

A・B・C・D のどれにも当たらないもの。

| 画面 | 現在のファイル名 | コマンド | 作り方 | 何用か | 確認したこと |
|---|---|---|---|---|---|
| Control > Carousel | `Modules/Control/ControlCarouselViewModel.cs` | `CurrentChangedCommand` | `MakeDelegateCommand`(同期) | 現在の項目の印 | 実機: 開く途中の初回は Busy、離れる途中は非アクティブで捨てる。初回の項目は VM が先に印を付けているので、捨てても表示は変わらない。スワイプはアクティブで Busy なし |
| UI 2 > Character | `Modules/UI/UICharacterViewModel.cs` | `SelectCommand` | `MakeDelegateCommand`(同期) | 選んだクラスの画像 | 実機: タップで 1 回、アクティブで来る |
| UI 1 > Grid | `Modules/UI/UIGridViewModel.cs` | `CellValueChangedCommand` | `MakeDelegateCommand`(同期) | 確認の列の結果 | ClamGrid は確認の列のタップでだけ出す |
| UI 1 > Grid の列の設定 | `Modules/UI/UIGridColumnViewModel.cs` | `CellValueChangedCommand` / `RowMovedCommand` | `MakeDelegateCommand`(同期) | 表示の切り替え・行の移動の結果 | ClamGrid はタップと行のドラッグの確定でだけ出す |
| Sample > PDF | `Modules/Sample/SamplePdfViewModel.cs` | `PageChangedCommand` | `MakeDelegateCommand`(同期) | ページの表示と前後のボタン | 実機: 開いた直後と F3 / F4 の後に、アクティブで Busy なしで来る(F キーのコマンドの実行の中ではない) |
| Basic > Behavior | `Modules/Basic/BasicBehaviorViewModel.cs` | `FocusedCommand` / `UnfocusedCommand` / `TypingStoppedCommand` / `SwitchToggledCommand` | `MakeDelegateCommand`(同期) | フォーカスの出入り・入力の停止・スイッチの表示 | 実機: 最初のフォーカスはアクティブで Busy なし。離れる途中のフォーカスの出入りは非アクティブで捨てる(捨てても変わらない)。入力の停止とスイッチは操作で 1 回 |
| Device > Audio | `Modules/Device/DeviceAudioViewModel.cs` | `SeekCommand` | `MakeDelegateCommand`(同期) | シークバーのドラッグの完了で再生位置を移す | ドラッグの完了で 1 回(Busy の計測はまだ) |
| Sample > Media | `Modules/Sample/SampleMediaViewModel.cs` | `SeekCommand` | `MakeDelegateCommand`(同期) | 同上 | 同上 |
| View > Lottie | `Modules/View/ViewLottieViewModel.cs` | `SeekCommand` | `MakeDelegateCommand`(同期) | 同上 | 同上 |

## 🔬検証の方法

- 一時的なログ(Smart.Maui の `ExtendViewModelBase` で、コマンドの実行のときの種類・`AcceptsCommand`・`BusyState.IsBusy` を受け付けの確認の前に記録し、`AppViewModelBase` で `OnActivated` / `OnDeactivated` を記録する。確認の後に外す)で、各画面を開く・操作する・離れるときの通知を記録する
- 遷移の途中の操作: 戻るキー → F キー / 戻るキー / タップ(新画面の `OnNavigatedToAsync` に一時的に待ちを入れる)、メニューのダブルタップ
- ドラッグは `adb shell input draganddrop`、QR の検出は一時的なコードで見立てる

## ✅項目

| 番号 | 内容 |
|---|---|
| 8-5 | Simple の一覧と理由の更新(見直しが終わるまで、コマンドを変えるたびに) |

## ⏳未確認

- HTTP の続きの読み込み(サーバーが要る)
- QR を写しての読み取り
- シーク(Audio / Media / Lottie)の、通知が来たときの Busy の計測
