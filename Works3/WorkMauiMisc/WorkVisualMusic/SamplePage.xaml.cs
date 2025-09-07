using Smart.Maui.ViewModels;
using Smart.Mvvm;

namespace WorkVisualMusic;

public partial class SamplePage : ContentPage
{
	public SamplePage()
	{
		InitializeComponent();
	}
}

public partial class SamplePageViewModel : ExtendViewModelBase
{
	[ObservableProperty]
	public partial double KnobValue1 { get; set; } = 50;

    [ObservableProperty]
    public partial double KnobValue3 { get; set; } = 50;

    [ObservableProperty]
    public partial double KnobValue2 { get; set; } = 50;

    [ObservableProperty]
    public partial double SliderValue1 { get; set; } = 50;

    [ObservableProperty]
    public partial double SliderValue3 { get; set; } = 50;

    [ObservableProperty]
    public partial double SliderValue2 { get; set; } = 50;

    public SamplePageViewModel()
	{
	}
}