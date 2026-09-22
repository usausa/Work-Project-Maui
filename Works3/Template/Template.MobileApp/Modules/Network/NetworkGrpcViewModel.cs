namespace Template.MobileApp.Modules.Network;

using Grpc.Core;

using Template.MobileApp.Services;

// gRPC チャット (双方向ストリーミング、認証なし。ユーザー名は端末名) と単項 RPC (サーバー時刻)
public sealed partial class NetworkGrpcViewModel : AppViewModelBase
{
    private readonly ChatRoomClient chatClient;

    private readonly Settings settings;

    private readonly IDeviceInfo deviceInfo;

    private readonly IDispatcher dispatcher;

    private Uri? address;

    private bool active;

    [ObservableProperty]
    public partial bool Configured { get; set; }

    [ObservableProperty]
    public partial string AddressDisplay { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string StateText { get; set; } = "切断";

    [ObservableProperty]
    public partial string LastError { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ServerTime { get; set; } = "-";

    [ObservableProperty]
    public partial string Input { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int PendingCount { get; set; }

    [ObservableProperty]
    public partial bool IsEmpty { get; set; } = true;

    public ObservableCollection<ChatMessageEntry> Messages { get; } = [];

    public IObserveCommand SendCommand { get; }

    public IObserveCommand ServerTimeCommand { get; }

    public NetworkGrpcViewModel(
        ChatRoomClient chatClient,
        Settings settings,
        IDeviceInfo deviceInfo,
        IDispatcher dispatcher)
    {
        this.chatClient = chatClient;
        this.settings = settings;
        this.deviceInfo = deviceInfo;
        this.dispatcher = dispatcher;

        SendCommand = MakeAsyncCommand(SendAsync, () => Configured && !String.IsNullOrWhiteSpace(Input));
        ServerTimeCommand = MakeAsyncCommand(GetServerTimeAsync, () => Configured);
    }

    public override Task OnNavigatingToAsync(INavigationContext context)
    {
        Configured = settings.IsGrpcConfigured();
        if (Configured)
        {
            address = new Uri(settings.GrpcEndPoint);
            AddressDisplay = address.ToString();
        }
        else
        {
            AddressDisplay = "未設定 (設定画面の QR で投入)";
        }

        return Task.CompletedTask;
    }

    public override Task OnNavigatedToAsync(INavigationContext context)
    {
        if (address is null)
        {
            return Task.CompletedTask;
        }

        active = true;
        chatClient.StateChanged += OnStateChanged;
        chatClient.MessageReceived += OnMessageReceived;
        return chatClient.ConnectAsync(address, deviceInfo.Name);
    }

    public override async Task OnNavigatingFromAsync(INavigationContext context)
    {
        if (!active)
        {
            return;
        }

        active = false;
        chatClient.StateChanged -= OnStateChanged;
        chatClient.MessageReceived -= OnMessageReceived;
        await chatClient.DisconnectAsync();
        Messages.Clear();
        IsEmpty = true;
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.NetworkMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();

    protected override Task OnNotifyFunction2() => Configured ? chatClient.ConnectAsync(address!, deviceInfo.Name) : Task.CompletedTask;

    //--------------------------------------------------------------------------------
    // Chat
    //--------------------------------------------------------------------------------

    private async Task SendAsync()
    {
        var text = Input.Trim();
        if (text.Length == 0)
        {
            return;
        }

        await chatClient.SendAsync(text);
        Input = string.Empty;
        PendingCount = chatClient.PendingCount;
    }

    private void OnStateChanged(object? sender, ChatStateEventArgs e)
    {
        var error = chatClient.LastError;
        dispatcher.Dispatch(() =>
        {
            StateText = e.State switch
            {
                ChatConnectionState.Connecting => "接続中...",
                ChatConnectionState.Connected => "接続済み",
                ChatConnectionState.Reconnecting => "再接続中...",
                _ => "切断"
            };
            LastError = error ?? string.Empty;
            PendingCount = chatClient.PendingCount;
        });
    }

    private void OnMessageReceived(object? sender, ChatMessageEventArgs e)
    {
        dispatcher.Dispatch(() =>
        {
            Messages.Add(e.Entry);
            IsEmpty = false;
            PendingCount = chatClient.PendingCount;
        });
    }

    //--------------------------------------------------------------------------------
    // Unary
    //--------------------------------------------------------------------------------

    private async Task GetServerTimeAsync()
    {
        try
        {
            var time = await ChatRoomClient.GetServerTimeAsync(address!);
            ServerTime = time.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
        catch (RpcException ex)
        {
            ServerTime = $"失敗 ({ex.StatusCode})";
        }
    }
}
