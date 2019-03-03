namespace Business.FormsApp.Components.Popup
{
    using System;

    using Smart.Resolver;

    using Xamarin.Forms;

    public sealed class SmartPopupFactory : IPopupFactory
    {
        private readonly IResolver resolver;

        public SmartPopupFactory(IResolver resolver)
        {
            this.resolver = resolver;
        }

        public View Get(Type type)
        {
            return (View)resolver.Get(type);
        }
    }
}
