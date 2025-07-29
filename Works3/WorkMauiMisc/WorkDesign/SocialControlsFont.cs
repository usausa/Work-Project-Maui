namespace WorkDesign;

using System.Collections.Concurrent;

using SkiaSharp;

public static class SKFontFactory
{
    private static readonly ConcurrentDictionary<string, SKTypeface> _typefaces = new();
    private static readonly ConcurrentDictionary<string, Microsoft.Maui.Graphics.Font> _fonts = new();

    public static void AddFont(IFontCollection fonts, string fontLocation, string alias)
    {
        if (string.IsNullOrEmpty(fontLocation))
            throw new ArgumentNullException(nameof(fontLocation));
        if (string.IsNullOrEmpty(alias))
            throw new ArgumentNullException(nameof(alias));

        // Normalize alias to lowercase
        string lower = alias.ToLowerInvariant();

        try
        {
            // Load SKTypeface from MauiFont file
            SKTypeface typeface;
            using (Stream data = FileSystem.OpenAppPackageFileAsync(fontLocation).GetAwaiter().GetResult())
            {
                if (data == null)
                    throw new FileNotFoundException($"Font file not found: {fontLocation}");
                typeface = SKTypeface.FromStream(data);
                if (typeface == null)
                    throw new InvalidOperationException($"Failed to load typeface from {fontLocation}");
            }

            // Cache typeface by both location and alias
            _typefaces.TryAdd(fontLocation, typeface);
            _typefaces.TryAdd(lower, typeface);

            // Create and cache Microsoft.Maui.Graphics.Font with platform-specific name
            var fontName = GetFontNameForPlatform(fontLocation, alias);
            var font = new Microsoft.Maui.Graphics.Font(fontName);
            _fonts.TryAdd(lower, font);

            // Register with MAUI's font collection
            fonts.AddFont(fontLocation, alias);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading font {fontLocation} with alias {alias}: {ex.Message}");
            throw;
        }
    }

    public static SKTypeface GetTypeface(string fontFamily)
    {
        if (string.IsNullOrEmpty(fontFamily))
            return SKTypeface.Default;

        string lower = fontFamily.ToLowerInvariant();
        return _typefaces.TryGetValue(lower, out var typeface) ? typeface : SKTypeface.Default;
    }

    public static Microsoft.Maui.Graphics.Font GetFont(string fontFamily)
    {
        if (string.IsNullOrEmpty(fontFamily))
            return Microsoft.Maui.Graphics.Font.Default;

        string lower = fontFamily.ToLowerInvariant();

        // Return cached font if available
        if (_fonts.TryGetValue(lower, out var font))
            return font;

        // Log cache miss and use fallback
        Console.WriteLine($"Font cache miss for: {fontFamily}");
        var fallbackFont = new Microsoft.Maui.Graphics.Font(fontFamily);
        _fonts.TryAdd(lower, fallbackFont);
        return fallbackFont;
    }

    public static void Dispose()
    {
        foreach (var typeface in _typefaces.Values.Distinct())
        {
            typeface?.Dispose();
        }
        _typefaces.Clear();
        _fonts.Clear();
    }

    private static string GetFontNameForPlatform(string fontLocation, string alias)
    {
#if WINDOWS
        return fontLocation.ToUpper() + "#" + alias;
#else
        return alias;
#endif
    }
}