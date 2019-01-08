namespace Business.FormsApp.Modules.Data
{
    using System.Threading.Tasks;

    using Smart.Forms.Input;
    using Smart.Navigation;

    public class DataRawViewModel : AppViewModelBase
    {
        public DataRawViewModel(ApplicationState applicationState)
            : base(applicationState)
        {
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.DataMenu);
        }
    }
}
