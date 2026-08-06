# コードレビュー対応 — 実施記録と残課題

Template.MobileApp のコードレビュー(2026-08-05)に始まる一連の修正の**単一の管理ドキュメント**。
旧 `Code_Review.md` / `Fix_Plan.md` / `Implementation_Plan.md` は作業用ファイルのため本書に集約し削除した。

- **ベースコミット**: `2f533e0f`「Apply code review fixes to Template.MobileApp」(125ファイル・Phase 0〜9)
- **本書の2軸**:
  - **第1部 = 残課題**(これから判断・対応するもの)
  - **第2部 = `2f533e0f` 以降に行った修正**(画面・ソース単位の記録)
- **状態**: ビルド 0 警告・0 エラー(`net10.0-android` Debug)。第2部の変更は**未コミット**

---

# 第1部 — 残課題

## 1. 判断が必要な項目

### J-3. `.editorconfig` の IDE0079 無効化(死んだ抑止の再発防止)

| 場所 | 設定 |
|---|---|
| `.editorconfig:99` | `dotnet_remove_unnecessary_suppression_exclusions = all:warning` |
| `.editorconfig:490` | `dotnet_diagnostic.IDE0079.severity = warning` |

99行目が**全ルールを IDE0079 の検査対象から除外**しているため、490行目の `warning` 指定は事実上死んでいる。これが「発火し得ない `#pragma` が9行残っていた」原因。

- **(a)** 99行目を `none` にして有効化 → 不要な抑止がビルド警告になる
- **(b)** 現状維持

⚠️ (a) にすると**残り34件の抑止から警告が噴き出す可能性**があり、量が読めない。まず変更してビルドし件数を見てから本採用を判断する進め方を推奨。

## 2. 未対応(優先度低)

- `SecureStorage.GetAsync` の復元・キーストア無効化時の例外が未捕捉(エッジケース)
- `HttpService` の `CancellationToken` を `NetworkOperator` のデリゲート型経由で呼び出し側から渡せるようにする(現在は口が無く未使用。転送は10分の有限タイムアウトで暫定対応済み)

## 3. 実機検証が必要な項目(実物・サーバが必要)

| # | 内容 | 関連 |
|---|---|---|
| 1 | NFC: タグ複数回読取→画面離脱→再入場。途中で離しても継続 | NFC 例外処理 |
| 2 | Bluetooth: ペアリング相手ありでの印刷成功パス | 状態復帰 |
| 3 | Android 11 実機でのレガシー Bluetooth 権限 | Manifest |
| 4 | Network: 大容量 Download/Upload の完走・進捗、サーバ停止時のリトライ上限 | 転送・リトライ |
| 5 | サーバ時刻表示の TZ(`ToLocalTime()` 追加済みだが要 API サーバ) | DateTime |
| 6 | CV サンプル: キャプチャ→検出の繰り返しでメモリ増加なし | SKBitmap |
| 7 | 端末 TZ を変えて DB 保存→表示 | DateTime |

## 4. レビュー時に「対応不要」と判断した項目(再掲)

除外: `OnNotifyFunction1` の116ファイル重複解消 / SemanticProperties・AutomationId の付与 / gRPC・SignalR・Ollama の実装 / QR コードからの通信先・APIキー無検証受け入れ

保留: ダークモード対応 / ローカライズ整備 / iOS 対応 / DB マイグレーション機構

---

# 第2部 — `2f533e0f` 以降に行った修正

すべて**未コミット**。ビルド 0 警告・0 エラーを維持。

## 2-1. 今回のコミットが持ち込んだ退行の修正

