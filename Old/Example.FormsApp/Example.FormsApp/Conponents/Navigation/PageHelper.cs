namespace Example.FormsApp.Conponents.Navigation
{
    using System;
    using System.Threading.Tasks;

    using Smart.Functional;

    using Xamarin.Forms;

    public static class PageHelper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Ignore")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Framework only")]
        public static void DestroyPage(Page page)
        {
            ProcessDispose(page);
            Cleanup(page);
        }

        public static void ProcessDispose(object page)
        {
            (page as IDisposable)?.Dispose();
            ((page as BindableObject)?.BindingContext as IDisposable)?.Dispose();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Framework only")]
        public static void Cleanup(VisualElement element)
        {
            element.Behaviors?.Clear();
            element.Effects?.Clear();
            element.BindingContext = null;

            if (element is Layout<View> layout)
            {
                foreach (var child in layout.Children)
                {
                    Cleanup(child);
                }
            }

            (element as ContentPage)?.Apply(x => Cleanup(x.Content));

            (element as ContentView)?.Apply(x => Cleanup(x.Content));
        }

        public static Task<bool> ProcessCanNavigateAsync(object page)
        {
            if (page is IConfirmNavigationAsync confirmNavigation)
            {
                return confirmNavigation.CanNavigateAsync();
            }

            confirmNavigation = (page as BindableObject)?.BindingContext as IConfirmNavigationAsync;
            if (confirmNavigation != null)
            {
                return confirmNavigation.CanNavigateAsync();
            }

            return Task.FromResult(ProcessCanNavigate(page));
        }

        public static bool ProcessCanNavigate(object page)
        {
            if (page is IConfirmNavigation confirmNavigation)
            {
                return confirmNavigation.CanNavigate();
            }

            confirmNavigation = (page as BindableObject)?.BindingContext as IConfirmNavigation;
            if (confirmNavigation != null)
            {
                return confirmNavigation.CanNavigate();
            }

            return true;
        }

        public static void ProcessNavigatedFrom(object page)
        {
            (page as INavigationAware)?.OnNavigatedFrom();
            ((page as BindableObject)?.BindingContext as INavigationAware)?.OnNavigatedFrom();
        }

        public static void ProcessNavigatingTo(object page)
        {
            (page as INavigationAware)?.OnNavigatingTo();
            ((page as BindableObject)?.BindingContext as INavigationAware)?.OnNavigatingTo();
        }

        public static void ProcessNavigatedTo(object page)
        {
            (page as INavigationAware)?.OnNavigatedTo();
            ((page as BindableObject)?.BindingContext as INavigationAware)?.OnNavigatedTo();
        }
    }
}
