# PushSample — MAUI + SignalR による自前プッシュ通知の最小サンプル

FCM を使わずにサーバから端末へ通知を届ける最小構成。クライアントは Android の前景サービスで SignalR の常時接続を保ち、受信した内容をローカル通知と画面の一覧に出す。`Works3/Template` とは独立したソリューションで、同じ解析設定(`.editorconfig` / `Directory.Build.props` / `Analyzers.ruleset` / `.sln.DotSettings`)を使う。

| プロジェクト | 内容 |
| --- | --- |
| `PushShared` | 通知のデータ(`PushMessage` = Title / Body / SentAt)とハブのパス / メソッド名(`PushHubInfo`) |
| `PushServer` | ASP.NET Core Minimal API + SignalR ハブ。`POST /push` で接続中の全クライアントへ配信、`GET /` で接続数 |
| `PushClient` | .NET MAUI(Android のみ、`pushsample.client`)。前景サービス + `HubConnection` の常時接続 + ローカル通知 |

## ファイル構成

| ファイル | 何用か |
| --- | --- |
| `PushSample.slnx` / `.editorconfig` / `Directory.Build.props` / `Analyzers.ruleset` / `PushSample.sln.DotSettings` | ソリューションと解析設定(`Directory.Build.props` が `Analyzers.ruleset` を `CodeAnalysisRuleSet` で参照) |
| `PushShared/PushMessage.cs` | `PushMessage` レコードと `PushHubInfo`(`Path = "/hubs/push"`、`ReceiveMethod = "Receive"`) |
| `PushServer/Program.cs` | `AddSignalR`(KeepAlive 15 秒 / ClientTimeout 30 秒)、`MapHub`、`POST /push`(`title` 必須、空は 400)、`GET /` |
| `PushServer/PushHub.cs` | 接続の出入りを数えて記録するだけのハブ(配信は `IHubContext` から) |
| `PushServer/PushApi.cs` / `Log.cs` | 配信 API の要求 / 結果(JSON)、`LoggerMessage` |
| `PushServer/Properties/launchSettings.json` | `http://localhost:5000`(ブラウザは開かない) |
| `PushClient/Services/PushConnection.cs` | 常時接続の本体(接続 / 再試行 / 自動再接続 / 停止、状態と受信のイベント) |
| `PushClient/Services/INotifier.cs` / `IPushHost.cs` / `PushSettings.cs` | 通知、常駐(前景サービス)の抽象と設定キー |
| `PushClient/Platforms/Android/PushService.cs` | 前景サービス(`specialUse`、`START_STICKY`)。常駐通知に接続の状態を表示 |
| `PushClient/Platforms/Android/PushHost.cs` / `Notifier.cs` | サービスの起動 / 停止 / 電池の最適化の設定、通知 2 チャンネル(常駐 `push.status` / 受信 `push.message`) |
| `PushClient/Platforms/Android/AndroidManifest.xml` / `MainActivity.cs` / `MainApplication.cs` | サービスの宣言(`PROPERTY_SPECIAL_USE_FGS_SUBTYPE`)、権限、平文 HTTP の許可 |
| `PushClient/MainPage.xaml` + `MainViewModel.cs` | 接続先 / 開始(常駐)/ 停止 / テスト送信 / 電池の最適化 / 状態 / 接続 ID / エラー / 受信一覧(新しいものが上) |
| `PushClient/MauiProgram.cs` / `App.xaml(.cs)` / `PushApi.cs` / `Log.cs` / `GlobalUsing.cs` | DI(`PushConnection` と `MainViewModel` はシングルトン)、テスト送信の JSON(ソース生成)、`LoggerMessage` |

## 実行

```bash
# サーバ(http://localhost:5000)
dotnet run --project PushServer
# LAN の端末から繋ぐ場合
dotnet run --project PushServer --urls http://0.0.0.0:5000

# クライアント(USB の端末。adb reverse で端末の localhost:5000 を PC へ転送)
adb reverse tcp:5000 tcp:5000
dotnet build PushClient -f net10.0-android -t:Run

# 配信(Windows の cp932 コンソールで日本語を渡す場合は UTF-8 のファイルから --data-binary @file)
curl -X POST http://localhost:5000/push -H "Content-Type: application/json" -d "{\"title\":\"件名\",\"body\":\"本文\"}"
```

アプリの操作:

