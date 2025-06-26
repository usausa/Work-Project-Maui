using System.Diagnostics;
using System.Windows.Input;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

namespace WorkSizeMap;

public partial class MainPageViewModel : ExtendViewModelBase
{
    [ObservableProperty]
    public partial Size DesiredSize { get; set; }

    [ObservableProperty]
    public partial ImageSource? Image { get; set; }

    public ICommand TestCommand { get; }

    public ICommand LoadCommand { get; }

    public MainPageViewModel()
    {
        TestCommand = MakeDelegateCommand(() =>
        {
            Debug.WriteLine(DesiredSize);
        });
        LoadCommand = MakeDelegateCommand(() =>
        {
            Image = ImageSource.FromFile("dummy.png");
        });
    }
}
