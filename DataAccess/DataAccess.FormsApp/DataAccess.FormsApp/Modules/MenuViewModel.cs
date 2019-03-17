namespace DataAccess.FormsApp.Modules
{
    public class MenuViewModel : AppViewModelBase
    {
        public static MenuViewModel DesignInstance { get; } = null; // For design

        public MenuViewModel(ApplicationState applicationState)
            : base(applicationState)
        {
        }
    }
}
