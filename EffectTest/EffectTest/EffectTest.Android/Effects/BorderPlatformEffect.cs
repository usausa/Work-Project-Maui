[assembly: Xamarin.Forms.ExportEffect(typeof(EffectTest.Droid.Effects.BorderPlatformEffect), nameof(EffectTest.Effects.BorderEffect))]

namespace EffectTest.Droid.Effects
{
    using System;
    using System.ComponentModel;

    using Android.Graphics.Drawables;

    using EffectTest.Effects;

    using Xamarin.Forms;
    using Xamarin.Forms.Platform.Android;

    public sealed class BorderPlatformEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            UpdateBorder();
        }

        protected override void OnDetached()
        {
            var view = Container ?? Control;

            var current = view.Background;
            view.SetBackground(null);
            ((IDisposable)current)?.Dispose();

            Control?.SetPadding(0, 0, 0, 0);
            view.ClipToOutline = false;
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (args.PropertyName == BorderEffect.WidthProperty.PropertyName)
            {
                UpdateBorder();
            }
            else if (args.PropertyName == BorderEffect.ColorProperty.PropertyName)
            {
                UpdateBorder();
            }
            else if (args.PropertyName == BorderEffect.RadiusProperty.PropertyName)
            {
                UpdateBorder();
            }

            // TODO Background?
        }

        private void UpdateBorder()
        {
            var view = Container ?? Control;

            var padding = BorderEffect.GetPadding(Element);
            var paddingLeft = (int)view.Context.ToPixels(padding.Left);
            var paddingTop = (int)view.Context.ToPixels(padding.Top);
            var paddingRight = (int)view.Context.ToPixels(padding.Right);
            var paddingBottom = (int)view.Context.ToPixels(padding.Bottom);

            var width = (int)view.Context.ToPixels(BorderEffect.GetWidth(Element));
            var color = BorderEffect.GetColor(Element).ToAndroid();
            var radius = view.Context.ToPixels(BorderEffect.GetRadius(Element));

            var border = new GradientDrawable();

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

            Control?.SetPadding(width + paddingLeft, width + paddingTop, width + paddingRight, width + paddingBottom);
            view.ClipToOutline = true;

            var current = view.Background;
            view.SetBackground(border);
            ((IDisposable)current)?.Dispose();
        }
    }
}