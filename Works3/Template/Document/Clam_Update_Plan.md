# 🐚Clam 系 3 種の更新と SkiaExtension の導入の計画

ClamCalendar・ClamGrid を最新版(2.1.0)に上げ、自前の Gauge を ClamGauge に置き換え、SkiaExtension 1.2.0 を入れて SkiaSharp の描画を見直す。ファイルパスは `Template.MobileApp/` からの相対。行番号は 2026-10-12 時点の目安。

## 🔖凡例

| 印 | 意味 |
| --- | --- |
| `- [x]` / `- [ ]` | 済み / 未着手 |
| ✅ / 🚧 / 📝 | 段階の状態: 完了 / 作業中 / 未着手 |
| ⚖️【判断】 | ユーザーが決める項目(決まるまで着手しない) |
| 💥 | ライブラリ側の破壊的変更(Template の修正が要る) |

## 🗂️対象

| パッケージ | 今 | 上げる版 | リポジトリ | 読む資料 |
| --- | --- | --- | --- | --- |
| ClamCalendar | 1.0.0 | 2.1.0(P5 のサイズ系の `double` 化と SkiaExtension 1.2.0 を含む) | `D:\GitHub\Other-ClamCalendar` | `README.md`・`Document/API.md` |
| ClamGrid | 1.1.0 | 2.1.0(同上) | `D:\GitHub\Other-ClamGrid` | `README.md`・`Document/API.md` |
| ClamGauge | なし(`Controls/Gauge.cs`) | 2.1.0 | `D:\GitHub\Other-ClamGauge` | `README.md`・`Document/API.md`・`Example/Modules/Gauge` |
| SkiaExtension | なし | 1.2.0 | `D:\GitHub\Other-SkiaExtension` | `README.md`・`Document/API.md` |

- 2026-10-12 17:00 の時点で、ClamCalendar・ClamGrid・ClamGauge 2.0.0 と SkiaExtension 1.1.0 は公開済み。上の表の次の版と SkiaExtension 1.2.0 はユーザーが公開する。各ステップの着手前に nuget.org(`https://api.nuget.org/v3-flatcontainer/<小文字の ID>/index.json`)で公開を確かめ、公開前ならユーザーに確認する
- Clam・SkiaExtension のソースはこの作業では変えない。ライブラリ側の問題を見つけたら、直さずに報告する

## 📏運用ルール

- 下の「🪜ステップ」の順に進め、1 ステップが終わるたびに報告して止まる。ユーザーが確かめてコミットする
- コミットと版上げはユーザーが行う(勝手にしない)
- ⚖️【判断】の項目は、変更箇所・破壊的変更の有無・効果・代替案を揃えて 1 件ずつ提示し、決まってから着手する
- 完了の条件: Release・Debug のビルドで警告 0、ReSharper の警告 0(`jb inspectcode`)、変えた XAML が `xstyler -p` に合格、実機で確認、`Document/Change_Summary.md` に記録
- ReSharper の検査は、2〜7 で変えたファイルを 8 でまとめて行う(ステップごとには行わない)
- ビルドと実機の確認(割り当ての測定を含む)は、変更の都度ではなく、一通り直してからまとめて行う
- 警告の抑制が要るときは、先にユーザーに確認する
- 書き方
  - 説明のコメントを足さない(`Controls/` の `//---` の区切りは既存の流儀どおり)
  - 引数の null チェック(`ArgumentNullException.ThrowIfNull`)を足さない
  - `using` で破棄するオブジェクトにオブジェクト初期化子を使わない(生成の後に代入する)
  - `&&`・`||` の項が比較か `is` のときは括弧で囲む(`if ((count == 1) && flag)`)
  - 割り当てを避けるために `foreach` を `for` にしたループには `// ReSharper disable once ForCanBeConvertedToForeach` を付ける
  - `Controls/` の並び順と定数の置き方は `Document/Change_Summary.md` の付録 A に従う
- 改行は CRLF。Write ツールは LF で書くので、新しいファイルは CRLF に直す。既存のファイルは Edit で直す(改行が保たれる)。Git Bash の `sed -i` は CRLF を壊すので使わない。直した後は perl の raw 読みで LF が混ざっていないかを数える
- Write・Edit の引数に書いたユニコードのエスケープ(バックスラッシュ + `u` + 16 進 4 桁)は実際の文字に変わる。エスケープを書くときは perl で `\x5C` を使う
- 実機(Pixel 9a)は別のセッションと共用する
  - 操作の前に `adb logcat -d -v time -s adbd:I | grep 'service requested'` で他のセッションの操作がないこと、`adb shell dumpsys activity activities | grep topResumedActivity` で前面のアプリを確かめる
  - 前面にあった他のアプリは、終わったら `adb shell monkey -p <パッケージ> -c android.intent.category.LAUNCHER 1` で既存のタスクのまま戻す
  - Git Bash では `export MSYS_NO_PATHCONV=1` を付ける
- Gauge の置き換えは `Document/Control_Extraction_Plan.md` の ④⑤ でもある。そちらの状態も同時に直す
- README の「My Libraries」の表(ClamGauge・SkiaExtension を足す)と TODO の表(コントロールの抽出の行)を、作業に合わせて直す

## ⚖️決定事項

