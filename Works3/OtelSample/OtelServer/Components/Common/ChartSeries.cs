namespace OtelServer.Components.Common;

public sealed record ChartPoint(DateTimeOffset Time, double Value);

public sealed record ChartSeries(string Label, IReadOnlyList<ChartPoint> Points);
