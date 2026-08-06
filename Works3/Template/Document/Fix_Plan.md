# Template (Template.MobileApp) 修正プラン

- **対象リポジトリ**: `Works3/Template` (プロジェクト: `Template.MobileApp`)
- **前提レビュー**: [Code_Review.md](Code_Review.md) (2026-08-05) の指摘番号 (H-*/M-*/L-*) を引き継ぐ
- **実施環境**: 本プランは別環境での修正実施を前提とした自己完結ドキュメント
- **ターゲット**: .NET 10 / `net10.0-android` (minSdk 30)。ビルド確認は `dotnet build Template.MobileApp/Template.MobileApp.csproj -f net10.0-android -c Debug`

---

## 0. 対象外項目 (指示による除外)

| レビュー番号 | 内容 | 備考 |
|---|---|---|
| M-21 | `OnNotifyFunction1() => OnNotifyBackAsync();` の116ファイル重複解消 | 対応不要 |
| M-31 | SemanticProperties / AutomationId の付与 | 対応不要 |
| L-8 | gRPC / SignalR / Ollama の実装または参照削除 | 未実装のまま維持 |
| H-5 | QRコードからの通信先/APIキー無検証受け入れ (付随する `UriFormatException` 時の不正値残存含む) | 対応不要 |

## 0-2. 本プランでは判断保留とする項目 (着手前に方針決定が必要)

| レビュー番号 | 内容 | 保留理由 |
|---|---|---|
| M-34 | ダークモード対応 (White直書き100+箇所の置換) | Light固定は意図的設計。対応するなら独立プロジェクト規模 |
| L-6 | ローカライズ整備 (日本語リテラル168箇所のresx化) | サンプルUIの性質上、全量resx化の要否は要判断 |
| §7 | iOS対応 (Components の iOS 実装追加) | Android専用テンプレートとして維持するか要判断。維持する場合は README への明記のみ推奨 |
| M-16 | DBマイグレーション機構の導入 | 「毎起動Rebuild」はサンプル仕様。テンプレ利用手順書への注意記載で代替可 (Phase 9 参照) |

---

## 修正時の共通ルール

1. **AGENTS.md 準拠**: メンバ変数に `_` プレフィックス禁止 / **ビルド警告ゼロを維持** / 警告抑止 (`#pragma`/`SuppressMessage`) が必要になった場合は追加前に相談
2. `Directory.Build.props` により `AnalysisMode=All` + StyleCop + `WarningsAsErrors=nullable`。修正がアナライザ警告を新規に発生させないこと
3. XAML変更時は XamlStyler 設定 (`Settings.XamlStyler`) に従う
4. ソリューションに自動テストは存在しない。検証は各項目の「検証」欄のとおりビルド+実機/エミュレータでの手動確認
5. コミットは Phase 単位を推奨。Phase 内の項目は原則独立しており並行作業可 (依存があるものは明記)

---

# Phase 1: 確定バグ修正 (小規模・独立・低リスク)

## 1-1. TextAlignment→Gravity 変換の左右反転 [H-2] — 規模S

- **対象**: `Behaviors/Extensions.android.cs:12-13`
- **現状**: `Start => GravityFlags.Right` / `End => GravityFlags.Left` と反転している
- **修正**:
  ```csharp
  Microsoft.Maui.TextAlignment.Start => GravityFlags.Start,
  Microsoft.Maui.TextAlignment.End => GravityFlags.End,
  ```
  RTL対応の観点から `Left/Right` ではなく `Start/End` を採用する
- **影響範囲**: `ButtonOption.android.cs` の `EnableTextAlignment` 利用画面
- **検証**: `HorizontalTextAlignment="Start"` のButtonで文字が左寄せ(LTR時)になること

## 1-2. DataService の同期 `Using` + async デリゲート [H-4] — 規模S

- **対象**: `Services/DataService.cs:130-135` (`QueryWorkListAsync` / `QueryWorkAsync` の2メソッドのみ)
- **現状**: 同期版 `provider.Using(...)` に `ValueTask` を返すデリゲートを渡しており、接続が await 前に Dispose される
- **修正**: 他メソッド (:59,80,87 等) と同じ `provider.UsingAsync(...)` に変更
- **検証**: Navigation > Edit 画面 (`EditListViewModel.cs:32` が実利用パス) で一覧表示・詳細取得が動作すること

## 1-3. Style の TargetType 不一致 [H-11] — 規模S

- **対象**: `Modules/View/ViewCollectionView.xaml:26-30` (定義) / `:133` (適用)
- **現状**: `x:Key="AddressHeaderGrid"` が `TargetType="Label"` のまま Grid に適用されている
- **修正**: `TargetType="Grid"` に変更。`Padding`/`BackgroundColor` の Setter はそのまま Grid で有効
- **検証**: View > Collection 画面のヘッダ行に Padding=8 と背景色が効くこと

## 1-4. Dispose済み SKBitmap のバインドと解放漏れの統一 [H-3] — 規模M

- **対象**:
  - `Modules/Sample/SampleCvLocalViewModel.cs:73-74` (`using var bitmap` → `Image.Bitmap = bitmap`)
  - `Modules/Sample/SampleCvNetObjectViewModel.cs:74-75` (同)
  - `Modules/Sample/SampleCvNetTagViewModel.cs:71-72` (同)
  - `Modules/UI/UITreeMapViewModel.cs:80,89` (逆に `using` なしで解放漏れ)
- **修正方針** (所有権ルールを一本化):
  1. バインド対象に渡すビットマップは `using` にしない。VM がフィールドで保持
  2. 差し替え時に旧ビットマップを Dispose するヘルパ (またはプロパティセッター) を導入:
     ```csharp
     private SKBitmap? currentBitmap;

     private void SetBitmap(SKBitmap bitmap)
     {
         var old = currentBitmap;
         currentBitmap = bitmap;
         Image.Bitmap = bitmap;
         old?.Dispose();
     }
     ```
  3. 画面破棄時の解放は `Disposables.Add(...)` か `Dispose(bool)` オーバーライドで実施 (`UITreeMapViewModel.cs:26` が `Disposables.Add(Drawing)` している方式に合わせる)
  4. `SampleCvLocalViewModel.cs:16-18` の `DetectDrawing` 等も `Disposables` 登録
- **検証**: CVサンプル各画面でキャプチャ→検出→再キャプチャを繰り返し、描画異常・ObjectDisposedException が発生しないこと

## 1-5. BusyOverlay 判定の恒真バグ [M-25] — 規模S

- **対象**: `Shell/ShellProperty.cs:199-210`
- **現状**: `bool` の BindableProperty に対し boxed 値の `is not null` 判定 → 常に true。`BusyOverlay="False"` でも Behavior 付与、値変更のたび remove→add
- **修正**: `(bool)oldValue` / `(bool)newValue` で判定:
  ```csharp
  if ((bool)oldValue) { /* Behavior除去 */ }
  if ((bool)newValue) { view.Behaviors.Add(new BusyOverlayBehavior()); }
  ```
- **検証**: BusyOverlay 未指定/False の画面で Behavior が付かないこと (デバッガで `view.Behaviors` 確認)

## 1-6. 診断パネルの CPU% 計算バグ + 監視制御 [M-26] — 規模S

