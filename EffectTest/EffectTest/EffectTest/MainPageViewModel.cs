namespace EffectTest
{
    using Smart.ComponentModel;
    using Smart.Forms.Input;
    using Smart.Forms.ViewModels;

    using Xamarin.Forms;

    public class MainPageViewModel : ViewModelBase
    {
        public NotificationValue<Color> BackgroundColor { get; } = new NotificationValue<Color>();

        public NotificationValue<Thickness> Padding { get; } = new NotificationValue<Thickness>();

        public NotificationValue<Color> BorderColor { get; } = new NotificationValue<Color>(Color.Black);

        public NotificationValue<double> BorderWidth { get; } = new NotificationValue<double>();

        public NotificationValue<double> BorderRadius { get; } = new NotificationValue<double>();

        public DelegateCommand<Color> BackgroundColorCommand { get; }
        public DelegateCommand<double> PaddingCommand { get; }
        public DelegateCommand<Color> ColorCommand { get; }
        public DelegateCommand<double> WidthCommand { get; }
        public DelegateCommand<double> RadiusCommand { get; }

        public MainPageViewModel()
        {
            BackgroundColorCommand = MakeDelegateCommand<Color>(x => BackgroundColor.Value = x);
            PaddingCommand = MakeDelegateCommand<double>(x => Padding.Value = new Thickness(x, 0, x, 0));
            ColorCommand = MakeDelegateCommand<Color>(x => BorderColor.Value = x);
            WidthCommand = MakeDelegateCommand<double>(x => BorderWidth.Value = x);
            RadiusCommand = MakeDelegateCommand<double>(x => BorderRadius.Value = x);
        }
    }
}
