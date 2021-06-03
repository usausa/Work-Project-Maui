[assembly: Xamarin.Forms.ExportEffect(typeof(WorkList.Droid.FocusablePlatformEffect), nameof(WorkList.FocusableEffect))]

namespace WorkList.Droid
{
    using Xamarin.Forms;
    using Xamarin.Forms.Platform.Android;

    public sealed class FocusablePlatformEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            ((VisualElement)Element).FocusChangeRequested += (sender, args) =>
            {
                args.Result = Control?.RequestFocus() ?? false;
            };
        }

        protected override void OnDetached()
        {
        }
    }
}
