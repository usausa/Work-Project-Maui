namespace Template.MobileApp.Modules.UI;

using Template.MobileApp.Components.Noise;

public sealed partial class UIGaugeViewModel : AppViewModelBase
{
    private readonly INoiseMonitor noiseMonitor;

    [ObservableProperty]
    public partial double Decibel { get; set; }

    public UIGaugeViewModel(INoiseMonitor noiseMonitor)
    {
        this.noiseMonitor = noiseMonitor;

        Disposables.Add(noiseMonitor.ObserveMeasuredOnCurrentContext().Subscribe(x => Decibel = x.Decibel));
    }

    public override void OnNavigatedTo(INavigationContext context)
    {
        noiseMonitor.Start();
    }

    public override void OnNavigatingFrom(INavigationContext context)
    {
        noiseMonitor.Stop();
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.UIMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
