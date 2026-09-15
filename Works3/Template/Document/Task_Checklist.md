# 残作業チェックリスト

残作業(実機確認 / 実テスト / 保留)のマスターチェックリスト。経緯・実装内容・ナレッジ・開発ポリシーは `Change_Summary.md`(付録含む)を参照。
優先順 = 0 節 → 2 節 → 3 節。以降は 1 節(要 SSH サーバ)。

## サマリ

| Category | Feature | 章 |
| --- | --- | --- |
| Review | 未コミット分の確認(0-1〜0-13) | 0 |
| Network | SCP の実機確認と転送実テスト(要 SSH サーバ) | 1 |
| Device | Biometric(生体認証) | 2-1 |
| Device | Push(FCM) | 2-2 |
| Basic | .NET 10 API(未適用 API の反映) | 3-2 |
| Diagnostics | Layout metrics(レイアウト診断メトリクス) | 3-3 |
| Basic | Global xmlns | 3-4 |
| View | StyleClass(文字サイズ × 配置) | 3-5 |
| Sample | Face identification(LargeFaceList) | 3-6 |
| View | Material 3(UseMaterial3) | 3-10 |
| Device | Background task(WorkManager) | 3-12 |

## 運用ルール

- 作業はこの番号で指示・進行する(例:「2-1 を実施」)。完了した項目は本書から削除し、内容は `Change_Summary.md` に記録する
- **【判断】印の項目はユーザーが決定**(勝手に進めない)。デザイン判断を伴う差分は 1 項目ずつ指示を受けて実施
- 実装・変更を行なう場合の完了条件 = **ビルド警告ゼロ** + `Change_Summary.md` への記録(開発ポリシーは同 付録A)
- コミットはユーザーが実施(グループ単位を推奨)
- `README.md` の TODO 表は本書のサマリ表(2〜3 節)と同期させる(項目の追加・削除・完了時に両方を更新。TODO 表に本書の番号は書かない)
- 描画・性能の計測は **Release ビルド + 実機**(手順は `Development.md` の「Releaseビルドでの検証と計測」)

## 前提(環境)

- **環境制約 (不具合ではない)**: ①地図タイルは Google Maps API キー未設定だと非表示 (ピン・カメラ移動は動作) ②SampleCvNet 系は AI エンドポイント未設定だと画面に入れない ③CommunityToolkit CameraView の `CaptureAsync` がまれに未完了になり Function キーが無反応化 (再起動で回復)
- 現在実機に入っているのは **Debug ビルド**(2026-09-14 デプロイ。性能・描画の確認時は Release へ入れ替える)

---

## 0.【最優先】未コミット分の確認

Control メニュー新設以降(2026-09-13〜14)の未コミット分。確認できたグループから順にコミットする(ビルド 0 警告 / inspectcode 0 件 / 実機確認は実施済み)。ファイルパスは `Template.MobileApp/` からの相対。

- コミット対象外: `results.xml`(inspectcode の出力。削除)/ `.claude/worktrees/angry-feistel-0b79a0/`(worktree の残骸。削除)

