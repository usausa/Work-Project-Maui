namespace KeySample.FormsApp.Modules.Dialog
{
    using Smart.Forms.ViewModels;

    using XamarinFormsComponents.Popup;

    public class DialogPopupViewModel : ViewModelBase, IPopupNavigatorAware
    {
        public IPopupNavigator PopupNavigator { get; set; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            System.Diagnostics.Debug.WriteLine($"{GetType()} is Disposed");
        }
    }
}
