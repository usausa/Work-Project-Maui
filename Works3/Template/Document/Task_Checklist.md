# ✅残作業チェックリスト

残作業(実機確認 / 実テスト / 保留)のマスターチェックリスト。経緯・実装内容・ナレッジ・開発ポリシーは `Change_Summary.md`(付録含む)を参照。
優先順 = 0 節 → 1 節 → 2 節 → 3 節 → 4 節 → 5 節 → 6 節。低優先の項目(2-2 OpenTelemetry の組み込み / 3-2 FCM / 4 節 生体認証 / 5 節 オフライン同期)の中では 2-2 を最初に行なう。小さな項目は 1 節にまとめ、大きな項目は章を分けている。

## 📋サマリ

| Category | Feature | 章 |
| --- | --- | --- |
| Review | 未コミット分の確認 | 0-1〜0-4 / 2-1 / 3-1 |
| Basic | .NET 10 API(未適用 API の反映) | 1-1 |
| Diagnostics | Layout metrics(レイアウト診断メトリクス) | 1-2 |
| Basic | Global xmlns | 1-3 |
| View | StyleClass(文字サイズ × 配置) | 1-4 |
| View | Material 3(UseMaterial3) | 1-5 |
| Device | Background task(WorkManager) | 1-6 |
| Basic | Startup screen(初期化の進捗・失敗・再試行) | 1-7 |
| Diagnostics | OpenTelemetry(クラッシュレポート / テレメトリ基盤の組み込み。低優先の中では最初) | 2-2 |
| Device | Push(FCM。低優先) | 3-2 |
| Device | Biometric(生体認証。低優先) | 4 |
| Network | Offline sync(未送信キュー・差分同期・競合解決。低優先) | 5 |

## 📏運用ルール

- 作業はこの番号で指示・進行する(例:「1-1 を実施」)。完了した項目は本書から削除し、内容は `Change_Summary.md` に記録する
- **⚖️【判断】印の項目はユーザーが決定**(勝手に進めない)。デザイン判断を伴う差分は 1 項目ずつ指示を受けて実施
- 実装・変更を行なう場合の完了条件 = **ビルド警告ゼロ** + `Change_Summary.md` への記録(開発ポリシーは同 付録A)
- コミットはユーザーが実施(グループ単位を推奨)
- `README.md` の TODO 表は本書のサマリ表(1〜5 節)と同期させる(項目の追加・削除・完了時に両方を更新。TODO 表に本書の番号は書かない)
- 描画・性能の計測は **Release ビルド + 実機**(手順は `Development.md` の「Releaseビルドでの検証と計測」)

## 🧭前提(環境)

- **環境制約 (不具合ではない)**: ①地図タイルは Google Maps API キー未設定だと非表示 (ピン・カメラ移動は動作) ②Sample > CV Net は AI エンドポイント未設定だと画面に入れない ③CommunityToolkit CameraView の `CaptureAsync` がまれに未完了になる(5 秒で打ち切って「撮影できませんでした」を出し、プレビューのまま続行できる)
- 現在実機に入っているのは **Debug ビルド**(2026-09-19 デプロイ。性能・描画の確認時は Release へ入れ替える)

---

## 🔥0.【最優先】未コミット分の確認

Control メニュー新設以降(2026-09-13〜20)の未コミット分のうち、未確認のもの(確認済みの項目は削除済み)。ネットワーク系は機能ごとに分けている(0-1 REST / 0-2 UL・DL / 0-3 SignalR / 0-4 gRPC)。OpenTelemetry の組み込みサンプル(`Works3/OtelSample`)は 2-1、プッシュ通知の自前サンプル(`Works3/PushSample`)は 3-1。確認できたグループから順にコミットする(ビルド 0 警告 / inspectcode 0 件 / 実機確認は実施済み)。

対向サーバーは `D:\GitHubTemplate\template-maui-server`(別リポジトリ、こちらも未コミット)。起動と端末の接続は `Document/Development.md`「サーバー処理」。サーバー側のパスは `template-maui-server/src/Template.MobileServer.Web/` からの相対((server) 印)。

- コミット対象外: `.claude/worktrees/angry-feistel-0b79a0/`(worktree は登録解除・ブランチ削除済み。空のディレクトリだけがロックで残っているので、アプリの再起動後に削除)

