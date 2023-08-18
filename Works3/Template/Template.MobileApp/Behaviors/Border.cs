namespace Template.MobileApp.Behaviors;

#if ANDROID
using Android.Graphics.Drawables;
#endif

using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

public static class Border
{
    // ReSharper disable InconsistentNaming
    public static readonly BindableProperty WidthProperty =
        BindableProperty.CreateAttached(
            "Width",
            typeof(double),
            typeof(Border),
            default(double));

    public static readonly BindableProperty ColorProperty =
        BindableProperty.CreateAttached(
            "Color",
            typeof(Color),
            typeof(Border),
            Colors.Transparent);
    // ReSharper restore InconsistentNaming

    public static void SetWidth(BindableObject bindable, double value) => bindable.SetValue(WidthProperty, value);

    public static double GetWidth(BindableObject bindable) => (double)bindable.GetValue(WidthProperty);

    public static void SetColor(BindableObject bindable, Color value) => bindable.SetValue(ColorProperty, value);

    public static Color GetColor(BindableObject bindable) => (Color)bindable.GetValue(ColorProperty);

    public static void UseCustomMapper(BehaviorOptions options)
    {
#if ANDROID
        if (options.Border)
        {
            EntryHandler.Mapper.Add("Width", static (handler, _) => UpdateBehaviors((Entry)handler.VirtualView));
            EntryHandler.Mapper.Add("Color", static (handler, _) => UpdateBehaviors((Entry)handler.VirtualView));

            EditorHandler.Mapper.Add("Width", static (handler, _) => UpdateBehaviors((Editor)handler.VirtualView));
            EditorHandler.Mapper.Add("Color", static (handler, _) => UpdateBehaviors((Editor)handler.VirtualView));

            LabelHandler.Mapper.Add("Width", static (handler, _) => UpdateBehaviors((Label)handler.VirtualView));
            LabelHandler.Mapper.Add("Color", static (handler, _) => UpdateBehaviors((Label)handler.VirtualView));
        }
#endif
    }

#if ANDROID
    private static void UpdateBehaviors(VisualElement element)
    {
        var width = GetWidth(element);
        var on = width > 0;
        var behavior = element.Behaviors.OfType<BorderBehavior>().FirstOrDefault();
        if (on)
        {
            if (behavior is not null)
            {
                behavior.UpdateBorder();
            }
            else
            {
                element.Behaviors.Add(new BorderBehavior());
            }
        }
        else if (behavior is not null)
        {
            element.Behaviors.Remove(behavior);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Ignore")]
    private sealed class BorderBehavior : PlatformBehavior<VisualElement, Android.Views.View>
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

            var width = (int)view.Context.ToPixels(GetWidth(element));
            var color = GetColor(element).ToAndroid();
            drawable.SetStroke(width, color);
            drawable.SetColor(element.BackgroundColor.ToAndroid());
        }
    }
#endif
}
