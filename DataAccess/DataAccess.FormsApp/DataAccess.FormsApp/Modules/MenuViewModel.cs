namespace DataAccess.FormsApp.Modules
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using DataAccess.FormsApp.Components;
    using DataAccess.FormsApp.Models;

    using Smart.ComponentModel;
    using Smart.Data;
    using Smart.Data.Mapper;
    //using Smart.Data.Mapper.Builders;
    using Smart.Forms.Input;

    using Microsoft.Data.Sqlite;

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
            MemoryInsertBulkCommand = MakeAsyncCommand(MemoryInsertBulk);
            MemoryDeleteAllCommand = MakeAsyncCommand(MemoryDeleteAll);

            IsCreated.Value = File.Exists(settings.DatabasePath);
        }

        private async Task Create()
        {
            await connectionFactory.UsingAsync(async con =>
                await con.ExecuteAsync(
                    "CREATE TABLE IF NOT EXISTS Test (" +
                    "Id INTEGER, " +
                    "StringValue TEXT, " +
                    "IntValue INTEGER, " +
                    "LongValue INTEGER, " +
                    "DoubleValue REAL, " +
                    "DecimalValue REAL, " +
                    "BoolValue INTEGER, " +
                    "DateTimeOffsetValue INTEGER, " +
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
            var ret = await connectionFactory.UsingAsync(async con =>
            {
                try
                {
                    await con.ExecuteAsync(
                        //SqlInsert<TestEntity>.Values(),
                        "INSERT INTO Test (" +
                        "Id, StringValue, IntValue, LongValue, DoubleValue, DecimalValue, BoolValue, DateTimeOffsetValue" +
                        ") VALUES (" +
                        "@Id, @StringValue, @IntValue, @LongValue, @DoubleValue, @DecimalValue, @BoolValue, @DateTimeOffsetValue" +
                        ")",
                        new TestEntity
                        {
                            Id = 1,
                            StringValue = "Test",
                            IntValue = 2,
                            LongValue = 3L,
                            DoubleValue = 4.5d,
                            DecimalValue = 6.7m,
                            BoolValue = true,
                            DateTimeOffsetValue = DateTimeOffset.Now
                        });
                    return true;
                }
                catch (SqliteException e)
                {
                    if (e.SqliteErrorCode == SQLitePCL.raw.SQLITE_CONSTRAINT)
                    {
                        return false;
                    }
                    throw;
                }
            });

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
            var effect = await connectionFactory.UsingAsync(con =>
                con.ExecuteAsync(
                    //SqlUpdate<TestEntity>.Set("StringValue = @StringValue", "Id = @Id"),
                    "UPDATE Test SET StringValue = @StringValue WHERE Id = @Id",
                    new { Id = 1, StringValue = "Updated" }));

            await dialogs.Information($"Effect={effect}");
        }

        private async Task Delete()
        {
            var effect = await connectionFactory.UsingAsync(con =>
                con.ExecuteAsync(
                    //SqlDelete<TestEntity>.ByKey(),
                    "DELETE FROM Test WHERE Id = @Id",
                    new { Id = 1 }));

            await dialogs.Information($"Effect={effect}");
        }

        private async Task Count()
        {
            var count = await connectionFactory.UsingAsync(con =>
                con.ExecuteScalarAsync<long>(
                    //SqlCount<TestEntity>.All()));
                    "SELECT COUNT(*) FROM Test"));

            await dialogs.Information($"Count={count}");
        }

        private async Task Select1()
        {
            var entity = await connectionFactory.UsingAsync(con =>
                con.QueryFirstOrDefaultAsync<TestEntity>(
                    //SqlSelect<TestEntity>.ByKey(),
                    "SELECT * FROM Test WHERE Id = @Id",
                    new { Id = 1 }));

            if (entity != null)
            {
                await dialogs.Information($"StringValue={entity.StringValue}");
            }
            else
            {
                await dialogs.Information("Not found");
            }
        }

        private async Task SelectAll()
        {
            var watch = Stopwatch.StartNew();

            // TODO
            var list = (await connectionFactory.UsingAsync(async con =>
                (await con.QueryAsync<TestEntity>(
                    //SqlSelect<TestEntity>.All())).ToList()));
                    "SELECT * FROM Test ORDER BY Id")).ToList()));

            await dialogs.Information($"Count={list.Count}\r\nElapsed={watch.ElapsedMilliseconds}");
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
