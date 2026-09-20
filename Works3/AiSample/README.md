# AiSample — Sample > CV Net / Chat を Windows で検証するコンソール

`Works3/Template` の Sample > CV Net(Azure AI Vision)と Sample > Chat(Ollama + 音声入力)と同じ処理を Windows のコンソールで実行する。アプリの `Usecase/AzureVisionUsecase.cs` は**そのままコピー**して使い、Chat はアプリの `SampleChatViewModel` と同じ処理(`OllamaApiClient` を `IChatClient` として使う)をコンソール向けに書き、カメラ / 画面の代わりに画像ファイルと標準入出力を担当する。`Works3/Template` とは独立したソリューションで、同じ解析設定(`.editorconfig` / `Directory.Build.props` / `Analyzers.ruleset` / `.sln.DotSettings`)を使う。

| プロジェクト | 内容 |
| --- | --- |
| `AiShared` | アプリからそのままコピーした `Usecase/AzureVisionUsecase.cs` と、それが使う `Settings` / `DetectResult` の最小の代替(`Settings` は ChatConsole の Ollama の設定も持つ)。クラスライブラリ(コピーは public のまま置ける) |
| `CvConsole` | Sample > CV Net の 4 画面(Object / Tag / People / Ocr)相当。画像ファイル → Azure AI Vision → 結果の一覧 + 枠とラベルを描いた PNG |
| `ChatConsole` | Sample > Chat 相当。Ollama(`IChatClient`)のストリーミング応答と、音声入力(Windows の音声認識で 1 回の発話を認識して入力欄へ反映) |

## ファイル構成

| 現在のファイル名 | 何用か |
| --- | --- |
| `AiSample.slnx` / `.editorconfig` / `Directory.Build.props` / `Analyzers.ruleset` / `AiSample.sln.DotSettings` / `.gitignore` | ソリューションと解析設定(Template のコピー)。`output/`(CvConsole の既定の出力先)は無視 |
| `AiShared/Usecase/AzureVisionUsecase.cs` | **アプリのコピー(無変更)**。Image Analysis 4.0(物体 / 人物 / タグ(ja)/ 文字) |
| `AiShared/State/Settings.cs` | アプリの `Settings` の代替。コピーと ChatConsole が使うメンバー(`AIServiceEndPoint` / `GetAIServiceKeyAsync` / `OllamaEndPoint` / `OllamaModel`)だけを同じ形で持ち、値は起動時に入れる |
| `AiShared/Usecase/DetectResult.cs` | アプリの `Usecase/DetectResult.cs` と同じ `DetectResult`(正規化した矩形 + 信頼度 + ラベル) |
| `AiShared/GlobalUsing.cs` / `AiShared.csproj` | コピーが前提にする global using(`SkiaSharp` / `Smart.Results` / `Template.MobileApp.State`)、パッケージ(アプリと同じ版: `Azure.AI.Vision.ImageAnalysis` 1.0.0 / `OllamaSharp` 5.4.30 / `SkiaSharp` 4.151.2 / `Usa.Smart.Results` 2.2.0) |
| `CvConsole/Program.cs` | 設定の読み込み(appsettings.json / 環境変数 / コマンドライン)、位置引数での 1 回実行、対話(種類 → 画像) |
| `CvConsole/VisionRunner.cs` | 画像の読み込み(EXIF の向きを反映)→ 解析 → 一覧表示 → PNG 保存。解析の呼び出しと失敗(`Result` の `Error`)の表示は `SampleCvNet*ViewModel` と同じ |
| `CvConsole/DetectRenderer.cs` | `Graphics/Drawing/DetectDrawing.cs` と同じ見た目(線幅 5 / 文字 16 / 色は信頼度)で枠とラベルを描く。長辺 1280 に縮小して画面と同じ縮尺にする |
| `CvConsole/Helpers/ImageHelper.cs` | アプリの `ImageHelper.ToNormalizeBitmap`(本体は同じ) |
| `CvConsole/VisionFeature.cs` / `CommandLine.cs` / `Terminal.cs` / `appsettings.json` / `CvConsole.csproj` | 解析の種類(`All` は全種類)、スイッチの対応表と位置引数、コンソール入出力、設定の既定値、パッケージ(`Microsoft.Extensions.Configuration.*` 10.0.11) |
| `ChatConsole/Program.cs` | 設定の読み込み、未設定なら「Ollama end point is not configured.」を出して終了(アプリはメニューで同じ文言を案内して画面に入らない)、`OllamaApiClient` の生成、あいさつ、入力ループ(`/voice` / `/exit`)、音声入力で反映した文章の送信 |
| `ChatConsole/ChatSession.cs` | 会話。履歴を保持して `GetStreamingResponseAsync` で応答を流し込む。`SampleChatViewModel` の `RespondAsync` と同じ |
| `ChatConsole/VoiceInput.cs` | 音声入力。1 回の発話を認識して文章を返す(Enter で停止、無音で自動停止、途中経過を表示)。アプリの `ChatView` のマイクボタン相当 |
| `ChatConsole/SpeechRecognizer.cs` | Windows の音声認識(`System.Speech` のディクテーション)。1 回の発話を認識し無音で自動停止、途中経過(仮説)を表示。アプリの `ISpeechService` 相当 |
| `ChatConsole/CommandLine.cs` / `Terminal.cs` / `appsettings.json` / `ChatConsole.csproj` | スイッチの対応表、コンソール入出力(行の書き直し / リダイレクト時は行入力)、設定の既定値(`http://localhost:11434` / `gemma2`)、パッケージ(`System.Speech` 10.0.11、`net10.0-windows`) |

