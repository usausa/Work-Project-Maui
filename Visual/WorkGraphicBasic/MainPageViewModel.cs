namespace WorkGraphicBasic;

using Smart.Maui.ViewModels;

using System.Collections.ObjectModel;

public sealed class MainPageViewModel : ViewModelBase
{
    public ObservableCollection<Item> Items { get; } = new();

    public MainPageViewModel()
    {
        Items.Add(new Item { Id = 1 });
        Items.Add(new Item { Id = 2 });
        Items.Add(new Item { Id = 3 });
    }
}
