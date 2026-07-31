# 画像アセット拡充計画

Template.MobileApp のサンプル各画面は、少数の画像を複数画面・複数用途で使い回している。
画面ごとに用途に合った画像を用意するため、追加を推奨する画像ファイルの一覧・推奨サイズ・
内容（どのような絵が欲しいか）をまとめる。

対象は `Template/Template.MobileApp/Resources/` 配下。

---

## 1. 現状の問題（調査結果）

### 1-1. 同一プレースホルダの三重使い回し（要修正）

以下の 3 ファイルは **MD5 が完全一致する同一バイナリ**（204×192px / 7,987 bytes）で、
名前だけ変えて別用途に使われている。

| ファイル | 用途 | 使用画面 |
| --- | --- | --- |
| `Images/profile.jpg` | 自分のアバター / タイトル / バナー | UILogin, UIProfile, UIShop, UISuper |
| `Raw/Avatar/mofusand.jpg` | メール差出人（猫キャラ） | UIMail |
| `Raw/Social/player.jpg` | ソーシャルHUDのプレイヤー | UISocial |

MD5: `DD53A69221D1680BA54228345B880E8F`（3ファイル共通）。
→ 実機では「プロフィール」「メール差出人」「ゲームのプレイヤー」がすべて同じ絵で表示される。
　解像度も 204×192 と低く、アバター表示（最大 110dp＝実効 330px 相当）には不足。

### 1-2. `social_background.png` の過剰流用

1 枚（1024×1536 の**縦長**）を、用途もアスペクト比も異なる 15 箇所以上へ流用している。

| 使用画面 | スロットの形状 | 縦長画像との相性 |
| --- | --- | --- |
| UIKitOnboard（3ページ） | AspectFit / 高さ240 | 3ページ全部が同じ絵 |
| UIStream ヒーロー | 横長・高さ380 AspectFill | 縦長を横長に切り抜き＝中央だけ表示 |
| UIStream ポスター（5件） | 縦長 110×160 | 5件全部が同じ絵 |
| UIStreamDetail プレイヤー/関連(7件) | 横長・縦長 混在 | 全件同じ絵 |
| UIProfile カバー | 横長・高さ140 AspectFill | 縦長を横長に切り抜き |
| UISocial 背景 | 全面 | — |
| UISuper バナー | 横長カルーセル | 縦長を横長に切り抜き |

### 1-3. `usa1〜8`（キャラアート）の用途転用

`usa{n}_full.jpg`（1024×1024）と `usa{n}_face.jpg`（256×256）は本来キャラクター立ち絵で、
UICharacter では用途に合っている。しかし他画面では以下へ転用されている。

| 転用先 | 実際の意味 | 使用画面 |
| --- | --- | --- |
| `usa{n}_full` | ファッション商品（ドレス/ジャケット/帽子） | UIShop |
| `usa{n}_full` | ペットの写真 | UIPet |
| `usa{n}_full` | プロフィール写真ギャラリー | UIProfile, UIProfile2 |
| `usa{n}_face` | 化粧品サムネイル（美容液/口紅） | UIShop, UICart, UIItem |
| `usa{n}_face` | チャット/友だちアバター | UIChat, UIStreamDetail |

→ 「ドレス」という商品名でウサギの立ち絵が出る等、サンプルの説得力が落ちている。

---

## 2. 現在の画像インベントリ

### MauiImage（`Resources/Images/` … XAMLで `Source="xxx.jpg"` 直接参照）
csproj: `<MauiImage Include="Resources\Images\*" />`

| ファイル | 実寸 | 用途 | 判定 |
| --- | --- | --- | --- |
| `stamp01〜08.png`（8枚） | 370×320 | チャットスタンプ | 専用でOK |
| `profile.jpg` | 204×192 | 自分アバター/カバー | **要差し替え**（1-1） |
| `social_background.png` | 1024×1536 | 背景/カバー/ポスター等 | **要分割**（1-2） |
| `usa1〜8_full.jpg`（8枚） | 1024×1024 | キャラ立ち絵→商品等に転用 | UICharacter以外は要専用画像 |
| `usa1〜8_face.jpg`（8枚） | 256×256 | 顔→商品/アバターに転用 | 同上 |
| `account.svg` / `ic_camera.svg` / `ic_sticker.svg` / `ic_send.svg` | ベクタ | UIアイコン | 専用でOK |

