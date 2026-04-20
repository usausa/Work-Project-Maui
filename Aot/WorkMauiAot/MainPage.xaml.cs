using System.Runtime.InteropServices;

namespace WorkMauiAot
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            FrameworkDescription.Text = RuntimeInformation.FrameworkDescription;
            RuntimeIdentifier.Text = RuntimeInformation.RuntimeIdentifier;
#if DEBUG
            Build.Text = "Debug";
#else
            Build.Text = "Release";
#endif
        }
    }
}
