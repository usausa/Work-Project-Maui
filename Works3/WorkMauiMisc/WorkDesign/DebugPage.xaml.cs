namespace WorkDesign;

using System.Diagnostics;
using Smart.Maui.Input;
using Smart.Maui.ViewModels;
using Smart.Mvvm;

public partial class DebugPage : ContentPage
{
	public DebugPage()
	{
		InitializeComponent();
	}
}

public partial class DebugPageViewModel : ExtendViewModelBase
{
    public IObserveCommand TestCommand { get; }

    [ObservableProperty]
    public partial bool DebugEnabled { get; set; }
    [ObservableProperty]
    public partial bool DebugWindowVisible { get; set; }

    public IObserveCommand DebugCommand { get; }

    public DebugPageViewModel()
    {
        DebugEnabled = true;
        DebugCommand = MakeDelegateCommand(() => DebugWindowVisible = !DebugWindowVisible);

        TestCommand = MakeDelegateCommand(() => Debug.WriteLine("* Clicked"));
    }
}