- **対象**: `Shell/DiagnosticPanel.xaml.cs:152` (計算)、`:99-102` (`IsVisible` 変更時)、`:17,77` (解放)
- **修正**:
  1. 計測区間の実経過時間で除算するよう `stopwatch.Restart()` を計測ごとに実行 (現状は起動からの累積で除算し0に収束)
  2. `IsVisible=false` になったときは `StartMonitor()` ではなく停止処理を呼ぶ
  3. `FrameUpdated += OnDisplayFrameUpdated` (:77) の解除処理と `currentProcess` の Dispose を追加 (`IDisposable` 実装 or Unloaded イベント)
- **検証**: DEBUGビルドで診断パネルを表示し、CPU% が時間経過で不自然に減衰しないこと

## 1-7. UIDock のパラメータコピペ誤り [M-27] — 規模S

- **対象**: `Modules/UI/UIDockViewModel.cs:240-258`
- **現状**: CPU ボタン (:245) と Memory ボタン (:258) の `Parameter` が両方 `"VolumeDown"`
- **修正**: それぞれのボタン意図に対応する値へ修正 (周辺定義の命名規則に従い `"Cpu"` / `"Memory"` 相当へ。既存のParameter値一覧を確認して整合させる)
- **検証**: Dock 画面で該当ボタン押下時の挙動確認

## 1-8. ダウンロード進捗の NaN [M-14] — 規模S

- **対象**: `Services/HttpService.cs:63,81,100,119`
- **現状**: `Content-Length` 不明時に `(double)processed / total` が NaN となり進捗が一切更新されない
- **修正**: `total <= 0` の場合は進捗コールバックをスキップ (または `-1` を渡して不確定表示):
  ```csharp
  if (total > 0)
  {
      var percent = (int)((double)processed / total * 100);
      ...
  }
  ```
- **検証**: Network > Download で進捗表示。可能ならチャンク転送 (Content-Lengthなし) 応答でも例外なく完了すること

## 1-9. DateTimeConverter の例外情報破棄 [L-9] — 規模S

- **対象**: `Helpers/Json/DateTimeConverter.cs:20-23`
- **現状**: `catch { throw new FormatException(); }` で元例外・メッセージを破棄
- **修正**: `catch (Exception ex) { throw new FormatException($"Invalid datetime format. value=[{value}]", ex); }` の形で InnerException と対象値を保持 (値のログ出力が情報漏えいにならないかは用途上問題なし)
- **検証**: ビルドのみ (単純変更)

## 1-10. コンバーター規約の統一 [L-13の一部] — 規模S

- **対象**: `Converters/MailDateTimeStringConverter.cs:3,28`
- **修正**: `public sealed class` 化、`ConvertBack` を他18クラスと同じ `NotSupportedException` に統一
- **検証**: ビルドのみ

## 1-11. ファイル名とクラス名の不一致 [L-1] — 規模S

- **対象**: `Modules/Basic/BasicLocalViewModel.cs` (クラスは `BasicLocaleViewModel`)
- **修正**: ファイル名を `BasicLocaleViewModel.cs` にリネーム (git mv)
- **検証**: ビルドのみ

---

# Phase 2: Components のリソースリーク・スレッド安全性・権限 (最重要領域)

> この Phase は `Components/` 配下に閉じており、Phase 1/3/4 と並行作業可。

## 2-1. RegisterReceiver の Exported フラグ [H-1] — 規模S

- **対象**: `Components/BluetoothSerial.android.cs:84,110`
- **現状**: フラグ未指定の動的 Receiver 登録。targetSdk 34+ (net10.0-android の既定) では `SecurityException` で即クラッシュ
- **修正**: AndroidX の互換APIを使用 (minSdk 30 のため直接フラグ指定はAPI 33分岐が必要になる。ContextCompat なら分岐不要):
  ```csharp
  AndroidX.Core.Content.ContextCompat.RegisterReceiver(
      context, receiver, filter, ContextCompat.ReceiverNotExported);
  ```
  受信対象はシステムブロードキャスト (`ActionFound`/`ActionDiscoveryFinished`/`ActionBondStateChanged`) のため `NotExported` で問題ない
- **検証**: Android 14+ 実機/エミュレータで Device > Bluetooth の探索がクラッシュしないこと

## 2-2. BluetoothSerial: Receiver解除の例外安全化 + タイムアウト復活 [M-6] — 規模M

- **対象**: `Components/BluetoothSerial.android.cs:75-128` (`DiscoverAsync` / `BondAsync`)
- **現状**:
  - `RegisterReceiver`→`await tcs.Task`→`UnregisterReceiver` が try/finally なし。途中例外で Receiver リーク (`IntentReceiverLeaked`)
  - `await tcs.Task` にタイムアウトなし (実装は :112-114,124 にコメントアウトで放置)。`ActionDiscoveryFinished` が来なければ永久待ち
- **修正**:
  ```csharp
  context.RegisterReceiver(...);  // 2-1 の ContextCompat 版
  try
  {
      if (!adapter.StartDiscovery()) return null;
      using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)); // 妥当値は要調整
      var device = await tcs.Task.WaitAsync(cts.Token).ConfigureAwait(false);
      return device;
  }
  catch (OperationCanceledException)
  {
      return null; // 既存の失敗時戻り値の流儀 (null返却) に合わせる
  }
  finally
  {
      adapter.CancelDiscovery();
      context.UnregisterReceiver(receiver);
  }
  ```
  `BondAsync` (:110-125) も同一構造で修正。コメントアウトされたタイムアウトコードは削除
- **付随修正**: `Dispose() => socket.Close();` (:231-234) を `socket.Close(); socket.Dispose();` に (Javaラッパー解放)
- **検証**: ペアリング相手なし環境で探索→30秒でタイムアウト復帰。連続実行で `Receiver not registered` 例外が出ないこと

## 2-3. NfcReader の解放処理 [H-7] — 規模M

- **対象**: `Components/Nfc.android.cs` 全体、`Components/Nfc.cs` (契約)、`Components/NfcExtensions.cs`、利用側 `Modules/Device/DeviceNfcViewModel.cs`
- **現状**:
  1. DIシングルトンが `RegisterActivityLifecycleCallbacks(this)` したまま `Dispose(bool)` オーバーライドなし → Activity 参照リーク
  2. `AndroidNfcF` (:10-44) が `Connect()` するのに `Close()`/`Dispose()` を呼ばない → タグ検出ごとに接続リーク
- **修正**:
  1. `ActivityRecognizer.android.cs:19-32` と同じ様式で `Dispose(bool disposing)` をオーバーライドし、`UnregisterActivityLifecycleCallbacks` + `DisableReaderMode` + `currentActivity = null` を実施。`INfcReader` に `IDisposable` を付与 (`INoiseMonitor`/`IBluetoothSerial` と同じ様式に揃える)
  2. タグ解放: `OnTagDiscovered` (:110-113) で `Detected` イベント呼び出し後に `nfc.Close()` + `nfc.Dispose()` を finally で実行する。イベント購読側が非同期でタグを使う設計のため、**同期利用契約にするか、`INfc` を `IDisposable` 化して受け側の `DeviceNfcViewModel` で `using` するかは実装時に選択** (推奨: `INfc : IDisposable` とし、イベント引数で所有権を渡して受け側で `using`)
- **依存**: `INfc` の契約変更は `NfcExtensions.cs` / `Domain/Logic/SuicaLogic.cs` 利用箇所へ波及。ビルドエラーで洗い出し可能
- **検証**: Device > NFC でタグを複数回読み取り後、`adb shell dumpsys meminfo` 等で Activity リークがないこと。画面離脱→再入場で正常動作

