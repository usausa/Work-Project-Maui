namespace WorkVisualMusic;

public sealed class SoundEqualizer : GraphicsView
{
    public static readonly BindableProperty LevelProperty =
        BindableProperty.Create(nameof(Level), typeof(int), typeof(SoundEqualizer), 10, propertyChanged: RedrawControl);

    public static readonly BindableProperty RangeProperty =
        BindableProperty.Create(nameof(Range), typeof(int), typeof(SoundEqualizer), 10, propertyChanged: RedrawControl);

    public static readonly BindableProperty ValuesProperty =
        BindableProperty.Create(nameof(Values), typeof(int[]), typeof(SoundEqualizer), Array.Empty<int>(), propertyChanged: RedrawControl);

    public static readonly BindableProperty StartColorProperty =
        BindableProperty.Create(nameof(StartColor), typeof(Color), typeof(SoundEqualizer), Colors.DarkGreen, propertyChanged: RedrawControl);

    public static readonly BindableProperty EndColorProperty =
        BindableProperty.Create(nameof(EndColor), typeof(Color), typeof(SoundEqualizer), Colors.LightGreen, propertyChanged: RedrawControl);

    public static readonly BindableProperty InactiveColorProperty =
        BindableProperty.Create(nameof(InactiveColor), typeof(Color), typeof(SoundEqualizer), Colors.Gray, propertyChanged: RedrawControl);

    public static readonly BindableProperty HorizontalSpacingProperty =
        BindableProperty.Create(nameof(HorizontalSpacing), typeof(float), typeof(SoundEqualizer), 2f, propertyChanged: RedrawControl);

    public static readonly BindableProperty VerticalSpacingProperty =
        BindableProperty.Create(nameof(VerticalSpacing), typeof(float), typeof(SoundEqualizer), 2f, propertyChanged: RedrawControl);

    private static void RedrawControl(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SoundEqualizer control)
        {
            control.Invalidate();
        }
    }

    public SoundEqualizer()
    {
        Drawable = new EqualizerDrawable(this);
    }

    public int Level
    {
        get => (int)GetValue(LevelProperty);
        set => SetValue(LevelProperty, value);
    }

    public int Range
    {
        get => (int)GetValue(RangeProperty);
        set => SetValue(RangeProperty, value);
    }

    public int[] Values
    {
        get => (int[])GetValue(ValuesProperty);
        set => SetValue(ValuesProperty, value);
    }

    public Color StartColor
    {
        get => (Color)GetValue(StartColorProperty);
        set => SetValue(StartColorProperty, value);
    }

    public Color EndColor
    {
        get => (Color)GetValue(EndColorProperty);
        set => SetValue(EndColorProperty, value);
    }

    public Color InactiveColor
    {
        get => (Color)GetValue(InactiveColorProperty);
        set => SetValue(InactiveColorProperty, value);
    }

    public float HorizontalSpacing
    {
        get => (float)GetValue(HorizontalSpacingProperty);
        set => SetValue(HorizontalSpacingProperty, value);
    }

    public float VerticalSpacing
    {
        get => (float)GetValue(VerticalSpacingProperty);
        set => SetValue(VerticalSpacingProperty, value);
    }

    private class EqualizerDrawable : IDrawable
    {
        private readonly SoundEqualizer _control;

        public EqualizerDrawable(SoundEqualizer control)
        {
            _control = control;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (_control.Level <= 0 || _control.Range <= 0)
            {
                return;
            }

            // Apply spacing to calculations
            float horizontalSpacing = _control.HorizontalSpacing;
            float verticalSpacing = _control.VerticalSpacing;

            // Calculate available space for cells (excluding spacing)
            float totalHorizontalSpacing = horizontalSpacing * (_control.Range - 1);
            float totalVerticalSpacing = verticalSpacing * (_control.Level - 1);

            // Calculate cell size (considering spacing)
            float cellWidth = (dirtyRect.Width - totalHorizontalSpacing) / _control.Range;
            float cellHeight = (dirtyRect.Height - totalVerticalSpacing) / _control.Level;

            // Get values array
            int[] values = _control.Values ?? Array.Empty<int>();

            // Draw all Range columns, regardless of values array length
            for (int i = 0; i < _control.Range; i++)
            {
                // Get value for this column, if available
                int value = (i < values.Length) ? Math.Clamp(values[i], 0, _control.Level) : 0;

                // Calculate the X position for this column
                float x = i * (cellWidth + horizontalSpacing);

                // Draw cells for this column
                for (int j = 0; j < _control.Level; j++)
                {
                    // Calculate Y position for this cell (from bottom to top)
                    float y = dirtyRect.Height - (j + 1) * cellHeight - j * verticalSpacing;

                    // Determine if this cell should be active or inactive
                    bool isActive = j < value;

                    if (isActive)
                    {
                        // Calculate gradient color (j=0 is StartColor, j=Level-1 is EndColor)
                        float factor = (float)j / (_control.Level - 1);
                        Color gradientColor = InterpolateColor(_control.StartColor, _control.EndColor, factor);
                        canvas.FillColor = gradientColor;
                    }
                    else
                    {
                        canvas.FillColor = _control.InactiveColor;
                    }

                    // Draw the cell
                    canvas.FillRectangle(x, y, cellWidth, cellHeight);
                }
            }
        }

        // Helper method to interpolate between two colors
        private Color InterpolateColor(Color startColor, Color endColor, float factor)
        {
            float r = startColor.Red + (endColor.Red - startColor.Red) * factor;
            float g = startColor.Green + (endColor.Green - startColor.Green) * factor;
            float b = startColor.Blue + (endColor.Blue - startColor.Blue) * factor;
            float a = startColor.Alpha + (endColor.Alpha - startColor.Alpha) * factor;

            return new Color(r, g, b, a);
        }
    }
}