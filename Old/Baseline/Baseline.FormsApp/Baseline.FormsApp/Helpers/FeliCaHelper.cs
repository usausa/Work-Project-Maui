namespace Baseline.FormsApp.Helpers
{
    using System;

    using Baseline.FormsApp.Components.Nfc;

    using Smart;

    public static class FeliCaHelper
    {
        public static byte[] ExecutePolling(INfcTag tag, short systemCode)
        {
            var command = new byte[6];
            command[0] = (byte)command.Length;
            command[1] = 0x00;
            command[2] = (byte)(systemCode >> 8);
            command[3] = (byte)(systemCode & 0xFF);
            command[4] = 0x01;
            command[5] = 0x00;

            var response = tag.Access(command);
            if (response.Length < 18)
            {
                return Array.Empty<byte>();
            }

            return response.SubArray(2, 8);
        }
    }
}
