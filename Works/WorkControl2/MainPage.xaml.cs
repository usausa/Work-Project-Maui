namespace WorkControl2
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnTintColorOnClicked(object sender, EventArgs e)
        {
            TintImage.TintColor = Colors.Red;
        }

        private void OnTintClearOnClicked(object sender, EventArgs e)
        {
            TintImage.TintColor = null;
        }
    }
}