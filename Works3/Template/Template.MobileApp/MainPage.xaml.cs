namespace Template.MobileApp;

using Template.MobileApp.Shell;

public sealed partial class MainPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override bool OnBackButtonPressed()
    {
        if (BindingContext is MainPageViewModel context)
        {
            context.Navigator.NotifyAsync(ShellEvent.Back);
        }

        return true;
    }
}
