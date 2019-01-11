namespace Business.FormsApp.Modules.Platform
{
    using System;
    using System.Threading.Tasks;

    using Smart.Forms.Input;
    using Smart.Navigation;

    using Xamarin.Essentials;

    public class PlatformActionViewModel : AppViewModelBase
    {
        public AsyncCommand VibrateCommand { get; }
        public AsyncCommand SpeakCommand { get; }

        public PlatformActionViewModel(ApplicationState applicationState)
            : base(applicationState)
        {
            VibrateCommand = MakeAsyncCommand(Vibrate);
            SpeakCommand = MakeAsyncCommand(Speak);
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.PlatformMenu);
        }

        private Task Vibrate()
        {
            try
            {
                Vibration.Vibrate();
            }
            catch (Exception)
            {
                // ignored
            }

            return Task.CompletedTask;
        }

        private async Task Speak()
        {
            await TextToSpeech.SpeakAsync("Hello World");
        }
    }
}
