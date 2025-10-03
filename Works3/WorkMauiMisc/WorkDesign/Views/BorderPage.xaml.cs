using Smart.Maui.Expressions;
using Smart.Maui.ViewModels;
using Smart.Mvvm;

using System.Collections.ObjectModel;
using System.Globalization;

namespace WorkDesign;

public partial class BorderPage : ContentPage
{
	public BorderPage()
	{
		InitializeComponent();
	}
}

public enum BorderShape
{
	RoundRectangle,
	Ellipse,
    Rectangle,
    Polygon
}

public sealed partial class BorderPageViewModel : ExtendViewModelBase
{
    public ObservableCollection<BorderShape> BorderShapes { get; } = [BorderShape.RoundRectangle, BorderShape.Ellipse, BorderShape.Rectangle, BorderShape.Polygon];

    public ObservableCollection<ColorItem> BorderColors { get; }

    public ObservableCollection<ColorItem> StrokeColors { get; }

    public ObservableCollection<LineJoin> LineJoins { get; } = [LineJoin.Miter, LineJoin.Bevel, LineJoin.Round];

    public ObservableCollection<LineCap> LineCaps { get; } = [LineCap.Butt, LineCap.Round, LineCap.Square];

    [ObservableProperty]
    public partial BorderShape BorderShape { get; set; } = BorderShape.RoundRectangle;

    [ObservableProperty]
	public partial ColorItem BorderColor { get; set; }

    [ObservableProperty]
    public partial ColorItem StrokeColor { get; set; }

    [ObservableProperty]
    public partial double StrokeThickness { get; set; } = 1;

    [ObservableProperty]
    public partial double StrokeDashLength1 { get; set; } = 0;

    [ObservableProperty]
    public partial double StrokeDashLength2 { get; set; } = 0;

    public DoubleCollection StrokeDashArray { get; } = new();

    [ObservableProperty]
    public partial double StrokeDashOffset { get; set; } = 0;

    [ObservableProperty]
    public partial LineJoin StrokeLineJoin { get; set; }= LineJoin.Miter;

    [ObservableProperty]
    public partial LineCap StrokeLineCap { get; set; } = LineCap.Butt;

    [ObservableProperty(NotifyAlso = [nameof(CornerRadius)])]
    public partial double CornerRadiusTopLeft { get; set; } = 20;

    [ObservableProperty(NotifyAlso = [nameof(CornerRadius)])]
    public partial double CornerRadiusTopRight { get; set; } = 20;

    [ObservableProperty(NotifyAlso = [nameof(CornerRadius)])]
    public partial double CornerRadiusBottomLeft { get; set; } = 20;

    [ObservableProperty(NotifyAlso = [nameof(CornerRadius)])]
    public partial double CornerRadiusBottomRight { get; set; } = 20;

    public CornerRadius CornerRadius => new(CornerRadiusTopLeft, CornerRadiusTopRight, CornerRadiusBottomLeft, CornerRadiusBottomRight);

    public BorderPageViewModel()
	{
		BorderColors = new(ResourceHelper.EnumColors().Where(x => x.Key.EndsWith("Default")).Select(x => new ColorItem(x.Key, x.Color)));
        StrokeColors = new(ResourceHelper.EnumColors().Where(x => x.Key.EndsWith("Accent1") || x.Key.EndsWith("Accent4")).Select(x => new ColorItem(x.Key, x.Color)));

        BorderColor = BorderColors[0];
        StrokeColor = StrokeColors[0];

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(StrokeDashLength1) or nameof(StrokeDashLength2))
            {
                StrokeDashArray.Clear();
                if (StrokeDashLength1 > 0)
                {
                    StrokeDashArray.Add(StrokeDashLength1);
                    if (StrokeDashLength2 > 0)
                    {
                        StrokeDashArray.Add(StrokeDashLength2);
                    }
                }
            }
        };
    }
}

public sealed class EqualsConverter : IValueConverter
{
    public IBinaryExpression Expression { get; set; } = default!;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Equals(value, parameter);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
