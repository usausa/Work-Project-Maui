using System.Collections.ObjectModel;
using Smart.Maui.ViewModels;

namespace WorkDesign;

public partial class UsaPage : ContentPage
{
	public UsaPage()
	{
		InitializeComponent();
	}

    private object? lastSelected;

    private void SelectableItemsView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not CollectionView collectionView)
        {
            return;
        }

        var currentSelectedItem = collectionView.SelectedItem;

        // 以前選択した項目と同じなら選択解除
        if (lastSelected != null && Equals(lastSelected, currentSelectedItem))
        {
            collectionView.SelectedItem = null;
            lastSelected = null;
        }
        else
        {
            lastSelected = currentSelectedItem;
        }

        var character = lastSelected as Character;
        Image.Source = character?.Full;
        Grid.IsVisible = character is not null;
    }
}

public class UsaPageViewModel : ExtendViewModelBase
{
    public ObservableCollection<Character> Characters { get; } = new();

    public UsaPageViewModel()
    {
        Characters.Add(new Character
        {
            Name = "Ruler",
            Color = Color.FromArgb("#81D4FA"),
            Face = "usa1_face.jpg",
            Full = "usa1_full.jpg"
        });
        Characters.Add(new Character
        {
            Name = "Caster",
            Color = Color.FromArgb("#F48FB1"),
            Face = "usa2_face.jpg",
            Full = "usa2_full.jpg"
        });
        Characters.Add(new Character
        {
            Name = "Saber",
            Color = Color.FromArgb("#80CBC4"),
            Face = "usa3_face.jpg",
            Full = "usa3_full.jpg"
        });
        Characters.Add(new Character
        {
            Name = "Berserker",
            Color = Color.FromArgb("#B0BEC5"),
            Face = "usa4_face.jpg",
            Full = "usa4_full.jpg"
        });
        Characters.Add(new Character
        {
            Name = "Lancer",
            Color = Color.FromArgb("#C5E1A5"),
            Face = "usa5_face.jpg",
            Full = "usa5_full.jpg"
        });
        Characters.Add(new Character
        {
            Name = "Rider",
            Color = Color.FromArgb("#EF9A9A"),
            Face = "usa6_face.jpg",
            Full = "usa6_full.jpg"
        });
        Characters.Add(new Character
        {
            Name = "Assassin",
            Color = Color.FromArgb("#B39DDB"),
            Face = "usa7_face.jpg",
            Full = "usa7_full.jpg"
        });
        Characters.Add(new Character
        {
            Name = "Alter Ego",
            Color = Color.FromArgb("#EEEEEE"),
            Face = "usa8_face.jpg",
            Full = "usa8_full.jpg"
        });
    }
}

public class Character
{
    public string Name { get; set; } = default!;

    public Color Color { get; set; } = default!;

    public string Face { get; set; } = default!;

    public string Full { get; set; } = default!;
}
