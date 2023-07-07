namespace Template.MobileApp.Modules;

using Smart.Navigation.Attributes;

public static partial class ViewRegistry
{
    [ViewSource]
    public static partial IEnumerable<KeyValuePair<ViewId, Type>> ListViews();
}
