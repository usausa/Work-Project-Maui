namespace Template.MobileApp.Modules.Basic;

public sealed class BasicLocaleViewModel : AppViewModelBase
{
    public string CultureName { get; } = CultureInfo.CurrentCulture.Name;

    public string UICultureName { get; } = CultureInfo.CurrentUICulture.Name;

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.BasicMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