## 2-4. NFC 例外でRxシーケンスが死ぬ問題 [M-5関連] — 規模S

- **対象**: `Components/Nfc.android.cs:34-43,119-122`、`Modules/Device/DeviceNfcViewModel.cs:21`
- **現状**: `Connect()` の一般 `IOException` が Rx の `Select` 内に伝播すると OnError でシーケンス終了 → 以後NFC無反応
- **修正**: `OnTagDiscovered` 内で例外を捕捉して WARN ログ (+ 必要なら失敗イベント) にとどめ、`Detected` イベントに例外を漏らさない。`TagLostException` 以外の `IOException` も同様に処理
- **検証**: タグを途中で離す操作を繰り返しても読み取りが継続すること

## 2-5. NoiseMonitor の Stop/Start 競合と例外消失 [H-8, M-8前半] — 規模M

- **対象**: `Components/NoiseMonitor.cs:40-91`、`Components/NoiseMonitor.android.cs:22-37`
- **現状**:
  1. `Stop()` が CTS を Cancel/Dispose するだけでループ完了を待たない → 直後の `Start()` で前ループの `CleanupMeasure()` が新バッファを `ArrayPool.Return` (二重返却=プール破壊)
  2. Cancel後のDispose済みCTSトークンを `WaitForNextTickAsync` が参照する窓 (`ObjectDisposedException`)
  3. `SetupMeasure()` (:63) が try の外 → `AudioRecord` 生成失敗 (権限拒否等) の例外が fire-and-forget タスク内に消え、`IsRunning=true` の不整合
- **修正**:
  ```csharp
  private Task? loopTask;

  public void Start(TimeSpan interval)
  {
      if (loopTask is not null) return;
      cts = new CancellationTokenSource();
      loopTask = Loop(interval, cts.Token);
  }

  public async ValueTask StopAsync()   // 契約を非同期化 (INoiseMonitor 変更)
  {
      if (loopTask is null) return;
      cts!.Cancel();
      try { await loopTask.ConfigureAwait(false); }
      catch (OperationCanceledException) { }
      cts.Dispose();
      cts = null;
      loopTask = null;
  }
  ```
  - `Loop` 内: `SetupMeasure()` を try 内へ移動し、例外時は `CleanupMeasure()` 実行+ログ出力+`IsRunning` を確実に false へ
  - `IsRunning` は `loopTask` の有無から導出するか `volatile` 化
- **依存**: `INoiseMonitor.Stop()` → `StopAsync()` への契約変更は利用側 (Device > Noise 系画面) へ波及。同期契約を維持したい場合は `Stop()` 内で `loopTask` 完了を待つ設計でも可 (ただしUIスレッドブロック回避のため待機はループ側フラグで行う)
- **検証**: Stop→即Start を高速に繰り返し、`ArrayPool` 破壊由来の異常 (別機能でのバッファ内容化け) やクラッシュがないこと

## 2-6. ActivityRecognizer の権限フロー [H-9, M-20] — 規模M

- **対象**: `Components/ActivityRecognizer.android.cs:39-44`、`Permissions.cs`、利用側VM
- **現状**: `ActivityCompat.RequestPermissions` 直後に同期で `RegisterListener`。結果受け口 (`OnRequestPermissionsResult`) なし、`CheckSelfPermission` 事前判定なし、毎回ダイアログ要求
- **修正方針**: 権限要求を Component から出し、**MAUI の Permissions API に統一**する (プロジェクト内の手本は `BluetoothSerial.android.cs:26-34` の Check→Request フロー):
  1. `Permissions.cs` にカスタム権限を追加:
     ```csharp
     public sealed class ActivityRecognitionPermission : Permissions.BasePlatformPermission
     {
     #if ANDROID
         public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
             [(global::Android.Manifest.Permission.ActivityRecognition, true)];
     #endif
     }
     ```
  2. `IActivityRecognizer.Start()` を「権限がある前提」の純粋な開始処理にし、権限の Check→Request は呼び出し側 (Device > Activity 画面のVM) で実施。拒否時はダイアログ表示して開始しない
  3. `ActivityRecognizer.android.cs` から `ActivityCompat.RequestPermissions` 呼び出しを削除
- **同時対応** (同じパターンを横展開):
  - `NoiseMonitor` 利用画面: `Permissions.Microphone` の Check→Request を Start 前に実施 (`NoiseMonitor.android.cs:22` は権限なしで `new AudioRecord` している)
  - OCR/カメラ利用画面: `Permissions.Camera` の Check→Request (現状は起動時一括要求に暗黙依存)
- **依存**: Phase 3-2 (起動時一括要求の削減) とセット。先にこちらを入れると起動時要求への依存が消える
- **検証**: 権限を「許可しない」に設定した状態で各画面に入り、クラッシュせず適切なメッセージが出ること。許可後は正常動作

## 2-7. CognitiveUsecase の初期化競合と ArrayPool 例外安全 [M-8後半] — 規模S

- **対象**: `Usecase/CognitiveUsecase.cs:41-89`
- **修正**:
  1. `PrepareSessionAsync` を `SemaphoreSlim(1,1)` で排他し、二重初期化 (InferenceSession リーク) を防止
  2. `ArrayPool<float>.Shared.Rent` (:68) 〜 `Return` (:75) を try/finally 化。あわせて `Return` の位置を「`session.Run` の結果 (`values`) を読み終えた後」に移動 (現状 :75 で返却後 :77-79 で `values` を読んでおり、`values` が入力バッファを参照する実装だと危険)
  3. `bitmap.Resize(...)` (:93) の戻り値を `using` で解放
- **検証**: CVサンプルで連続推論してもメモリ増加が定常に収まること

## 2-8. 非搭載端末での NRE 防御 [L-12] — 規模M

- **対象**:
  - `Components/BluetoothSerial.android.cs:20-21` — コンストラクタで `bluetoothManager.Adapter!` → **DI解決の瞬間にクラッシュ**
  - `Components/Nfc.android.cs:57-58` — `GetSystemService(...)!` / `DefaultAdapter!`
  - `Platforms/Android/AndroidHelper.cs:9-10` — `GetExternalFilesDir(...)!.Path`
- **修正方針**: 「非搭載＝機能無効」を契約に追加:
  1. `IBluetoothSerialFactory` / `INfcReader` に `bool IsSupported { get; }` を追加。コンストラクタでは null 許容で保持し、`Start`/`ConnectAsync` 時に未サポートなら false/失敗を返す (throw しない)
  2. 利用側VMは `IsSupported` を見てボタン無効化またはメッセージ表示
  3. `AndroidHelper.GetExternalFilesDir` は null 時に内部ストレージへフォールバック
- **検証**: エミュレータ (NFC非搭載) で Device > NFC / Bluetooth 画面に入ってもクラッシュしないこと

## 2-9. OCR の失敗情報とキャンセル [M-5] — 規模S

- **対象**: `Components/OcrReader.android.cs:32,46-49`、`Components/OcrReader.cs:5`
- **修正**:
  1. `OnFailure` で `ILogger` に WARN 出力 (例外内容を保持)。戻り値は null のままでよいが「失敗」と「文字なし」を区別したい場合は戻り値を `OcrResult?`+成否フラグ化 (任意)
  2. `ReadTextAsync(CancellationToken cancellationToken = default)` を追加し、`await listener.Task.WaitAsync(cancellationToken)` で永久待ちを解消
