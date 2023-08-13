namespace Template.MobileApp.Providers;

using Shiny.BluetoothLE.Hosting;
using Shiny.BluetoothLE.Hosting.Managed;

[BleGattCharacteristic(BleId.UserService, BleId.UserCharacteristic)]
public sealed class UserCharacteristic : BleGattCharacteristic
{
    // TODO
    private readonly Guid guid = Guid.NewGuid();

    public override Task<GattResult> OnRead(ReadRequest request)
    {
        return Task.FromResult(new GattResult(GattState.Success, guid.ToByteArray()));
    }
}
