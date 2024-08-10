namespace CameraSample;

using Smart.Maui.Resolver;

public partial class App
{
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        ResolveProvider.Default.Provider = serviceProvider;

        MainPage = serviceProvider.GetRequiredService<MainPage>();
    }
}
