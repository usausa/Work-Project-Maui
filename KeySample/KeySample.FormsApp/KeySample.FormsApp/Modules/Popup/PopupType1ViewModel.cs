namespace KeySample.FormsApp.Modules.Popup
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    using KeySample.FormsApp.Components.Dialog;

    using KeySample.FormsApp.Models.Input;

    using XamarinFormsComponents.Popup;

    public class PopupType1ViewModel : AppDialogViewModelBase, IPopupResult<string>, IPopupInitialize<TextInputParameter>
    {
        private readonly IApplicationDialog dialogs;

        public string Result { get; private set; } = string.Empty;

        public ICommand CloseCommand { get; }
        public ICommand CommitCommand { get; }

        public PopupType1ViewModel(
            IApplicationDialog dialogs)
        {
            this.dialogs = dialogs;

            //ClearCommand = MakeDelegateCommand(() => Input.Clear());
            //PopCommand = MakeDelegateCommand(() => Input.Pop());
            //PushCommand = MakeDelegateCommand<string>(x => Input.Push(x));

            CloseCommand = MakeAsyncCommand(Close);
            CommitCommand = MakeAsyncCommand(Commit);
        }

        public void Initialize(TextInputParameter parameter)
        {
        }

        private async Task Close()
        {
            //if ((currentText != Input.Text) &&
            //    (!await dialogs.Confirm("入力した内容をキャンセルし戻ります。よろしいですか？")))
            //{
            //    return;
            //}

            await PopupNavigator.PopAsync();
        }

        private async Task Commit()
        {
            //var result = Input.Text;
            //var message = callback?.Invoke(result);
            //if (!String.IsNullOrEmpty(message))
            //{
            //    await dialogs.Information(message);
            //    return;
            //}

            Result = "x";

            await PopupNavigator.PopAsync();
        }
    }
}
