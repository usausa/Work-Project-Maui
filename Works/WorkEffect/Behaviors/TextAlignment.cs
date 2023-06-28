//namespace WorkEffect.Behaviors;

//using Microsoft.Maui.Handlers;

//public static class TextAlignment
//{
//    public static readonly BindableProperty OnProperty = BindableProperty.CreateAttached(
//        "On",
//        typeof(bool),
//        typeof(TextAlignment),
//        false);

//    public static bool GetOn(BindableObject bindable) => (bool)bindable.GetValue(OnProperty);

//    public static void SetOn(BindableObject bindable, bool value) => bindable.SetValue(OnProperty, value);

//    public static readonly BindableProperty VerticalProperty =
//        BindableProperty.CreateAttached(
//            "Vertical",
//            typeof(Microsoft.Maui.TextAlignment),
//            typeof(TextAlignment),
//            Microsoft.Maui.TextAlignment.Center);

//    public static Microsoft.Maui.TextAlignment GetVertical(BindableObject bindable) =>
//        (Microsoft.Maui.TextAlignment)bindable.GetValue(VerticalProperty);

//    public static void SetVertical(BindableObject bindable, Microsoft.Maui.TextAlignment value) =>
//        bindable.SetValue(VerticalProperty, value);

//    public static readonly BindableProperty HorizontalProperty =
//        BindableProperty.CreateAttached(
//            "Horizontal",
//            typeof(Microsoft.Maui.TextAlignment),
//            typeof(TextAlignment),
//            Microsoft.Maui.TextAlignment.Center);

//    public static Microsoft.Maui.TextAlignment GetHorizontal(BindableObject bindable) =>
//        (Microsoft.Maui.TextAlignment)bindable.GetValue(HorizontalProperty);

//    public static void SetHorizontal(BindableObject bindable, Microsoft.Maui.TextAlignment value) =>
//        bindable.SetValue(HorizontalProperty, value);

//    public static void UseCustomMapper()
//    {
//#if ANDROID
//        LabelHandler.Mapper.Add("On", (handler, _) =>
//        {
//            var element = (Label)handler.VirtualView;
//            var on = GetOn(element);
//            if (on)
//            {
//                handler.PlatformView.Gravity = GetVertical(element).ToVerticalGravity() |
//                                               GetHorizontal(element).ToHorizontalGravity();
//            }
//        });
//#endif
//    }
//}