### 0-1 Control メニューの新設と画面の移動

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Modules/Main/MenuView.xaml` | メインメニュー | Row 4 が View \| Control の 2 列、Control のアイコン |
| `Modules/Control/ControlMenuView.xaml` + `ControlMenuViewModel.cs` | Control メニュー(新規、9 段 × 2 列) | Collection \| Carousel / Refresh \| − / Toolkit \| Custom / Chart \| Sf Chart / Grid \| Card List / Bottom Sheet \| Drawer、空き 3 段は無効ボタン、Back で Main |
| `Modules/Control/ControlCollectionView` / `ControlCarouselView` / `ControlRefreshView` / `ControlToolkitView` / `ControlCustomView`(各 `.xaml` / `.xaml.cs` / `ViewModel.cs`) | View から移動(旧 `Modules/View/View*` は削除) | 表示と Back 先が Control メニュー |
| `Modules/Control/ControlChartView` / `ControlSfChartView`(同上) | Sample から移動(旧 `Modules/Sample/SampleChartView` / `SampleSfChartView` は削除) | 同上 |
| `Modules/View/ViewMenuView.xaml` / `Modules/Sample/SampleMenuView.xaml` | 移動元のメニュー | 移動した項目が無い、空セルは無効ボタン |
| `Modules/ViewId.cs` / `Markup/AppIcons.cs` | `Control*` の ViewId、追加アイコン(`ViewModule` / `TableChart` / `ViewAgenda` / `MenuOpen` / `VerticalAlignBottom` / `SmallVideocam` / `SmallStop` / `SmallNotifications` ほか) | ビルド |
| `Document/Control_Collection.png` / `Control_Carousel.png` / `Control_Refresh.png` / `Control_Chart.png` / `Control_SfChart.png` | 画像の改名(旧 `UI_Collection` / `UI_Carousel` / `UI_Refresh` / `Sample_Chart` / `Sample_SfChart` は削除) | README の画像リンク |

- [ ] **0-1** Main > Control の 7 画面(Collection / Carousel / Refresh / Toolkit / Custom / Chart / Sf Chart)の表示と Back。Toolkit に「シート」タブが無い(Bottom Sheet へ移動)

### 0-2 Grid(ClamGrid)と Card list

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Template.MobileApp.csproj` | `ClamGrid` 1.0.0 / `Plugin.Maui.ScreenRecording` 1.0.0-preview5 の追加 | パッケージ参照 |
| `Modules/Control/ControlGridView.xaml` + `ControlGridViewModel.cs` / `ControlGridStyles.cs` | 受注一覧 2,000 行 | 見出しタップでソート(3 段階・順位)、見出し長押しで列設定、行タップで選択、行長押しで未処理の一括選択 / 全解除、確認列のチェック、F2 選択解除 / F3 既定へ / F4 再読込、確定 / 状態更新。状態 / フラグ / ランク / 受付の絵文字と色 |
| `Modules/Control/ControlGridColumnView.xaml` + `ControlGridColumnViewModel.cs` | 列設定 | チェックで表示、行ヘッダのドラッグで順序、Apply で反映 / Reset |
| `Models/Control/OrderRow.cs` / `OrderStatus.cs` / `OrderChannel.cs` / `OrderSamples.cs` / `ColumnOptionAccessors.cs` | 行モデル / 状態 / 受付経路 / ダミー / 列設定行 | — |
| `Modules/Control/ControlCardListView.xaml` + `ControlCardListViewModel.cs` / `Models/Control/Visit.cs` | 訪問先一覧 40 件 | 行タップで選択(青地 + 白文字)、右端で展開、担当アバター / 状態・重点・今日・初回・区分のバッジ、絵文字の情報行、並替パネル(最大 3 キー・順位)、昇降 / 全展開 / 再読込、F3 未訪問の一括選択 / F4 確定 |
| `Modules/Parameters.cs` | 列設定セッションと列順序の受け渡し | — |
| `Helpers/ImageHelper.cs` | `old?.Dispose()` の ReSharper 抑止コメント | — |
| `Document/Control_Grid.png` / `Control_CardList.png` | 画像 | README |

- [ ] **0-2** 上記の操作

### 0-3 WiFi manager

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Components/WiFiManager.cs` + `WiFiManager.android.cs` | 接続情報(NetworkCallback)とスキャン結果(`registerScanResultsCallback`)、無線オン / オフの追従 | — |
| `Modules/Device/DeviceWiFiView.xaml` + `DeviceWiFiViewModel.cs` / `Converters/WiFiSignalIconConverter.cs` | 接続ブロック + アクセスポイント一覧 | 接続情報(タップで詳細)、一覧のバッジ / 絵文字(タップで展開)、最後の結果から 30 秒後の自動スキャン、未検出は半透明 → 60 秒で消える、F4 設定 |
| `Modules/Device/DeviceMenuView.xaml` / `Permissions.cs` / `Platforms/Android/AndroidManifest.xml` / `Extensions.cs` / `MauiProgram.cs` | WiFi ボタン有効化、`NearbyWifiDevices` 権限、`CHANGE_WIFI_STATE` / `NEARBY_WIFI_DEVICES`、`StateChangedAsObservable`、DI | — |
| `Document/Device_WiFi.png` | 画像 | README |

- [ ] **0-3** Device > WiFi の表示。無線オフ → 全行が未検出 → 消える → オンで再検出

### 0-4 Bottom sheet / Drawer

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Controls/BottomSheetView.cs` | 自作ボトムシート(Grid 派生) | — |
| `Controls/SideDrawer.cs` + `SideDrawer.android.cs` | 自作ドロワー(左端の帯の上下中央 200dp をシステムジェスチャから除外) | — |
| `Modules/Control/ControlBottomSheetView.xaml` + `ControlBottomSheetViewModel.cs` | `SfBottomSheet` と自作の比較 | F2 Sf / F3 自作。ドラッグで 半開 ⇔ 全開 ⇔ 閉じる、背景タップで閉じる、項目選択で「結果」に反映 |
| `Modules/Control/ControlDrawerView.xaml` + `ControlDrawerViewModel.cs` | `SfNavigationDrawer` と自作の比較 | セグメントで切替、F2 開閉、項目タップで「選択中」、背景タップ、パネルのドラッグで閉じる、自作は帯の中央からの端スワイプで開く(外は戻る操作) |
| `Modules/Control/ControlToolkitView.xaml` + `ControlToolkitViewModel.cs` | シートタブの削除、`SfSegmentedControl.SelectedIndex` を TwoWay | セグメントのタップが選択インデックスに反映 |
| `Document/Control_BottomSheet.png` / `Control_Drawer.png` | 画像 | README |

- [ ] **0-4** 上記の操作

