using System.Diagnostics;

namespace WorkControl2.Controls;

public class TintImage : Image
{
    public static readonly BindableProperty TintColorProperty = BindableProperty.Create(
        nameof(TintColor),
        typeof(Color),
        typeof(Image),
        propertyChanged: OnTintColorChanged);

    public Color? TintColor
    {
        get => (Color?)GetValue(TintColorProperty);
        set => SetValue(TintColorProperty, value);
    }

    static void OnTintColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (TintImage)bindable;
        var tintColor = control.TintColor;

        if ((control.Handler is null) || (control.Handler.PlatformView is null))
        {
            Debug.WriteLine("* HandlerChanged add");
            // Workaround for when this executes the Handler and PlatformView is null
            control.HandlerChanged += OnHandlerChanged;
            return;
        }

        if (tintColor is not null)
        {
#if ANDROID
            ImageExtensions.ApplyColor((Android.Widget.ImageView)control.Handler.PlatformView, tintColor);
#endif
        }
        else
        {
#if ANDROID
            ImageExtensions.ClearColor((Android.Widget.ImageView)control.Handler.PlatformView);
#endif
        }

        void OnHandlerChanged(object s, EventArgs e)
        {
            Debug.WriteLine("* HandlerChanged remove");
            OnTintColorChanged(control, oldValue, newValue);
            control.HandlerChanged -= OnHandlerChanged;
        }
    }
}
