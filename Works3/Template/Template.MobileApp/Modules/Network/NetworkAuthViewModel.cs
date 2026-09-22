namespace Template.MobileApp.Modules.Network;

using Template.MobileApp.Services;
using Template.MobileApp.Usecase;

public sealed partial class NetworkAuthViewModel : AppViewModelBase
{
    private const string DummyLoginId = "user";

    private readonly NetworkUsecase networkUsecase;

    private readonly ApiContext apiContext;

    [ObservableProperty]
    public partial string LoginId { get; set; } = DummyLoginId;

    [ObservableProperty]
    public partial bool IsAuthenticated { get; set; }

    [ObservableProperty]
    public partial string LoginState { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string TokenExpires { get; set; } = string.Empty;

    public IObserveCommand LoginCommand { get; }
    public IObserveCommand LogoutCommand { get; }

    public IObserveCommand SecureCommand { get; }
    public IObserveCommand InvalidateCommand { get; }

    public NetworkAuthViewModel(
        NetworkUsecase networkUsecase,
        ApiContext apiContext)
    {
        this.networkUsecase = networkUsecase;
        this.apiContext = apiContext;

        LoginCommand = MakeAsyncCommand(LoginAsync, () => LoginId.Trim().Length > 0);
        LogoutCommand = MakeDelegateCommand(Logout, () => IsAuthenticated);
        SecureCommand = MakeAsyncCommand(SecureAsync);
        InvalidateCommand = MakeDelegateCommand(Invalidate, () => IsAuthenticated);
    }

    public override Task OnNavigatedToAsync(INavigationContext context)
    {
        UpdateState();
        return Task.CompletedTask;
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.NetworkMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();

    //--------------------------------------------------------------------------------
    // Login
    //--------------------------------------------------------------------------------

    private async Task LoginAsync()
    {
        await networkUsecase.PostAccountLoginAsync(LoginId.Trim());
        UpdateState();
    }

    private void Logout()
    {
        networkUsecase.AccountLogout();
        UpdateState();
    }

    //--------------------------------------------------------------------------------
    // Secure
    //--------------------------------------------------------------------------------

    private async Task SecureAsync()
    {
        await networkUsecase.GetSecretMessageAsync();
        UpdateState();
    }

    private void Invalidate()
    {
        networkUsecase.InvalidateToken();
        UpdateState();
    }

    //--------------------------------------------------------------------------------
    // State
    //--------------------------------------------------------------------------------

    private void UpdateState()
    {
        IsAuthenticated = apiContext.IsAuthenticated;
        LoginState = apiContext.IsAuthenticated ? $"ログイン済み ({apiContext.LoginId})" : "未ログイン";
        TokenExpires = apiContext.TokenExpires is { } expires ? expires.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture) : "-";
    }
}
