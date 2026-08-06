# 修正実施チェックリスト(検証後の残課題)

[Code_Review.md](Code_Review.md) → [Fix_Plan.md](Fix_Plan.md) → [Implementation_Plan.md](Implementation_Plan.md) の実施結果を、
**2026-08-06 に第三者検証**(静的レビュー5系統 + 実機 Pixel 9a での動作確認)した結果の**残課題のみ**を残す。

**検証で妥当と確認できた項目(Phase 0〜9 の大半)は本書から削除済み。**

## 検証サマリ

| 観点 | 結果 |
|---|---|
| ビルド | `dotnet build -f net10.0-android -c Debug` **0 警告 / 0 エラー**(ライブラリ更新後) |
| 実機起動 | 正常(起動 → メニュー → 各画面遷移) |
| Phase 1(確定バグ11件) | **全件 OK**。実機で Data CRUD / EditList 表示も確認 |
| Phase 2(Components 11件) | 主目的すべて OK(二重返却・デッドロック・リークとも解消)。2-4 に残存経路 |
| Phase 3(起動・権限・Manifest) | 3-1 OK。**3-2 / 3-3 に退行**(下記 A-2 / 対応済 A-1) |
| Phase 4(セキュリティ4件) | **全件 OK** |
| Phase 5(データ・通信7件) | 5-2/5-4/5-5/5-6 OK。**5-1 に退行**(下記 A-3)、5-3/5-7 は効果限定 |
| Phase 6/7/9 | 6-1/6-3/6-4/7-1/7-2/7-3/9-3 OK。9-2 は未達 |
| Phase 8(UI/XAML 6件) | 8-2/8-3/8-4 OK。8-1 は無害だが実効なし |

---

## A. 実機で再現を確認した不具合(最優先)

### A-1. BATTERY_STATS 削除でクラッシュ [3-3 の退行] — **対応済み(要コミット)**

- 症状: **Device > Status でアプリが即クラッシュ**。`NetworkOperator` 経由でも発生しうる
- 原因: `Permissions.EnsureDeclared<Battery>()` は**付与(grant)ではなく宣言(declare)の有無**を見るため、`BATTERY_STATS` を manifest から消すと `IBattery` が `PermissionException` を投げる。`DeviceState` のコンストラクタが `battery.ChargeLevel` を読むため DI 解決時点で落ちる
- 実機ログ: `PermissionException: You need to declare using the permission: 'android.permission.BATTERY_STATS'` → `BatteryImplementation.get_ChargeLevel()` → `DeviceState..ctor`
- [x] **対応済**: `AndroidManifest.xml` に `BATTERY_STATS` を理由コメント付きで復活 → **実機で Status 画面の正常表示を確認**(Battery 80% / Network 表示)
- 備考: Fix_Plan の削除根拠「signature|privileged で通常アプリには付与されない」は grant の話で、declare の要否とは別問題だった

### A-2. QR スキャン・設定画面のカメラ権限漏れ [3-2 の退行] — **対応済み(要コミット)**

- 症状: **新規インストール時に QR スキャンが動作しない**(カメラプレビューが白画面のまま・権限ダイアログも出ない)
- 実機検証: `pm revoke ... CAMERA` 後に Device > QR Scan を開いて再現確認済み
- 原因: 3-2 で起動時の一括権限要求を削除した際、`Controller.Enable = true` のみの2画面が権限要求の追加対象から漏れた。`BarcodeScanning.Native.Maui` はライブラリ側で自動要求しない設計
- [x] `Modules/Device/DeviceQrScanViewModel.cs` — `OnNavigatedToAsync` を async 化し `Permissions.RequestCameraAsync()` 許可時のみ `Controller.Enable = true`(`DeviceCameraViewModel` と同じパターン)
- [x] `Modules/Main/SettingViewModel.cs` — 同上
- [x] **実機確認**: 権限剥奪状態で QR Scan を開くと権限ダイアログが出て、許可後にカメラプレビューが表示される。設定画面も同様に動作
- [ ] 拒否時に何も表示しない点は他画面(`DeviceCameraViewModel` 等)と同じ挙動。メッセージ表示を統一するかは別途判断

