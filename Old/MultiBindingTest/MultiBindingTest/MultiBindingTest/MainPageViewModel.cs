namespace MultiBindingTest
{
    using Smart.ComponentModel;

    public class MainPageViewModel
    {
        public NotificationValue<bool> IsChecked1 { get; } = new NotificationValue<bool>();

        public NotificationValue<bool> IsChecked2 { get; } = new NotificationValue<bool>();
    }
}
