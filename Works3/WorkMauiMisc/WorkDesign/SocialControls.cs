using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

using System.Reflection;

using Font = Microsoft.Maui.Graphics.Font;

namespace WorkDesign;

public sealed class SocialMenuButton : GraphicsView, IDrawable
{
    public SocialMenuButton()
    {
        Drawable = this;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = new Color(1, 1, 1, 0.5f);
        canvas.FillRectangle(dirtyRect);

#if ANDROID
        canvas.Font = new Microsoft.Maui.Graphics.Font("DSEG7.ttf", 18);
#elif IOS || MACCATALYST
               canvas.Font = new Microsoft.Maui.Graphics.Font("DSEG7");
#else
        canvas.Font = new Microsoft.Maui.Graphics.Font("NotoSerifJP-Medium");
#endif

        canvas.FontSize = 18;
        canvas.FontColor = Colors.Black;
        //canvas.DrawText("", );
        canvas.DrawString("電力", dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height, HorizontalAlignment.Center, VerticalAlignment.Center);
    }
}

public class CustomFontSkiaView : SKCanvasView
{
    // 描画するテキスト
    public static readonly BindableProperty DrawTextProperty =
        BindableProperty.Create(nameof(DrawText), typeof(string), typeof(CustomFontSkiaView), "Custom Font Text!",
            propertyChanged: (bindable, oldValue, newValue) => ((CustomFontSkiaView)bindable).InvalidateSurface());

    public string DrawText
    {
        get => (string)GetValue(DrawTextProperty);
        set => SetValue(DrawTextProperty, value);
    }

    // テキストの色
    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(CustomFontSkiaView), Colors.Black,
            propertyChanged: (bindable, oldValue, newValue) => ((CustomFontSkiaView)bindable).InvalidateSurface());

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    // カスタムフォントのSKTypefaceインスタンスをキャッシュ
    private SKTypeface _customTypeface;
    private const string CustomFontResourceName = "NotoSerifJP-Medium.ttf"; // ★重要: ここをプロジェクトとフォントのパスに合わせてください

    public CustomFontSkiaView()
    {
        // PaintSurfaceイベントハンドラを設定
        PaintSurface += OnPaintSurface;
        LoadCustomFont(); // フォントを読み込む
    }

    private void LoadCustomFont()
    {
        // 埋め込みリソースからフォントを読み込む
        try
        {
            var assembly = typeof(CustomFontSkiaView).GetTypeInfo().Assembly;
            using (var stream = FileSystem.OpenAppPackageFileAsync(CustomFontResourceName).Result)
            {
                if (stream != null)
                {
                    _customTypeface = SKTypeface.FromStream(stream);
                }
                else
                {
                    // リソースが見つからない場合のログまたはエラー処理
                    System.Diagnostics.Debug.WriteLine($"Error: Custom font resource not found: {CustomFontResourceName}");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading custom font: {ex.Message}");
        }
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs args)
    {
        SKImageInfo info = args.Info;
        SKSurface surface = args.Surface;
        SKCanvas canvas = surface.Canvas;

        canvas.Clear(new SKColor(255, 255, 255, 64)); // 背景色をクリア

        if (!string.IsNullOrEmpty(DrawText) && _customTypeface != null)
        {
            using (SKPaint textPaint = new SKPaint())
            {
                textPaint.Color = TextColor.ToSKColor(); // MAUI ColorをSkiaSharp Colorに変換
                textPaint.IsAntialias = true;
                textPaint.TextSize = 18; // フォントサイズを調整

                // ★ここにカスタムフォントを設定
                textPaint.Typeface = _customTypeface;

                // テキストのバウンディングボックスを取得して中央揃えを計算
                SKRect textBounds = new SKRect();
                textPaint.MeasureText(DrawText, ref textBounds);

                float x = info.Width / 2 - textBounds.MidX;
                float y = info.Height / 2 - textBounds.MidY;

                canvas.DrawText(DrawText, x, y, textPaint);
            }
        }
        else if (_customTypeface == null)
        {
            // フォントの読み込みに失敗した場合の代替表示
            using (SKPaint errorPaint = new SKPaint())
            {
                errorPaint.Color = SKColors.Red;
                errorPaint.TextSize = 30;
                errorPaint.TextAlign = SKTextAlign.Center;
                canvas.DrawText("Font Load Error!", info.Width / 2, info.Height / 2, errorPaint);
            }
        }
    }
}