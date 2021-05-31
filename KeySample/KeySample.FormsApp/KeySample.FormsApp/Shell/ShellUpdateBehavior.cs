namespace KeySample.FormsApp.Shell
{
    using System;
    using System.Linq;

    using KeySample.FormsApp.Input;

    using Smart.Forms.Interactivity;
    using Smart.Navigation;

    using Xamarin.Forms;

    public sealed class ShellUpdateBehavior : BehaviorBase<ContentPage>
    {
        public static readonly BindableProperty NavigatorProperty = BindableProperty.Create(
            nameof(Navigator),
            typeof(INavigator),
            typeof(ShellUpdateBehavior),
            propertyChanged: HandlePropertyChanged);

        public INavigator? Navigator
        {
            get => (INavigator)GetValue(NavigatorProperty);
            set => SetValue(NavigatorProperty, value);
        }

        protected override void OnDetachingFrom(ContentPage bindable)
        {
            if (Navigator is not null)
            {
                Navigator.Navigated -= NavigatorOnNavigated;
                Navigator.Exited -= NavigatorOnExited;
            }

            base.OnDetachingFrom(bindable);
        }

        private static void HandlePropertyChanged(BindableObject bindable, object? oldValue, object? newValue)
        {
            ((ShellUpdateBehavior)bindable).OnNavigatorPropertyChanged(oldValue as INavigator, newValue as INavigator);
        }

        private void OnNavigatorPropertyChanged(INavigator? oldValue, INavigator? newValue)
        {
            if (newValue == oldValue)
            {
                return;
            }

            if (oldValue is not null)
            {
                oldValue.Navigated -= NavigatorOnNavigated;
                oldValue.Exited -= NavigatorOnExited;
            }

            if (newValue is not null)
            {
                newValue.Navigated += NavigatorOnNavigated;
                newValue.Exited += NavigatorOnExited;
            }
        }

        private void NavigatorOnNavigated(object sender, Smart.Navigation.NavigationEventArgs e)
        {
            var view = e.ToView as Element;

            UpdateShell(view);

            if ((AssociatedObject is not null) && (view is not null) && ShellProperty.GetDefaultFocus(view))
            {
                Device.InvokeOnMainThreadAsync(() =>
                {
                    var target = ElementHelper.EnumerateActive(AssociatedObject).FirstOrDefault();
                    target?.Focus();
                });
            }
        }

        private void NavigatorOnExited(object sender, EventArgs e)
        {
            UpdateShell(null);
        }

        private void UpdateShell(BindableObject? view)
        {
            if (AssociatedObject?.BindingContext is IShellControl shell)
            {
                ShellProperty.UpdateShellControl(shell, view);
            }
        }
    }
}
