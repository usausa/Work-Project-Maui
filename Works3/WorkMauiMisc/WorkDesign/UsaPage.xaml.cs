using System.Collections.ObjectModel;
using Smart.Maui.ViewModels;

namespace WorkDesign;

public partial class UsaPage : ContentPage
{
	public UsaPage()
	{
		InitializeComponent();
	}
}

public class UsaPageViewModel : ExtendViewModelBase
{
    public ObservableCollection<Character> Characters { get; } = new();

    public UsaPageViewModel()
    {
        Characters.Add(new Character
        {
            Name = "Form 1",
            Color = "#808080",
            Face = "usa1_face.jpg",
            Full = "usa1_full.jpg"
        });
        Characters.Add(new Character
        {
            Name = "Form 2",
            Color = "#808080",
            Face = "usa2_face.jpg",
            Full = "usa2_full.jpg"
        });
        Characters.Add(new Character
        {
            Name = "Form 3",
            Color = "#808080",
            Face = "usa3_face.jpg",
            Full = "usa3_full.jpg"
        });
        Characters.Add(new Character
        {
            Name = "Form 4",
            Color = "#808080",
            Face = "usa4_face.jpg",
            Full = "usa4_full.jpg"
        });
        Characters.Add(new Character
        {
            Name = "Form 5",
            Color = "#808080",
            Face = "usa5_face.jpg",
            Full = "usa5_full.jpg"
        });
        Characters.Add(new Character
        {
            Name = "Form 6",
            Color = "#808080",
            Face = "usa6_face.jpg",
            Full = "usa6_full.jpg"
        });
        Characters.Add(new Character
        {
            Name = "Form 7",
            Color = "#808080",
            Face = "usa7_face.jpg",
            Full = "usa7_full.jpg"
        });
        Characters.Add(new Character
        {
            Name = "Form 8",
            Color = "#808080",
            Face = "usa8_face.jpg",
            Full = "usa8_full.jpg"
        });
    }
}

public class Character
{
    public string Name { get; set; } = default!;

    public string Color { get; set; } = default!;

    public string Face { get; set; } = default!;

    public string Full { get; set; } = default!;
}
