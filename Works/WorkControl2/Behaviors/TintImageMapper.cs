using Microsoft.Maui.Handlers;

namespace WorkControl2.Behaviors;

public static class TintImageMapper
{
    public static readonly BindableProperty TintColorProperty = BindableProperty.CreateAttached(
        "TintColor",
        typeof(Color),
        typeof(Image),
        null);

    public static Color GetTintColor(BindableObject view) => (Color)view.GetValue(TintColorProperty);

    public static void SetTintColor(BindableObject view, Color? value) => view.SetValue(TintColorProperty, value);

    public static void ApplyTintColor()
    {
        ImageHandler.Mapper.Add("TintColor", (handler, view) =>
        {
            var tintColor = GetTintColor((Image)handler.VirtualView);

            if (tintColor is not null)
            {
#if ANDROID
                ImageExtensions.ApplyColor(handler.PlatformView, tintColor);
#endif
            }
            else
            {
#if ANDROID
                ImageExtensions.ClearColor(handler.PlatformView);
#endif
            }
        });
    }
}
