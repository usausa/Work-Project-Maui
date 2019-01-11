[assembly: Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]

namespace Business.FormsApp
{
    using System.Reflection;

    using Business.FormsApp.Components.Dialogs;
    using Business.FormsApp.Components.Popup;
    using Business.FormsApp.Modules;

    using Smart.Forms.Components;
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
            popupNavigator = new PopupNavigator(resolver);
            popupNavigator.AutoRegister(Assembly.GetExecutingAssembly().ExportedTypes);

            // Show MainWindow
            MainPage = resolver.Get<MainPage>();

            navigator.Forward(ViewId.Menu);
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
            config.Bind<IDialogService>().To<DialogService>().InSingletonScope();
            config.Bind<ApplicationState>().ToSelf().InSingletonScope();
            config.Bind<IDialog>().To<Dialog>().InSingletonScope();

            provider.RegisterComponents(config);

            return config.ToResolver();
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
