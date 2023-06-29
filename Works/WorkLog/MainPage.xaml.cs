using Other;

namespace WorkLog
{
    public partial class MainPage : ContentPage
    {
        private readonly IServiceProvider provider;

        public MainPage(IServiceProvider provider)
        {
            InitializeComponent();

            this.provider = provider;
        }

        private void SingletonButton_OnClicked(object sender, EventArgs e)
        {
            var service = provider.GetRequiredService<SingletonService>();
            service.Execute();
        }

        private void TransientButton_OnClicked(object sender, EventArgs e)
        {
            var service = provider.GetRequiredService<TransientService>();
            service.Execute();
        }

        private void OtherButton_OnClicked(object sender, EventArgs e)
        {
            var service = provider.GetRequiredService<OtherService>();
            service.Execute();
        }
    }
}