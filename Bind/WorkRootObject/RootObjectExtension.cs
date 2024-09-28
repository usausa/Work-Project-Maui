namespace WorkRootObject;

using System;

public sealed class RootObjectExtension : IMarkupExtension
{
    public object ProvideValue(IServiceProvider serviceProvider)
    {
        var provider = serviceProvider.GetRequiredService<IRootObjectProvider>();
        var root = provider.RootObject;
        return RootBind.GetTag((BindableObject)root);
    }
}
