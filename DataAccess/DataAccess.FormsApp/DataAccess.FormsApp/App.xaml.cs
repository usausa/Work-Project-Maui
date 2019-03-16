[assembly: Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]

namespace DataAccess.FormsApp
{
    using System.IO;

    using Microsoft.Data.Sqlite;

    using Smart.Data.Mapper;

    using Xamarin.Essentials;

    public partial class App
    {
        public App(IComponentProvider provider)
        {
            InitializeComponent();

            var path = Path.Combine(FileSystem.AppDataDirectory, "Test.db");
            var connectionString = $"Data Source={path}";
            var con = new SqliteConnection(connectionString);
            con.Open();

            con.Execute("CREATE TABLE IF NOT EXISTS Item (Id int PRIMARY KEY, Name text, Price int)");

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
