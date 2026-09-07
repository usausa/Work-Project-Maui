# 残作業チェックリスト

実装フェーズは全完了(2026-09-03)。**残作業(実機確認 / 実テスト / 画像アセット / 保留)を 1 本で管理する**マスターチェックリスト。
2026-09-03 に `UI_Verification_Checklist.md` + `Implementation_Checklist.md` + 旧 `UI_Task_Checklist.md` + `Image_Asset_Expansion_Plan.md` を統合した。
経緯・実装内容・ナレッジ・開発ポリシーは `Change_Summary.md`(付録含む)を参照。

**2026-09-06: 実機確認(旧 1 節の 1-1〜1-12)が完了**したため、確認 OK の記述は削除し **NG 項目だけを 1 節に残した**。同日、**7 節(Smart ライブラリの活用・予備調査済み)を追加**。
**現在の優先事項 = 1 節(実機確認の NG 5 件)→ 7 節(Smart ライブラリ活用)→ 6 節(`tmpl-plan-maui.md` 移管課題)**。以降は 2〜5 節。
※ 7 節は着手前に決める【判断】項目あり(7-3-0 パッケージ欠落 / 7-4-0 採用可否)。

## 運用ルール

- 作業はこの番号で指示・進行する(例:「4-4-A を実施」)。完了した項目は `[x]` にし、行末に完了日を追記。問題があれば行末にメモ
- **【判断】印の項目はユーザーが決定**(勝手に進めない)。デザイン判断を伴う差分は 1 項目ずつ指示を受けて実施
- 実装・変更を行なう場合の完了条件 = **ビルド警告ゼロ** + `Change_Summary.md` への記録(開発ポリシーは同 付録A)
- コミットはユーザーが実施(グループ単位を推奨)
- 描画・性能の計測は **Release ビルド + 実機**(手順は `Development.md` の「Releaseビルドでの検証と計測」)

## 前提(環境)

- **環境制約 (不具合ではない)**: ①地図タイルは Google Maps API キー未設定だと非表示 (ピン・カメラ移動は動作) ②SampleCvNet 系は AI エンドポイント未設定だと画面に入れない ③CommunityToolkit CameraView の `CaptureAsync` がまれに未完了になり Function キーが無反応化 (再起動で回復)
- 現在実機に入っているのは **Debug ビルド**(2026-09-06 の DI ファクトリ検証時にデプロイ。性能・描画の確認時は Release へ入れ替える)

---

## 1. 実機確認で判明した要対応項目

**フェーズ1〜9 + メニュー再編 + DI 移行の実機確認は 2026-09-06 に完了**(確認 OK の項目は本節から削除した。確認内容の詳細は `Change_Summary.md` の各区間を参照)。
以下は確認で **NG** となった項目。**対応は未着手**。

- [ ] **1-1** Basic > Behavior: **`MaskedBehavior` が正しく動作しない**(電話番号の `000-0000-0000` 整形が効かない)。原因調査から
- [ ] **1-2** Basic > Validation: パスワード一致の相関検証が**フォーカスを外した時のみ**動く → **入力が変わる度**に検証する方式へ
- [ ] **1-3** App > Sudoku: 盤面の**線が描画されていない箇所がある**
- [ ] **1-4** App > Calculator: **ボタンをもう少し大きく**したい(上部に余白があるため活用できる)
- [ ] **1-5** View > DragDrop: **ドロップ先が分かりやすい**表現がほしい(現状はゴミ箱のみハイライト)

---

## 2. SCP の実機確認と転送実テスト(要 SSH サーバ)

フェーズ6(2026-09-02)の SCP は SSH サーバが必要なため未確認のまま。
※同フェーズ同梱の Scene ダブルバッファ既定 ON は、2026-09-05 の描画バグ修正時に 4 画面(Flight/Tactical/Telemetry/Energy)とも確認済み。

- [ ] **2-1** Network メニュー: 「SCP」が追加され遷移できる(未設定時は接続先が「未設定 (設定画面の QR で投入)」でボタン無効)
- [ ] **2-2** Main > Setting: 項目の**ラベルと現在値が横並び**で表示される。SCP セクション(Host/User/Password)があり、QR(`ScpHost=...` 形式)を読むと反映される
- [ ] **2-3** Network > SCP: QR 投入後、「アップロード」でファイル選択 → 進捗バー → 完了ログ。「ダウンロード」で同ファイルがキャッシュへ取得される。転送中「キャンセル」で中断。接続後にサーバのホスト鍵指紋が参考表示される(照合は行わない)

