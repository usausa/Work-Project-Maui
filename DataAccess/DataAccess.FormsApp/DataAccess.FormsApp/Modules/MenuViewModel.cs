namespace DataAccess.FormsApp.Modules
{
    using System.IO;
    using System.Threading.Tasks;

    using DataAccess.FormsApp.Components;

    using Smart.ComponentModel;
    using Smart.Data;
    using Smart.Data.Mapper;
    using Smart.Forms.Input;

    public class MenuViewModel : AppViewModelBase
    {
        public static MenuViewModel DesignInstance { get; } = null; // For design

        private readonly Settings settings;

        private readonly IDialogs dialogs;

        private readonly IConnectionFactory connectionFactory;

        public NotificationValue<bool> IsCreated { get; } = new NotificationValue<bool>();

        public AsyncCommand CreateCommand { get; }
        public DelegateCommand DropCommand { get; }
        public AsyncCommand InsertCommand { get; }
        public AsyncCommand UpdateCommand { get; }
        public AsyncCommand DeleteCommand { get; }
        public AsyncCommand CountCommand { get; }
        public AsyncCommand Select1Command { get; }
        public AsyncCommand SelectAllCommand { get; }
        public AsyncCommand InsertBulkCommand { get; }
        public AsyncCommand DeleteAllCommand { get; }
        public AsyncCommand MemoryInsertBulkCommand { get; }
        public AsyncCommand MemoryDeleteAllCommand { get; }

        public MenuViewModel(
            ApplicationState applicationState,
            Settings settings,
            IDialogs dialogs,
            IConnectionFactory connectionFactory)
            : base(applicationState)
        {
            this.settings = settings;
            this.dialogs = dialogs;
            this.connectionFactory = connectionFactory;

            CreateCommand = MakeAsyncCommand(Create, () => !IsCreated.Value).Observe(IsCreated);
            DropCommand = MakeDelegateCommand(Drop, () => IsCreated.Value).Observe(IsCreated);
            InsertCommand = MakeAsyncCommand(Insert, () => IsCreated.Value).Observe(IsCreated);
            UpdateCommand = MakeAsyncCommand(Update, () => IsCreated.Value).Observe(IsCreated);
            DeleteCommand = MakeAsyncCommand(Delete, () => IsCreated.Value).Observe(IsCreated);
            CountCommand = MakeAsyncCommand(Count, () => IsCreated.Value).Observe(IsCreated);
            Select1Command = MakeAsyncCommand(Select1, () => IsCreated.Value).Observe(IsCreated);
            SelectAllCommand = MakeAsyncCommand(SelectAll, () => IsCreated.Value).Observe(IsCreated);
            InsertBulkCommand = MakeAsyncCommand(InsertBulk, () => IsCreated.Value).Observe(IsCreated);
            DeleteAllCommand = MakeAsyncCommand(DeleteAll, () => IsCreated.Value).Observe(IsCreated);
            MemoryInsertBulkCommand = MakeAsyncCommand(MemoryInsertBulk, () => IsCreated.Value).Observe(IsCreated);
            MemoryDeleteAllCommand = MakeAsyncCommand(MemoryDeleteAll, () => IsCreated.Value).Observe(IsCreated);

            IsCreated.Value = File.Exists(settings.DatabasePath);
        }

        private async Task Create()
        {
            // TODO
            await connectionFactory.UsingAsync(async con =>
                await con.ExecuteAsync(
                    "CREATE TABLE IF NOT EXISTS Data (" +
                    "Id int, " +
                    "Name text, " +
                    "PRIMARY KEY (Id))"));

            IsCreated.Value = true;
        }

        private void Drop()
        {
            File.Delete(settings.DatabasePath);

            IsCreated.Value = false;
        }

        private async Task Insert()
        {
            // TODO dup
            await connectionFactory.UsingAsync(async con =>
                await con.ExecuteAsync(
                    ""));
        }

        private async Task Update()
        {
            // TODO
            var effect = await connectionFactory.UsingAsync(async con =>
                await con.ExecuteAsync(
                    ""));

            await dialogs.Information($"Effect={effect}");
        }

        private async Task Delete()
        {
            // TODO
            var effect = await connectionFactory.UsingAsync(async con =>
                await con.ExecuteAsync(
                    ""));

            await dialogs.Information($"Effect={effect}");
        }

        private async Task Count()
        {
            var count = await connectionFactory.UsingAsync(async con =>
                await con.ExecuteScalarAsync<long>(
                    "SELECT COUNT(*) FROM Text"));

            await dialogs.Information($"Count={count}");
        }

        private async Task Select1()
        {
            // TODO
            await Task.Delay(0);
        }

        private async Task SelectAll()
        {
            // TODO
            await Task.Delay(0);
        }

        private async Task InsertBulk()
        {
            // TODO
            await Task.Delay(0);
        }

        private async Task DeleteAll()
        {
            // TODO
            await Task.Delay(0);
        }

        private async Task MemoryInsertBulk()
        {
            // TODO
            await Task.Delay(0);
        }

        private async Task MemoryDeleteAll()
        {
            // TODO
            await Task.Delay(0);
        }
    }
}