1. 接続先(既定 `http://localhost:5000`。LAN なら `http://<PC の IP>:5000`)を入力して「開始 (常駐)」。初回は通知の許可ダイアログ(拒否しても接続はするが常駐通知と受信通知は出ない)
2. 常駐通知「Push sample」に接続の状態(接続を開始しています / 接続中 / 再接続中 / 接続を試みています)が出る。画面には状態 / 接続 ID / 直近のエラー
3. 「テスト送信」は自分から `POST /push` を呼ぶ。受信は通知(チャンネル「受信通知」)と一覧に出る
4. 「停止」でサービスを止める(常駐通知が消える)。「電池の最適化」はシステムの設定画面を開く(常駐を殺されにくくするため、アプリを最適化の対象外にする)

## 再接続とエラー処理

| 局面 | 動作 |
| --- | --- |
| 初回接続の失敗(サーバ停止 / ネットワーク無し / タイムアウト) | `PushConnection.ConnectWithRetryAsync` が 0 → 2 → 5 → 10 → 30 → 60 秒(以降 60 秒)のバックオフで無期限に再試行。SignalR の自動再接続は接続確立後の切断だけが対象のため、ここは自前 |
| 接続後の切断 | `WithAutomaticReconnect(IRetryPolicy)` の無期限ポリシー(同じ間隔)。`Reconnecting` / `Reconnected` で状態を更新 |
| 自動再接続を諦めた / サーバから閉じられた(`Closed`) | `TaskCompletionSource` で待ち受け、初回接続からやり直す |
| 無音の切断の検知 | キープアライブ 15 秒 / タイムアウト 30 秒をサーバ(`KeepAliveInterval` / `ClientTimeoutInterval`)とクライアント(`KeepAliveInterval` / `ServerTimeout`)で対にし、30 秒以内に検知 |
| ネットワークの復帰 | `Connectivity.ConnectivityChanged`(Internet)でバックオフの待ち(`SemaphoreSlim`)を打ち切り、すぐに再試行 |
| 停止 | `CancellationToken` で接続ループを止め、`StopAsync` はループの終了(`HubConnection.DisposeAsync` まで)を待つ。`StartAsync` は同じ接続先で動作中なら何もせず、違う接続先なら繋ぎ直す(`SemaphoreSlim` で直列化) |
| 捕捉する例外 | `HttpRequestException` / `IOException` / `SocketException` / `WebSocketException` / `TimeoutException` / `OperationCanceledException`(トークン以外)/ `InvalidOperationException`。停止のキャンセルは `when (token.IsCancellationRequested)` で区別して再スロー |
| 前景サービスの起動拒否 | Android 12 以降のバックグラウンドからの起動制限(`ForegroundServiceStartNotAllowedException` = `Java.Lang.IllegalStateException`)は捕捉して `StopSelf` / `NotSticky` |
| 強制終了後の再起動 | `START_STICKY` で再起動されるが Intent が null のため、接続先は `Preferences` から読む |
| スレッド | 状態変更 / 受信のイベントはバックグラウンドスレッドから発火する。VM は `MainThread.BeginInvokeOnMainThread` で反映 |
| サーバ側 | `title` が空なら 400。配信結果は `sentAt` と接続数 |

## 確認済みの動作(Pixel 9a / Android 16、Debug ビルド)

1. 開始 → 通知の許可 → 「接続済み」。接続 ID はサーバログの `Client connected` と一致、常駐通知は「接続中」
2. テスト送信 → 受信通知(「テスト」)と一覧の先頭に反映、サーバログに `Push sent`
3. サーバを強制終了 → 「再接続中...」(エラー: `The remote party closed the WebSocket connection without completing the close handshake.`)→ サーバ再起動 → 新しい接続 ID で「接続済み」
4. HOME でバックグラウンドにして `POST /push` → 通知シェードに表示、アプリへ戻ると一覧に反映
5. 停止 → 「停止」、常駐通知が消え、サーバログに `Client disconnected`
6. サーバ停止中に開始 → 「接続中...」(エラー: `unexpected end of stream on com.android.okhttp.Address@…`)→ サーバ起動 → 12 秒以内に「接続済み」
7. `POST /push` の `title` 空 → 400
8. 電池の最適化 → システムの設定画面が開く

通知の確認は `adb shell dumpsys notification --noredact`(`android.title=` / `android.text=`)、接続数は `curl http://localhost:5000/`。

## 解析設定と実装上の制約

