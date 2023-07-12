namespace Template.MobileApp.Modules.Sample;

using Template.MobileApp;

public class SampleConverterViewModel : AppViewModelBase
{
    public SampleConverterViewModel(
        ApplicationState applicationState)
        : base(applicationState)
    {
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.SampleMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
