# Template (Template.MobileApp) 実装計画

- **前提ドキュメント**: [Code_Review.md](Code_Review.md) (2026-08-05) / [Fix_Plan.md](Fix_Plan.md)
- **本書の位置づけ**: Fix_Plan.md の修正内容を **本環境 (Works3 チェックアウト)** で実施するための実行計画。修正内容そのものは Fix_Plan.md を正とし、本書は「順序・コミット単位・本環境での検証方法・環境固有の前提」を定める
- **作成日**: 2026-08-05

---

## 1. 環境検証結果 (実施済み)

計画立案時に以下を実測で確認した。

| 項目 | 結果 | 対応 |
|---|---|---|
| .NET SDK | 10.0.302 + android ワークロード 36.1.43 (VS 18.8) | ビルド検証は本環境で可能 |
| 実機 / エミュレータ | **なし** (`adb devices` 空、AVD 未作成、emulator 未インストール) | 実行時検証は不可 → §5 のチェックリスト方式で代替 |
| サブモジュール | `Works3/Smart.Maui` / `Works3/MauiComponents` が**未初期化** (空ディレクトリ) → ProjectReference 解決不能で926エラー | `git submodule update --init` を実施済み (gitlink どおりのコミットをチェックアウト) |
| パッケージ整合 | CommunityToolkit.Maui が Template=14.2.0 / MauiComponents(サブモジュール)=14.2.2 で **NU1605 復元失敗** | Template 側を 14.2.2 へ更新済み (未コミット。Phase 0 としてコミットする) |
| ベースラインビルド | **成功 (0 エラー) だが警告 16 件** — 警告ゼロではない | 残警告 3 種の解消を Phase 0 に追加 (下記) |
| XamlStyler | CLI ツール未導入 (`Settings.XamlStyler` は存在) | Phase 8 着手前に `dotnet tool install -g XamlStyler.Console` を導入し `xstyler` で整形確認 |

### ベースラインの残警告 (Phase 0 で解消する)

| 警告 | 内容 | 対応方針 |
|---|---|---|
| CS8785 ×2 | `AccessorGenerator` (BunnyTail.MemberAccessor 1.9.0) が `MissingMethodException` で生成失敗。**Accessor が生成されていない=実行時の欠落リスクあり** | NuGet 最新 1.11.0 へ更新 (BunnyTail.EmbeddedBuildProperty は既に 1.11.0 で世代が揃う)。更新後もう一度失敗するなら報告 |
| CA1849 ×2 | [DeviceMiscViewModel.cs:103](../Template.MobileApp/Modules/Device/DeviceMiscViewModel.cs) の `speech.RecognizeCancel()` — サブモジュール側 `ISpeechService` の API 変化 (Async 版追加) に未追随 | `OnNavigatingFromAsync` 内で `RecognizeCancelAsync()` を await する形へ |
| XA4301 ×14 | APK 内 `libbarhopper_v3.so` (MLKit バーコード) の重複。原因は `BarcodeScanning.Native.Maui` が依存先 `Xamarin.Google.MLKit.BarcodeScanning` と同一のネイティブライブラリを自身の .aar に再同梱していること (最新 3.1.0 でも未解消、アプリ側で修正不能) | **ユーザー承認済み (2026-08-05)**: NoWarn に XA4301 を理由コメント付きで追加。同一ファイルの重複でありビルドは2個目以降を無視するため実害なし |

## 2. 実施方針

1. **修正対象は `Template/Template.MobileApp` のみ**。サブモジュール (Smart.Maui / MauiComponents) は変更しない。サブモジュール側の修正が必要になった場合は作業を止めて報告する
2. **検証の二本立て**:
   - 本環境: `dotnet build Template.MobileApp/Template.MobileApp.csproj -f net10.0-android -c Debug` が**警告ゼロ**で成功すること (全項目共通のゲート)
   - 実行時検証が必要な項目: §5 の実機検証チェックリストへ転記し、修正コード自体は静的レビュー (該当画面の呼び出し経路の机上確認) で担保
3. **コミット単位**: Phase 単位を基本とし、以下は例外
   - Phase 1 の各項目は独立性が高いため 1項目=1コミット
   - 契約変更を伴う項目 (2-3, 2-5, 2-8, 4-1) は波及修正込みで 1項目=1コミット
   - Phase 8-2 (Style キーのリネーム) は Fix_Plan の推奨どおり 1ファイル=1コミット