### 0-5 診断パネルのメモリ推移とリーク検出(DEBUG)

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Shell/DiagnosticPanel.xaml` + `.xaml.cs` | 直近 60 秒のワーキングセットのスパークライン | 📈 でパネルを出すと下部にグラフ(色は Memory の値に連動) |
| `Extender/LeakDetectionPlugin.cs` / `MauiProgram.cs` / `Log.cs` | 閉じたビューと VM の回収を 5 秒後に確認(DEBUG 限定登録) | 画面を行き来して logcat に `Leak suspected` が出ない(`Closed object collected` が出る) |

- [ ] **0-5** 上記

### 0-6 Azure AI Vision / Ollama チャット / 音声入力

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Usecase/AzureVisionUsecase.cs` | Image Analysis 4.0(物体 / 人物 / タグ / 文字)と Face | — |
| `Modules/Sample/SampleCvNetObjectView.xaml` / `Tag` / `People` / `Ocr` / `Face`(各 `.xaml` + `ViewModel.cs`)/ `Graphics/Drawing/DetectDrawing.cs` | 撮影 → 解析 → 枠(ラベル + 信頼度)/ タグのパネル | Setting の AI EndPoint / Key が必要。Tag は日本語のタグ、Face は Face API のあるリソースのみ(無ければ 401 のダイアログ) |
| `Modules/Sample/SampleChatView.xaml` + `SampleChatViewModel.cs` | Ollama のストリーミング応答(未設定は疑似応答)、音声入力(端末の音声認識)と Ollama の抽出 | 冒頭のあいさつに動作モード、応答、音声フローの 4 ステップ |
| `State/Settings.cs` / `Modules/Main/SettingView.xaml` + `SettingViewModel.cs` | `OllamaEndPoint` / `OllamaModel`(QR のキー名も同じ) | Setting 画面の Ollama / Model の行 |
| `Document/Sample_CvNet_Tag.png` | 画像 | README |

- [ ] **0-6** 上記(端末には Foundry の EndPoint / Key と Ollama(`http://localhost:11434` / `gemma2`)を設定済み。不要なら Setting の QR で上書き)

### 0-7 画面録画

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Modules/Device/DeviceMiscView.xaml` + `DeviceMiscViewModel.cs`(Recording カード)/ `MauiProgram.cs`(`UseScreenRecording`)/ `Platforms/Android/AndroidManifest.xml`(前景サービス + `FOREGROUND_SERVICE` / `FOREGROUND_SERVICE_MEDIA_PROJECTION`) | `Plugin.Maui.ScreenRecording` | Start → 共有ダイアログ(画面全体を共有)→ 録画中 → Stop で mp4 のパス |

- [ ] **0-7** 上記

### 0-8 ローカル通知

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Components/NotificationService.cs` + `NotificationService.android.cs` | 即時(アクションボタン + ペイロード)/ 予約(AlarmManager + レシーバ)/ 取消 / タップの受け口 | — |
| `Platforms/Android/MainActivity.cs` / `MainPageViewModel.cs` | 起動 / 再起動 Intent からタップを拾う、初期遷移後にトースト | 通知のタップでアプリが前面になりトースト |
| `Modules/Device/DeviceMiscView.xaml` + `DeviceMiscViewModel.cs`(Notification カード)/ `Permissions.cs` / `Platforms/Android/AndroidManifest.xml`(`POST_NOTIFICATIONS` / `SCHEDULE_EXACT_ALARM`)/ `Extensions.cs` / `MauiProgram.cs` | UI と権限、`TappedAsObservable`、DI | Notify → シェードの本体 / 承認 / 却下 → カードにタップ結果、Schedule → 10 秒前後で通知、Cancel、Exact alarm で設定画面 |

- [ ] **0-8** 上記