---

## 3. 他案件の残課題(旧 Fix_Checklist.md 第 1 部より)

### 3-1. 未対応(優先度低)

- [ ] `SecureStorage.GetAsync` の復元・キーストア無効化時の例外が未捕捉 (エッジケース)
- [ ] `HttpService` の `CancellationToken` を `NetworkOperator` のデリゲート型経由で呼び出し側から渡せるようにする (現在は口が無く未使用。転送は 10 分の有限タイムアウトで暫定対応済み)

### 3-2. 実機検証が必要な項目(実物・サーバが必要)

| # | 内容 | 関連 |
| --- | --- | --- |
| 1 | NFC: タグ複数回読取→画面離脱→再入場。途中で離しても継続 | NFC 例外処理 |
| 2 | Bluetooth: ペアリング相手ありでの印刷成功パス | 状態復帰 |
| 3 | Android 11 実機でのレガシー Bluetooth 権限 | Manifest |
| 4 | Network: 大容量 Download/Upload の完走・進捗、サーバ停止時のリトライ上限 | 転送・リトライ |
| 5 | サーバ時刻表示の TZ (`ToLocalTime()` 追加済みだが要 API サーバ) | DateTime |
| 6 | CV サンプル: キャプチャ→検出の繰り返しでメモリ増加なし | SKBitmap |
| 7 | 端末 TZ を変えて DB 保存→表示 | DateTime |

---

## 4. 画像アセット拡充(素材待ち)

サンプル画面のプレースホルダ画像(profile.jpg の三重使い回し、縦長 social_background.png の 15 箇所流用、usa キャラ絵の商品転用)を専用画像に差し替える(2026-08-01 策定の `Image_Asset_Expansion_Plan.md` は 2026-09-03 に本節へ統合・削除)。

### 4-0. 指針(命名・配置・フォーマット)

- 配置先: XAML の `<Image Source>` から使う → `Resources/Images/`(MauiImage・密度別を自動生成)。コードで `OpenAppPackageFileAsync` から読む → `Resources/Raw/<カテゴリ>/`(MauiAsset・`LogicalName` はカテゴリ相対パス。例 `Social/player.jpg`)
- 命名(MauiImage は厳格): **小文字のみ・先頭は英字・使用可は英数字と `_`**(`-`・大文字・空白・日本語は不可)。連番はゼロ埋め 2 桁
- フォーマット: 写真=`.jpg`(品質 80 前後)/ 透過・図版・ロゴ=`.png` / ベクタで済む UI アイコンは `.svg`
- サイズ: 「一番大きく表示される箇所の dp × 3」を 1 枚用意(過大な原画はビルド/実行を重くする)。**表示スロットのアスペクト比に合わせるのが最重要**(縦長→横長流用の切り抜け問題の再発防止)
- **フォルダ構成(2026-09-03 階層化済み)**: `Resources/Images/` = Banner / Character / Chat / Common / Login / Onboard / Pet / Profile / Shop / Stream の用途別 10 フォルダ(Raw と同じ PascalCase)(csproj の `MauiImage` は `Resources\Images\**`)。**参照はフォルダ名を含まないファイル名のみ**のため全体で重複名不可
- **現状維持と判断済み(差し替え不要)**: UICharacter の usa 系(キャラ用途に合致)/ UIChat のスタンプ / UISocial の通貨・資源アイコン / UIDock のデッキボタン / UIMail の genbaneko・usausa

### 4-1.【判断】候補ファイル名の確定

- [ ] ファイル名を確定する(現候補: `avatar_user` / `profile_cover` / `gallery` / `product_apparel` / `product_beauty` / `poster` / `stream_hero` / `stream_clip` / `onboard` / `pet` / `banner` / `avatar_person` / `login_hero`)
  - 変更例: `poster`→`movie`、`gallery`→`photo` など。確定後、本書 4-3/4-4 の名前を一括更新する。
  - 制約: 小文字・先頭英字・英数字と `_` のみ(MauiImage)。

### 4-2.【判断】画像の作成手段

- [ ] 作成手段を決定する:
  - (a) 素材支給を待つ(従来方針。作成はスコープ外)
  - (b) Claude がプレースホルダをプログラム生成(正しいアスペクト比・スロット別に区別できる内容。後日、本素材へ同名差し替え可能)
  - (a)(b) 併用(例: ★★★ は支給・★★ 以下は生成)も可。

