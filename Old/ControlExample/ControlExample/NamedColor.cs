namespace ControlExample;

using System.Reflection;
using System.Text;

internal class NamedColor
{
    public string Name { private init; get; } = default!;

    public string FriendlyName { private init; get; } = default!;

    public Color Color { private init; get; } = default!;

    public string RgbDisplay { private init; get; } = default!;

    public static IList<NamedColor> All { get; }

    static NamedColor()
    {
        var all = new List<NamedColor>();
        var stringBuilder = new StringBuilder();

        foreach (var fieldInfo in typeof(Colors).GetRuntimeFields())
        {
            if (fieldInfo.IsPublic && fieldInfo.IsStatic && (fieldInfo.FieldType == typeof(Color)))
            {
                var name = fieldInfo.Name;
                stringBuilder.Clear();
                var index = 0;

                foreach (var ch in name)
                {
                    if (index != 0 && Char.IsUpper(ch))
                    {
                        stringBuilder.Append(' ');
                    }
                    stringBuilder.Append(ch);
                    index++;
                }

                // Instantiate a NamedColor object.
                var color = (Color)fieldInfo.GetValue(null)!;

                var namedColor = new NamedColor
                {
                    Name = name,
                    FriendlyName = stringBuilder.ToString(),
                    Color = color,
                    RgbDisplay = $"{(int)(255 * color.Red):X2}-{(int)(255 * color.Green):X2}-{(int)(255 * color.Blue):X2}"
                };

                all.Add(namedColor);
            }
        }

        all.TrimExcess();
        All = all;
    }
}