### 🔗0-1 REST(Web API)

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Modules/Network/NetworkHttpView.xaml` + `NetworkHttpViewModel.cs` | Web API(Data の CRUD) | ログイン状態(Id / 有効期限)、一覧(20 件ずつ追加読み込み)、行選択で詳細、作成 / 更新 / 削除 / クリア、未ログインの作成は 401、重複は 409、10 秒待つ API とキャンセル、ログ。F2 = Reload |
| `Services/HttpService.cs` / `ApiContext.cs` | API 呼び出し、認証状態(`LoginId` / `TokenExpires`) | PUT / DELETE は `HttpClient` を `RestResponse` に包む |
| `Usecase/NetworkOperator.cs` / `NetworkUsecase.cs` | 401 の再ログイン再送、CRUD / 遅延のユースケース | 再ログインは 1 回だけ |
| `Models/Api/DataListResponse.cs`(`Id` long / `Total`)/ `DataResponse.cs` / `DataCreateRequest.cs` / `DataCreateResponse.cs` / `DataUpdateRequest.cs` | 契約 DTO | サーバー側と同じ形 |
| `Helpers/JwtHelper.cs`(新規) | JWT の有効期限の取り出し | — |
| (server) `Endpoints/DataEndpoints.cs` / `Models/Api/DataListResponse.cs` / `Core/Services/DataService.cs` / `Core/Models/RangeResult.cs` | 一覧の範囲取得(`offset` / `size`、`Total`) | 省略時は全件 |

- [ ] **0-1** 上記(実機確認は `Change_Summary.md` の「ネットワーク実装」参照)

### 📤0-2 UL / DL(ストレージ)

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Modules/Network/NetworkStorageView.xaml` + `.xaml.cs` + `NetworkStorageViewModel.cs` | ストレージ(簡易 FTP API) | ディレクトリの一覧 / 下階層 / 上へ、ファイル(FilePicker)と写真(MediaPicker)のアップロード、ダウンロード(公開フォルダ)、削除(確認ダイアログ)、進捗とキャンセル。F2 = Reload |
| `Usecase/NetworkUsecase.cs`(ストレージ) | 一覧 / アップロード / ダウンロード / 削除のユースケース | `ExecuteTransfer` はインジケーターなし(進捗とキャンセルは画面側) |
| `Models/Api/StorageListResponse.cs` | 契約 DTO | サーバー側と同じ形 |

- [ ] **0-2** 上記(実機確認は `Change_Summary.md` の「ネットワーク実装」参照)