- ビルドはサーバ / クライアントとも Debug / Release で 0 警告、`jb inspectcode PushSample.slnx -f=xml -o=results.xml --no-build --no-swea --properties:Configuration=Release` は 0 件
- サーバは exe のため public 型が CA1515 になる。型は internal にし、SignalR がインスタンス化する `PushHub` は CA1812 を `#pragma warning disable` で抑止、配信 API の要求は `readonly record struct`。`Hub<T>` の `T` は動的プロキシ生成のため public が必要なので、型なしの `Hub` + `SendAsync(メソッド名, 引数)` で配信する
- クライアントの命名: `IPushHost` は `StartService` / `StopService`(CA1716 で `Stop` は不可)、接続先は `address`(CA1054 / CA1056 で string の `url` は不可)、受信イベントは `EventHandler<PushMessageEventArgs>`(CA1003)、ロックは `System.Threading.Lock`(IDE0330)。UI 側の `await` は `ConfigureAwait(true)` を明示
- `Platforms/Android` のファイルは IDE0130 を抑止して `PushClient.Services` 名前空間(`Android` を含む名前空間は `Android.*` と衝突する)。`App` は `Android.App` との衝突(CA1724)を抑止。`GlobalUsing.cs` は `#pragma warning disable`
- inspectcode 向け: `App` / `MainPage` の code-behind は基底型を書かない(XAML 側で決まる)、空クラスは `;` 本体、既定テンプレートの `Styles.xaml` にある `Shell` / `NavigationPage` / `TabbedPage` のスタイルは削除(`Xaml.RedundantPropertyTypeQualifier`)
- MVVM は `CommunityToolkit.Mvvm`(`[ObservableProperty]` の partial プロパティ + `[RelayCommand]`)

## ナレッジ

- **SignalR の再接続の範囲**: `WithAutomaticReconnect` は接続確立後の切断だけを扱う。初回の `StartAsync` の失敗と、再接続を諦めた後の `Closed` は呼び出し側で再試行する。`IRetryPolicy.NextRetryDelay` が null を返さない限り自動再接続は続く。`On<T>` は `IDisposable` を返す(購読は `using` で外す)
- **接続先が閉じているときの例外**: `adb reverse` 経由で PC 側のポートが閉じていると、接続はいったん受け付けられてから切れる(`IOException: unexpected end of stream`)。接続拒否(`SocketException`)だけを再試行対象にすると取りこぼす
- **前景サービスの種別(Android 14 以降)**: 常時接続は `specialUse`(`FOREGROUND_SERVICE_SPECIAL_USE` + manifest の `PROPERTY_SPECIAL_USE_FGS_SUBTYPE`。Google Play で配布する場合は用途の申告が必要)。`dataSync` は Android 15 で 1 日 6 時間の上限があり使えない。`StartForeground` は API 34 以降で種別付きのオーバーロードを使う
- **サービスと DI**: `PushConnection` は DI のシングルトンで、サービスのフィールドに持つと CA2213(未破棄)になる。所有せず `IPlatformApplication.Current.Services` から都度解決する
- **通知**: Android 13 以降は `POST_NOTIFICATIONS` の実行時許可(MAUI の `Permissions.PostNotifications`)。常駐通知は `SetOngoing` + `SetSilent` + `SetOnlyAlertOnce` で文言だけ差し替える。`NotificationCompat.Builder` のバインディングは `Set*` の戻り値が nullable なのでメソッドチェーンにせず 1 行ずつ呼ぶ
- **Windows のコンソールから日本語 JSON を送る**: `curl -d` の直書きは cp932 で化けるので UTF-8 のファイルを `--data-binary @file` で渡す

## FCM との違い(制約)

- アプリのプロセスが生きている間しか受信できない。常駐通知(前景サービス)が必須で、電池を消費する。Doze / App Standby / メーカー独自の省電力で接続が切れることがあるため、「電池の最適化」からアプリを除外してもらう前提
- iOS には同等の常駐手段が無い(APNs が必要)
- サーバは接続を維持する分のリソースを使う。多数の端末では Redis バックプレーン / Azure SignalR Service 等のスケールアウトが要る

## 本番で追加が必要なもの

- 認証: `WithUrl(..., o => o.AccessTokenProvider = ...)` でトークンを渡し、ハブに `[Authorize]`
- TLS: `https` にして manifest の `usesCleartextTraffic` を外す
- 宛先: ユーザー / 端末単位の配信(`IUserIdProvider` または `Groups`)
- 未達の補完: 再接続時に最終受信時刻以降のメッセージを API で取り直す(SignalR は再送しない)
