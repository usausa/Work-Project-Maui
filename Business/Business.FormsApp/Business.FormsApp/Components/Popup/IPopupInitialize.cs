namespace Business.FormsApp.Components.Popup
{
    using System.Threading.Tasks;

    public interface IPopupInitialize<in T>
    {
        Task Initialize(T parameter);
    }
}
