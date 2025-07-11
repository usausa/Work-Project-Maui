using System.Diagnostics;

namespace WorkWifi;

using Android.Bluetooth;
using Android.Content;
using Android.Net.Wifi;

using MauiComponents;

public partial class MainPage : ContentPage
{
    private readonly AccessPointScanner accessPointScanner = new();

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnStartClick(object? sender, EventArgs e)
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                return;
            }
        }

        Debug.WriteLine($"** {accessPointScanner.Test()}");
    }

    private void OnStopClick(object? sender, EventArgs e)
    {
    }
}

public sealed class AccessPointScanner
{
    private readonly WifiManager wifiManager;


    public AccessPointScanner()
    {
        var context = ActivityResolver.CurrentActivity.ApplicationContext!;
        wifiManager = (WifiManager)context.GetSystemService(Context.WifiService)!;
    }

    public bool Test()
    {
        return wifiManager.StartScan();
    }
}