namespace WorkDesign;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

public partial class StickPage : ContentPage
{
	public StickPage()
	{
		InitializeComponent();
	}
}

public sealed partial class StickPageViewModel : ExtendViewModelBase
{
	[ObservableProperty]
    public partial double StickX { get; set; }

    [ObservableProperty]
    public partial double StickY { get; set; }

    [ObservableProperty]
    public partial bool ButtonA { get; set; }

    [ObservableProperty]
    public partial bool ButtonB { get; set; }

    [ObservableProperty]
    public partial bool ButtonX { get; set; }

    [ObservableProperty]
    public partial bool ButtonY { get; set; }
}
