namespace WorkVisualTimeline;

using System.Collections.ObjectModel;

using Smart.Mvvm.ViewModels;

public sealed class MainPageViewModel : ViewModelBase
{
    public ObservableCollection<Item> Items { get; } = new();

    public MainPageViewModel()
    {
        Items.Add(new Item { Name = "Data-1", Color = Color.FromArgb("#EEB611") });
        Items.Add(new Item { Name = "Data-2", Color = Color.FromArgb("#5677CB") });
        Items.Add(new Item { Name = "Data-3", Color = Color.FromArgb("#51C6BF") });
        Items.Add(new Item { Name = "Data-4", Color = Color.FromArgb("#EE376C") });
        Items.Add(new Item { Name = "Data-5", Color = Color.FromArgb("#51C6BF") });
    }
}
