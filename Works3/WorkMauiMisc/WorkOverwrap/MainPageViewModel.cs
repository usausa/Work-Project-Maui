using System.Diagnostics;
using System.Windows.Input;
using Smart.Maui.Input;
using Smart.Maui.ViewModels;
using Smart.Mvvm.ViewModels;

namespace WorkOverwrap;

public sealed class MainPageViewModel : ExtendViewModelBase
{
    public ICommand StandaloneCommand { get; }

    public ICommand StandaloneWithBusyCommand { get; }

    public ICommand LinkedCommand { get; }

    public ICommand OtherLinkedDelegateCommand { get; }

    public ICommand OtherLinkedAsyncCommand { get; }

    public ICommand OtherWithBusyDelegateCommand { get; }

    public ICommand OtherWithBusyAsyncCommand { get; }

    public MainPageViewModel()
    {
        StandaloneCommand = new AsyncCommand(async () =>
        {
            Debug.WriteLine("*1");
            await Task.Delay(5000);
        });

        StandaloneWithBusyCommand = new AsyncCommand(async () =>
        {
            // [MEMO] 自分は対象外、確認対象は全て更新される
            Debug.WriteLine("*2");
            using var _ = BusyState.Begin();
            await Task.Delay(5000);
        });
        LinkedCommand = MakeAsyncCommand(async () =>
        {
            // [MEMO] 自分、確認対象は全て更新される
            Debug.WriteLine("*3");
            await Task.Delay(5000);
        });

        // 確認対象
        OtherLinkedDelegateCommand = MakeDelegateCommand(() => {});
        OtherLinkedAsyncCommand = MakeAsyncCommand(() => Task.CompletedTask);

        OtherWithBusyDelegateCommand = MakeDelegateCommand(() => {}, () => !BusyState.IsBusy);
        OtherWithBusyAsyncCommand = MakeAsyncCommand(() => Task.CompletedTask, () => !BusyState.IsBusy);
    }
}
