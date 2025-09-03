namespace WorkNavigation;

using WorkNavigation.Shell;

public sealed partial class MainPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override bool OnBackButtonPressed()
    {
        if (BindingContext is MainPageViewModel { BusyState.IsBusy: false } context)
        {
            context.Navigator.NotifyAsync(ShellEvent.Back);
        }

        return true;
    }
}