| 項目 | 決定 |
| --- | --- |
| 破壊的変更 | ライブラリの破壊的変更(💥)は、Template を新しい API に合わせて直す。旧名の別名やラッパーなどの互換の層は作らない |
| Gauge のサンプル | ClamGauge の Example(`D:\GitHub\Other-ClamGauge\Example\Modules\Gauge`)の 6 種類(Pressure・Humidity・Temperature・Wind・Speed・RPM)と同じ内容にする。画面は分けず、1 ページの中でタブのように切り替える |
| 割り当ての対象 | SkiaSharp の API の内部の割り当ては対象外(`DrawText(string …)` が毎回作る `SKTextBlob`、`SKObject` のロック用オブジェクト、`SKPaint`・`SKFont`・`SKShader`・`SKPath` の生成、配列しか取らない API に渡す配列)。自前のコードの割り当てを直す(列挙子のボックス化・描画中の LINQ・毎フレームの配列や `List`・クロージャーやデリゲートの生成・値のボックス化・入力が変わらない文字列の組み立て) |
| 文字列の保持 | 毎フレーム作る文字列は、入力が不変のものだけ保持する。結果が変わりうるもの(利用者のフォーマッター・コンバーター、時刻など)は毎回作る |
| 書式 | 描画のたびにプロパティから実際の書式を決めない。解析・選択した書式はプロパティとは別に保持し、プロパティが変わったときに作り直す |
| 言語のヒント | `MauiProgram.cs` の `ConfigureGlobalSettings` で `FallbackFonts.Languages = ["ja"]` を設定する(前の `CalendarFonts`・`GridFonts` の既定と同じ。端末の言語に関係なく漢字を日本語の字形で描く) |
| 和文の書体 | `MatchCharacter('睡' / 'あ')` で選んだ書体で描いていた KitSleepBand・TextToggle・MailAvatarImage は、SkiaExtension の `FontResolver("sans-serif")` と `DrawableText` で描く(漢字は日本語の字形、数字・英字は Roboto) |
| 戻す基準 | 割り当てを減らすために `for` にした所・キャッシュにした所のうち、効果の無かったものは戻す。SkiaExtension を使うようにした所と、ユーザーの別の指示による変更は残す |
| Gauge のサンプルの画面 | 種類の切り替えは `mcc:SegmentedView`(幅が足りないので略称)。種類ごとの切り替えは F2〜F4 を選んだ種類に合わせて替える(Example と同じ操作。F キーの文字も種類で変わる)。色の候補はスライダーの下の 1 行(Pressure・Humidity・Temperature)。ViewModel は `UIGaugeViewModel` 1 つに種類ごとの個々のプロパティ。メニューとページの名前は今のまま(UI 2 の「Gauge」、`UIGaugeView`) |
| 描画で回す自前の一覧 | 盤面の状態や Scene のシミュレーションのように、自分で作って持つ一覧は、配列か `List<T>` にして `foreach` で回す(`IReadOnlyList<T>` の `foreach` は列挙子がボックス化される)。public の型の CA1819・CA1002 は `#pragma` で抑止する(internal の型では出ないので付けない) |
| 外から受け取る一覧 | バインドするプロパティと、描画の部品に外から設定する一覧は配列にしない(`List<T>` や `ObservableCollection<T>` も渡せるように)。一般的なコントロールと同じくインターフェイスで受け、描画で回すので `IReadOnlyList<T>` にして `for` で回す。バインドするプロパティは、値が `INotifyCollectionChanged` なら変更のたびに描き直す |
| 一覧から作るキャッシュ | 要素の数が変わる一覧から作るキャッシュは持たない(元の一覧が変わるたびに作り直しになる)。描画のたびに一覧から計算する |
| BottomSheet | UI Visit の並べ替えと Sample Map1 の地図の種類・経路・範囲・円の切り替えを、F2 で開く `mcc:BottomSheetView` にする。戻るキーは開いているシートを先に閉じる。ほかの画面は対象にしない |

## 💥破壊的変更とその扱い

### 🧱ClamGrid 1.1.0 → 2.1.0

| 変更 | Template の該当 | 扱い |
| --- | --- | --- |
| 💥 `GridStyle` の `ShowColumnHeaders`・`ShowRowHeaders`・`ShowVerticalLines`・`ShowSortPriority` を `ColumnHeadersVisible`・`RowHeadersVisible`・`VerticalLinesVisible`・`SortPriorityVisible` に改名(旧名なし) | `Modules/UI/UIGridStyles.cs` の L57 `ShowSortPriority`、L64・L77 `ShowRowHeaders`、L78 `ShowVerticalLines` | 新しい名前に直す。値はそのまま |
| 💥 `GridFonts` を削除(SkiaExtension の `FallbackFonts` に移った) | 使っていない | なし。言語のヒントは下の「共通」 |
| 💥 `ScrollIntoView(int, int)` が `(int, int? = null, ScrollToPosition = MakeVisible)` になった | 使っていない(検索して確かめる) | なし |
| `GridStyle` を代入の時に検証し、規則に合わないと `ArgumentException` | `UIGridStyles.cs` の値 | 値が `Document/API.md` の規則を満たすことを確かめる |
| `GridColumn.Format` は `{` を含むと複合書式、含まないと書式指定子(今までどおり)。代入の時に解析し、不正な複合書式は `FormatException` | `Modules/UI/UIGridView.xaml` の `N0`・`MM/dd`・`MM/dd HH:mm` | そのまま動く |
| `GridColumnOrder` に保存用の `Width` が増えた | `Modules/UI/UIGridViewModel.cs` の `ColumnOrders`(保存した列の設定) | 保存済みの設定が読み込めることを実機で確かめる |
| 💥 `GridStyle.FontSize`・`HorizontalPadding`・`VerticalPadding` の型が `float` から `double` になった(P5) | `Modules/UI/UIGridStyles.cs` の L73 `FontSize = 15`・L76 `HorizontalPadding = 10` | 整数リテラルの代入はそのまま通る。`float` の変数で受けている箇所があればビルドエラーになるので直す |
| 追加: `AlternatingRowBackground`、`GridDataView` の `Filter`・`SourceCount`、列幅の保存、スクロール位置、複数行の見出し、`Invalidate()` | なし | 任意。使うときは ⚖️【判断】で提案する |

### 📅ClamCalendar 1.0.0 → 2.1.0

| 変更 | Template の該当 | 扱い |
| --- | --- | --- |
| 💥 `CalendarFonts` を削除(SkiaExtension の `FallbackFonts` に移った) | 使っていない | なし。言語のヒントは下の「共通」 |
| `HeaderFormat` は `{` を含むと複合書式、含まないと書式指定子 | 使っていない | なし |
| `CalendarStyle` を代入の時に検証する | 使っていない(既定のスタイル) | なし |
| 💥 `CalendarStyle` の 18 個(フォントサイズ・高さ・幅・余白)と `CalendarStamp.FontSize`・`Opacity` の型が `float` から `double` になった(P5) | `Services/CalendarService.cs` の L211–219 `new CalendarStamp { … FontSize, Opacity }` | `float` から `double` への代入はそのまま通る。`float` の変数で受けている箇所があればビルドエラーになるので直す |

Template のコードの修正は要らない見込み(P5 の型の変更もビルドで確かめる)。版を上げて、実機で確かめる。

### 🎛️ClamGauge 2.1.0(`Controls/Gauge.cs` からの置き換え)

