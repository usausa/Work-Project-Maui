namespace Template.MobileApp.Modules.View;

using Svg.Skia;

public sealed partial class ViewSvgViewModel : AppViewModelBase
{
    private readonly IFileSystem fileSystem;

    [ObservableProperty]
    public partial SKSvg? Svg { get; set; }

    [ObservableProperty]
    public partial string Selected { get; set; } = "dotnet_bot";

    public IObserveCommand SelectCommand { get; }

    public ViewSvgViewModel(IFileSystem fileSystem)
    {
        this.fileSystem = fileSystem;

        SelectCommand = MakeAsyncCommand<string>(LoadAsync);
    }

    public override Task OnNavigatingToAsync(INavigationContext context) => LoadAsync("dotnet_bot");

    private async Task LoadAsync(string name)
    {
        var path = name switch
        {
            "vite" => Path.Combine("web-app", "vite.svg"),
            "react" => Path.Combine("web-app", "assets", "react-CHdo91hT.svg"),
            _ => Path.Combine("Svg", "dotnet_bot.svg")
        };

        var svg = new SKSvg();
        await using var stream = await fileSystem.OpenAppPackageFileAsync(path);
        svg.Load(stream);
        Svg = svg;
        Selected = name;
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.ViewMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
