# 🔐生体認証の実装計画(Device > Biometric)

Device > Biometric の画面に生体認証のサンプルを作る。本人確認(認証の成否を受け取る)、生体認証で守る秘密(Android Keystore の鍵で暗号化して保存し、生体認証で復号して取り出す)、鍵での署名(サーバーでの認証の流れを、サーバーを使わずに端末の中で確かめる)の 3 つと、本人確認と鍵の小さな追加(4-5)。パスキー(Credential Manager + WebAuthn)は計画だけを書き、当面対応しない。番号は `Task_Checklist.md` の 4 と同じ。

| 番号 | 段階 | 内容 |
| --- | --- | --- |
| 4-1 | 本人確認 | 使えるかどうか(センサー無し / 未登録 / 一時的に使えない)の表示、認証(生体だけ / 画面ロックの PIN なども可)、結果(成功・キャンセル・失敗回数の超過など)の表示 |
| 4-2 | 生体認証で守る秘密 | 鍵の作成、文字列の暗号化と保存、生体認証での復号と表示、鍵と保存した値の削除、生体情報の登録が変わって鍵が使えなくなったときの作り直し |
| 4-3 | 鍵での署名(サーバー認証の疑似) | 署名の鍵の作成と公開鍵の登録、チャレンジへの生体認証付きの署名、登録した公開鍵での検証、改ざんと使い回しの検出、鍵の削除と作り直し。サーバーの役(公開鍵の登録・チャレンジの発行・署名の検証)は端末の中で行う |
| 4-5 | 本人確認と鍵の追加 | ダイアログのサブタイトルと説明・顔認証などの後の確認のボタンを省く設定、弱い生体認証、結果の方式(生体 / 画面ロック)、端末に合う文言、最後の本人確認からの時間、未登録のときの登録の画面、署名の鍵を StrongBox に置くことと置き場所の表示 |
| 4-4 | パスキー | 当面対応しない(採否は 4-4-0 の判断)。Credential Manager でパスキーを作ってログインする。サーバーの WebAuthn の API が要る |

ファイルパスは `Template.MobileApp/` からの相対。

## ⚖️決定事項

| 項目 | 決定 |
| --- | --- |
| 範囲 | 本人確認、端末の中で完結する秘密の保護(暗号化と復号)、鍵での署名と検証(サーバーは使わず、検証も端末の中で行う)。パスキーは計画だけ |
| 方式 | Android 本体の `android.hardware.biometrics` の `BiometricPrompt` / `BiometricManager` を直接使う薄いラッパー(`Components/Biometric.cs` + `.android.cs`)。minSdk 30 なので、`setAllowedAuthenticators` / `canAuthenticate(int)`(API 30)と鍵と結び付けた認証(`BiometricPrompt.CryptoObject`)が本体だけで使える。AndroidX のパッケージは足さない。ダイアログはシステムが出す(Activity は要らない)|
| 鍵 | Android Keystore の鍵。秘密(4-2)は AES-256 鍵(GCM)、署名(4-3)は EC 鍵(P-256、`SHA256withECDSA`)で、別名を分ける。どちらも使うたびに強い生体認証(クラス 3)を求め(`setUserAuthenticationParameters(0, AUTH_BIOMETRIC_STRONG)`)、生体情報の登録が変わると使えなくなる(`setInvalidatedByBiometricEnrollment(true)`)。署名の鍵は StrongBox があれば StrongBox に置く(無ければ TEE)|
| 検証 | 4-3 のサーバーの役(公開鍵の登録・チャレンジの発行・署名の検証)は画面の VM の内部クラス `BiometricSignInServer` にまとめる。検証は .NET の `ECDsa`(`ImportSubjectPublicKeyInfo` + `VerifyData`。Android の署名は DER 形式なので `DSASignatureFormat.Rfc3279DerSequence`)で行い、サーバーに移すときもそのまま使える形にする |
| 保存 | 暗号文と IV を Base64 で設定(`Settings`)に保存する。平文は保存しない |
| 本人確認の種類 | 強い生体認証だけ(`BIOMETRIC_STRONG`)、弱い生体認証(`BIOMETRIC_WEAK`。鍵には使えない)、画面ロックの PIN なども可(`BIOMETRIC_STRONG` + `DEVICE_CREDENTIAL`)を画面で切り替える |

## 📱画面

