namespace Business.FormsApp.Components.Popup
{
    using System.Threading.Tasks;

    public interface IPopupNavigator
    {
        Task<TResult> PopupAsync<TResult>(object id);

        Task<TResult> PopupAsync<TParameter, TResult>(object id, TParameter parameter);

        Task PopupAsync(object id);

        Task PopupAsync<TParameter>(object id, TParameter parameter);

        Task PopAsync();
    }
}
