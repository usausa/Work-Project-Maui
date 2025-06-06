namespace Template.MobileApp.Modules.Network;

using Template.MobileApp.Services;
using Template.MobileApp.Usecase;

public sealed class NetworkMenuViewModel : AppViewModelBase
{
    public IObserveCommand ForwardCommand { get; }
    public IObserveCommand ServerTimeCommand { get; }
    public IObserveCommand DataListCommand { get; }
    public IObserveCommand SecureCommand { get; }
    public IObserveCommand LoginCommand { get; }
    public IObserveCommand LogoutCommand { get; }
    public IObserveCommand TestErrorCommand { get; }
    public IObserveCommand TestDelayCommand { get; }
    public IObserveCommand DownloadCommand { get; }
    public IObserveCommand UploadCommand { get; }

    public NetworkMenuViewModel(
        ApiContext apiContext,
        SampleUsecase sampleUsecase)
    {
        var configured = apiContext.BaseAddress is not null;

        ForwardCommand = MakeAsyncCommand<ViewId>(x => Navigator.ForwardAsync(x));
        ServerTimeCommand = MakeAsyncCommand(async () => await sampleUsecase.GetServerTimeAsync(), () => configured);
        DataListCommand = MakeAsyncCommand(async () => await sampleUsecase.GetDataListAsync(), () => configured);
        DownloadCommand = MakeAsyncCommand(async () => await sampleUsecase.DownloadAsync(), () => configured);
        SecureCommand = MakeAsyncCommand(async () => await sampleUsecase.GetSecretMessageAsync(), () => configured);
        LoginCommand = MakeAsyncCommand(async () => await sampleUsecase.PostAccountLoginAsync("user"), () => configured);
        LogoutCommand = MakeDelegateCommand(sampleUsecase.AccountLogout, () => configured);
        UploadCommand = MakeAsyncCommand(async () => await sampleUsecase.UploadAsync(), () => configured);
        TestErrorCommand = MakeAsyncCommand<int>(async x => await sampleUsecase.GetTestErrorAsync(x), _ => configured);
        TestDelayCommand = MakeAsyncCommand<int>(async x => await sampleUsecase.GetTestDelayAsync(x), _ => configured);
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.Menu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
