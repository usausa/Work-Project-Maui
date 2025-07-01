namespace WorkDesign;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        //return new Window(new MainPage());
        //return new Window(new Home2Page());
        //return new Window(new Home3Page());
        //return new Window(new Home5Page());
        //return new Window(new Home4Page());

        return new Window(new ButtonPage());
    }
}