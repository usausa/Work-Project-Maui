using System.Text;

using CvConsole;

using Microsoft.Extensions.Configuration;

using Template.MobileApp.State;
using Template.MobileApp.Usecase;

// 設定は appsettings.json < 環境変数 (AISAMPLE_ 接頭辞) < コマンドライン (--endpoint / --key / --out / --show) の順で上書き
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables("AISAMPLE_")
    .AddCommandLine(args, CommandLine.Switches)
    .Build();

var settings = new Settings
{
    AIServiceEndPoint = configuration["AIServiceEndPoint"] ?? string.Empty,
    AIServiceKey = configuration["AIServiceKey"] ?? string.Empty
};
var outputDirectory = configuration["OutputDirectory"];
if (String.IsNullOrEmpty(outputDirectory))
{
    outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "output");
}

var showResult = Boolean.TryParse(configuration["ShowResult"], out var show) && show;

Console.OutputEncoding = Encoding.UTF8;
Terminal.WriteLine("CvConsole - Azure AI Vision (Image Analysis 4.0)");
Terminal.WriteLine($"EndPoint : {(String.IsNullOrEmpty(settings.AIServiceEndPoint) ? "(未設定)" : settings.AIServiceEndPoint)}");
Terminal.WriteLine($"Key      : {(String.IsNullOrEmpty(settings.AIServiceKey) ? "(未設定)" : "設定済み")}");
Terminal.WriteLine($"Output   : {outputDirectory}");
Terminal.WriteLine();

var runner = new VisionRunner(new AzureVisionUsecase(settings), outputDirectory, showResult);

// 位置引数: <種類> <画像>... で 1 回だけ実行。無ければ対話
var positional = CommandLine.GetPositional(args);
if (positional.Length > 0)
{
    if (!VisionFeatures.TryParse(positional[0], out var features))
    {
        Terminal.WriteLine($"種類は {VisionFeatures.Names} のいずれかです: {positional[0]}");
        return 1;
    }

    if (positional.Length == 1)
    {
        Terminal.WriteLine("使い方: CvConsole [--endpoint <url>] [--key <key>] [--out <dir>] [--show true] [<種類> <画像>...]");
        return 1;
    }

    foreach (var path in positional.Skip(1))
    {
        await runner.RunAsync(features, path).ConfigureAwait(false);
    }

    return 0;
}

var current = VisionFeature.Object;
while (true)
{
    var input = Terminal.Prompt($"種類 ({VisionFeatures.Names}) [{current}]> ");
    if (input is null)
    {
        break;
    }

    var features = new[] { current };
    if (!String.IsNullOrWhiteSpace(input))
    {
        if (!VisionFeatures.TryParse(input, out features))
        {
            Terminal.WriteLine("種類が不正です");
            continue;
        }

        if (features.Length == 1)
        {
            current = features[0];
        }
    }

    var path = Terminal.Prompt("画像 (空で終了)> ");
    if (String.IsNullOrEmpty(path))
    {
        break;
    }

    await runner.RunAsync(features, path).ConfigureAwait(false);
}

return 0;