| 対象 | 内容 | 検証 |
|---|---|---|
| `Platforms/Android/AndroidManifest.xml` | **`BATTERY_STATS` を復活**。`Permissions.EnsureDeclared` は権限の付与ではなく **manifest への宣言**の有無を見るため、削除すると `IBattery` が `PermissionException` を投げる。`DeviceState` のコンストラクタが `battery.ChargeLevel` を読むため **Device > Status でクラッシュ**していた | ✅ 実機でクラッシュ再現→修正後 Status 画面の正常表示(Battery 80%) |
| `Modules/Device/DeviceQrScanViewModel.cs` | **カメラ権限の Check→Request 追加**。起動時の一括要求を削除した際の漏れで、新規インストール時に QR スキャンが動作しなかった | ✅ 権限剥奪→再現→修正後は権限ダイアログ→プレビュー動作 |
| `Modules/Main/SettingViewModel.cs` | 同上 | ✅ 設定画面の正常動作 |
| `Usecase/NetworkUsecase.cs` | **サーバ時刻表示に `.ToLocalTime()` 追加**。DateTime の UTC 統一の副作用で、DB経路には変換が入ったが API経路は未変更のままで**表示が9時間ずれていた**(修正前は正しく表示されていた退行) | ⬜ 要 API サーバ |

## 2-2. 実装漏れ・積み残しの補完

| 対象 | 内容 | 検証 |
|---|---|---|
| `Modules/Device/DeviceNfcViewModel.cs` `Components/NfcExtensions.cs` `Log.cs` | **NFC の Rx シーケンス死亡経路を解消**。`ConvertResult` を `ParseTag` に分離し解析例外(`ArgumentException`/`OverflowException`/`IndexOutOfRangeException`)を null 化+WARNログ、`Subscribe` に `onError` 追加、`SubArray` の負サイズをガード。※`IsValidLog` は process バイトしか見ないが `new DateTime` の month は4bit・day は5bit のため、化けたブロックで確実に例外になる | ⬜ 要 NFC タグ |
| `Modules/Device/DeviceBluetoothViewModel.cs` | **`finally` で状態復帰**。想定外の例外で `State=Printing` のまま `IsBusy` が固着し印刷ボタンが永久に無効化される問題 | ✅ 相手不在で実行→30秒でタイムアウト→「Failed to connect.」表示、**ボタン再操作可** |
| `Modules/UI/UIMeterViewModel.cs` | **二重起動ガード**(`if (loopTask is not null) return;`)+ `await loopTask` の try/finally 化 | ✅ 入場→退場→再入場で 63 fps |
| `Modules/Device/DeviceAudioViewModel.cs` | 購読前に `polling?.Dispose()`(二重購読の防止) | ✅ 再入場で正常 |
| `Modules/UI/UIGraph2ViewModel.cs` | `?? RepositoryData.Empty` に統一(`UIGraphViewModel` と同じ修正が漏れていた) | ✅ グラフ表示正常 |
| `Controls/RadarScreen.cs` | **`Loaded`/`Unloaded` を併用**。CTS 解放が `OnHandlerChanged` のみに依存しており、Handler が null にならない経路で 60fps ループが生き残る | ✅ 再入場後も掃引継続(2フレーム間で3641点が変化) |
| `ApplicationInitializer.cs` `App.xaml.cs` `Log.cs` | **DB初期化失敗時に原因を提示して終了**。従来は例外が `async void` の `OnStart` から漏れ、**無言でクラッシュ→次回起動でも同じ所で落ちる**ループから復帰できなかった。`IOException` / `UnauthorizedAccessException` / `SqliteException` のみを捕捉して `InitializeError` に保持し、`OnStart` で `DisplayAlertAsync` 表示後に `Quit()`。**捕捉する型を絞ったため CA1031 の新規抑止は不要** | ✅ 一時的に例外を注入して検証 — 「Initialize error / Failed to initialize database.」が表示され、Exit でプロセス正常終了(FATAL 0件、ERRORログ出力を確認)。注入コードは除去済み |

## 2-3. 効果が限定的だった箇所の是正

| 対象 | 内容 |
|---|---|
| `Services/AppHostBuilderExtensions.cs` | 転送クライアントの Timeout を `Timeout.InfiniteTimeSpan` → **10分**。呼び出し側にトークンを渡す口が無く「タイムアウト無し・中断手段無し」になっていた |
| `Services/DataService.cs` | `busy_timeout` を**接続文字列 `Default Timeout=3`** に変更(PRAGMA は接続単位で他接続に効かなかった)。WAL 有効化に伴う **`-wal`/`-shm` の削除漏れ**も修正 |
| `MainPageViewModel.cs` | `HeaderVisible` の既定値を `true` → **`false`**。初回ナビゲーションまで空タイトルのヘッダーが表示されていた |

## 2-4. 軽微な修正

