namespace Business.FormsApp.Components.Wifi
{
    using System;

    public interface IWifiDirectManager
    {
        IObservable<string> Connected { get; }
    }
}
