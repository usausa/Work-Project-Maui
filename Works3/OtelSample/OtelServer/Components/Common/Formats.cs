namespace OtelServer.Components.Common;

using System.Globalization;

using MudBlazor;

using OtelServer.Telemetry.Models;

// 画面の表示書式
internal static class Formats
{
    public static string Time(DateTimeOffset value) => value.ToLocalTime().ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);

    public static string DateTime(DateTimeOffset value) => value.ToLocalTime().ToString("MM/dd HH:mm:ss", CultureInfo.InvariantCulture);

    public static string Duration(TimeSpan value)
    {
        if (value.TotalSeconds >= 1)
        {
            return value.TotalSeconds.ToString("0.00", CultureInfo.InvariantCulture) + " s";
        }

        if (value.TotalMilliseconds >= 1)
        {
            return value.TotalMilliseconds.ToString("0.0", CultureInfo.InvariantCulture) + " ms";
        }

        return (value.TotalMilliseconds * 1000).ToString("0", CultureInfo.InvariantCulture) + " us";
    }

    public static string Number(double value)
    {
        var abs = Math.Abs(value);
        if (abs >= 1_000_000_000)
        {
            return (value / 1_000_000_000d).ToString("0.##", CultureInfo.InvariantCulture) + "G";
        }

        if (abs >= 1_000_000)
        {
            return (value / 1_000_000d).ToString("0.##", CultureInfo.InvariantCulture) + "M";
        }

        if (abs >= 10_000)
        {
            return (value / 1_000d).ToString("0.##", CultureInfo.InvariantCulture) + "k";
        }

        return value.ToString(abs >= 1 ? "0.##" : "0.###", CultureInfo.InvariantCulture);
    }

    public static string Value(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);

    public static string ShortId(string? id) => String.IsNullOrEmpty(id) ? "-" : id[..Math.Min(8, id.Length)];

    public static string Attributes(IReadOnlyList<KeyValueAttr> attributes, int take = 3)
    {
        if (attributes.Count == 0)
        {
            return "-";
        }

        var text = String.Join(", ", attributes.Take(take).Select(static x => $"{x.Key}={Truncate(x.Value, 40)}"));
        return attributes.Count > take ? $"{text} (+{attributes.Count - take})" : text;
    }

    public static string Truncate(string? value, int max)
    {
        if (String.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var line = value.IndexOf('\n', StringComparison.Ordinal) is var index && index >= 0 ? value[..index] : value;
        return line.Length <= max ? line : line[..(max - 1)] + "…";
    }

    // OTLP の SeverityNumber (1-4 TRACE / 5-8 DEBUG / 9-12 INFO / 13-16 WARN / 17-20 ERROR / 21-24 FATAL)
    public static Color SeverityColor(int severity) => severity switch
    {
        >= 21 => Color.Error,
        >= 17 => Color.Error,
        >= 13 => Color.Warning,
        >= 9 => Color.Info,
        _ => Color.Default
    };

    public static Color StatusColor(string status) => status switch
    {
        "ERROR" => Color.Error,
        "OK" => Color.Success,
        _ => Color.Default
    };
}
