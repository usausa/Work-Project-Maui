namespace Baseline.FormsApp.Droid
{
    using System;

    using Acr.UserDialogs;

    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.Graphics;
    using Android.OS;
    using Android.Runtime;
    using Android.Views;

    using Baseline.FormsApp.Components.Barcode;
    using Baseline.FormsApp.Components.Device;
    using Baseline.FormsApp.Components.Nfc;
    using Baseline.FormsApp.Droid.Components.Barcode;
    using Baseline.FormsApp.Droid.Components.Device;
    using Baseline.FormsApp.Droid.Components.Nfc;

    using Smart.Resolver;

    using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

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

        private NfcReader nfcReader;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            deviceManager = new DeviceManager(this);
            Window.DecorView.ViewTreeObserver.GlobalLayout += ViewTreeObserverOnGlobalLayout;
            deviceManager.RegisterWakeLockReceiver();

            nfcReader = new NfcReader(this);

            ZXing.Mobile.MobileBarcodeScanner.Initialize(Application);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();

            UserDialogs.Init(this);
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App(new ComponentProvider(this)));

            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>()
                .UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        }

        protected override void OnResume()
        {
            base.OnResume();

            nfcReader.Resume();
        }

        protected override void OnPause()
        {
            nfcReader.Pause();

            base.OnResume();
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            nfcReader.OnNewIntent(intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            //Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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
            Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed);
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

                config.Bind<IDeviceManager>().ToConstant(activity.deviceManager);
                config.Bind<INfcReader>().ToConstant(activity.nfcReader).InSingletonScope();
                config.Bind<IBarcodeReader>().To<BarcodeReader>().InSingletonScope();
            }
        }
    }
}