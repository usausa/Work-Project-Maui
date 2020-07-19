using System.Threading.Tasks;
using Smart.ComponentModel;
using Smart.Forms.Input;
using XamarinFormsComponents.Dialogs;

namespace DatabaseSample
{
    using Smart.Forms.ViewModels;

    public class MainPageViewModel : ViewModelBase
    {
        public static MainPageViewModel DesignInstance { get; } = null; // For design

        private readonly IDialogs dialogs;

        public NotificationValue<int> BulkDataCount { get; } = new NotificationValue<int>();

        public AsyncCommand InsertCommand { get; }
        public AsyncCommand UpdateCommand { get; }
        public AsyncCommand DeleteCommand { get; }
        public AsyncCommand QueryCommand { get; }

        public AsyncCommand BulkInsertCommand { get; }
        public AsyncCommand DeleteAllCommand { get; }
        public AsyncCommand QueryAllCommand { get; }

        public MainPageViewModel(
            IDialogs dialogs)
        {
            this.dialogs = dialogs;

            InsertCommand = MakeAsyncCommand(Insert);
            UpdateCommand = MakeAsyncCommand(Update);
            DeleteCommand = MakeAsyncCommand(Delete);
            QueryCommand = MakeAsyncCommand(Query);

            BulkInsertCommand = MakeAsyncCommand(BulkInsert, () => BulkDataCount.Value == 0).Observe(BulkDataCount);
            DeleteAllCommand = MakeAsyncCommand(DeleteAll, () => BulkDataCount.Value > 0).Observe(BulkDataCount);
            QueryAllCommand = MakeAsyncCommand(QueryAll);
        }

        public async Task Initialize()
        {
            await Task.Delay(0);
        }

        private async Task Insert()
        {
            await Task.Delay(5000);

            await dialogs.Information("test");
        }

        private async Task Update()
        {
            await Task.Delay(5000);

            await dialogs.Information("test");
        }

        private async Task Delete()
        {
            await Task.Delay(5000);

            await dialogs.Information("test");
        }

        private async Task Query()
        {
            await Task.Delay(5000);

            await dialogs.Information("test");
        }

        private async Task BulkInsert()
        {
            await Task.Delay(5000);

            await dialogs.Information("test");
        }

        private async Task DeleteAll()
        {
            await Task.Delay(5000);

            await dialogs.Information("test");
        }

        private async Task QueryAll()
        {
            await Task.Delay(5000);

            await dialogs.Information("test");
        }
    }
}
