# 📱端末情報の取得の計画

端末とプロセスの情報(電池・通信・無線 LAN の状態、CPU / メモリ / スレッド / GC)を `Components/DeviceInformation` で取得する。変化する状態は Android の通知で受けて保持し、変化をイベントで知らせる。画面(`DeviceState`・診断パネル)、テレメトリ(`Diagnostics`)、通信の再接続は、このクラスの値を読むか、イベントを受けるだけにする。

## 📋決定事項

| 項目 | 決定 |
| --- | --- |
| 状態の取得 | 自前の受信(`ACTION_BATTERY_CHANGED` の受信と `NetworkCallback`)。通知に含まれる値を読み、システムへ問い合わせ直さない |
| クラス | `Components/DeviceInformation` 1 つ。Android の部分だけを partial(`.android.cs`)で分け、機能(Control / Identity / Battery / Network / Process)ごとに区切りのコメントを入れる |
| 役割 | 端末とプロセスの情報の取得まで(読む・保持する・通知する)。スレッドは扱わない(通知は受けたスレッドで出す)。スレッドの切り替えと値のキャッシュ(ロック)は使う側で行う |
| 開始・停止 | 起動処理(`MauiProgram`)で開始し、プロセスのあいだ動かす(背面でもメトリクスを送るため) |
| 診断パネル | スナップショット(1 秒ごとの計算・判定・推移)は `Shell` で管理する(`Shell/DiagnosticSampler`。開始・停止は `MainPageViewModel`)。FPS は MauiComponents の `IDisplay` を直接使う(`DeviceInformation` では包まない) |
| `Diagnostics` 名前空間 | 残す(OTEL の送信と計器、クラッシュ、直近のログ) |
| 無線 LAN | 信号強度を送る(`application.wifi.signal_strength`、dBm)。リンク速度も一緒に取得する(送らない) |

## 📏通知 1 回の処理(実機)

Pixel 9a、USB 充電中、4 分間。自前 = 通知に含まれる値を読む処理。MAUI = 同じ通知で MAUI が行う読み直し(`IBattery` の残量・状態・電源、`IConnectivity` の接続の状態と種類)。

| 区分 | 自前 | MAUI | 通知の回数 |
| --- | --- | --- | --- |
| 電池 | 0.09〜0.72 ms(UI スレッド)| 3.1〜14.0 ms(UI スレッド)| 5 回(充電中と充電停止の切り替えごと)|
| 通信 | 0.6〜1.3 ms(通信のスレッド)| 4.8〜23.1 ms(UI スレッド)| 能力の変化 3 回(無線 LAN の信号の変化を含む)|

- 登録は 1 回だけ: 電池の受信 2.5 ms、`NetworkCallback` 0.8 ms
- 位置情報の権限と `IncludeLocationInfo` なしで、無線 LAN の信号強度(-49〜-37 dBm)、リンク速度(156 Mbit/s)、周波数(2432 MHz)が読める
- テレメトリは保持した値を読むだけなので、送信の読み取りの重さは問題にならない

## ⚙️MAUI の処理が重い理由(実装)

- `IBattery`: 受信した Intent の値を使わず、受信のたびに `ChargeLevel` / `State` / `PowerSource` を読む。3 つとも、読むたびに `RegisterReceiver(null, ACTION_BATTERY_CHANGED)` でシステムへ問い合わせる(プロセス間の呼び出しが 3 回)
- `IConnectivity`: `NetworkCallback` で届いた能力を使わず、通知のたびにアプリ内へブロードキャストを送り、受信側で `NetworkAccess` と `ConnectionProfiles` を読む。どちらも読むたびに `GetAllNetworks()` と、ネットワークごとの `GetNetworkCapabilities()` を呼ぶ(ネットワーク 2 つで、プロセス間の呼び出しが 6 回)

## 🧩構成

| 対象 | 内容 |
| --- | --- |
| `Components/DeviceInformation.cs` + `.android.cs` | Control(`Start` / `Stop`)/ Identity(`DeviceId`)/ Battery(`Battery` = 残量・状態・電源、`BatteryChanged`)/ Network(`Network` = 接続の状態と種類、`NetworkChanged`。`WiFi` = 信号強度とリンク速度、`WiFiChanged`)/ Process(`StartTime`、`ReadProcessStatistics()` = CPU 時間・常駐メモリ・スレッド数・GC 回数・割り当て量、`ReadHeapSize()`)。状態は通知のたびに不変の record で置き換え、変わったときだけイベントを出す |
| `State/DeviceState.cs` | 画面の表示用に残す。`DeviceInformation` のイベントを受けて(値は通知の時点で取り、UI スレッドへ移す)値を持つ(電池、接続の状態と種類、無線 LAN の信号強度 `WiFiSignalStrength`) |
| `Services/MonitorConnection.cs` | インターネットに接続できたこと(`NetworkChanged`)を再接続のきっかけにする |
| `Diagnostics/DiagnosticsInstrumentation.cs` | プロセスの値を `DeviceInformation` から読む(同じ集計の計器で 1 回の読み取りを共有する 500 ms のキャッシュをロックで持つ)。電池と無線 LAN は `DeviceInformation` の値 |
| `Shell/DiagnosticSampler.cs` | 診断パネルのスナップショット。プロセスの値、電池残量、無線 LAN の信号強度を `DeviceInformation` から読み、FPS(`IDisplay` のフレーム)と Measure / Arrange の回数は自身で計測する。パネルを表示していて前面にあるあいだだけ `MainPageViewModel` が動かす |
| `Modules/Main/DiagnosticsViewModel.cs` | 起動時刻を `DeviceInformation.StartTime` から読む |

- 接続の種類は、アプリの通信に使える(前面の)ネットワークだけ。Wi-Fi の接続中に裏で待機しているモバイル回線(`FOREGROUND` が無い)は `NetworkCallback` に届かない

## 🔢段階

| 番号 | 内容 | 状態 |
| --- | --- | --- |
| 1 | 状態(電池・通信・無線 LAN)、イベント、開始・停止。`DeviceState` / `MonitorConnection` / 計器をイベントと値の受け手にする | 完了(2026-09-25)。通信の変化での再接続は未確認 |
| 2 | プロセスの値(`StatisticsReader` を統合)と、無線 LAN の計器 | 完了(2026-09-25) |
| 3 | 診断パネルのスナップショット(FPS・Measure / Arrange の回数・判定・推移)を `Shell` で管理する(`TelemetryService` は送信だけにする) | 完了(2026-09-25) |
| 4 | `Diagnostics` 名前空間の整理(OTEL の送信と計器、クラッシュ、直近のログだけにする) | 完了(2026-09-25) |
