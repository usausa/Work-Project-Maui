namespace Template.MobileApp.Usecase;

using Accord.MachineLearning;

using SkiaSharp;

public record ColorCount(
    byte R,
    byte G,
    byte B,
    int Count);

public sealed class SampleUsecase
{
    //--------------------------------------------------------------------------------
    // Image
    //--------------------------------------------------------------------------------

#pragma warning disable CA1822
    // ReSharper disable once MemberCanBeMadeStatic.Global
    public List<ColorCount> ClusterColors(
        SKBitmap bitmap,
        int maxClusters,
        int maxIterations,
        double tolerance)
    {
        var width = bitmap.Width;
        var height = bitmap.Height;

        var observations = new double[width * height][];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var c = bitmap.GetPixel(x, y);
                observations[(y * width) + x] = [c.Red, c.Green, c.Blue];
            }
        }

        var actualClusters = Math.Min(maxClusters, observations.Length);

        // KMeans
        var algorithm = new KMeans(actualClusters)
        {
            MaxIterations = maxIterations,
            Tolerance = tolerance
        };
        var clusters = algorithm.Learn(observations);
        var labels = clusters.Decide(observations);

        // Count by cluster
        var counts = new int[actualClusters];
        foreach (var label in labels)
        {
            counts[label]++;
        }

        var result = new List<ColorCount>(actualClusters);
        for (var i = 0; i < actualClusters; i++)
        {
            var centroid = clusters.Centroids[i];
            var r = (byte)Math.Clamp((int)Math.Round(centroid[0]), 0, 255);
            var g = (byte)Math.Clamp((int)Math.Round(centroid[1]), 0, 255);
            var b = (byte)Math.Clamp((int)Math.Round(centroid[2]), 0, 255);
            result.Add(new ColorCount(r, g, b, counts[i]));
        }

        result.Sort(static (x, y) => y.Count - x.Count);
        return result;
    }
#pragma warning restore CA1822
}
