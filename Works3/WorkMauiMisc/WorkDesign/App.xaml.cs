namespace WorkDesign;

public partial class App : Application
{
    public App()
    {
        Current!.UserAppTheme = AppTheme.Light;

        InitializeComponent();
    }

    // https://editor.method.ac/
    protected override Window CreateWindow(IActivationState? activationState)
    {
        //return new Window(new HeaderPage());
        //return new Window(new ButtonPage());
        //return new Window(new PathPage());
        return new Window(new FontPage());
        // -----
        //return new Window(new ChatPage());
        //return new Window(new MailPage());
        //return new Window(new PosPage());
        // -----
        //return new Window(new SchedulePage());
        //return new Window(new CalendarPage());

        // -----
        //return new Window(new MainPage());
        //return new Window(new ActivityPage());
        //return new Window(new LabelPage());
        //return new Window(new BadgePage());
        //return new Window(new StylePage());
        //return new Window(new MoneyPage2());
    }
}