### 📡0-3 SignalR(Realtime)

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Modules/Network/NetworkRealtimeView.xaml` + `NetworkRealtimeViewModel.cs` | SignalR(MonitorHub、認証なし) | 遷移時に `Connect()` を購読 / 離脱時に破棄(= 切断)、受信は `ServerStatus` / `Notifications` の購読、状態 / 接続 ID / サーバー時刻、サーバーの CPU / メモリ / 接続数のグラフ、端末の状態を 10 秒ごとに送信、通知の一覧(前面 = トースト、バックグラウンド = ローカル通知)。F2 = Connect |
| `Helpers/ReactiveHubConnection.cs`(旧 `ReactiveSignalR.cs`) | 汎用の SignalR 接続維持(`HubConnection` の生成・維持・破棄をクラスが持つ。`Connect(url, configure, resume)` = 購読で接続・破棄で切断・後勝ち、`On<T>()`、`InvokeAsync`(引数なし / 1 つ)、`IsConnected`。初回接続と自動再接続の間隔は ctor の `retryDelays`) | 初回接続のバックオフ再試行(`RetryWhen`)/ ネットワーク復帰で待ち打ち切り(`Amb`)/ 自動再接続 / `Closed` 後のやり直し(`Repeat`)/ 破棄で `StopAsync` / 再購読は前の接続を止めてから新しい接続(同時に 2 本にならない)/ Dispose 後は `ObjectDisposedException`(後勝ち / 切断からの復帰 / 経路なしの初回接続 / 再入は 2026-09-21 に実機確認済み) |
| `Services/MonitorConnection.cs`(新規) | MonitorHub 固有(ハブのパス / KeepAlive / ServerTimeout、`Connect(baseAddress)`、`ServerStatus` / `Notifications`、`ReportDeviceStatusAsync`、ログ) | 認証なし。接続の管理は `ReactiveHubConnection` |
| `Models/Api/MonitorMessages.cs` | 契約 DTO | サーバー側と同じ形 |
| `Extensions.cs` | `ConnectivityChangedAsObservable`(ネットワーク復帰で接続の待ちを打ち切る) | — |
| `State/Session.cs` / `App.xaml.cs` | 前面かどうか(`IsForeground`)を Window の Resumed / Stopped で更新 | — |
| (server) `Hubs/MonitorHub.cs` / `Infrastructure/Monitor/DeviceRegistry.cs` / `DeviceEntry.cs` / `MonitorNotifier.cs` / `Models/Api/MonitorMessages.cs` / `Workers/ServerStatusWorker.cs` / `NotificationRelayWorker.cs` / `Application/ApplicationExtensions.cs` / `Program.cs` / `Application/Log.cs` | SignalR ハブと状態配信 / 通知 | 認証なし、KeepAlive 15 秒 / ClientTimeout 30 秒、`DeviceRegistry.Disconnect` |
| (server) `Components/Pages/DevicesPage.razor(.cs)` / `Layout/NavMenu.razor` / `Pages/Home.razor(.cs)` / `wwwroot/css/app.css` | 管理画面の Devices(端末一覧、通知の送信、切断 = `HubCallerContext.Abort`)、Home の接続数 | 切断で端末が `Closed` → 新 ID で再接続 |

- [ ] **0-3** 上記(実機確認は `Change_Summary.md` の「ネットワーク実装」参照)

### 💬0-4 gRPC(チャット)

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Modules/Network/NetworkGrpcView.xaml` + `NetworkGrpcViewModel.cs` | gRPC(チャット) | 接続先 / 状態 / 未配送数、単項 RPC(サーバー時刻)、チャット(管理画面 `/chat` と相互)、切断中の送信は再接続後に配送。F2 = Connect |
| `Services/ChatRoomClient.cs`(関連型も同じファイル)/ `Services/Protos/chat.proto` / `server.proto` | gRPC チャット(サーバーの WPF サンプルの移植)と proto のコピー | 指数バックオフ再接続、送信キュー、トークンは接続ごとに取得 |
| `Usecase/NetworkUsecase.cs`(`EnsureLoginAsync` / `GetTokenAsync`) | gRPC チャット用のトークン取得 | 再接続ループからは `NetworkOperator`(インジケーター)を通さない |
| (server) `Protos/server.proto` / `Handlers/ServerInfoHandler.cs` | 単項 RPC(サーバー時刻、匿名) | — |

- [ ] **0-4** 上記(実機確認は `Change_Summary.md` の「ネットワーク実装」参照)

---

## 🔗1. 取り込み候補(リンク集・小さな追加項目)

`■MAUI.txd`(リンク集)は全件に判定を付記済み(🟩 取り込む / 🟦 取り込まないが記事として有用 / 🟥 古い・参照不要 / 🟨 要判断)。本節へ移した項目の元行は同書から削除している。本節は取り込む価値のあるトピックだけ。ファイルパスは `Template.MobileApp/` からの相対。章を分けるほどではない小さな追加項目(1-7)も本節に置く。

### 🧩1-1 .NET 10 の未適用 API の反映

参照: https://learn.microsoft.com/ja-jp/dotnet/maui/whats-new/dotnet-10?view=net-maui-10.0(要約: https://www.telerik.com/blogs/recap-whats-new-net-maui-net-10)
適用済み: `MauiXamlInflator=SourceGen` / `UseMonoRuntime=false`(CoreCLR)/ `SafeAreaEdges` / Async 系アニメーション API(`TranslateToAsync` 等)/ `Switch.OffColor` / `SearchBar.SearchIconColor` / `SearchBar.ReturnType`(Basic > Setting)/ `Geolocation.IsEnabled`(Device > Location の空状態)/ `Vibration.IsSupported` / `HapticFeedback.IsSupported`(Device > Misc のボタン有効化)/ `dotnet run --device`(`Development.md`)。`ListView` / `TableView` / `MessagingCenter` / `DisplayAlert` / `Page.IsBusy` は未使用のため非推奨化の影響なし。`Shell.NavBarVisibilityAnimationEnabled` は Shell 不使用のため対象外。

残りは UI の追加や共有ライブラリの変更を伴うもの。

