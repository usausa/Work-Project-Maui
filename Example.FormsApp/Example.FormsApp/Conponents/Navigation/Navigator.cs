namespace Example.FormsApp.Conponents.Navigation
{
    using System;
    using System.Threading.Tasks;

    using Xamarin.Forms;

    public sealed class Navigator : IDisposable
    {
        private bool navigating;

        public Navigator()
        {
            // TODO proxy?
            Application.Current.ModalPopping += OnModalPopping;
            Application.Current.ModalPopped += OnModalPopped;
        }

        public void Dispose()
        {
            Application.Current.ModalPopping -= OnModalPopping;
            Application.Current.ModalPopped -= OnModalPopped;
        }

        //--------------------------------------------------------------------------------
        //
        //--------------------------------------------------------------------------------

        private void OnModalPopping(object sender, ModalPoppingEventArgs modalPoppingEventArgs)
        {
            if (!navigating)
            {
                // TODO
            }
        }

        private void OnModalPopped(object sender, ModalPoppedEventArgs modalPoppedEventArgs)
        {
            if (!navigating)
            {
                // TODO
            }
        }

        //--------------------------------------------------------------------------------

        public async Task<bool> ForwardAsync<TPage>()
            where TPage : Page
        {
            // TODO
            await Task.Delay(0);

            return false;
        }

        public async Task<bool> PopForwardAsync<TPage>()
            where TPage : Page
        {
            // TODO
            await Task.Delay(0);

            return false;
        }

        public async Task<bool> PushForwardAsync()
        {
            // TODO
            await Task.Delay(0);

            return false;
        }

        public async Task<bool> PopModalAsync<TPage>()
            where TPage : Page
        {
            // TODO
            await Task.Delay(0);

            return false;
        }

        public async Task<bool> PushModalAsync()
        {
            // TODO
            await Task.Delay(0);

            return false;
        }
    }

    //        public async Task<bool> ForwardAsync(string name)
    //        {
    //            // Guard
    //            if (Application.Current.MainPage.Navigation.ModalStack.Count > 0)
    //            {
    //                return false;
    //            }
    //
    //            // Stack
    //            var navigationStack = Application.Current.MainPage.Navigation.NavigationStack;
    //
    //            var fromPage = navigationStack.Count > 0 ? navigationStack[navigationStack.Count - 1] : null;
    //
    //            var previousName = fromPage != null ? pageResolver.ResolveName(fromPage.GetType()) : string.Empty;
    //            var normalizeName = pageResolver.NormarizeName(previousName, name);
    //
    //            // Context
    //            var context = new NavigationContext(
    //                parameters ?? new NavigationParameters(),
    //                false,
    //                normalizeName,
    //                previousName);
    //
    //            // Confirm
    //            if (fromPage != null)
    //            {
    //                if (!await PageHelper.ProcessCanNavigateAsync(fromPage))
    //                {
    //                    return false;
    //                }
    //            }
    //
    //            try
    //            {
    //                Navigating = true;
    //
    //                // Prepare
    //                var toPage = CreatePage(normalizeName);
    //                if (toPage == null)
    //                {
    //                    throw new ArgumentException(
    //                        String.Format(CultureInfo.InvariantCulture, "Invalid name. [{0}]", name), nameof(name));
    //                }
    //
    //                // From event
    //                ProcessNavigatedFrom(fromPage);
    //
    //                // To event
    //                ProcessNavigatingTo(toPage);
    //
    //                // Replace new page
    //                await Application.Current.MainPage.Navigation.PushAsync(toPage);
    //                fromPage?.Apply(Application.Current.MainPage.Navigation.RemovePage);
    //
    //                // To event
    //                ProcessNavigatedTo(toPage);
    //
    //                // Remove old page
    //                ClosePage(fromPage);
    //
    //                return true;
    //            }
    //            finally
    //            {
    //                Navigating = false;
    //            }
    //        }
    //
    //        public async Task<bool> PushModelAsync(string name)
    //        {
    //            // Stack
    //            var modalStack = Application.Current.MainPage.Navigation.ModalStack;
    //            var navigationStack = Application.Current.MainPage.Navigation.NavigationStack;
    //
    //            var fromPage = modalStack.Count > 0
    //                ? modalStack[modalStack.Count - 1]
    //                : navigationStack.Count > 0 ? navigationStack[navigationStack.Count - 1] : null;
    //
    //            var previousName = fromPage != null ? pageResolver.ResolveName(fromPage.GetType()) : string.Empty;
    //            var normalizeName = pageResolver.NormarizeName(previousName, name);
    //
    //            // Context
    //            var context = new NavigationContext(
    //                parameters ?? new NavigationParameters(),
    //                false,
    //                normalizeName,
    //                previousName);
    //
    //            // Confirm
    //            if (fromPage != null)
    //            {
    //                if (!await PageHelper.ProcessCanNavigateAsync(fromPage))
    //                {
    //                    return false;
    //                }
    //            }
    //
    //            try
    //            {
    //                Navigating = true;
    //
    //                // Prepare
    //                var toPage = CreatePage(normalizeName);
    //                if (toPage == null)
    //                {
    //                    throw new ArgumentException(
    //                        String.Format(CultureInfo.InvariantCulture, "Invalid name. [{0}]", name), nameof(name));
    //                }
    //
    //                // From event
    //                ProcessNavigatedFrom(fromPage);
    //
    //                // To event
    //                ProcessNavigatingTo(toPage);
    //
    //                // Replace new page
    //                await Application.Current.MainPage.Navigation.PushModalAsync(toPage);
    //
    //                // To event
    //                ProcessNavigatedTo(toPage);
    //
    //                return true;
    //            }
    //            finally
    //            {
    //                Navigating = false;
    //            }
    //        }
    //
    //        public async Task<bool> PopModalAsync(NavigationParameters parameters)
    //        {
    //            // Stack
    //            var modalStack = Application.Current.MainPage.Navigation.ModalStack;
    //            var navigationStack = Application.Current.MainPage.Navigation.NavigationStack;
    //
    //            if (modalStack.Count == 0)
    //            {
    //                return false;
    //            }
    //
    //            var fromPage = modalStack[modalStack.Count - 1];
    //            var toPage = modalStack.Count > 1
    //                ? modalStack[modalStack.Count - 2]
    //                : navigationStack.Count > 0 ? navigationStack[navigationStack.Count - 1] : null;
    //
    //            var previousName = pageResolver.ResolveName(fromPage.GetType());
    //            var name = toPage != null ? pageResolver.ResolveName(toPage.GetType()) : string.Empty;
    //
    //            // Context
    //            var context = new NavigationContext(
    //                parameters ?? new NavigationParameters(),
    //                true,
    //                name,
    //                previousName);
    //
    //            // Confirm
    //            if (!await PageHelper.ProcessCanNavigateAsync(fromPage))
    //            {
    //                return false;
    //            }
    //
    //            try
    //            {
    //                Navigating = true;
    //
    //                // From event
    //                ProcessNavigatedFrom(fromPage);
    //
    //                // To event
    //                ProcessNavigatingTo(toPage);
    //
    //                // Replace new page
    //                await Application.Current.MainPage.Navigation.PopModalAsync();
    //
    //                // To event
    //                ProcessNavigatedTo(toPage);
    //
    //                // Remove old page
    //                ClosePage(fromPage);
    //
    //                return true;
    //            }
    //            finally
    //            {
    //                Navigating = false;
    //            }
    //        }
    //
    //        private Page CreatePage(string name)
    //        {
    //            var type = pageResolver.ResolveType(name);
    //            if (type == null)
    //            {
    //                return null;
    //            }
    //
    //            var page = (Page)activator.Get(type);
    //
    //            foreach (var plugin in plugins)
    //            {
    //                plugin.OnCreate(page);
    //            }
    //
    //            return page;
    //        }
    //
    //        private void ClosePage(Page page)
    //        {
    //            if (page == null)
    //            {
    //                return;
    //            }
    //
    //            foreach (var plugin in plugins)
    //            {
    //                plugin.OnClose(page);
    //            }
    //
    //            PageHelper.DestroyPage(page);
    //        }
    //
    //        private void ProcessNavigatedFrom(Page page)
    //        {
    //            if (page == null)
    //            {
    //                return;
    //            }
    //
    //            PageHelper.ProcessNavigatedFrom(page);
    //
    //            foreach (var plugin in plugins)
    //            {
    //                plugin.OnNavigatedFrom(page);
    //            }
    //        }
    //
    //        private void ProcessNavigatingTo(Page page)
    //        {
    //            if (page == null)
    //            {
    //                return;
    //            }
    //
    //            foreach (var plugin in plugins)
    //            {
    //                plugin.OnNavigatingTo(page);
    //            }
    //
    //            PageHelper.ProcessNavigatingTo(page);
    //        }
    //
    //        private void ProcessNavigatedTo(Page page)
    //        {
    //            if (page == null)
    //            {
    //                return;
    //            }
    //
    //            foreach (var plugin in plugins)
    //            {
    //                plugin.OnNavigatedTo(page);
    //            }
    //
    //            PageHelper.ProcessNavigatedTo(page);
    //        }
}