- **検証**: Device > OCR で読み取り失敗時にログが出ること

## 2-10. Behaviors の細部 [L-11] — 規模S

- **対象と修正**:
  1. `Behaviors/EntryOption.android.cs:95-105` — `EditorAction` ハンドラを `-=` してから `+=` (Mapper再実行での多重購読防止)
  2. `Behaviors/Focus.cs:102-108` — `OnDetachingFrom` で `border.AbortAnimation(AnimationName)` を追加
  3. `Behaviors/Focus.cs:141` — 16msごとの `new SolidColorBrush` をアニメーション開始時に生成した単一インスタンスの `Color` 更新に変更 (SolidColorBrush.Color は bindable)
  4. `Behaviors/CameraBind.cs:178-184` — `CaptureAsync` を try/finally 化し `MediaCaptured`/`MediaCaptureFailed` の解除漏れを解消。`result.Task.WaitAsync(token)` でキャンセル対応
- **検証**: Entry 多数配置画面でEnterキー動作が1回のみ発火すること。カメラキャプチャの中断で以後の操作が正常なこと

## 2-11. コンポーネントのイベントスレッド契約の明文化 [M-29] — 規模S (コード変更なし)

- **対象**: `Components/` 各インターフェース (`INfcReader.Detected`、`INoiseMonitor.Measured`、`IActivityRecognizer` 等)
- **現状**: イベントは非UIスレッドから発火し、VM側の `ObserveOnCurrentContext()` に暗黙依存
- **修正**: 各イベントの XML doc コメントに「UIスレッド以外から発火する。UI更新時は ObserveOnCurrentContext 等でマーシャリングすること」を明記。発火側でのマーシャリングは行わない (既存VMの購読パターンを正とする)
- **検証**: ビルドのみ

---

# Phase 3: 起動シーケンス・権限要求・Manifest

## 3-1. 起動初期化の競合解消 (async void 排除) [H-10] — 規模M

- **対象**: `ApplicationInitializer.cs`、`App.xaml.cs:31-44`、`MauiProgram.cs:332`
- **現状**: `IMauiInitializeService.Initialize` (同期契約) 内で `async void` により `RebuildAsync()` を投げっぱなし。`App.OnStart` の画面遷移と競合し、例外は捕捉不能
- **修正方針** (推奨案):
  1. `ApplicationInitializer` から async 処理を分離し、同期部分 (ResolveProvider設定、Navigated購読、UniqueId設定、ApiContext設定) のみ `Initialize` に残す
  2. 非同期初期化 (`DataService.RebuildAsync`) は `Task` を公開する形にする:
     ```csharp
     public sealed class ApplicationInitializer : IMauiInitializeService
     {
         public Task StartupTask { get; private set; } = Task.CompletedTask;

         public void Initialize(IServiceProvider services)
         {
             // 同期初期化...
             StartupTask = InitializeAsync(services);   // 開始だけして Task を保持
         }

         private async Task InitializeAsync(IServiceProvider services) { ... }
     }
     ```
  3. `App.OnStart` で `ForwardAsync(ViewId.Menu)` の前に `await initializer.StartupTask` (DI経由で `ApplicationInitializer` を取得。`MauiProgram.cs:332` の登録を「実クラス + IMauiInitializeService へのバインド」に変更)
  4. `App.OnStart` 全体を try/catch し、初期化失敗時は `CrashReport` へのログ + エラーダイアログ表示で fail-fast (Development.md の例外設計方針に準拠)
- **注意**: `App.OnStart` 自体が `async void` なのは MAUI のイベントオーバーライドの制約上許容 (ReSharperコメントの現状維持)。内部を全て try/catch で覆うことが本質
- **検証**: 起動→メニュー表示→DB系画面 (Data) が即操作できること。`RebuildAsync` に一時的に `Task.Delay(3000)` を仕込み、画面遷移がそれを待つこと (仕込みは検証後削除)

## 3-2. 起動時の権限一括要求の削減 [M-19] — 規模S

- **対象**: `App.xaml.cs:37-39`、`Permissions.cs:17-21`
- **現状**: 起動時にカメラ・マイク・`LocationAlways` (バックグラウンド位置) を無条件連続要求
- **修正**:
  1. `App.OnStart` から3つの権限要求を削除 (Phase 2-6 で利用画面側の Check→Request に置き換え済みであること)
  2. `Permissions.cs` の `RequestLocationAsync` を `Permissions.LocationWhenInUse` ベースに変更 (バックグラウンド測位機能は存在しないため)。`LocationAlways` を要求するAPIは削除
- **依存**: **Phase 2-6 完了後に実施** (先に消すと権限チェックのない Component がクラッシュする)
- **検証**: 初回起動で権限ダイアログが出ないこと。各デバイス画面初回利用時に該当権限のみ要求されること

## 3-3. AndroidManifest の権限棚卸し [M-17, M-18, L-14] — 規模S

- **対象**: `Platforms/Android/AndroidManifest.xml:4-27`
- **削除する権限** (根拠はレビュー M-17):
  ```
  CHANGE_WIFI_STATE            … 使用実装なし (WiFiManagerは未実装)
  READ_EXTERNAL_STORAGE        … GetExternalFilesDir はアプリ専用領域で権限不要
  WRITE_EXTERNAL_STORAGE       … minSdk 30 の Scoped Storage 下で無効
  BATTERY_STATS                … signature|privileged 権限で通常アプリには付与されない
  FLASHLIGHT                   … 非推奨・未強制 (Torch は CAMERA で可)
  ACCESS_BACKGROUND_LOCATION   … バックグラウンド測位機能なし・審査高リスク
  USE_FINGERPRINT              … API 28 で非推奨、USE_BIOMETRIC と重複
  ```
  ※ `ACCESS_WIFI_STATE` は WiFi manager 実装予定 (README TODO) があるなら残置可、なければ削除。`USE_BIOMETRIC` は Biometric 画面実装予定に合わせて判断 (未実装のため削除推奨、実装時に再追加)
- **追加する宣言**:
  ```xml
  <!-- Android 11 (minSdk 30) 向けレガシーBluetooth -->
  <uses-permission android:name="android.permission.BLUETOOTH" android:maxSdkVersion="30" />
  <uses-permission android:name="android.permission.BLUETOOTH_ADMIN" android:maxSdkVersion="30" />
  ```
  - `BLUETOOTH_SCAN` に `android:usesPermissionFlags="neverForLocation"` を付与 (位置推定用途でないため)
  - uses-feature を追加 (非搭載端末のPlayストア除外を防ぐ):
    ```xml
    <uses-feature android:name="android.hardware.nfc" android:required="false" />
    <uses-feature android:name="android.hardware.camera" android:required="false" />
    <uses-feature android:name="android.hardware.bluetooth" android:required="false" />
    <uses-feature android:name="android.hardware.microphone" android:required="false" />
    ```
- **検証**: Android 11 実機 (あれば) で Bluetooth 探索が動作すること。`aapt dump badging` で権限一覧を確認

---

# Phase 4: セキュリティ・ビルド設定

## 4-1. AIServiceKey を SecureStorage へ [H-6] — 規模M

