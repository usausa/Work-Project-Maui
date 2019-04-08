namespace Baseline.FormsApp.Components.Popup
{
    public interface IPopupResult<out T>
    {
        T Result { get; }
    }
}
