namespace HttpExample;

public partial class MainPage
{
    public MainPage(MainPageViewModel vm)
    {
        BindingContext = vm;
        InitializeComponent();
    }
}