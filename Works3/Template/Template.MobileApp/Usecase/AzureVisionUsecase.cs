namespace Template.MobileApp.Usecase;

using Azure;
using Azure.AI.Vision.ImageAnalysis;

public sealed record TagResult(string Name, float Confidence);

// Azure AI Vision (Image Analysis 4.0)。接続先とキーは設定から呼び出しのたびに取得する
public sealed class AzureVisionUsecase
{
    private const int JpegQuality = 90;

    // 枠を描く物体 / 人物の信頼度の下限
    private const float MinConfidence = 0.5f;

    private readonly Settings settings;

    public AzureVisionUsecase(Settings settings)
    {
        this.settings = settings;
    }

    // 物体 (枠 + ラベル + 信頼度)
    public async Task<DetectResult[]> DetectObjectsAsync(SKBitmap bitmap)
    {
        var result = await AnalyzeAsync(bitmap, VisualFeatures.Objects, default).ConfigureAwait(false);
#pragma warning disable IDE0028
        return result.Objects.Values
            .Where(static x => (x.Tags.Count > 0) && (x.Tags[0].Confidence >= MinConfidence))
            .Select(x => ToDetectResult(x.BoundingBox, bitmap, x.Tags[0].Confidence, x.Tags[0].Name))
            .ToArray();
#pragma warning restore IDE0028
    }

    // 人物 (枠 + 信頼度)
    public async Task<DetectResult[]> DetectPeopleAsync(SKBitmap bitmap)
    {
        var result = await AnalyzeAsync(bitmap, VisualFeatures.People, default).ConfigureAwait(false);
#pragma warning disable IDE0028
        return result.People.Values
            .Where(static x => x.Confidence >= MinConfidence)
            .Select(x => ToDetectResult(x.BoundingBox, bitmap, x.Confidence, "person"))
            .ToArray();
#pragma warning restore IDE0028
    }

    // タグ (画像全体の内容。日本語)
    public async Task<TagResult[]> DetectTagsAsync(SKBitmap bitmap)
    {
        var result = await AnalyzeAsync(bitmap, VisualFeatures.Tags, new ImageAnalysisOptions { Language = "ja" }).ConfigureAwait(false);
#pragma warning disable IDE0028
        return result.Tags.Values
            .Select(x => new TagResult(x.Name, x.Confidence))
            .ToArray();
#pragma warning restore IDE0028
    }

    // 文字 (行ごとの外接矩形 + テキスト)
    public async Task<DetectResult[]> ReadTextAsync(SKBitmap bitmap)
    {
        var result = await AnalyzeAsync(bitmap, VisualFeatures.Read, default).ConfigureAwait(false);
#pragma warning disable IDE0028
        return result.Read.Blocks
            .SelectMany(static x => x.Lines)
            .Select(x => new DetectResult(
                x.BoundingPolygon.Min(static p => p.X) / (float)bitmap.Width,
                x.BoundingPolygon.Min(static p => p.Y) / (float)bitmap.Height,
                x.BoundingPolygon.Max(static p => p.X) / (float)bitmap.Width,
                x.BoundingPolygon.Max(static p => p.Y) / (float)bitmap.Height,
                1f,
                x.Text))
            .ToArray();
#pragma warning restore IDE0028
    }

    private async Task<ImageAnalysisResult> AnalyzeAsync(SKBitmap bitmap, VisualFeatures features, ImageAnalysisOptions options)
    {
        var (endpoint, credential) = await ResolveCredentialAsync().ConfigureAwait(false);
        var client = new ImageAnalysisClient(endpoint, credential);
        var response = await client.AnalyzeAsync(Encode(bitmap), features, options).ConfigureAwait(false);
        return response.Value;
    }

    private async ValueTask<(Uri EndPoint, AzureKeyCredential Credential)> ResolveCredentialAsync()
    {
        var endpoint = settings.AIServiceEndPoint;
        var key = await settings.GetAIServiceKeyAsync().ConfigureAwait(false);
        if (String.IsNullOrEmpty(endpoint) || String.IsNullOrEmpty(key))
        {
            throw new InvalidOperationException("AI service is not configured.");
        }

        return (new Uri(endpoint), new AzureKeyCredential(key));
    }

    private static BinaryData Encode(SKBitmap bitmap)
    {
        using var data = bitmap.Encode(SKEncodedImageFormat.Jpeg, JpegQuality);
        return BinaryData.FromBytes(data.ToArray());
    }

    private static DetectResult ToDetectResult(ImageBoundingBox box, SKBitmap bitmap, float score, string label) =>
        new(box.X / (float)bitmap.Width, box.Y / (float)bitmap.Height, (box.X + box.Width) / (float)bitmap.Width, (box.Y + box.Height) / (float)bitmap.Height, score, label);
}
