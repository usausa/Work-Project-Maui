using System.Collections.ObjectModel;
using System.Windows.Input;
using Smart.Maui.ViewModels;
using Smart.Mvvm;

namespace WorkDesign;

public partial class UsaPage : ContentPage
{
	public UsaPage()
	{
		InitializeComponent();
	}
}

public partial class UsaPageViewModel : ExtendViewModelBase
{
    public ObservableCollection<Character> Characters { get; } = new();

    [ObservableProperty]
    public partial string? SelectedImage { get; set; }

    public ICommand SelectCommand { get; }

    public UsaPageViewModel()
    {
        SelectCommand = MakeDelegateCommand<Character>(x => SelectedImage = x.Full);

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
