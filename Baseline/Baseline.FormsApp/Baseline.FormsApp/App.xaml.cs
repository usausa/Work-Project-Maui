namespace Baseline.FormsApp
{
    using System.Reflection;

    using Baseline.FormsApp.Components.Dialogs;
    using Baseline.FormsApp.Components.Location;
    using Baseline.FormsApp.Components.Popup;
    using Baseline.FormsApp.Components.Settings;
    using Baseline.FormsApp.Modules;
    using Baseline.FormsApp.Services;

    using Smart.Forms.Resolver;
    using Smart.Navigation;
    using Smart.Resolver;

    public partial class App
    {
        private readonly Navigator navigator;

        private readonly PopupNavigator popupNavigator;

        public App(IComponentProvider provider)
        {
            InitializeComponent();

            // Config Resolver
            var resolver = CreateResolver(provider);
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

            // Popup
            popupNavigator = new PopupNavigator(new SmartPopupFactory(resolver));
            popupNavigator.AutoRegister(Assembly.GetExecutingAssembly().ExportedTypes);

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
            config.Bind<IPopupNavigator>().ToMethod(kernel => popupNavigator).InSingletonScope();

            config.Bind<IDialogs>().To<Dialogs>().InSingletonScope();
            config.Bind<ILocationManager>().To<LocationManager>().InSingletonScope();
            config.Bind<ISetting>().To<Setting>().InSingletonScope();

            config.Bind<Session>().ToSelf().InSingletonScope();
            config.Bind<ApplicationState>().ToSelf().InSingletonScope();

            config.Bind<ConfigurationService>().ToSelf().InSingletonScope();
            config.Bind<DataService>().ToSelf().InSingletonScope();
            config.Bind<NetworkService>().ToSelf().InSingletonScope();
            config.Bind<TransferService>().ToSelf().InSingletonScope();

            provider.RegisterComponents(config);

            return config.ToResolver();
        }

        protected override async void OnStart()
        {
            if (await Permissions.IsPermissionRequired())
            {
                await Permissions.RequestPermissions();
            }

            await navigator.ForwardAsync(ViewId.Menu);
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
