namespace ViewSample.FormsApp.Helpers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using SkiaSharp;

    public static class ImageHelper
    {
        private const double Rotate90 = 90d;
        private const double Rotate180 = 180d;
        private const double Rotate270 = 270d;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator", Justification = "Ignore")]
        public static async ValueTask<byte[]> NormalizeImageAsync(byte[] data, int maxSize, double rotation, int quality)
        {
            return await Task.Run(() =>
            {
                var source = SKBitmap.Decode(data);

                var factor = Math.Max((double)source.Width / maxSize, (double)source.Height / maxSize);
                var newWidth = (int)(source.Width / factor);
                var newHeight = (int)(source.Height / factor);

                var destination = source.Resize(new SKSizeI(newWidth, newHeight), SKFilterQuality.Medium);
                if (rotation == Rotate90)
                {
                    var rotated = new SKBitmap(destination.Height, destination.Width);
                    using var surface = new SKCanvas(rotated);
                    surface.Translate(rotated.Width, 0);
                    surface.RotateDegrees(90f);
                    surface.DrawBitmap(destination, 0, 0);

                    destination = rotated;
                }
                else if (rotation == Rotate180)
                {
                    using var surface = new SKCanvas(destination);
                    surface.RotateDegrees(180f, (float)destination.Width / 2, (float)destination.Height / 2);
                    surface.DrawBitmap(destination.Copy(), 0, 0);
                }
                else if (rotation == Rotate270)
                {
                    var rotated = new SKBitmap(destination.Height, destination.Width);
                    using var surface = new SKCanvas(rotated);
                    surface.Translate(0, rotated.Height);
                    surface.RotateDegrees(270f);
                    surface.DrawBitmap(destination, 0, 0);

                    destination = rotated;
                }

                using var ms = new MemoryStream();
                destination.Encode(ms, SKEncodedImageFormat.Jpeg, quality);
                return ms.ToArray();
            });
        }
    }
}