### 4-3. 画像の用意(新規 42 枚+差し替え 2 枚)

**前準備済み(2026-09-03)**: 42 枚すべてに**既存画像コピーのプレースホルダを配置済み**(比率・内容は仮)。実素材は各カテゴリフォルダへ**同名上書き**で反映できる。下記チェックは実素材に差し替えたら付ける。
作成できたファイルにチェック。サイズは「最大表示 dp×3」基準・**アスペクト比がスロットと一致していることが最重要**。
写真=`.jpg`(品質 80)、透過=`.png`(規約は 4-0)。

#### ★★★ プロフィール(8 枚)
- [ ] `avatar_user.jpg` — 512×512 / 1:1(自分のアバター。人物ポートレート/顔アイコン)
- [ ] `profile_cover.jpg` — 1600×800 / 2:1(カバー。横長の風景・抽象・グラデ)
- [ ] `gallery01.jpg` — 1000×1000 / 1:1(投稿写真: 旅行/料理/風景/日常)
- [ ] `gallery02.jpg` — 1000×1000 / 1:1
- [ ] `gallery03.jpg` — 1000×1000 / 1:1
- [ ] `gallery04.jpg` — 1000×1000 / 1:1
- [ ] `gallery05.jpg` — 1000×1000 / 1:1
- [ ] `gallery06.jpg` — 1000×1000 / 1:1

#### ★★★ ショッピング(9 枚)
- [ ] `product_apparel01.jpg` — 900×1200 / 3:4(ドレス。スタジオ物撮り縦位置)
- [ ] `product_apparel02.jpg` — 900×1200 / 3:4(ジャケット)
- [ ] `product_apparel03.jpg` — 900×1200 / 3:4(帽子)
- [ ] `product_beauty01.jpg` — 800×800 / 1:1(美容液 Aqua Serum)
- [ ] `product_beauty02.jpg` — 800×800 / 1:1(口紅 Velvet Lip)
- [ ] `product_beauty03.jpg` — 800×800 / 1:1(クリーム Glow Cream)
- [ ] `product_beauty04.jpg` — 800×800 / 1:1(ミスト Pure Mist)
- [ ] `product_beauty05.jpg` — 800×800 / 1:1(マスク Silky Mask)
- [ ] `product_beauty06.jpg` — 800×800 / 1:1(チーク Petal Blush)

#### ★★★ 動画配信(10 枚)
- [ ] `poster01.jpg` — 600×900 / 2:3(作品ポスター。各作品で異なるビジュアル)
- [ ] `poster02.jpg` — 600×900 / 2:3
- [ ] `poster03.jpg` — 600×900 / 2:3
- [ ] `poster04.jpg` — 600×900 / 2:3
- [ ] `poster05.jpg` — 600×900 / 2:3
- [ ] `poster06.jpg` — 600×900 / 2:3
- [ ] `stream_hero.jpg` — 1600×900 / 16:9(ヒーロー/詳細トップのキービジュアル)
- [ ] `stream_clip01.jpg` — 1280×720 / 16:9(予告編サムネ)
- [ ] `stream_clip02.jpg` — 1280×720 / 16:9
- [ ] `stream_clip03.jpg` — 1280×720 / 16:9

#### ★★ オンボーディング(3 枚)
- [ ] `onboard01.jpg` — 1080×1080 / 1:1(Welcome)
- [ ] `onboard02.jpg` — 1080×1080 / 1:1(Stay Connected)
- [ ] `onboard03.jpg` — 1080×1080 / 1:1(Get Started)

※ AspectFit・高さ 240 表示のため、余白込みの正方形イラストが収まりやすい(透過が必要なら `.png`)

#### ★★ ペット(3 枚)
- [ ] `pet01.jpg` — 1000×1000 / 1:1(動物写真。画面で使うのはまず 1 枚)
- [ ] `pet02.jpg` — 1000×1000 / 1:1(バリエーション)
- [ ] `pet03.jpg` — 1000×1000 / 1:1(バリエーション)

#### ★★ プロモ/Super バナー(3 枚)
- [ ] `banner01.jpg` — 1200×600 / 2:1(サマーフェス。文字が乗る余白構図)
- [ ] `banner02.jpg` — 1200×600 / 2:1(新キャラクター)
- [ ] `banner03.jpg` — 1200×600 / 2:1(プレミアム会員)

