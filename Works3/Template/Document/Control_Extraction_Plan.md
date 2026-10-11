# 🧩コントロールの抽出の計画

`Template.MobileApp/Controls` の汎用のコントロールを別のアセンブリへ移す。SkiaSharp を使わないものは 1 つのライブラリにまとめ、SkiaSharp で描くものは Gauge だけを ClamGauge にする。ライブラリの見本のアプリで動きを確かめ、完全に移せると判断してから Template の参照を替え、Template のものを消す。

ファイルパスは `Template.MobileApp/` からの相対。

## 🔖凡例

| 列 | 絵文字 |
| --- | --- |
| 汎用性 | ★★★★★ どのアプリでも使う形 / ★★★★ よく使う形 / ★★★ 用途によって使う |
| 移行 | ✅ 移す |
| 状態 | 📝 未着手 / 🧪 ライブラリへ移して見本のアプリで確認中 / 👀 見本のアプリで確認済み・Template の切り替え待ち |

## ⚖️決定事項

| 項目 | 決定 |
| --- | --- |
| 非 Skia のアセンブリ | `MauiComponents.Controls`(パッケージ `Components.Maui.Controls`)。`D:\GitHub\Components-MauiComponents` に足し、見本は `MauiComponentsExample`(`Components.Maui.Resolver` と同じ並び) |
| Skia のアセンブリ | `ClamGauge`(パッケージも `ClamGauge`)。Gauge だけを入れ、型名は `ClamGaugeView`(`GaugeRange`・`GaugeUnitPosition` はそのまま)。リポジトリは `D:\GitHub\Other-ClamGauge` で、ClamGrid・ClamCalendar と同じ構成(本体・Tests・Example・Document・README・AGENTS.md・Settings.XamlStyler)、対象は `net10.0;net10.0-android` |
| 対象の選び方 | 汎用性が ★★ 以下のものは対象にしない。1 つの UI の画面だけで使うものは、その画面専用の可能性が高いので対象にしない。ただし、よく使う形(★★★★ 以上)として作ったものは対象にする |

## 🚀進め方

| 段階 | 内容 |
| --- | --- |
| ① | ライブラリを作る |
| ② | コントロールを移し、Template への結合を外す(表の「移す前の手直し」) |
| ③ | 見本のアプリに画面を作り、実機で動きを確かめる(ClamGauge は下の「🧪ClamGauge の Example」) |
| ④ | 全部を確かめたら、Template の参照をライブラリに替え、XAML の名前空間を書き換えて、`Controls` から消す |
| ⑤ | Template の使っている画面を実機で確かめる(見た目と動きが前と同じ) |

## 🧪ClamGauge の Example

ClamGrid・ClamCalendar の Example と同じく、パターンごとの画面を用意し、値と見た目を画面の中で変えられるようにする。

| 項目 | 内容 |
| --- | --- |
| パターン | UI Gauge の 6 つの形(気圧・湿度・温度・風向・速度・回転数)を元に、形ごとの画面。色の帯(`GaugeRange`)・目盛り・数字の書式・中央の値と単位の位置(`GaugeUnitPosition`)・針の違いが分かるように並べる |
| 値 | タップするたびに新しい値(乱数)にする。スライダーで値を動かす。値が変わると針が動くことを確かめる |
| 見た目 | パターンによって、色(円弧・色の帯・地の色)を切り替えられるようにする(色の候補のボタンなど) |

## 📋一覧

### 🧱非 Skia(MAUI だけで作るもの)

