# Template project for MAUI

MAUI用テンプレートプロジェクトの使用方法について記述する。

# 🎉始め方

## ビルド

テンプレートプロジェクトを取得しVisual Studioでビルドを行なうと機能サンプルの入ったテンプレートを作成できる。  
テンプレートプロジェクトはそのままAndroidで実行可能なので、動作確認を行ないながらソースを参照して構造を理解する。  

## Flavor

単一ソースでHW固有機能を使用する複数モデルのビルドに対応する場合、設定ファイルで対象とするHW用の定義を切り替えてビルドを行なう。  
プロジェクト中のDeviceProfile.sample.propsファイルを.DeviceProfile.propsとしてコピーすると、その内容がビルドに反映されるような設定となっている。  

なお、.DeviceProfile.propsは各ユーザーが開発環境でのみ使用するものとし、CI環境でリリース物をビルドする場合にはビルドオプションで同様の指定を行なう形とする。  

.DeviceProfile.props中では以下の項目を設定する。

* DefineConstants

DefineConstantsを設定するとプリプロセッサで使用される条件付きコンパイルの定義を設定できる。  
物理キーボードの有無や、使用するバーコードライブラリの違い等はこの設定で条件付きコンパイルにより切り替える形とする。  

* EmbeddedBuildProperty

商用版とテスト版といったアプリケーションのフレーバーや、接続先に関するシークレット情報等はEmbeddedBuildPropertyで指定すると、指定した内容がアプリケーション中から参照可能となる。  
プロジェクト中のVariants.csのように、BuildProperty属性を指定したメソッドでビルド時にEmbeddedBuildPropertyによって指定した値が参照可能となる。  

# 🌱新規プロジェクト作成

テンプレートプロジェクトを元に新規プロジェクトを作成する手順について記述する。  

## 名称変更

ソース中の「Template.MobileApp」の文言をアプリケーション固有のものに変更する。  
推奨する名称は「(システム名).MobileApp」、「(顧客企業名).(システム名).MobileApp」等。  

また、csproj中にあるApplicationTitle、ApplicationIdも併せて名称の変更を行なう。  

## 不要ライブラリ参照削除

テンプレートプロジェクトでは各種機能のサンプルを用意しているが、使用しない項目がある場合はライブラリへの参照を削除する。  
詳細は使用ライブラリの項目を参照。  

## 不要ソース削除

テンプレートプロジェクトでは各種機能のサンプルを用意しているが、不要な機能のサンプルや画面については削除してプロジェクトのベースとする。  
なお、Behaviors等についてはサンプルの実装を流用しても問題はない。  
以下に削除可能なソースについて記述する。  

### 削除可能ソース一覧

|フォルダ|概要|補足|
|:----|:----|:----|
|Behaviors|不要サンプル削除|全削除でも問題はない|
|Components|不要サンプル削除|後述|
|Controls|不要サンプル削除|全削除でも問題はない|
|Converters|不要サンプル削除|全削除でも問題はない|
|Domain|不要サンプル削除|全削除でも問題はない|
|Extender|不要拡張削除|後述|
|Helper|不要サンプル削除|後述|
|Input|物理キー制御|後述|
|Messaging|不要サンプル削除|全削除でも問題はない|
|Models|不要サンプル削除|全削除でも問題はない|
|Modules|不要サンプル削除|後述|
|Services|不要サンプル削除|後述|
|Ussecase|不要サンプル削除|後述|

### Components

StorageManagerは汎用的に使用するが、それ以外不要なものは削除しても問題ない。  

### Extender

画面遷移ライブラリのフォーカス制御拡張だが、キーパッドが無い機種の開発では削除しても問題ない。  

### Helper

Data、JsonはそれぞれDB、Web API用のヘルパーなので、それらの機能を未使用時は削除。  

### Input

キーパッドが無い機種向けの開発では削除。  

### Modules

各画面のサンプルなので、初期画面であるMain下のMenuView以外のサブフォルダは削除して問題ない。  
また、ダイアログを使用しない場合はAppDialogViewModelBase.cs、DialogId.cs、DialogSize.cs、PopupNavigatorExtensions.csは削除可能。  

### Services/Ussecase

