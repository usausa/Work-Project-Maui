namespace WorkTimer
{
    public partial class MainPage : ContentPage
    {
        private readonly TimerService timerService = new();

        public MainPage()
        {
            InitializeComponent();

            timerService.Handle += TimerServiceOnHandle;
            UpdateStatus();
        }

        private async void OnStartButtonClicked(object sender, EventArgs e)
        {
            await timerService.StartAsync();
            UpdateStatus();
        }

        private async void OnStopButtonClicked(object sender, EventArgs e)
        {
            await timerService.StopAsync();
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            RunningLabel.Text = timerService.IsRunning ? "Running" : "Stop";
        }

        private void TimerServiceOnHandle(object? sender, EventArgs e)
        {
            Dispatcher.Dispatch(() => ValueLabel.Text = DateTime.Now.ToString("HH:mm:ss"));
        }
    }
}