| 対象 | 内容 |
|---|---|
| `Extensions.cs` | `ReplaceBitmap` に自己代入ガード。※早期 return だとアナライザの所有権追跡が切れて CA2000 が出るため、**代入は無条件のまま Dispose のみ条件化** |
| `Modules/UI/UIMailViewModel.cs` | `SKBitmap` を `Disposables.Add` して画面破棄時に解放(アンマネージドメモリのリーク) |
| `Shell/DiagnosticPanel.xaml.cs` | 世代番号でタイマーの多重起動を防止(`StopMonitor` はフラグを倒すだけで次tickまでタイマーが生存するため) |
| `Modules/Device/DeviceBleScanViewModel.cs` | 外側タイマ購読にも `onError` を追加 |
| `State/Settings.cs` | `SetAIServiceKeyAsync` が空文字なら `SecureStorage.Remove`(キーを削除できなかった) |
| `Resources/Styles/Styles.xaml` | 参照0のスタイル4件を削除(`FillHorizontalStack` / `InputEntry` / `ItemCollectionLabel` / `SideFlexLayout`) |
| `Modules/Device/DeviceOcrViewModel.cs` | カメラ権限の戻り値を `IsCameraEnabled` で受け、未許可ならキャプチャしない(他画面と不統一だった) |
| `Modules/Sample/SampleCvLocalViewModel.cs` | **再入防止ガード**。`await DetectAsync(bitmap)` 中に再実行されると `ReplaceBitmap` が推論中のビットマップを破棄する(use-after-dispose) |

## 2-5. `#pragma warning disable` の棚卸し

**方針**: 除去可能なものは削除。理由コメントの追加は不要。複数クラスをまとめてファイル先頭で抑止しているケースは対応不要。

**削除した「発火し得ない抑止」9行 / 5ファイル**(除去してもビルド警告が出ないことを確認):

| ファイル | ルール | 理由 |
|---|---|---|
| `Controls/SpeedGauge.cs` | CA1001 | disposable フィールドが1つも無い(`RadarScreen` からのコピー残り) |
| `Models/Sample/NewsItem.cs` | CA1056 | Url/Uri メンバが無い |
| `Modules/Device/DeviceInfoViewModel.cs` | SA1135 | using ディレクティブが1本も無い |
| `Behaviors/LabelOption.android.cs` | CA1416×2 | `Analyzers.ruleset` で Hidden 指定のため元々発火しない |
| `Controls/SocialControls.cs` | CA1822 | restore 欠落でファイル末尾まで抑止が効いていたが、除去しても警告ゼロ=不要だった |

**リーク系に実害なしと確認**: CA2000 の5件はすべて所有権移譲(Animation→AnimationManager、TileLayer→Map.Layers、InputFilter→ネイティブ TextView 等)で、破棄すると逆に機能が壊れる。`Border.android.cs` の CA1001 は `OnDetachedFrom` で確実に解放。

※ 分析では「除去可能」とされた **IDE0028 の3件は実際には必要**だった(除去→警告化→差し戻し)。

## 2-6. Style キー命名の一貫性(8-2 の仕上げ)

**方針**: 重複しない名称を付けるのは可。ただし**衝突したキーだけを直すのではなく、同じ画面の同じ役割グループ全体を同じ規則に揃える**。

コミット時点では各画面で**衝突した1〜2キーだけにプレフィックスが付き、同じ `Header*` / `Value*` などの兄弟キーが取り残されて**いた。以下40キー(98箇所)を追加でリネームし、グループ内の表記を統一。

| 画面 | プレフィックス | 追加リネームしたキー |
|---|---|---|
| DeviceBleScanView | `BleScan` | `ValueHorizontalStack` |
| UIGraphView | `Graph` | `HeaderBarBorder` |
| UIItemView | `Item` | `TitleRowGrid` |
| UILoadView | `Load` | `ValueSpan` / `UnitSpan` |
| UIShopView | `Shop` | `HeaderTextStack` |
| UIMailView | `Mail` | `HeaderRowLayout` / `HeaderColumnLayout` / `HeaderOptionButton` / `HeaderToggle` / `HeaderFilterButton` |
| ViewRefreshView | `Refresh` | `HeaderLayout` / `HeaderRow` / `HeaderIcon` / `HeaderCountBorder` / `HeaderCountLabel` |
| UIPosView | `Pos` | `SubNameLabel` / `NameWithSubLabel` / `NameWithSubNameSpan` / `NameWithSubOptionSpan` / `SubValueLabel` / `LargeValueLabel` / `HugeValueLabel` |
| UIMoneyView | `Money` | `Header*` 14件(`HeaderBackgroundBorder`〜`HeaderPointUnitSpan`)+ `MenuBorder` / `MenuImageButton` / `MenuLabel` |

