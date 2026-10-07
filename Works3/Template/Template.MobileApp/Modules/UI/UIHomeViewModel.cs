namespace Template.MobileApp.Modules.UI;

public sealed class UIHomeViewModel : AppViewModelBase
{
    public SmartHome Home { get; } = new();

    public IObserveCommand BackCommand { get; }
    public IObserveCommand RoomCommand { get; }
    public IObserveCommand ToggleCommand { get; }
    public IObserveCommand DownCommand { get; }
    public IObserveCommand UpCommand { get; }

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public UIHomeViewModel()
    {
        BackCommand = MakeAsyncCommand(OnNotifyBackAsync);
        RoomCommand = MakeDelegateCommand<HomeRoom>(Home.Select);
        ToggleCommand = MakeDelegateCommand<HomeDevice>(static x => x.Toggle());
        DownCommand = MakeDelegateCommand(() => Home.ChangeTemperature(-SmartHome.TemperatureStep));
        UpCommand = MakeDelegateCommand(() => Home.ChangeTemperature(SmartHome.TemperatureStep));
    }

    //--------------------------------------------------------------------------------
    // Navigation
    //--------------------------------------------------------------------------------

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.UIMenu2);
}
