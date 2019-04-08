namespace Baseline.FormsApp.Components.Settings
{
    using System;

    using Newtonsoft.Json;

    using Xamarin.Essentials;

    public class Setting : ISetting
    {
        private readonly JsonSerializerSettings settings = new JsonSerializerSettings();

        public void Remove(string key) => Preferences.Remove(key);

        public bool ReadBool(string key, bool defaultValue = default) => Preferences.Get(key, defaultValue);

        public int ReadInteger(string key, int defaultValue = default) => Preferences.Get(key, defaultValue);

        public long ReadLong(string key, long defaultValue = default) => Preferences.Get(key, defaultValue);

        public double ReadDouble(string key, long defaultValue = default) => Preferences.Get(key, defaultValue);

        public string ReadString(string key, string defaultValue = default) => Preferences.Get(key, defaultValue);

        public DateTime ReadDateTime(string key, DateTime defaultValue = default) => Preferences.Get(key, defaultValue);

        public T ReadObject<T>(string key, T defaultValue = default)
        {
            var json = Preferences.Get(key, string.Empty);
            try
            {
                return JsonConvert.DeserializeObject<T>(json, settings);
            }
            catch (JsonException)
            {
                return defaultValue;
            }
        }

        public void WriteBool(string key, bool value) => Preferences.Set(key, value);

        public void WriteInteger(string key, int value) => Preferences.Set(key, value);

        public void WriteLong(string key, long value) => Preferences.Set(key, value);

        public void WriteDouble(string key, double value) => Preferences.Set(key, value);

        public void WriteString(string key, string value) => Preferences.Set(key, value);

        public void WriteDateTime(string key, DateTime value) => Preferences.Set(key, value);

        public void WriteObject<T>(string key, T value)
        {
            var json = JsonConvert.SerializeObject(value, Formatting.None, settings);
            Preferences.Set(key, json);
        }
    }
}
