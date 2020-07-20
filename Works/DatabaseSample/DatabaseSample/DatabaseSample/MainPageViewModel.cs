namespace DatabaseSample
{
    using System;
    using System.Threading.Tasks;

    using DatabaseSample.Models;
    using DatabaseSample.Services;

    using Smart.ComponentModel;
    using Smart.Forms.Input;
    using Smart.Forms.ViewModels;

    using XamarinFormsComponents.Dialogs;


    public class MainPageViewModel : ViewModelBase
    {
        public static MainPageViewModel DesignInstance { get; } = null; // For design

        private readonly IDialogs dialogs;

        private readonly DataService dataService;

        public NotificationValue<int> BulkDataCount { get; } = new NotificationValue<int>();

        public AsyncCommand InsertCommand { get; }
        public AsyncCommand UpdateCommand { get; }
        public AsyncCommand DeleteCommand { get; }
        public AsyncCommand QueryCommand { get; }

        public AsyncCommand BulkInsertCommand { get; }
        public AsyncCommand DeleteAllCommand { get; }
        public AsyncCommand QueryAllCommand { get; }

        public MainPageViewModel(
            IDialogs dialogs,
            DataService dataService)
        {
            this.dialogs = dialogs;
            this.dataService = dataService;

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
            var ret = await dataService.InsertDataAsync(new DataEntity { Id = 1L, Name = "Data-1", CreateAt = DateTime.Now });

            if (ret)
            {
                await dialogs.Information("Inserted");
            }
            else
            {
                await dialogs.Information("Key duplicate");
            }
        }

        private async Task Update()
        {
            var effect = await dataService.UpdateDataAsync(1L, "Updated");

            await dialogs.Information($"Effect={effect}");
        }

        private async Task Delete()
        {
            var effect = await dataService.DeleteDataAsync(1L);

            await dialogs.Information($"Effect={effect}");
        }

        private async Task Query()
        {
            var entity = await dataService.QueryDataAsync(1L);

            if (entity != null)
            {
                await dialogs.Information($"Name={entity.Name}\r\nDate={entity.CreateAt:yyyy/MM/dd HH:mm:ss}");
            }
            else
            {
                await dialogs.Information("Not found");
            }
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
