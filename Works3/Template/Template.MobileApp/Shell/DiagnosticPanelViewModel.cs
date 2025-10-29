namespace Template.MobileApp.Shell;

[ObservableGeneratorOption(Reactive = true, ViewModel = true)]
public sealed partial class DiagnosticPanelViewModel : ExtendViewModelBase
{
    [ObservableProperty]
    public partial float CpuUsage { get; set; }
    // TODO
}