### MauiAsset（`Resources/Raw/<カテゴリ>/` … `OpenAppPackageFileAsync` で手動ロード）
csproj: `<MauiAsset Include="Resources\Raw\**" LogicalName="%(RecursiveDir)%(Filename)%(Extension)" />`

| ファイル | 実寸 | 用途 | 判定 |
| --- | --- | --- | --- |
| `Avatar/mofusand.jpg` | 204×192 | メール差出人 | **要差し替え**（1-1・専用） |
| `Avatar/usausa.png` | 160×160 | メール差出人 | 専用でOK（低解像） |
| `Avatar/genbaneko.png` | 280×280 | メール差出人 | 専用でOK |
| `Social/player.jpg` | 204×192 | HUDプレイヤー | **要差し替え**（1-1） |
| `Social/gem.png` `moneybag.png` `wrench.png` | 128×128 | HUDアイコン | 専用でOK |
| `Stamp/Good.png` `Space.png` `Apology.png` | 256×256 | サンプルチャットスタンプ | 専用でOK |
| `DeckButtons/*.png`（20枚超） | 192×192 | ストリームデッキ用アイコン | 専用でOK |

---

## 3. 画面単位 対応表（現状 → 推奨）

各画面の画像スロットごとに「現在のファイル」「何用か」「推奨する追加/差し替えファイル」を示す。
`（共用）`は他画面と同じ新規ファイルを使い回す想定、`（専用）`はその画面専用。
`—` は追加不要（現状で用途に合致）。

### UILogin
| 画面内スロット | 現在のファイル | 何用（現状） | 推奨する追加/差し替え | 推奨サイズ・比率 | 内容 |
| --- | --- | --- | --- | --- | --- |
| タイトル画像 | `profile.jpg` | ログイン画面のロゴ枠 | `login_hero.png`（新規・任意） | 512×512 / 1:1・透過PNG | アプリロゴ or ヒーローイラスト |

### UIProfile

※ 2026-07-07 の Profile 統合（旧 UIProfile2 ベースへ集約・UIProfile2 は削除済み）を反映。

| 画面内スロット | 現在のファイル | 何用（現状） | 推奨する追加/差し替え | 推奨サイズ・比率 | 内容 |
| --- | --- | --- | --- | --- | --- |
| カバー（パララックス・高さ140） | `social_background.png` | 上部の横長カバー | `profile_cover.jpg`（新規） | 1600×800 / 2:1 | 横長の風景・抽象・グラデ |
| アバター | `profile.jpg` | 自分の顔アイコン | `avatar_user.jpg`（新規） | 512×512 / 1:1 | 人物ポートレート/顔アイコン |
| 写真ギャラリー | `usa1〜6_full.jpg` | 投稿写真グリッド | `gallery01〜06.jpg`（新規6） | 1000×1000 / 1:1 | 旅行/料理/風景/日常の写真 |

### UIChat
| 画面内スロット | 現在のファイル | 何用（現状） | 推奨する追加/差し替え | 推奨サイズ・比率 | 内容 |
| --- | --- | --- | --- | --- | --- |
| 相手/自分アバター | `usa1〜5_face.jpg` | Alice/Bob/Carol/Dave/自分 | `avatar_person01〜05.jpg`（新規5・任意） | 256×256 / 1:1 | 人物アバター5種 |
| スタンプ | `stamp01〜08.png` | チャットスタンプ | — | — | 現状維持（専用） |

### UIShop
| 画面内スロット | 現在のファイル | 何用（現状） | 推奨する追加/差し替え | 推奨サイズ・比率 | 内容 |
| --- | --- | --- | --- | --- | --- |
| ショップ主アバター | `profile.jpg` | 店舗アカウントの顔 | `avatar_user.jpg`（共用） | 512×512 / 1:1 | 人物/店舗ロゴ |
| アパレル商品（3件） | `usa1〜3_full.jpg` | Velvet Dress 等 | `product_apparel01〜03.jpg`（新規3） | 900×1200 / 3:4 | 服のスタジオ物撮り（縦） |
| 化粧品商品（6件） | `usa1〜6_face.jpg` | Aqua Serum 等 | `product_beauty01〜06.jpg`（新規6） | 800×800 / 1:1 | 化粧品・小物の物撮り |

### UIItem
| 画面内スロット | 現在のファイル | 何用（現状） | 推奨する追加/差し替え | 推奨サイズ・比率 | 内容 |
| --- | --- | --- | --- | --- | --- |
| 商品メイン画像 | `usa1_face.jpg` | 商品詳細のメイン | `product_beauty01.jpg`（共用） | 800×800 / 1:1 | 化粧品の物撮り |