### 0-9 ドキュメント

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Document/Change_Summary.md` | 区間 10 のエントリ(Control メニュー / Grid・Card list / WiFi / Bottom sheet・Drawer / 診断・リーク / Azure・Ollama / 画面録画 / ローカル通知)とナレッジ、付録D 不採用 (1) の追記 | 記述と実装の一致 |
| `Document/Task_Checklist.md` / `README.md` | 残項目(1 SCP / 2 節 / 3 節)、TODO / Implement / 画像 | README の TODO とサマリ表の同期、画像リンク切れなし |

- [ ] **0-9** 上記

### 0-10 プッシュ通知の自前サンプル(別フォルダ `Works3/PushSample`)

本アプリのコードは無変更。構成 / 実行手順 / 再接続の設計 / 確認済みの動作 / 解析上の制約はすべて `Works3/PushSample/README.md`。

- [ ] **0-10** `Works3/PushSample`(README に沿って確認。`PushHub` の CA1812 抑止と `CommunityToolkit.Mvvm` の採用を含む)

### 0-11 ネットワーク(対向サーバー = template-maui-server)

対向サーバーは `D:\GitHubTemplate\template-maui-server`(別リポジトリ、こちらも未コミット)。起動と端末の接続は `Document/Development.md`「サーバー処理」。サーバー側のパスは `template-maui-server/src/Template.MobileServer.Web/` からの相対((server) 印)。

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Modules/Network/NetworkMenuView.xaml` + `NetworkMenuViewModel.cs` | Network メニュー | Download \| Upload の行が HTTP \| Storage に、Realtime \| gRPC の 2 列、SCP \| 空き。単発ボタン(server time / data list / secure / login / logout / error / delay)は残置 |
| `Modules/Network/NetworkHttpView.xaml` + `NetworkHttpViewModel.cs` | Web API(Data の CRUD) | ログイン状態(Id / 有効期限)、一覧(20 件ずつ追加読み込み)、行選択で詳細、作成 / 更新 / 削除 / クリア、未ログインの作成は 401、重複は 409、10 秒待つ API とキャンセル、ログ。F2 = Reload |
| `Modules/Network/NetworkStorageView.xaml` + `.xaml.cs` + `NetworkStorageViewModel.cs` | ストレージ(簡易 FTP API) | ディレクトリの一覧 / 下階層 / 上へ、ファイル(FilePicker)と写真(MediaPicker)のアップロード、ダウンロード(公開フォルダ)、削除(確認ダイアログ)、進捗とキャンセル。F2 = Reload |
| `Modules/Network/NetworkRealtimeView.xaml` + `NetworkRealtimeViewModel.cs` | SignalR(MonitorHub) | 遷移時に接続 / 離脱時に切断、状態 / 接続 ID / サーバー時刻、サーバーの CPU / メモリ / 接続数のグラフ、端末の状態を 10 秒ごとに送信、通知の一覧(前面 = トースト、バックグラウンド = ローカル通知)。F2 = Connect |
| `Modules/Network/NetworkGrpcView.xaml` + `NetworkGrpcViewModel.cs` | gRPC(チャット) | 接続先 / 状態 / 未配送数、単項 RPC(サーバー時刻)、チャット(管理画面 `/chat` と相互)、切断中の送信は再接続後に配送。F2 = Connect |
| `Modules/Network/NetworkScpViewModel.cs` | SCP | 転送を BusyState の外で実行(キャンセル可能に) |
| `Services/HttpService.cs` / `ApiContext.cs` / `Log.cs`(新規) | API 呼び出し、認証状態(`LoginId` / `TokenExpires`)、通信系ログ | PUT / DELETE は `HttpClient` を `RestResponse` に包む。ログは `Services/Log.cs` に分離 |
| `Services/MonitorConnection.cs`(新規) | SignalR の常時接続 | 初回接続のバックオフ再試行 / 自動再接続 / Closed 後のやり直し / ネットワーク復帰 / 停止 |
| `Services/Chat/ChatClient.cs` + `ChatConnectionState.cs` / `ChatMessageEntry.cs` / `ChatMessageEventArgs.cs` / `ChatStateEventArgs.cs` / `chat.proto` / `server.proto` | gRPC チャット(サーバーの WPF サンプルの移植)と proto のコピー | 指数バックオフ再接続、送信キュー、トークンは接続ごとに取得 |
| `Usecase/NetworkOperator.cs` / `NetworkUsecase.cs` | 401 の再ログイン再送、CRUD / ストレージ / 遅延のユースケース、常時接続用の `EnsureLoginAsync` / `GetTokenAsync` | 再ログインは 1 回だけ、`ExecuteTransfer` はインジケーターなし |
| `Models/Api/DataListResponse.cs`(`Id` long / `Total`)/ `DataResponse.cs` / `DataCreateRequest.cs` / `DataCreateResponse.cs` / `DataUpdateRequest.cs` / `StorageListResponse.cs` / `MonitorMessages.cs` | 契約 DTO | サーバー側と同じ形 |
| `Helpers/JwtHelper.cs`(新規)/ `Helpers/ReactiveSignalR.cs`(削除) | JWT の有効期限の取り出し、未使用ヘルパの削除 | — |
| `State/Settings.cs` / `Modules/Main/SettingView.xaml` + `SettingViewModel.cs` | `MonitorEndPoint` → `GrpcEndPoint`(QR キー同名)、Setting の gRPC 行、QR 読み取り後の表示更新 | Setting 画面の Network セクション |
| `State/Session.cs` / `App.xaml.cs` | 前面かどうか(`IsForeground`)を Window の Resumed / Stopped で更新 | — |
| `MauiProgram.cs` / `Modules/ViewId.cs` / `Markup/AppIcons.cs` / `Template.MobileApp.csproj` | Rester の JSON を PascalCase(大文字小文字を区別しない)に、DI、`NetworkStorage`、アイコン(`Http` / `FolderOpen` / `Hub`)、proto の参照 | ビルド |
| `Document/Development.md` | 「サーバー処理」 | 起動 / ポート / `adb reverse` / QR の手順 |
| (server) `Components/Pages/QrPage.razor(.cs)` / `Settings/ClientSetting.cs` / `appsettings.json` | 設定 QR(全キー、接続先以外は `Client` セクションの初期値) | `/qr` の内容(キーは環境変数か user-secrets) |
| (server) `Endpoints/DataEndpoints.cs` / `Models/Api/DataListResponse.cs` / `Core/Services/DataService.cs` / `Core/Models/RangeResult.cs` | 一覧の範囲取得(`offset` / `size`、`Total`) | 省略時は全件 |
| (server) `Hubs/MonitorHub.cs` / `Infrastructure/Monitor/DeviceRegistry.cs` / `DeviceEntry.cs` / `MonitorNotifier.cs` / `Models/Api/MonitorMessages.cs` / `Workers/ServerStatusWorker.cs` / `NotificationRelayWorker.cs` / `Application/ApplicationExtensions.cs` / `Program.cs` / `Application/Log.cs` | SignalR ハブと状態配信 / 通知 | JWT(`access_token` クエリも可)、KeepAlive 15 秒 / ClientTimeout 30 秒 |
| (server) `Components/Pages/DevicesPage.razor(.cs)` / `Layout/NavMenu.razor` / `Pages/Home.razor(.cs)` / `wwwroot/css/app.css` | 管理画面の Devices(端末一覧と通知の送信)、Home の接続数 | — |
| (server) `Protos/server.proto` / `Handlers/ServerInfoHandler.cs` | 単項 RPC(サーバー時刻、匿名) | — |
| (server) `appsettings.Development.json` / `Assembly.cs` | JWT 有効期限 5 分(開発)、テストへの `InternalsVisibleTo` | — |
| (server) `tests/.../DeviceRegistryTests.cs` / `DevicesPageTests.cs` / `QrPageTests.cs` / `NavMenuTests.cs` / `README.md` | テスト(33 件)と README(API 一覧 / SignalR / QR の書式) | — |

