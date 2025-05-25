using System.Diagnostics;
using Android.Views;
using TextAlignment = Microsoft.Maui.TextAlignment;

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
    //public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create(
    //    nameof(HorizontalTextAlignment),
    //    typeof(TextAlignment),
    //    typeof(ButtonEx),
    //    TextAlignment.Center);

    //public static readonly BindableProperty VerticalTextAlignmentProperty = BindableProperty.Create(
    //    nameof(VerticalTextAlignment),
    //    typeof(TextAlignment),
    //    typeof(ButtonEx),
    //    TextAlignment.Center);

    //public TextAlignment HorizontalTextAlignment
    //{
    //    get => (TextAlignment)GetValue(HorizontalTextAlignmentProperty);
    //    set => SetValue(HorizontalTextAlignmentProperty, value);
    //}

    //public TextAlignment VerticalTextAlignment
    //{
    //    get => (TextAlignment)GetValue(VerticalTextAlignmentProperty);
    //    set => SetValue(VerticalTextAlignmentProperty, value);
    //}

//     [MEMO] Loaded以降でないとHandlerがないのでpropertyChangedを使う方法は不適切？
//    private static void OnTextAlignmentChanged(BindableObject bindable, object oldValue, object newValue)
//    {
//        var button = (ButtonEx)bindable;

//#if ANDROID
//        if (button.Handler?.PlatformView is Android.Widget.TextView view)
//        {
//            var horizontalFlag = button.HorizontalTextAlignment switch
//            {
//                TextAlignment.Start => Android.Views.GravityFlags.Left,
//                TextAlignment.Center => Android.Views.GravityFlags.CenterHorizontal,
//                TextAlignment.End => Android.Views.GravityFlags.Right,
//                _ => Android.Views.GravityFlags.Center
//            };
//            var verticalFlag = button.VerticalTextAlignment switch
//            {
//                TextAlignment.Start => Android.Views.GravityFlags.Top,
//                TextAlignment.Center => Android.Views.GravityFlags.CenterVertical,
//                TextAlignment.End => Android.Views.GravityFlags.Bottom,
//                _ => Android.Views.GravityFlags.Center
//            };
//            view.Gravity = horizontalFlag | verticalFlag;
//        }
//#endif
//    }
}

// TODO 継承なしで？、その場合Effectで特定したものだけにする必要性？、Nameでわかるか？
internal static class AppHostBuilderExtensions
{
    public static MauiAppBuilder ConfigureCustomBehaviors(this MauiAppBuilder builder)
    {
        //ButtonHandler.Mapper.AppendToMapping("TextAlignment", UpdateTextAlignment);
        Buttons.UseCustomMapper();

        return builder;
    }

    //    private static void UpdateTextAlignment(IButtonHandler handler, IButton view)
    //    {
    //        if (view is ButtonEx button)
    //        {
    //#if IOS
    //            //handler.PlatformView.VerticalAlignment = UIControlContentVerticalAlignment.Bottom;
    //            //handler.PlatformView.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
    //#elif ANDROID
    //            handler.PlatformView.Gravity = button.HorizontalTextAlignment.ToHorizontalGravity() |
    //                                           button.VerticalTextAlignment.ToVerticalGravity();
    //#endif
    //        }
    //    }
}

public static class Buttons
{
    // TODO enable?

    public static readonly BindableProperty EnableTextAlignmentProperty = BindableProperty.CreateAttached(
        "EnableTextAlignment",
        typeof(bool),
        typeof(Buttons),
        false);

    public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.CreateAttached(
        "HorizontalTextAlignment",
        typeof(TextAlignment),
        typeof(Buttons),
        TextAlignment.Center);

    public static readonly BindableProperty VerticalTextAlignmentProperty = BindableProperty.CreateAttached(
        "VerticalTextAlignment",
        typeof(TextAlignment),
        typeof(Buttons),
        TextAlignment.Center);

    public static bool GetEnableTextAlignment(BindableObject bindable) => (bool)bindable.GetValue(EnableTextAlignmentProperty);

    public static void SetEnableTextAlignment(BindableObject bindable, bool value) => bindable.SetValue(EnableTextAlignmentProperty, value);


    public static TextAlignment GetHorizontalTextAlignment(BindableObject bindable) => (TextAlignment)bindable.GetValue(HorizontalTextAlignmentProperty);

    public static void SetHorizontalTextAlignment(BindableObject bindable, TextAlignment value) => bindable.SetValue(HorizontalTextAlignmentProperty, value);

    public static TextAlignment GetVerticalTextAlignment(BindableObject bindable) => (TextAlignment)bindable.GetValue(VerticalTextAlignmentProperty);

    public static void SetVerticalTextAlignment(BindableObject bindable, TextAlignment value) => bindable.SetValue(VerticalTextAlignmentProperty, value);

    public static void UseCustomMapper()
    {
        ButtonHandler.Mapper.AppendToMapping("TextAlignment", UpdateTextAlignment);
    }

    private static void UpdateTextAlignment(IButtonHandler handler, IButton view)
    {
        if ((view is Button button) && GetEnableTextAlignment(button))
        {
            Debug.WriteLine($"* {button.Text} {GetHorizontalTextAlignment(button)} {GetVerticalTextAlignment(button)}");
#if IOS
                //handler.PlatformView.VerticalAlignment = UIControlContentVerticalAlignment.Bottom;
                //handler.PlatformView.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
#elif ANDROID
            handler.PlatformView.Gravity = GetHorizontalTextAlignment(button).ToHorizontalGravity() |
                                           GetVerticalTextAlignment(button).ToVerticalGravity();
#endif
        }
    }
}

public static class AlignmentExtensions
{
    public static GravityFlags ToHorizontalGravity(this TextAlignment alignment)
    {
        return alignment switch
        {
            TextAlignment.Center => GravityFlags.CenterHorizontal,
            TextAlignment.Start => GravityFlags.Right,
            TextAlignment.End => GravityFlags.Left,
            _ => GravityFlags.Center
        };
    }

    public static GravityFlags ToVerticalGravity(this TextAlignment alignment)
    {
        return alignment switch
        {
            TextAlignment.Center => GravityFlags.CenterVertical,
            TextAlignment.Start => GravityFlags.Top,
            TextAlignment.End => GravityFlags.Bottom,
            _ => GravityFlags.Center
        };
    }
}