未使用な機能のものについては削除で良いが、使用するものについてはサンプルのメソッド削除して使用する。
また、単純なアプリケーションの場合、Ussecaseは使用せずにServicesのみを使用する形でも問題ない。

## その他リソース

Resourceフォルダ中の項目から不要なリソースの削除及び変更を行なう。  
対象項目は以下。  

|フォルダ|概要|変更内容|
|:----|:----|:----|
|AppIcon|アプリケーション用アイコン|アプリケーション用に変更|
|Fonts|カスタムフォント|不要フォント削除|
|Images|画像|不要画像削除|
|Raw|その他リソースファイル|不要リソース削除|
|Splash|スプラッシュ用画像|アプリケーション用に変更|

# 📕アプリケーション構造

テンプレートプロジェクトのアプリケーション構造について記述する。  
この構成をアプリケーションを構築する際のスタンダードとする  

## プロジェクト直下

(📝作成中)




## Behaviors

(📝作成中)




## Components

(📝作成中)




## Controls

(📝作成中)




## Converters

(📝作成中)




## Domain

(📝作成中)





## Extender

(📝作成中)




## Helpers

(📝作成中)




## Input

(📝作成中)




## Markup

(📝作成中)





## Messaging

(📝作成中)




## Models

(📝作成中)




## Modukles

(📝作成中)





## Platforms

(📝作成中)




## Services

(📝作成中)





## Shell

(📝作成中)




## State

(📝作成中)




## Usecase

(📝作成中)





# 📦使用ライブラリ

使用するライブラリについて記述する。

## 一覧

|名称|用途|必須|自前|
|:----|:----|:----|:----|
|Camera.MAUI|カメラ|カメラ未使用時は不要| |
|CommunityToolkit.Maui|MAUI準公式| | |
|Components.Maui|MAUI各主機能| |○|
|Microsoft.AppCenter.*|AppCenter|AppCenter未使用時は不要| |
|Microsoft.Data.Sqlite|SQLite|DB未使用時は不要| |
|Microsoft.Extensions.Logging.Debug|ログ出力| | |
|QRCoder|QR機能|QR出力がない場合は不要| |
|Rester|Web API|API未使用時は不要|○|
|System.Interactive|LINQ拡張| | |
|System.Linq.Async|LINQ拡張| | |
|System.Reactive|Rx| | |
|Usa.Smart.Core|共通ライブラリ| |○|
|Usa.Smart.Converter|型変換| |○|
|Usa.Smart.Data.*|データアクセス|DB未使用時は不要|○|
|Usa.Smart.Mapper|オブジェクトマッパー| |○|
|Usa.Smart.Maui.*|MAUI固有機能| |○|
|Usa.Smart.Navigation.*|画面遷移| |○|
|Usa.Smart.Reactive.*|Rx| |○|
|Usa.Smart.Resolver.*|Dependency Injection拡張| |○|

なお、必須としないライブラリについては、それに関する機能が必要ない場合には参照を削除する。  

## MAUI

### CommunityToolkit.Maui [[ドキュメント]](https://learn.microsoft.com/ja-jp/dotnet/communitytoolkit/maui/)

MAUI準公式の拡張機能としてCommunityToolkit.Mauiを使用する。

## System

### System.Interactive [[公式]](https://github.com/dotnet/reactive)

LINQ補助。  

### System.Linq.Async [[公式]](https://github.com/dotnet/reactive)

非同期LINQ。  
非同期処理はWebでもMAUIでも使用するので必須とする。  

### System.Reactive [[公式]](https://github.com/dotnet/reactive)

Reactive Extensions。  
WPF、MAUI等でのイベントの取り扱いに必須となる。  
Rx自体については https://reactivex.io/ を参照。  

## Microsoft

### Microsoft.Extensions.Logging [[ドキュメント]](https://learn.microsoft.com/ja-jp/dotnet/core/extensions/logging)

アプリケーションからのログ出力は自前で作るのではなくILoggerインターフェースを使用して行なう。  
Logging Providerの実装としては、MauiComponentsでAndroidのLogcatへ出力するものとファイルへの出力を行なうものを用意しており、それを使用する。  

### Microsoft.AppCenter [[公式]](https://azure.microsoft.com/ja-jp/products/app-center/)