### UICart
| 画面内スロット | 現在のファイル | 何用（現状） | 推奨する追加/差し替え | 推奨サイズ・比率 | 内容 |
| --- | --- | --- | --- | --- | --- |
| カート内商品サムネ（3件） | `usa1〜3_face.jpg` | Aqua Serum 等 | `product_beauty01〜03.jpg`（共用） | 800×800 / 1:1 | 化粧品の物撮り |

### UICharacter
| 画面内スロット | 現在のファイル | 何用（現状） | 推奨する追加/差し替え | 推奨サイズ・比率 | 内容 |
| --- | --- | --- | --- | --- | --- |
| キャラ顔・立ち絵（8体） | `usa1〜8_face.jpg` / `usa1〜8_full.jpg` | キャラ一覧の顔・全身 | — | — | 現状維持（用途に合致） |

### UIPet
| 画面内スロット | 現在のファイル | 何用（現状） | 推奨する追加/差し替え | 推奨サイズ・比率 | 内容 |
| --- | --- | --- | --- | --- | --- |
| ペット写真 | `usa1_full.jpg` | ペットの写真 | `pet01〜03.jpg`（新規3） | 1000×1000 / 1:1 | 犬/猫など動物の写真 |

### UIKitOnboard
| 画面内スロット | 現在のファイル | 何用（現状） | 推奨する追加/差し替え | 推奨サイズ・比率 | 内容 |
| --- | --- | --- | --- | --- | --- |
| ページ画像①〜③ | `social_background.png`（×3） | オンボード各ページ図版 | `onboard01〜03.jpg`（新規3） | 1080×1080 / 1:1 | Welcome/Connected/Get Started の各図 |

### UIStream
| 画面内スロット | 現在のファイル | 何用（現状） | 推奨する追加/差し替え | 推奨サイズ・比率 | 内容 |
| --- | --- | --- | --- | --- | --- |
| ヒーロー | `social_background.png` | トップのキービジュアル | `stream_hero.jpg`（新規） | 1600×900 / 16:9 | 横長キービジュアル |
| 作品ポスター（5件） | `social_background.png`（×5） | 作品ポスター行 | `poster01〜05.jpg`（新規） | 600×900 / 2:3 | 縦長ポスター5種 |

### UIStreamDetail
| 画面内スロット | 現在のファイル | 何用（現状） | 推奨する追加/差し替え | 推奨サイズ・比率 | 内容 |
| --- | --- | --- | --- | --- | --- |
| プレイヤー画像 | `social_background.png` | 再生画面サムネ | `stream_hero.jpg`（共用） | 1600×900 / 16:9 | 横長キービジュアル |
| 一緒に視聴中アバター（3件） | `usa1〜3_face.jpg` | 友だちの顔 | `avatar_person01〜03.jpg`（共用） | 256×256 / 1:1 | 人物アバター |
| 予告編/クリップ（3件） | `social_background.png`（×3） | 予告編サムネ | `stream_clip01〜03.jpg`（新規3） | 1280×720 / 16:9 | 横長サムネ |
| 関連作品（4件） | `social_background.png`（×4） | 関連作品ポスター | `poster01〜06.jpg`（共用） | 600×900 / 2:3 | 縦長ポスター |

### UISuper
| 画面内スロット | 現在のファイル | 何用（現状） | 推奨する追加/差し替え | 推奨サイズ・比率 | 内容 |
| --- | --- | --- | --- | --- | --- |
| バナー①「サマーフェス」 | `social_background.png` | プロモ告知 | `banner01.jpg`（新規） | 1200×600 / 2:1 | キャンペーンバナー |
| バナー②「新キャラクター」 | `usa3_full.jpg` | プロモ告知 | `banner02.jpg`（新規） | 1200×600 / 2:1 | キャンペーンバナー |
| バナー③「プレミアム会員」 | `profile.jpg` | プロモ告知 | `banner03.jpg`（新規） | 1200×600 / 2:1 | キャンペーンバナー |

