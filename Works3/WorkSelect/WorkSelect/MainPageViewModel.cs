namespace WorkSelect;

using System.Windows.Input;
using Smart.Maui.Input;
using Smart.Mvvm;
using Smart.Mvvm.ViewModels;

public partial class MainPageViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial int? Value { get; set; }

    public ICommand ExecuteCommand { get; }

    public MainPageViewModel()
    {
        ExecuteCommand = new DelegateCommand(() =>
        {
            Value = Value.HasValue ? Value + 1 : 1;
        });
    }
}