| 現在のファイル名 | 何用か | 使っている画面 | 汎用性 | 移行 | 移す前の手直し | 状態 |
| --- | --- | --- | --- | --- | --- | --- |
| `Controls/BottomSheetView.cs` | 下から出るシート(半開・全開・閉じる、ドラッグ・フリング・背景のタップ) | View Bottom Sheet・Sample Map2・UI Graph・Money・Schedule・Shop | ★★★★★ | ✅ | つまみの色と背景の暗さをプロパティに | 📝 |
| `Controls/SideDrawer.cs` / `SideDrawer.android.cs` / `SideDrawer.ios.cs` | 左から出るドロワー(端のスワイプ・フリング) | View Drawer・UI Mail | ★★★★★ | ✅ | Android の部分があるので、対象に `net10.0-android` を含める | 📝 |
| `Controls/TabStrip.cs` | 横にスクロールするタブの見出しと下線(スワイプに追従) | UI News・Profile | ★★★★ | ✅ | CarouselOption.PagePosition と組で使うことを README に書く | 📝 |
| `Controls/ChipGroup.cs` | 1 つ選ぶチップの並び(折り返し) | UI Item・Shop・Timeline | ★★★★ | ✅ | 押したときの波紋は Template から付ける形に(ライブラリは TapGestureRecognizer) | 📝 |
| `Controls/SegmentedView.cs` | 区切りから 1 つを選ぶ(選択の塗りが滑る) | UI Kit Dash | ★★★★ | ✅ | 押したときの波紋は Template から付ける形に。使う画面は 1 つだが、Sf の部品の置き換えとして作った汎用の形 | 📝 |
| `Controls/AccordionView.cs` | 押すと開閉する項目の並び(1 つだけ開く設定あり) | UI Item・Kit Setting | ★★★★ | ✅ | 矢印のアイコン(MaterialIcons)と固定の色をプロパティに。押したときの波紋は Template から付ける形に | 📝 |
| `Controls/ShimmerView.cs` | 読み込み中の骨組みの上を光の帯が流れる | UI News | ★★★★ | ✅ | 地の色の既定を BindableProperty の既定値に。使う画面は 1 つだが、Sf の部品の置き換えとして作った汎用の形 | 📝 |
| `Controls/StepIndicator.cs` | 段階の点の並びと「n / N」 | Device Bluetooth・Navigation Stack・Wizard | ★★★★ | ✅ | 固定の色をプロパティに | 📝 |
| `Controls/MarqueeLabel.cs` | 右から左へ流れ続ける文字 | View Custom・UI News | ★★★★ | ✅ | — | 📝 |
| `Controls/AvatarGroup.cs` | 重ねたアバターと「+N」 | View Custom・UI Schedule・Stream Detail | ★★★★ | ✅ | OverlapPanel と一緒に移す | 📝 |
| `Controls/OverlapPanel.cs` | X・Y のずらし量で重ねるレイアウト | View Layout(AvatarGroup の中でも使う) | ★★★★ | ✅ | — | 📝 |
| `Controls/StaggeredGrid.cs` | 高さの違うカードを低い列へ詰めるレイアウト | View Layout・UI Weather | ★★★★ | ✅ | — | 📝 |
| `Controls/VariableSizeWrapPanel.cs` | 列数固定のタイル(ColumnSpan・RowSpan) | View Layout・UI Kit Dash | ★★★★ | ✅ | — | 📝 |
| `Controls/CircularLayout.cs` | 円周・円弧・同心円に並べるレイアウト | View Layout | ★★★ | ✅ | — | 📝 |
| `Controls/CameraOverlayView.cs` | カメラのプレビューに重ねる三分割の線と四隅のかぎ | Device OCR・QR Scan・Sample CV Local・CV Net | ★★★ | ✅ | — | 📝 |
| `Controls/ColorPicker.cs` | RGBA のスライダー 4 本とプレビュー | View Custom・UI Kit Setting | ★★★ | ✅ | 固定の色とフォント(JetBrainsMono)をプロパティに | 📝 |
| `Controls/DurationPicker.cs` | 時と分(5 分刻み)の Picker を TimeSpan にまとめる | View Custom | ★★★ | ✅ | 文言(時間・分)・刻み・色をプロパティに | 📝 |
| `Controls/TreeView.cs` | 開閉できる簡単なツリー | View Custom | ★★★ | ✅ | 節の型(`TreeNode`)を一緒に移す。アイコンと色をプロパティに | 📝 |

### 🎨Skia(SkiaSharp で描くもの → ClamGauge)

| 現在のファイル名 | 何用か | 使っている画面 | 汎用性 | 移行 | 移す前の手直し | 状態 |
| --- | --- | --- | --- | --- | --- | --- |
| `Controls/Gauge.cs`(`GaugeRange`・`GaugeUnitPosition` を含む) | 円形のゲージ一式(地のグラデーション・外と内のベゼル・円弧の溝と塗り・色の帯・大小の目盛り・数字・中央の値と単位・針と軸。値が変わると針が動く) | UI Gauge | ★★★★ | ✅ | 名前空間を ClamGauge に、型名を `ClamGaugeView` に | 📝 |

## ⚠️制約

- Behaviors は対象にしない(AnimationOption はアプリ固有の動き、Android の Handler を使う Option は共通化しない、LazyViewOption は Android 固有の部分があるので今のまま)
- ライブラリには Template の色・フォント・文言・型を持ち込まない(プロパティで渡す)
- Template の画面の見た目と動きは変えない
- 参照を替えるのは、見本のアプリで全部の動きを確かめてから
