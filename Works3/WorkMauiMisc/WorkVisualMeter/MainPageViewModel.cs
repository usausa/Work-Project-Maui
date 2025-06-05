namespace WorkVisualMeter;

using System.Diagnostics;

using Smart.Mvvm.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    public bool Pressed
    {
        get => field;
        set
        {
            field = value;
            Debug.WriteLine($"* Changed : {value}");
        }
    }
}
