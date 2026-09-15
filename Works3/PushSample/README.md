# PushSample — MAUI + SignalR による自前プッシュ通知の最小サンプル

FCM を使わずに、サーバからの通知を端末へ届ける最小構成。クライアントは前景サービスで SignalR の常時接続を保ち、受信した内容をローカル通知で表示する。

| プロジェクト | 内容 |
| --- | --- |
| `PushShared` | 通知のデータ (`PushMessage`) とハブの接続先 / メソッド名 |
| `PushServer` | ASP.NET Core (Minimal API) + SignalR ハブ。`POST /push` で接続中の全クライアントへ配信 |
| `PushClient` | .NET MAUI (Android)。前景サービス + `HubConnection` の常時接続 + ローカル通知 |

## 実行

```bash
# サーバ (http://localhost:5000)
dotnet run --project PushServer
# LAN の端末から繋ぐ場合
dotnet run --project PushServer --urls http://0.0.0.0:5000

# クライアント (USB の端末。adb reverse で端末の localhost:5000 を PC へ転送)
adb reverse tcp:5000 tcp:5000
dotnet build PushClient -f net10.0-android -t:Run

# 配信
curl -X POST http://localhost:5000/push -H "Content-Type: application/json" -d "{\"title\":\"件名\",\"body\":\"本文\"}"
```

アプリの「開始 (常駐)」で前景サービスが起動し、常駐通知に接続の状態(接続中 / 再接続中 / 接続を試みています)が出る。「テスト送信」は自分から `POST /push` を呼ぶ。

## 再接続とエラー処理

- 初回接続の失敗(サーバ停止 / ネットワーク無し / タイムアウト)は `PushConnection` が 0 → 2 → 5 → 10 → 30 → 60 秒のバックオフで無期限に再試行する。SignalR の自動再接続は接続確立後の切断だけが対象のため、ここは自前
- 接続後の切断は `WithAutomaticReconnect`(同じ間隔で無期限)に任せる。それでも `Closed` になった場合(サーバ側から閉じられた等)は初回接続からやり直す
- キープアライブ 15 秒 / タイムアウト 30 秒をサーバとクライアントで対にし、無音の切断を 30 秒以内に検知する
- ネットワークの復帰(`Connectivity.ConnectivityChanged`)でバックオフの待ちを打ち切り、すぐに再試行する
- 停止は `CancellationToken` で行い、`StopAsync` は接続ループの終了まで待つ。`StartAsync` は同じ接続先なら何もせず、違う接続先なら繋ぎ直す
- 受信 / 状態変更のイベントはバックグラウンドスレッドから発火するため、UI は `MainThread` へマーシャリングする
- 前景サービスは `START_STICKY`。強制終了後の再起動では Intent が null のため、接続先は `Preferences` から読む。Android 12 以降でバックグラウンドからの前景サービス起動が拒否された場合(`IllegalStateException`)は諦めて停止する

## FCM との違い(制約)

- アプリのプロセスが生きている間しか受信できない。常駐通知(前景サービス)が必須で、電池を消費する。Doze / App Standby / メーカー独自の省電力で接続が切れることがあるため、「電池の最適化」からアプリを除外してもらう前提
- Android 14 以降の前景サービス種別は `specialUse`(用途の宣言が必要。Google Play で配布する場合は審査の対象)。`dataSync` は Android 15 で 1 日 6 時間の上限があるため常時接続には使えない
- サーバはハブへの接続を維持する分のリソースを使う。端末側の識別(ユーザー / 端末ごとの配信)、認証(`AccessTokenProvider`)、TLS、未達メッセージの再送は本サンプルには含めない