| 項目 | 内容 |
| --- | --- |
| 使えるかどうか | `BiometricManager.canAuthenticate` の結果を「使える」と 3 区分(センサー無し / 未登録 / 一時的に使えない)で出す。強い生体認証・弱い生体認証・画面ロックを分けて出す。最後の本人確認からの時間と、強い生体認証が未登録のときに設定の登録の画面を開くボタン |
| 本人確認 | 認証のボタン(強い生体認証 / 弱い生体認証 / 強い生体認証か画面ロック)と、結果(成功 / キャンセル / 失敗回数の超過で一時的に使えない / その他のエラー)と使われた方式(生体 / 画面ロック)。ダイアログにはサブタイトルと端末に合う説明を出し、顔認証などの後の確認のボタンは省く |
| 秘密 | 文字列の入力と「保存」(生体認証の後に暗号化して保存)、「取り出し」(生体認証の後に復号して表示。画面を離れたら消す)、「削除」(鍵と保存した値)。鍵が使えなくなったときはその旨を出し、削除から作り直せる |
| 署名 | 「Register」(鍵を作り、公開鍵をサーバーの役に登録する)、「Sign in」(チャレンジに生体認証付きで署名し、サーバーの役が検証する)、「Delete」(鍵と登録した公開鍵)。鍵の状態・置き場所(StrongBox / TEE)・公開鍵の指紋・チャレンジ・署名(16 進の先頭)・署名の結果を見出しと値のタイルの 2 列に、検証の結果 ✅ / ❌・1 バイト変えたチャレンジと同じチャレンジの 2 回目の結果(🛡️ 拒否)を 3 列に並べて出す。鍵が使えなくなったときはその旨を出し、登録し直せる。画面はスクロールしない高さに収める |

## 🔑鍵と暗号化の流れ

1. 保存: 鍵が無ければ作る → `Cipher`(`AES/GCM/NoPadding`)を暗号化で初期化 → `CryptoObject` を付けて生体認証 → 成功したら `doFinal` で暗号化 → 暗号文と IV を保存
2. 取り出し: 保存した IV で `Cipher` を復号で初期化 → `CryptoObject` を付けて生体認証 → 成功したら `doFinal` で復号して表示
3. 鍵が使えないとき: `Cipher` の初期化で `KeyPermanentlyInvalidatedException` → 鍵と保存した値を消し、作り直しを促す
4. 登録(署名): 署名の鍵を作る(あれば作り直す)→ 公開鍵(X.509 の SubjectPublicKeyInfo)を書き出し、サーバーの役に登録する(画面を開き直したときは、端末の鍵の公開鍵を登録済みとして読み直す)
5. 認証(署名): サーバーの役がチャレンジ(32 バイトの乱数)を出す → `Signature`(`SHA256withECDSA`)を秘密鍵で初期化 → `CryptoObject` を付けて生体認証 → 成功したらチャレンジに署名する(秘密鍵は Keystore の外に出ず、アプリが受け取るのは署名だけ)→ サーバーの役が登録した公開鍵で検証し、チャレンジを使い済みにする(結果によらず 1 回。同じチャレンジの 2 回目は失敗)
6. 署名の鍵が使えないとき: `Signature` の初期化で `KeyPermanentlyInvalidatedException` → 鍵を消し、登録し直しを促す

## 🏗️構成

共通のインターフェース + `*.android.cs`(MauiComponents の `WiFi.cs` + `WiFi.WiFiManager.cs` / `.android.cs` と同じ構成)。公開のメソッドは共通の側に置き、処理は `.android.cs` の static な `PlatformXxx`(MAUI Essentials と同じ形)。

| ファイル | 役割 |
| --- | --- |
| `Components/Biometric.cs` | 新規。`IBiometricAuthenticator`(使えるかどうか / 本人確認 / 秘密の暗号化と復号 / 署名の鍵の作成と公開鍵 / 署名 / 鍵の削除)と結果の種別 |
| `Components/Biometric.android.cs` | 新規。`BiometricManager` / `BiometricPrompt` / Android Keystore の実装 |
| `Converters/HexTextConverter.cs` | 新規。バイト列の 16 進の表示(先頭だけと全体のバイト数)|
| `Modules/Device/DeviceBiometricViewModel.cs` + `DeviceBiometricView.xaml` | 空の画面を実装する(4-3 のサーバーの役は VM の内部クラス)|
| `Modules/Device/DeviceMenuView.xaml` | `Grid.Row="7" Grid.Column="1"` のボタンの `IsEnabled="False"` を削除 |
| `Markup/AppIcons.cs` | ボタンのアイコン(`SmallFingerprint` / `SmallPassword` / `SmallKey` / `SmallLogin` / `SmallFace` / `SmallHowToReg`)|
| `State/Settings.cs` | 暗号文と IV |
| `MauiProgram.cs` | DI の登録 |
| `Platforms/Android/AndroidManifest.xml` | `USE_BIOMETRIC` |

## ⚠️制約

