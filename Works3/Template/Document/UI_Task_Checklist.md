# UI 残作業チェックリスト(作業推進用)

MAUI 画面まわり(統合・ブラッシュアップ・画像アセット)の**残作業を進めるためのマスターチェックリスト**(2026-08-01 作成)。
以降の作業はこの番号で指示・進行する(例:「1-2 は (b) で」「1-4-A を実施」)。

## 運用ルール

- 完了した項目は `[x]` にし、行末に完了日を追記する。問題があれば行末にメモ。
- **【判断】印の項目はユーザーが決定**する(勝手に進めない)。デザイン判断を伴う差分は 1 項目ずつ指示を受けて実施。
- コミットはユーザーが実施(グループ単位のコミットを推奨)。
- 本書が完了した際の扱い(削除 or `UI_Development_Log.md` へ要点統合)はユーザー判断。

## 関連ドキュメント

| ドキュメント | 役割 |
| --- | --- |
| `UI_Development_Log.md` | 経緯・決定事項・完了記録(歴史) |
| `UI_Verification_Checklist.md` | 実機確認の観点一覧(画面別・133 項目) |
| `Image_Asset_Expansion_Plan.md` | 画像拡充の詳細(画面別対応表・サイズ・命名・修正コード) |
| 本書 | 残作業の進行管理(何をどの順でやるか) |

---

## 1. 画像アセット拡充

### 1-1.【判断】候補ファイル名の確定

- [ ] ファイル名を確定する(現候補: `avatar_user` / `profile_cover` / `gallery` / `product_apparel` / `product_beauty` / `poster` / `stream_hero` / `stream_clip` / `onboard` / `pet` / `banner` / `avatar_person` / `login_hero`)
  - 変更例: `poster`→`movie`、`gallery`→`photo` など。確定後、計画書 §3/§4/§6 と本書 1-3/1-4 の名前を一括更新する。
  - 制約: 小文字・先頭英字・英数字と `_` のみ(MauiImage)。

### 1-2.【判断】画像の作成手段

- [ ] 作成手段を決定する:
  - (a) 素材支給を待つ(従来方針。作成はスコープ外)
  - (b) Claude がプレースホルダをプログラム生成(正しいアスペクト比・スロット別に区別できる内容。後日、本素材へ同名差し替え可能)
  - (a)(b) 併用(例: ★★★ は支給・★★ 以下は生成)も可。

### 1-3. 画像の用意(新規 42 枚+差し替え 2 枚)

作成できたファイルにチェック。サイズは「最大表示 dp×3」基準・**アスペクト比がスロットと一致していることが最重要**。
写真=`.jpg`(品質 80)、透過=`.png`。詳細な内容イメージは計画書 §4。

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

### 1-4. コード反映(グループ単位・★★★→★★→★ の順)

画像を `Resources/Images/` に置いたうえで、下表の「現在→新」を差し替える。
Raw 2 件は**同名上書きのためコード変更不要**。各グループ完了後に 1-5 の実機確認へ。

#### 1-4-A. ★★★ プロフィール
- [ ] 反映する

| 修正ファイル | スロット(何用) | 現在のファイル | 新ファイル |
| --- | --- | --- | --- |
| `UIProfileView.xaml` | カバー(パララックス) | `social_background.png` | `profile_cover.jpg` |
| `UIProfileView.xaml` | アバター | `profile.jpg` | `avatar_user.jpg` |
| `UIProfileViewModel.cs` | 写真ギャラリー 6 件 | `usa1〜6_full.jpg` | `gallery01〜06.jpg` |

#### 1-4-B. ★★★ ショッピング
- [ ] 反映する

| 修正ファイル | スロット(何用) | 現在のファイル | 新ファイル |
| --- | --- | --- | --- |
| `UIShopViewModel.cs` | 化粧品商品 6 件 | `usa1〜6_face.jpg` | `product_beauty01〜06.jpg` |
| `UIShopViewModel.cs` | アパレル商品 3 件 | `usa1〜3_full.jpg` | `product_apparel01〜03.jpg` |
| `UIShopView.xaml` | ショップ主アバター | `profile.jpg` | `avatar_user.jpg`(共用) |
| `UIItemView.xaml` | 商品メイン画像 | `usa1_face.jpg` | `product_beauty01.jpg`(共用) |
| `UICartViewModel.cs` | カート明細 3 件 | `usa1〜3_face.jpg` | `product_beauty01〜03.jpg`(共用) |

#### 1-4-C. ★★★ 動画配信
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

#### 1-4-D. ★★ オンボーディング
- [ ] 反映する

| 修正ファイル | スロット(何用) | 現在のファイル | 新ファイル |
| --- | --- | --- | --- |
| `UIKitOnboardViewModel.cs` | ページ画像 ①〜③ | `social_background.png`×3 | `onboard01〜03.jpg` |

#### 1-4-E. ★★ ペット
- [ ] 反映する

