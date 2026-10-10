namespace Template.MobileApp.Controls;

using SkiaSharp.Views.Maui.Controls;

//--------------------------------------------------------------------------------
// Avatar
//--------------------------------------------------------------------------------
// 差出人のアイコン。画像のファイル名 (アプリの中の Avatar フォルダー) があれば読み込み、無ければ色の地に名前の頭文字を描く
public sealed class MailAvatarImage : Image
{
    private const int InitialSize = 144;

    // 頭文字のアイコンの色 (名前から決める)
    private static readonly SKColor[] InitialColors =
    [
        SKColor.Parse("#FB8C00"),
        SKColor.Parse("#00897B"),
        SKColor.Parse("#D81B60"),
        SKColor.Parse("#43A047"),
        SKColor.Parse("#3949AB"),
        SKColor.Parse("#6D4C41"),
        SKColor.Parse("#8E24AA")
    ];

    private static readonly SKTypeface InitialTypeface = SKFontManager.Default.MatchCharacter('あ') ?? SKTypeface.Default;

    // 同じ画像は作り直さない
    private static readonly Dictionary<string, ImageSource> Files = [];

    private static readonly Dictionary<string, ImageSource> Initials = [];

    public static readonly BindableProperty AvatarProperty = BindableProperty.Create(
        nameof(Avatar),
        typeof(string),
        typeof(MailAvatarImage),
        propertyChanged: OnSourceChanged);

    public static readonly BindableProperty SenderProperty = BindableProperty.Create(
        nameof(Sender),
        typeof(string),
        typeof(MailAvatarImage),
        propertyChanged: OnSourceChanged);

    public string? Avatar
    {
        get => (string?)GetValue(AvatarProperty);
        set => SetValue(AvatarProperty, value);
    }

    public string? Sender
    {
        get => (string?)GetValue(SenderProperty);
        set => SetValue(SenderProperty, value);
    }

    private static void OnSourceChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        var image = (MailAvatarImage)bindable;
        image.Source = image.Avatar is { Length: > 0 } avatar
            ? LoadFile(avatar)
            : image.Sender is { Length: > 0 } sender ? CreateInitial(sender) : null;
    }

    private static ImageSource LoadFile(string fileName)
    {
        if (!Files.TryGetValue(fileName, out var source))
        {
            var path = Path.Combine("Avatar", fileName);
            source = ImageSource.FromStream(_ => FileSystem.OpenAppPackageFileAsync(path));
            Files[fileName] = source;
        }
        return source;
    }

    // 色の地に名前の頭文字 (丸く切り抜くのは XAML)
    private static ImageSource CreateInitial(string name)
    {
        if (!Initials.TryGetValue(name, out var source))
        {
            var bitmap = new SKBitmap(InitialSize, InitialSize);
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(InitialColors[name.Sum(static x => x) % InitialColors.Length]);
                using var font = new SKFont(InitialTypeface, InitialSize * 0.42f);
                font.Embolden = true;
                using var paint = new SKPaint();
                paint.IsAntialias = true;
                paint.Color = SKColors.White;
                var metrics = font.Metrics;
                canvas.DrawText(name[..1], InitialSize / 2f, (InitialSize - metrics.Ascent - metrics.Descent) / 2f, SKTextAlign.Center, font, paint);
            }
            source = new SKBitmapImageSource { Bitmap = bitmap };
            Initials[name] = source;
        }
        return source;
    }
}
