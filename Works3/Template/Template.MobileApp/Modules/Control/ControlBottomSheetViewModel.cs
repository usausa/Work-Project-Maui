namespace Template.MobileApp.Modules.Control;

// Syncfusion の SfBottomSheet と自作の BottomSheetView (Controls/BottomSheetView.cs) を同じ内容で比べる
public sealed partial class ControlBottomSheetViewModel : AppViewModelBase
{
    [ObservableProperty]
    public partial bool IsSfSheetOpen { get; set; }

    [ObservableProperty]
    public partial bool IsCustomSheetOpen { get; set; }

    [ObservableProperty]
    public partial string Message { get; set; } = "シートはまだ開いていません";

    public IReadOnlyList<string> Actions { get; } = ["共有", "リンクをコピー", "お気に入りに追加", "レポート"];

    public IObserveCommand OpenSfSheetCommand { get; }

    public IObserveCommand OpenCustomSheetCommand { get; }

    public IObserveCommand CloseCommand { get; }

    public IObserveCommand SelectCommand { get; }

    public ControlBottomSheetViewModel()
    {
        OpenSfSheetCommand = MakeDelegateCommand(() => IsSfSheetOpen = true);
        OpenCustomSheetCommand = MakeDelegateCommand(() => IsCustomSheetOpen = true);
        CloseCommand = MakeDelegateCommand(Close);
        SelectCommand = MakeDelegateCommand<string>(x =>
        {
            Message = $"選択: {x}";
            Close();
        });
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.ControlMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();

    protected override Task OnNotifyFunction2()
    {
        IsSfSheetOpen = true;
        return Task.CompletedTask;
    }

    protected override Task OnNotifyFunction3()
    {
        IsCustomSheetOpen = true;
        return Task.CompletedTask;
    }

    private void Close()
    {
        IsSfSheetOpen = false;
        IsCustomSheetOpen = false;
    }
}
