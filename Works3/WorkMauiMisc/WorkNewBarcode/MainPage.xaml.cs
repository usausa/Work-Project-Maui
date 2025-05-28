namespace WorkNewBarcode
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void MainPage_OnLoaded(object? sender, EventArgs e)
        {
            if (!await IsGrantedCameraPermissionAsync())
            {
                await DisplayAlert("警告", "カメラ権限がありません。", "OK");
                return;
            }
        }

        private async Task<bool> IsGrantedCameraPermissionAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status == PermissionStatus.Granted)
            {
                return true;
            }

            status = await Permissions.RequestAsync<Permissions.Camera>();
            return status == PermissionStatus.Granted;
        }
    }
}
