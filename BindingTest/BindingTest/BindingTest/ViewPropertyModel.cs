namespace BindingTest
{
    using Smart.ComponentModel;

    public class ViewPropertyModel : NotificationObject
    {
        private string title;

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }
    }
}
