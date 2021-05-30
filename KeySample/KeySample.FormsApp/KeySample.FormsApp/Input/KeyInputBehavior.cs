namespace KeySample.FormsApp.Input
{
    using System.Linq;

    using Smart.Forms.Interactivity;

    using Xamarin.Forms;

    public class KeyInputBehavior : BehaviorBase<Page>, IKeyInputHandler
    {
        protected override void OnAttachedTo(Page bindable)
        {
            base.OnAttachedTo(bindable);

            KeyInputManager.Default.PushHandler(this);
        }

        protected override void OnDetachingFrom(Page bindable)
        {
            KeyInputManager.Default.PopHandler(this);

            base.OnDetachingFrom(bindable);
        }

        public bool Handle(KeyCode key)
        {
            if (AssociatedObject is null)
            {
                return false;
            }

            if (key == KeyCode.Up)
            {
                ElementHelper.MoveFocus(AssociatedObject, false);
                return true;
            }

            if (key == KeyCode.Down)
            {
                ElementHelper.MoveFocus(AssociatedObject, true);
                return true;
            }

            if (((key >= KeyCode.Num0) && (key <= KeyCode.Num9)) ||
                ((key >= KeyCode.Function1) && (key <= KeyCode.Function4)))
            {
                var button = (Button?)ElementHelper.EnumerateActive(AssociatedObject)
                    .FirstOrDefault(x => x is Button b && Shortcut.GetKey(b) == key);
                button?.SendClicked();
            }

            return false;
        }
    }
}
