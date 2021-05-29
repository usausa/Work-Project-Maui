# KeySample project for Xamarin.Forms

## 基本構想

### ソフトウエアキーボード

* ソフトウエアキーボードはOSレベルで無効化
* EntryRendererのキーボード処理を潰すのが大がかりなので
* やろうと思えばできる

### タブ移動

* 標準のタブ機構は使い物にならないので自前実装
* ButtonのFastRendererを考慮していない？
* IsEnabled=falseなコンテナ上のボタン等にも移動してしまう
* 移動順序がおかしい？
* 自前機構としてはとりあえず項目のレイアウト順のみを考慮だが、Tabオーダー対応も可能

### Android側キー制御

* 優先してキーフックするためDispatchKeyEvent()でキーフック
* Entry、ListView等、独自のキー制御が入るものについては、キー制御が入る条件の時はフックを除外 

### コントロール全般

* 標準でフォーカス移動の対応に問題があるコントロール(Button、ListView等)はEffectで該当設定を変更

### Entry

