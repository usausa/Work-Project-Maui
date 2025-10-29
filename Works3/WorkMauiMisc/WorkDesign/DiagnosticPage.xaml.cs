namespace WorkDesign;

using System.Diagnostics;
using Smart.Maui.Input;
using Smart.Maui.ViewModels;
using Smart.Mvvm;

public partial class DiagnosticPage : ContentPage
{
	public DiagnosticPage()
	{
		InitializeComponent();
	}
}

public partial class DiagnosticPageViewModel : ExtendViewModelBase
{
    public IObserveCommand TestCommand { get; }

    [ObservableProperty]
    public partial bool DiagnosticEnabled { get; set; }
    [ObservableProperty]
    public partial bool DiagnosticWindowVisible { get; set; }

    public IObserveCommand DiagnosticCommand { get; }

    public DiagnosticPageViewModel()
    {
        DiagnosticEnabled = true;
        DiagnosticCommand = MakeDelegateCommand(() => DiagnosticWindowVisible = !DiagnosticWindowVisible);

        TestCommand = MakeDelegateCommand(() => Debug.WriteLine("* Clicked"));
    }
}