4. **AGENTS.md 準拠**: `_` プレフィックス禁止 / 警告ゼロ維持 / **新規の警告抑止が必要になったら作業を止めて相談** (Phase 9-2 で既存抑止を削減する際も同様)
5. 各項目の着手時に Fix_Plan 記載の行番号を実コードで再確認する (レビュー時点からのズレがあり得るため。既に H-2 / H-4 / M-25 / NoWarn は実コードと一致することを確認済み)

## 3. 実施順序

Fix_Plan の依存関係 (「実施順序とマイルストーン」) をそのまま採用し、以下のバッチで直列に実施する。

| バッチ | 内容 | 項目数 | 依存 / 注意 |
|---|---|---|---|
| **Phase 0** | 環境整備: サブモジュール初期化 (済) + CommunityToolkit.Maui 14.2.2 化 (済・未コミット) + BunnyTail 1.11.0 化 + CA1849/XA4301 解消 → **警告ゼロのベースライン確立** | 4 | 最初にコミット。以降の全作業の前提 |
| **Phase 1** | 確定バグ 11項目 (1-1〜1-11) | 11 | 相互独立。1-4 (SKBitmap 所有権) のみ規模M |
| **Phase 2** | Components 11項目 (2-1〜2-11) | 11 | 2-1→2-2 は同一ファイルのため連続実施。2-3→2-4 も同様 (Nfc)。2-6 の権限フローは NoiseMonitor / OCR / カメラへ横展開 |
| **Phase 3** | 起動・権限・Manifest (3-1〜3-3) | 3 | **3-2 は 2-6 完了後** (起動時一括権限要求を消す前に画面側 Check→Request が必要) |
| **Phase 4** | セキュリティ (4-1〜4-4) | 4 | 4-4 (NU1903) は `dotnet list package --vulnerable --include-transitive` の実測から着手 |
| **Phase 5** | データ・通信 (5-1〜5-7) | 7 | 5-4 (NetworkOperator) は 7-2 Step 1 (エラー分類の切り出し) と**同時実施** |
| **Phase 6** | VM 例外・ライフサイクル (6-1〜6-5) | 5 | 独立。「予期できる失敗のみ対処」の方針厳守 |
| **Phase 7** | 設計リファクタ (7-1〜7-3) | 3 | 7-2 Step 1 は Phase 5 で実施済みの前提。ここでは Step 2 (INetworkInteraction 抽出) と 7-1 / 7-3 |
| **Phase 8** | UI/XAML (8-1〜8-6) | 6 | XamlStyler 導入後に着手。8-4 / 8-5 は削除前に参照ゼロを grep で再確認 |
| **Phase 9** | クリーンアップ・ドキュメント (9-1〜9-3) | 3 | 9-2 (`#pragma` 棚卸し) は Phase 2 の成果で抑止が不要化した箇所から。**抑止を残す判断はすべて理由コメント付与 + 報告に集約** |

マイルストーン: A=Phase 0〜3 完了 (安定化) / B=Phase 4 (セキュリティ) / C=Phase 5〜7 (堅牢化) / D=Phase 8〜9 (品質)。

## 4. 本環境向けの実装上の具体化

Fix_Plan の記載に対し、本環境で実施する際の補足のみ列挙する。

- **1-7 (UIDock Parameter)**: 修正値は既存の `Parameter` 値一覧 (`UIDockViewModel.cs` 内の他ボタン定義と対応する Execute 側の分岐) を確認して決定し、コミットメッセージに根拠を記載
- **2-3 / 2-5 / 2-8 / 4-1 (契約変更系)**: インターフェース変更 → ビルドエラーで波及先を全量洗い出し → 呼び出し側修正、の順で機械的に実施。ビルドエラーが出ない=波及なし、を確認してからコミット
- **2-6 (権限フロー)**: `Permissions.cs` へのカスタム権限追加後、`BluetoothSerial` の Check→Request パターン (`BluetoothSerial.android.cs:26-34`) を横展開。3-2 と合わせて「起動時要求ゼロ + 画面別要求」の一貫性を静的に確認
- **3-3 (Manifest)**: `aapt dump badging` は build-tools が存在するため本環境で実行可能 (`C:\Program Files (x86)\Android\android-sdk\build-tools`)。ビルド後の APK に対して権限一覧を確認する
- **4-4 (NU1903/NU1608)**: 復元が通るようになった時点で `dotnet list package --vulnerable --include-transitive` を実行し、原因パッケージと更新可否を報告してから修正。なお今回 NU1605 (CommunityToolkit) は Phase 0 で解消済み
- **5-2 (SqlHelper)**: 検証用の一時コードは scratchpad 側に作成し、プロジェクトには含めない
- **8-2 / 8-4 / 8-5 (Style / リソース削除)**: 削除・リネーム前に `grep` で全 XAML の参照を確認し、結果 (参照件数) をコミットメッセージに記録。StaticResource 解決エラーは起動時クラッシュになるため、実行確認できない本環境では特に慎重に行う

