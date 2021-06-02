namespace KeySample.FormsApp.Shell
{
    using Rg.Plugins.Popup.Pages;

    using Xamarin.Forms;

    using XamarinFormsComponents.Popup;

    public sealed class FocusControlPopupPageFactory : IPopupPageFactory
    {
        public PopupPage Create(View content)
        {
            return new PopupPage
            {
                Content = content,
                CloseWhenBackgroundIsClicked = false,
                HasSystemPadding = true,
                Padding = PopupProperty.GetThickness(content)
            };
        }
    }
}
