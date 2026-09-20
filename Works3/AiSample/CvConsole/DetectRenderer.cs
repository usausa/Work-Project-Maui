namespace CvConsole;

using Template.MobileApp.Usecase;

// アプリの Graphics/Drawing/DetectDrawing.cs と同じ見た目で検出結果を画像に描く。
// 画面と同じ縮尺にするため長辺が DisplaySize になるよう縮小してから、線幅 5 / 文字 16 / 色は信頼度 (赤〜黄) で描く
internal static class DetectRenderer
{
    private const int DisplaySize = 1280;

    private const float StrokeWidth = 5f;

    private const float FontSize = 16f;

    public static async Task SaveAsync(SKBitmap bitmap, DetectResult[] results, string path)
    {
        using var display = ToDisplayBitmap(bitmap);
        using var canvas = new SKCanvas(display);
        Draw(canvas, display.Width, display.Height, results);
        canvas.Flush();

        using var image = SKImage.FromBitmap(display);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        await File.WriteAllBytesAsync(path, data.ToArray()).ConfigureAwait(false);
    }

    private static SKBitmap ToDisplayBitmap(SKBitmap bitmap)
    {
        var scale = (double)DisplaySize / Math.Max(bitmap.Width, bitmap.Height);
        if (scale >= 1)
        {
            return bitmap.Copy();
        }

        return bitmap.Resize(new SKImageInfo((int)(bitmap.Width * scale), (int)(bitmap.Height * scale)), new SKSamplingOptions(SKCubicResampler.Mitchell)) ?? bitmap.Copy();
    }

    private static void Draw(SKCanvas canvas, float width, float height, IEnumerable<DetectResult> results)
    {
        // 日本語のラベル (OCR の行など) を描けるタイプフェイス
        using var typeface = SKFontManager.Default.MatchCharacter('あ');
        using var font = new SKFont(typeface ?? SKTypeface.Default, FontSize);
        using var strokePaint = new SKPaint();
        strokePaint.Style = SKPaintStyle.Stroke;
        strokePaint.StrokeWidth = StrokeWidth;
        strokePaint.IsAntialias = true;
        using var textPaint = new SKPaint();
        textPaint.Color = SKColors.White;
        textPaint.IsAntialias = true;

        foreach (var result in results)
        {
            var x = result.Left * width;
            var y = result.Top * height;
            var w = (result.Right - result.Left) * width;
            var h = (result.Bottom - result.Top) * height;

            var c = (byte)(255 * (1 - result.Score));
            strokePaint.Color = new SKColor(255, c, 0);
            canvas.DrawRect(x, y, w, h, strokePaint);

            var text = String.IsNullOrEmpty(result.Label) ? $"{result.Score:F2}" : $"{result.Label} {result.Score:F2}";
            canvas.DrawText(text, x + w, y + h - font.Metrics.Descent, SKTextAlign.Right, font, textPaint);
        }
    }
}
