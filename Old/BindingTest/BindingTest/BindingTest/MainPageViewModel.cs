namespace BindingTest
{
    using Smart.ComponentModel;

    public class MainPageViewModel : NotificationObject, IViewPropertySink
    {
        private string title;

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }
    }
}
