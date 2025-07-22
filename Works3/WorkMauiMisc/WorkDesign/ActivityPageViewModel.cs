using System.Windows.Input;
using Smart.Maui.ViewModels;
using Smart.Mvvm;
using Template.MobileApp.Graphics;

namespace WorkDesign;

public sealed partial class ActivityPageViewModel : ExtendViewModelBase
{
    [ObservableProperty]
    public partial int Step { get; set; }

    public ActivityGraphics Graphics { get; } = new();

    public ICommand StepCommand { get; }

    public ActivityPageViewModel()
    {
        StepCommand = MakeDelegateCommand(() =>
        {
            Step += Random.Shared.Next(100) + 1;
            Graphics.Step = Step;
        });
    }
}
