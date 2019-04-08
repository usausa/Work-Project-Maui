namespace Baseline.FormsApp.Components.Settings
{
    using System;

    public interface ISetting
    {
        void Remove(string key);

        bool ReadBool(string key, bool defaultValue = default);

        int ReadInteger(string key, int defaultValue = default);

        long ReadLong(string key, long defaultValue = default);

        double ReadDouble(string key, long defaultValue = default);

        string ReadString(string key, string defaultValue = default);

        DateTime ReadDateTime(string key, DateTime defaultValue = default);

        T ReadObject<T>(string key, T defaultValue = default);

        void WriteBool(string key, bool value);

        void WriteInteger(string key, int value);

        void WriteLong(string key, long value);

        void WriteDouble(string key, double value);

        void WriteString(string key, string value);

        void WriteDateTime(string key, DateTime value);

        void WriteObject<T>(string key, T value);
    }
}
