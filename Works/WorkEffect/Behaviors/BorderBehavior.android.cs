#nullable enable
namespace WorkEffect.Behaviors;

using Android.Graphics.Drawables;

using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Platform;

public sealed class BorderBehavior : PlatformBehavior<VisualElement, Android.Views.View>
{
    private Drawable? originalDrawable;

    private GradientDrawable drawable = default!;

    private VisualElement? element;

    private Android.Views.View view = default!;


    protected override void OnAttachedTo(VisualElement bindable, Android.Views.View platformView)
    {
        base.OnAttachedTo(bindable, platformView);

        element = bindable;
        view = platformView;

        originalDrawable = platformView.Background;
        drawable = new GradientDrawable();
        platformView.Background = drawable;

        UpdateBorder();
    }

    protected override void OnDetachedFrom(VisualElement bindable, Android.Views.View platformView)
    {
        platformView.Background = originalDrawable;
        drawable.Dispose();

        element = null;
        view = null!;

        base.OnDetachedFrom(bindable, platformView);
    }

    internal void UpdateBorder()
    {
        if (element is null)
        {
            return;
        }

        var width = (int)view.Context.ToPixels(Border.GetBorderWidth(element));
        var color = Border.GetBorderColor(element).ToAndroid();
        drawable.SetStroke(width, color);
        drawable.SetColor(element.BackgroundColor.ToAndroid());
    }
}