- [ ] **0-11** 上記(実機確認は `Change_Summary.md` の「ネットワーク実装」参照)

### 0-12 .NET 10 API の適用 / IChatClient 抽象化 / ドキュメント

| 現在のファイル名 | 何用か | 確認観点 |
| --- | --- | --- |
| `Modules/Basic/BasicSettingView.xaml` | `SearchBar.SearchIconColor` / `ReturnType`、`Switch.OffColor` | 検索アイコンが青、Switch オフ時の色 |
| `Modules/Device/DeviceMiscViewModel.cs` | `IVibration.IsSupported` / `IHapticFeedback.IsSupported` でボタンの有効化 | Pixel では有効のまま |
| `Modules/Device/DeviceLocationViewModel.cs` + `DeviceLocationView.xaml` | `IGeolocation.IsEnabled` で空状態に「Location service is disabled」 | 位置情報サービスを切ると表示 |
| `Services/AiChatClientFactory.cs`(新規)/ `Modules/Sample/SampleChatViewModel.cs` / `MauiProgram.cs` | チャットの依存を `IChatClient`(`Microsoft.Extensions.AI`)に。履歴は VM が保持、抽出は `GetResponseAsync` | Ollama で応答と 2 回目の文脈、未設定時の疑似応答 |
| `Document/Development.md` | `dotnet run --device <シリアル>` の手順 | — |
| `Document/Other_App_Candidates.md`(新規) | 本サンプルでは対象外、別アプリで導入を検討する項目(ディープリンク) | — |
| `Document/Telemetry_Study.md`(新規) | クラッシュレポート / テレメトリ基盤の検討資料(DeviceManager 型 / OpenTelemetry / 外部サービス、Aspire の位置付け) | 別セッションでの検討の入力 |

- [ ] **0-12** 上記

### 0-13 OpenTelemetry の組み込みサンプル(別フォルダ `Works3/OtelSample`)

本アプリのコードは無変更。構成 / 実行手順 / 送信の設計(OTLP/HTTP、ILogger の転送、ディスク退避と再送、クラッシュ、MAUI のレイアウト計測)/ 確認済みの動作 / 解析上の制約 / ナレッジはすべて `Works3/OtelSample/README.md`。`Document/Telemetry_Study.md` の候補 B の実証。

- [ ] **0-13** `Works3/OtelSample`(README に沿って確認。`EventSourceSupport=true`、`AddMetrics`、`AddView` によるタグの集約、`OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY=disk` の採用を含む)

---

## 1. SCP の実機確認と転送実テスト(要 SSH サーバ)

sshd は対向サーバー(template-maui-server、4 節)に含まれない。

- [ ] **1-1** Network メニュー: 「SCP」が追加され遷移できる(未設定時は接続先が「未設定 (設定画面の QR で投入)」でボタン無効)
- [ ] **1-2** Main > Setting: 項目の**ラベルと現在値が横並び**で表示される。SCP セクション(Host/User/Password)があり、QR(`ScpHost=...` 形式)を読むと反映される
- [ ] **1-3** Network > SCP: QR 投入後、「アップロード」でファイル選択 → 進捗バー → 完了ログ。「ダウンロード」で同ファイルがキャッシュへ取得される。転送中「キャンセル」で中断。接続後にサーバのホスト鍵指紋が参考表示される(照合は行わない)

---

## 2.【優先】`tmpl-plan-maui.md` からの移管課題

`D:\GitHubTemplate\tmpl-plan-maui.md`(MAUI トラック強化プラン)の未対応項目(同書の番号を併記)。**keyboard / blazor 向けの対応(同書 §4 / §5)と iOS 対応(同書 3-13。保留継続)は対象外**。
参照サンプルは `C:\Users\machi\Desktop\Maui`(残置 18 件)。ファイルパスは `Template.MobileApp/` からの相対。

