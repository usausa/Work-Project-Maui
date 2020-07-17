namespace KeyboardSample
{
    using KeyboardSample.Input;

    using Smart.ComponentModel;
    using Smart.Forms.ViewModels;

    public class MainPageViewModel : ViewModelBase
    {
        public InputModel Input1 { get; }

        public InputModel Input2 { get; }

        public NotificationValue<string> Previous1 { get; } = new NotificationValue<string>();

        public NotificationValue<string> Previous2 { get; } = new NotificationValue<string>();

        public MainPageViewModel()
        {
            Input1 = new InputModel(MakeDelegateCommand<string>(Input1Completed));
            Input2 = new InputModel(false, MakeDelegateCommand<string>(Input2Completed));
        }

        private void Input1Completed(string value)
        {
            Input2.Enable = true;
            Input2.FocusRequest();
        }

        private void Input2Completed(string value)
        {
            Previous1.Value = Input1.Text;
            Previous2.Value = Input2.Text;

            Input1.Text = string.Empty;
            Input2.Text = string.Empty;

            Input1.FocusRequest();
            Input2.Enable = false;
        }
    }
}
