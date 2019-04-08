namespace Baseline.FormsApp.Components.Popup
{
    using System;

    using Xamarin.Forms;

    public interface IPopupFactory
    {
        View Get(Type type);
    }
}
