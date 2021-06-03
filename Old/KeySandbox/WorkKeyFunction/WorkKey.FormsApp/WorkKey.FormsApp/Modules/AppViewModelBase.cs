namespace WorkKey.FormsApp.Modules
{
    using System.Threading.Tasks;

    using Smart.Forms.ViewModels;
    using Smart.Navigation;

    using WorkKey.FormsApp.Shell;

    public class AppViewModelBase : ViewModelBase, INavigatorAware, INavigationEventSupport, INotifySupportAsync<ShellEvent>
    {
        public INavigator Navigator { get; set; }

        public ApplicationState ApplicationState { get; }

        protected AppViewModelBase(ApplicationState applicationState)
            : base(applicationState)
        {
            ApplicationState = applicationState;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            System.Diagnostics.Debug.WriteLine($"{GetType()} is Disposed");
        }

        public virtual void OnNavigatingFrom(INavigationContext context)
        {
        }

        public virtual void OnNavigatingTo(INavigationContext context)
        {
        }

        public virtual void OnNavigatedTo(INavigationContext context)
        {
        }

        public Task NavigatorNotifyAsync(ShellEvent parameter)
        {
            return parameter switch
            {
                ShellEvent.Back => OnNotifyBackAsync(),
                ShellEvent.Function1 => OnNotifyFunction1Async(),
                ShellEvent.Function2 => OnNotifyFunction2Async(),
                ShellEvent.Function3 => OnNotifyFunction3Async(),
                ShellEvent.Function4 => OnNotifyFunction4Async(),
                _ => Task.CompletedTask
            };
        }

        protected virtual Task OnNotifyBackAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnNotifyFunction1Async()
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnNotifyFunction2Async()
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnNotifyFunction3Async()
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnNotifyFunction4Async()
        {
            return Task.CompletedTask;
        }
    }
}
