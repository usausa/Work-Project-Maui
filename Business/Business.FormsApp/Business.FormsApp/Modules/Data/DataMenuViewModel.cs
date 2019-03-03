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
        public static DataMenuViewModel DesignInstance { get; } = null; // For design

        private readonly IDialogService dialogService;

        public AsyncCommand<ViewId> ForwardCommand { get; }

        public AsyncCommand TestCommand { get; }

        public AsyncCommand TestEncryptCommand { get; }

        public AsyncCommand Benchmark1Command { get; }

        public AsyncCommand Benchmark2Command { get; }

        public AsyncCommand Benchmark3Command { get; }

        public AsyncCommand Benchmark4Command { get; }

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
            Benchmark3Command = MakeAsyncCommand(Benchmark3);
            Benchmark4Command = MakeAsyncCommand(Benchmark4);

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
            using (var con = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Test.db")))
            {
                var watch = Stopwatch.StartNew();
                Test(con);
                await dialogService.DisplayAlert("Result", $"Elapsed={watch.Elapsed}", "ok");
            }
        }

        private async Task TestEncrypt()
        {
            using (var con = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TestEncrypt.db"), key: "12345678"))
            {
                var watch = Stopwatch.StartNew();
                Test(con);
                await dialogService.DisplayAlert("Result", $"Elapsed={watch.Elapsed}", "ok");
            }
        }

        private void Test(SQLiteConnection con)
        {
            con.Execute("DELETE FROM Test");

            con.Insert(new TestEntity { Id = 1, Name = "Data-1", Qty = 2, Price = 123.45m, Time = DateTime.Now });
            con.Execute("INSERT INTO Test (Id, Name, Qty, Price, Time) VALUES (?, ?, ?, ?, ?)", 2, "Data-2", 1, 100m, null);

            con.Query<TestEntity>("SELECT * FROM Test");
        }

        private async Task Benchmark1()
        {
            using (var con = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Benchmark1.db"), key: "12345678"))
            {
                con.CreateTable<TestEntity>();

                con.Execute("DELETE FROM Test");

                var list = Enumerable.Range(1, 10000).Select(x => new TestEntity
                {
                    Id = x,
                    Name = "Data",
                    Qty = 0,
                    Price = 0m,
                    Time = DateTime.Now
                });

                var watch = Stopwatch.StartNew();

                con.InsertAll(list, typeof(TestEntity));

                await dialogService.DisplayAlert("Result", $"Elapsed={watch.Elapsed}", "ok");
            }
        }

        private async Task Benchmark2()
        {
            using (var con = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Benchmark2.db"), key: "12345678"))
            {
                con.CreateTable<TestEntity>();

                con.Execute("DELETE FROM Test");

                var list = Enumerable.Range(1, 10000).Select(x => new TestEntity
                {
                    Id = x,
                    Name = "Data",
                    Qty = 0,
                    Price = 0m,
                    Time = DateTime.Now
                });

                var watch = Stopwatch.StartNew();

                con.BeginTransaction();

                foreach (var entity in list)
                {
                    con.Execute("INSERT INTO Test (Id, Name, Qty, Price, Time) VALUES (?, ?, ?, ?, ?)", entity.Id, entity.Name, entity.Qty, entity.Price, entity.Time);
                }

                con.Commit();

                await dialogService.DisplayAlert("Result", $"Elapsed={watch.Elapsed}", "ok");
            }
        }

        private async Task Benchmark3()
        {
            using (var con = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Benchmark1.db"), key: "12345678"))
            {
                var watch = Stopwatch.StartNew();

                var list = con.Query<TestEntity>("SELECT * FROM Test");

                await dialogService.DisplayAlert("Result", $"Elapsed={watch.Elapsed}\r\nCount={list.Count}", "ok");
            }
        }

        private async Task Benchmark4()
        {
            // Slow?
            using (var con = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Benchmark1.db"), SQLiteOpenFlags.ReadOnly | SQLiteOpenFlags.NoMutex, key: "12345678"))
            {
                var watch = Stopwatch.StartNew();

                var list = con.Query<TestEntity>("SELECT * FROM Test");

                await dialogService.DisplayAlert("Result", $"Elapsed={watch.Elapsed}\r\nCount={list.Count}", "ok");
            }
        }
    }
}
