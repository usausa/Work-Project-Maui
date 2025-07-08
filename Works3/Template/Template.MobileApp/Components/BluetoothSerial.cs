namespace Template.MobileApp.Components;

public interface IBluetoothSerial : IDisposable
{
    ValueTask WriteAsync(byte[] buffer, int offset, int count);

    ValueTask<int> ReadAsync(byte[] buffer, int offset, int count);
}

public interface IBluetoothSerialFactory
{
    ValueTask<IBluetoothSerial?> ConnectAsync(string name, string? pin = null);
}

public sealed partial class BluetoothSerialFactory : IBluetoothSerialFactory
{
    public partial ValueTask<IBluetoothSerial?> ConnectAsync(string name, string? pin = null);
}