### 機能実装(同書 §3)

2-1 は画面・`ViewId`・メニューボタンが配置済み(`DeviceMenuView.xaml` の該当ボタンが `IsEnabled="False"`、画面は `Not implemented` 表示、ViewModel は 8 行)。プラットフォーム実装は `Components/WiFiManager.cs` + `WiFiManager.android.cs` と同じ構成(共通インターフェース + `*.android.cs`)に揃える。

#### 2-1 生体認証(同書 3-2)

参照: `Bio`(`Maui.Biometric-main` / `MauiBiometricPluginSample-main` / `NET-MAUI-FingerPrint-main`)

| 種別 | URL | 概要 | 適用先 |
| --- | --- | --- | --- |
| ライブラリ | https://github.com/oscoreio/Maui.Biometric | `Plugin.Fingerprint` の後継。`IBiometricAuthentication.CheckAvailabilityAsync` が `AvailabilityResult`(`AuthenticationAvailability`: NoSensor / NoBiometric / TemporaryUnavailable / NoPermission / NotSupported 等 + 検出した `BiometricSensor` の集合)を返し、`AuthenticateAsync(new AuthenticationRequest(title, reason) { Authenticators = Biometric \| DeviceCredential, ConfirmationRequired })` が `AuthenticationResult` を返す。Android 実装は `AndroidX.Biometric.BiometricPrompt`。`.UseBiometricAuthentication()` で DI 登録。v2.5.1(2026-03) | 案B の候補。可用性 3 区分は `NoSensor` / `NoBiometric` / `TemporaryUnavailable` が対応。`sample/MainViewModel.cs` が可用性表示 + 認証 + 結果表示の最小例 |
| 公式 | https://developer.android.com/training/sign-in/biometric-auth?hl=ja | 「生体認証ダイアログを表示する」。`BiometricManager.canAuthenticate` による可用性判定(`BIOMETRIC_SUCCESS` / `ERROR_NO_HARDWARE` / `ERROR_NONE_ENROLLED` / `ERROR_HW_UNAVAILABLE`)、`BiometricPrompt.PromptInfo` の組み立て、認証コールバック、`CryptoObject` | 案A の一次資料。可用性 3 区分は `canAuthenticate` の戻り値をそのまま対応付ける |

- [ ] **2-1-0**【判断】実装方式 — 案A `Components/Biometric.cs` + `.android.cs` を自作(`Xamarin.AndroidX.Biometric` を追加)/ 案B `Maui.Biometric` パッケージを参照

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

- [ ] **2-1** 可用性は「ハードウェア無し / 未登録 / 一時利用不可」を区別して表示する。範囲は認証成否の表示まで(鍵の解錠に使う `CryptoObject` は対象外)
  - 制約: `androidx.biometric` は `androidx.fragment` に依存する。csproj は `Xamarin.AndroidX.Fragment.Ktx` をピン止めしているため、追加後に `dotnet list package --include-transitive` で競合を確認する
  - `BiometricPrompt` が要求する `FragmentActivity` は `MainActivity`(`MauiAppCompatActivity` 派生)で満たしている。基底クラスの変更は不要

#### 2-2 プッシュ通知 FCM(同書 3-4)

参照: `0_maui-samples/10.0/WebServices/PushNotificationsDemo`(`Xamarin.Firebase.Messaging` + 自前 `FirebaseMessagingService`)。ローカル通知は `Components/NotificationService.cs` + `.android.cs` で実装済み(即時 / スケジュール / アクションボタン / タップ時ペイロード。Device > Misc の Notification カード)。FCM を使わない自前配信(SignalR + 前景サービス)は `Works3/PushSample`(同フォルダの README)。

| 種別 | URL | 概要 | 適用先 |
| --- | --- | --- | --- |
| 記事 | https://www.andreasnesheim.no/push-notifications-in-net-maui-with-firebase/ | FCM を `Plugin.Firebase` で扱う手順。Firebase Console 登録 → `google-services.json` 配置 → `MauiProgram` 初期化 → `CrossFirebaseCloudMessaging.Current.GetTokenAsync()` でトークン取得 → Console からテスト送信(2022-09) | 採用時の方式候補①(`Plugin.Firebase`)。方式候補②は上記 PushNotificationsDemo |
| ライブラリ | https://github.com/Gekidoku/BetterFireBaseNotificationsPlugin | `CrossFirebasePlugin` の MAUI 移植。データ付き通知 / アクションボタン / サイレント通知 / アプリ終了時の受信に対応。NuGet 配布はなくプロジェクト参照前提(2026-01) | 実装範囲(データ / アクション / サイレント)のチェックリスト |

- [ ] **2-2-0**【判断】プッシュ通知(FCM)の要否 — Firebase プロジェクトと `google-services.json` が前提。採用する場合、トークン表示と受信ログを Notification カードに追記する(低優先)