## 設定(接続先とキー)

| キー | 用途 | appsettings.json の既定 | 環境変数 | コマンドライン |
| --- | --- | --- | --- | --- |
| `AIServiceEndPoint` | Azure AI Vision の接続先(CvConsole) | 空 | `AISAMPLE_AIServiceEndPoint` | `--endpoint <url>` |
| `AIServiceKey` | Azure AI Vision のキー(CvConsole) | 空 | `AISAMPLE_AIServiceKey` | `--key <key>` |
| `OutputDirectory` | 枠を描いた PNG の出力先(CvConsole) | 空 = カレントの `output/` | `AISAMPLE_OutputDirectory` | `--out <dir>` |
| `ShowResult` | 保存した PNG を関連付けで開く(CvConsole) | `false` | `AISAMPLE_ShowResult` | `--show true` |
| `OllamaEndPoint` | Ollama の接続先(ChatConsole) | `http://localhost:11434` | `AISAMPLE_OllamaEndPoint` | `--endpoint <url>` |
| `OllamaModel` | Ollama のモデル名(ChatConsole)。接続先かモデル名が空だと起動時に終了 | `gemma2` | `AISAMPLE_OllamaModel` | `--model <name>` |

優先順は appsettings.json < 環境変数 < コマンドライン。`appsettings.json` は各プロジェクトの直下(ビルドで出力先へコピー)で、キーを書いてもよい(コミットしないこと)。アプリの Setting 画面 / QR のキー名(`AIServiceEndPoint` / `AIServiceKey` / `OllamaEndPoint` / `OllamaModel`)と同じ。

## 実行

```bash
# CvConsole: 1 回実行 (<種類> = Object / Tag / People / Ocr / All、画像は複数可)
dotnet run --project CvConsole -- --endpoint https://<resource>.cognitiveservices.azure.com/ --key <key> Tag photo.jpg
dotnet run --project CvConsole -- All photo.jpg

# CvConsole: 対話 (種類 → 画像のパス。パスはエクスプローラーからのドラッグ & ドロップ可。空で終了)
dotnet run --project CvConsole

# ChatConsole: Ollama (既定 http://localhost:11434 / gemma2)
dotnet run --project ChatConsole
dotnet run --project ChatConsole -- --endpoint http://localhost:12321 --model gemma2
```

