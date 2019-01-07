namespace Business.FormsApp.Droid.Components.Keyboard
{
    using Business.FormsApp.Components.Keyboard;

    public sealed class KeyboardManager : KeyboardManagerBase
    {
        public void UpdateState(bool visible)
        {
            RaiseStateChanged(visible);
        }
    }
}