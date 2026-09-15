# 別アプリケーションでの導入検討項目

本サンプル(Template)では対象外とするが、別のアプリケーションでは導入を検討する項目の一覧。`Task_Checklist.md` から外した項目を、導入時に必要な情報(参照 / 実装範囲 / 本テンプレートでの実装位置)ごと移す。

| 項目 | 概要 | 参照 |
| --- | --- | --- |
| ディープリンク(App Links / カスタムスキーム) | URL からアプリを起動し、指定画面へ遷移する | 下記 |

## ディープリンク(App Links / カスタムスキーム)

参照: https://github.com/redth/maui.applinks.sample — `MainActivity` に `[IntentFilter(new[] { Intent.ActionView }, AutoVerify = true, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataScheme = "https", DataHost = "...")]` を付けると、URL は MAUI 側が `Application.OnAppLinkRequestReceived(Uri)` に渡す。`https` リンクの検証には Google Search Console でのドメイン所有確認と `/.well-known/assetlinks.json`(パッケージ名 + 署名 SHA-256)の配置が必要。iOS は Universal Links(`apple-app-site-association`)。

導入時の範囲: カスタムスキーム(例 `template://device/nfc`)でアプリを起動し、初期遷移(`StartupState`)の完了後に該当画面へ遷移する(ローカル通知のタップを起動後に処理する `TakePendingTap` と同じ流れ)。`https` の検証リンク(`assetlinks.json`)は配布ドメインが決まった時点で追加する。

本テンプレートで変更する位置(パスは `Template.MobileApp/` からの相対):

| ファイル | 何用か | 変更 |
| --- | --- | --- |
| `Platforms/Android/MainActivity.cs` | 起動 Intent の受け口(`LaunchMode = SingleInstance`) | `[IntentFilter]`(`ActionView` + `CategoryDefault` / `CategoryBrowsable` + `DataScheme = "template"`)を追加 |
| `App.xaml.cs` | アプリ | `OnAppLinkRequestReceived(Uri)` をオーバーライドし、URI を保持 / 通知 |
| `MainPageViewModel.cs` | 初期遷移 | 初期遷移の完了後に保持した URI の画面へ遷移 |
| `Modules/ViewId.cs` | 画面 ID | URI のパスから `ViewId` への対応表 |
| `Document/Development.md` | 手順 | `adb shell am start -a android.intent.action.VIEW -d "template://device/nfc"` の確認手順 |

確認: 未起動 / 起動中(`OnNewIntent`)の両方で該当画面へ遷移すること。
