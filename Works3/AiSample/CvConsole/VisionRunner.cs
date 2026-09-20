namespace CvConsole;

using System.Diagnostics;

using Azure;

using CvConsole.Helpers;

using Template.MobileApp.Usecase;

// 画像ファイルを読み込み (EXIF の向きを反映)、解析して結果を表示し、枠を描いた PNG を保存する。
// 解析の呼び出しと例外の扱いはアプリの SampleCvNet*ViewModel と同じ
internal sealed class VisionRunner
{
    private readonly AzureVisionUsecase usecase;

    private readonly string outputDirectory;

    private readonly bool showResult;

    public VisionRunner(AzureVisionUsecase usecase, string outputDirectory, bool showResult)
    {
        this.usecase = usecase;
        this.outputDirectory = outputDirectory;
        this.showResult = showResult;
    }

    public async Task RunAsync(VisionFeature[] features, string path)
    {
        if (!File.Exists(path))
        {
            Terminal.WriteLine($"ファイルがありません: {path}");
            return;
        }

        using var bitmap = Load(path);
        if (bitmap is null)
        {
            Terminal.WriteLine($"画像として読み込めません: {path}");
            return;
        }

        Terminal.WriteLine($"{Path.GetFileName(path)} ({bitmap.Width}x{bitmap.Height})");

        foreach (var feature in features)
        {
            await RunAsync(feature, path, bitmap).ConfigureAwait(false);
        }
    }

    private static SKBitmap? Load(string path)
    {
        if (SKBitmap.DecodeBounds(path).IsEmpty)
        {
            return null;
        }

        using var stream = File.OpenRead(path);
        return ImageHelper.ToNormalizeBitmap(stream);
    }

    private async Task RunAsync(VisionFeature feature, string path, SKBitmap bitmap)
    {
        Terminal.WriteLine($"[{feature}]");

        var watch = Stopwatch.StartNew();
        try
        {
            if (feature == VisionFeature.Tag)
            {
                var results = await usecase.DetectTagsAsync(bitmap).ConfigureAwait(false);
                watch.Stop();

                // 表示はアプリの TagsText と同じ
                Terminal.WriteLine(results.Length > 0
                    ? String.Join("\n", results.Select(static x => $"🏷 {x.Name}  {x.Confidence:P0}"))
                    : "タグは検出されませんでした");
            }
            else
            {
                var results = await DetectAsync(feature, bitmap).ConfigureAwait(false);
                watch.Stop();

                if (results.Length == 0)
                {
                    Terminal.WriteLine("検出されませんでした");
                }

                foreach (var result in results)
                {
                    Terminal.WriteLine(
                        $"{result.Label} {result.Score:F2}  " +
                        $"({result.Left:F3}, {result.Top:F3})-({result.Right:F3}, {result.Bottom:F3})  " +
                        $"px ({(int)(result.Left * bitmap.Width)}, {(int)(result.Top * bitmap.Height)})-({(int)(result.Right * bitmap.Width)}, {(int)(result.Bottom * bitmap.Height)})");
                }

                await SaveAsync(feature, path, bitmap, results).ConfigureAwait(false);
            }

            Terminal.WriteLine($"({watch.ElapsedMilliseconds:N0} ms)");
        }
        catch (Exception ex) when (ex is RequestFailedException or HttpRequestException or InvalidOperationException)
        {
            Terminal.WriteLine($"解析に失敗しました。\n{ex.Message}");
        }

        Terminal.WriteLine();
    }

    private Task<DetectResult[]> DetectAsync(VisionFeature feature, SKBitmap bitmap) =>
        feature switch
        {
            VisionFeature.Object => usecase.DetectObjectsAsync(bitmap),
            VisionFeature.People => usecase.DetectPeopleAsync(bitmap),
            VisionFeature.Ocr => usecase.ReadTextAsync(bitmap),
            _ => throw new ArgumentOutOfRangeException(nameof(feature))
        };

    private async Task SaveAsync(VisionFeature feature, string path, SKBitmap bitmap, DetectResult[] results)
    {
        Directory.CreateDirectory(outputDirectory);
        var output = Path.Combine(outputDirectory, $"{Path.GetFileNameWithoutExtension(path)}.{feature}.png");
        await DetectRenderer.SaveAsync(bitmap, results, output).ConfigureAwait(false);
        Terminal.WriteLine($"→ {output}");

        if (showResult)
        {
            using var process = Process.Start(new ProcessStartInfo(output) { UseShellExecute = true });
        }
    }
}
