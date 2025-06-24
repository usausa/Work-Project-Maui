namespace Template.MobileApp.Helpers;

public static class ImageHelper
{
    public static MemoryStream ToMemoryStream(Stream stream)
    {
        if (stream is MemoryStream memoryStream)
        {
            return memoryStream;
        }

        var ms = new MemoryStream();
        stream.CopyTo(ms);
        ms.Position = 0;
        return ms;
    }
}
