namespace Business.FormsApp
{
    using Smart.ComponentModel;
    using Smart.Forms.ViewModels;

    public class ApplicationState : NotificationObject, IBusyState
    {
        private bool isBusy;

        private bool keyboardVisible;

        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        public bool KeyboardVisible
        {
            get => keyboardVisible;
            set => SetProperty(ref keyboardVisible, value);
        }
    }
}
