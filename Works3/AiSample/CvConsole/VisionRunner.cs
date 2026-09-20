namespace CvConsole;

using System.Diagnostics;

using CvConsole.Helpers;

using Smart.Results;

using Template.MobileApp.Usecase;

// 画像ファイルを読み込み (EXIF の向きを反映)、解析して結果を表示し、枠を描いた PNG を保存する。
// 解析の呼び出しと失敗の表示はアプリの SampleCvNet*ViewModel と同じ (失敗は Usecase が Result で返す)
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
        if (feature == VisionFeature.Tag)
        {
            var result = await usecase.DetectTagsAsync(bitmap).ConfigureAwait(false);
            watch.Stop();

            // 表示はアプリの TagsText と同じ
            if (result.IsSuccess)
            {
                Terminal.WriteLine(result.Value.Length > 0
                    ? String.Join("\n", result.Value.Select(static x => $"🏷 {x.Name}  {x.Confidence:P0}"))
                    : "タグは検出されませんでした");
            }
            else
            {
                Terminal.WriteLine($"解析に失敗しました。\n{result.Error.Message}");
            }
        }
        else
        {
            var result = await DetectAsync(feature, bitmap).ConfigureAwait(false);
            watch.Stop();

            if (result.IsSuccess)
            {
                if (result.Value.Length == 0)
                {
                    Terminal.WriteLine("検出されませんでした");
                }

                foreach (var item in result.Value)
                {
                    Terminal.WriteLine(
                        $"{item.Label} {item.Score:F2}  " +
                        $"({item.Left:F3}, {item.Top:F3})-({item.Right:F3}, {item.Bottom:F3})  " +
                        $"px ({(int)(item.Left * bitmap.Width)}, {(int)(item.Top * bitmap.Height)})-({(int)(item.Right * bitmap.Width)}, {(int)(item.Bottom * bitmap.Height)})");
                }

                await SaveAsync(feature, path, bitmap, result.Value).ConfigureAwait(false);
            }
            else
            {
                Terminal.WriteLine($"解析に失敗しました。\n{result.Error.Message}");
            }
        }

        Terminal.WriteLine($"({watch.ElapsedMilliseconds:N0} ms)");
        Terminal.WriteLine();
    }

    private Task<Result<DetectResult[]>> DetectAsync(VisionFeature feature, SKBitmap bitmap) =>
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
