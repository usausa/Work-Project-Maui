namespace Business.FormsApp.Droid
{
    using System;

    using Acr.UserDialogs;

    using Android.App;
    using Android.Content.PM;
    using Android.Graphics;
    using Android.OS;
    using Android.Views;

    using Business.FormsApp.Components.Device;
    using Business.FormsApp.Components.Log;
    using Business.FormsApp.Components.Wifi;
    using Business.FormsApp.Droid.Components.Device;
    using Business.FormsApp.Droid.Components.Log;
    using Business.FormsApp.Droid.Components.Wifi;

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
        private readonly Handler handler = new Handler();

        private DeviceManager deviceManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            deviceManager = new DeviceManager(this);
            Window.DecorView.ViewTreeObserver.GlobalLayout += ViewTreeObserverOnGlobalLayout;
            deviceManager.RegisterWakeLockReceiver();

            NLog.LogManager.ThrowExceptions = true;
            NLog.LogManager.ThrowConfigExceptions = true;
            NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration("assets/NLog.config");

            UserDialogs.Init(this);
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App(new ComponentProvider(this)));
        }

        private void ViewTreeObserverOnGlobalLayout(object sender, EventArgs e)
        {
            var contentView = Window.DecorView;

            var rect = new Rect();
            contentView.GetWindowVisibleDisplayFrame(rect);

            var screenHeight = contentView.RootView.Height;
            var keypadHeight = screenHeight - rect.Bottom;
            var visible = keypadHeight > screenHeight * 0.15;

            handler.Post(() => deviceManager.UpdateKeyboardState(visible));
        }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                System.Diagnostics.Debug.WriteLine("Android back button: There are some pages in the PopupStack");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Android back button: There are not any pages in the PopupStack");

                MoveTaskToBack(true);
            }
        }

        private class ComponentProvider : IComponentProvider
        {
            private readonly MainActivity activity;

            public ComponentProvider(MainActivity activity)
            {
                this.activity = activity;
            }

            public void RegisterComponents(ResolverConfig config)
            {
                config.Bind<ILogger>().To<Logger>().InSingletonScope();
                config.Bind<IDeviceManager>().ToConstant(activity.deviceManager);
                config.Bind<IWifiDirectManager>().ToConstant(new WifiDirectManager(activity));
            }
        }
    }
}