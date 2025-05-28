namespace WorkNewBarcode;

using System.Diagnostics;
using System.Windows.Input;

using BarcodeScanning;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

public partial class MainPageViewModel : ExtendViewModelBase
{
    public BarcodeController Controller { get; } = new();

    [ObservableProperty]
    public partial string Text { get; set; } = default!;

    public ICommand FlipCommand { get; }

    public ICommand DetectCommand { get; }

    public MainPageViewModel()
    {
        FlipCommand = MakeDelegateCommand(() => Controller.Enable = !Controller.Enable);
        DetectCommand = MakeDelegateCommand<IReadOnlySet<BarcodeResult>>(result =>
        {
            if (result.Count > 0)
            {
                Debug.WriteLine("* Detected");
                Text = result.First().DisplayValue;
            }
        });
    }
}
