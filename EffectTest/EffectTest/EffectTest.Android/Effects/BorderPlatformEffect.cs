[assembly: Xamarin.Forms.ExportEffect(typeof(EffectTest.Droid.Effects.BorderPlatformEffect), nameof(EffectTest.Effects.BorderEffect))]

namespace EffectTest.Droid.Effects
{
    using System.ComponentModel;

    using Android.Graphics.Drawables;

    using EffectTest.Effects;

    using Xamarin.Forms;
    using Xamarin.Forms.Platform.Android;

    public sealed class BorderPlatformEffect : PlatformEffect
    {
        private Android.Views.View view;

        private Drawable oldDrawable;

        private GradientDrawable border;

        private Android.Graphics.Color color;

        private int width;

        private float radius;

        protected override void OnAttached()
        {
            view = Container ?? Control;
            oldDrawable = view.Background;
            border = new GradientDrawable();

            UpdateWidth();
            UpdateColor();
            UpdateRadius();
            UpdateBorder();
        }

        protected override void OnDetached()
        {
            if (oldDrawable != null)
            {
                view.Background = oldDrawable;
                oldDrawable = null;

                view.SetPadding(0, 0, 0, 0);
                view.ClipToOutline = false;
            }

            border?.Dispose();
            border = null;
            view = null;
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (args.PropertyName == BorderEffect.WidthProperty.PropertyName)
            {
                UpdateWidth();
                UpdateBorder();
            }
            else if (args.PropertyName == BorderEffect.ColorProperty.PropertyName)
            {
                UpdateColor();
                UpdateBorder();
            }
            else if (args.PropertyName == BorderEffect.RadiusProperty.PropertyName)
            {
                UpdateRadius();
                UpdateBorder();
            }
        }

        private void UpdateWidth()
        {
            width = (int)view.Context.ToPixels(BorderEffect.GetWidth(Element));
        }

        private void UpdateColor()
        {
            color = BorderEffect.GetColor(Element).ToAndroid();
        }

        private void UpdateRadius()
        {
            radius = view.Context.ToPixels(BorderEffect.GetRadius(Element));
        }

        private void UpdateBorder()
        {
            border.SetStroke(width, color);
            border.SetCornerRadius(radius);

            if (Element is BoxView boxView)
            {
                var backgroundColor = boxView.Color;
                if (backgroundColor != Color.Default)
                {
                    border.SetColor(backgroundColor.ToAndroid());
                }
            }
            else
            {
                var backgroundColor = ((View)Element).BackgroundColor;
                if (backgroundColor != Color.Default)
                {
                    border.SetColor(backgroundColor.ToAndroid());
                }
            }

            view.SetPadding(width, width, width, width);
            view.ClipToOutline = true;

            view.SetBackground(border);
        }
    }
}