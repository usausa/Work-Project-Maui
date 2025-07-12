namespace Template.MobileApp.Modules.Basic;

public sealed partial class BasicConverterViewModel : AppViewModelBase
{
    [ObservableProperty]
    public partial bool ReverseChecked { get; set; }

    [ObservableProperty]
    public partial bool MultiChecked1 { get; set; }
    [ObservableProperty]
    public partial bool MultiChecked2 { get; set; }

    [ObservableProperty]
    public partial bool ToChecked { get; set; }

    [ObservableProperty]
    public partial string Text { get; set; }

    public IObserveCommand SwitchTextCommand { get; }

    public BasicConverterViewModel()
    {
        Text = string.Empty;

        SwitchTextCommand = MakeDelegateCommand(() => Text = String.IsNullOrEmpty(Text) ? "Hello World" : string.Empty);
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.BasicMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