| API | 概要 | 現在のファイル名 | 変更 |
| --- | --- | --- | --- |
| `Picker` の Open / Close API、`DatePicker.Date` / `TimePicker.Time` の nullable 化 | プログラムからの開閉、未選択状態(null) | `Modules/Basic/BasicSettingView.xaml` + `BasicSettingViewModel.cs` | 未選択の表現(null)と開くボタン |
| `RefreshView.IsRefreshEnabled` | `IsEnabled` と分離した引き下げ更新の有効 / 無効 | `Modules/Control/ControlRefreshView.xaml` + `ControlRefreshViewModel.cs` | 切替スイッチを追加 |
| `SpeechOptions.Rate` | 読み上げ速度 | `Modules/Device/DeviceMiscView.xaml` + `DeviceMiscViewModel.cs`(Speech カード)、MauiComponents の `ISpeechService.SpeakAsync` | 速度の引数(共有ライブラリ側)と速度スライダー |
| `HybridWebView.WebResourceRequested` / `InvokeJavaScriptAsync`(戻り値なし)/ `WebViewInitializing` / `WebViewInitialized` | リクエストの横取り(ローカル応答・ヘッダ変更)、初期化イベント、JS 例外の .NET 側再スロー | `Modules/Sample/SampleWebAppView.xaml` + `SampleWebAppViewModel.cs` | ローカル応答のデモ |
| `WebView` の Android 全画面動画(`allowfullscreen`)/ JavaScript 有効・無効の platform-specific | Android 固有の WebView 設定 | `Modules/Sample/SampleWebBasicView.xaml` | 設定を追加 |

- [ ] **1-1-1** Basic > Setting(Picker の Open / Close、DatePicker / TimePicker の null)
- [ ] **1-1-2** Control > Refresh(`IsRefreshEnabled`)
- [ ] **1-1-3** Device > Misc(`SpeechOptions.Rate`。MauiComponents 側の変更を含む)
- [ ] **1-1-4** Sample > HybridWebView / Web view

### 📐1-2 レイアウト診断メトリクス(DiagnosticPanel)