## 5. 実機検証チェックリスト (デバイス必要項目)

本環境では実行できないため、修正完了後に実機 (Android 14+ 推奨 + 可能なら Android 11) で以下を確認する。ビルドと静的確認は本環境で完了させる前提。

| # | 確認内容 | 関連項目 |
|---|---|---|
| 1 | Device > Bluetooth: 探索がクラッシュしない (Android 14+)、相手なしで30秒タイムアウト復帰、連続実行で例外なし | 2-1, 2-2 |
| 2 | Device > NFC: タグ複数回読取→画面離脱→再入場で正常。タグを途中で離しても継続 | 2-3, 2-4 |
| 3 | Device > Noise: Stop→即Start の高速繰り返しで異常なし | 2-5 |
| 4 | 権限「許可しない」状態で Device 各画面 (Activity/Noise/OCR) に入ってもクラッシュせずメッセージ表示 | 2-6, 3-2 |
| 5 | エミュレータ等の非搭載環境で Device > NFC / Bluetooth / Sensor 画面が開ける | 2-8, 6-1 |
| 6 | 初回起動で権限ダイアログが出ない。各画面初回利用時に該当権限のみ要求 | 3-2 |
| 7 | 起動→メニュー表示→Data 画面が即操作可能 (RebuildAsync 待機の確認) | 3-1 |
| 8 | 設定画面で AIServiceKey 保存→再起動→保持。SharedPreferences XML に平文が残っていない (`adb shell run-as`) | 4-1 |
| 9 | Network > Download/Upload: 30秒超の転送が完走、進捗表示、サーバ停止時のリトライ上限打ち切り | 1-8, 5-3, 5-4 |
| 10 | CV サンプル: キャプチャ→検出の繰り返しで描画異常・ObjectDisposedException・メモリ増加なし | 1-4, 2-7 |
| 11 | 端末TZ変更しながら DB 保存→表示 / API 取得→表示で時刻がずれない | 5-1 |
| 12 | F1〜F4・シェル表示が主要画面で従来どおり (7-1 後の回帰確認) | 7-1 |
| 13 | UI 系画面 (Mail/Profile/Character/Stream/Calendar) の見た目・スクロール回帰確認 | 8-2, 8-3, 8-5 |
| 14 | 診断パネル (DEBUG) の CPU% が減衰しない。画面退避後に CPU 使用率が下がる | 1-6, 6-5 |

## 6. ユーザー判断事項・報告事項

- **判断保留 4項目 (Fix_Plan §0-2: M-34 ダークモード / L-6 ローカライズ / §7 iOS / M-16 マイグレーション)**: 本計画では**対応しない**前提で進める。着手希望があれば別計画
- **9-1 の選択肢**: `State/Session.cs` / `Domain/Length.cs` は「削除」を第一候補とするが、テンプレート見本として残す判断もあり得るため、Phase 9 着手時に確認する (回答がなければ削除で進め、コミットを分けて戻せるようにする)
- **8-6 (絵文字置換)**: デザイン意図のある装飾 (`🐰` 等) は残置し、ナビゲーション系記号 (`◀▶` 等) のみ置換する方針で進める
- **警告抑止**: 新規追加が必要になった場合はその場で作業を止めて相談 (AGENTS.md)
- **Phase 0 の csproj 変更 (CommunityToolkit.Maui 14.2.0→14.2.2)**: サブモジュール側 (14.2.2) に合わせた。逆にサブモジュールの gitlink を古いコミットに戻す選択肢もあるが、gitlink は親リポジトリでコミット済みの状態を正とした

## 7. 完了条件 (Fix_Plan 共通条件の本環境版)

