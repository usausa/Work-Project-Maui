namespace WorkSample.Pages.Controls;
using Syncfusion.Maui.Toolkit.Charts;

public class LegendExt : ChartLegend
{
    protected override double GetMaximumSizeCoefficient()
    {
        return 0.5;
    }
}
