using Smart.Maui.ViewModels;
using Smart.Mvvm;

using System.Collections.ObjectModel;
using System.Globalization;

namespace WorkDesign;

public partial class ShadowPage : ContentPage
{
	public ShadowPage()
	{
		InitializeComponent();
	}
}

public sealed partial class ShadowPageViewModel : ExtendViewModelBase
{
    public ObservableCollection<ColorItem> BorderColors { get; }

    public ObservableCollection<ColorItem> ShadowColors { get; }

    [ObservableProperty]
    public partial ColorItem BorderColor { get; set; }

    [ObservableProperty]
    public partial ColorItem ShadowColor { get; set; }

    [ObservableProperty(NotifyAlso = [nameof(ShadowOffset)])]
    public partial double ShadowOffsetX { get; set; } = 4;

    [ObservableProperty(NotifyAlso = [nameof(ShadowOffset)])]
    public partial double ShadowOffsetY { get; set; } = 4;

    public Point ShadowOffset => new(ShadowOffsetX, ShadowOffsetY);

    [ObservableProperty]
	public partial double ShadowRadius { get; set; } = 10;

    [ObservableProperty]
    public partial double ShadowOpacity { get; set; } = 0.5;

    public ShadowPageViewModel()
    {
        BorderColors = new(ResourceHelper.EnumColors().Where(x => x.Key.EndsWith("Default")).Select(x => new ColorItem(x.Key, x.Color)));
        ShadowColors = new(ResourceHelper.EnumColors().Where(x => x.Key.StartsWith("Gray")).Select(x => new ColorItem(x.Key, x.Color)));

        BorderColor = BorderColors[0];
        ShadowColor = ShadowColors[0];
    }
}

public static class ResourceHelper
{
    public static IEnumerable<(string Key, Color Color)> EnumColors()
    {
        if (Application.Current?.Resources is { } resources)
        {
            foreach (var key in resources.Keys)
            {
                var value = resources[key];
                if (value is Color color)
                {
                    yield return (key, color);
                }
            }

            if (resources.MergedDictionaries is not null)
            {
                foreach (var dictionary in resources.MergedDictionaries)
                {
                    foreach (var key in dictionary.Keys)
                    {
                        var value = dictionary[key];
                        if (value is Color color)
                        {
                            yield return (key, color);
                        }
                    }
                }
            }
        }
    }
}

public record ColorItem(string Key, Color Color);

public class ColorToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Color color)
        {
            // ColorからSolidColorBrushを作成して返します。
            return new SolidColorBrush(color);
        }

        // 変換できない場合はnullやFallback値などを返す
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // 逆変換は必要に応じて実装します。BrushからColorへの変換
        if (value is SolidColorBrush brush)
        {
            return brush.Color;
        }

        // SolidColorBrushでない場合は実装に応じて例外を投げるかnullを返す
        return null; // または throw new NotImplementedException();
    }
}