| 変更 | Template の該当 | 扱い |
| --- | --- | --- |
| 💥 型と名前空間: `Controls.Gauge` → `ClamGauge.ClamGaugeView`(`xmlns:clam="clr-namespace:ClamGauge;assembly=ClamGauge"`)。`GaugeRange`・`GaugeUnitPosition` も ClamGauge のものを使う | `Modules/UI/UIGaugeView.xaml`(6 つの Style の `TargetType`、`x:Array Type`、要素) | 書き換えて、`Controls/Gauge.cs` を消す |
| 💥 `ShowValue` → `ValueVisible` | Style の Setter(Wind・Tachometer) | 新しい名前に直す |
| 💥 `GaugeBackgroundColor`・`GaugeBackgroundColor2` → `GaugeBackgroundStartColor`・`GaugeBackgroundEndColor` | Style の Setter | 新しい名前に直す。単色は Start だけ |
| 💥 線の太さ・針の幅・軸の半径が物理ピクセルから DIP になった(フォントの大きさは今も DIP 相当で変わらない) | 6 つの Style の `BezelOuterThickness`・`BezelInnerThickness`・`MajorTickThickness`・`MinorTickThickness`・`NeedleStartWidth`・`NeedleEndWidth`・`PivotRadius` | 値を見直す。ClamGauge の Example の値が DIP での手本(`Control_Extraction_Plan.md` の「Template の切り替えで設定すること」) |
| サイズ系の型が `float` から `double` になり、不正な値は代入の時に `ArgumentOutOfRangeException` | XAML の文字列での指定 | そのまま通る。C# から代入しているなら型を合わせる |
| `LabelFormat`・`ValueFormat` は `{` を含むと複合書式、含まないと書式指定子 | `{0:N0}`・`{0:N1}`・`{0:N0}%` | そのまま動く |
| 追加: `FontFamily`(既定 `sans-serif`、足りない文字は `FallbackFonts`)、`Dispose()` | なし | 日本語の単位やラベルが豆腐にならないことを実機で確かめる |
| `LabelConverter`・`LabelFormatter` は今までどおり | `Converters/CompassDirectionConverter.cs`(Wind) | そのまま使う |

### 🔤共通(SkiaExtension)

| 変更 | 扱い |
| --- | --- |
| 💥 文字ごとのフォントのフォールバックの設定は、SkiaExtension の `FallbackFonts`(`Languages`・`Typefaces`)の 1 か所になった。`Languages` の既定は端末の UI の言語(以前の `CalendarFonts`・`GridFonts` は `["ja"]`) | `MauiProgram.cs` の `ConfigureGlobalSettings` で `FallbackFonts.Languages = ["ja"]` を設定する(「⚖️決定事項」) |
| SkiaExtension は Clam の 3 つから推移的に入る | Template のコードから直接使うので、`Template.MobileApp.csproj` に `SkiaExtension` 1.2.0 を明示的に足す |
| `SKCanvas.DrawArc(SKRect, …)` は SkiaSharp 4.153.1 にある。SkiaExtension の同じ形の `DrawArc(SKRect, …)` 拡張は 1.2.0 で外した | 円弧は SkiaSharp の `DrawArc(SKRect, …)` か、SkiaExtension の `DrawArc(SKPoint center, float radius, …)` で描く |

## 🪜ステップ

| ステップ | 内容 | 状態 |
| --- | --- | --- |
| 1 | 準備(今の見た目の記録、割り当ての測り方の用意) | ✅ |
| 2 | ClamGrid を 2.1.0 に上げる | ✅ |
| 3 | ClamCalendar を 2.1.0 に上げる | ✅ |
| 4 | SkiaExtension を参照し、言語のヒントを決める | ✅ |
| 5 | Gauge を ClamGauge に置き換える(`Control_Extraction_Plan.md` の ④) | ✅ |
| 6 | Gauge のサンプルを 1 ページのタブ切り替えにする | ✅ |
| 7 | SkiaSharp の描画を見直す(共通部品への置き換えと割り当て、領域ごと) | ✅ |
| 8 | 動作確認一式と記録(`Control_Extraction_Plan.md` の ⑤、Change_Summary、README) | 📝 |

## 🧰1 準備

- [x] 2・3・5・6・7 で触る画面を実機で撮っておく(UIGrid・UIGridColumn・UICalendar・UISchedule・UIGauge と、7 の各画面)
- [x] 下の「🧪割り当ての測り方」を用意し、7 の対象の画面の 1 フレームの割り当てを測っておく

## 🧱2 ClamGrid 2.1.0

- [x] `Template.MobileApp.csproj` の `ClamGrid` を 2.1.0 にする
- [x] `Modules/UI/UIGridStyles.cs` の 4 か所を改名する(上の表)
- [x] `ScrollIntoView` などほかの変更点を使っていないことを検索で確かめる
- [x] ビルド(Release・Debug)の警告 0(ReSharper は 8 でまとめて)
- [x] 実機: UIGrid(並べ替えのマークと優先度、固定列、列の設定の画面、真偽値の編集、選択)、UIGridColumn(行のドラッグ、行見出し)、保存済みの列の設定が読めること

## 📅3 ClamCalendar 2.1.0

- [x] `Template.MobileApp.csproj` の `ClamCalendar` を 2.1.0 にする
- [x] ビルド(Release・Debug)の警告 0(ReSharper は 8 でまとめて)
- [x] 実機: UICalendar(選択の 4 つのモード、予定・スタンプ・休日、月の移動、今日へ戻る)、UISchedule
- [x] UICalendar の選択のモードの切り替えを、下のバーから F2(Setting)で開く `BottomSheetView` に移す。今日へ戻るのは F3(Today)にする(ユーザーの指示)

## 🔤4 SkiaExtension

- [x] `Template.MobileApp.csproj` に `SkiaExtension` 1.2.0 を足す
- [x] `GlobalUsing.cs` に `global using SkiaExtension;` を足す(`SkiaSharp` と同じ並び)
- [x] ⚖️【判断】`FallbackFonts.Languages` の設定(上の「共通」)→ `["ja"]` を設定する
- [x] ビルド(Release・Debug)の警告 0(ReSharper は 8 でまとめて)

## 🎛️5 Gauge の置き換え

- [x] `Template.MobileApp.csproj` に `ClamGauge` 2.1.0 を足す
- [x] `Modules/UI/UIGaugeView.xaml` を `clam:ClamGaugeView` に書き換える(xmlns・Style の `TargetType`・`GaugeRange`)
- [x] 改名(`ShowValue`・`GaugeBackgroundColor`・`GaugeBackgroundColor2`)と、DIP になった値の見直し(上の表)
- [x] `Controls/Gauge.cs` を消す(ほかに参照がないことを確かめる。`NoiseGauge`・`SpeedGauge`・`HomeBatteryGauge` は別物で対象外)
- [x] ビルド(Release・Debug)の警告 0(ReSharper は 8 でまとめて)、`xstyler -p`
- [x] 実機: 6 つのゲージが、置き換える前と同じ見た目と動きであること(1 で撮った画像と比べる)
- [x] `Document/Control_Extraction_Plan.md` の ④ の状態を直す

## 🧭6 Gauge のサンプル(1 ページでタブ切り替え)

手本は `D:\GitHub\Other-ClamGauge\Example\Modules\Gauge`。6 つの画面(`GaugeXxxView.xaml`・`GaugeXxxViewModel.cs`)と `GaugeViewModelBase.cs`・`ColorSwatch.cs` が持つものを、Template の UIGauge の 1 ページにまとめる。

