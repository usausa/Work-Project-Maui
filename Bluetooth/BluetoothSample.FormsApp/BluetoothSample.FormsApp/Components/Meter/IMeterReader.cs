namespace BluetoothSample.FormsApp.Components.Meter
{
    using System.Threading.Tasks;

    public interface IMeterReader
    {
        ValueTask<bool> Discover();
    }
}
