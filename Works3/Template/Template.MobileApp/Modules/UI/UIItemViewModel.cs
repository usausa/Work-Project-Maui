namespace Template.MobileApp.Modules.UI;

public sealed class UIItemViewModel : AppViewModelBase
{
    public string Title { get; } = "Aqua Serum";

    public string Price { get; } = "$ 24.00";

    public string Category { get; } = "Skin Care";

    public string Description { get; } =
        "A lightweight hydrating serum with hyaluronic acid and marine minerals. Leaves your skin fresh, plump and deeply moisturized.";

    public IObserveCommand BackCommand { get; }

    public IObserveCommand CartCommand { get; }

    public UIItemViewModel()
    {
        BackCommand = MakeAsyncCommand(() => Navigator.ForwardAsync(ViewId.UIShop));
        CartCommand = MakeAsyncCommand(() => Navigator.ForwardAsync(ViewId.UICart));
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.UIShop);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
