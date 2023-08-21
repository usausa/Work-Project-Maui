namespace WorkSound;

using System.Windows.Input;

using Plugin.Maui.Audio;

using Smart.Maui.ViewModels;


public class MainPageViewModel : ViewModelBase
{
    private readonly IAudioManager audioManager;

    private IAudioPlayer? audioPlayer;

    public ICommand InitializeCommand { get; }

    public ICommand PlayCommand { get; }
    public ICommand PauseCommand { get; }
    public ICommand StopCommand { get; }

    public MainPageViewModel()
    {
        audioManager = AudioManager.Current;

        InitializeCommand = MakeAsyncCommand(async () =>
        {
            audioPlayer = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("read.wav"));
            Disposables.Add(audioPlayer);
        });

        PlayCommand = MakeDelegateCommand(() => audioPlayer?.Play());
        PauseCommand = MakeDelegateCommand(() =>
        {
            if (audioPlayer is not null)
            {
                if (audioPlayer.IsPlaying)
                {
                    audioPlayer.Stop();
                }
                else
                {
                    audioPlayer.Play();
                }
            }
        });
        StopCommand = MakeDelegateCommand(() => audioPlayer?.Stop());
    }
}
