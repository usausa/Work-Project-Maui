namespace OnyxSample;

using OnyxSample.Shell;

public partial class MainPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override bool OnBackButtonPressed()
    {
        if ((BindingContext is MainPageViewModel context) && !context.ApplicationState.IsBusy)
        {
            context.Navigator.NotifyAsync(ShellEvent.Back);
        }

        return true;
    }
}
