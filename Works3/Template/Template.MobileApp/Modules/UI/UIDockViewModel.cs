namespace Template.MobileApp.Modules.UI;

using System.Diagnostics;

using Template.MobileApp.Components;

public sealed class UIDockViewModel : AppViewModelBase
{
    private const int TimerSeconds = 30;

    private const double MegaByte = 1024 * 1024;

    private const string FolderColor1 = "#ffb347";

    private const string FolderColor2 = "#ffcc33";

    private static readonly (string Color1, string Color2)[] Colors =
    [
        new("#F44336", "#FF8A80"),
        new("#E91E63", "#FF80AB"),
        new("#9C27B0", "#EA80FC"),
        new("#673AB7", "#B388FF"),
        new("#3F51B5", "#8C9EFF"),
        new("#2196F3", "#82B1FF"),
        new("#03A9F4", "#80D8FF"),
        new("#00BCD4", "#84FFFF"),
        new("#009688", "#A7FFEB"),
        new("#4CAF50", "#B9F6CA"),
        new("#8BC34A", "#CCFF90"),
        new("#CDDC39", "#F4FF81"),
        new("#FFEB3B", "#FFFF8D"),
        new("#FFC107", "#FFE57F"),
        new("#FF9800", "#FFD180"),
        new("#FF5722", "#FF9E80")
    ];

    // フォルダーの中のボタン(画像・名前)
    private static readonly (string Name, (string Image, string Label)[] Items)[] Folders =
    [
        ("Media",
        [
            ("fast_rewind.png", "Rewind"),
            ("play_arrow.png", "Play"),
            ("pause.png", "Pause"),
            ("fast_forward.png", "Forward"),
            ("stop.png", "Stop")
        ]),
        ("System",
        [
            ("home.png", "Home"),
            ("resume.png", "Resume"),
            ("power_settings_circle.png", "Power")
        ])
    ];

    private readonly IDialog dialog;

    private readonly IScreen screen;

    private readonly IFileSystem fileSystem;

    private readonly DeviceInformation deviceInformation;

    private readonly IDispatcherTimer timer;

    private readonly int processorCount = Environment.ProcessorCount;

    private readonly List<DeckButtonInfo> rootButtons = [];

    private readonly Dictionary<string, List<DeckButtonInfo>> folderButtons = [];

    private bool folderOpened;

    private ProcessStatistics previous;

    private DeckButtonInfo? cpuButton;

    private DeckButtonInfo? memButton;

    private DeckButtonInfo? timerButton;

    private DeckButtonInfo? muteButton;

    private byte[] muteImage = [];

    private byte[] mutedImage = [];

    private bool muted;

    private int remainSeconds;

    public ObservableCollection<DeckButtonInfo> Buttons { get; } = [];

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public UIDockViewModel(
        IDialog dialog,
        IScreen screen,
        IFileSystem fileSystem,
        IDispatcher dispatcher,
        DeviceInformation deviceInformation)
    {
        this.dialog = dialog;
        this.screen = screen;
        this.fileSystem = fileSystem;
        this.deviceInformation = deviceInformation;

        timer = dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        Disposables.Add(timer.TickAsObservable().Subscribe(_ => OnTimerTick()));
    }

    //--------------------------------------------------------------------------------
    // Navigation
    //--------------------------------------------------------------------------------

    public override async Task OnNavigatedToAsync(INavigationContext context)
    {
        if (!context.Attribute.IsRestore())
        {
            await Navigator.PostActionAsync(InitializeAsync);
        }

        screen.SetFullscreen(true);
        previous = deviceInformation.ReadProcessStatistics();
        timer.Start();
    }

    public override Task OnNavigatingFromAsync(INavigationContext context)
    {
        timer.Stop();
        screen.SetFullscreen(false);
        return Task.CompletedTask;
    }

    protected override Task OnNotifyBackAsync()
    {
        if (folderOpened)
        {
            CloseFolder();
            return Task.CompletedTask;
        }

        return Navigator.ForwardAsync(ViewId.UIMenu1);
    }

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();

    //--------------------------------------------------------------------------------
    // Operation
    //--------------------------------------------------------------------------------

