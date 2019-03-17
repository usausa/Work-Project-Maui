using System.IO;

namespace DataAccess.FormsApp.Modules
{
    using System.Threading.Tasks;

    using Smart.ComponentModel;
    using Smart.Forms.Input;

    public class MenuViewModel : AppViewModelBase
    {
        public static MenuViewModel DesignInstance { get; } = null; // For design

        public NotificationValue<bool> IsCreated { get; } = new NotificationValue<bool>();

        public AsyncCommand CreateCommand { get; }
        public AsyncCommand DropCommand { get; }
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
            Settings settings)
            : base(applicationState)
        {
            // TODO DI
            CreateCommand = MakeAsyncCommand(Create, () => !IsCreated.Value).Observe(IsCreated);
            DropCommand = MakeAsyncCommand(Drop, () => IsCreated.Value).Observe(IsCreated);
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
            await Task.Delay(0);

            IsCreated.Value = true;
        }

        private async Task Drop()
        {
            // TODO
            await Task.Delay(0);

            IsCreated.Value = false;
        }

        private async Task Insert()
        {
            // TODO
            await Task.Delay(0);
        }

        private async Task Update()
        {
            // TODO
            await Task.Delay(0);
        }

        private async Task Delete()
        {
            // TODO
            await Task.Delay(0);
        }

        private async Task Count()
        {
            // TODO
            await Task.Delay(0);
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
