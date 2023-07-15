namespace WorkLocalize;

using System.Globalization;

using WorkLocalize.Resources.Strings;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void OnButtonJaOnClicked(object sender, EventArgs e)
    {
        StringResource.Culture = new CultureInfo("ja-jp");
        App.Current.MainPage = new MainPage();
    }

    private void OnButtonUsOnClicked(object sender, EventArgs e)
    {
        StringResource.Culture = new CultureInfo("en-us");
        App.Current.MainPage = new MainPage();
    }
}

