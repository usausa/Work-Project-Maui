namespace WorkGridBind;

using WorkGridBind;

public class ButtonDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate TextTemplate { get; set; } = default!;

    public DataTemplate ImageTemplate { get; set; } = default!;

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        return ((DeckButtonInfo)item).ButtonType == DeckButtonType.Image ? ImageTemplate : TextTemplate;
    }
}
