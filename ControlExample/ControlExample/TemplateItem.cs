namespace ControlExample;

public class TemplateItem
{
    public string Name { get; set; } = default!;

    public bool Flag { get; set; }

    public static IReadOnlyList<TemplateItem> Items = new[]
    {
        new TemplateItem { Name = "Data-1", Flag = false },
        new TemplateItem { Name = "Data-2", Flag = true },
        new TemplateItem { Name = "Data-3", Flag = false },
        new TemplateItem { Name = "Data-4", Flag = true }
    };
}
