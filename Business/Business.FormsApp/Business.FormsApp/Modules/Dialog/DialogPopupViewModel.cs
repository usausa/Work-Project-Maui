namespace Business.FormsApp.Modules.Dialog
{
    using System.Threading.Tasks;

    using Business.FormsApp.Components.Popup;

    using Smart.ComponentModel;
    using Smart.Forms.Input;

    public class DialogPopupViewModel : AppDialogViewModelBase, IPopupInitializeAsync<string>, IPopupResult<bool>
    {
        public static DialogPopupViewModel DesignInstance { get; } = null; // For design

        public NotificationValue<string> Text { get; } = new NotificationValue<string>();

        public AsyncCommand<bool> CloseCommand { get; }

        public bool Result { get; set; }

        public DialogPopupViewModel()
        {
            CloseCommand = MakeAsyncCommand<bool>(Close);
        }

        public Task Initialize(string parameter)
        {
            Text.Value = parameter;

            return Task.CompletedTask;
        }

        private async Task Close(bool result)
        {
            Result = result;
            await PopupNavigator.PopAsync();
        }
    }
}
