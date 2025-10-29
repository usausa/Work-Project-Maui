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

[ObservableGeneratorOption(Reactive = true, ViewModel = true)]
public partial class DiagnosticPageViewModel : ExtendViewModelBase
{
    public IObserveCommand TestCommand { get; }

    [ObservableProperty]
    public partial bool DiagnosticEnabled { get; set; }

    [ObservableProperty]
    public partial bool DiagnosticPanelVisible { get; set; }

    public IObserveCommand DiagnosticCommand { get; }

    public DiagnosticPageViewModel()
    {
        DiagnosticEnabled = true;
        DiagnosticCommand = MakeDelegateCommand(() => DiagnosticPanelVisible = !DiagnosticPanelVisible);
        Disposables.Add(ObserveDiagnosticPanelVisible().Subscribe(x =>
        {
            if (x)
            {
                // TODO
            }
        }));

        TestCommand = MakeDelegateCommand(() => Debug.WriteLine("* Clicked"));
    }
}