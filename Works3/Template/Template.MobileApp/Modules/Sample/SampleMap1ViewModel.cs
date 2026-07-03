namespace Template.MobileApp.Modules.Sample;

using Microsoft.Maui.Maps;

public sealed partial class SampleMap1ViewModel : AppViewModelBase
{
    // 東京駅
    private const double InitialLatitude = 35.681167;
    private const double InitialLongitude = 139.767052;

    public MapController Controller { get; } = new(InitialLatitude, InitialLongitude, 3);

    [ObservableProperty]
    public partial MapType CurrentMapType { get; set; } = MapType.Street;

    public IReadOnlyList<MapSpot> Spots { get; } =
    [
        new() { Name = "皇居", Description = "千代田区千代田", Location = new Location(35.685175, 139.752800) },
        new() { Name = "東京タワー", Description = "港区芝公園", Location = new Location(35.658581, 139.745433) },
        new() { Name = "東京スカイツリー", Description = "墨田区押上", Location = new Location(35.710063, 139.810700) },
        new() { Name = "浅草寺", Description = "台東区浅草", Location = new Location(35.714765, 139.796655) }
    ];

    public ICommand HomeCommand { get; }

    public ICommand ToggleMapTypeCommand { get; }

    public SampleMap1ViewModel()
    {
        HomeCommand = MakeDelegateCommand(Controller.MoveToHome);
        ToggleMapTypeCommand = MakeDelegateCommand(() => CurrentMapType = CurrentMapType == MapType.Street ? MapType.Hybrid : MapType.Street);
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.SampleMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