| 修正ファイル | スロット(何用) | 現在のファイル | 新ファイル |
| --- | --- | --- | --- |
| `UIPetView.xaml` | ペット写真 | `usa1_full.jpg` | `pet01.jpg` |

#### 1-4-F. ★★ プロモ(Super)
- [ ] 反映する

| 修正ファイル | スロット(何用) | 現在のファイル | 新ファイル |
| --- | --- | --- | --- |
| `UISuperViewModel.cs` | バナー①サマーフェス | `social_background.png` | `banner01.jpg` |
| `UISuperViewModel.cs` | バナー②新キャラクター | `usa3_full.jpg` | `banner02.jpg` |
| `UISuperViewModel.cs` | バナー③プレミアム会員 | `profile.jpg` | `banner03.jpg` |

#### 1-4-G. ★ チャット
- [ ] 反映する

| 修正ファイル | スロット(何用) | 現在のファイル | 新ファイル |
| --- | --- | --- | --- |
| `UIChatViewModel.cs` | Alice/Bob/Carol/Dave/自分 | `usa1〜5_face.jpg`(定数 5 件) | `avatar_person01〜05.jpg` |

#### 1-4-H. ★ ログイン
- [ ] 反映する

| 修正ファイル | スロット(何用) | 現在のファイル | 新ファイル |
| --- | --- | --- | --- |
| `UILoginView.xaml` | タイトル画像 | `profile.jpg` | `login_hero.png` |

#### 1-4-I. ★ Raw 差し替え(コード変更なし)
- [ ] `Social/player.jpg` を上書き(UISocial 表示確認のみ)
- [ ] `Avatar/mofusand.jpg` を上書き(UIMail 表示確認のみ)

### 1-5. 実機表示確認(Pixel 9a・グループ完了ごと)

- [ ] ★★★ プロフィール(カバー比率/パララックス/ギャラリー 6 枚)
- [ ] ★★★ ショッピング(Shop 一覧・Item 詳細・Cart 明細でサムネ一貫)
- [ ] ★★★ 動画配信(ヒーロー 16:9 切れなし/ポスター縦長/クリップ横長)
- [ ] ★★ オンボーディング(3 ページが別画像)
- [ ] ★★ ペット/プロモ(バナー 3 種が別画像)
- [ ] ★ チャット/ログイン/Social/Mail
- [ ] ビルド 0 エラー・新規警告ゼロの維持(既知 CS8785×1 / XA4301×7 のみ)

---

## 2. コミット(ユーザー実施)

- [ ] 復元済み `Image_Asset_Expansion_Plan.md` のコミット(**現在未追跡。再消失防止のため早めに**)
- [ ] 本書(`UI_Task_Checklist.md`)のコミット
- [ ] 以降、画像追加・コード反映のグループ単位コミット

---

## 3. 実機確認チェックリストの消化(0 / 133 項目)

`UI_Verification_Checklist.md` は全項目未チェック。

- [ ]【判断】消化の進め方を決める: (a) ユーザーが実施 / (b) 表示・遷移系は Claude が実機スクショで代行し、操作感・アニメ質感のみユーザー確認 / (c) 画像反映(1-4/1-5)と同時に該当画面分を消化
- [ ] 決めた方式で消化を進める(進捗は同ファイル内のチェックで管理)

---

## 4.【判断】ドキュメント構成

- [ ] `Image_Asset_Expansion_Plan.md`(+本書)を独立維持するか、既存 2 本(`UI_Development_Log.md` / `UI_Verification_Checklist.md`)へ統合するかを決める
  - 参考案: 画像作業が完了するまでは独立維持 → 完了後に要点を Log へ統合し、計画書と本書は削除(過去のチェックリスト運用と同様)。

---

## 5. クローズ済み(対応不要・触らない)

再掲(確認用)。以下は完了 or「対応不要」で確定済みのため、本書の管理対象外。

- 画面統合・整理: Profile 統合 / Graph2 改名 / Cockpit 廃止 / HUD 4 画面独立維持 / 改名 3 件(Timeline/Flight/Tactical)/ メニュー 10 行 30 ボタン・空セル 0 / Radar↔HUD 重複整理(スコープ外)/ 共通部品化・近縁ペア(対応不要)
- ブラッシュアップ第 1 弾・第 2 弾: 全フェーズ+FontSize 統一+スタイル切り出し 36 画面(コミット済み)
- 仕様・許容: 未結線ボタン=仕様(UIShop 検索のみ実機能)/ 既知警告(CS8785×1, XA4301×7)/ 環境制約 3 件(Maps API キー・CvNet エンドポイント・CameraView まれにハング)

---

## 6. 任意・潜在(バックログ。指示があれば着手)

- [ ] UISocial 背景の専用化(1080×1920 / 9:16。計画書 §3 で「任意」扱い・44 枚には含まず)
- [ ] `AnimationOption.ResetEnter` の Scale 1 固定リセット(静的 Scale+EnterAnimation 併用が将来出た場合に、TranslationY と同じ基準値退避パターンで対処)
