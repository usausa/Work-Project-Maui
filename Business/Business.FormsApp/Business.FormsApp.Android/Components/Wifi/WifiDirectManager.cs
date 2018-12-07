namespace Business.FormsApp.Droid.Components.Wifi
{
    using Android.Content;

    using Business.FormsApp.Components.Wifi;

    public sealed class WifiDirectManager : WifiDirectManagerBase
    {
        // TODO field
        public Context Context { get; }

        public WifiDirectManager(Context context)
        {
            Context = context;
        }

        protected override void Start()
        {
            // TODO
        }

        protected override void Stop()
        {
            // TODO
        }
    }
}