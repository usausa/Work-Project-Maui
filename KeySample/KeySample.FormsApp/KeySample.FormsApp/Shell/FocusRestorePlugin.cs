namespace KeySample.FormsApp.Shell
{
    using System.Collections.Generic;

    using KeySample.FormsApp.Helpers;

    using Smart.Forms;
    using Smart.Navigation;
    using Smart.Navigation.Plugins;

    using Xamarin.Forms;

    public sealed class FocusRestorePlugin : PluginBase
    {
        private readonly Dictionary<object, VisualElement> focusBackup = new();

        public override void OnClose(IPluginContext pluginContext, object view, object target)
        {
            focusBackup.Remove(view);
        }

        public override void OnNavigatingFrom(IPluginContext pluginContext, INavigationContext navigationContext, object? view, object? target)
        {
            if (navigationContext.Attribute.IsStacked() && (view is Element element))
            {
                var page = element.FindParent<Page>();
                if (page is not null)
                {
                    var focused = ElementHelper.FindFocused(page);
                    if (focused is not null)
                    {
                        focusBackup[view] = focused;
                    }
                }
            }
        }

        public override void OnNavigatedTo(IPluginContext pluginContext, INavigationContext navigationContext, object view, object target)
        {
            if (navigationContext.Attribute.IsRestore() && focusBackup.TryGetValue(view, out var focused))
            {
                Device.InvokeOnMainThreadAsync(() =>
                {
                    focused.Focus();
                });
            }
        }
    }
}
