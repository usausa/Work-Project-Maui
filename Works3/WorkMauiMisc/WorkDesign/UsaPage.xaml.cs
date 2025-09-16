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
            Name = "Avatar1",
            EnglishName = "Avatar1",
            Color = "#808080",
            Avatar = "face1.jpg",
            Face = "face1.jpg",
            Full = "usamusume1.jpg"
        });
        Characters.Add(new Character
        {
            Name = "Avatar2",
            EnglishName = "Avatar2",
            Color = "#808080",
            Avatar = "face2.jpg",
            Face = "face2.jpg",
            Full = "usamusume2.jpg"
        });
        Characters.Add(new Character
        {
            Name = "Avatar3",
            EnglishName = "Avatar3",
            Color = "#808080",
            Avatar = "face3.jpg",
            Face = "face3.jpg",
            Full = "usamusume3.jpg"
        });
        Characters.Add(new Character
        {
            Name = "Avatar4",
            EnglishName = "Avatar4",
            Color = "#808080",
            Avatar = "face4.jpg",
            Face = "face4.jpg",
            Full = "usamusume4.jpg"
        });
        Characters.Add(new Character
        {
            Name = "Avatar5",
            EnglishName = "Avatar5",
            Color = "#808080",
            Avatar = "face5.jpg",
            Face = "face5.jpg",
            Full = "usamusume5.jpg"
        });
        Characters.Add(new Character
        {
            Name = "Avatar6",
            EnglishName = "Avatar6",
            Color = "#808080",
            Avatar = "face6.jpg",
            Face = "face6.jpg",
            Full = "usamusume6.jpg"
        });
        Characters.Add(new Character
        {
            Name = "Avatar7",
            EnglishName = "Avatar7",
            Color = "#808080",
            Avatar = "face7.jpg",
            Face = "face7.jpg",
            Full = "usamusume7.jpg"
        });
        Characters.Add(new Character
        {
            Name = "Avatar8",
            EnglishName = "Avatar8",
            Color = "#808080",
            Avatar = "face8.jpg",
            Face = "face8.jpg",
            Full = "usamusume8.jpg"
        });
    }
}

public class Character
{
    public string Name { get; set; } = default!;

    public string EnglishName { get; set; } = default!;

    public string Color { get; set; } = default!;

    public string Avatar { get; set; } = default!;

    public string Face { get; set; } = default!;

    public string Full { get; set; } = default!;
}
