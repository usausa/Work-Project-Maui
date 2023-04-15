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
            var backgroundColor = ResolveBackgroundColor();
            view.SetBackground(backgroundColor != Color.Default
                ? new ColorDrawable(backgroundColor.ToAndroid())
                : null);
            ((IDisposable) current)?.Dispose();

            Control?.SetPadding(0, 0, 0, 0);
            view.ClipToOutline = false;
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (args.PropertyName == Border.WidthProperty.PropertyName)
            {
                UpdateBorder();
            }
            else if (args.PropertyName == Border.ColorProperty.PropertyName)
            {
                UpdateBorder();
            }
            else if (args.PropertyName == Border.RadiusProperty.PropertyName)
            {
                UpdateBorder();
            }
            else if (args.PropertyName == Border.PaddingProperty.PropertyName)
            {
                UpdateBorder();
            }
            else if (args.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
            {
                UpdateBorder();
            }
        }

        private void UpdateBorder()
        {
            var view = Container ?? Control;

            var padding = Border.GetPadding(Element);
            var paddingLeft = (int) view.Context.ToPixels(padding.Left);
            var paddingTop = (int) view.Context.ToPixels(padding.Top);
            var paddingRight = (int) view.Context.ToPixels(padding.Right);
            var paddingBottom = (int) view.Context.ToPixels(padding.Bottom);
            var width = (int) view.Context.ToPixels(Border.GetWidth(Element));
            var color = Border.GetColor(Element).ToAndroid();
            var radius = view.Context.ToPixels(Border.GetRadius(Element));

            var border = new GradientDrawable();

            border.SetStroke(width, color);
            border.SetCornerRadius(radius);

            var backgroundColor = ResolveBackgroundColor();
            if (backgroundColor != Color.Default)
            {
                border.SetColor(backgroundColor.ToAndroid());
            }

            Control?.SetPadding(paddingLeft, paddingTop, paddingRight, paddingBottom);
            view.ClipToOutline = true;

            var current = view.Background;
            view.SetBackground(border);
            ((IDisposable) current)?.Dispose();
        }

        private Color ResolveBackgroundColor()
        {
            if (Element is BoxView boxView)
            {
                return boxView.Color;
            }

            return ((View)Element).BackgroundColor;
        }
    }
}