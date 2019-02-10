namespace PayClient.FormsApp.Droid
{
    using Acr.UserDialogs;

    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.OS;
    using Android.Views;

    using PayClient.FormsApp.Components.Barcode;
    using PayClient.FormsApp.Droid.Components.Barcode;

    using Smart.Resolver;

    [Activity(
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait,
        WindowSoftInputMode = SoftInput.AdjustResize)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            ZXing.Mobile.MobileBarcodeScanner.Initialize(Application);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();

            UserDialogs.Init(this);

            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);

            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App(new ComponentProvider(this)));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private sealed class ComponentProvider : IComponentProvider
        {
            private readonly MainActivity activity;

            public ComponentProvider(MainActivity activity)
            {
                this.activity = activity;
            }

            public void RegisterComponents(ResolverConfig config)
            {
                config.Bind<Context>().ToConstant(activity).InSingletonScope();

                config.Bind<IBarcodeReader>().To<BarcodeReader>().InSingletonScope();
            }
        }
    }
}