#### ★ チャット(5 枚)
- [ ] `avatar_person01.jpg` — 256×256 / 1:1(Alice)
- [ ] `avatar_person02.jpg` — 256×256 / 1:1(Bob)
- [ ] `avatar_person03.jpg` — 256×256 / 1:1(Carol)
- [ ] `avatar_person04.jpg` — 256×256 / 1:1(Dave)
- [ ] `avatar_person05.jpg` — 256×256 / 1:1(自分)

#### ★ ログイン(1 枚)
- [ ] `login_hero.png` — 512×512 / 1:1(透過 PNG。アプリロゴ/ヒーロー)

#### ★ Raw 差し替え(2 枚)
- [ ] `Resources/Raw/Social/player.jpg` — 256×256 / 1:1(プレイヤー顔。同名上書き)
- [ ] `Resources/Raw/Avatar/mofusand.jpg` — 256×256 / 1:1(差出人アバター。同名上書き)

### 4-4. コード反映(グループ単位・★★★→★★→★ の順)

実素材を `Resources/Images/<カテゴリ>/` のプレースホルダへ同名上書きしたうえで、下表の「現在→新」を差し替える。
Raw 2 件は**同名上書きのためコード変更不要**。各グループ完了後に 4-5 の実機表示確認へ。

#### 4-4-A. ★★★ プロフィール
- [ ] 反映する

| 修正ファイル | スロット(何用) | 現在のファイル | 新ファイル |
| --- | --- | --- | --- |
| `UIProfileView.xaml` | カバー(パララックス) | `social_background.png` | `profile_cover.jpg` |
| `UIProfileView.xaml` | アバター | `profile.jpg` | `avatar_user.jpg` |
| `UIProfileViewModel.cs` | 写真ギャラリー 6 件 | `usa1〜6_full.jpg` | `gallery01〜06.jpg` |

#### 4-4-B. ★★★ ショッピング
- [ ] 反映する

| 修正ファイル | スロット(何用) | 現在のファイル | 新ファイル |
| --- | --- | --- | --- |
| `UIShopViewModel.cs` | 化粧品商品 6 件 | `usa1〜6_face.jpg` | `product_beauty01〜06.jpg` |
| `UIShopViewModel.cs` | アパレル商品 3 件 | `usa1〜3_full.jpg` | `product_apparel01〜03.jpg` |
| `UIShopView.xaml` | ショップ主アバター | `profile.jpg` | `avatar_user.jpg`(共用) |
| `UIItemView.xaml` | 商品メイン画像 | `usa1_face.jpg` | `product_beauty01.jpg`(共用) |
| `UICartViewModel.cs` | カート明細 3 件 | `usa1〜3_face.jpg` | `product_beauty01〜03.jpg`(共用) |

#### 4-4-C. ★★★ 動画配信
- [ ] 反映する

| 修正ファイル | スロット(何用) | 現在のファイル | 新ファイル |
| --- | --- | --- | --- |
| `UIStreamView.xaml` | ヒーロー | `social_background.png` | `stream_hero.jpg` |
| `UIStreamViewModel.cs` | 作品ポスター 5 件 | `social_background.png`×5 | `poster01〜05.jpg` |
| `UIStreamDetailView.xaml` | プレイヤー画像 | `social_background.png` | `stream_hero.jpg`(共用) |
| `UIStreamDetailViewModel.cs` | 予告編 3 件 | `social_background.png`×3 | `stream_clip01〜03.jpg` |
| `UIStreamDetailViewModel.cs` | 関連作品 4 件 | `social_background.png`×4 | `poster03〜06.jpg`(共用) |
| `UIStreamDetailView.xaml` | 一緒に視聴中アバター 3 件 | `usa1〜3_face.jpg` | `avatar_person01〜03.jpg` ※ |

※ ★グループの `avatar_person01〜03` に依存。動画配信を先行する場合は「その 3 枚だけ先行作成」or「このスロットは現状維持で後回し」をその時点で選ぶ。

#### 4-4-D. ★★ オンボーディング
- [ ] 反映する

| 修正ファイル | スロット(何用) | 現在のファイル | 新ファイル |
| --- | --- | --- | --- |
| `UIKitOnboardViewModel.cs` | ページ画像 ①〜③ | `social_background.png`×3 | `onboard01〜03.jpg` |