- 種類ごとの Style(値の範囲・目盛り・帯・色・単位・針・アニメーション)
- ゲージのタップでランダムな値、スライダーでの値の変更
- 種類ごとの切り替え: Pressure(Unit・Labels・Value・帯の色)、Humidity(Unit・Labels・Bands・背景の色)、Temperature(Bands・Ticks・Labels・弧の色)、Wind(Labels・Needle・Value)、Speed(Easing・Time・Unit)、RPM(Value・Angle・Needle)

- [x] ⚖️【判断】画面の作りを案にして確認する: タブの部品(MauiComponents.Controls などの既存の部品を使うか)、種類ごとの切り替えの置き場所(シェルの Function2〜4 をタブに合わせて変えるか、ページの中のボタンにするか)、ViewModel の分け方、メニュー(`Modules/UI/UIMenu2View.xaml` の Gauge)とページの名前 → 「⚖️決定事項」
- [x] 決まった形で作る(Style は種類ごとにページのリソースに置く)
- [x] `xstyler -p`、ビルド(Release・Debug)の警告 0(ReSharper は 8 でまとめて)
- [x] 実機: 6 種類の切り替え、タップでのランダムな値、スライダー、種類ごとの切り替え、日本語・記号の表示
- [x] README の画像 `Document/Images/UI_Gauge.png` を撮り直すかをユーザーに確認する → `UI_Gauge.png`(開いたときの Press)と `UI_Calendar.png`(3 のシートの変更の後)を撮り直した

## 🖌️7 SkiaSharp の描画の見直し

- [x] 7-1 計器(ArcMeter・GlowRing・TimerDial。RangeBar は対象の処理なし)
- [x] 7-2 ホーム・天気(HomeBlurredImage・HomeThermostatDial・SeriesSegmentView・WeatherCompassDial・WeatherSunArcView。HomeGlowIcon・HomeBatteryGauge・SegmentBar・WeatherMoonPhaseView は対象の処理なし)
- [x] 7-3 ゲーム(MinesweeperBoard・Puzzle2048Board・SudokuBoard)
- [x] 7-4 その他の UI(SocialControls・DayTimetableView・GraphRowSurface・BarcodeView・KitSleepBand・TextToggle・MailAvatarImage。書体の置き換え後の割り当ては 7-6 の後にまとめて測る)
- [x] 7-5 Scene
- [x] 7-6 地図・画像

領域ごとに 7-1〜7-6 として進め、領域ごとに報告して止まる。各領域でやること:

- [x] 共通部品に置き換えられる処理を SkiaExtension に置き換える(下の「置き換えの対応」)。見た目が変わる置き換えは ⚖️【判断】にする
- [x] 自前のコードの割り当てを直す(「⚖️決定事項」の割り当ての対象)
- [x] 置き換えの前後で 1 フレームの割り当てを測って記録する
- [x] ビルドの警告 0、実機で見た目と動きが変わらないこと(ReSharper は 8 でまとめて)

置き換えの対応:

| 今の書き方 | SkiaExtension(または SkiaSharp) | 注意 |
| --- | --- | --- |
| `SKPathBuilder.AddArc` で円弧を描く | SkiaSharp の `canvas.DrawArc(SKRect, …)`、中心と半径なら SkiaExtension の `canvas.DrawArc(SKPoint, float, …)` | 同じ見た目になる |
| `Cos`・`Sin` で円周上の点を求める | `AngleHelper.GetPoint`(3 時が 0 度の時計回り)。12 時が 0 度の角度は `AngleHelper.ToSkiaAngle` で変換 | 角度の基準を合わせる |
| スイープグラデーションを `WithLocalMatrix` で回す | `AngleHelper.CreateArcGradient` | 色の位置の配列の意味を確かめる |
| `Ascent`・`Descent` で文字の縦の位置を計算する | `GetCenterBaseline`・`GetTopBaseline`・`GetBottomBaseline`・`GetLineHeight` | 計算が同じものだけ置き換える。`MidY + 3.2f` のような目分量の位置や、字形の境界(`MeasureText(…, out rect)`)で合わせているものは見た目が変わるので ⚖️【判断】 |
| 色の補間(`Lerp`・`Blend`・`Mix`・`Darken`・`Shade`) | `SKColor.Lerp(to, amount)` | SkiaExtension の `Lerp` はアルファも補間する。RGB だけの補間と結果が違うものは置き換えない |
| `(byte)(… * 255)` の不透明度 | `WithOpacity(float)` | 四捨五入になるので最大 1 違う |
| `ImageHelper.ToNormalizeBitmap`・`Resize` | `SKCodec.DecodeOriented()`・`SKBitmap.Resize(float)` | 今の処理は向き 8 通りのうち 3 通りだけ、読み込みが 2 回、180 度のときに `bitmap.Copy()` を破棄していない。置き換えで 3 つとも直る |
| `MatchCharacter('あ')` などで和文の書体を決める、`SKTypeface.Default` で描く | `FontResolver`・`canvas.DrawText(DrawableText, …)` | 文字ごとのフォールバック(和文・絵文字)になり見た目が変わることがあるので ⚖️【判断】 |

領域と主な対象(調査の結果。着手の前に読み直す):

| 領域 | ファイル | 共通部品の候補 | 自前の割り当ての候補 |
| --- | --- | --- | --- |
| 7-1 計器 | `Controls/ArcMeter.cs`・`GlowRing.cs`・`TimerDial.cs`・`RangeBar.cs` | 円弧(ArcMeter L357・GlowRing L143)、円周上の点、弧のグラデーション(GlowRing L135・ArcMeter L298)、不透明度 | ArcMeter L288–300 の `OrderBy`・`Select`・`ToArray`(描くたび)、TimerDial の 60 本の目盛りを毎回計算 |
| 7-2 ホーム・天気 | `Controls/HomeControls.cs`・`WeatherControls.cs` | HomeThermostatDial の円弧と値の弧(GlowRing とほぼ同じ処理)、WeatherCompassDial・SunArcView の点と円弧、文字の縦の位置 | HomeBlurredImage のキャッシュのキーを描くたびに文字列で作る(L119) |
| 7-3 ゲーム | `Controls/MinesweeperBoard.cs`・`Puzzle2048Board.cs`・`SudokuBoard.cs` | 文字の縦の位置、`Darken`・`Blend`、不透明度 | 2048 の L190 `Merged.Any(…)`(タイルごと)・L168/L183 の `foreach`、Sudoku の L449 `flashCells.Contains`(セルごと)・L224 の `foreach`、数字の `ToString`(セルごと) |
| 7-4 その他の UI | `Controls/SocialControls.cs`・`KitControls.cs`・`MailControls.cs`・`TextToggle.cs`・`DayTimetableView.cs`・`GraphRowSurface.cs`・`BarcodeView.cs` | 和文の書体の決め方(`MatchCharacter`)、文字の縦の位置 | DayTimetableView L362–376 の LINQ・`List`・配列・`FindIndex` のクロージャー、BarcodeView の LINQ・`List`(描くたび)、Social の数値の文字列、各所の `IReadOnlyList` の `foreach` |
| 7-5 Scene | `Graphics/Scene/SceneObject.cs`・`TelemetryScene.cs`・`FlightHudScene.cs`・`MechHudScene.cs`・`EnergyFlowScene.cs` | 円周上の点、`Lerp`(Mech L652・Energy L902)、不透明度 | 60 fps の毎フレーム: `Post` のクロージャー、`DrawCachedLayer`・`DrawStaticImage` に渡すクロージャー、Mech L379 の `string[]`、`Contacts`・`Squad` の `foreach`、時刻や値の文字列 |
| 7-6 地図・画像 | `Messaging/MapsuiController.cs`・`Helpers/ImageHelper.cs` | `Lerp`(L236)、`DecodeOriented`・`Resize` | 地図を動かすたびの `new SKPoint[route.Count]` |