参照: https://learn.microsoft.com/ja-jp/dotnet/maui/whats-new/dotnet-10?view=net-maui-10.0(「Diagnostics」)、https://github.com/dotnet/maui/pull/31058 — `ActivitySource` / `Meter` 名 `"Microsoft.Maui"` で `IView.Measure` / `Arrange` の回数と所要時間(`maui.layout.measure_count` / `measure_duration` / `arrange_count` / `arrange_duration`)を記録する。`System.Diagnostics.Metrics.Meter.IsSupported` のフィーチャースイッチで AOT / トリミング時に無効化できる。

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Shell/DiagnosticPanel.xaml` + `.xaml.cs` | メモリ推移のスパークライン(DEBUG 限定) | `MeterListener` で measure / arrange の回数・平均時間を購読し、メモリの横に表示 |
| `Template.MobileApp.csproj` | ビルド設定 | Release で `Meter.IsSupported` が無効であることを確認(必要なら `RuntimeHostConfigurationOption` で false) |

- [ ] **1-2** 上記。画面遷移で回数が増え、静止時は増えないことを確認

### 🏷️1-3 XAML の global xmlns 化

参照: https://learn.microsoft.com/ja-jp/dotnet/maui/whats-new/dotnet-10?view=net-maui-10.0(「Implicit and Global XML namespaces」)— `GlobalXmlns.cs` に `[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "Template.MobileApp.Controls")]` 等を並べ、XAML 側は `xmlns="http://schemas.microsoft.com/dotnet/maui/global"` の 1 行で `controls:` / `behaviors:` / `converters:` 等の接頭辞を省略できる。`XmlnsPrefix` で接頭辞を残す運用も可。ルートの `xmlns` / `xmlns:x` まで省く implicit 版は `MauiAllowImplicitXmlnsDeclaration` + `EnablePreviewFeatures` のプレビュー機能。

- [ ] **1-3-0**⚖️【判断】適用範囲 — 案A `GlobalXmlns.cs` の追加のみ(既存 XAML は変更不要、新規画面から接頭辞を省略)/ 案B 全 XAML(約 120 ファイル)の接頭辞を一括で除去 / 見送り。implicit 版(プレビュー)は対象外

### 🎨1-4 StyleClass の活用(文字サイズ × 配置)

方針(付録A): スタイルの基本は共有 `Styles.xaml` からの `BasedOn` 派生。`StyleClass` は複数指定できる利点を、文字サイズ × 配置のように**直交する属性の組み合わせ**にだけ使い、サイズ × 配置の数だけスタイルを用意しなくて済むようにする。色や余白などは Style 側に置き、同じプロパティを Style と StyleClass の両方で指定しない。Crosswind(https://github.com/sthewissen/Plugin.Maui.Crosswind)のような全面的なユーティリティクラスは採用しない。

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- | 
| `Resources/Styles/StyleClasses.xaml` | — | 新規。`Label` 向けのクラス(`Class` 付き Style)を定義: サイズ `size-10` / `size-11` / `size-12` / `size-14` / `size-16` / `size-18` / `size-20` / `size-24` / `size-28` / `size-36` / `size-48`(付録A の許可値のうち使用中のもの)、水平配置 `align-start` / `align-center` / `align-end`、太字 `bold` |
| `App.xaml` | リソース辞書のマージ | `StyleClasses.xaml` を追加(共有 `Styles.xaml` は変更しない) |
| `Modules/Network/NetworkHttpView.xaml` / `NetworkStorageView.xaml` / `NetworkRealtimeView.xaml` / `NetworkGrpcView.xaml` / `NetworkScpView.xaml` | 第 1 弾の適用先 | 画面ローカルの `CaptionLabel` / `ValueLabel` / `LogLabel` 等のうち FontSize だけを持つものを `StyleClass="size-12"` 等に置き換え(色を持つものは BasedOn 派生に残しサイズだけクラスへ) |
| `Document/Change_Summary.md` 付録A | 開発ポリシー | 上記の方針を追記(1-4-1 で実施) |

- [ ] **1-4-1** クラスの定義と `App.xaml` へのマージ、付録A への方針の追記
- [ ] **1-4-2** Network の 5 画面へ適用し、表示が変わらないことを実機で確認
- [ ] **1-4-3** 既存画面は触るときに適用する(一括変更はしない)。Style と StyleClass の同一プロパティ指定が無いことを inspectcode / 目視で確認

### 🧱1-5 Material 3(`UseMaterial3`)

参照: https://devblogs.microsoft.com/dotnet/dotnet-maui-material-3/ — Android の Material 3(Material You)対応(2026-05)。`Microsoft.Maui.Controls` 10.0.60 以降(本プロジェクトは 10.0.100)で csproj に `<UseMaterial3>true</UseMaterial3>` を置くだけで有効(既定 false。Handler / `styles.xml` の変更は不要)。対象は Entry / Editor / SearchBar / RadioButton / ProgressBar / Slider / Picker / TimePicker / DatePicker / CheckBox / Switch / ImageButton / Button / Shell の既定外観(Entry / Editor は outlined の `TextInputLayout`、DatePicker はカレンダー ダイアログ)。XAML / C# で明示した色・スタイルは優先される。未対応: コントロール単位の opt-in、動的カラー トークンの API、NavigationPage / TabbedPage / FlyoutPage / CollectionView / Border の再スタイル。

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Template.MobileApp.csproj` | ビルド設定 | `<UseMaterial3>true</UseMaterial3>` を追加 |
| `Behaviors/EntryOption.android.cs` | Entry / Editor の NoBorder(`BackgroundTintList`)/ フォーカス枠 | outlined `TextInputLayout` 化との干渉を確認(枠線が二重になる場合は NoBorder の実装を Material 3 用に切替) |
| `Resources/Styles/Styles.xaml` | 共有スタイル | 変更しない(明示指定が優先されるため差分は既定外観のみ) |

- [ ] **1-5-0**⚖️【判断】採否 — 有効化は 1 行だが Android の入力系コントロールの既定外観が全画面で変わる(Basic / Setting / Kit 系の Entry・Switch・DatePicker が主な影響範囲)。採用時は全画面の目視確認と `Document/*.png` の撮り直しが必要

### ⏰1-6 バックグラウンド定期タスク(WorkManager)

参照: https://www.nuget.org/packages/Shaunebu.MAUI.BackgroundTaskManager — Android は `WorkManager`(最短 15 分間隔)、iOS は `BGTaskScheduler` を使い、CRON 式でフォアグラウンド / バックグラウンドのジョブを登録して `Preferences` に永続化する薄いラッパー(`RegisterJob<T>()` / `Schedule(jobId, cron, callback)` / `ScheduleInBackground(jobId, cron, jobType)`。2025-10、230 DL、リポジトリ公開なし)。採用はせず API の形(ジョブ登録 / CRON 近似 / 永続化)だけ参考にし、`AndroidX.Work` を直接使う。