- **対象**: `State/Settings.cs:42-46`、参照箇所 (`Modules/Main/SettingViewModel.cs`、`SampleCvNetMenuViewModel.cs:11-22` 等)
- **現状**: APIキーが平文 `Preferences` (Android: SharedPreferences XML) に保存
- **修正方針**: `SecureStorage` は async API のため、プロパティ契約を変更する:
  1. `Settings` に `ValueTask<string?> GetAIServiceKeyAsync()` / `ValueTask SetAIServiceKeyAsync(string value)` を追加し、内部は `SecureStorage.Default.GetAsync/SetAsync`
  2. 同期プロパティ `AIServiceKey` は削除 (ビルドエラーで参照箇所を洗い出し、async 呼び出しへ書き換え)
  3. **マイグレーション**: 初回アクセス時に Preferences 側に旧値があれば SecureStorage へ移送して Preferences から削除
  4. 将来のキー類 (認証トークンを永続化する場合等) も SecureStorage を使う旨を Development.md に追記
- **検証**: 設定画面でキー保存→アプリ再起動→保持されていること。`adb shell run-as` で SharedPreferences XML に平文キーが残っていないこと

## 4-2. 証明書検証バイパスコードの削除 [L-4] — 規模S

- **対象**: `Services/AppHostBuilderExtensions.cs:24-26`
- **現状**: `RemoteCertificateValidationCallback = (…) => true` がコメントアウトで残置 (コメント解除だけで全証明書受け入れになる)
- **修正**: コメントごと削除。開発時に自己署名証明書が必要な場合の手順は Development.md に「`#if DEBUG` で限定し、コミットしないこと」として記載
- **検証**: ビルドのみ

## 4-3. 署名設定の外部化 [L-5] — 規模S

- **対象**: `Template.MobileApp.csproj:42-48`、`Template.MobileApp/example.keystore`
- **現状**: keystore ファイルとパスワード (`example`) がリポジトリにコミット。ダミーだが「パスワード直書き」パターンが実案件へコピーされる危険
- **修正**: `.EmbeddedProperty.props` と同じ方式で `..\.Signing.props` (gitignore対象) から `AndroidSigningKeyStore` 等を注入する形へ変更。csproj には `<Import Project="..\.Signing.props" Condition="Exists('..\.Signing.props')" />` と、未設定時にDebug署名で通るよう `AndroidKeyStore=False` の既定を記述。`example.keystore` は削除し、生成手順 (`keytool` コマンド) を Development.md に記載
- **検証**: Release ビルドが `.Signing.props` なしでも成功する (debug署名) こと

## 4-4. NU1903 (脆弱性警告) の解消 [M-30] — 規模S〜M

- **対象**: `Template.MobileApp.csproj:14-15` (`<NoWarn>$(NoWarn);NU1608;NU1903</NoWarn>` + `<!-- TODO -->`)
- **手順**:
  1. `dotnet list Template.MobileApp/Template.MobileApp.csproj package --vulnerable --include-transitive` で原因パッケージを特定
  2. 直接参照なら更新。推移的参照なら該当パッケージの上位を更新するか、安全なバージョンを直接参照でピン止め
  3. 解消後 `NU1903` を NoWarn から削除。`NU1608` (バージョン制約違反) も同様に原因 (おそらく MAUI 10 プレビュー期の依存不整合) を確認し、解消できるなら削除、できないなら理由をコメントで明記して TODO を解消
- **検証**: `dotnet restore` で NU1903/NU1608 警告が出ないこと (または残す場合は理由コメントあり)

---

# Phase 5: データ・通信層の堅牢化

## 5-1. DateTime の TZ 一貫性 [M-9] — 規模M

- **対象**: `Helpers/Data/DateTimeTypeHandler.cs:7-16`、`Helpers/Json/DateTimeConverter.cs:18,28`、書き込み元 (`Modules/Data/DataViewModel.cs:55` の `DateTime.Now` 等)
- **方針決定**: 「**アプリ内部は UTC、表示時のみローカル変換**」に統一する (DateTimeOffset 全面採用は変更範囲が大きいため見送り)
- **修正**:
  1. `DateTimeTypeHandler.SetValue`: `value.Kind == DateTimeKind.Local ? value.ToUniversalTime().Ticks : value.Ticks` で UTC 正規化して保存
  2. `DateTimeTypeHandler.Parse`: `new DateTime((long)value, DateTimeKind.Utc)` で Kind を復元
  3. `DateTimeConverter.Read`: `DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal)` で常に UTC Kind に
  4. `DateTimeConverter.Write`: `Kind=Unspecified` の値は UTC とみなす (`DateTime.SpecifyKind(value, DateTimeKind.Utc)` 経由) — 1〜3 実施後は Unspecified が流れてこない前提だが防御として
  5. 表示箇所 (`{Binding ...:yyyy/MM/dd HH:mm:ss}` 等) は `.ToLocalTime()` 変換を挟むか Converter で対応
- **検証**: 端末TZを変更しながら DB 保存→表示、API 取得→表示で時刻がずれないこと

## 5-2. SqlHelper の NOT NULL 判定修正 [M-10] — 規模S

- **対象**: `Helpers/Data/SqlHelper.cs:47-52`
- **現状**: コンパイラ内部生成の `System.Runtime.CompilerServices.NullableAttribute` をリフレクションで見ており、Nullable値型 (`int?` 等) が誤って NOT NULL になる。別アセンブリのエンティティでは常に NOT NULL
- **修正**: .NET 6+ の公式APIに置き換え:
  ```csharp
  private static readonly NullabilityInfoContext NullabilityContext = new();

  // 判定
  var nullability = NullabilityContext.Create(column.Property);
  var isNullable = nullability.WriteState == NullabilityState.Nullable
                || Nullable.GetUnderlyingType(column.Property.PropertyType) is not null;
  if (!isNullable || hasPrimaryKey)
  {
      sql.Append(" NOT NULL");
  }
  ```
- **検証**: `DateTime?`/`string?`/`int?` プロパティを持つテスト用エンティティで `MakeCreate<T>()` の出力SQLを確認 (一時的なユニット検証コードで良い。確認後削除)

## 5-3. HttpService への CancellationToken 導入と転送用タイムアウト分離 [M-11] — 規模M

- **対象**: `Services/HttpService.cs` 全メソッド、`Services/AppHostBuilderExtensions.cs`、`Services/ApiNames.cs`、呼び出し元 (`Usecase/NetworkUsecase.cs`)
- **修正**:
  1. 全公開メソッドに `CancellationToken cancellationToken = default` を追加し、Rester 呼び出しへ伝播
  2. 転送系 (Download/Upload 4メソッド) 用に名前付きクライアント `ApiNames.Transfer` を追加: `Timeout = Timeout.InfiniteTimeSpan`、キャンセルは呼び出し側の CancellationToken (+必要なら `CancellationTokenSource.CreateLinkedTokenSource` で全体上限) で制御
  3. 通常APIは現行 30 秒を維持
  4. `using var client = ...` は Factory 製クライアントに対して不要なため `var client = ...` へ統一 (誤解防止。Dispose しても実害はないが、テンプレートとして真似されやすいため)
- **検証**: 大きめファイルの Download/Upload が30秒を超えても完走すること。画面離脱でキャンセルされること (VM側でトークンを渡している場合)

## 5-4. NetworkOperator のリトライ上限と重複統合 [M-13, M-24の一部] — 規模M

