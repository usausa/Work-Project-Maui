namespace WorkBle;

using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;

using Shiny.BluetoothLE;

using System.Collections.ObjectModel;

public partial class MainPage : ContentPage
{
    private readonly IBluetoothLE ble;
    private readonly IAdapter adapter;

    private readonly IBleManager bleManager;

    private bool isScanning;

    private IDisposable? scanning;

    public ObservableCollection<string> Advertisements { get; } = new();

    public MainPage(IServiceProvider services)
    {
        InitializeComponent();
        BindingContext = this;

        // TODO DI based
        ble = CrossBluetoothLE.Current;
        adapter = CrossBluetoothLE.Current.Adapter;

        bleManager = services.GetRequiredService<IBleManager>();

        adapter.DeviceAdvertised += AdapterOnDeviceAdvertised;

        Update();
    }

    private void Update()
    {
        PluginButton.Text = isScanning ? "Stop" : "Start";
        ShinyButton.Text = scanning is not null ? "Stop" : "Start";
    }

    private async void OnPluginClicked(object? sender, EventArgs e)
    {
        if (!ble.IsOn)
        {
            await DisplayAlert("Bluetooth Off", "Please turn on Bluetooth", "OK");
            return;
        }

        if (!isScanning)
        {
            if (OperatingSystem.IsAndroid())
            {
                var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permission", "Location permission is required", "OK");
                    return;
                }
            }

            Advertisements.Clear();

            await adapter.StartScanningForDevicesAsync();
            isScanning = true;
        }
        else
        {
            await adapter.StopScanningForDevicesAsync();
            isScanning = false;
        }

        Update();
    }

    private void AdapterOnDeviceAdvertised(object? sender, DeviceEventArgs e)
    {
        var ts = DateTime.Now.ToString("HH:mm:ss");
        var dump = $"{ts} | {e.Device.Id} | {e.Device.Name} | {e.Device.Rssi} | {MakeDump(e.Device.AdvertisementRecords)}";

        MainThread.BeginInvokeOnMainThread(() =>
        {
            Advertisements.Insert(0, dump);
            if (Advertisements.Count > 100)
            {
                Advertisements.RemoveAt(Advertisements.Count - 1);
            }
        });
    }

    private string MakeDump(IReadOnlyList<AdvertisementRecord> records)
    {
        foreach (var record in records)
        {
            if (record.Type == AdvertisementRecordType.ManufacturerSpecificData)
            {
                return Convert.ToHexString(record.Data);
            }
        }

        return string.Empty;
    }

    private async void OnShinyClicked(object? sender, EventArgs e)
    {
        if (scanning is null)
        {
            if (OperatingSystem.IsAndroid())
            {
                var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permission", "Location permission is required", "OK");
                    return;
                }
            }

            Advertisements.Clear();

            scanning = bleManager.Scan().Subscribe(OnAdvertisementReceived);
        }
        else
        {
            scanning.Dispose();
            scanning = null;
        }

        Update();
    }

    private void OnAdvertisementReceived(ScanResult result)
    {
        var ts = DateTime.Now.ToString("HH:mm:ss");
        var dump = $"{ts} | {result.Peripheral.Uuid} | {result.Peripheral.Name} | {result.Rssi} | {MakeDump(result.AdvertisementData)}";

        MainThread.BeginInvokeOnMainThread(() =>
        {
            Advertisements.Insert(0, dump);
            if (Advertisements.Count > 100)
            {
                Advertisements.RemoveAt(Advertisements.Count - 1);
            }
        });
    }


    private string MakeDump(IAdvertisementData data)
    {
        if (data.ManufacturerData is not null)
        {
            return $"{data.ManufacturerData.CompanyId:X4} {Convert.ToHexString(data.ManufacturerData.Data)}";
        }

        return string.Empty;
    }
}
