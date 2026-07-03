namespace Template.MobileApp.Modules.UI;

public sealed class UIKitTrackingStep
{
    public string Title { get; init; } = string.Empty;
    public string Time { get; init; } = string.Empty;
    public bool Done { get; init; }
    public bool IsComplete => Done;
    public bool IsPending => !Done;
}

public sealed class UIKitTrackingViewModel : AppViewModelBase
{
    public string OrderNumber { get; } = "Order #A1284";
    public string Eta { get; } = "Estimated arrival: Today 16:30";

    public IReadOnlyList<UIKitTrackingStep> Steps { get; } =
    [
        new() { Title = "Order received", Time = "09:12", Done = true },
        new() { Title = "Packed", Time = "10:45", Done = true },
        new() { Title = "Shipped", Time = "12:30", Done = true },
        new() { Title = "Out for delivery", Time = "15:00", Done = false },
        new() { Title = "Delivered", Time = "--:--", Done = false }
    ];

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.UIKitDash);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
