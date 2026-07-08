namespace Template.MobileApp.Modules.Navigation.Wizard;

public sealed partial class WizardContext : ObservableObject, IInitializable, IDisposable
{
    private readonly ILogger<WizardContext> log;

    public WizardContext(ILogger<WizardContext> log)
    {
        this.log = log;
    }

    [ObservableProperty]
    public partial string? Data1 { get; set; }

    [ObservableProperty]
    public partial string? Data2 { get; set; }

    public void Initialize()
    {
        // TODO Extension
#pragma warning disable CA1848
        log.LogInformation("**** WizardContext Initialize ****");
#pragma warning restore CA1848
    }

    public void Dispose()
    {
#pragma warning disable CA1848
        log.LogInformation("**** WizardContext Dispose ****");
#pragma warning restore CA1848
    }
}
