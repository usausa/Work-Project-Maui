namespace WorkDesign;

using Android.Text;

using Microsoft.Maui;

using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

using System;
using System.Collections.ObjectModel;
using System.Globalization;

using static Microsoft.Maui.LifecycleEvents.AndroidLifecycle;

public partial class MailPage : ContentPage
{
	public MailPage()
	{
		InitializeComponent();
	}
}

public partial class MailPageViewModel : ExtendViewModelBase
{
    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    public ObservableCollection<MailMessage> Messages { get; } = [];

    public MailPageViewModel()
    {
        _ = Task.Run(InitializeData);
    }

    private void InitializeData()
    {
        IsLoading = true;

        Messages.Add(new MailMessage
        {
            DateTime = DateTime.Now,
            Image = ImageSource.FromFile("profile.jpg"),
            From = "山奥通信",
            Title = "タイトルだよもんタイトルだよもんタイトルだよもんタイトルだよもんタイトルだよもん",
            Body = "こんにちは。\nうさうさです、どうぞよろしくお願いしますだよもん。\n文章はまだ続きます。"
        });
        Messages.Add(new MailMessage
        {
            DateTime = DateTime.Now.AddHours(-3).AddMinutes(-12),
            Image = ImageSource.FromFile("genbaneko.jpg"),
            From = "現場猫bot",
            Title = "作業前安全確認",
            Body = "今日も一日ゼロ災ヨシ！\nあああああああああああ!!!!!!!!!!"
        });
        Messages.Add(new MailMessage
        {
            DateTime = DateTime.Now.AddHours(-6).AddMinutes(-15),
            Image = ImageSource.FromFile("profile.jpg"),
            From = "山奥通信",
            Title = "タイトルだよもんタイトルだよもんタイトルだよもんタイトルだよもんタイトルだよもん",
            Body = "こんにちは。\n私はうさうさです。\nどうぞよろしくお願いしますだよもん。\n文章はまだ続きます。"
        });
        Messages.Add(new MailMessage
        {
            DateTime = DateTime.Now.AddDays(-1),
            Image = ImageSource.FromFile("usausa.jpg"),
            From = "うさうさ・メープル・フレンチトースト",
            Title = "Re: Re: おはようございます！",
            Body = "こんにちは！\n先日はありがとうございました"
        });
        Messages.Add(new MailMessage
        {
            DateTime = DateTime.Now.AddDays(-2),
            Image = ImageSource.FromFile("genbaneko.jpg"),
            From = "現場猫bot",
            Title = "作業前安全確認",
            Body = "今日も一日ゼロ災ヨシ！\nあああああああああああ!!!!!!!!!!"
        });
        Messages.Add(new MailMessage
        {
            DateTime = DateTime.Now.AddDays(-5),
            Image = ImageSource.FromFile("genbaneko.jpg"),
            From = "現場猫bot",
            Title = "作業前安全確認",
            Body = "今日も一日ゼロ災ヨシ！\nあああああああああああ!!!!!!!!!!"
        });
        Messages.Add(new MailMessage
        {
            DateTime = DateTime.Now.AddDays(-10),
            Image = ImageSource.FromFile("genbaneko.jpg"),
            From = "現場猫bot",
            Title = "作業前安全確認",
            Body = "今日も一日ゼロ災ヨシ！\nあああああああああああ!!!!!!!!!!"
        });
        Messages.Add(new MailMessage
        {
            DateTime = DateTime.Now.AddDays(-20),
            Image = ImageSource.FromFile("genbaneko.jpg"),
            From = "現場猫bot",
            Title = "作業前安全確認",
            Body = "今日も一日ゼロ災ヨシ！\nあああああああああああ!!!!!!!!!!"
        });

        IsLoading = false;
    }
}

public sealed class MailMessage
{
    public DateTime DateTime { get; set; }

    public ImageSource Image { get; set; } = default!;

    public string From { get; set; } = default!;

    public string Title { get; set; } = default!;

    public string Body { get; set; } = default!;
}

public class MailDateTimeStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            var now = DateTime.Now;
            var today = now.Date;
            var oneWeekAgo = today.AddDays(-7);

            if (dateTime.Date == today)
            {
                return dateTime.ToString("HH:mm", culture);
            }
            if (dateTime.Date > oneWeekAgo)
            {
                return dateTime.ToString("dddd", culture);
            }
            return dateTime.ToString("MM/dd", culture);
        }

        return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public sealed class ToggleChangedEventArgs : EventArgs
{
    public static readonly ToggleChangedEventArgs On = new(true);
    public static readonly ToggleChangedEventArgs Off = new(false);

    public bool IsToggled { get; }

    private ToggleChangedEventArgs(bool isToggled)
    {
        IsToggled = isToggled;
    }
}

public class TextToggle : SKCanvasView
{
    private static readonly SKTypeface Typeface = SKFontManager.Default.MatchCharacter('あ');

    public event EventHandler<ToggleChangedEventArgs>? ToggleChanged;

    private float animationProgress;

    // State

    public static readonly BindableProperty IsToggledProperty = BindableProperty.Create(
        nameof(IsToggled),
        typeof(bool),
        typeof(TextToggle),
        false,
        propertyChanged: (b, _, n) => ((TextToggle)b).AnimateToggle((bool)n));

    public bool IsToggled
    {
        get => (bool)GetValue(IsToggledProperty);
        set => SetValue(IsToggledProperty, value);
    }

    // Text

