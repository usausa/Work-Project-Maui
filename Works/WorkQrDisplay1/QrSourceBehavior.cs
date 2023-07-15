namespace WorkQrDisplay1;

using QRCoder;

using Smart.Maui.Interactivity;

public sealed class QrSourceBehavior : BehaviorBase<Image>
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(QrSourceBehavior),
        propertyChanged: Update);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    private static void Update(BindableObject bindable, object oldvalue, object newvalue)
    {
        ((QrSourceBehavior)bindable).Update();
    }

    private void Update()
    {
        if (AssociatedObject is null)
        {
            return;
        }

        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(Text, QRCodeGenerator.ECCLevel.L, true);
        var png = new PngByteQRCode(data);
        var bytes = png.GetGraphic(20);
        AssociatedObject.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
    }
}
