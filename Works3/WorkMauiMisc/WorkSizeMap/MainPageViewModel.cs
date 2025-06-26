using System.Diagnostics;
using System.Windows.Input;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

namespace WorkSizeMap;

public partial class MainPageViewModel : ExtendViewModelBase
{
    [ObservableProperty]
    public partial Size DesiredSize { get; set; }

    public ImageSource Image { get; }

    public ICommand TestCommand { get; }

    public MainPageViewModel()
    {
        TestCommand = MakeDelegateCommand(() =>
        {
            Debug.WriteLine(DesiredSize);
        });

        Image = ImageSource.FromFile("dotnet_bot.png");
    }
}
