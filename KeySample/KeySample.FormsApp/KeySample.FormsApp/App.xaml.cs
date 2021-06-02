namespace KeySample.FormsApp
{
    using System.Reflection;

    using KeySample.FormsApp.Modules;
    using KeySample.FormsApp.Shell;
    using KeySample.FormsApp.State;

    using Smart.Forms.Resolver;
    using Smart.Navigation;
    using Smart.Resolver;

    using XamarinFormsComponents;
    using XamarinFormsComponents.Popup;

    public partial class App
    {
        private readonly Navigator navigator;

        public App(IComponentProvider provider)
        {
            InitializeComponent();

            // Config Resolver
            var resolver = CreateResolver(provider);
            ResolveProvider.Default.UseSmartResolver(resolver);

            // Config Navigator
            navigator = new NavigatorConfig()
                .UseFormsNavigationProvider()
                .AddPlugin<FocusRestorePlugin>()
                .UseResolver(resolver)
                .UseIdViewMapper(m => m.AutoRegister(Assembly.GetExecutingAssembly().ExportedTypes))
                .ToNavigator();
            navigator.Navigated += (_, args) =>
            {
                // for debug
                System.Diagnostics.Debug.WriteLine(
                    $"Navigated: [{args.Context.FromId}]->[{args.Context.ToId}] : stacked=[{navigator.StackedCount}]");
            };

            // Popup Navigator
            var popupNavigator = resolver.Get<IPopupNavigator>();
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
                .UsePropertyInjector()
                .UsePageContextScope();

            config.UseXamarinFormsComponents(adapter =>
            {
                adapter.AddDialogs();
                adapter.AddPopupNavigator();
                adapter.AddJsonSerializer();
                adapter.AddSettings();

                // Custom
                adapter.AddComponent<IPopupPageFactory, FocusControlPopupPageFactory>();
            });

            config.BindSingleton<INavigator>(_ => navigator);

            config.BindSingleton<ApplicationState>();

            config.BindSingleton<Configuration>();
            config.BindSingleton<Session>();

            provider.RegisterComponents(config);

            return config.ToResolver();
        }

        protected override async void OnStart()
        {
            // Navigate
            await navigator.ForwardAsync(ViewId.Menu);
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