- **対象**: `Usecase/NetworkOperator.cs:36-94` (`Execute<T>`)、`:100-163` (`Execute`)、`:169-210` (進捗付き版)
- **修正**:
  1. `while (true)` に最大リトライ回数 (例: 3回、const で定義) を導入。上限到達時は失敗を返す
  2. `Execute<T>` と `Execute` のほぼ同一ロジックを統合: `Execute` を `Execute<T>` ベースの実装に寄せる (`Execute(...) => ExecuteInternal<object?>(...)` の形、またはコア判定部分 (`RestResult`→メッセージ/継続判定) をプライベートメソッドへ抽出して両方から呼ぶ)
  3. 進捗付き版にも同じコア判定を適用し、リトライ有無の非対称を解消 (進捗付きはリトライなしを仕様とするならその旨コメント)
  4. エラー時に `IRestResponse` の StatusCode/InnerException を WARN ログ出力 (現状は情報ゼロで障害解析不能)
- **注意**: エラーメッセージ英語直書き (:44,60,70,89) の resx 化は L-6 (保留) と合わせて判断。本 Phase では触らない
- **検証**: サーバ停止状態で Network 系画面の操作 → リトライ確認ダイアログが上限回数で打ち切られること

## 5-5. ApiContext のスレッド安全化 [M-12の一部] — 規模S

- **対象**: `Services/ApiContext.cs`
- **現状**: `BaseAddress`/`Token` が排他なしの可変プロパティで、`ApiDelegatingHandler` (通信スレッド) から読まれる
- **修正**: 各プロパティのバッキングフィールドを `Volatile.Read/Write` で包むか、単に `volatile` 参照 (string/Uri は不変オブジェクトのため参照の可視性だけ保証すれば十分):
  ```csharp
  private volatile string token = string.Empty;
  public string Token { get => token; set => token = value; }
  ```
  `default!` 初期化 (:7) も `string.Empty` に変更し null を排除
- **注記**: 401時のトークンリフレッシュはサーバ側仕様が未確定のため対象外とし、`ApiDelegatingHandler` に「401 ハンドリングは実案件で実装すること」のコメントを追記
- **検証**: ビルドのみ

## 5-6. InsertWork の採番修正 [M-15] — 規模S

- **対象**: `Services/DataService.cs:148-153`
- **現状**: `SELECT MAX(Id)`→`INSERT` が非アトミック
- **修正** (いずれか。推奨は 1):
  1. `WorkEntity.Id` を SQLite の `INTEGER PRIMARY KEY` 自動採番 (rowid) に任せ、INSERT で Id を渡さない (SqlHelper/エンティティ属性の対応要確認)
  2. 現行構造を維持するなら `UsingTxAsync` でトランザクション化し、`INSERT ... SELECT COALESCE(MAX(Id),0)+1 ...` の単文にする
- **検証**: Data 画面で連続追加して重複キー例外が出ないこと

## 5-7. SQLite 接続設定の強化 [M-16の一部・軽量対応のみ] — 規模S

- **対象**: `Services/DataService.cs:26-43`
- **修正**: `RebuildAsync` 内の PRAGMA に `journal_mode=WAL` と `busy_timeout=3000` を追加 (複数スレッド同時アクセス時の `SQLITE_BUSY` 対策)。マイグレーション機構自体は導入しない (§0-2 のとおり保留、Phase 9-3 のドキュメント対応で代替)
- **検証**: Data 画面の Bulk 系操作中に他のDB操作を行っても busy エラーにならないこと

---

# Phase 6: ViewModel の例外処理・ライフサイクル底上げ

> 方針: Development.md の例外設計 (「予期せぬ例外は個別 catch しない。発生が予期できる失敗は戻り値/TryParse で処理」) に従い、**「予期できる失敗」だけを対処**する。全VMへの機械的な try/catch 追加は行わない。

## 6-1. センサー非搭載端末での画面遷移失敗 [M-1] — 規模S

- **対象**: `Modules/Device/DeviceSensorViewModel.cs:105-110`
- **現状**: `barometer.Start()` 等が非搭載端末で `FeatureNotSupportedException` → 画面遷移ごと失敗
- **修正**: 各センサーの `IsSupported` を確認してから Start。非搭載のものは対応表示項目を "N/A" 表示:
  ```csharp
  if (barometer.IsSupported) { barometer.Start(SensorSpeed.Default); }
  ```
  Stop側 (`OnNavigatingFrom` 等) も `IsMonitoring`/`IsSupported` ガード
- **検証**: エミュレータ (気圧計なし) で Device > Sensor 画面が開けること

## 6-2. Bluetooth 印刷の状態復帰 [M-2] — 規模S

- **対象**: `Modules/Device/DeviceBluetoothViewModel.cs:52-60`
- **現状**: IO例外で `State=Printing` / `IsBusy=true` のままコマンド永久無効化
- **修正**: try/catch(IOException等)/finally で `State` を確実に復帰し、失敗はダイアログ通知。「HttpClient同様、外部IFで発生が予期される例外」のため個別 catch が設計方針に合致
- **検証**: 未ペアリング状態で印刷実行→エラー表示→再操作可能なこと

## 6-3. バックキーの fire-and-forget [M-3] — 規模S

- **対象**: `MainPage.xaml.cs:12-20`
- **現状**: `context.Navigator.NotifyAsync(ShellEvent.Back)` の Task 破棄 (例外未観測)
- **修正**: 破棄する場合も例外だけは観測する拡張を利用:
  ```csharp
  context.Navigator.NotifyAsync(ShellEvent.Back).AsTask()
      .ContinueWith(static t => Log.UnhandledNavigationError(t.Exception!), TaskContinuationOptions.OnlyOnFaulted);
  ```
  もしくはプロジェクト共通の `Forget(ILogger)` 拡張メソッドを `Extensions.cs` に追加して使用 (`Extensions.cs:83-90` のローカル `async void` ハンドラにも同様の考慮)
- **検証**: 戻る処理中に例外を強制発生させ、クラッシュせずログに残ること

## 6-4. BLE スキャンの OnError [M-4] — 規模S

- **対象**: `Modules/Device/DeviceBleScanViewModel.cs:18-22`
- **修正**: `Subscribe(onNext, onError)` の形で onError を追加し、WARN ログ + 必要ならダイアログ表示。Rx の未処理 OnError はアプリを落とすため必須
- **検証**: BT を OFF にしてスキャン開始→クラッシュしないこと

## 6-5. 画面退避後もループが回り続ける問題 [M-7] — 規模M

- **対象**: `Modules/UI/UIMeterViewModel.cs:74` (60fpsループ)、`Modules/Device/DeviceAudioViewModel.cs:48-50` (250msポーリング)
- **現状**: コンストラクタで `_ = StartTimerAsync()` を起動し、スタック退避 (`Stack` ナビゲーション) 中も回り続ける。`OperationCanceledException` 以外は未観測
- **修正**: `INavigationEventSupportAsync` (プロジェクト既存のライフサイクルフック) を利用し、`OnNavigatedTo` で開始 / `OnNavigatingFrom` で CTS キャンセル+完了待ち。ループ本体は try/catch(OperationCanceledException) で正常終了扱い、他例外はログ
- **検証**: 該当画面から他画面へ遷移した後、CPU使用率が下がること (診断パネルで確認可)

---

# Phase 7: 設計リファクタリング (中規模)

## 7-1. シェル情報3重複の解消 [M-22] — 規模M

