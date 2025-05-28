namespace WorkNewBarcode;

using System.Diagnostics;
using System.Windows.Input;

using BarcodeScanning;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

public partial class MainPageViewModel : ExtendViewModelBase
{
    [ObservableProperty]
    public partial string Text { get; set; } = default!;

    public ICommand DetectCommand { get; }

    public MainPageViewModel()
    {
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
