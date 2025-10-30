namespace Template.MobileApp.Converters;

using Template.MobileApp.Shell;

public sealed class AlertLevelToColorConverter : IValueConverter
{
    public Color SafeColor { get; set; } = Colors.Green;

    public Color WarningColor { get; set; } = Colors.Orange;

    public Color CriticalColor { get; set; } = Colors.Red;

    public Color UnknownColor { get; set; } = Colors.Gray;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is AlertLevel level)
        {
            return level switch
            {
                AlertLevel.Safe => SafeColor,
                AlertLevel.Warning => WarningColor,
                AlertLevel.Critical => CriticalColor,
                _ => UnknownColor
            };
        }

        return UnknownColor;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
