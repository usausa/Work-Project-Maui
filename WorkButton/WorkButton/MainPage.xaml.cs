namespace WorkButton
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            MultiLineButton.Text = String.Join(Environment.NewLine, "CPU", "100%");
        }
    }
}
