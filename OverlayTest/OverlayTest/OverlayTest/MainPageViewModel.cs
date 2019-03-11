namespace OverlayTest
{
    using Smart.ComponentModel;
    using Smart.Forms.Input;
    using Smart.Forms.ViewModels;

    public class MainPageViewModel : ViewModelBase
    {
        public NotificationValue<bool> IsOverlay { get; } = new NotificationValue<bool>();

        public DelegateCommand ShowCommand { get; }
        public DelegateCommand CloseCommand { get; }

        public MainPageViewModel()
        {
            ShowCommand = MakeDelegateCommand(() => IsOverlay.Value = true);
            CloseCommand = MakeDelegateCommand(() => IsOverlay.Value = false);
        }
    }
}