---

## 3. リンク集からの取り込み候補

`■MAUI.txd`(リンク集)は全件に判定を付記済み(🟩 取り込む / 🟦 取り込まないが記事として有用 / 🟥 古い・参照不要 / 🟨 要判断)。本節へ移した項目の元行は同書から削除している。本節は取り込む価値のあるトピックだけ。ファイルパスは `Template.MobileApp/` からの相対。

### 3-2 .NET 10 の未適用 API の反映

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

- [ ] **3-2-1** Basic > Setting(Picker の Open / Close、DatePicker / TimePicker の null)
- [ ] **3-2-2** Control > Refresh(`IsRefreshEnabled`)
- [ ] **3-2-3** Device > Misc(`SpeechOptions.Rate`。MauiComponents 側の変更を含む)
- [ ] **3-2-4** Sample > HybridWebView / Web view

### 3-3 レイアウト診断メトリクス(DiagnosticPanel)

参照: https://learn.microsoft.com/ja-jp/dotnet/maui/whats-new/dotnet-10?view=net-maui-10.0(「Diagnostics」)、https://github.com/dotnet/maui/pull/31058 — `ActivitySource` / `Meter` 名 `"Microsoft.Maui"` で `IView.Measure` / `Arrange` の回数と所要時間(`maui.layout.measure_count` / `measure_duration` / `arrange_count` / `arrange_duration`)を記録する。`System.Diagnostics.Metrics.Meter.IsSupported` のフィーチャースイッチで AOT / トリミング時に無効化できる。

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Shell/DiagnosticPanel.xaml` + `.xaml.cs` | メモリ推移のスパークライン(DEBUG 限定) | `MeterListener` で measure / arrange の回数・平均時間を購読し、メモリの横に表示 |
| `Template.MobileApp.csproj` | ビルド設定 | Release で `Meter.IsSupported` が無効であることを確認(必要なら `RuntimeHostConfigurationOption` で false) |

- [ ] **3-3** 上記。画面遷移で回数が増え、静止時は増えないことを確認

### 3-4 XAML の global xmlns 化

参照: https://learn.microsoft.com/ja-jp/dotnet/maui/whats-new/dotnet-10?view=net-maui-10.0(「Implicit and Global XML namespaces」)— `GlobalXmlns.cs` に `[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "Template.MobileApp.Controls")]` 等を並べ、XAML 側は `xmlns="http://schemas.microsoft.com/dotnet/maui/global"` の 1 行で `controls:` / `behaviors:` / `converters:` 等の接頭辞を省略できる。`XmlnsPrefix` で接頭辞を残す運用も可。ルートの `xmlns` / `xmlns:x` まで省く implicit 版は `MauiAllowImplicitXmlnsDeclaration` + `EnablePreviewFeatures` のプレビュー機能。

- [ ] **3-4-0**【判断】適用範囲 — 案A `GlobalXmlns.cs` の追加のみ(既存 XAML は変更不要、新規画面から接頭辞を省略)/ 案B 全 XAML(約 120 ファイル)の接頭辞を一括で除去 / 見送り。implicit 版(プレビュー)は対象外

### 3-5 StyleClass の活用(文字サイズ × 配置)

方針(付録A): スタイルの基本は共有 `Styles.xaml` からの `BasedOn` 派生。`StyleClass` は複数指定できる利点を、文字サイズ × 配置のように**直交する属性の組み合わせ**にだけ使い、サイズ × 配置の数だけスタイルを用意しなくて済むようにする。色や余白などは Style 側に置き、同じプロパティを Style と StyleClass の両方で指定しない。Crosswind(https://github.com/sthewissen/Plugin.Maui.Crosswind)のような全面的なユーティリティクラスは採用しない。

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- | 
| `Resources/Styles/StyleClasses.xaml` | — | 新規。`Label` 向けのクラス(`Class` 付き Style)を定義: サイズ `size-10` / `size-11` / `size-12` / `size-14` / `size-16` / `size-18` / `size-20` / `size-24` / `size-28` / `size-36` / `size-48`(付録A の許可値のうち使用中のもの)、水平配置 `align-start` / `align-center` / `align-end`、太字 `bold` |
| `App.xaml` | リソース辞書のマージ | `StyleClasses.xaml` を追加(共有 `Styles.xaml` は変更しない) |
| `Modules/Network/NetworkHttpView.xaml` / `NetworkStorageView.xaml` / `NetworkRealtimeView.xaml` / `NetworkGrpcView.xaml` / `NetworkScpView.xaml` | 第 1 弾の適用先 | 画面ローカルの `CaptionLabel` / `ValueLabel` / `LogLabel` 等のうち FontSize だけを持つものを `StyleClass="size-12"` 等に置き換え(色を持つものは BasedOn 派生に残しサイズだけクラスへ) |
| `Document/Change_Summary.md` 付録A | 開発ポリシー | 上記の方針を追記(3-5-1 で実施) |

- [ ] **3-5-1** クラスの定義と `App.xaml` へのマージ、付録A への方針の追記
- [ ] **3-5-2** Network の 5 画面へ適用し、表示が変わらないことを実機で確認
- [ ] **3-5-3** 既存画面は触るときに適用する(一括変更はしない)。Style と StyleClass の同一プロパティ指定が無いことを inspectcode / 目視で確認

### 3-6 Face 識別(LargeFaceList)

参照: https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/face/Azure.AI.Vision.Face/samples/Sample6_LargeFaceListAsync.md — `LargeFaceListClient` で顔リストを作成 → 顔画像を追加 → 学習 → `FindSimilarAsync` で類似顔を検索する `Azure.AI.Vision.Face` の公式サンプル。

- [ ] **3-6-0**【判断】要否 — `SampleCvNetFace` は顔検出(`DetectAsync`)まで。識別(Identify / Verify / FindSimilar)は Azure の Limited Access 申請が承認されたサブスクリプションと Face API のあるリソースが必要(現在の Foundry リソースは Face が 401)。承認済みの場合のみ、登録画面(顔リストへの追加)と照合画面を追加

### 3-10 Material 3(`UseMaterial3`)

参照: https://devblogs.microsoft.com/dotnet/dotnet-maui-material-3/ — Android の Material 3(Material You)対応(2026-05)。`Microsoft.Maui.Controls` 10.0.60 以降(本プロジェクトは 10.0.100)で csproj に `<UseMaterial3>true</UseMaterial3>` を置くだけで有効(既定 false。Handler / `styles.xml` の変更は不要)。対象は Entry / Editor / SearchBar / RadioButton / ProgressBar / Slider / Picker / TimePicker / DatePicker / CheckBox / Switch / ImageButton / Button / Shell の既定外観(Entry / Editor は outlined の `TextInputLayout`、DatePicker はカレンダー ダイアログ)。XAML / C# で明示した色・スタイルは優先される。未対応: コントロール単位の opt-in、動的カラー トークンの API、NavigationPage / TabbedPage / FlyoutPage / CollectionView / Border の再スタイル。

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Template.MobileApp.csproj` | ビルド設定 | `<UseMaterial3>true</UseMaterial3>` を追加 |
| `Behaviors/EntryOption.android.cs` | Entry / Editor の NoBorder(`BackgroundTintList`)/ フォーカス枠 | outlined `TextInputLayout` 化との干渉を確認(枠線が二重になる場合は NoBorder の実装を Material 3 用に切替) |
| `Resources/Styles/Styles.xaml` | 共有スタイル | 変更しない(明示指定が優先されるため差分は既定外観のみ) |