#### 4-4-E. ★★ ペット
- [ ] 反映する

| 修正ファイル | スロット(何用) | 現在のファイル | 新ファイル |
| --- | --- | --- | --- |
| `UIPetView.xaml` | ペット写真 | `usa1_full.jpg` | `pet01.jpg` |

#### 4-4-F. ★★ プロモ(Super)
- [ ] 反映する

| 修正ファイル | スロット(何用) | 現在のファイル | 新ファイル |
| --- | --- | --- | --- |
| `UISuperViewModel.cs` | バナー①サマーフェス | `social_background.png` | `banner01.jpg` |
| `UISuperViewModel.cs` | バナー②新キャラクター | `usa3_full.jpg` | `banner02.jpg` |
| `UISuperViewModel.cs` | バナー③プレミアム会員 | `profile.jpg` | `banner03.jpg` |

#### 4-4-G. ★ チャット
- [ ] 反映する

| 修正ファイル | スロット(何用) | 現在のファイル | 新ファイル |
| --- | --- | --- | --- |
| `UIChatViewModel.cs` | Alice/Bob/Carol/Dave/自分 | `usa1〜5_face.jpg`(定数 5 件) | `avatar_person01〜05.jpg` |

#### 4-4-H. ★ ログイン
- [ ] 反映する

| 修正ファイル | スロット(何用) | 現在のファイル | 新ファイル |
| --- | --- | --- | --- |
| `UILoginView.xaml` | タイトル画像 | `profile.jpg` | `login_hero.png` |

#### 4-4-I. ★ Raw 差し替え(コード変更なし)
- [ ] `Social/player.jpg` を上書き(UISocial 表示確認のみ)
- [ ] `Avatar/mofusand.jpg` を上書き(UIMail 表示確認のみ)

### 4-5. 実機表示確認(Pixel 9a・グループ完了ごと)

- [ ] ★★★ プロフィール(カバー比率/パララックス/ギャラリー 6 枚)
- [ ] ★★★ ショッピング(Shop 一覧・Item 詳細・Cart 明細でサムネ一貫)
- [ ] ★★★ 動画配信(ヒーロー 16:9 切れなし/ポスター縦長/クリップ横長)
- [ ] ★★ オンボーディング(3 ページが別画像)
- [ ] ★★ ペット/プロモ(バナー 3 種が別画像)
- [ ] ★ チャット/ログイン/Social/Mail
- [ ] ビルド 0 エラー・新規警告ゼロの維持(既知 CS8785×1 / XA4301×7 のみ)

---

## 5. バックログ(任意・後日。指示があれば着手)

- [ ]【判断】`Controls/ChatView` バブル色のバインダブル化(C-13・D18): 検討扱い・未確定
- [ ] UISocial 背景の専用化(1080×1920 / 9:16 のゲーム風背景。「任意」扱い・44 枚には含まず)
- [ ] `AnimationOption.ResetEnter` の Scale 1 固定リセット(静的 Scale+EnterAnimation 併用が将来出た場合に、TranslationY と同じ基準値退避パターンで対処)

---

## 6.【優先】`tmpl-plan-maui.md` からの移管課題(2026-09-06 組み込み)

`D:\GitHubTemplate\tmpl-plan-maui.md`(MAUI トラック強化プラン)のうち**未対応の項目を優先事項として本書へ移管**した(2026-09-06 ユーザー指示)。対応済みの項目は記載しない。**keyboard / blazor 向けの対応(同書 §4 / §5)は対象外**。
画像アセット拡充(同書 3-8)は本書 **4 節**で管理中、iOS 対応(同書 3-13)は**保留継続**のため、この節には含めない。

### template-maui への反映(同書 §1 の残り)

- [ ] **6-1** `Document/Development.md` を template-maui へ持ち込む(同書 1-4)。作業用ドキュメント 13 ファイルの除外時に**必須の Development.md まで落ちている**。DB マイグレーション未実装の唯一の緩和策(「実案件適用時の注意」章)なので必ず入れる
- [ ] **6-2** template-maui README の TODO 実態同期(同書 1-9)。実装済みの **Chat / Chart / Gauge / Calendar / Media / Cognitive(SampleCvNetFace+Azure.AI.Vision)/ HybridWebView(WebViewBind・WebViewController)** が TODO に残っている。未実装の WiFi manager / Biometric / Bottom sheet / Push / Local notification は TODO のまま残す

