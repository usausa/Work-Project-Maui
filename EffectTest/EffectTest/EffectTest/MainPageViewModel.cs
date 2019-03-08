namespace EffectTest
{
    using Smart.ComponentModel;
    using Smart.Forms.Input;
    using Smart.Forms.ViewModels;

    using Xamarin.Forms;

    public class MainPageViewModel : ViewModelBase
    {
        public NotificationValue<BorderSetting> Setting { get; } = new NotificationValue<BorderSetting>();

        public DelegateCommand ChangeCommand { get; }

        private readonly BorderSetting[] settings =
        {
            new BorderSetting(),
            new BorderSetting { BackgroundColor = Color.LightPink, Width = 5, Color = Color.DeepPink, Radius = 8 },
            new BorderSetting { BackgroundColor = Color.LightGray },
            new BorderSetting { BackgroundColor = Color.LightYellow, Width = 1, Color = Color.Yellow, Radius = 1 },
            //new BorderSetting { BackgroundColor = Color.LightPink, Padding = new Thickness(3, 0, 3, 0), Width = 5, Color = Color.DeepPink, Radius = 5 },
            new BorderSetting { BackgroundColor = Color.LightBlue, Width = 1, Color = Color.Blue, Radius = 8 },
            new BorderSetting { BackgroundColor = Color.Orange, Radius = 8 },
            new BorderSetting { BackgroundColor = Color.LightPink, Width = 5, Color = Color.DeepPink },
            new BorderSetting { BackgroundColor = Color.LightBlue, Width = 0.2, Color = Color.Blue },
            new BorderSetting { BackgroundColor = Color.White, Padding = new Thickness(3, 0, 3, 0), Width = 0.2, Color = Color.Black },
            new BorderSetting { BackgroundColor = Color.Red, Radius = 12 },
        };

        private int index;

        public MainPageViewModel()
        {
            ChangeCommand = MakeDelegateCommand(ChangeSetting);
            Setting.Value = settings[index];
        }

        private void ChangeSetting()
        {
            index++;
            if (index >= settings.Length)
            {
                index = 0;
            }

            Setting.Value = settings[index];
        }
    }
}
