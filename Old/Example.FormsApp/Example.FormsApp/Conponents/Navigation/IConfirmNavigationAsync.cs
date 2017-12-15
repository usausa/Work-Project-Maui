namespace Example.FormsApp.Conponents.Navigation
{
    using System.Threading.Tasks;

    public interface IConfirmNavigationAsync
    {
        Task<bool> CanNavigateAsync();
    }
}
