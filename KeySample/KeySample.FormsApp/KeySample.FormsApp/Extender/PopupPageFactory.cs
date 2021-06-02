namespace KeySample.FormsApp.Extender
{
    using System.Linq;

    using KeySample.FormsApp.Helpers;
    using KeySample.FormsApp.Input;

    using Rg.Plugins.Popup.Pages;

    using Xamarin.Forms;

    using XamarinFormsComponents.Popup;

    public sealed class PopupPageFactory : IPopupPageFactory
    {
        public PopupPage Create(View content)
        {
            var page = new PopupPage
            {
                Content = content,
                CloseWhenBackgroundIsClicked = false,
                HasSystemPadding = true,
                Padding = PopupProperty.GetThickness(content)
            };

            page.Behaviors.Add(new KeyInputBehavior());
            page.Appearing += (_, _) =>
            {
                Device.InvokeOnMainThreadAsync(() => page.SetDefaultFocus());
            };

            return page;
        }
    }
}
