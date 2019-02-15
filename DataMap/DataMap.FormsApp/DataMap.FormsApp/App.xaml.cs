[assembly: Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]

namespace DataMap.FormsApp
{
    using System.IO;
    using System.Reflection;

    using DataMap.FormsApp.Components.Dialogs;
    using DataMap.FormsApp.Modules;
    using DataMap.FormsApp.Services;

    using Microsoft.Data.Sqlite;

    using Smart.Data;
    using Smart.Forms.Resolver;
    using Smart.Navigation;
    using Smart.Resolver;

    using Xamarin.Essentials;

    public partial class App
    {
        private readonly SmartResolver resolver;

        private readonly Navigator navigator;

        public App(IComponentProvider provider)
        {
            InitializeComponent();

            // Config Resolver
            resolver = CreateResolver(provider);
            ResolveProvider.Default.UseSmartResolver(resolver);

            // Config Navigator
            navigator = new NavigatorConfig()
                .UseFormsNavigationProvider()
                .UseResolver(resolver)
                .UseIdViewMapper(m => m.AutoRegister(Assembly.GetExecutingAssembly().ExportedTypes))
                .ToNavigator();
            navigator.Navigated += (sender, args) =>
            {
                // for debug
                System.Diagnostics.Debug.WriteLine(
                    $"Navigated: [{args.Context.FromId}]->[{args.Context.ToId}] : stacked=[{navigator.StackedCount}]");
            };

            // Show MainWindow
            MainPage = resolver.Get<MainPage>();
        }

        private SmartResolver CreateResolver(IComponentProvider provider)
        {
            var config = new ResolverConfig()
                .UseAutoBinding()
                .UseArrayBinding()
                .UseAssignableBinding()
                .UsePropertyInjector();

            config.Bind<INavigator>().ToMethod(kernel => navigator).InSingletonScope();
            config.Bind<IDialogs>().To<Dialogs>().InSingletonScope();

            var path = Path.Combine(FileSystem.AppDataDirectory, "Test.db");
            var connectionString = $"Data Source={path}";
            config.Bind<IConnectionFactory>()
                .ToConstant(new CallbackConnectionFactory(() => new SqliteConnection(connectionString)));

            config.Bind<ApplicationState>().ToSelf().InSingletonScope();

            config.Bind<DataService>().ToSelf().InSingletonScope();

            provider.RegisterComponents(config);

            return config.ToResolver();
        }

        protected override void OnStart()
        {
            var dataService = resolver.Get<DataService>();
            dataService.Initialize();

            navigator.Forward(ViewId.Menu);
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
