namespace WorkNamespace;

using System.Collections.ObjectModel;

using WorkNamespace.Models;

internal class MainPageViewModel
{
    public ObservableCollection<DataEntity> Items { get; } =
    [
        new() { Name = "Item 1" },
        new() { Name = "Item 2" },
        new() { Name = "Item 3" }
    ];
}