### A-3. サーバ時刻表示が9時間ずれる [5-1 の退行] — **対応済み(要コミット・実機未検証)**

- 症状: Network > サーバ時刻取得の表示が **UTC のまま**(JST で9時間前)。**修正前は正しく表示されていた明確な退行**
- 原因: `DateTimeConverter` を UTC 正規化した副作用。DB 経路(`DataViewModel.cs:87`)には `.ToLocalTime()` が入ったが、API 経路は**このコミットで一度も変更されていない**ため変換が抜けた
- [x] `Usecase/NetworkUsecase.cs:37` — `result.Value.DateTime.ToLocalTime()` に変更(DB 経路と同じ方針)
- [ ] **実機検証は API サーバが必要なため未実施**(Implementation_Plan §5 の #9/#11 と併せて実施)
- [ ] 併せて `DateTimeTypeHandler.SetValue` が `Kind=Unspecified` を UTC 扱いで保存する点の可否を判断(現状 Unspecified を保存する経路はないが、DatePicker 値等を保存すると同種のずれが出る)

---

## B. 静的検証で見つかった要対応(実機未再現・発生条件あり)

### B-1. NFC の Rx シーケンス死亡経路が残存 [2-4]

- 計画された修正(`Access` の例外捕捉)は入っているが、**購読側 `Select`(`ConvertResult`)内の例外**は依然として OnError → シーケンス永久終了 → `ObserveOnCurrentContext` 経由で UI スレッド未処理例外(クラッシュ)になる
- 現実的な発火点: `Domain/Logic/SuicaLogic.cs:104,114` の `new DateTime(year, month, day)`(化けたブロックで月/日が0)、`Components/NfcExtensions.cs:78` の `SubArray` 長さ負
- [ ] `ConvertResult` を try/catch して失敗時 null を返す、または `Subscribe(onNext, onError)` を付ける
- [ ] `Nfc.android.cs:170` のログが `Debug.WriteLine`(Release で消える)→ 計画どおり ILogger の WARN へ

### B-2. Bluetooth 印刷の状態復帰が不完全 [6-2] — **対応済み(要コミット)**

- `catch (IOException)` / `catch (Java.Lang.Throwable)` のみで **finally がない**。想定外の例外(`ObjectDisposedException` 等)で `State=Printing` のまま `IsBusy` が固着し、**印刷ボタンが二度と押せなくなる**
- [x] `finally` で `State is Connecting or Printing` なら `Failed` へ復帰させる(`DeviceBluetoothViewModel.cs`)
- [x] **実機確認**: 相手不在で Print 実行 → 約30秒のタイムアウト後に「Failed / Failed to connect.」表示、**Print ボタンが再度有効**(2-2 のタイムアウトも同時に検証)

### B-3. ループ画面の二重起動ガードなし [6-5] — **対応済み(要コミット)**

- `UIMeterViewModel.cs` / `DeviceAudioViewModel.cs` が `OnNavigatedToAsync` で CTS / loopTask / polling を**無条件に上書き**。2回呼ばれると前回のループを停止する手段が失われ 60fps ループが回り続ける
- `UIMeterViewModel` の `await loopTask` が finally 外のため、`OperationCanceledException` 以外で落ちると Dispose 群がスキップされる
- [x] `UIMeterViewModel` — `if (loopTask is not null) return;` の二重起動ガード + `await loopTask` を try/finally 化
- [x] `DeviceAudioViewModel` — `polling?.Dispose();` を購読前に追加
- [x] **実機確認**: UI > Meter / Device > Audio とも「入場→退場→再入場」で正常動作(Meter は再入場後も 63 fps)

### B-4. UIGraph2ViewModel の未修正 [9-1 の片手落ち] — **対応済み(要コミット)**

- `UIGraphViewModel` は `?? RepositoryData.Empty` に修正されたが、**同一構造の `UIGraph2ViewModel` は `data!.Commits` のまま**だった。同ファイルの `RepositoryData.Empty` は完全な未使用コード
- [x] `UIGraphViewModel` と同じ `?? RepositoryData.Empty` に統一
- [x] **実機確認**: UI > Graph2 のコミットグラフが従来どおり表示

### B-5. `#pragma warning disable` の棚卸し [9-2] — **対応済み(要コミット)**

**方針(ユーザー決定)**: 除去可能なものは削除。理由コメントの追加は不要。複数クラスをまとめてファイル先頭で抑止しているケースは対応不要。

- [x] **死んだ抑止 9行 / 5ファイルを削除**(ビルドで「除去しても警告が出ない」ことを確認済み)
  - `Controls/SpeedGauge.cs` CA1001 — disposable フィールドが1つも無く発火し得なかった(`RadarScreen` からのコピー残り)
  - `Models/Sample/NewsItem.cs` CA1056 — Url/Uri メンバが無く発火し得なかった
  - `Modules/Device/DeviceInfoViewModel.cs` SA1135 — using ディレクティブが1本も無く発火し得なかった
  - `Behaviors/LabelOption.android.cs` CA1416×2 — `Analyzers.ruleset` で Hidden 指定のため元々発火しない
  - `Controls/SocialControls.cs` CA1822 — restore 欠落でファイル末尾まで抑止が効いていたが、除去しても警告ゼロ=不要だった
- [x] **リーク系(CA1001/CA2000/CA2213)に実害のあるリークが無いことを確認** — CA2000 5件はすべて所有権移譲(Animation→AnimationManager、TileLayer→Map.Layers、InputFilter→ネイティブTextView 等)で、破棄すると逆に機能が壊れる。`Border.android.cs` は `OnDetachedFrom` で確実に解放
- [x] 理由コメントの追加は**方針により対応不要**(残す34件はそのまま)
- [ ] 参考: `.editorconfig` の `dotnet_remove_unnecessary_suppression_exclusions = all:warning` により **IDE0079(不要な抑止の検出)が無効化**されている。これが死んだ抑止の残存原因。`none` にすれば今後は自動検出できる(要判断)

### B-6. RadarScreen の停止経路 — **対応済み(要コミット)**

- `CancellationTokenSource` の解放が `OnHandlerChanged` の `Handler == null` 時のみに依存。Handler が null にならない経路が生じると **60fps ループが回り続け、クロージャ経由で RadarScreen 自身も生存**する
- [x] `SocialPlayer`/`SocialCounter`/`SocialStatus` と同じく **`Loaded`/`Unloaded` を併用**(`OnHandlerChanged` は従来どおり残す)。`StartTimer`/`StopTimer` は冪等なので二重呼び出しは無害
- [x] **実機確認**: Radar の掃引・輝点が正常表示。退場→再入場後も 2 フレーム間で 3641 サンプル点が変化=ループが正しく再開することを確認

---

## C. 効果が限定的だった項目 — **対応済み(要コミット)**

### C-1. 転送の中断手段がない [5-3] — 対応済み

- `HttpService` 全メソッドへの CancellationToken 追加・転送用クライアント分離は正しく入っていたが、`NetworkUsecase` / `NetworkOperator` のデリゲート型に**トークンを通す口がない**ため一切渡されず、`Timeout.InfiniteTimeSpan` と相まって**無応答時に中断不能**(修正前の30秒アボートより悪化)だった
- [x] Transfer クライアントの Timeout を `Timeout.InfiniteTimeSpan` → **10分**に変更(呼び出し側がトークンを渡す場合はそちらが優先される旨をコメント)
- [ ] デリゲート型への CancellationToken 追加(本格対応)は未実施。必要なら別途

### C-2. busy_timeout が実質無効 [5-7] — 対応済み

- `PRAGMA busy_timeout` は**接続単位**で `RebuildAsync` の接続にしか適用されず、以後の CRUD は毎回新規接続のため効いていなかった(実測: 別接続では 0)
- [x] 接続文字列に `Default Timeout=3` を追加し、全接続に適用されるよう変更
- [x] WAL 有効化に伴う `-wal` / `-shm` の削除漏れも修正(異常終了後に旧 WAL が新 DB へ適用されるのを防止)

### C-3. x:DataType 付与が実効していない [8-1] — 現状維持と判断

- 10箇所すべてに追加され型も正しいが、生成コードは従来の `Binding`(リフレクション)のままで **TypedBinding 化されていない**。`x:DataType` を外しても警告は出ない(検証済み)
- **判断: 現状維持**。害はなく、意図の記述として機能し、将来 MAUI 側が対応すれば自動的に効果が出るため

### C-4. HeaderVisible の既定値変更 [7-1] — 対応済み

- `new NotificationValue<bool>(true)` により初回ナビゲーションまで**空タイトルのヘッダーバーが表示**されていた(Phase 3-1 で起動待ちが伸びたため目立つ)
- [x] `new()`(false)に戻した

---

## D. 軽微 — **主要項目は対応済み(要コミット)**

- [x] `Extensions.cs` `ReplaceBitmap` の自己代入ガード追加(※早期 return だとアナライザの所有権追跡が切れて CA2000 が出るため、**代入は無条件のまま Dispose のみ条件化**)
- [x] `UIMailViewModel` の `SKBitmap` リーク — `Disposables.Add(bitmap)` で画面破棄時に解放
- [x] `DiagnosticPanel` のタイマー多重起動 — 世代番号(`monitorGeneration`)で古いタイマーを打ち切り
- [x] `DeviceBleScanViewModel` 外側タイマ購読の OnError ハンドラ追加
- [x] `Settings.SetAIServiceKeyAsync` — 空文字時は `SecureStorage.Remove` でキーを削除できるように
- [x] `Styles.xaml` の参照0キー4件を削除(`FillHorizontalStack` / `InputEntry` / `ItemCollectionLabel` / `SideFlexLayout`。削除前に全 XAML/CS で参照0を再確認、削除後の起動も確認)
- [ ] `SampleCvLocalViewModel` — `ReplaceBitmap` 後の `await DetectAsync(bitmap)` 中に再実行されると use-after-dispose(コマンドの実行中ガードに依存。**要判断**)
- [ ] `DeviceOcrViewModel` — `RequestCameraAsync()` の戻り値を破棄しており他画面と不統一(拒否時の扱いを統一するか要判断)
- [ ] `SecureStorage.GetAsync` の復元/キーストア無効化時の例外が未捕捉(エッジケース)
- [ ] `UIProfileView` — `FlexLayout.Basis="33.33%"` + `Margin="2"` で3枚目が折り返す可能性(見た目の確認が必要)
- [ ] 初期化失敗時に「クラッシュ→ログ表示→再クラッシュ」の起動ループから復帰できない(3-1 の fail-fast 方針の副作用・**要判断**)
- [ ] `SQLitePCLRaw` のピンが後続コミットで 2.1.12 → 3.0.5 に変更されている(**意図確認**)

---

## E. 本検証で変更したファイル(未コミット)

すべてビルド 0 警告・0 エラーを確認済み。

| ファイル | 対応 | 実機確認 |
|---|---|---|
| `Platforms/Android/AndroidManifest.xml` | A-1(BATTERY_STATS 復活) | ✅ Status 画面の正常表示 |
| `Modules/Device/DeviceQrScanViewModel.cs` | A-2(カメラ権限 Check→Request) | ✅ 権限ダイアログ→プレビュー動作 |
| `Modules/Main/SettingViewModel.cs` | A-2(同上) | ✅ 設定画面の正常動作 |
| `Usecase/NetworkUsecase.cs` | A-3(`ToLocalTime()` 追加) | ⬜ 要 API サーバ |
| `Modules/Device/DeviceNfcViewModel.cs` | B-1(解析例外を null 化+onError) | ⬜ 要 NFC タグ |
| `Components/NfcExtensions.cs` | B-1(`SubArray` の負サイズガード) | ⬜ 要 NFC タグ |
| `Log.cs` | B-1(`WarnNfcReadError` 追加) | — |
| `Modules/Device/DeviceBluetoothViewModel.cs` | B-2(finally で状態復帰) | ✅ 失敗後もボタン再操作可 |
| `Modules/UI/UIMeterViewModel.cs` | B-3(二重起動ガード+try/finally) | ✅ 再入場で 63 fps |
| `Modules/Device/DeviceAudioViewModel.cs` | B-3(購読の重複防止) | ✅ 再入場で正常 |
| `Modules/UI/UIGraph2ViewModel.cs` | B-4(null フォールバック統一) | ✅ グラフ表示正常 |
| `Controls/SpeedGauge.cs` ほか5ファイル | B-5(死んだ抑止 9行の削除) | ✅ 起動確認 |
| `Controls/RadarScreen.cs` | B-6(Loaded/Unloaded 併用) | ✅ 再入場で掃引が再開 |
| `Services/AppHostBuilderExtensions.cs` | C-1(転送 Timeout を10分に) | — |
| `Services/DataService.cs` | C-2(`Default Timeout`+`-wal`/`-shm` 削除) | ✅ 起動=DB再構築が正常 |
| `MainPageViewModel.cs` | C-4(HeaderVisible 既定値) | ✅ 起動確認 |
| `Extensions.cs` | D(`ReplaceBitmap` 自己代入ガード) | ✅ 起動確認 |
| `Modules/UI/UIMailViewModel.cs` | D(SKBitmap の解放) | ✅ 起動確認 |
| `Shell/DiagnosticPanel.xaml.cs` | D(タイマー多重起動の防止) | ✅ 起動確認 |
| `Modules/Device/DeviceBleScanViewModel.cs` | D(外側タイマの onError) | ✅ 起動確認 |
| `State/Settings.cs` | D(キー削除経路) | ✅ 起動確認 |
| `Resources/Styles/Styles.xaml` | D(未参照スタイル4件の削除) | ✅ 起動確認(解決失敗なし) |
| `Modules/Navigation/Edit/EditListView.xaml` | 下記の既存不具合 | ✅ 編集画面が開き値が渡る |

### EditList の編集/削除ボタンでクラッシュ(既存不具合・今回の退行ではない)

- 症状: **Navigation > Edit > 一覧の編集(鉛筆)/削除ボタンでアプリがクラッシュ**
- 原因: `Button` の**要素レベル** `x:DataType="{x:Type module:EditListViewModel}"`(137/144行)により `CommandParameter="{Binding}"` が `TypedBinding<EditListViewModel, EditListViewModel>` としてコンパイルされ、実際の BindingContext(`WorkEntity`)と不一致で **null** になる。null が `SelectCommand` に渡り遷移先 `EditDetailViewModel.OnNavigatingToAsync` で NRE
- 実機ログ: `NullReferenceException at EditDetailViewModel.OnNavigatingToAsync` → `AsyncCommand<WorkEntity>`
- 該当行は今回のコミットでは変更されておらず**以前から存在した不具合**(8-1 の作業中に発見)
- 対応: 137/144 行の要素レベル `x:DataType` を削除 → **編集ボタンで `EditDetailUpdate` 画面が開き `Sample-1` が正しく渡ることを実機で確認**

---

## F. 実機検証チェックリスト(Implementation_Plan §5)の消化状況

| # | 内容 | 状況 |
|---|---|---|
| 7 | 起動→メニュー→Data 画面が即操作可能 | ✅ 確認済 |
| 9(一部) | Data CRUD(Insert/Query/Update)| ✅ 確認済(採番・UsingAsync とも正常) |
| 4/6(一部) | 権限まわり(カメラ) | ✅ **不具合検出→修正済**(A-2)。剥奪→要求→許可→動作まで確認 |
| — | Device > Status | ✅ **クラッシュ検出→修正済**(A-1) |
| — | Navigation > Edit の編集/削除 | ✅ **クラッシュ検出→修正済**(E) |
| — | Device > QR Scan / Setting | ✅ カメラプレビュー動作 |
| 11(一部) | TZ(API 経路) | ⬜ 修正済だが**要 API サーバ** |
| 1,2,3,5,8,10,12,13,14 | NFC/Bluetooth/Noise/CV/シェル/UI 回帰ほか | ⬜ 未実施(実物・サーバが必要なものを含む) |