### 機能実装(同書 §3 継続課題)

- [ ] **6-3** WiFi manager 実装(同書 3-1。`DeviceWiFiViewModel` は 6 行の空スタブ)
- [ ] **6-4** 生体認証の完成(同書 3-2。`DeviceBiometricViewModel` も 6 行の空スタブ。画面と ViewId は登録済み)
- [ ] **6-5** Bottom sheet(同書 3-3。実装なし)
- [ ] **6-6** 【判断】Push 通知(FCM)/ Local notification(同書 3-4)
- [ ] **6-7** DB マイグレーション機構(同書 3-5。`DataService.RebuildAsync`=毎起動で物理削除→再作成の user_version ベース置換。6-1 の Development.md 章で緩和する前提のため連動)
- [ ] **6-8** ダークモード(同書 3-6。`UserAppTheme=Light` 固定・`AppThemeBinding` 0 件。Colors.xaml は 4 テンプレートでバイト一致のため**対応するなら 4 本同時が効率的**)
- [ ] **6-9** ローカライズ拡充(同書 3-7。resx は Messages / Names とも 5 件のみ。機構は動作済み)
- [ ] **6-10** `Controls/SocialControls.cs` の TODO 10 件整理(同書 3-9。2026-09-06 時点で 10 件現存を確認)
- [ ] **6-11** 【判断】TimeProvider の MAUI 方式(同書 3-10。設定は EmbeddedBuildProperty のビルド時注入方式のため、wpf / avalonia の `AddOptions<T>().ValidateOnStart()` はそのまま移植不可)
- [ ] **6-12** 【判断】Analyzers.ruleset 正典差分 11 ルールの扱い(同書 3-12。CA1416 / CA2007 は MAUI 固有の合理性あり単純追随不可。CA1014 / CA1305 / CA1824 / CA1861 は再検討余地。正典統一トラック〈aidd 側セッション〉と連動)

### ソースレビュー由来(同書 付録のうち、本リポジトリで未対応と実測確認した分)

- [ ] **6-13** `Converters/MailDateTimeStringConverter.cs` の `ConvertBack` 是正(現状 `NotSupportedException` を throw。`Binding.DoNothing` 返却か OneWay 専用の明示へ)

> 注: 同書 付録の「`ApplicationInitializer` の async void → 起動ゲート化」は **StartupState 方式で対応済み**(`Change_Summary.md` 区間 8)、「`App.xaml.cs` の権限要求遅延」は**区間 2 のコードレビュー対応で画面側へ移動済み**のため記載しない。BACK/白画面修正の template-maui 反映(同書 §2-1)も**決着済み**(A-1 反映済み・新 B-1 は反映不要=ユーザー判断。詳細は `Change_Summary.md` 区間 6 B-3)。機能追加アイデア(オフライン同期・ディープリンク・アクセシビリティ設定・起動状態画面 等)は必要になったら同書 付録を参照。

---

## 7. Smart ライブラリの活用(2026-09-06 予備調査済み)

4 件。**予備調査は完了済み**(ライブラリのソースは `D:\GitHub\Smart-Net-*`)。

**全項目に共通する前提**: 4 件中 3 件は **csproj に `PackageReference` と `TrimmerRootAssembly` が既にあり、コードからは 1 箇所も使われていない**「参照だけ入っている」状態だった。したがって作業は*パッケージ追加*ではなく **「使う」または「参照を消す」の二択**から始まる。

| ライブラリ | 参照中の版 | コードでの使用 | 対応 |
| --- | --- | --- | --- |
| `Usa.Smart.Navigation(.Maui)` | 3.9.0 | 使用中(Effect 機構も 2026-09-07 から使用) | 7-1 **完了** |
| `Usa.Smart.Results` | 2.2.0 | 使用中(2026-09-06 から。自前 `Models/Result.cs` は撤去) | 7-2 **完了** |
| `Usa.Smart.Data.Accessor` | 3.0.0-beta8 | **0 件**(`Data.Mapper` 2.16.0 を使用中) | 7-3 |
| `Usa.Smart.Mapper` | 1.0.0-beta8 | **0 件**(2023-07 追加以来ずっと未使用) | 7-4 |

**残りの推奨着手順**: 7-4(小・要判断)→ 7-3(大・前提整備が必要)。7-1 / 7-2 は完了(内容・実機確認結果・ハマりどころは `Change_Summary.md` 区間10 に記録)。

