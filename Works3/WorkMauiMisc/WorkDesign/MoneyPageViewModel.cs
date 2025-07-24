namespace WorkDesign;

using System.Diagnostics;
using System.Windows.Input;

using Smart.Maui.ViewModels;

public sealed class MoneyPageViewModel : ExtendViewModelBase
{
    public ICommand Execute { get; }

    public MoneyPageViewModel()
    {
        Execute = MakeDelegateCommand(() =>
        {
            Debug.WriteLine("*");
        });
    }
}
