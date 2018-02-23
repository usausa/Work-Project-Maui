namespace BindingTest
{
    using Smart.ComponentModel;
    using Smart.Forms.Input;
    using Smart.Forms.ViewModels;

    public class View2Model : ViewModelBase
    {
        public NotificationValue<string> Title { get; } = new NotificationValue<string>("View2");

        public DelegateCommand<string> UpdateTitle { get; }

        public View2Model()
        {
            UpdateTitle = MakeDelegateCommand<string>(x => Title.Value = x);
        }
    }
}
