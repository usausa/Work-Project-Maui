[assembly: Xamarin.Forms.ExportEffect(typeof(WorkCustomMove.Droid.Effects.FocusablePlatformEffect), nameof(WorkCustomMove.Effects.FocusableEffect))]

namespace WorkCustomMove.Droid.Effects
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
