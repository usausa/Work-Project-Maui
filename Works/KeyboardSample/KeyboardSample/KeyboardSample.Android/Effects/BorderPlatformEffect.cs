[assembly: Xamarin.Forms.ExportEffect(typeof(KeyboardSample.Droid.Effects.BorderPlatformEffect), nameof(KeyboardSample.Effects.BorderEffect))]

namespace KeyboardSample.Droid.Effects
{
    using System;
    using System.ComponentModel;

    using Android.Graphics.Drawables;

    using KeyboardSample.Effects;

    using Xamarin.Forms;
    using Xamarin.Forms.Platform.Android;

    public sealed class BorderPlatformEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            UpdateBorder();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Ignore")]
        protected override void OnDetached()
        {
            var view = Container ?? Control;

            var current = view.Background;
            var backgroundColor = ResolveBackgroundColor();
            view.SetBackground(backgroundColor != Color.Default
                ? new ColorDrawable(backgroundColor.ToAndroid())
                : null);
            ((IDisposable)current)?.Dispose();

            Control?.SetPadding(0, 0, 0, 0);
            view.ClipToOutline = false;
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if ((args.PropertyName == Border.WidthProperty.PropertyName) ||
                (args.PropertyName == Border.ColorProperty.PropertyName) ||
                (args.PropertyName == Border.RadiusProperty.PropertyName) ||
                (args.PropertyName == Border.PaddingProperty.PropertyName) ||
                (args.PropertyName == VisualElement.BackgroundColorProperty.PropertyName))
            {
                UpdateBorder();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Ignore")]
        private void UpdateBorder()
        {
            var view = Container ?? Control;

            var (left, top, right, bottom) = Border.GetPadding(Element);
            var paddingLeft = (int)view.Context.ToPixels(left);
            var paddingTop = (int)view.Context.ToPixels(top);
            var paddingRight = (int)view.Context.ToPixels(right);
            var paddingBottom = (int)view.Context.ToPixels(bottom);
            var width = (int)view.Context.ToPixels(Border.GetWidth(Element));
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
            ((IDisposable)current)?.Dispose();
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
