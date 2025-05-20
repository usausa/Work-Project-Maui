using WorkSmartMaui.Shell;

namespace WorkSmartMaui;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Windows.Input;

using Smart.Maui.Messaging;
using Smart.Mvvm;

internal partial class MainPageViewModel : AppViewModelBase
{
    public IProgressView ProgressView { get; }

    public FocusController FocusController { get; } = new();

    public ValidationFocusRequest ValidationFocusRequest { get; } = new();

    [ObservableProperty]
    public partial bool Enable { get; set; }

    [Required(ErrorMessage = "Required")]
    [ObservableProperty]
    public partial string Text1 { get; set; } = default!;

    public ICommand BusyCommand { get; }

    public ICommand EnableCommand { get; }

    public ICommand Focus1Command { get; }
    public ICommand Focus2Command { get; }

    public ICommand ErrorCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand FocusErrorCommand { get; }

    public MainPageViewModel()
    {
        ProgressView = ProgressResolver.ResolveView();
        var progress = ProgressResolver.ResolveController();

        BusyCommand = MakeAsyncCommand(async () =>
        {
            Debug.WriteLine("* Start");
            await Task.Delay(2000);
            progress.Circle();
            await Task.Delay(3000);
            progress.Clear();
            await Task.Delay(2000);
            Debug.WriteLine("* End");
        });

        EnableCommand = MakeDelegateCommand(() => Enable = !Enable);

        Focus1Command = MakeDelegateCommand(() => FocusController.FocusRequest("Target1"));
        Focus2Command = MakeAsyncCommand(async () =>
        {
            await Task.Delay(1000);
            FocusController.FocusRequest("Target2");

            // Test
            Dispatcher.GetForCurrentThread()?.DispatchDelayed(TimeSpan.FromMilliseconds(50), () =>
            {
                var focused = FocusController.FindRequest();
                Debug.WriteLine($"* Focused {focused}");
            });
        });

        ErrorCommand = MakeDelegateCommand(() =>
        {
            Errors.AddError(nameof(Text1), "Manual error");
        });
        ClearCommand = MakeDelegateCommand(() =>
        {
            Errors.ClearErrors(nameof(Text1));
        });
        FocusErrorCommand = MakeDelegateCommand(() =>
        {
            ValidationFocusRequest.FocusRequest();
        });
    }
}