### 7-1. アニメーション付き画面遷移 — **2026-09-07 全項目完了**

7-1-1(メニュー+デモ画面)/ 7-1-2(カスタム効果 Zoom/Drop/Flip/Rotate + Dialog 片側効果)/ 7-1-3(`DialogEffectPlugin` による自動付与)/ 7-1-4(実機確認)とも完了。要注意点 3 件の結果: はみ出し=`IsClippedToBounds` をアプリ側で設定 / Slide 系の Activate・Deactivate 素通り=ライブラリ側で 4 フェーズ化済み(リリース待ち)/ 連打=例外にならない。内容は `Change_Summary.md` 区間10「遷移効果(Effect)デモの追加」を参照。

- [ ] **7-1-5** `Usa.Smart.Navigation` / `.Maui` を Slide 4 フェーズ化版(3.9.0 より後の版)へ更新したら、Push (Stack) で遷移元が下へスライドアウトし、Pop で遷移先が上からスライドインすることを実機で再確認する(アプリ側の変更は不要)

### 7-2. `Usa.Smart.Results` の活用 — **2026-09-07 全項目完了**

7-2-1(自前 `Models/Result.cs` 撤去 + `NetworkError`)/ 7-2-2(`ExpressionCalculator` → `Result<double>`)/ 7-2-3(`CropDrawing.ExportCrop` → `Result<(int,int)>`)/ 7-2-5(対象外の確定)とも完了。7-2-4 の `ScpTransferResult` は**対応不要**。詳細は `Change_Summary.md` 区間10「`Usa.Smart.Results` の採用」を参照。

- [ ] **7-2-6** Network > Get server time の**成功経路**のみ API サーバが必要なため未確認(失敗経路は確認済み)。サーバが用意できた時点で確認する

### 7-3. `Usa.Smart.Data.Mapper` → `Usa.Smart.Data.Accessor` への移行

**調査結果**: v3 は **`[DataAccessor]` 付き partial class + partial メソッドをソースジェネレータが実装展開**する方式(v2 の interface + リフレクション方式は廃止)。`IsAotCompatible=true` で、**実 SQLite に対する AOT テスト**(`Smart.Data.Accessor.AotTests`)もあり MAUI/トリミングとの相性は良好。むしろ現行の `SqlHelper`(`NullabilityInfoContext` によるリフレクション DDL 生成)が消えるぶん改善。SQLite は標準 Builder 属性(コアパッケージ同梱)で利用でき、MySql/Postgres/SqlServer パッケージは方言拡張専用。

現行規模: **エンティティ 3 型・SQL 17 本・動的 SQL ゼロ(全て静的 CRUD)・呼び出し側 18 箇所**。シグネチャ互換で置換できるため移行自体は機械的。

**着手前に潰すべき前提が 2 つある**:

- [ ] **7-3-0**【要対応・ブロッカー】`Usa.Smart.Data.Accessor.Extensions.DependencyInjection` の **3.0.0-beta8 がローカルフィードに存在しない**(あるのは beta7 と、古い `3.0.0`=Smart.Data 2.10.0 依存)。SemVer 上 `3.0.0 > 3.0.0-beta8` のため**素直に入れると古い版を掴む罠**。beta8 を pack するかバージョン固定するかを決める
- [ ] **7-3-1** 土台のみ投入して **MAUI/Android でソースジェネレータと `.sql` の `AdditionalFiles` 取り込みが動くことを先に確認**(空の `[DataAccessor]` クラス 1 個でビルド)。**移行の最大の不確実性はここ**
- [ ] **7-3-2** `DateTimeTypeHandler` を `IValueConverter<long, DateTime>` へ移植し、クラススコープ `[TypeHandler]` で適用(グローバル設定 `SqlMapperConfig.Default.ConfigureTypeHandlers` は廃止)。※`GuidTypeHandler` は**どのエンティティにも Guid が無く実質デッドコード**のため移植不要
- [ ] **7-3-3** 読み取り系 4 メソッドを移行(`DataService` はラッパとして残し呼び出し側は無変更)
- [ ] **7-3-4** 更新系(非トランザクション)を移行。`InsertDataAsync` の `SQLITE_CONSTRAINT` 判定はラッパ側に残す
- [ ] **7-3-5** トランザクション系 2 メソッド。**`IDbProvider` パターンは複数メソッドを 1 トランザクションで括れない**ため、`DbTransaction` を引数で渡す形にし `UsingTx` は `DataService` に残す
- [ ] **7-3-6** DDL(`SqlHelper.MakeCreate<T>()`)を `.sql` ファイル化(PRAGMA 3 本 + CREATE TABLE 3 本)
- [ ] **7-3-7** `Usa.Smart.Data.Mapper` / `.Builders` の参照・`TrimmerRootAssembly`・`SqlHelper.cs`・TypeHandler 2 種・エンティティの `[PrimaryKey]` を撤去
- [ ] **7-3-8** 実機確認(Data 画面 / Edit 画面 / 初回起動の DB 再構築 / Bulk 10,000 件)

