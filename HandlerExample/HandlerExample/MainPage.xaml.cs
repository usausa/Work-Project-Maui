namespace HandlerExample;

public partial class MainPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void Button_OnClicked(object? sender, EventArgs e)
    {
        Entry3.Text = DateTime.Now.ToString("HH:mm:ss");
    }
}
