namespace WorkButton
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            MultiLineButton.Text = String.Join(Environment.NewLine, "CPU", "100%");
        }

        // ReSharper disable once AsyncVoidMethod
        private async void SourceButton_OnClicked(object? sender, EventArgs e)
        {
            // [MEMO] svgだと動作しない？
            //await using var stream = await FileSystem.OpenAppPackageFileAsync("folder.svg");
            await using var stream = await FileSystem.OpenAppPackageFileAsync("folder2.png");
            var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
            // ReSharper disable once AccessToDisposedClosure
            var source = ImageSource.FromStream(() => ms);
            SourceButton.Source = source;
        }
    }
}
