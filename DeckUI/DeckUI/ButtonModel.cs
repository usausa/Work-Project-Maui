using System.Collections.ObjectModel;

namespace DeckUI;

public enum ButtonType
{
    Text,
    Image
}

public class ButtonModel
{
    public int Row { get; set; }

    public int Column { get; set; }

    public ButtonType ButtonType { get; set; }

    public string Label { get; set; } = default!;

    public string Image { get; set; } = default!;

    public string Text { get; set; } = default!;

    public Color TextColor { get; set; } = Colors.White;

    public Color BackColor1 { get; set; } = Colors.Black;

    public Color BackColor2 { get; set; } = Colors.Black;
}

public class DeckModel
{
    public int Rows { get; set; }

    public int Columns { get; set; }

    public ObservableCollection<ButtonModel> Buttons { get; } = new();
}

public class ButtonDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate TextTemplate { get; set; } = default!;

    public DataTemplate ImageTemplate { get; set; } = default!;

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        return ((ButtonModel)item).ButtonType == ButtonType.Image ? ImageTemplate : TextTemplate;
    }
}
