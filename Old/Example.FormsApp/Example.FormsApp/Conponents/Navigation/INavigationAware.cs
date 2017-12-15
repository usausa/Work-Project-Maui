namespace Example.FormsApp.Conponents.Navigation
{
    public interface INavigationAware
    {
        void OnNavigatedFrom();

        void OnNavigatingTo();

        void OnNavigatedTo();
    }
}
