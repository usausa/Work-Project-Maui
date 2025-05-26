using System.Collections.ObjectModel;
using Smart.Collections.Generic;

namespace WorkSelect;

using System.Windows.Input;
using Smart.Maui.Input;
using Smart.Mvvm;
using Smart.Mvvm.ViewModels;

public partial class MainPageViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial List<SelectItem> Items { get; set; }

    [ObservableProperty]
    public partial int? Value1 { get; set; }

    [ObservableProperty]
    public partial int? Value2 { get; set; }

    public ICommand Execute1Command { get; }

    public ICommand Execute2Command { get; }

    public MainPageViewModel()
    {
        Items = Enumerable.Range(1, 20).Select(x => new SelectItem(x, $"Data-{x}")).ToList();

        Execute1Command = new DelegateCommand(() =>
        {
            Value1 = Value1.HasValue ? Value1 + 1 : 1;
        });
        Execute2Command = new DelegateCommand(() =>
        {
            Value2 = Value2.HasValue ? Value2 + 1 : 1;
        });
    }
}
