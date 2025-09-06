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
	public partial double KnobValue1 { get; set; }

    [ObservableProperty]
    public partial double KnobValue3 { get; set; }

    [ObservableProperty]
    public partial double KnobValue2 { get; set; }

    public SamplePageViewModel()
	{
	}
}