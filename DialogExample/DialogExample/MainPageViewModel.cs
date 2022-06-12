namespace DialogExample;

using System.Windows.Input;

using DialogExample.Components.Dialog;

using Smart.Maui.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    public ICommand InformationCommand { get; }

    public ICommand ConfirmCommand { get; }

    public ICommand SelectCommand { get; }

    public MainPageViewModel()
    {
        InformationCommand = MakeAsyncCommand(async () => await Dialog.Information("information"));
        ConfirmCommand = MakeAsyncCommand(async () => await Dialog.Confirm("confirm"));
        SelectCommand = MakeAsyncCommand(async () => await Dialog.Select(new[] { "Item-1", "Item-2", "Item-3" }));
    }
}
