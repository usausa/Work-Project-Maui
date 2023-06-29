using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace WorkAdjust
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
            On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        }
    }
}