範囲: 制約付き(ネットワーク接続時 / 充電中)の一回限りワーク + 15 分周期の定期ワーク。実行結果はローカル通知(`Components/NotificationService`)か画面のログに出す。正確な時刻指定は `AlarmManager`(通知で実装済み)の担当のままとし、WorkManager は遅延可・再起動後も残る処理に限定する。

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Template.MobileApp.csproj` | パッケージ | `Xamarin.AndroidX.Work.Runtime` を追加(`dotnet list package --include-transitive` で `Fragment.Ktx` ピン止めとの競合を確認) |
| `Components/BackgroundTask.cs` + `.android.cs` | — | 新規。登録(一回 / 定期)/ 取消 / 状態取得。`AndroidX.Work.Worker` 派生の `DoWork()` で処理 |
| `Modules/Device/DeviceMiscView.xaml` + `DeviceMiscViewModel.cs` | 雑多なデバイス機能(Device メニューは満杯) | `InfoCard` を追加(登録 / 取消 / 最終実行時刻) |
| `MauiProgram.cs` | `ConfigureComponents` | DI 登録 |

- [ ] **1-6-0**⚖️【判断】要否 — 再起動後も残る遅延処理(同期 / 送信キュー)の需要があるか。WorkManager は再起動後に自動で再スケジュールされるため `RECEIVE_BOOT_COMPLETED` は不要。常駐(前景サービス)は対象外

### 🚀1-7 起動状態と再試行画面

現状: `App.OnStart` がフォントの準備 → データベース初期化(`DataService.RebuildAsync`)→ `StartupState.NotifyCompleted()` の順に進め、`MainPageViewModel.OnCreated` は完了を待ってから Menu へ遷移する。初期化中は空の画面のままで、失敗時は「Failed to initialize database」のダイアログを出して終了するだけ(再試行の導線が無い)。

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `State/StartupState.cs` | 完了の通知(`TaskCompletionSource`) | 段階(フォント / データベース / 設定)と状態(実行中 / 失敗 + 例外 / 完了)を観測できる形にする(`ObservableProperty`)。再試行は `App` が登録した初期化処理を呼び直す |
| `App.xaml.cs` | 起動時の初期化 | 初期化を再試行できる 1 メソッドにまとめ、段階ごとに `StartupState` を更新する。失敗はダイアログ + 終了ではなく `StartupState` へ通知(終了は再試行画面の Exit) |
| `MainPage.xaml` + `MainPageViewModel.cs` | 初期遷移まで空表示 | 案A: 起動オーバーレイ(段階の文言 + `ActivityIndicator`、失敗時はメッセージ + Retry)を追加し `StartupState` をバインドする。初期遷移で非表示 |
| `MainPageViewModel.cs` | 初期遷移 | 案B: `OnCreated` で完了を待たずに Startup へ遷移し、完了を受けて Menu へ(Activity 再生成のガードは維持) |
| `Modules/Main/StartupView.xaml` + `StartupViewModel.cs` / `Modules/ViewId.cs` | — | 案B: 新規。段階の一覧(済 / 実行中 / 失敗)と進捗インジケーター、失敗時はメッセージ + Retry / Exit。機能キーは無効 |

- 段階ごとの所要時間を `StartupState` に記録して診断ログ(DEBUG)へ出す。起動時に行なうのはフォント準備(計測済み)と DB 初期化だけで、DI の生成は解決時まで遅延、地図 / カメラ / AI / SkiaSharp は画面ごとに初期化されるため、使うときまで遅らせる対象は現状無い(`UseXxx()` のハンドラー登録は遅延できない)。重い初期化が増えたときはこの計測で見つけて遅らせる
- [ ] **1-7-0**⚖️【判断】採否と方式 — 進捗の段階はフォント / データベースの 2 つだけなので、案A(`MainPage` のオーバーレイ + Retry。新規画面なし)/ 案B(専用画面 `StartupView`。起動時にマスタ同期などの処理を足すならこちら)/ 見送り。確認は DEBUG のフォールト注入(データベース初期化で例外を投げる)で 失敗 → Retry → 成功 を見る

## 📈2. OpenTelemetry(クラッシュレポート / テレメトリ)

検討資料は `Document/Telemetry_Study.md`(現状 / 候補 A・B・C / 比較 / アプリ側の組み込み設計 / 論点)。候補 B(OTLP)の実証サンプルは `Works3/OtelSample`(同フォルダの README に完結)。

### 🧪2-1 組み込みサンプル(`Works3/OtelSample`)の確認

本アプリのコードは無変更。構成 / 実行手順 / 送信の設計(OTLP/HTTP、ILogger の転送、ディスク退避と再送、クラッシュ、MAUI のレイアウト計測)/ サーバの画面 / 確認済みの動作 / 解析上の制約 / ナレッジはすべて `Works3/OtelSample/README.md`。`Document/Telemetry_Study.md` の候補 B の実証。サーバ(`OtelServer`)は 2026-09-19 に Blazor(MudBlazor)+ SQLite のダッシュボード(Dashboard / Logs / Traces / Metrics / Dummy Data)へ刷新し、OTLP/gRPC の受け口(4317)も追加。クライアントは HTTP / gRPC を切替可(未コミット。`App_Data/` は `.gitignore`)。

- [ ] **2-1** `Works3/OtelSample`(README に沿って確認。クライアント = `EventSourceSupport=true`、`AddMetrics`、`AddView` によるタグの集約、`OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY=disk` の採用 / サーバ = Razor コンポーネントのための CA1515 抑止(`GlobalSuppressions.cs`)、MudBlazor 9.10.0 と Microsoft.Data.Sqlite 10.0.12 の採用、ダッシュボードの各画面(実機の Pixel 9a とダミーデータで確認済み))

### 🧩2-2 本アプリへの組み込み(低優先の中では最初)

- [ ] **2-2-0**⚖️【判断】方式と範囲 — `Telemetry_Study.md` 6 節の論点(方式 A / B / C または組み合わせ、収集する項目と粒度、端末の識別と個人情報、サーバーの配置と運用、ライブラリの形、本テンプレートでの見せ方)。組み込み点は同 5 節(`ITelemetry` の抽象化と Null 実装、`Settings` の接続先(`OtelEndPoint` は投入済み)、`CrashReport` / ナビゲーション / `DeviceState` / `NetworkOperator` / `DiagnosticPanel` からの収集、バックグラウンド移行でのフラッシュ)

## 🔔3. プッシュ通知

ローカル通知は `Components/NotificationService.cs` + `.android.cs` で実装済み(即時 / スケジュール / アクションボタン / タップ時ペイロード。Device > Misc の Notification カード)。FCM を使わない自前配信(SignalR + 前景サービス)は `Works3/PushSample`(同フォルダの README に完結)。

### 📦3-1 自前配信のサンプル(`Works3/PushSample`)の確認

本アプリのコードは無変更。構成 / 実行手順 / 再接続の設計 / 確認済みの動作 / 解析上の制約はすべて `Works3/PushSample/README.md`。

- [ ] **3-1** `Works3/PushSample`(README に沿って確認。`PushHub` の CA1812 抑止と `CommunityToolkit.Mvvm` の採用を含む)

### ☁️3-2 FCM(低優先)

参照: `0_maui-samples/10.0/WebServices/PushNotificationsDemo`(`Xamarin.Firebase.Messaging` + 自前 `FirebaseMessagingService`)。

| 種別 | URL | 概要 | 適用先 |
| --- | --- | --- | --- |
| 記事 | https://www.andreasnesheim.no/push-notifications-in-net-maui-with-firebase/ | FCM を `Plugin.Firebase` で扱う手順。Firebase Console 登録 → `google-services.json` 配置 → `MauiProgram` 初期化 → `CrossFirebaseCloudMessaging.Current.GetTokenAsync()` でトークン取得 → Console からテスト送信(2022-09) | 採用時の方式候補①(`Plugin.Firebase`)。方式候補②は上記 PushNotificationsDemo |
| ライブラリ | https://github.com/Gekidoku/BetterFireBaseNotificationsPlugin | `CrossFirebasePlugin` の MAUI 移植。データ付き通知 / アクションボタン / サイレント通知 / アプリ終了時の受信に対応。NuGet 配布はなくプロジェクト参照前提(2026-01) | 実装範囲(データ / アクション / サイレント)のチェックリスト |

- [ ] **3-2-0**⚖️【判断】プッシュ通知(FCM)の要否 — Firebase プロジェクトと `google-services.json` が前提。採用する場合、トークン表示と受信ログを Notification カードに追記する(低優先)

## 🔐4. 生体認証(低優先)

画面・`ViewId`・メニューボタンが配置済み(`DeviceMenuView.xaml` の該当ボタンが `IsEnabled="False"`、画面は `Not implemented` 表示、ViewModel は 8 行)。プラットフォーム実装は MauiComponents の `WiFi.cs` + `WiFi.WiFiManager.cs` / `.android.cs` と同じ構成(共通インターフェース + `*.android.cs`)に揃える。

参照: `Bio`(`Maui.Biometric-main` / `MauiBiometricPluginSample-main` / `NET-MAUI-FingerPrint-main`)

| 種別 | URL | 概要 | 適用先 |
| --- | --- | --- | --- |
| ライブラリ | https://github.com/oscoreio/Maui.Biometric | `Plugin.Fingerprint` の後継。`IBiometricAuthentication.CheckAvailabilityAsync` が `AvailabilityResult`(`AuthenticationAvailability`: NoSensor / NoBiometric / TemporaryUnavailable / NoPermission / NotSupported 等 + 検出した `BiometricSensor` の集合)を返し、`AuthenticateAsync(new AuthenticationRequest(title, reason) { Authenticators = Biometric \| DeviceCredential, ConfirmationRequired })` が `AuthenticationResult` を返す。Android 実装は `AndroidX.Biometric.BiometricPrompt`。`.UseBiometricAuthentication()` で DI 登録。v2.5.1(2026-03) | 案B の候補。可用性 3 区分は `NoSensor` / `NoBiometric` / `TemporaryUnavailable` が対応。`sample/MainViewModel.cs` が可用性表示 + 認証 + 結果表示の最小例 |
| 公式 | https://developer.android.com/training/sign-in/biometric-auth?hl=ja | 「生体認証ダイアログを表示する」。`BiometricManager.canAuthenticate` による可用性判定(`BIOMETRIC_SUCCESS` / `ERROR_NO_HARDWARE` / `ERROR_NONE_ENROLLED` / `ERROR_HW_UNAVAILABLE`)、`BiometricPrompt.PromptInfo` の組み立て、認証コールバック、`CryptoObject` | 案A の一次資料。可用性 3 区分は `canAuthenticate` の戻り値をそのまま対応付ける |

- [ ] **4-1-0**⚖️【判断】実装方式 — 案A `Components/Biometric.cs` + `.android.cs` を自作(`Xamarin.AndroidX.Biometric` を追加)/ 案B `Maui.Biometric` パッケージを参照

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Components/Biometric.cs` | — | 新規。`IBiometricAuthenticator`(可用性判定 / 認証 / 結果種別) |
| `Components/Biometric.android.cs` | — | 新規。`AndroidX.Biometric.BiometricPrompt` 実装 |
| `Modules/Device/DeviceBiometricViewModel.cs` | 空スタブ | 実装 |
| `Modules/Device/DeviceBiometricView.xaml` | 空状態表示 | 可用性表示 + 認証ボタン + 結果表示 |
| `Modules/Device/DeviceMenuView.xaml` | `Grid.Row="7" Grid.Column="1"` | `IsEnabled="False"` を削除 |
| `MauiProgram.cs` | `ConfigureComponents` | DI 登録 |
| `Platforms/Android/AndroidManifest.xml` | 権限 | `USE_BIOMETRIC` 追加 |
| `Template.MobileApp.csproj` | パッケージ | 案A: `Xamarin.AndroidX.Biometric` 追加 |

- [ ] **4-1** 可用性は「ハードウェア無し / 未登録 / 一時利用不可」を区別して表示する。範囲は認証成否の表示まで(鍵の解錠に使う `CryptoObject` は対象外)
  - 制約: `androidx.biometric` は `androidx.fragment` に依存する。csproj は `Xamarin.AndroidX.Fragment.Ktx` をピン止めしているため、追加後に `dotnet list package --include-transitive` で競合を確認する
  - `BiometricPrompt` が要求する `FragmentActivity` は `MainActivity`(`MauiAppCompatActivity` 派生)で満たしている。基底クラスの変更は不要

## 🔄5. オフライン同期(低優先)

ローカル DB に未送信の変更を保持し、接続回復後に差分同期する。競合解決の画面を含む。対向(template-maui-server)の API 追加も要る。

- [ ] **5-1-0**⚖️【判断】採否 — 範囲: Network > HTTP(Data の CRUD)を対象に、未送信キュー(ローカル DB)/ `Connectivity` 復帰で送信 / サーバーの更新時刻による競合検出 / 競合一覧で端末側・サーバー側を選ぶ画面
