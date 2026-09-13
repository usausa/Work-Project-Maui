# 残作業チェックリスト

残作業(実機確認 / 実テスト / 保留)のマスターチェックリスト。経緯・実装内容・ナレッジ・開発ポリシーは `Change_Summary.md`(付録含む)を参照。
優先順 = 1 節 → 5 節 → 6 節。以降は 2〜4 節。

## 運用ルール

- 作業はこの番号で指示・進行する(例:「5-2 を実施」)。完了した項目は本書から削除し、内容は `Change_Summary.md` に記録する
- **【判断】印の項目はユーザーが決定**(勝手に進めない)。デザイン判断を伴う差分は 1 項目ずつ指示を受けて実施
- 実装・変更を行なう場合の完了条件 = **ビルド警告ゼロ** + `Change_Summary.md` への記録(開発ポリシーは同 付録A)
- コミットはユーザーが実施(グループ単位を推奨)
- 描画・性能の計測は **Release ビルド + 実機**(手順は `Development.md` の「Releaseビルドでの検証と計測」)

## 前提(環境)

- **環境制約 (不具合ではない)**: ①地図タイルは Google Maps API キー未設定だと非表示 (ピン・カメラ移動は動作) ②SampleCvNet 系は AI エンドポイント未設定だと画面に入れない ③CommunityToolkit CameraView の `CaptureAsync` がまれに未完了になり Function キーが無反応化 (再起動で回復)
- 現在実機に入っているのは **Debug ビルド**(2026-09-13 デプロイ。性能・描画の確認時は Release へ入れ替える)

---

## 1. 実機確認で判明した要対応項目

- [ ] **1-1** Basic > Behavior: **`MaskedBehavior` が正しく動作しない**(電話番号の `000-0000-0000` 整形が効かない)。原因調査から
- [ ] **1-2** Basic > Validation: パスワード一致の相関検証が**フォーカスを外した時のみ**動く → **入力が変わる度**に検証する方式へ
- [ ] **1-3** App > Sudoku: 盤面の**線が描画されていない箇所がある**
- [ ] **1-4** App > Calculator: **ボタンをもう少し大きく**したい(上部に余白があるため活用できる)
- [ ] **1-5** View > DragDrop: **ドロップ先が分かりやすい**表現がほしい(現状はゴミ箱のみハイライト)

---

## 2. SCP の実機確認と転送実テスト(要 SSH サーバ)

- [ ] **2-1** Network メニュー: 「SCP」が追加され遷移できる(未設定時は接続先が「未設定 (設定画面の QR で投入)」でボタン無効)
- [ ] **2-2** Main > Setting: 項目の**ラベルと現在値が横並び**で表示される。SCP セクション(Host/User/Password)があり、QR(`ScpHost=...` 形式)を読むと反映される
- [ ] **2-3** Network > SCP: QR 投入後、「アップロード」でファイル選択 → 進捗バー → 完了ログ。「ダウンロード」で同ファイルがキャッシュへ取得される。転送中「キャンセル」で中断。接続後にサーバのホスト鍵指紋が参考表示される(照合は行わない)

---

## 3. 他案件の残課題(優先度低)

- [ ] **3-1** `SecureStorage.GetAsync` の復元・キーストア無効化時の例外が未捕捉 (エッジケース)
- [ ] **3-2** `HttpService` の `CancellationToken` を `NetworkOperator` のデリゲート型経由で呼び出し側から渡せるようにする (現在は口が無く未使用。転送は 10 分の有限タイムアウトで暫定対応済み)

---

## 4. バックログ(任意・後日。指示があれば着手)

- [ ] **4-1**【判断】`Controls/ChatView` バブル色のバインダブル化(C-13・D18): 検討扱い・未確定
- [ ] **4-2** UISocial 背景の専用化(1080×1920 / 9:16 のゲーム風背景)
- [ ] **4-3** `AnimationOption.ResetEnter` の Scale 1 固定リセット(静的 Scale+EnterAnimation 併用が将来出た場合に、TranslationY と同じ基準値退避パターンで対処)

---

## 5.【優先】`tmpl-plan-maui.md` からの移管課題

`D:\GitHubTemplate\tmpl-plan-maui.md`(MAUI トラック強化プラン)の未対応項目(同書の番号を併記)。**keyboard / blazor 向けの対応(同書 §4 / §5)と iOS 対応(同書 3-13。保留継続)は対象外**。

### template-maui への反映(同書 §1)

- [ ] **5-1** template-maui README の TODO 実態同期(同書 1-9)。実装済みの **Chat / Chart / Gauge / Calendar / Media / Cognitive(SampleCvNetFace+Azure.AI.Vision)/ HybridWebView(WebViewBind・WebViewController)** が TODO に残っている。未実装の WiFi manager / Biometric / Bottom sheet / Push / Local notification は TODO のまま残す

### 機能実装(同書 §3)

- [ ] **5-2** WiFi manager 実装(同書 3-1。`DeviceWiFiViewModel` は 6 行の空スタブ)
- [ ] **5-3** 生体認証の完成(同書 3-2。`DeviceBiometricViewModel` も 6 行の空スタブ。画面と ViewId は登録済み)
- [ ] **5-4** Bottom sheet(同書 3-3。実装なし)
- [ ] **5-5** 【判断】Push 通知(FCM)/ Local notification(同書 3-4)
- [ ] **5-6** DB マイグレーション機構(同書 3-5。`DataService.RebuildAsync`=毎起動で物理削除→再作成の user_version ベース置換)
- [ ] **5-7** ダークモード(同書 3-6。`UserAppTheme=Light` 固定・`AppThemeBinding` 0 件。Colors.xaml は 4 テンプレートでバイト一致のため**対応するなら 4 本同時が効率的**)
- [ ] **5-8** ローカライズ拡充(同書 3-7。resx は Messages / Names とも 5 件のみ。機構は動作済み)
- [ ] **5-9** `Controls/SocialControls.cs` の TODO 10 件整理(同書 3-9)
- [ ] **5-10** 【判断】TimeProvider の MAUI 方式(同書 3-10。設定は EmbeddedBuildProperty のビルド時注入方式のため、wpf / avalonia の `AddOptions<T>().ValidateOnStart()` はそのまま移植不可)
- [ ] **5-11** 【判断】Analyzers.ruleset 正典差分 11 ルールの扱い(同書 3-12。CA1416 / CA2007 は MAUI 固有の合理性あり単純追随不可。CA1014 / CA1305 / CA1824 / CA1861 は再検討余地。正典統一トラック〈aidd 側セッション〉と連動)

### ソースレビュー由来(同書 付録)

- [ ] **5-12** `Converters/MailDateTimeStringConverter.cs` の `ConvertBack` 是正(現状 `NotSupportedException` を throw。`Binding.DoNothing` 返却か OneWay 専用の明示へ)

---

## 6. 外部リファレンス評価 第2弾(`Reference_Nova_Nalu.md`)

N2(chrome / プラットフォーム 4 件)と N3 の【判断】項目は同書の番号で指示を受けて着手する。

### 6-1. N3 の判断

- [ ] **N3-6**【判断】Radial / Orbit / Bubble / Loop(Hex は `HoneycombLayout` として実装済み)。デザイン案は提示済み、採否待ち
- [ ] **N3-11**【判断】小改善 2 点(①`ScrollView` 内の `GraphicsView` / `SKCanvasView` のタッチ横取り抑止 ②`WheelDrawing` の色パレット差し替え)。内容の確認中
