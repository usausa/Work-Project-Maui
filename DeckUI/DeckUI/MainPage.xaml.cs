namespace DeckUI
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            // https://fonts.google.com/icons
            // https://gradientbuttons.colorion.co/

            CpuButton.Text = String.Join(Environment.NewLine, "CPU", "13%");
            MemoryButton.Text = String.Join(Environment.NewLine, "MEM", "74%");
        }
    }
}
