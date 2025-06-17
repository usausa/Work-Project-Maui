namespace WorkPopup2;

using CommunityToolkit.Maui;

public partial class App : Application
{
    private readonly IServiceProvider provider;

    public App(IServiceProvider provider)
    {
        InitializeComponent();

        this.provider = provider;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new MainPage(provider.GetRequiredService<IPopupService>()));
    }
}