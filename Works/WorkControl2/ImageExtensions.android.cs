using Android.Graphics;
using Android.Widget;
using Microsoft.Maui.Platform;

namespace WorkControl2;

public static class ImageExtensions
{
    public static void ApplyColor(ImageView imageView, Microsoft.Maui.Graphics.Color color)
    {
        imageView.SetColorFilter(new PorterDuffColorFilter(color.ToPlatform(), PorterDuff.Mode.SrcIn ?? throw new NullReferenceException()));
    }

    public static void ClearColor(ImageView imageView)
    {
        imageView.ClearColorFilter();
    }
}
