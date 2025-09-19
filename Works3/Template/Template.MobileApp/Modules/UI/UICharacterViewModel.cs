namespace Template.MobileApp.Modules.UI;

public sealed class UICharacterViewModel : AppViewModelBase
{
    public ObservableCollection<CharacterItem> Characters { get; } = new();

    public UICharacterViewModel()
    {
        Characters.Add(new CharacterItem { Name = "Form 1", Color = Colors.Pink, Face = "usa1_face.jpg", Full = "usa1_full.jpg" });
        Characters.Add(new CharacterItem { Name = "Form 2", Color = Colors.Pink, Face = "usa2_face.jpg", Full = "usa2_full.jpg" });
        Characters.Add(new CharacterItem { Name = "Form 3", Color = Colors.Pink, Face = "usa3_face.jpg", Full = "usa3_full.jpg" });
        Characters.Add(new CharacterItem { Name = "Form 4", Color = Colors.Pink, Face = "usa4_face.jpg", Full = "usa4_full.jpg" });
        Characters.Add(new CharacterItem { Name = "Form 5", Color = Colors.Pink, Face = "usa5_face.jpg", Full = "usa5_full.jpg" });
        Characters.Add(new CharacterItem { Name = "Form 6", Color = Colors.Pink, Face = "usa6_face.jpg", Full = "usa6_full.jpg" });
        Characters.Add(new CharacterItem { Name = "Form 7", Color = Colors.Pink, Face = "usa7_face.jpg", Full = "usa7_full.jpg" });
        Characters.Add(new CharacterItem { Name = "Form 8", Color = Colors.Pink, Face = "usa8_face.jpg", Full = "usa8_full.jpg" });
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.UIMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