- **対象**: `Shell/ShellProperty.cs:143-173` (UpdateShellControl)、`Shell/IShellControl.cs:5-25`、`MainPageViewModel.cs:12-42`
- **現状**: Title/HeaderVisible/FunctionVisible/Function1-4 Text/Enabled の11項目が3クラスで手書き重複。F5追加時に4ファイル修正
- **修正方針**: Function ボタンを配列化した中間モデルを導入:
  ```csharp
  public sealed class FunctionState : ObservableObject
  {
      public NotificationValue<string> Text { get; } = new();
      public NotificationValue<bool> Enabled { get; } = new();
  }

  public interface IShellControl
  {
      NotificationValue<string> Title { get; }
      NotificationValue<bool> HeaderVisible { get; }
      NotificationValue<bool> FunctionVisible { get; }
      IReadOnlyList<FunctionState> Functions { get; }   // [0]..[3]
  }
  ```
  - `ShellProperty.UpdateShellControl` はループで反映
  - `MainPage.xaml` のバインディングは `Functions[0].Text.Value` 形式へ変更 (compiled binding のインデクサ対応は要確認。不可なら `Function1`〜`Function4` プロパティを `FunctionState` 型で残す妥協案でも重複は大幅減)
- **注意**: 添付プロパティ (`ShellProperty.Function1Text` 等) の XAML 利用面は互換維持する (全画面の XAML 書き換えを避けるため)
- **検証**: F1〜F4 のテキスト/有効状態が全画面で従来どおり動作すること (Navigation/Device/UI 系から数画面ずつ抜き取り確認)

## 7-2. Usecase 層の IDialog 依存除去 [M-23] — 規模L (段階実施可)

- **対象**: `Usecase/NetworkOperator.cs`、`Usecase/NetworkUsecase.cs`、呼び出し元VM
- **現状**: ユースケース層がダイアログ表示・リトライ確認まで担い、UIなしテスト・再利用が不可能
- **修正方針** (段階案):
  - **Step 1 (必須)**: `NetworkOperator` を `Usecase/` から新設の役割へ再定義 — 実態は「通信実行ポリシー (リトライ/接続判定/エラー分類)」なので、判定結果を列挙型で返す純粋ロジック部分 (`ClassifyError(IRestResponse) => NetworkErrorKind`) を切り出す。Phase 5-4 の重複統合と同時実施が効率的
  - **Step 2 (推奨)**: ダイアログ表示 (「Retry しますか?」「エラーメッセージ」) を VM 側 or 薄い UIアダプタ (`INetworkInteraction` インターフェース) に移し、`NetworkOperator` は `INetworkInteraction` 抽象にのみ依存。ダイアログ実装は DI で注入
  - `CognitiveUsecase` の位置づけ (実態はONNX推論インフラ) は Components/Services への移動が理屈だが、参照変更のみで効果が薄いため**任意** (実施する場合は `Services/` へ移動し namespace 変更)
- **検証**: Network 系全シナリオ (成功/サーバエラー/切断/リトライ) の手動確認

## 7-3. 基底クラス・サービスの重複整理 [M-24, M-28] — 規模M

- **対象と修正**:
  1. `Modules/AppDialogViewModelBase.cs:26-48` の `Validate` — `AppViewModelBase.cs:25-46` とほぼコピー。共通静的ヘルパ (`ValidationHelper.Validate(object target, string propertyName, Errors errors)` 等) へ抽出し両基底から呼ぶ。コピー先に混入している重複行 `validationResults ??= [];` (:33,:40) はこの統合で自然消滅
  2. `Services/HttpService.cs:54-126` の Download/Upload 4メソッド — 進捗計算 (`percent > progress` 判定含む) をプライベートメソッド `CreateProgressCallback(IProgress<int>)` に抽出 (Phase 1-8 の NaN 修正を取り込んだ形で)
  3. CvNet系5VM (`SampleCvNetObject/Tag/People/Ocr/Face`) — プレビュー・ズーム・キャプチャの共通骨格を `SampleCvNetViewModelBase` (抽象基底) へ抽出。各VMは推論呼び出しと結果反映のみ実装
  4. `ScheduleService`/`HolidayService` [M-28] — `Modules/UI/UICalendarViewModel.cs:16-17` 等の `new` 直生成を DI 登録+コンストラクタ注入に変更。配置は実態 (静的サンプルデータ生成器) に合わせ `Models/Sample/` へ移動し、`CA1822` 抑止を除去して static クラス化する案でも可 (テンプレートの見本として DI 注入形を残すなら前者)
  5. `ScheduleService.cs:69-73,142-146` / `HolidayService.cs:8-12` の「2ヶ月前打ち切り」ガード3箇所コピー — 定数+共通判定メソッドへ
- **検証**: Basic > Validation、Network 系、CVサンプル、UI > Calendar/Schedule の動作確認

---

# Phase 8: UI/XAML 品質

## 8-1. RelativeSource バインディングへの x:DataType 付与 [M-35] — 規模S

- **対象** (10箇所): `UIMailView.xaml:289,300`、`UIShopView.xaml:292,334`、`UICharacterView.xaml:234`、`UIChatView.xaml:473`、`UIGraph2View.xaml:108`、`UIStreamView.xaml:296`、`EditListView.xaml:140,147`
- **修正**: 正しく書けている `ViewCollectionView.xaml:140` を見本に、`x:DataType={x:Type module:XxxViewModel}` をバインディング式へ追加
- **検証**: ビルド (strict XAML compilation が通ること) + 該当コマンドの動作確認

## 8-2. グローバル Style キーのシャドーイング解消 [M-33] — 規模M

- **対象**: 10キー/16ファイル (詳細はレビュー M-33 の表)。特に `UIMailView.xaml:222-235` はグローバル `MenuGrid`/`MenuButton` と別意味の同名再定義
- **修正**: ページローカル Style を `Mail` 等の画面プレフィックス付きキーにリネーム (`MailMenuGrid`、`MailTabButton` 等)。参照箇所も同時修正。機械的作業だが取り違えに注意し、1ファイルずつコミット推奨
- **検証**: 各画面の見た目が変わっていないこと (リネームのみ)

## 8-3. ScrollView × CollectionView の入れ子解消 [M-32] — 規模M

- **対象と修正**:
  1. `UIProfileView.xaml:406-411` (最優先) — 縦 ScrollView 内の `GridItemsLayout Orientation="Vertical"` CollectionView。アイテム数固定 (Photos) のため `BindableLayout` + `FlexLayout`/`Grid` へ置換
  2. `UICharacterView.xaml:175,199,224` — 固定高で回避済みだが同様に BindableLayout 化を検討 (アイテム数が少なければ)
  3. `UIStreamView.xaml:278` 等の BindableLayout→CollectionView 三重ネスト — サンプルUIとしての見た目維持を優先し、**変更は任意**。対応する場合は外側を `CollectionView` + セクションを DataTemplate 化 (`ItemTemplateSelector`) する構造へ
- **検証**: 各画面のスクロールが滑らかで、競合 (内側だけスクロールして外側が動かない等) がないこと

## 8-4. 未使用リソース・空 Style の削除 [UIレビュー指摘] — 規模S

- **対象**: `Resources/Styles/Styles.xaml`
  - `:705-721` `LeftSelectButton`/`CenterSelectButton`/`RightSelectButton` (中身が `<!-- TODO -->` のみ。UI_Development_Log.md に「対応不要で削除」と記録済みの残骸)
  - `:23` `NoErrorColor`、`:849-854` `GroupSpan`、`:927-936` `ItemCollectionGrid` (参照0)