    private Task InitializeAsync()
    {
        return BusyState.UsingAsync(async () =>
        {
            muteImage = await LoadImageAsync("volume_mute.png");
            mutedImage = await LoadImageAsync("volume_off.png");

            // Row0 - Row3
            for (var i = 0; i < Colors.Length; i++)
            {
                rootButtons.Add(new DeckButtonInfo
                {
                    Row = i / 4,
                    Column = i % 4,
                    ButtonType = DeckButtonType.Text,
                    Label = $"Color{i + 1}",
                    Text = "Color",
                    BackColor1 = Color.FromArgb(Colors[i].Color1),
                    BackColor2 = Color.FromArgb(Colors[i].Color2),
                    Command = MakeAsyncCommand<string>(ExecuteAsync),
                    Parameter = $"Color{i + 1}"
                });
            }

            // Row4
            for (var i = 0; i < Folders.Length; i++)
            {
                var (name, items) = Folders[i];
                rootButtons.Add(new DeckButtonInfo
                {
                    Row = 4,
                    Column = i,
                    ButtonType = DeckButtonType.Image,
                    Label = name,
                    BackColor1 = Color.FromArgb(FolderColor1),
                    BackColor2 = Color.FromArgb(FolderColor2),
                    ImageBytes = await LoadImageAsync("folder.png"),
                    Command = MakeDelegateCommand<string>(OpenFolder),
                    Parameter = name
                });
                folderButtons[name] = await CreateFolderButtonsAsync(items);
            }
            rootButtons.Add(new DeckButtonInfo
            {
                Row = 4,
                Column = 2,
                ButtonType = DeckButtonType.Image,
                Label = "Sound1",
                BackColor1 = Color.FromArgb("#fc00ff"),
                BackColor2 = Color.FromArgb("#00dbde"),
                ImageBytes = await LoadImageAsync("music_note.png"),
                Command = MakeAsyncCommand<string>(ExecuteAsync),
                Parameter = "Sound1"
            });
            rootButtons.Add(new DeckButtonInfo
            {
                Row = 4,
                Column = 3,
                ButtonType = DeckButtonType.Image,
                Label = "Sound2",
                BackColor1 = Color.FromArgb("#fc00ff"),
                BackColor2 = Color.FromArgb("#00dbde"),
                ImageBytes = await LoadImageAsync("music_note.png"),
                Command = MakeAsyncCommand<string>(ExecuteAsync),
                Parameter = "Sound2"
            });

            // Row5
            rootButtons.Add(new DeckButtonInfo
            {
                Row = 5,
                Column = 0,
                ButtonType = DeckButtonType.Image,
                Label = "Volume up",
                BackColor1 = Color.FromArgb("#f46b45"),
                BackColor2 = Color.FromArgb("#eea849"),
                ImageBytes = await LoadImageAsync("volume_up.png"),
                Command = MakeAsyncCommand<string>(ExecuteAsync),
                Parameter = "VolumeUp"
            });
            muteButton = new DeckButtonInfo
            {
                Row = 5,
                Column = 1,
                ButtonType = DeckButtonType.Image,
                Label = "Mute",
                BackColor1 = Color.FromArgb("#f46b45"),
                BackColor2 = Color.FromArgb("#eea849"),
                ImageBytes = muteImage,
                Command = MakeDelegateCommand(ToggleMute),
                Parameter = "Mute"
            };
            rootButtons.Add(muteButton);
            rootButtons.Add(new DeckButtonInfo
            {
                Row = 5,
                Column = 2,
                ButtonType = DeckButtonType.Image,
                Label = "Volume down",
                BackColor1 = Color.FromArgb("#f46b45"),
                BackColor2 = Color.FromArgb("#eea849"),
                ImageBytes = await LoadImageAsync("volume_down.png"),
                Command = MakeAsyncCommand<string>(ExecuteAsync),
                Parameter = "VolumeDown"
            });

            // Row6
            timerButton = new DeckButtonInfo
            {
                Row = 6,
                Column = 1,
                ButtonType = DeckButtonType.Image,
                Label = FormatTimer(TimerSeconds),
                BackColor1 = Color.FromArgb("#1fa2ff"),
                BackColor2 = Color.FromArgb("#12d8fa"),
                ImageBytes = await LoadImageAsync("timer.png"),
                Command = MakeDelegateCommand(ToggleTimer),
                Parameter = "Timer"
            };
            rootButtons.Add(timerButton);
            rootButtons.Add(new DeckButtonInfo
            {
                Row = 6,
                Column = 2,
                ButtonType = DeckButtonType.Image,
                Label = "Lock",
                BackColor1 = Color.FromArgb("#a770ef"),
                BackColor2 = Color.FromArgb("#cf8bf3"),
                ImageBytes = await LoadImageAsync("lock.png"),
                Command = MakeAsyncCommand<string>(ExecuteAsync),
                Parameter = "Lock"
            });
            rootButtons.Add(new DeckButtonInfo
            {
                Row = 6,
                Column = 3,
                ButtonType = DeckButtonType.Image,
                Label = "Settings",
                BackColor1 = Color.FromArgb("#0cebeb"),
                BackColor2 = Color.FromArgb("#20e3b2"),
                ImageBytes = await LoadImageAsync("settings.png"),
                Command = MakeAsyncCommand<string>(ExecuteAsync),
                Parameter = "Settings"
            });

            // Row7
            rootButtons.Add(new DeckButtonInfo
            {
                Row = 7,
                Column = 0,
                ButtonType = DeckButtonType.Image,
                Label = "Exit",
                BackColor1 = Color.FromArgb("#00d2ff"),
                BackColor2 = Color.FromArgb("#00d2ff"),
                ImageBytes = await LoadImageAsync("exit_to_app.png"),
                Command = MakeAsyncCommand<string>(ExecuteAsync),
                Parameter = "Exit"
            });
            cpuButton = new DeckButtonInfo
            {
                Row = 7,
                Column = 2,
                ButtonType = DeckButtonType.Text,
                Label = "CPU",
                Text = String.Join(Environment.NewLine, "CPU", "-"),
                BackColor1 = Color.FromArgb("#616161"),
                BackColor2 = Color.FromArgb("#424242"),
                Command = MakeAsyncCommand<string>(ExecuteAsync),
                Parameter = "Cpu"
            };
            rootButtons.Add(cpuButton);
            memButton = new DeckButtonInfo
            {
                Row = 7,
                Column = 3,
                ButtonType = DeckButtonType.Text,
                Label = "Memory",
                Text = String.Join(Environment.NewLine, "MEM", "-"),
                BackColor1 = Color.FromArgb("#616161"),
                BackColor2 = Color.FromArgb("#424242"),
                Command = MakeAsyncCommand<string>(ExecuteAsync),
                Parameter = "Memory"
            };
            rootButtons.Add(memButton);

            ShowButtons(rootButtons);
        });
    }

