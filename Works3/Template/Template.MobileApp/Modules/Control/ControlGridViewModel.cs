namespace Template.MobileApp.Modules.Control;

using ClamGrid;

using Template.MobileApp.Models.Control;

public sealed partial class ControlGridViewModel : AppViewModelBase
{
    private const int RowCount = 2000;

    private static readonly GridSortOrder[] DefaultSortOrders = [new("DueDate"), new("OrderNo")];

    private readonly IDialog dialog;

    // 行・選択・ソート状態をまとめて持つ (グリッドの ItemsSource)
    public GridDataView<OrderRow> Rows { get; }

    public GridStyle GridStyle { get; } = ControlGridStyles.CreateList();

    // 列の表示 / 順序。TwoWay で結び、グリッドが正規化した値を書き戻す (null は既定へ戻す)
    [ObservableProperty]
    public partial IReadOnlyList<GridColumnOrder>? ColumnOrders { get; set; }

    // ソートキーと方向。見出しタップ後にグリッドが書き戻す
    [ObservableProperty]
    public partial IReadOnlyList<GridSortOrder>? SortOrders { get; set; }

    [ObservableProperty]
    public partial int OpenCount { get; set; }

    [ObservableProperty]
    public partial int SelectedCount { get; set; }

    [ObservableProperty]
    public partial string Message { get; set; } = string.Empty;

    // 行の長押し: true なら未処理だけを選択、false なら全解除
    public IObserveCommand SelectAllCommand { get; }

    // 見出しの長押し: 列設定画面へ (編集用のコピーを渡し、戻りで反映する)
    public IObserveCommand ColumnEditCommand { get; }

    public IObserveCommand CellValueChangedCommand { get; }

    public IObserveCommand CommitCommand { get; }

    public IObserveCommand AdvanceCommand { get; }

    public ControlGridViewModel(IDialog dialog)
    {
        this.dialog = dialog;

        Rows = new GridDataView<OrderRow>(Array.Empty<OrderRow>(), static x => x.Id);
        Rows.RegisterSort("Status", static x => x.Status);
        Rows.RegisterSort("OrderNo", static x => x.Id);
        Rows.RegisterSort("Customer", static x => x.Customer, StringComparer.CurrentCulture);
        Rows.RegisterSort("Rank", static x => x.Rank);
        Rows.RegisterSort("Product", static x => x.Product, StringComparer.CurrentCulture);
        Rows.RegisterSort("Quantity", static x => x.Quantity);
        Rows.RegisterSort("Amount", static x => x.Amount);
        Rows.RegisterSort("DueDate", static x => x.DueDate);
        Rows.RegisterSort("Channel", static x => x.Channel);
        Rows.RegisterSort("Staff", static x => x.Staff, StringComparer.CurrentCulture);
        Rows.RegisterSort("UpdatedAt", static x => x.UpdatedAt);
        Rows.PropertyChanged += OnRowsPropertyChanged;
        Disposables.Add(Rows);

        SortOrders = DefaultSortOrders;

        SelectAllCommand = MakeDelegateCommand<bool>(x => Rows.UpdateSelection(row => x && (row.Status == OrderStatus.Open)));
        ColumnEditCommand = MakeAsyncCommand<GridColumnConfigurationEventArgs>(x =>
            Navigator.PushAsync(ViewId.ControlGridColumn, Parameters.MakeColumnEditSession(x.CreateEditSession())));
        CellValueChangedCommand = MakeDelegateCommand<GridCellValueEventArgs>(x =>
            Message = $"確認: {((OrderRow)x.Item).OrderNo} = {(x.NewValue ? "済" : "未")}");
        CommitCommand = MakeAsyncCommand(CommitAsync, () => SelectedCount > 0);
        AdvanceCommand = MakeDelegateCommand(Advance, () => SelectedCount > 0);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Rows.PropertyChanged -= OnRowsPropertyChanged;
        }

        base.Dispose(disposing);
    }

    public override Task OnNavigatingToAsync(INavigationContext context)
    {
        if (!context.Attribute.IsRestore())
        {
            Load();
        }
        else if (context.Parameter.TryGetColumnOrders(out var orders))
        {
            ColumnOrders = orders;
            Message = $"列設定を反映: 表示 {orders.Count(static x => x.IsVisible)} / {orders.Length} 列";
        }

        return Task.CompletedTask;
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.ControlMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();

    protected override Task OnNotifyFunction2()
    {
        Rows.ClearSelection();
        return Task.CompletedTask;
    }

    protected override Task OnNotifyFunction3()
    {
        // 列設定とソートを既定へ (null を渡すとグリッドが既定を適用して正規化した値を書き戻す)
        ColumnOrders = null;
        SortOrders = DefaultSortOrders;
        Message = "列設定とソートを既定に戻しました";
        return Task.CompletedTask;
    }

    protected override Task OnNotifyFunction4()
    {
        Load();
        return Task.CompletedTask;
    }

    private void Load()
    {
        Rows.SetSource(OrderSamples.Create(RowCount));
        UpdateCounts();
        Message = "見出しタップで並べ替え、長押しで列設定。行タップで選択、長押しで未処理を一括選択";
    }

    private async Task CommitAsync()
    {
        var selected = Rows.SelectedItems.Cast<OrderRow>().ToList();
        var amount = selected.Sum(static x => (long)x.Amount);
        await dialog.InformationAsync(String.Format(CultureInfo.CurrentCulture, "{0} 件を確定しました。\n合計 ¥{1:N0}", selected.Count, amount));
        Message = $"確定: {selected.Count} 件 / {String.Join(", ", selected.Take(3).Select(static x => x.OrderNo))}{(selected.Count > 3 ? " …" : string.Empty)}";
    }

    // 選択行の状態を進める。行の変更通知でグリッドが並べ替えと色を追従させる
    private void Advance()
    {
        var selected = Rows.SelectedItems.Cast<OrderRow>().ToList();
        foreach (var row in selected)
        {
            row.AdvanceStatus();
        }

        UpdateCounts();
        Message = $"状態更新: {selected.Count} 件";
    }

    private void UpdateCounts()
    {
        var open = 0;
        foreach (var row in Rows)
        {
            if (row.Status == OrderStatus.Open)
            {
                open++;
            }
        }

        OpenCount = open;
    }

    private void OnRowsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(Rows.SelectedCount) or nameof(Rows.Count))
        {
            SelectedCount = Rows.SelectedCount;
            CommitCommand.RaiseCanExecuteChanged();
            AdvanceCommand.RaiseCanExecuteChanged();
        }
    }
}