1. `dotnet build -f net10.0-android -c Debug` が**警告ゼロ**で成功 (各コミット時点)
2. 新規の `#pragma warning disable` / `GlobalSuppressions` 追加なし
3. §5 チェックリストを納品物として整備 (実機確認は本環境外)
4. XAML 変更は XamlStyler フォーマット適用済み
5. 各 Phase 完了時に変更サマリを報告

---

# 実施結果 (2026-08-06 完了)

全 Phase (0〜9) を実施済み。**コミットはフェーズ単位に1つへ統合** (計11コミット: ドキュメント追加 + Phase 0〜9)。各フェーズ時点および最終状態で `dotnet build -f net10.0-android -c Debug` の**警告ゼロ**を確認。新規の警告抑止追加なし (XA4301 のみユーザー承認済み)。改行コードは作業対象ファイルのワーキングツリーを CRLF に統一 (`.gitattributes` の `text=auto` によりリポジトリ格納は LF 正規化)。実装後に Fix_Plan 全項目の反映を機械検証し 102項目 OK / 0 NG。プッシュは未実施。

## 計画からの逸脱・判断事項

| 項目 | 内容 |
|---|---|
| 1-4 | 同一の SKBitmap バグが Fix_Plan 記載の3画面に加え CvNetFace/Ocr/People と ViewDrawing にも存在したため計8VMへ拡大適用。共通拡張 `SKBitmapImageSource.ReplaceBitmap` を導入 |
| 2-3 | タグ解放は「Detected イベントハンドラ内でのみ有効」の同期利用契約を採用 (現行 Rx パイプラインは Select 内同期消費のため適合)。`INfc : IDisposable` 案は不採用 |
| 3-1 | `App.OnStart` 全体の try/catch は追加せず。汎用 catch は CA1031 の新規抑止を要するため、既存のグローバルクラッシュハンドリング (CrashReport) による fail-fast を採用 |
| 4-4 | NU1608 は Phase 0 の CommunityToolkit.Maui 14.2.2 整合で既に解消していた。NU1903 は SQLitePCLRaw.bundle_e_sqlite3 2.1.12 の直接参照ピンで解消 (Microsoft.Data.Sqlite は最新 10.0.10 でも脆弱版参照のため) |
| 7-3(4) | ScheduleService/HolidayService は static 化ではなく DI 注入案を採用 (テンプレートの DI 見本として)。CA1822 は理由コメント付きで残置 |
| 8-3 | UIProfileView のみ対応 (FlexLayout+BindableLayout 化)。UICharacterView は既存の固定高回避あり・UIStreamView は任意項目のため見送り |
| 8-4 | `LeftSelectButton`/`CenterSelectButton`/`RightSelectButton` は BasicStyleView.xaml から参照が残っており削除せず (Fix_Plan の「参照0」前提と実コードが相違)。参照ゼロの NoErrorColor/GroupSpan/ItemCollectionGrid のみ削除 |
| 8-5 | **ユーザー指示により取り消し**: 旧 CalendarView は削除せず維持 (CalendarView/CalendarView2 併存の現状を保持) |
| 8-6 | 矢印記号 (◀️▶️等) はシェルのファンクションボタン等「通常フォントのテキスト」であり、アイコンフォント (PUA文字) への置換にはボタン側のフォント変更を伴うため残置 (任意項目) |
| 9-1 | **ユーザー指示により削除を取り消し**: ParameterBuilder (URLエンコードを `Uri.EscapeDataString` で修正の上維持)・Session (DI登録含む)・Domain/Length を維持。ReactiveSignalR の async void 解放は fire-and-forget Task + try/finally に変更 (Rx 購読解除は同期契約のため IAsyncDisposable 化は不可)。`RepositoryData.Empty` は null フォールバックとして実利用に変更 |
| XamlStyler | CLI (`xstyler`) は導入せず、既存フォーマットを保つ最小差分の編集で対応 (対象ファイルは XamlStyler 済みのため整形は維持されている) |

## 残課題 (別途対応)

- §5 実機検証チェックリスト (14項目) の実施
- 保留項目 (Fix_Plan §0-2): M-34 ダークモード / L-6 ローカライズ / §7 iOS / M-16 マイグレーション
- 上流報告 (任意): BarcodeScanning.Native.Maui の libbarhopper_v3.so 二重同梱 (XA4301)
