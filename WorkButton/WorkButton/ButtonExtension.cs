namespace WorkButton;

using Microsoft.Maui.Handlers;

#if IOS
using UIKit;
using Foundation;
#endif

#if ANDROID
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
#endif

public class ButtonEx : Button
{
    public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create(
        nameof(HorizontalTextAlignment),
        typeof(TextAlignment),
        typeof(ButtonEx),
        TextAlignment.Center);

    public static readonly BindableProperty VerticalTextAlignmentProperty = BindableProperty.Create(
        nameof(VerticalTextAlignment),
        typeof(TextAlignment),
        typeof(ButtonEx),
        TextAlignment.Center);

    public TextAlignment HorizontalTextAlignment
    {
        get => (TextAlignment)GetValue(HorizontalTextAlignmentProperty);
        set => SetValue(HorizontalTextAlignmentProperty, value);
    }

    public TextAlignment VerticalTextAlignment
    {
        get => (TextAlignment)GetValue(VerticalTextAlignmentProperty);
        set => SetValue(VerticalTextAlignmentProperty, value);
    }

    // [MEMO] Loaded以降でないとHandlerがないのでpropertyChangedを使う方法は不適切？
    private static void OnTextAlignmentChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var button = (ButtonEx)bindable;

#if ANDROID
        if (button.Handler?.PlatformView is Android.Widget.TextView view)
        {
            var horizontalFlag = button.HorizontalTextAlignment switch
            {
                TextAlignment.Start => Android.Views.GravityFlags.Left,
                TextAlignment.Center => Android.Views.GravityFlags.CenterHorizontal,
                TextAlignment.End => Android.Views.GravityFlags.Right,
                _ => Android.Views.GravityFlags.Center
            };
            var verticalFlag = button.VerticalTextAlignment switch
            {
                TextAlignment.Start => Android.Views.GravityFlags.Top,
                TextAlignment.Center => Android.Views.GravityFlags.CenterVertical,
                TextAlignment.End => Android.Views.GravityFlags.Bottom,
                _ => Android.Views.GravityFlags.Center
            };
            view.Gravity = horizontalFlag | verticalFlag;
        }
#endif
    }
}

// TODO 継承なしで？、その場合Effectで特定したものだけにする必要性
internal static class AppHostBuilderExtensions
{
    public static MauiAppBuilder ConfigureCustomBehaviors(this MauiAppBuilder builder)
    {
        Microsoft.Maui.Handlers.ButtonHandler.Mapper.AppendToMapping("TextAlignment", UpdateTextAlignment);

        return builder;
    }

    private static void UpdateTextAlignment(IButtonHandler handler, IButton view)
    {
        if (view is ButtonEx button)
        {
#if IOS
            //handler.PlatformView.VerticalAlignment = UIControlContentVerticalAlignment.Bottom;
            //handler.PlatformView.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
#elif ANDROID
            var horizontalFlag = button.HorizontalTextAlignment switch
            {
                TextAlignment.Start => Android.Views.GravityFlags.Left,
                TextAlignment.Center => Android.Views.GravityFlags.CenterHorizontal,
                TextAlignment.End => Android.Views.GravityFlags.Right,
                _ => Android.Views.GravityFlags.Center
            };
            var verticalFlag = button.VerticalTextAlignment switch
            {
                TextAlignment.Start => Android.Views.GravityFlags.Top,
                TextAlignment.Center => Android.Views.GravityFlags.CenterVertical,
                TextAlignment.End => Android.Views.GravityFlags.Bottom,
                _ => Android.Views.GravityFlags.Center
            };
            handler.PlatformView.Gravity = horizontalFlag | verticalFlag;
#endif
        }
    }
}