    public static readonly BindableProperty OnTextProperty = BindableProperty.Create(
        nameof(OnText),
        typeof(string),
        typeof(TextToggle),
        propertyChanged: Invalidate);

    public string OnText
    {
        get => (string)GetValue(OnTextProperty);
        set => SetValue(OnTextProperty, value);
    }

    public static readonly BindableProperty OffTextProperty = BindableProperty.Create(
        nameof(OffText),
        typeof(string),
        typeof(TextToggle),
        propertyChanged: Invalidate);

    public string OffText
    {
        get => (string)GetValue(OffTextProperty);
        set => SetValue(OffTextProperty, value);
    }

    // Font

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize),
        typeof(double),
        typeof(TextToggle),
        14d,
        propertyChanged: Invalidate);

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    // Color

    public static readonly BindableProperty SelectedTextColorProperty = BindableProperty.Create(
        nameof(SelectedTextColor),
        typeof(Color),
        typeof(TextToggle),
        Colors.Blue,
        propertyChanged: Invalidate);

    public Color SelectedTextColor
    {
        get => (Color)GetValue(SelectedTextColorProperty);
        set => SetValue(SelectedTextColorProperty, value);
    }

    public static readonly BindableProperty SelectedBackgroundColorProperty = BindableProperty.Create(
        nameof(SelectedBackgroundColor),
        typeof(Color),
        typeof(TextToggle),
        Colors.White,
        propertyChanged: Invalidate);

    public Color SelectedBackgroundColor
    {
        get => (Color)GetValue(SelectedBackgroundColorProperty);
        set => SetValue(SelectedBackgroundColorProperty, value);
    }

    public static readonly BindableProperty UnselectedTextColorProperty = BindableProperty.Create(
        nameof(UnselectedTextColor),
        typeof(Color),
        typeof(TextToggle),
        Colors.White,
        propertyChanged: Invalidate);

    public Color UnselectedTextColor
    {
        get => (Color)GetValue(UnselectedTextColorProperty);
        set => SetValue(UnselectedTextColorProperty, value);
    }

    public static readonly BindableProperty UnselectedBackgroundColorProperty = BindableProperty.Create(
        nameof(UnselectedBackgroundColor),
        typeof(Color),
        typeof(TextToggle),
        Colors.LightGray,
        propertyChanged: Invalidate);

    public Color UnselectedBackgroundColor
    {
        get => (Color)GetValue(UnselectedBackgroundColorProperty);
        set => SetValue(UnselectedBackgroundColorProperty, value);
    }

    public TextToggle()
    {
        EnableTouchEvents = true;
        Touch += (_, _) =>
        {
            IsToggled = !IsToggled;
            ToggleChanged?.Invoke(this, IsToggled ? ToggleChangedEventArgs.On : ToggleChangedEventArgs.Off);
            AnimateToggle(IsToggled);
        };
    }

    private static void Invalidate(BindableObject bindable, object oldValue, object newValue)
    {
        ((TextToggle)bindable).InvalidateSurface();
    }

    private void AnimateToggle(bool newState)
    {
        var start = animationProgress;
        var end = newState ? 1f : 0f;
        var animation = new Animation(v =>
        {
            animationProgress = (float)v;
            InvalidateSurface();
        }, start, end);

        animation.Commit(this, "ToggleAnimation", 16, 200, Easing.SinInOut);
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear();

        using var paint = new SKPaint();
        paint.IsAntialias = true;

        using var font = new SKFont(Typeface, size: (float)(FontSize * DeviceDisplay.MainDisplayInfo.Density));

        var radius = e.Info.Height / 2f;
        var onTextWidth = font.MeasureText(OnText);
        var offTextWidth = font.MeasureText(OffText);
        var onWidth = e.Info.Width * (onTextWidth + (radius * 2)) / (onTextWidth + offTextWidth + (radius * 4));
        var offWidth = e.Info.Width - onWidth;

        // Background
        paint.Color = UnselectedBackgroundColor.ToSKColor();
        canvas.DrawRoundRect(new SKRoundRect(e.Info.Rect, radius), paint);

        // Active background
        var animStart = onWidth * animationProgress;
        var animWidth = onWidth + (offWidth - onWidth) * animationProgress;
        var activeRect = new SKRoundRect(new SKRect(animStart, e.Info.Rect.Top, animStart + animWidth, e.Info.Rect.Bottom), radius);
        paint.Color = SelectedBackgroundColor.ToSKColor();
        canvas.DrawRoundRect(activeRect, paint);

        // Unselected text
        paint.Color = UnselectedTextColor.ToSKColor();
        var onTextX = (onWidth - onTextWidth) / 2f;
        var offTextX = onWidth + ((offWidth - offTextWidth) / 2f);
        var textY = (e.Info.Height - font.Metrics.Ascent - font.Metrics.Descent) / 2f;
        canvas.DrawText(OnText, onTextX, textY, font, paint);
        canvas.DrawText(OffText, offTextX, textY, font, paint);

        // Active mask
        canvas.SaveLayer();

        // Background
        paint.Color = SelectedBackgroundColor.ToSKColor();
        canvas.DrawRoundRect(activeRect, paint);

        // Selected text
        paint.Color = SelectedTextColor.ToSKColor();
        paint.BlendMode = SKBlendMode.SrcIn;
        canvas.DrawText(OnText, onTextX, textY, font, paint);
        canvas.DrawText(OffText, offTextX, textY, font, paint);

        // Reset mode
        canvas.Restore();
    }
}