- **修正**: 削除前に全 XAML を対象キーで grep して参照0を再確認してから削除
- **検証**: ビルド + 起動確認 (StaticResource 解決エラーは起動時クラッシュになるため必ず実行)

## 8-5. CalendarView 旧版の削除 [L-3] — 規模S

- **対象**: `Controls/CalendarView.xaml(.cs)` (約1,400行、現用は CalendarView2)
- **修正**: `CalendarView` (無印) への参照が0であることを grep で確認のうえ削除。あわせて `CalendarView2` → `CalendarView` へのリネームは**任意** (リネームする場合は利用XAMLの要素名変更を伴う)
- **検証**: UI > Calendar 画面の動作確認

## 8-6. 絵文字/生Unicode の置換 [L-7] — 規模S (任意)

- **対象**: 36箇所 (詳細はレビュー L-7 の表。`UIMoneyView.xaml:434,487` の `🐰`/`➡️`、Wizard系の `◀️▶️`、`CalendarView2.xaml:22,40` の `◀▶` 等)
- **修正**: `markup:Material`/`markup:Fluent` の該当アイコン (`ChevronLeft`/`ChevronRight`/`ArrowForward` 等) へ置換。装飾目的の `🐰` などデザイン意図があるものは残置可 — その場合は UI_Development_Log.md のポリシー側に例外を明記
- **検証**: 見た目確認のみ

---

# Phase 9: クリーンアップ・ドキュメント

## 9-1. デッドコード削除 [L-2] — 規模S

| 対象 | 対応 |
|---|---|
| `Services/ParameterBuilder.cs` | 削除 (未使用。URLエンコード欠陥もあるため残すなら `Uri.EscapeDataString` 修正必須だが、削除を推奨) |
| `State/Session.cs` (空+TODO) | 削除し `MauiProgram.cs:303` の DI 登録も除去、**または**テンプレートの意図 (ログイン情報の置き場見本) としてサンプルプロパティ+コメントを実装。どちらにするか実施時に判断 |
| `Domain/Length.cs` (空+TODO) | 同上 (削除 or データ長定数の見本を1つ実装) |
| `Helpers/ReactiveSignalR.cs` | L-8 除外 (SignalR未実装維持) に伴い**残置**。ただし `:24-32` の async void ラムダによる解放は `IAsyncDisposable` 化しておく (残すなら品質は担保) — 規模S |
| `Modules/UI/UIGraphViewModel.cs:55` `RepositoryData.Empty` | 削除 |
| `Modules/ViewId.cs:75-76` `NetworkRealtime`/`NetworkScp` | 対応画面が骨格のみ。ViewId と画面は残置 (機能追加予定のため) |

## 9-2. `#pragma warning disable` の棚卸し [L-10] — 規模M

- **対象**: 75箇所 (CA1819×9、CA5394×8、CA2012×6、CA1822×6、CA2000×5、CA2213 等)
- **方針**: AGENTS.md「抑止は事前相談」に基づき、以下の優先順で削減。**削減できないものは抑止理由をコメントで併記**する
  1. **リーク系 (CA2213/CA1001/CA2000)**: Phase 2 の修正で実際にリークを解消すれば抑止自体が不要になる箇所が大半 (例: `NoiseMonitor.android.cs:11-13` の CA2213 は 2-5 で解消)
  2. **CA2012 (ValueTask誤用)**: `CameraBind.cs:120,132,146` / `DrawingBind.cs:95` — 2-10 の修正と合わせて正しい await 構造にして抑止除去
  3. **CA1822 (static化可能)**: `ScheduleService` 等は 7-3 の整理で解消
  4. **CA5394 (不安全な乱数)**: サンプルデータ生成用途のため抑止妥当。理由コメントを付けて残置
  5. **CA1819 (配列プロパティ)**: BindableProperty 都合のものは残置妥当。理由コメント付与
- **検証**: 各除去後にビルド警告ゼロを確認

## 9-3. ドキュメント更新 — 規模S

- **対象**: `Document/Development.md`、`README.md` / `README-ja.md`
- **修正**:
  1. Development.md 冒頭の `(TODO Update)` を解消し、以下を追記:
     - 「毎起動 `RebuildAsync` (DB全削除) はサンプル仕様。実案件では必ずマイグレーション方式に置き換えること」(M-16 の代替対応)
     - シークレットの扱い: 「EmbeddedBuildProperty はアセンブリに平文で埋まるため真のシークレットには使わない。実行時キーは SecureStorage を使用」(4-1 と整合)
     - Google Maps API キーは APK Manifest に平文で入るため、キー側でパッケージ名+署名制限をかける運用を必須とする旨
     - コンポーネントイベントのスレッド契約 (2-11 と整合)
  2. README: 現状は実質 Android 専用である旨を明記 (iOS は Behaviors の空スタブのみ)。TODO セクションの重複 (`# TODO` が2回) を修正
- **検証**: 目視レビュー

---

# 実施順序とマイルストーン

```
Phase 1 (確定バグ)          … 依存なし。最初に実施 ────────────┐
Phase 2 (Components)        … 依存なし。並行可                  ├→ マイルストーン A (安定化)
Phase 3 (起動・Manifest)    … 3-2 は Phase 2-6 完了後 ─────────┘
Phase 4 (セキュリティ)      … 依存なし。並行可 ────────────────→ マイルストーン B (セキュリティ)
Phase 5 (データ・通信)      … 5-4 は 7-2 Step1 と同時実施が効率的 ┐
Phase 6 (VM例外処理)        … 依存なし                            ├→ マイルストーン C (堅牢化)
Phase 7 (設計リファクタ)    … 7-2 は 5-4 の後                     ┘
Phase 8 (UI/XAML)           … 依存なし。いつでも可 ─────────────→ マイルストーン D (品質)
Phase 9 (クリーンアップ)    … 9-2 は Phase 2 完了後が効率的 ─────→ 最終
```

## 規模サマリ

| Phase | 項目数 | 規模感 | 主な触るファイル |
|---|---|---|---|
| 1 | 11 | S×9, M×2 | Behaviors/Services/Shell/Modules 点在 (相互独立) |
| 2 | 11 | S×6, M×5 | `Components/` 集中 |
| 3 | 3 | S×2, M×1 | 起動系 + AndroidManifest |
| 4 | 4 | S×3, M×1 | Settings/csproj/AppHostBuilderExtensions |
| 5 | 7 | S×4, M×3 | Services/Helpers/Usecase |
| 6 | 5 | S×4, M×1 | Modules/Device 中心 |
| 7 | 3 | M×2, L×1 | Shell/Usecase/基底クラス |
| 8 | 6 | S×4, M×2 | XAML のみ |
| 9 | 3 | S×2, M×1 | 全体 + ドキュメント |

## 全 Phase 共通の完了条件

1. `dotnet build -f net10.0-android` が**警告ゼロ**で成功 (Directory.Build.props の厳格設定下)
2. 新規の `#pragma warning disable` / `GlobalSuppressions` 追加なし (必要になった場合は追加前に相談 — AGENTS.md)
3. 実機またはエミュレータで該当画面のスモーク確認 (各項目の「検証」欄)
4. XAML 変更は XamlStyler フォーマット適用済み