CvConsole の出力:

- Object / People / Ocr = 1 件ごとに `ラベル 信頼度 (左, 上)-(右, 下)`(0〜1 の正規化座標と画素)。枠とラベルを描いた PNG を `output/<画像名>.<種類>.png` に保存(検出 0 件でも元画像を保存)
- Tag = `🏷 名前  信頼度%`(アプリのパネルと同じ書式)。0 件は「タグは検出されませんでした」
- 失敗はアプリのダイアログと同じ「解析に失敗しました。」+ 例外のメッセージ(`RequestFailedException` / `HttpRequestException` / 未設定の `InvalidOperationException`)。所要時間を ms で表示

ChatConsole の操作:

1. `you>` に質問を入力すると `ai>` に応答がストリーミングで流れる(応答まで `...`)。履歴ごと送るので 2 回目は 1 回目の内容を踏まえる
2. `/voice` で音声入力。話すと途中経過が行に出て、無音が続くと自動で止まる(Enter で即停止)。認識した文章が入力欄に入る
3. 反映した文章はプロンプトに `you [文章]>` と出る。そのまま Enter で送信、別の文字列を入力するとそちらを送信
4. `/exit` で終了

## Azure の設定

1. Azure portal で **Azure AI services**(マルチサービス)か **Computer Vision** のリソースを作る。リージョンは Image Analysis 4.0 に対応しているもの(対応リージョンは https://learn.microsoft.com/azure/ai-services/computer-vision/overview-image-analysis の Region availability)。Microsoft Foundry の AI Services リソース(`https://<name>.services.ai.azure.com`)も Image Analysis 4.0 に応答する
2. リソースの「キーとエンドポイント」の Endpoint と Key 1 を `AIServiceEndPoint` / `AIServiceKey` に設定する(価格レベルは Free F0 で可。F0 は 20 回 / 分)
3. 端末のアプリに入れる値も同じ(Setting 画面の QR、キー名は `AIServiceEndPoint` / `AIServiceKey`)

## Ollama / 音声認識の準備

- Ollama をインストールし `ollama pull gemma2`。既定の接続先は `http://localhost:11434`。`ollama list` はサーバを自動起動する
- 11434 に bind できない(`listen tcp 127.0.0.1:11434: bind: An attempt was made to access a socket in a way forbidden by its access permissions.`)ときは、Hyper-V / WSL の動的ポート予約に 11434 が入っている(`netsh int ipv4 show excludedportrange protocol=tcp`)。別ポートで起動(`OLLAMA_HOST=127.0.0.1:12434 ollama serve`)して `--endpoint http://localhost:12434` で繋ぐか、管理者で `netsh int ipv4 add excludedportrange protocol=tcp startport=11434 numberofports=1` を予約してから再起動する
- 端末のアプリから Wi-Fi 経由で使うときは、Ollama を全インターフェースで待ち受け(`OLLAMA_HOST=0.0.0.0:<port>`)、ファイアウォールで受信を許可し、アプリの `OllamaEndPoint` を `http://<PC の IP>:<port>` にする(`adb reverse` は不要)。アプリのマニフェストは `usesCleartextTraffic="true"`(localhost 以外への平文 HTTP は既定で拒否される)
- 最初の要求はモデルのロードで 1 分近くかかる
- 音声認識は Windows の言語パックの音声認識(設定 > 時刻と言語 > 音声認識 の言語。`System.Speech` は `HKLM\SOFTWARE\Microsoft\Speech\Recognizers\Tokens` の認識エンジン)と既定の録音デバイスが必要。無いときは「(音声認識を開始できませんでした)」+ 理由を表示し、`t` の文字入力で代用できる。認識の言語は `CultureInfo.CurrentCulture`(日本語 Windows なら ja-JP)

## アプリとの対応

| アプリ | コンソール |
| --- | --- |
| `CameraController` で撮影 → `ImageHelper.ToNormalizeBitmap` | 画像ファイルを `ImageHelper.ToNormalizeBitmap`(同じ本体)で読む |
| `AzureVisionUsecase.DetectObjectsAsync` / `DetectPeopleAsync` / `DetectTagsAsync` / `ReadTextAsync`(失敗は `Result` で返す) | 同じ(コピー)。`Settings` の値は起動時の設定 |
| `DetectDrawing`(枠 + ラベル + 信頼度、`GraphicsView`) | `DetectRenderer`(同じ描画を SkiaSharp で PNG に) |
| `TagsText`(`🏷 名前  信頼度%`) | 同じ書式で標準出力 |
| `IDialog.InformationAsync("解析に失敗しました。...")` | 同じ文言を標準出力 |
| `OllamaApiClient` を `IChatClient` として生成(`SampleChatViewModel`)、未設定はメニューの案内で画面に入らない | `Program.cs`(未設定は同じ文言を出して終了) |
| `SampleChatViewModel` の履歴 / ストリーミング / 例外の扱い | `ChatSession`(同じ処理) |
| `ISpeechService.RecognizeAsync` / `RecognizeStopAsync` / `RecognizeCancel`(無音で自動停止、部分結果) | `SpeechRecognizer`(`System.Speech`、`RecognizeMode.Single`、`RecognizeAsyncStop` / `RecognizeAsyncCancel`、`SpeechHypothesized`) |
| `ChatView` のマイクボタン(認識中は停止ボタン、途中結果を入力欄へ、最終結果で終了) | `VoiceInput`(Enter で停止、途中経過を行に表示) |
| 認識した文章が入力欄に入る | プロンプトに `you [文章]>`。空のまま Enter で送信 |

## 確認済みの動作(2026-09-19、Windows 11 / .NET SDK 10.0.401)

- ビルド 0 警告(Debug / Release)、`jb inspectcode AiSample.slnx --properties:Configuration=Release` 0 件
- CvConsole(Foundry の AI Services リソース): Tag = ノート PC の商品画像で「コンピューター 100% / ノート 100% / ノートパソコン 99% / … / mac 55%」の日本語タグ、Ocr = アプリのスクリーンショット(`Document/Sample_CvNet_Tag.png`)で 9 行(「霧 95%」「Back」など)の枠とテキスト、Object / People = Microsoft のサンプル写真(Azure-Samples/cognitive-services-sample-data-files の `objects.jpg` / `faces.jpg`、docs の `presentation.png`)で person 0.94 / Skateboard 0.85 / Jeans 0.57、人物 4 人、person / display / plant を検出して枠と信頼度の色(赤〜橙)を確認。`printed_text.jpg` / `handwritten_text.jpg` の Ocr は栄養表示 15 行 / 手書き 2 行(生成画像の商品 / キャラクターでは Object / People とも 0 件)。未設定は「AI service is not configured.」、存在しないファイル / 画像でないファイル / 不正な種類はメッセージを出して継続
- ChatConsole(PC の Ollama gemma2、`--endpoint http://localhost:12321`。2026-09-20): 質問への応答がストリーミングで表示、2 回目の質問が 1 回目の内容を踏まえる、`/voice` は無音で「音声を認識できませんでした」(Enter で停止)。`--model ""` は「Ollama end point is not configured.」で終了

## 解析上の制約

- `AnalysisMode=All` のため exe では public 型が CA1515 になる。コピーしたファイル(public のまま)はクラスライブラリ `AiShared` に置く
- CA1303 は `Console.Write` / `Console.WriteLine` へのリテラルを対象にし、`Console.Out`(TextWriter)は async メソッド内で CA1849(`WriteLineAsync` を使え)になる。出力は `Terminal` に集約する
- CA2007 のため await はすべて `ConfigureAwait(false)`。CA1308 のため小文字化はせず種類名は PascalCase(`output/<画像名>.Tag.png`、入力は大文字小文字を区別しない)
- ReSharper: `SKPaint` の初期化はオブジェクト初期化子でなく `using var` の後に代入(UsingStatementResourceInitialization)
