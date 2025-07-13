namespace WorkDesign;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    // https://editor.method.ac/
    protected override Window CreateWindow(IActivationState? activationState)
    {
        //return new Window(new MainPage());
        //return new Window(new ButtonPage());
        return new Window(new BadgePage());
        //return new Window(new PathPage());
    }
}