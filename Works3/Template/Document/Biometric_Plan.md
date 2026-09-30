# 🔐生体認証の実装計画(Device > Biometric)

Device > Biometric の画面に生体認証のサンプルを作る。本人確認(認証の成否を受け取る)と、生体認証で守る秘密(Android Keystore の鍵で暗号化して保存し、生体認証で復号して取り出す)の 2 つ。番号は `Task_Checklist.md` の 4 と同じ。

| 番号 | 段階 | 内容 |
| --- | --- | --- |
| 4-1 | 本人確認 | 使えるかどうか(センサー無し / 未登録 / 一時的に使えない)の表示、認証(生体だけ / 画面ロックの PIN なども可)、結果(成功・キャンセル・失敗回数の超過など)の表示 |
| 4-2 | 生体認証で守る秘密 | 鍵の作成、文字列の暗号化と保存、生体認証での復号と表示、鍵と保存した値の削除、生体情報の登録が変わって鍵が使えなくなったときの作り直し |

ファイルパスは `Template.MobileApp/` からの相対。

## ⚖️決定事項

| 項目 | 決定 |
| --- | --- |
| 範囲 | 本人確認と、端末の中で完結する秘密の保護(暗号化と復号)|
| 方式 | Android 本体の `android.hardware.biometrics` の `BiometricPrompt` / `BiometricManager` を直接使う薄いラッパー(`Components/Biometric.cs` + `.android.cs`)。minSdk 30 なので、`setAllowedAuthenticators` / `canAuthenticate(int)`(API 30)と鍵と結び付けた認証(`BiometricPrompt.CryptoObject`)が本体だけで使える。AndroidX のパッケージは足さない。ダイアログはシステムが出す(Activity は要らない)|
| 鍵 | Android Keystore の AES-256 鍵(GCM)。使うたびに強い生体認証(クラス 3)を求め(`setUserAuthenticationParameters(0, AUTH_BIOMETRIC_STRONG)`)、生体情報の登録が変わると使えなくなる(`setInvalidatedByBiometricEnrollment(true)`)|
| 保存 | 暗号文と IV を Base64 で設定(`Settings`)に保存する。平文は保存しない |
| 本人確認の種類 | 生体だけ(`BIOMETRIC_STRONG`)と、画面ロックの PIN なども可(`BIOMETRIC_STRONG` + `DEVICE_CREDENTIAL`)を画面で切り替える |

## 📱画面

| 項目 | 内容 |
| --- | --- |
| 使えるかどうか | `BiometricManager.canAuthenticate` の結果を「使える」と 3 区分(センサー無し / 未登録 / 一時的に使えない)で出す。強い生体認証と画面ロックの PIN を分けて出す |
| 本人確認 | 認証のボタンと、結果(成功 / キャンセル / 失敗回数の超過で一時的に使えない / その他のエラー)|
| 秘密 | 文字列の入力と「保存」(生体認証の後に暗号化して保存)、「取り出し」(生体認証の後に復号して表示。画面を離れたら消す)、「削除」(鍵と保存した値)。鍵が使えなくなったときはその旨を出し、削除から作り直せる |

## 🔑鍵と暗号化の流れ

1. 保存: 鍵が無ければ作る → `Cipher`(`AES/GCM/NoPadding`)を暗号化で初期化 → `CryptoObject` を付けて生体認証 → 成功したら `doFinal` で暗号化 → 暗号文と IV を保存
2. 取り出し: 保存した IV で `Cipher` を復号で初期化 → `CryptoObject` を付けて生体認証 → 成功したら `doFinal` で復号して表示
3. 鍵が使えないとき: `Cipher` の初期化で `KeyPermanentlyInvalidatedException` → 鍵と保存した値を消し、作り直しを促す

## 🏗️構成

共通のインターフェース + `*.android.cs`(MauiComponents の `WiFi.cs` + `WiFi.WiFiManager.cs` / `.android.cs` と同じ構成)。

