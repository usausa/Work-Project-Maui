namespace PushClient;

#pragma warning disable CA1724
public partial class App
{
    private readonly MainPage page;

    public App(MainPage page)
    {
        InitializeComponent();

        this.page = page;
    }

    protected override Window CreateWindow(IActivationState? activationState) => new(page);
}
