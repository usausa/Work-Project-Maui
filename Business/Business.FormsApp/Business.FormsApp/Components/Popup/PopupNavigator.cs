namespace Business.FormsApp.Components.Popup
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    using Rg.Plugins.Popup.Pages;
    using Rg.Plugins.Popup.Services;

    using Smart.Resolver;

    using Xamarin.Forms;

    public class PopupNavigator : IPopupNavigator
    {
        private readonly IResolver resolver;

        private readonly Dictionary<object, Type> popupTypes = new Dictionary<object, Type>();

        public PopupNavigator(IResolver resolver)
        {
            this.resolver = resolver;
        }

        public void AutoRegister(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                foreach (var attr in type.GetTypeInfo().GetCustomAttributes<PopupAttribute>())
                {
                    popupTypes[attr.Id] = type;
                }
            }
        }

        public Task<TResult> PopupAsync<TResult>(object id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<TResult> PopupAsync<TParameter, TResult>(object id, TParameter parameter)
        {
            if (!popupTypes.TryGetValue(id, out var type))
            {
                throw new ArgumentException($"Invalid id=[{id}]", nameof(id));
            }

            var content = (View)resolver.Get(type);

            if (content.BindingContext is IPopupNavigatorAware aware)
            {
                aware.PopupNavigator = this;
            }

            if (content.BindingContext is IPopupInitialize<TParameter> initialize)
            {
                await initialize.Initialize(parameter);
            }

            // TODO
            var popup = new PopupPage
            {
                Content = content,
                CloseWhenBackgroundIsClicked = false,
                HasSystemPadding = true,
                Padding = new Thickness(10)
            };

            var cts = new TaskCompletionSource<TResult>();
            popup.Disappearing += (sender, args) =>
            {
                if (((PopupPage)sender).BindingContext is IPopupResult<TResult> result)
                {
                    cts.SetResult(result.Result);
                }
                else
                {
                    cts.SetResult(default);
                }
            };

            await PopupNavigation.Instance.PushAsync(popup);

            return await cts.Task;
        }

        public async Task PopupAsync(object id)
        {
            if (!popupTypes.TryGetValue(id, out var type))
            {
                throw new ArgumentException($"Invalid id=[{id}]", nameof(id));
            }

            var content = (View)resolver.Get(type);

            if (content.BindingContext is IPopupNavigatorAware aware)
            {
                aware.PopupNavigator = this;
            }

            // TODO
            var popup = new PopupPage
            {
                Content = content,
                CloseWhenBackgroundIsClicked = false,
                HasSystemPadding = true,
                Padding = new Thickness(10)
            };

            var cts = new TaskCompletionSource<object>();
            popup.Disappearing += (sender, args) =>
            {
                cts.SetResult(default);
            };

            await PopupNavigation.Instance.PushAsync(popup);

            await cts.Task;
        }

        public Task PopupAsync<TParameter>(object id, TParameter parameter)
        {
            throw new System.NotImplementedException();
        }

        public async Task PopAsync()
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}
