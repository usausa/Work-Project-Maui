using Smart.Forms.Input;

namespace BindingTest
{
    using Smart.ComponentModel;
    using Smart.Forms.ViewModels;

    public class View1Model : ViewModelBase
    {
        public NotificationValue<string> Title { get; } = new NotificationValue<string>("View1");

        public DelegateCommand<string> UpdateTitle { get; }

        public View1Model()
        {
            UpdateTitle = MakeDelegateCommand<string>(x => Title.Value = x);
        }
    }
}
