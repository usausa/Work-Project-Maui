namespace WorkGraphicBasic;

using Smart.Maui.ViewModels;

using System.Collections.ObjectModel;

public sealed class MainPageViewModel : ViewModelBase
{
    public ObservableCollection<CellInfo> Items { get; } = new();

    public MainPageViewModel()
    {
        Items.Add(new CellInfo { No = 0, LineNos = [0], Id = "0000000000000000", Text = "🐛うさうさだよもん" });
        Items.Add(new CellInfo { No = 0, LineNos = [0], In = 1, Id = "0000000000000000", Text = "🐛うさうさだよもん" });
        Items.Add(new CellInfo { No = 1, LineNos = [0, 1], Id = "0000000000000000", Text = "🐛うさうさだよもん" });
        Items.Add(new CellInfo { No = 1, LineNos = [0, 1], Id = "0000000000000000", Text = "🐛うさうさだよもん" });
        Items.Add(new CellInfo { No = 0, LineNos = [0, 1], In = 1, Id = "0000000000000000", Text = "🐛うさうさだよもん" });
        Items.Add(new CellInfo { No = 1, LineNos = [0, 1], Id = "0000000000000000", Text = "🐛うさうさだよもん" });
        Items.Add(new CellInfo { No = 1, LineNos = [0, 1], In = 2, Id = "0000000000000000", Text = "🐛うさうさだよもん" });
        Items.Add(new CellInfo { No = 1, LineNos = [0, 1, 2], Id = "0000000000000000", Text = "🐛うさうさだよもん" });
        Items.Add(new CellInfo { No = 2, LineNos = [0, 1, 2], Id = "0000000000000000", Text = "🐛うさうさだよもん" });
        Items.Add(new CellInfo { No = 1, LineNos = [0, 1], Out = 2, Id = "0000000000000000", Text = "🐛うさうさだよもん" });
        Items.Add(new CellInfo { No = 0, LineNos = [0], Out = 1, Id = "0000000000000000", Text = "🐛うさうさだよもん" });
        Items.Add(new CellInfo { No = 0, LineNos = [0], Id = "0000000000000000", Text = "🐛うさうさだよもん" });
    }
}
