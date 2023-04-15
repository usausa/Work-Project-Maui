namespace ControlExample;

public class TemplateItemSelector : DataTemplateSelector
{
    public DataTemplate DefaultTemplate { get; set; } = default!;

    public DataTemplate SpecialTemplate { get; set; } = default!;

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        return ((TemplateItem)item).Flag ? SpecialTemplate : DefaultTemplate;
    }
}