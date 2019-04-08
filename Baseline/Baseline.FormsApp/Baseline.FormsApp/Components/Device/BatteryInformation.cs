namespace Baseline.FormsApp.Components.Device
{
    public class BatteryInformation
    {
        public BatteryStatus Status { get; }

        public int Level { get; }

        public BatteryInformation(BatteryStatus status, int level)
        {
            Status = status;
            Level = level;
        }
    }
}