| ファイル | 役割 |
| --- | --- |
| `Components/Biometric.cs` | 新規。`IBiometricAuthenticator`(使えるかどうか / 本人確認 / 秘密の暗号化と復号 / 鍵の削除)と結果の種別 |
| `Components/Biometric.android.cs` | 新規。`BiometricManager` / `BiometricPrompt` / Android Keystore の実装 |
| `Modules/Device/DeviceBiometricViewModel.cs` + `DeviceBiometricView.xaml` | 空の画面を実装する |
| `Modules/Device/DeviceMenuView.xaml` | `Grid.Row="7" Grid.Column="1"` のボタンの `IsEnabled="False"` を削除 |
| `Markup/AppIcons.cs` | ボタンのアイコン(`SmallFingerprint` / `SmallPassword`)|
| `State/Settings.cs` | 暗号文と IV |
| `MauiProgram.cs` | DI の登録 |
| `Platforms/Android/AndroidManifest.xml` | `USE_BIOMETRIC` |

## ⚠️制約

- 鍵と結び付けられるのは強い生体認証(クラス 3)だけ。顔認証がクラス 3 でない機種では、秘密の保存と取り出しに顔認証は使えない
- 画面ロックが無い端末では鍵を作れない。本人確認の成功・キャンセル・失敗回数の超過の確認には、画面ロックと生体情報を登録した端末が要る(登録と認証は利用者の操作)
- 本体の `BiometricManagerAuthenticators` は `[Flags]` の無い列挙で、`CanAuthenticate` / `SetAllowedAuthenticators` の引数は int。組み合わせは int にしてから行う

## 🔗参照

| 種別 | URL | 概要 |
| --- | --- | --- |
| 公式 | https://developer.android.com/training/sign-in/biometric-auth?hl=ja | 「生体認証ダイアログを表示する」。`BiometricManager.canAuthenticate`(`BIOMETRIC_SUCCESS` / `ERROR_NO_HARDWARE` / `ERROR_NONE_ENROLLED` / `ERROR_HW_UNAVAILABLE`)、`BiometricPrompt.PromptInfo`、認証のコールバック、`CryptoObject` |
| ライブラリ | https://github.com/oscoreio/Maui.Biometric | 使えるかどうかの区分(`NoSensor` / `NoBiometric` / `TemporaryUnavailable`)と認証の結果の対応付け。`sample/MainViewModel.cs` が表示 + 認証 + 結果の最小例 |
| ソース | `Bio`(`Maui.Biometric-main` / `MauiBiometricPluginSample-main` / `NET-MAUI-FingerPrint-main`)| 上のライブラリと MAUI のサンプル |

## 🪜段階ごとの作業

### 🔍4-1 本人確認

`Components/Biometric`(使えるかどうか・本人確認)、画面の「使えるかどうか」と「本人確認」、メニューのボタン、権限とパッケージ。

確認: 生体情報を登録した実機で成功・キャンセル・失敗回数の超過、画面ロックの PIN での認証。センサー無しと未登録の表示はエミュレーターで確かめる。

実装済み(2026-09-29)。画面ロックと指紋の無い実機とエミュレーターで、「未登録」の表示と、本人確認がエラーで返ることを確認した。成功・キャンセル・失敗回数の超過・PIN は、登録した端末での確認が残る。

### 🔒4-2 生体認証で守る秘密

鍵の作成と削除、暗号化と復号、画面の「秘密」。

確認: 保存して取り出すと同じ文字列、キャンセルでは保存も表示もしない、削除、生体情報の登録を足すと鍵が使えなくなり作り直しを促す(エミュレーターで確かめる)。

## 🚫対象外

| 項目 | 内容 |
| --- | --- |
| サーバーとの連携 | 鍵での署名とサーバーでの検証 |
| パスキー | Credential Manager によるログイン |
| iOS | Face ID / Touch ID |