### UISocial（SkiaSharp HUD）
| 画面内スロット | 現在のファイル | 何用（現状） | 推奨する追加/差し替え | 推奨サイズ・比率 | 内容 |
| --- | --- | --- | --- | --- | --- |
| 背景 | `social_background.png` | HUD背景 | 専用背景（任意） | 1080×1920 / 9:16 | ゲーム風背景 |
| プレイヤー | `Raw/Social/player.jpg` | プレイヤー顔（＝プレースホルダ） | `Raw/Social/player.jpg` 差し替え | 256×256 / 1:1 | キャラ/プレイヤー顔 |
| 通貨・資源アイコン | `Raw/Social/gem.png` `moneybag.png` `wrench.png` | HUDアイコン | — | — | 現状維持（専用） |

### UIMail
| 画面内スロット | 現在のファイル | 何用（現状） | 推奨する追加/差し替え | 推奨サイズ・比率 | 内容 |
| --- | --- | --- | --- | --- | --- |
| 差出人アバター | `Raw/Avatar/mofusand.jpg`（＝プレースホルダ） / `genbaneko.png` / `usausa.png` | メール差出人の顔 | `Raw/Avatar/mofusand.jpg` 差し替え | 256×256 / 1:1 | 差出人アバター |

### UIDock
| 画面内スロット | 現在のファイル | 何用（現状） | 推奨する追加/差し替え | 推奨サイズ・比率 | 内容 |
| --- | --- | --- | --- | --- | --- |
| デッキボタンアイコン | `Raw/DeckButtons/*.png` | ボタン絵柄 | — | — | 現状維持（専用） |

---

## 4. 追加推奨画像 定義一覧（新規作成するファイル）

重複を排除した「作成すべきファイル」の一覧。推奨ピクセルサイズは
「レイアウト上の表示dp × 3倍（@3x相当）」を目安に算出。
MauiImage は元画像を最大密度として各解像度を自動生成するため、下記サイズを 1 枚用意すればよい。
写真は `.jpg`（品質80前後）、透過・図版は `.png` を推奨。

### 4-A. プロフィール（UIProfile / UIProfile2）★最優先

| 追加ファイル | 種別/配置 | 推奨サイズ | 比率 | 内容 |
| --- | --- | --- | --- | --- |
| `avatar_user.jpg` | MauiImage | 512×512 | 1:1 | 「自分」のアバター。人物ポートレート or スタイライズド顔アイコン。`profile.jpg` の高解像・専用置き換え |
| `profile_cover.jpg` | MauiImage | 1600×800 | 2:1 | プロフィール上部のカバー写真（風景・グラデ・抽象など横長）。`social_background.png` の代替 |
| `gallery01〜06.jpg`（6枚） | MauiImage | 1000×1000 | 1:1 | 個人の写真グリッド用（旅行・料理・風景・日常）。キャラ立ち絵からの置き換え |

### 4-B. ショッピング（UIShop / UIItem / UICart）★最優先

| 追加ファイル | 種別/配置 | 推奨サイズ | 比率 | 内容 |
| --- | --- | --- | --- | --- |
| `product_apparel01〜03.jpg`（3枚） | MauiImage | 900×1200 | 3:4 | アパレル商品（ドレス/ジャケット/帽子）。無地スタジオ背景の縦位置 |
| `product_beauty01〜06.jpg`（6枚） | MauiImage | 800×800 | 1:1 | 化粧品・小物（美容液/口紅/クリーム等）。正方形スタジオ背景。UICart/UIItem サムネと共用 |

### 4-C. 動画配信（UIStream / UIStreamDetail）★最優先

ポスターは縦長カード（110×160dp）、ヒーローは横長（高さ380dp）と形状が異なる点に注意。

| 追加ファイル | 種別/配置 | 推奨サイズ | 比率 | 内容 |
| --- | --- | --- | --- | --- |
| `poster01〜06.jpg`（6枚） | MauiImage | 600×900 | 2:3 | 映画/番組のポスター風。作品ごとに異なるビジュアル |
| `stream_hero.jpg` | MauiImage | 1600×900 | 16:9 | ヒーロー/詳細トップの横長キービジュアル |
| `stream_clip01〜03.jpg`（3枚） | MauiImage | 1280×720 | 16:9 | 予告編・関連動画のサムネイル（横長） |

### 4-D. オンボーディング（UIKitOnboard）

| 追加ファイル | 種別/配置 | 推奨サイズ | 比率 | 内容 |
| --- | --- | --- | --- | --- |
| `onboard01.jpg` | MauiImage | 1080×1080 | 1:1 | 「Welcome」：アプリ紹介のイラスト/ヒーロー |
| `onboard02.jpg` | MauiImage | 1080×1080 | 1:1 | 「Stay Connected」：同期・つながりを表す図 |
| `onboard03.jpg` | MauiImage | 1080×1080 | 1:1 | 「Get Started」：開始を促す図 |

