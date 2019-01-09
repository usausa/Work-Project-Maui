namespace Business.FormsApp.Modules.Data
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Business.FormsApp.Models;

    using Smart.Forms.Components;
    using Smart.Forms.Input;
    using Smart.Navigation;

    using SQLite;

    public class DataMenuViewModel : AppViewModelBase
    {
        private readonly IDialogService dialogService;

        public AsyncCommand<ViewId> ForwardCommand { get; }

        public AsyncCommand TestCommand { get; }

        public AsyncCommand TestEncryptCommand { get; }

        public AsyncCommand Benchmark1Command { get; }

        public AsyncCommand Benchmark2Command { get; }

        public DataMenuViewModel(
            ApplicationState applicationState,
            IDialogService dialogService)
            : base(applicationState)
        {
            this.dialogService = dialogService;

            ForwardCommand = MakeAsyncCommand<ViewId>(x => Navigator.ForwardAsync(x));
            TestCommand = MakeAsyncCommand(Test);
            TestEncryptCommand = MakeAsyncCommand(TestEncrypt);
            Benchmark1Command = MakeAsyncCommand(Benchmark1);
            Benchmark2Command = MakeAsyncCommand(Benchmark2);

            using (var con = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Test.db")))
            {
                con.CreateTable<TestEntity>();
            }

            using (var con = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TestEncrypt.db"), key: "12345678"))
            {
                con.CreateTable<TestEntity>();
            }
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.Menu);
        }

        private async Task Test()
        {
            var con = new SQLiteAsyncConnection(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Test.db"));
            try
            {
                var watch = Stopwatch.StartNew();
                await Test(con);
                await dialogService.DisplayAlert("Result", $"Elapsed={watch.Elapsed}", "ok");
            }
            finally
            {
                await con.CloseAsync();
            }
        }

        private async Task TestEncrypt()
        {
            var con = new SQLiteAsyncConnection(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TestEncrypt.db"),
                key: "12345678");
            try
            {
                var watch = Stopwatch.StartNew();
                await Test(con);
                await dialogService.DisplayAlert("Result", $"Elapsed={watch.Elapsed}", "ok");
            }
            finally
            {
                await con.CloseAsync();
            }
        }

        private async Task Test(SQLiteAsyncConnection con)
        {
            await con.ExecuteAsync("DELETE FROM Test");

            await con.InsertAsync(new TestEntity { Id = 1, Name = "Data-1", Qty = 2, Price = 123.45m, Time = DateTime.Now });
            await con.ExecuteAsync("INSERT INTO Test (Id, Name, Qty, Price, Time) VALUES (?, ?, ?, ?, ?)", 2, "Data-2", 1, 100m, null);

            await con.QueryAsync<TestEntity>("SELECT * FROM Test");
        }

        private async Task Benchmark1()
        {
            var con = new SQLiteAsyncConnection(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Benchmark1.db"),
                key: "12345678");
            try
            {
                await con.CreateTableAsync<TestEntity>();

                await con.ExecuteAsync("DELETE FROM Test");

                var list = Enumerable.Range(1, 10000).Select(x => new TestEntity
                {
                    Id = x,
                    Name = "Data",
                    Qty = 0,
                    Price = 0m,
                    Time = DateTime.Now
                });

                var watch = Stopwatch.StartNew();

                await con.InsertAllAsync(list, typeof(TestEntity));

                await dialogService.DisplayAlert("Result", $"Elapsed={watch.Elapsed}", "ok");
            }
            finally
            {
                await con.CloseAsync();
            }
        }

        private async Task Benchmark2()
        {
            var con = new SQLiteAsyncConnection(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Benchmark2.db"),
                key: "12345678");
            try
            {
                await con.CreateTableAsync<TestEntity>();

                await con.ExecuteAsync("DELETE FROM Test");

                var list = Enumerable.Range(1, 10000).Select(x => new TestEntity
                {
                    Id = x,
                    Name = "Data",
                    Qty = 0,
                    Price = 0m,
                    Time = DateTime.Now
                });

                var watch = Stopwatch.StartNew();

                await con.RunInTransactionAsync(c =>
                {
                    foreach (var entity in list)
                    {
                        c.Execute("INSERT INTO Test (Id, Name, Qty, Price, Time) VALUES (?, ?, ?, ?, ?)", entity.Id, entity.Name, entity.Qty, entity.Price, entity.Time);
                    }
                });

                await dialogService.DisplayAlert("Result", $"Elapsed={watch.Elapsed}", "ok");
            }
            finally
            {
                await con.CloseAsync();
            }
        }
    }
}