- [ ] **3-10-0**【判断】採否 — 有効化は 1 行だが Android の入力系コントロールの既定外観が全画面で変わる(Basic / Setting / Kit 系の Entry・Switch・DatePicker が主な影響範囲)。採用時は全画面の目視確認と `Document/*.png` の撮り直しが必要

### 3-12 バックグラウンド定期タスク(WorkManager)

参照: https://www.nuget.org/packages/Shaunebu.MAUI.BackgroundTaskManager — Android は `WorkManager`(最短 15 分間隔)、iOS は `BGTaskScheduler` を使い、CRON 式でフォアグラウンド / バックグラウンドのジョブを登録して `Preferences` に永続化する薄いラッパー(`RegisterJob<T>()` / `Schedule(jobId, cron, callback)` / `ScheduleInBackground(jobId, cron, jobType)`。2025-10、230 DL、リポジトリ公開なし)。採用はせず API の形(ジョブ登録 / CRON 近似 / 永続化)だけ参考にし、`AndroidX.Work` を直接使う。

範囲: 制約付き(ネットワーク接続時 / 充電中)の一回限りワーク + 15 分周期の定期ワーク。実行結果はローカル通知(`Components/NotificationService`)か画面のログに出す。正確な時刻指定は `AlarmManager`(通知で実装済み)の担当のままとし、WorkManager は遅延可・再起動後も残る処理に限定する。

| 現在のファイル名 | 何用か | 変更 |
| --- | --- | --- |
| `Template.MobileApp.csproj` | パッケージ | `Xamarin.AndroidX.Work.Runtime` を追加(`dotnet list package --include-transitive` で `Fragment.Ktx` ピン止めとの競合を確認) |
| `Components/BackgroundTask.cs` + `.android.cs` | — | 新規。登録(一回 / 定期)/ 取消 / 状態取得。`AndroidX.Work.Worker` 派生の `DoWork()` で処理 |
| `Modules/Device/DeviceMiscView.xaml` + `DeviceMiscViewModel.cs` | 雑多なデバイス機能(Device メニューは満杯) | `InfoCard` を追加(登録 / 取消 / 最終実行時刻) |
| `MauiProgram.cs` | `ConfigureComponents` | DI 登録 |

- [ ] **3-12-0**【判断】要否 — 再起動後も残る遅延処理(同期 / 送信キュー)の需要があるか。WorkManager は再起動後に自動で再スケジュールされるため `RECEIVE_BOOT_COMPLETED` は不要。常駐(前景サービス)は対象外

