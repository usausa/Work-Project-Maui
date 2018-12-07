namespace Business.FormsApp.Components.Wifi
{
    using System;
    using System.Reactive.Subjects;

    public abstract class WifiDirectManagerBase : IWifiDirectManager
    {
        private readonly Subject<string> connected = new Subject<string>();

        private bool enabled;

        public IObservable<string> Connected => connected;

        public bool Enabled
        {
            get => enabled;
            set
            {
                if (value == enabled)
                {
                    return;
                }

                if (value)
                {
                    Start();
                }
                else
                {
                    Stop();
                }

                enabled = value;
            }
        }

        protected abstract void Start();

        protected abstract void Stop();

        protected void RaiseConnected(string text)
        {
            connected.OnNext(text);
        }
    }
}
