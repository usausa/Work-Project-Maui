#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace PushClient.Services;

using Android.App;
using Android.Content;

public sealed class PushHost : IPushHost
{
    public void StartService(string address)
    {
        var context = Application.Context;
        using var intent = new Intent(context, typeof(PushService));
        intent.PutExtra(PushService.ExtraAddress, address);
        context.StartForegroundService(intent);
    }

    public void StopService()
    {
        var context = Application.Context;
        using var intent = new Intent(context, typeof(PushService));
        context.StopService(intent);
    }

    public void OpenBatterySettings()
    {
        using var intent = new Intent(Android.Provider.Settings.ActionIgnoreBatteryOptimizationSettings);
        intent.AddFlags(ActivityFlags.NewTask);
        Application.Context.StartActivity(intent);
    }
}