クラッシュレポート、監視に利用する。  
コンシューマー用途等ではモニタリングは必須とする。  

### Microsoft.Data.Sqlite [[ドキュメント]](https://learn.microsoft.com/ja-jp/dotnet/standard/data/sqlite/)

端末内のデータベースとしてはSQLiteを使用し、ライブラリとしてはPC側と同じくMicrosoft.Data.Sqliteを使用する。  
Microsoft.Data.SqliteはADO.NET標準のインターフェースを実装しており、Usa.Smart.Data.Mapperをはじめ一般的なデータアクセスライブラリが使用可能である。

## デバイス

(📝作成中)

|Camera.MAUI|カメラ|カメラ未使用時は不要| |
|QRCoder|QR機能|QR出力がない場合は不要| |



## Smart

(📝作成中)

GitLabのリポジトリを参照


|Usa.Smart.Maui.*|MAUI固有機能| |○|

|Usa.Smart.Converter|オブジェクトマッパー| |○|
|Usa.Smart.Data.*|データアクセス|DB未使用時は不要|○|
|Usa.Smart.Mapper|オブジェクトマッパー| |○|
|Usa.Smart.Navigation.*|画面遷移| |○|
|Usa.Smart.Reactive.*|Rx| |○|
|Usa.Smart.Resolver.*|Dependency Injection拡張| |○|

|Components.Maui|MAUI各主機能| |○|
|Rester|Web API|API未使用時は不要|○|

## その他

テンプレート中には含めていないが採用を検討するライブラリについて。

- System.Text.Encoding.CodePages/Smart.Text.Japanese SJISを扱う必要がある場合
- Shiny (BLE、プッシュ通信等)

# 🎨アーキテクチャ

MAUIでアプリケーションを作成する際に必要となる知識及びアーキテクチャの方針について記述する。  

## XAML

(📝作成中)




## MVVM

(📝作成中)




## Messaging

(📝作成中)




## 非同期処理

(📝作成中)




## Reactive

(📝作成中)




# 🔖機能サンプル

(📝作成中)



# 🚧未実装機能

テンプレートの機能サンプルとして未実装の機能について記述する。  
これらの機能が必要になった時は先に相談すること。  

- QR
- Camera
- NFC
- WiFi情報
- Bluetoothe(プリンター等)
- BLE(センサー値の取得等)
- Audio再生
- 指紋認証
- 地図
- Push通知
- AIサービス
- チャート/グラフ
- SSH/FTP

# 開発TIPS

開発手法に関するTIPSを記述する。  
Analyzers及びEditorConfigについてはテンプレートプロジェクトにおいて設定済みとなっている。  

## CI

開発時はJenkinsを用いてCIを行う。  
CIでは静的チェックツールを使用し、品質を確保する。  

以降に記述するツールを使用し、ソース更新時は常時ビルドと静的チェックを行なうようにする。  
また、成果物についてはCIを使用してビルドしたもののみを正とする。  

静的チェックツールとして使用するAnalyzersについてはDirectory.Build.propsを使用して全プロジェクトに適用する。  
静的チェックの項目のうち、適合しないものはルールを無効化し、特定ケースのみマッチしないものについてはSuppressMessageして、「ルール自体は緩くするが警告は0」形を基本とする。  
CIの静的チェックで警告が出力された場合には優先して対処する。  

## .NET ソース コード分析

AnalysisModeの設定によりビルド時の静的チェックを有効にする。  

- https://learn.microsoft.com/ja-jp/dotnet/fundamentals/code-analysis/overview

## InspectCode

InspectCode(JetBrains.ReSharper.CommandLineTools)を使用して静的チェックを行なう。  

- https://www.nuget.org/packages/JetBrains.ReSharper.CommandLineTools
- https://pleiades.io/help/resharper/InspectCode.html

## StyleCop.Analyzers

コードスタイル統一のチェックにはStyleCop.Analyzersを使用する。  

## EditorConfig

書式の統一はVisual Studioが標準で対応しているEditorConfigを使用する。  

### Xaml Styler

XAML記述の統一にはXaml Stylerを使用する。  
Visual Studioの拡張機能としてインストールすること。  