※ AspectFit・高さ240 表示のため、余白込みの正方形イラストが収まりやすい。透過が必要なら `.png`。

### 4-E. ペット（UIPet）

| 追加ファイル | 種別/配置 | 推奨サイズ | 比率 | 内容 |
| --- | --- | --- | --- | --- |
| `pet01〜03.jpg`（3枚） | MauiImage | 1000×1000 | 1:1 | 実際の動物写真（犬/猫など）。`usa1_full` の置き換え |

### 4-F. プロモ/スーパーアプリ（UISuper）

| 追加ファイル | 種別/配置 | 推奨サイズ | 比率 | 内容 |
| --- | --- | --- | --- | --- |
| `banner01〜03.jpg`（3枚） | MauiImage | 1200×600 | 2:1 | キャンペーンバナー横長。文字が乗る前提で余白のある構図 |

### 4-G. チャット（UIChat）※任意・優先度低

現状 `usa{n}_face` で成立しているが、人物アバターに寄せるなら専用化。

| 追加ファイル | 種別/配置 | 推奨サイズ | 比率 | 内容 |
| --- | --- | --- | --- | --- |
| `avatar_person01〜05.jpg`（5枚） | MauiImage | 256×256 | 1:1 | Alice/Bob/Carol/Dave/自分の人物アバター |

### 4-H. ログイン（UILogin）※任意

| 追加ファイル | 種別/配置 | 推奨サイズ | 比率 | 内容 |
| --- | --- | --- | --- | --- |
| `login_hero.png` | MauiImage | 512×512 | 1:1 | アプリロゴ/ヒーローイラスト（透過PNG）。`profile.jpg` の代替 |

### 4-I. ソーシャルHUD（UISocial・SkiaSharp）

| 追加ファイル | 種別/配置 | 推奨サイズ | 比率 | 内容 |
| --- | --- | --- | --- | --- |
| `Raw/Social/player.jpg` 差し替え | MauiAsset | 256×256 | 1:1 | ゲームのプレイヤー/キャラ顔。現状はプレースホルダ流用 |

### 4-J. メール差出人（UIMail）

| 追加ファイル | 種別/配置 | 推奨サイズ | 比率 | 内容 |
| --- | --- | --- | --- | --- |
| `Raw/Avatar/mofusand.jpg` 差し替え | MauiAsset | 256×256 | 1:1 | 差出人アバター（現状はプレースホルダ流用・要専用画像） |

---

## 5. 命名・配置・フォーマットの指針

### 配置先の選び方
- **`<Image Source="...">` でXAMLから直接使う** → `Resources/Images/`（MauiImage）。
  ファイル名で参照する。密度別画像はビルド時に自動生成される。
- **SkiaSharp やコードで `OpenAppPackageFileAsync` から読む** → `Resources/Raw/<カテゴリ>/`（MauiAsset）。
  `LogicalName` はカテゴリ配下の相対パス（例 `Social/player.jpg`）。原本がそのまま同梱される。

### 命名規則（MauiImage は厳格）
- 小文字のみ、先頭は英字、使用可は `英数字` と `_`（`-` や大文字・空白・日本語は不可）。
- 連番は `poster01`, `poster02` のようにゼロ埋めで揃える。

### フォーマット
- 写真 → `.jpg`（品質80前後で十分軽量）。
- 透過・図版・ロゴ → `.png`。ベクタで済むUIアイコンは `.svg`（既存 `account.svg` 等に倣う）。

### サイズ方針
- 表示dp × 3 を上限目安に用意（過大な原画はビルド/実行を重くする）。
- **表示スロットのアスペクト比に合わせる**のが最重要（現状の縦長→横長流用の切り抜け問題を回避）。
- MauiImage は縮小自動生成なので「一番大きく表示される箇所 × 3倍」を1枚用意すれば全密度に足りる。

---

## 6. 新規ファイル名 候補一覧・枚数・優先度

### 6-1. 候補ファイル名（全展開・作成チェックリスト）

下記は各画像に付ける**ファイル名の候補**（`movie01.jpg` のような連番命名）。名前は暫定案なので、
プロジェクトの好みに合わせて変更してよい（例：`poster`→`movie`、`gallery`→`photo` 等）。
連番はゼロ埋め2桁で揃える。