- `Graphics/Drawing/*.cs` と、MAUI の `GraphicsView` で描くコントロール(`NoiseGauge`・`SpeedGauge` など)は SkiaSharp ではないので対象外
- 時刻(`DateTime.Now.ToString(…)`)など入力が毎フレーム変わる文字列は、そのままにする

## 🧪割り当ての測り方

Template にはテストのプロジェクトがないので、実機(Debug)で一時的に測る。測り終えたら測定用のコードは消す。

- 描画のメソッドの前後で `GC.GetAllocatedBytesForCurrentThread()` の差を取り、数百フレームの平均を `Android.Util.Log` で出す
- 型ごとの内訳が要るときは、`EventListener` で `Microsoft-Windows-DotNETRuntime` を GC キーワード(1)・Verbose で有効にし、`GCAllocationTick` の `TypeName` を数える(約 100 KB ごとのサンプリングなので、数千フレーム以上回す)

7 の前の 1 フレームの割り当て(2026-10-12、Debug、Pixel 9a。バイト。2 回目以降の描画の平均で、SkiaSharp の内部の割り当てを含む。Scene の「ループ」は更新・描画・`Post` までの 1 回分):

| 領域 | 1 フレームの割り当て(バイト) |
| --- | --- |
| 7-1 | ArcMeter 2958・GlowRing 1674・TimerDial 2040・RangeBar 1086 |
| 7-2 | HomeThermostatDial 2568・HomeGlowIcon 1552・HomeBatteryGauge 1488・HomeBlurredImage 1280・SeriesSegmentView 1739・SegmentBar 941・WeatherCompassDial 2030・WeatherSunArcView 2015・WeatherMoonPhaseView 1548 |
| 7-3 | Puzzle2048Board 11396・SudokuBoard 8288・MinesweeperBoard 1400 |
| 7-4 | SocialStatus 22031・SocialNotification 20827・SocialPlayer 16847・SocialInformation 13600・SocialAlert 12184・SocialMenu 10040・SocialEpisode 7632・SocialCounter 5816・SocialIcon 3344・DayTimetableView 10359・TextToggle 4009・KitSleepBand 3411・BarcodeView 3001・GraphRowSurface 1152 |
| 7-5 | 描画: EnergyFlow 19210・Telemetry 11542・FlightHud 10918・MechHud 8604。ループ: EnergyFlow 23123・Telemetry 15454・FlightHud 14867・MechHud 12548 |
| 7-6 | MapRoute 848 |

平均には、描画と関係なくランタイムが割り当てる 27〜56 KB の突出が 1 フレームだけ入ることがある(変えていないコントロールでも起きる)。前後の比較は、インスタンスごとの最小(いつもの値)で行う。

## 🧪確認の手順

| 項目 | やり方 | 合格の条件 |
| --- | --- | --- |
| ビルド | `dotnet build Template.MobileApp.slnx -c Release` と `-c Debug` | 警告 0・エラー 0 |
| ReSharper | `jb inspectcode Template.MobileApp.slnx --profile=Template.MobileApp.sln.DotSettings --severity=WARNING --include="<変えたファイル;…>" --format=Text --output=<出力先> --no-build`(先にビルドする) | 指摘なし |
| XAML | `xstyler -p -c Settings.XamlStyler -f <ファイル>` | 合格 |
| 改行 | 変えたファイルを perl の raw 読みで数える | すべて CRLF |
| 実機 | `dotnet build Template.MobileApp/Template.MobileApp.csproj -c Debug -t:Install` で入れ、`adb shell monkey -p template.mobileapp -c android.intent.category.LAUNCHER 1` で起動する。画面は `adb exec-out screencap -p > <ファイル>.png`(1080×2424) | 落ちない(`adb logcat -d -s AndroidRuntime:E` が空)、見た目と動きが前と同じ |

## 📝作業の記録

