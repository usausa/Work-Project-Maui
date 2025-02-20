namespace DeviceSample.Modules.Device;

using Plugin.Maui.Audio;

public class CameraViewModel : AppViewModelBase
{
    private readonly IFileSystem fileSystem;

    private readonly IAudioManager audioManager;

#pragma warning disable CA2213
    private IAudioPlayer? audioPlayer;
#pragma warning restore CA2213

    public NotificationValue<string> Barcode { get; } = new();

    public CameraViewModel(
        ApplicationState applicationState,
        IFileSystem fileSystem,
        IAudioManager audioManager)
        : base(applicationState)
    {
        this.fileSystem = fileSystem;
        this.audioManager = audioManager;

        //Camera = new CameraController(CameraPosition.Back, MakeDelegateCommand<BarcodeResult>(x =>
        //{
        //    Barcode.Value = x.Text;
        //    audioPlayer?.Play();
        //}))
        //{
        //    BarcodeDetection = true
        //};
    }

    public override async void OnNavigatedTo(INavigationContext context)
    {
        if (!context.Attribute.IsRestore())
        {
            audioPlayer = audioManager.CreatePlayer(await fileSystem.OpenAppPackageFileAsync("Read.wav"));
            Disposables.Add(audioPlayer);
        }

        //await Navigator.PostActionAsync(() => BusyState.UsingAsync(() => Camera.StartPreviewAsync()));
    }

    //public override async void OnNavigatingFrom(INavigationContext context)
    //{
    //    await Camera.StopPreviewAsync();
    //}

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.Menu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
