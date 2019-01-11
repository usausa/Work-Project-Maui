namespace Business.FormsApp.Modules
{
    using Business.FormsApp.Components.Popup;

    using Smart.Forms.ViewModels;

    public class AppDialogViewModelBase : ViewModelBase, IPopupNavigatorAware
    {
        public IPopupNavigator PopupNavigator { get; set; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            System.Diagnostics.Debug.WriteLine($"{GetType()} is Disposed");
        }
    }
}
