namespace WorkSmartMaui;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Windows.Input;

using Smart.Maui.Messaging;
using Smart.Mvvm;

using WorkSmartMaui.Shell;

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

            await Task.Delay(1000);

            var message = progress.Message();
            message.Update("Executing 1");
            await Task.Delay(1000);
            message.Update("Executing 2");
            await Task.Delay(1000);
            message.Update("Executing 3");
            await Task.Delay(1000);

            var rate = progress.Rate();
            for (var i = 0; i <= 100; i++)
            {
                rate.Update(i);
                await Task.Delay(20);
            }

            progress.Circle();
            await Task.Delay(3000);

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
        FocusErrorCommand = MakeDelegateCommand(ValidationFocusRequest.FocusRequest);
    }
}
