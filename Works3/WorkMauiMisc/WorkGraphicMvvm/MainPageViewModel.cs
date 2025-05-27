namespace WorkGraphicMvvm;

using System.Windows.Input;

using Smart.ComponentModel;
using Smart.Maui.ViewModels;

public sealed class MainPageViewModel : ViewModelBase
{
    //public NotificationValue<Color> Color { get; } = new(Colors.Transparent);

    public SampleGraphics Graphics { get; } = new();

    public ICommand ColorCommand { get; }

    public MainPageViewModel()
    {
        ColorCommand = MakeDelegateCommand<Color>(x => Graphics.Color = x);
    }
}
