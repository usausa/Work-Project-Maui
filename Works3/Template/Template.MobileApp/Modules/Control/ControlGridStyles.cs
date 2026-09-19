namespace Template.MobileApp.Modules.Control;

using ClamGrid;

using Template.MobileApp.Models.Control;

// ClamGrid の見た目。罫線は横だけ、見出しは白地。状態・フラグ・ランク・受付・金額・数量・納期は値に応じて色を付ける
internal static class ControlGridStyles
{
    private static readonly Color HeaderText = Color.FromArgb("#607D8B");

    private static readonly Color Line = Color.FromArgb("#ECEFF1");

    private static readonly GridColors OpenColors = new(Color.FromArgb("#616161"), Color.FromArgb("#F5F5F5"));

    private static readonly GridColors ProcessingColors = new(Color.FromArgb("#1565C0"), Color.FromArgb("#E3F2FD"));

    private static readonly GridColors HoldColors = new(Color.FromArgb("#EF6C00"), Color.FromArgb("#FFF3E0"));

    private static readonly GridColors CompletedColors = new(Color.FromArgb("#2E7D32"), Color.FromArgb("#E8F5E9"));

    private static readonly Color OverdueText = Color.FromArgb("#C62828");

    private static readonly Color OverdueBackground = Color.FromArgb("#FFEBEE");

    private static readonly Color DueSoonText = Color.FromArgb("#EF6C00");

    private static readonly Color DueSoonBackground = Color.FromArgb("#FFF3E0");

    private static readonly Color TopRankBackground = Color.FromArgb("#FFF8E1");

    private static readonly Color LargeAmountText = Color.FromArgb("#2E7D32");

    private static readonly Color BulkText = Color.FromArgb("#1565C0");

    private static readonly Color StoreText = Color.FromArgb("#6A1B9A");

    private static readonly Color WebText = Color.FromArgb("#1565C0");

    private static readonly Color PhoneText = Color.FromArgb("#2E7D32");

    private static readonly Color FaxText = Color.FromArgb("#6D4C41");

    public static GridStyle CreateList() => CreateBase() with
    {
        FrozenLineColor = Color.FromArgb("#CFD8DC"),
        AscendingHeaderBackground = Color.FromArgb("#E3F2FD"),
        DescendingHeaderBackground = Color.FromArgb("#FFF3E0"),
        AscendingSortMark = "▲",
        DescendingSortMark = "▼",
        SortMarkPosition = GridSortMarkPosition.End,
        ShowSortPriority = true,
        CellColors = SelectCellColors
    };

    // 行ヘッダはドラッグの取っ手 (AllowRowDragging のときライブラリが描く)
    public static GridStyle CreateSettings() => CreateBase() with
    {
        ShowRowHeaders = true,
        RowHeaderWidth = 56,
        RowHeaderBackground = Color.FromArgb("#F5F7FA"),
        CornerText = string.Empty
    };

    private static GridStyle CreateBase() => new()
    {
        FontFamily = "sans-serif",
        FontSize = 15,
        RowHeight = 46,
        HeaderHeight = 42,
        HorizontalPadding = 10,
        ShowRowHeaders = false,
        ShowVerticalLines = false,
        TextColor = Color.FromArgb("#37474F"),
        Background = Colors.White,
        HeaderBackground = Color.FromArgb("#F5F7FA"),
        HeaderTextColor = HeaderText,
        GridLineColor = Line,
        SelectedBackground = Color.FromArgb("#E3F2FD"),
        SelectedTextColor = Color.FromArgb("#0D47A1")
    };

    // 選択中は選択色を優先する
    private static GridColors SelectCellColors(GridCellColorContext context)
    {
        if (context.IsSelected || context.Item is not OrderRow row)
        {
            return context.DefaultColors;
        }

        return context.Column.Key switch
        {
            "Status" => row.Status switch
            {
                OrderStatus.Open => OpenColors,
                OrderStatus.Processing => ProcessingColors,
                OrderStatus.Hold => HoldColors,
                _ => CompletedColors
            },
            "Flags" when row.IsOverdue => context.DefaultColors with { Background = OverdueBackground },
            "Flags" when row.IsDueSoon => context.DefaultColors with { Background = DueSoonBackground },
            "Rank" when row.Rank >= 3 => context.DefaultColors with { Background = TopRankBackground },
            "Amount" when row.IsLargeAmount => context.DefaultColors with { TextColor = LargeAmountText },
            "Quantity" when row.IsBulk => context.DefaultColors with { TextColor = BulkText },
            "DueDate" when row.IsOverdue => context.DefaultColors with { TextColor = OverdueText },
            "DueDate" when row.IsDueSoon => context.DefaultColors with { TextColor = DueSoonText },
            "Channel" => context.DefaultColors with
            {
                TextColor = row.Channel switch
                {
                    OrderChannel.Store => StoreText,
                    OrderChannel.Web => WebText,
                    OrderChannel.Phone => PhoneText,
                    _ => FaxText
                }
            },
            _ => context.DefaultColors
        };
    }
}
