namespace WorkGraphicBasic;

public class CellInfo
{
    public int No { get; set; }

    public int[] LineNos { get; set; } = default!;

    public int? In { get; set; }

    public int? Out { get; set; }

    public string Id { get; set; } = default!;

    public string Text { get; set; } = default!;
}
