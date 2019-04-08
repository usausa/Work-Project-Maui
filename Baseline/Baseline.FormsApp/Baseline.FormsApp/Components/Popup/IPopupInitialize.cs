namespace Baseline.FormsApp.Components.Popup
{
    public interface IPopupInitialize<in T>
    {
        void Initialize(T parameter);
    }
}
