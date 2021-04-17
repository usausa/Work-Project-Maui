namespace ViewSample.FormsApp.Modules
{
    using System.Threading.Tasks;

    using ViewSample.FormsApp.Models.Input;

    using XamarinFormsComponents.Popup;

    public static class PopupNavigatorExtensions
    {
        public static ValueTask<string> InputNumberAsync(this IPopupNavigator popupNavigator, string value, int maxLength)
        {
            return popupNavigator.PopupAsync<NumberInputParameter, string>(
                DialogId.InputNumber,
                new NumberInputParameter(value, maxLength, 0, false));
        }
    }
}
