namespace WorkNewCamera;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

public partial class MainPageViewModel : ExtendViewModelBase
{
    [ObservableProperty]
    public partial string Text { get; set; } = default!;
}