- **対象外**: `RootGrid` / `RootScroll` / `SectionTitleRow` / `SectionAccent` などは、以前のスタイル切り出し作業で**画面をまたいで同じ役割に同じキー名を使う規約**として意図的に統一されたもののため触っていない
- **検証**: 全 XAML を走査し**未解決の `StaticResource` 参照 0 件**を確認(解決失敗は起動時クラッシュになるため)。実機で Money(17キー・最多)/ Mail / POS の表示が変化していないことを確認
- 補足: `UIMoneyView` と `UIKitTrackingView` は `HeaderBorder` を**それぞれ独立にローカル定義**しており相互参照はない(Money 側のみ変更)

## 2-7. 過剰と判断して差し戻したもの

| 対象 | 内容 |
|---|---|
| `Modules/Sample/SampleCvNet{Object,Tag,Face,Ocr,People}ViewModel.cs` | **基底クラス共通化(Phase 7-3③)を差し戻し**、5画面を独立 VM に戻して `SampleCvNetViewModelBase.cs` を削除。サンプルは1画面で完結して読める方が良いため。**H-3 のビットマップ所有権(`ReplaceBitmap`)とカメラ権限チェックは維持**。推論が `// TODO` で await が無いため再入ガードは付けていない |
| `#pragma warning disable` の理由コメント | Phase 9-2 で追記された「〜のため抑止」形式のコメント **27件を削除**(不要との判断) |

## 2-8. 今回のコミット以前から存在した不具合の修正

| 対象 | 内容 | 検証 |
|---|---|---|
| `Modules/Navigation/Edit/EditListView.xaml` | **編集/削除ボタンでクラッシュ**。`Button` の**要素レベル** `x:DataType` により `CommandParameter="{Binding}"` が `TypedBinding<EditListViewModel, EditListViewModel>` としてコンパイルされ、実際の BindingContext(`WorkEntity`)と不一致で **null** になる。null が `SelectCommand` に渡り `EditDetailViewModel.OnNavigatingToAsync` で NRE。該当行は今回のコミットでは変更されておらず**以前から存在**(8-1 の作業中に発見) | ✅ クラッシュ再現→修正後は `EditDetailUpdate` 画面が開き `Sample-1` が正しく渡る |

## 2-9. 調査の結果「問題なし」と確認した項目

| 項目 | 結果 |
|---|---|
| `UIProfileView` の FlexLayout 折り返し | **問題なし**。実機で**3列×2行**の正常表示を確認(`Basis="33.33%"` + `Margin="2"` でも折り返さない) |
| `x:DataType` の RelativeSource 付与(8-1) | 生成コードは従来の `Binding` のままで **TypedBinding 化されていない**(外しても警告は出ない)。害はなく意図の記述として機能し、将来 MAUI が対応すれば効くため**現状維持** |
| `SQLitePCLRaw` 3.0.5 | `dotnet list package --vulnerable` で**「脆弱なパッケージはありません」**。実機で Data の CRUD も正常動作。**対応不要** |

## 2-10. 環境に関する記録

- ビルド失敗(NU1605×8)が一時発生したが、**今回の修正とは無関係**の submodule 版数ズレが原因だった(作業ツリーの MauiComponents / Smart.Maui が gitlink より新しく、Template の csproj が要求する版と乖離)。ユーザーがライブラリを更新して解消
- 検証環境: Pixel 9a(`4A071JEBF16992`)。`adb` は PATH 未登録(`C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe`)
- 端末には類似の別 MAUI アプリ(`template.mobileapp2` 等)が入っているため、撮影前に `dumpsys window | grep mCurrentFocus` でフォアグラウンドを確認すること