| 日付 | ステップ | 内容 |
| --- | --- | --- |
| 2026-10-12 | 作成 | 計画を作った。4 つのパッケージが nuget.org に公開済みであることを確かめた |
| 2026-10-12 | 1 | 上げる版は ClamCalendar・ClamGrid・ClamGauge とも 2.1.0(ユーザーの指示)。17:20 の時点の nuget.org は 3 つとも 2.0.0 まで、SkiaExtension は 1.2.0 まで公開済み |
| 2026-10-12 | 1 | 実機で撮影: UIGrid・UIGridColumn・UICalendar・UISchedule・UIGauge と、7 の対象の Money(支払いのシートを含む)・Mail・MailDetail・Weather・Kit・Home・Habit・Graph・Graph2・TreeMap・Social・Flight・Tactical・Telemetry・Energy・Device Status・Sensor・Timer・Sudoku・2048・Minesweeper・Map2・CV Local・Drawing(30 画面、67 枚。作業用のフォルダーに置き、リポジトリには入れない) |
| 2026-10-12 | 1 | 割り当ての測り方を用意: 一時的な `AllocationProbe`(描画の入口で `GC.GetAllocatedBytesForCurrentThread()` の差を取り、コントロール 1 つごとに 120 回描き直させて平均を `Android.Util.Log` に出す。Scene は描画とループの 1 回分を 300 フレームごと)。差し込みと取り外しは作業用のスクリプトで行い、ソースに残さない。較正の `byte[1000]` は 1032 バイトで正確 |
| 2026-10-12 | 1 | ユーザーの指示で Clam の置き換え(2〜6)を先に進める。7 の前の基準の測定は 2 と並行して実機で回した |
| 2026-10-12 | 2 | 17:47 に 3 つの 2.1.0 の公開を確かめた。`ClamGrid` を 2.1.0 に、`UIGridStyles.cs` の `ShowSortPriority`・`ShowRowHeaders`(2 か所)・`ShowVerticalLines` を `SortPriorityVisible`・`RowHeadersVisible`・`VerticalLinesVisible` に。`ScrollIntoView`・`GridFonts` は使っていない。`GridStyle` の値(フォント 15・行 46・見出し 42・余白 10・行見出し 56)は検証の規則を満たす。`ColumnOrders` は画面の受け渡しだけで保存はしていない。Debug のビルドは警告 0 |
| 2026-10-12 | 2 | Release のビルド(`Template.MobileApp.slnx`)はエラー 0。警告は Android SDK のマーシャルメソッドの警告(Shiny の BLE の `OnServiceAdded`。前からあり、コードの変更とは関係ない)だけ。実機: UIGrid は変更前と画素まで同じ。並べ替え(状態 ▲1・受注番号 ▲3 の優先度の表示と見出しの色)、選択(2 件)、固定列(状態・受注番号)を残した横のスクロール、`MM/dd HH:mm` の書式、確認の列のチェックの切り替え、見出しの長押しから列の設定(行見出しの番号と取っ手、行のドラッグで並べ替え、表示を外す)、Apply で表に反映(顧客が前へ、更新が隠れる)、Reset で元に戻ることを確かめた。落ちていない |
| 2026-10-12 | 3 | `ClamCalendar` を 2.1.0 に。`CalendarFonts` は使っていない。`CalendarService.cs` のスタンプの `float` の値は `double` のプロパティにそのまま入る。Debug のビルドは警告 0、Release は 2 と同じ Android SDK の警告だけ。実機: UICalendar・UISchedule は変更前と画素まで同じ。Single・Multi・Range の選択、次の月、今日へ戻ることを確かめた。落ちていない |
| 2026-10-12 | 3 | ユーザーの指示で、UICalendar の下のバー(選択のモードの 4 つのボタンと今日へのボタン)をやめ、モードは F2(Setting)で開くシート(`mcc:BottomSheetView`、ボタンは高さ 44)、今日へ戻るのは F3(Today)にした。シートはモードを選ぶと閉じ、F2 をもう一度押すか戻るキーでも閉じる。実機で確かめた(Single の選択、Range の範囲の選択、次の月から Today で今月に戻る)。ほかの画面のシートの候補は、この対応の後にユーザーに確認する |
| 2026-10-12 | 4 | `SkiaExtension` 1.2.0 を参照に足し、`GlobalUsing.cs` に `global using SkiaExtension;`。Template に SkiaExtension と同じ名前の型・SKxxx の拡張メソッドはない。⚖️ 言語のヒントは `["ja"]` に決まり、`MauiProgram.cs` の `ConfigureGlobalSettings` に `FallbackFonts.Languages = ["ja"];`。端末は `ja-JP`。Debug は警告 0、Release は 2 と同じ Android SDK の警告だけ。実機: UICalendar・UIGrid は設定の前と画素まで同じ |
| 2026-10-12 | 5 | `ClamGauge` 2.1.0 を参照に足し、`UIGaugeView.xaml` を `clam:ClamGaugeView`・`clam:GaugeRange` に、`ShowValue` を `ValueVisible` に、`GaugeBackgroundColor`・`GaugeBackgroundColor2` を `GaugeBackgroundStartColor`・`GaugeBackgroundEndColor` に。旧 Gauge の物理ピクセルの値(既定に頼っていた値を含む)は、Pixel 9a の密度 2.625 で割って小数 1 桁の DIP にした(ベゼル 6→2.3・10→3.8・3→1.1、大目盛り 2→0.8・3→1.1、小目盛り 1→0.4・0.5→0.2、針の根元 5→1.9・6→2.3・8→3・12→4.6、針の先 2→0.8、軸 5→1.9・6→2.3・8→3・10→3.8)。`Controls/Gauge.cs` を消した(ほかに参照なし)。XAML Styler は `xmlns:clam` と `xmlns:conv` を 1 行にまとめる(どちらも `xmlns:*` のグループ)。Debug は警告 0、Release は 2 と同じ Android SDK の警告だけ。実機: 6 つのゲージは置き換える前と同じ見た目。違いは文字のアンチエイリアスと、中央の値と単位の間隔(ClamGauge の固定値で、下 2・右 4)が DIP になったことによる 1〜3 画素のずれだけ。スライダーで Speed・Tacho の針が新しい値まで動く。落ちていない |
| 2026-10-12 | 6 | ⚖️ 画面の作りが決まった(「⚖️決定事項」)。`UIGaugeView.xaml` を作り直した: 上に `mcc:SegmentedView`(Press・Humid・Temp・Wind・Speed・RPM、幅 60)、その下に種類ごとのゲージ・スライダー・色の候補。選んでいない種類は `LazyViewOption` で選んだときに作る。ゲージの値は Example の DIP の値、スライダーの色は前の Template のまま。F2〜F4 の文字はページのリソースの `s:MapToTextConverter` で種類ごとに選ぶ。書式(`{0:N1}` など)・Wind のラベルのコンバーター・RPM の太い針は、VM の bool と XAML の `DataTrigger` で切り替える。`UIGaugeViewModel.cs` は種類ごとの個々のプロパティと F2〜F4 の振り分け、上に `GaugeKind`・`GaugeColorSwatch`。`CompassDirectionConverter` を 8 方位にし、`DegreeTextConverter` を足した。`xmlns:conv` を `xmlns:converters` にしたので、XAML Styler が xmlns を 1 行にまとめなくなった。Debug は警告 0、Release は 2 と同じ Android SDK の警告だけ。実機: 6 種類とも F2〜F4・色の候補・タップでのランダムな値・スライダーが Example どおりに動き、F キーの文字が種類で変わる。記号(°C・°・%・hPa・km/h・x1000 rpm)は正しく出る(このページに日本語の文字はない)。落ちていない |
| 2026-10-12 | 6 | ユーザーの指示で README の画像 `Document/Images/UI_Gauge.png`(開いたときの Press)と `UI_Calendar.png`(F2・F3 になった後)を撮り直した |
| 2026-10-12 | 7-1 | ArcMeter: 円周上の点を `AngleHelper.GetPoint`、円弧を `DrawArc(center, radius, …)`、弧のグラデーションを `AngleHelper.CreateArcGradient` に。描くたびの `OrderBy`・`Select`・`ToArray` を、同じ並び(安定)になる配列への挿入に。GlowRing: 円弧と弧のグラデーションを同じく置き換え。TimerDial: 目盛りとつまみの点を `GetPoint` に(円弧は GlowRing の処理)。RangeBar は対象の処理なし。どれも計算が同じで、見た目は変わらない(Habit は画素まで同じ。Status・Timer は充電の状態と経過時間による差だけ)。Debug は警告 0 |
| 2026-10-12 | 7-2 | HomeBlurredImage: 描くたびに作っていたキャッシュのキーの文字列をタプルの比較に、暗くする色を `WithOpacity` に。HomeThermostatDial: 円弧・弧のグラデーション・つまみの点を 7-1 と同じく置き換え。SeriesSegmentView: 文字の縦の位置を `GetLineHeight`・`GetBottomBaseline` に(式が同じ)。WeatherCompassDial: 円周上の点を `GetPoint` + `ToSkiaAngle`、矢じりの幅の向きを `GetPoint`、ラベルの縦の位置を `GetCenterBaseline` に。WeatherSunArcView: 点線の道と通った道の円弧を SkiaSharp の `DrawArc(oval, …)` に(楕円上の点と塗りの形はそのまま)。HomeGlowIcon は字形の境界で位置を合わせているので対象外。Home は画素まで同じ、Weather は天気のデータが時刻で変わる以外は同じ描き方。Debug は警告 0 |
| 2026-10-12 | 7-1・7-2 | 1 フレームの割り当て(いつもの値、前 → 後。バイト): ArcMeter 2816 → 1592、GlowRing 1672 → 1360、TimerDial 2040 → 1728、HomeThermostatDial 2568 → 2088、HomeBlurredImage 1280 → 848、WeatherSunArcView 2008 → 1672。RangeBar 1272・HomeGlowIcon 1552・HomeBatteryGauge 1488・SeriesSegmentView 1776・SegmentBar 936・WeatherCompassDial 2024・WeatherMoonPhaseView 1440 は変わらない(残りは SkiaSharp の内部の割り当て) |
| 2026-10-12 | 7-3 | MinesweeperBoard: 地雷の芯の色(`Darken`、RGB の切り捨て)を `Lerp`(黒、アルファは元の色のまま)に、めくるときの覆いを `WithOpacity` に、数字の縦の位置を `GetCenterBaseline` に。Puzzle2048Board: `Slides`・`Tiles`(`IReadOnlyList`)の `foreach` を `for` に、タイルごとの `Merged.Any(…)` のクロージャーを `for` の判定に、ハイライトの色(`Blend`)を `Lerp` に、数字の文字列を値ごとに保持(512 以上は毎回作っていた)、縦の位置を `GetCenterBaseline` に。SudokuBoard: フラッシュ中の `flashCells` の `foreach` と `Max(GetFlashDistance)` を `for` に、縦の位置を `GetCenterBaseline` に。0〜9 の数字の `ToString` はランタイムが共有の文字列を返すので割り当てがなく、数字の文字列の配列は作らない。Minesweeper は画素まで同じ、2048 はハイライトの色の四捨五入の差(最大 1)だけ、Sudoku は盤の外の経過時間の差だけ。Debug は警告 0 |
| 2026-10-12 | 7-3 | 1 フレームの割り当て(いつもの値、前 → 後。バイト): Puzzle2048Board 11344 → 10752。MinesweeperBoard 1400 → 1424 は変えた処理を 1 つも通らない初期の盤での差で、SudokuBoard 8288 → 8312 も同じ +24 なので、SkiaSharp の内部の揺れ(フラッシュや移動の動きの間の割り当ては、静止した盤の測定には出ない) |
| 2026-10-12 | 7-4 | SocialControls: 9 つの描画で合わせて 43 回呼んでいた `DeviceDisplay.MainDisplayInfo.Density`(1 回で約 2.1 KB の割り当て)を、描画の最初に 1 回だけ取る形に(キャンバスから求めた密度は 2.625 に誤差が乗り、`(int)(幅 × 密度)` の切り捨てで 1 画素ずれるおそれがあるので、値は今のまま)。行の高さの `Descent - Ascent` を `GetLineHeight` に、`Units` の `foreach` を `for` に。数え上げの数値の文字列は動きの間は毎フレーム変わるので毎回作る。DayTimetableView: 予定の並べ替え・列の割り当て・空き時間・「空き …」・所要時間・「hh:mm - hh:mm」を、`Events`・`StartTime`・`EndTime`・`MinimumFreeSlotForLabel` が変わったときだけ作る形に、時刻の軸の文字列を時ごとに保持。GraphRowSurface: `Segments` の `foreach` を `for` に。BarcodeView: `Value` が変わったときだけ符号化する形に。KitSleepBand: `Spans` の `foreach` を `for` に、時刻のラベルを 0〜23 時の配列に。TextToggle・MailAvatarImage: 文字の縦の位置を `GetCenterBaseline` に(式が同じ。Mail は名前ごとに 1 回だけ描く)。書体(`MatchCharacter`)の置き換えは ⚖️ で確認する。Social・Money・Schedule は画素まで同じ、ほかは時刻やスクロールの量、乱数の支払いの番号による差だけ。Debug は警告 0 |
| 2026-10-12 | 7-4 | 1 フレームの割り当て(いつもの値、前 → 後。バイト): SocialStatus 21992 → 5056、SocialNotification 16736 → 3872、SocialPlayer 16840 → 3976、SocialInformation 13600 → 4992、SocialAlert 12184 → 3608、SocialMenu 10040 → 3608、SocialEpisode 7632 → 3344、SocialCounter 5816 → 3744、DayTimetableView 9832 → 5360、BarcodeView 3000 → 936、KitSleepBand 3408 → 3000、GraphRowSurface 1488 → 1448。SocialIcon 3344・TextToggle 4008 は変わらない(`DeviceDisplay` の 1 回分の約 2.1 KB が残る) |
| 2026-10-12 | 7-4 | ⚖️ 和文の書体: 言語のヒントのない `MatchCharacter` は、この端末のフォールバックの順(zh-Hans → zh-Hant → ja → ko)で中国語(簡体字)の書体を返していた。ユーザーの決定で、KitSleepBand・TextToggle はインスタンスごとの `FontResolver("sans-serif")`(ハンドラーが外れたら破棄)と `DrawableText` に、MailAvatarImage は頭文字を描くときだけ `FontResolver` を作る形にした(太字は `CreateText` の `bold`)。縦の位置は `FontResolver.GetMetrics` の `GetCenterBaseline` |
| 2026-10-12 | 7-5 | SceneObject: ループが UI スレッドに渡す 2 つの処理(描き直し、更新と描き直し)を、フレームごとのラムダから、ループの前に 1 回だけ作るデリゲート(ローカル関数)に。`DrawCachedLayer`・`DrawStaticImage` を、描く処理に状態を渡す形(`TState` + `static` ラムダ)にしてクロージャーをなくした。TelemetryScene・FlightHudScene・MechHudScene: 円周上の点を `AngleHelper.GetPoint`(12 時が 0 度の角度は `ToSkiaAngle`)、円弧を `DrawArc(center, radius, …)` に。MechHudScene の地形の色と EnergyFlowScene の流れの色を `SKColor.Lerp` に(自前の `Lerp`・`Mix` を消した)。Mech の通信の 3 行を static の配列に |
| 2026-10-12 | 7-6 | MapsuiController: 色の補間を `SKColor.Lerp` に(自前の `Lerp` を消した)。ImageHelper: `ToNormalizeBitmap` を `SKCodec.DecodeOriented()` に、`Resize` を `SKBitmap.Resize(float)` に |
| 2026-10-12 | 7 | ユーザーの指示で、描画で回す自前の一覧を配列・`List<T>` にして `foreach` に: 2048 の `Slides`・`Merged`・`Tiles`、数独の `Flash`、Flight の `Contacts`、Mech の `Squad`・`Contacts`、Energy の `Events`(`List<T>`)。public の型(2048・数独)の CA1819 は `#pragma` で抑止(Flight・Mech・Energy は internal の型なので付けない) |
| 2026-10-12 | 7 | 外から受け取る一覧は、一般的なコントロールの型を確かめて、配列にせず `IReadOnlyList<T>` で受けて `for` で回す形にした。確かめた型: MAUI は `ItemsSource` が `IEnumerable`(Picker は `IList`)、図形の点などは `ObservableCollection<T>` の派生。Toolkit・Syncfusion は `IList`・`IList<T>`・`IEnumerable<T>`・`ObservableCollection<T>`。ClamGauge の `Ranges` は `IList<T>`(`for` で回し、変更を監視)、ClamCalendar は `IEnumerable<T>`。データの一覧に配列を使う例はない。バインドするプロパティ(RadarScreen の `Targets`・DayTimetableView の `Events`・KitSleepBand の `Spans`・SocialInformation の `Units`)は、`INotifyCollectionChanged` の変更で描き直す。RadarScreen は消えたターゲットの残光の時刻を捨てる。描画の部品に外から設定する一覧(ChartDrawing の `SeriesNames`・WheelDrawing の `SetItems`・GraphRow の `Segments`)は元の `IReadOnlyList<T>` のままで、GraphRowSurface の `Segments` の `foreach` は `for` に。地図の経路の `SetOverlayRoute` は一度変換するだけなので `IEnumerable<T>` で受け、変換した内部の値は `List<T>` で持つ。VM・モデルの型は元の `IReadOnlyList<T>` のまま。Radar は VM を `ObservableCollection<T>` にして 5 秒ごとに中身を入れ替え、`TGT` の数は `Targets.Count` にバインド。MauiComponents の TreeView は変えない(元の `IReadOnlyList<T>`) |
| 2026-10-12 | 7 | ユーザーの指示で、要素の数が変わる一覧から作るキャッシュを外した: DayTimetableView のレイアウト(7-4 で入れた形)は、描画のたびに `Events` から計算する元の形に。地図の経路の画面の座標の配列は描くたびに作る。時刻の軸の文字列(時ごと)の保持は残す |
| 2026-10-12 | 7 | ユーザーの指示で、型を複数持つファイルに区切りのコメント(`//---` と名前)を入れた: SceneObject(Interface・Base)、TelemetryScene(Simulation・Scene)、FlightHudScene(Enum・Data・Simulation・Scene)、MechHudScene(Data・Simulation・Scene)、EnergySim(Enum・Data・Helper・Simulation)、TextToggle(Event・Toggle) |
| 2026-10-12 | 7 | ユーザーの指示で、UI Visit の並べ替えのパネルと Sample Map1 の地図の種類・経路・範囲・円の切り替えを、F2 で開く `mcc:BottomSheetView` に移した。Visit は並べ替えのボタンからも開く。Map1 は F2 が「Layer」で、地図の上のボタンはホームだけにした。戻るキーは開いているシートを先に閉じる |
| 2026-10-12 | 7 | 実機(Debug): Radar は 5 秒ごとに点と `TGT` の数が変わる(5 → 6 → 5)。Schedule は日を替えると予定と空き時間が描き直される。Kit の睡眠の帯、Social の部隊の一覧、Graph・Graph2、Wheel の回転、Chart、Custom の TreeView、2048(新しいゲームで移動と合体)・数独・マインスイーパー、Flight・Tactical・Telemetry・Energy は描き方が変わらない。Visit・Map は F2 でシートが開き、戻るキーでシートだけ閉じる。Map のシートで経路・範囲・円が出る。Map2 の Overlay で経路のグラデーションが出る(地図のタイルは変更の前から白いまま)。TreeMap の撮影で画像が縦向きに回って表示され、色の集計が出る。落ちていない。ChartDrawing・WheelDrawing・CommitGraph・MapsuiController の一覧の型の直しは、ユーザーの指示で画面の再確認をしていない(ビルドで確かめた) |
| 2026-10-12 | 7 | 1 フレームの割り当て(いつもの値、7 の前 → 後。バイト): Scene の描画は EnergyFlow 19304 → 18104・Telemetry 11552 → 11232・FlightHud 10528 → 10240・MechHud 8536 → 8176、ループは EnergyFlow 23192 → 21992・Telemetry 15440 → 15120・FlightHud 14448 → 14128・MechHud 12456 → 12064。DayTimetableView 9832 → 9392(レイアウトのキャッシュを外したので 7-4 の 5360 より増えた)、GraphRowSurface 1488 → 1112、SocialInformation 13600 → 4992。TextToggle 4008 → 5216・KitSleepBand 3408 → 4016 は `FontResolver` の `CreateText`(SkiaExtension の内部)の分が増えた。MapRoute は前も後も経路を出す前の値(848)で、経路を描くときの割り当ては測れていない。Puzzle2048Board は盤のタイルの数が違う(新しいゲーム)ので比べられない |
| 2026-10-12 | 7 | 最後の形で Debug のビルドは警告 0、Release は 2 と同じ Android SDK の警告だけ。測定用のコードは外し、作業ツリーにも index にも残っていないことを確かめた |
| 2026-10-12 | 7 | ユーザーの指示で、`for` にした所・キャッシュにした所のうち効果の無かったものを戻した(測り直しはしない): SocialIcon の密度の変数(呼び出しは元から 1 回)、SudokuBoard のフラッシュの長さの計算のループ(盤が変わるときに 1 回だけなので LINQ の `Max` に戻した)、MapsuiController の経路の座標の持ち方(点が 4 つで、描くたびに減るのは `MPoint` 4 個だけなので元の `MPoint` に。SkiaExtension の `Lerp` と `SetOverlayRoute` の引数の `IEnumerable<T>` は残す)。ほかの `for`・キャッシュ(ArcMeter・HomeBlurredImage・2048・KitSleepBand・GraphRowSurface・Social・DayTimetableView の時刻の文字・BarcodeView・Scene)は割り当てが減ったので残す。2048 の数字の文字列の保持は 300 以上の値(512 から)で効く |