> **既知の落とし穴**: ①`Data.Mapper.Builders` はテーブル名から `Entity` サフィックスを自動除去する(`DataEntity`→`Data`)が、**Accessor にこの機能は無い**。Builder 属性に `Table = "Data"` を明示しないと実行時まで気付けない ②TypeHandler の適用範囲がグローバル→アクセサクラス単位に変わるため、アクセサを分割するなら `[AccessorProfile]` + `[ExecuteConfig]` で共有する ③エンティティ型は XAML のコンパイル済みバインドやナビゲーションパラメータを跨ぐため、**改名・再形状化はスコープ外**(属性の差し替えのみに留める)

### 7-4. `Usa.Smart.Mapper` の活用

**調査結果**: 想定していた実行時 API(`SmartMapper` / `MapperConfig` / `IMapper`)は**削除済み**で、現在は **`[Mapper]` を付けた `static partial` メソッドをソースジェネレータが実装展開する方式**。DI 登録は不要(静的メソッドのため `MauiProgram` の変更もゼロ)。リフレクション・式木を使わず、AOT 非安全な経路に落ちると **SMP0402 でコンパイルエラー**になるため実行時に黙って壊れる余地がない。**このアプリの `[ObservableProperty] public partial` プロパティに対して正常動作することは実ビルドで検証済み**。

**ただし適用候補が乏しい**: このアプリは**エンティティを直接 UI にバインドする設計**で Entity→ViewModel の変換層が無く、API レスポンスも変換せず直接消費している。全 526 ファイルを走査した結果、マッピング形状の箇所は 10 件程度で、**明確な置き換え候補は 1 件のみ**だった。

- [ ] **7-4-0**【判断】**使うか、参照を消すか**を決める。現在 1.0.0-beta8(正式版前)を 2023-07 から未使用のまま参照しており、テンプレートとして良くない状態。採用しないなら csproj の `PackageReference` と `TrimmerRootAssembly` の 2 行を削除する
- [ ] **7-4-1** 置き換え(採用する場合): `Models/Sample/SwitchBotTemperature.cs` の `CopyTo` 拡張メソッド(**6/6 プロパティが変換ロジックなしの単純コピー**)を `[Mapper]` 化。呼び出し元は `DeviceBleScanViewModel.cs` 1 箇所
- [ ] **7-4-2**【判断】使用例の追加(採用する場合の本命): `Models/Api/DataListResponseEntry`(Id int, Name string)と `Models/Entity/WorkEntity`(Id long, Name string)が**ほぼ同形なのに変換コードが無い**。ここに「API レスポンス → DB エンティティ」の変換を足すと、型変換(int↔long)・`[MapProperty]` の名前解決・`[MapConstant]`/`[MapExpression]` による `CreateAt` 補完まで 1 画面で見せられる。置き場所は Data 画面(既に CRUD ボタンが並ぶ)が最適
- [ ] **7-4-3** 対象外の確認(記録): `MonthViewBuilder` / `DeviceInfoViewModel`(ソースが 3 つ)/ `GraphBuilder` / `ScheduleService` / NFC・BLE のバイト列パース等は、**置き換えるとコード量が増え可読性が落ちる**ため見送り。`Select` 射影 28 件にオブジェクト間マッピングは 1 件も無い

> **紛らわしい点**: `Usa.Smart.Mapper`(オブジェクトマッパー。ソース= `D:\GitHub\Smart-Net-Mapper`)と `Usa.Smart.Data.Mapper`(SQL マイクロ ORM。ソース= `D:\GitHub\Smart-Net-Data-Mapper`)は**別物**。7-3 で移行するのは後者。
