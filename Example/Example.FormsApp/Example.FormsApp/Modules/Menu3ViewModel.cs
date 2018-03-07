namespace Example.FormsApp.Modules
{
    using Smart.Forms.Input;
    using Smart.Navigation;

    public class Menu3ViewModel : AppViewModelBase
    {
        public AsyncCommand<ViewId> ForwardCommand { get; }

        public Menu3ViewModel(ApplicationState applicationState)
            : base(applicationState)
        {
            ForwardCommand = MakeAsyncCommand<ViewId>(x => Navigator.ForwardAsync(x));
        }
    }
}