    private async ValueTask<List<DeckButtonInfo>> CreateFolderButtonsAsync((string Image, string Label)[] items)
    {
        // 左上に戻るのボタン、2 段目から中のボタン
        var buttons = new List<DeckButtonInfo>
        {
            new()
            {
                Row = 0,
                Column = 0,
                ButtonType = DeckButtonType.Image,
                Label = "Back",
                BackColor1 = Color.FromArgb(FolderColor1),
                BackColor2 = Color.FromArgb(FolderColor2),
                ImageBytes = await LoadImageAsync("arrow_circle_left.png"),
                Command = MakeDelegateCommand(CloseFolder),
                Parameter = "Back"
            }
        };
        for (var i = 0; i < items.Length; i++)
        {
            buttons.Add(new DeckButtonInfo
            {
                Row = 1 + (i / 4),
                Column = i % 4,
                ButtonType = DeckButtonType.Image,
                Label = items[i].Label,
                BackColor1 = Color.FromArgb("#1fa2ff"),
                BackColor2 = Color.FromArgb("#12d8fa"),
                ImageBytes = await LoadImageAsync(items[i].Image),
                Command = MakeAsyncCommand<string>(ExecuteAsync),
                Parameter = items[i].Label
            });
        }

        return buttons;
    }

    private async ValueTask<byte[]> LoadImageAsync(string image)
    {
        if (!String.IsNullOrEmpty(image))
        {
            await using var stream = await fileSystem.OpenAppPackageFileAsync(Path.Combine("DeckButtons", image));
            return await stream.ReadAllBytesAsync();
        }

        return [];
    }

    private async Task ExecuteAsync(string parameter)
    {
        if (parameter == "Exit")
        {
            await Navigator.ForwardAsync(ViewId.UIMenu1);
        }
        else
        {
            await dialog.InformationAsync(parameter);
        }
    }

    private void OpenFolder(string name)
    {
        if (folderButtons.TryGetValue(name, out var buttons))
        {
            folderOpened = true;
            ShowButtons(buttons);
        }
    }

    private void CloseFolder()
    {
        folderOpened = false;
        ShowButtons(rootButtons);
    }

    private void ToggleMute()
    {
        muted = !muted;
        if (muteButton is not null)
        {
            muteButton.ImageBytes = muted ? mutedImage : muteImage;
            muteButton.Label = muted ? "Muted" : "Mute";
        }
    }

    private async Task NotifyTimeUpAsync() => await dialog.InformationAsync("Time's up");

    private void ToggleTimer()
    {
        remainSeconds = remainSeconds > 0 ? 0 : TimerSeconds;
        timerButton?.Label = FormatTimer(remainSeconds > 0 ? remainSeconds : TimerSeconds);
    }

    //--------------------------------------------------------------------------------
    // Event
    //--------------------------------------------------------------------------------

    private void OnTimerTick()
    {
        UpdateUsage();

        if ((remainSeconds > 0) && (timerButton is not null))
        {
            remainSeconds--;
            timerButton.Label = FormatTimer(remainSeconds > 0 ? remainSeconds : TimerSeconds);
            if (remainSeconds == 0)
            {
                _ = NotifyTimeUpAsync();
            }
        }
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    private void ShowButtons(List<DeckButtonInfo> buttons)
    {
        Buttons.Clear();
        foreach (var button in buttons)
        {
            Buttons.Add(button);
        }
    }

    // 診断のパネル(DiagnosticSampler)と同じ取り方。CPU は前回からの CPU 時間の増分、メモリは WorkingSet
    private void UpdateUsage()
    {
        var statistics = deviceInformation.ReadProcessStatistics();
        var elapsed = Stopwatch.GetElapsedTime(previous.Timestamp, statistics.Timestamp).TotalSeconds;
        if ((elapsed > 0) && (cpuButton is not null) && (memButton is not null))
        {
            var cpuUsage = (statistics.CpuTime - previous.CpuTime).TotalSeconds / elapsed * 100 / processorCount;
            previous = statistics;
            cpuButton.Text = String.Join(Environment.NewLine, "CPU", $"{cpuUsage:F1}%");
            memButton.Text = String.Join(Environment.NewLine, "MEM", $"{statistics.WorkingSet / MegaByte:F0} MB");
        }
    }

    private static string FormatTimer(int seconds) => TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss", CultureInfo.InvariantCulture);
}
