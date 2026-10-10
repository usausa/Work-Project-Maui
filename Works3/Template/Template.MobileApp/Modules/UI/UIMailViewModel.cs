namespace Template.MobileApp.Modules.UI;

public enum MailPage
{
    Mail,
    Schedule,
    Application
}

public sealed partial class UIMailViewModel : AppViewModelBase
{
    private const int InitialSize = 144;

    // 頭文字のアイコンの色 (名前から決める)
    private static readonly SKColor[] InitialColors =
    [
        SKColor.Parse("#FB8C00"),
        SKColor.Parse("#00897B"),
        SKColor.Parse("#D81B60"),
        SKColor.Parse("#43A047"),
        SKColor.Parse("#3949AB"),
        SKColor.Parse("#6D4C41"),
        SKColor.Parse("#8E24AA")
    ];

    private static readonly SKTypeface InitialTypeface = SKFontManager.Default.MatchCharacter('あ') ?? SKTypeface.Default;

    private readonly IFileSystem fileSystem;

    [Scope]
    [ObservableProperty]
    public partial UIMailContext Context { get; set; } = default!;

    [ObservableProperty]
    public partial MailPage Selected { get; set; } = MailPage.Mail;

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial bool IsDrawerOpen { get; set; }

    public string OwnerName { get; } = "うさうささん";

    public ImageSource OwnerImage { get; }

    public ICommand ArchiveCommand { get; }

    public ICommand DeleteCommand { get; }

    public IObserveCommand SelectCommand { get; }

    public IObserveCommand OpenCommand { get; }

    public IObserveCommand StarCommand { get; }

    public IObserveCommand DrawerCommand { get; }

    public IObserveCommand FolderCommand { get; }

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public UIMailViewModel(IFileSystem fileSystem)
    {
        this.fileSystem = fileSystem;

        SelectCommand = MakeDelegateCommand<MailPage>(x => Selected = x);
        ArchiveCommand = MakeDelegateCommand<MailMessage>(Archive);
        DeleteCommand = MakeDelegateCommand<MailMessage>(Delete);
        OpenCommand = MakeAsyncCommand<MailMessage>(OpenAsync);
        StarCommand = MakeDelegateCommand<MailMessage>(ToggleStar);
        DrawerCommand = MakeDelegateCommand(() => IsDrawerOpen = true);
        FolderCommand = MakeDelegateCommand<UIMailFolderItem>(ShowFolder);

        OwnerImage = CreateInitialImage(OwnerName);
    }

    //--------------------------------------------------------------------------------
    // Navigation
    //--------------------------------------------------------------------------------

    // 本文の画面から戻ったときは読み込まない
    // ReSharper disable once ArrangeModifiersOrder
    public override async Task OnNavigatedToAsync(INavigationContext context)
    {
        if (!Context.IsLoaded)
        {
            await Navigator.PostActionAsync(LoadMessagesAsync);
        }
    }

    protected override Task OnNotifyBackAsync()
    {
        if (IsDrawerOpen)
        {
            IsDrawerOpen = false;
            return Task.CompletedTask;
        }

        return Navigator.ForwardAsync(ViewId.UIMenu1);
    }

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();

    //--------------------------------------------------------------------------------
    // Operation
    //--------------------------------------------------------------------------------

    private Task OpenAsync(MailMessage message)
    {
        Context.Open(message);
        return Navigator.PushAsync(ViewId.UIMailDetail);
    }

    private void Archive(MailMessage message) => Context.Archive(message);

    private void Delete(MailMessage message) => Context.Delete(message);

    private void ToggleStar(MailMessage message) => Context.ToggleStar(message);

    private void ShowFolder(UIMailFolderItem folder)
    {
        IsDrawerOpen = false;
        Context.Show(folder.View);
    }

    private async Task LoadMessagesAsync()
    {
        IsLoading = true;

        // Simulate delay
        await Task.Delay(500);

        await Context.LoadAsync(LoadImage, CreateInitialImage);

        IsLoading = false;
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    private async ValueTask<ImageSource> LoadImage(string fileName)
    {
        await using var stream = await fileSystem.OpenAppPackageFileAsync(Path.Combine("Avatar", fileName));
        var bitmap = SKBitmap.Decode(stream);
        // SKBitmapはアンマネージドメモリを持つため画面破棄時に解放する
        Disposables.Add(bitmap);
        return new SKBitmapImageSource { Bitmap = bitmap };
    }

    // 色の地に名前の頭文字 (丸く切り抜くのは XAML)
    private ImageSource CreateInitialImage(string name)
    {
        var bitmap = new SKBitmap(InitialSize, InitialSize);
        using (var canvas = new SKCanvas(bitmap))
        {
            canvas.Clear(InitialColors[name.Sum(static x => x) % InitialColors.Length]);
            using var font = new SKFont(InitialTypeface, InitialSize * 0.42f);
            font.Embolden = true;
            using var paint = new SKPaint();
            paint.IsAntialias = true;
            paint.Color = SKColors.White;
            var metrics = font.Metrics;
            canvas.DrawText(name[..1], InitialSize / 2f, (InitialSize - metrics.Ascent - metrics.Descent) / 2f, SKTextAlign.Center, font, paint);
        }
        Disposables.Add(bitmap);
        return new SKBitmapImageSource { Bitmap = bitmap };
    }
}
