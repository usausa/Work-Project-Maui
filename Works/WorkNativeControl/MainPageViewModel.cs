namespace WorkNativeControl;

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

using Smart.Collections.Generic;
using Smart.Maui.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    public ObservableCollection<DataEntity> List { get; } = new();

    public ICommand DeleteCommand { get; }

    public MainPageViewModel()
    {
        List.AddRange(Enumerable.Range(1, 10).Select(x => new DataEntity { Id = x, Name = $"Data-{x}" }));

        DeleteCommand = MakeDelegateCommand<DataEntity>(x =>
        {
            Debug.WriteLine($"{x.Id} delete.");
        });
    }
}
