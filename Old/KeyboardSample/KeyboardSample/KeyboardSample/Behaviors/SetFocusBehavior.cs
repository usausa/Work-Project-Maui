namespace KeyboardSample.Behaviors
{
    using Smart.Forms.Interactivity;

    using Xamarin.Forms;

    public sealed class SetFocusBehavior : BehaviorBase<VisualElement>
    {
        protected override void OnAttachedTo(VisualElement bindable)
        {
            base.OnAttachedTo(bindable);

            Device.BeginInvokeOnMainThread(() =>
            {
                bindable.Focus();
            });
        }
    }
}
