[assembly: Xamarin.Forms.ExportEffect(typeof(KeySample.FormsApp.Droid.Effects.FocusablePlatformEffect), nameof(KeySample.FormsApp.Effects.FocusableEffect))]

namespace KeySample.FormsApp.Droid.Effects
{
    using Xamarin.Forms;
    using Xamarin.Forms.Platform.Android;

    public sealed class FocusablePlatformEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            ((VisualElement)Element).FocusChangeRequested += (_, args) =>
            {
                args.Result = Control?.RequestFocus() ?? false;
            };
        }

        protected override void OnDetached()
        {
        }
    }
}
