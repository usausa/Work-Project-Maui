namespace Business.FormsApp.Droid
{
    using Acr.UserDialogs;

    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.OS;

    using Business.FormsApp.Components.Wifi;
    using Business.FormsApp.Droid.Components.Wifi;
    using Smart.Resolver;

    [Activity(
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            UserDialogs.Init(this);

            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App(new ComponentProvider(Application)));
        }

        public override void OnBackPressed()
        {
            MoveTaskToBack(true);
        }

        private class ComponentProvider : IComponentProvider
        {
            private readonly Context context;

            public ComponentProvider(Context context)
            {
                this.context = context;
            }

            public void RegisterComponents(ResolverConfig config)
            {
                config.Bind<IWifiDirectManager>().ToConstant(new WifiDirectManager(context));
            }
        }
    }
}