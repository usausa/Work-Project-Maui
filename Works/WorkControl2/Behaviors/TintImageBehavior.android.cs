using Android.Widget;

namespace WorkControl2.Behaviors;

public sealed partial class TintImageBehavior : PlatformBehavior<Image, Android.Widget.ImageView>
{
    protected override void OnAttachedTo(Image bindable, ImageView platformView)
    {
        base.OnAttachedTo(bindable, platformView);

        var color = TintColor;
        if (color is not null)
        {
            ImageExtensions.ApplyColor(platformView, color);
        }
        else
        {
            ImageExtensions.ClearColor(platformView);
        }
    }

    protected override void OnDetachedFrom(Image bindable, ImageView platformView)
    {
        ImageExtensions.ClearColor(platformView);

        base.OnDetachedFrom(bindable, platformView);
    }
}
