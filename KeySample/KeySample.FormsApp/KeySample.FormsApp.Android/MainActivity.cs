namespace KeySample.FormsApp.Droid
{
    using System.Collections.Generic;

    using Acr.UserDialogs;

    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.OS;
    using Android.Runtime;
    using Android.Views;

    using KeySample.FormsApp.Components.Dialog;
    using KeySample.FormsApp.Droid.Components.Dialog;

    using Smart.Forms.Resolver;
    using Smart.Resolver;

    [Activity(
        Name = "keysample.app.MainActivity",
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme.Splash",
        MainLauncher = true,
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize,
        ScreenOrientation = ScreenOrientation.Portrait,
        WindowSoftInputMode = SoftInput.AdjustResize)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.MainTheme);
            base.OnCreate(savedInstanceState);

            // Components
            UserDialogs.Init(this);
            Rg.Plugins.Popup.Popup.Init(this);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            // Boot
            LoadApplication(new App(new ComponentProvider(this)));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed()
        {
            Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed);
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            // Disable sound
            //if ((e.KeyCode == Keycode.VolumeDown) || (e.KeyCode == Keycode.VolumeUp))
            //{
            //    return true;
            //}

            System.Diagnostics.Debug.WriteLine($"*DispatchKeyEvent : KeyCode=[{e.KeyCode}]");
            return base.DispatchKeyEvent(e);
        }

        public override bool DispatchKeyShortcutEvent(KeyEvent e)
        {
            System.Diagnostics.Debug.WriteLine($"*DispatchKeyShortcutEvent : KeyCode=[{e.KeyCode}]");
            return base.DispatchKeyShortcutEvent(e);
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            System.Diagnostics.Debug.WriteLine($"*OnKeyDown : KeyCode=[{keyCode}], KeyCode=[{e.KeyCode}]");
            return base.OnKeyDown(keyCode, e);
        }

        public override bool OnKeyUp(Keycode keyCode, KeyEvent e)
        {
            System.Diagnostics.Debug.WriteLine($"*OnKeyUp : KeyCode=[{keyCode}], KeyCode=[{e.KeyCode}]");
            return base.OnKeyUp(keyCode, e);
        }

        public override bool OnKeyLongPress(Keycode keyCode, KeyEvent e)
        {
            System.Diagnostics.Debug.WriteLine($"*OnKeyLongPress : KeyCode=[{keyCode}], KeyCode=[{e.KeyCode}]");
            return base.OnKeyLongPress(keyCode, e);
        }

        public override bool OnKeyMultiple(Keycode keyCode, int repeatCount, KeyEvent e)
        {
            System.Diagnostics.Debug.WriteLine($"*OnKeyMultiple : KeyCode=[{keyCode}], repeat=[{repeatCount}] KeyCode=[{e.KeyCode}]");
            return base.OnKeyMultiple(keyCode, repeatCount, e);
        }

        public override bool OnKeyShortcut(Keycode keyCode, KeyEvent e)
        {
            System.Diagnostics.Debug.WriteLine($"*OnKeyShortcut : KeyCode=[{keyCode}], KeyCode=[{e.KeyCode}]");
            return base.OnKeyShortcut(keyCode, e);
        }

        public override bool SuperDispatchKeyEvent(KeyEvent e)
        {
            System.Diagnostics.Debug.WriteLine($"*SuperDispatchKeyEvent : KeyCode=[{e.KeyCode}]");
            return base.SuperDispatchKeyEvent(e);
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

                config.Bind<IApplicationDialog>().To<ApplicationDialog>().InSingletonScope();
            }
        }
    }
}