- 鍵と結び付けられるのは強い生体認証(クラス 3)だけ。顔認証がクラス 3 でない機種では、秘密の保存と取り出しに顔認証は使えない
- 画面ロックが無い端末では鍵を作れない(署名の鍵も同じで、使うたびに生体認証を求める鍵は生体情報を登録していないと作成で失敗する)。本人確認の成功・キャンセル・失敗回数の超過の確認には、画面ロックと生体情報を登録した端末が要る(登録と認証は利用者の操作)
- 本体の `BiometricManagerAuthenticators` は `[Flags]` の無い列挙で、`CanAuthenticate` / `SetAllowedAuthenticators` の引数は int。組み合わせは int にしてから行う
- 4-3 の検証は端末の中なので、なりすましの防止にはならない(仕組みを確かめる見本)。実際の認証には、サーバーでの公開鍵の登録と検証、鍵がハードウェアの中にあることの証明(Key Attestation の証明書チェーン)の確認が要る

## 🔗参照

| 種別 | URL | 概要 |
| --- | --- | --- |
| 公式 | https://developer.android.com/privacy-and-security/keystore?hl=ja | 「Android Keystore システム」。鍵の作成(`KeyGenParameterSpec`)、使うたびの本人確認を求める鍵、鍵を使える場所(TEE / StrongBox) |
| 公式 | https://developer.android.com/identity/credential-manager?hl=ja | 「認証情報マネージャーについて」。パスキーの作成とログイン(4-4) |
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

### ✍️4-3 鍵での署名(サーバー認証の疑似)

署名の鍵の作成と削除、公開鍵の書き出し、生体認証付きの署名(`Components/Biometric`)。サーバーの役と検証(VM)、画面の「署名」。

確認: 登録して認証すると ✅、1 バイト変えたチャレンジでは ❌、同じチャレンジの 2 回目は ❌、キャンセルでは署名しない、削除の後は登録し直す、生体情報の登録を足すと鍵が使えなくなり登録し直しを促す。鍵を作るには、画面ロックと指紋を登録した端末が要る。

実装済み(2026-09-30)。画面ロックと指紋の無い実機で、鍵が「無し」でボタンが押せない表示と、鍵が無いときの API の動き(状態・作成・公開鍵・署名・削除。鍵の作成は例外を捕まえて null)を確認した。サーバーの役の検証(受け付け・改ざん・使い回し・別の鍵・形式の違い)は、PC で .NET の鍵の DER 形式の署名で確かめた。登録と署名の成功・キャンセル・生体情報の登録を足したときの無効化は、登録した端末での確認が残る。

### ➕4-5 本人確認と鍵の追加

`Components/Biometric` に、ダイアログの文言(`BiometricPromptText`: サブタイトル・説明・確認のボタン)、弱い生体認証、結果の方式(`BiometricAuthenticationResult`)、端末に合う文言(`GetLabels`。Android 12 から)、最後の本人確認からの時間(`GetTimeSinceLastAuthentication`。Android 15 から)、登録の画面(`OpenEnrollment`)、鍵の置き場所(`GetSigningKeyLocation`)を足し、署名の鍵を StrongBox に置く。画面は Availability・Authenticate・Key sign in に足し、スクロールしない高さのままにする。

確認: Weak での本人確認、結果の方式、Enroll で設定の登録の画面が開く、最後の本人確認からの時間、ダイアログのサブタイトルと説明、鍵の置き場所(StrongBox)。

実装済み(2026-09-30)。ビルドと静的解析まで。実機での画面の確認が残り、成功の流れと鍵の置き場所は、画面ロックと指紋を登録した端末での確認になる。

### 🔑4-4 パスキー(当面対応しない)

Credential Manager(`androidx.credentials`)でパスキーを作り、パスキーでログインする。4-3 と同じく公開鍵での署名と検証だが、鍵の作成・保管・同期は Google パスワード マネージャーが行い、やり取りは WebAuthn の形にそろう。採否は `Task_Checklist.md` の 4-4-0 で判断する。

| 前提 | 内容 |
| --- | --- |
| サーバー | WebAuthn の登録(オプションの発行と、証明の検証と公開鍵の保存)と認証(チャレンジの発行と署名の検証)の API(template-maui-server。.NET の WebAuthn のライブラリを使う) |
| アプリとドメインの関連付け | サーバーのドメイン(Relying Party の ID)の `/.well-known/assetlinks.json` に、アプリのパッケージ名と署名の証明書の指紋を載せる |
| 端末 | Google アカウントと画面ロック |
| パッケージ | `androidx.credentials`(`Xamarin.AndroidX.Credentials`)のダウンロードが要る |

## 🚫対象外

| 項目 | 内容 |
| --- | --- |
| サーバーとの連携 | 公開鍵の登録・チャレンジの発行・署名の検証の API(4-3 は端末の中で疑似的に行う)|
| iOS | Face ID / Touch ID |
