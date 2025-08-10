using Com.Epson.Epos2.Printer;

namespace WorkBind;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void OnTestClicked(object? sender, EventArgs e)
    {
        var printer = new Printer(Printer.TmM30, 1, null);
    }
}