```
Resources/Images/   （MauiImage … XAMLで <Image Source="ファイル名"> と直接指定）

  ■ プロフィール（UIProfile / UIProfile2）  ★★★
      avatar_user.jpg
      profile_cover.jpg
      gallery01.jpg  gallery02.jpg  gallery03.jpg
      gallery04.jpg  gallery05.jpg  gallery06.jpg

  ■ ショッピング（UIShop / UIItem / UICart）  ★★★
      product_apparel01.jpg  product_apparel02.jpg  product_apparel03.jpg
      product_beauty01.jpg   product_beauty02.jpg   product_beauty03.jpg
      product_beauty04.jpg   product_beauty05.jpg   product_beauty06.jpg

  ■ 動画配信（UIStream / UIStreamDetail）  ★★★
      poster01.jpg   poster02.jpg   poster03.jpg
      poster04.jpg   poster05.jpg   poster06.jpg
      stream_hero.jpg
      stream_clip01.jpg  stream_clip02.jpg  stream_clip03.jpg

  ■ オンボーディング（UIKitOnboard）  ★★
      onboard01.jpg  onboard02.jpg  onboard03.jpg

  ■ ペット（UIPet）  ★★
      pet01.jpg  pet02.jpg  pet03.jpg

  ■ プロモ／スーパーアプリ（UISuper）  ★★
      banner01.jpg  banner02.jpg  banner03.jpg

  ■ チャット（UIChat）  ★（任意）
      avatar_person01.jpg  avatar_person02.jpg  avatar_person03.jpg
      avatar_person04.jpg  avatar_person05.jpg

  ■ ログイン（UILogin）  ★（任意）
      login_hero.png

Resources/Raw/      （MauiAsset … 既存プレースホルダの差し替え）

  ■ ソーシャルHUD（UISocial）  ★
      Social/player.jpg      ← 差し替え

  ■ メール差出人（UIMail）  ★
      Avatar/mofusand.jpg    ← 差し替え
```

### 6-2. 枚数・優先度サマリ

| 優先度 | 画面/用途 | 候補ファイル名 | 枚数 |
| --- | --- | --- | --- |
| ★★★ | プロフィール | `avatar_user` / `profile_cover` / `gallery01`〜`06` | 8 |
| ★★★ | ショッピング | `product_apparel01`〜`03` / `product_beauty01`〜`06` | 9 |
| ★★★ | 動画配信 | `poster01`〜`06` / `stream_hero` / `stream_clip01`〜`03` | 10 |
| ★★ | オンボーディング | `onboard01`〜`03` | 3 |
| ★★ | ペット | `pet01`〜`03` | 3 |
| ★★ | プロモ（Super） | `banner01`〜`03` | 3 |
| ★ | チャット | `avatar_person01`〜`05` | 5 |
| ★ | ログイン | `login_hero` | 1 |
| ★ | ソーシャルHUD | `Social/player.jpg` 差し替え | 1 |
| ★ | メール | `Avatar/mofusand.jpg` 差し替え | 1 |

**合計：新規 42 枚 + 差し替え 2 枚**

まず ★★★ の 3 グループ（プロフィール・ショッピング・動画配信＝流用が最も目立つ画面）から
着手すると、サンプルの見栄えが大きく改善する。

---

## 7. 差し替え時に修正が必要なコード（参考）

追加画像を反映する際に編集する主なファイル。

- プロフィール: `Modules/UI/UIProfileViewModel.cs`（`usa{n}_full` 配列）、`UIProfileView.xaml`（カバー=`social_background.png` / アバター=`profile.jpg`）
- ショッピング: `UIShopViewModel.cs`（商品Image）、`UICartViewModel.cs`、`UIItemView.xaml`（`usa1_face.jpg`）
- 動画配信: `UIStreamViewModel.cs`・`UIStreamView.xaml`（`social_background.png`）、`UIStreamDetailViewModel.cs`・`UIStreamDetailView.xaml`
- オンボーディング: `UIKitOnboardViewModel.cs`（3件とも `social_background.png`）
- ペット: `UIPetView.xaml`（`usa1_full.jpg`）
- プロモ: `UISuperViewModel.cs`
- ログイン: `UILoginView.xaml`（`profile.jpg`）
- ソーシャル: `Controls/SocialControls.cs`（`player.jpg`）
- メール: `UIMailViewModel.cs`（`mofusand.jpg` 他）
