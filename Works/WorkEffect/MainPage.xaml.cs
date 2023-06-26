namespace WorkEffect;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void CounterBtn_OnClicked(object sender, EventArgs e)
    {
        Entry1.IsEnabled = !Entry1.IsEnabled;
        Entry2.IsEnabled = !Entry2.IsEnabled;
    }
}

