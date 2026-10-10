namespace Template.MobileApp.Behaviors;

using CommunityToolkit.Maui.Views;

// CommunityToolkit の LazyView は LoadViewAsync() 呼び出しで初めて中身を生成する。
// code-behind を使わず VM のフラグから起動できるよう添付プロパティで橋渡しする
// ContentView には Template (DataTemplate) を渡すと、Load が初めて true になったときに中身を作る (以後は残す)
public static class LazyViewOption
{
    public static readonly BindableProperty LoadProperty = BindableProperty.CreateAttached(
        "Load",
        typeof(bool),
        typeof(LazyViewOption),
        false,
        propertyChanged: HandleLoadChanged);

    public static bool GetLoad(BindableObject bindable) => (bool)bindable.GetValue(LoadProperty);

    public static void SetLoad(BindableObject bindable, bool value) => bindable.SetValue(LoadProperty, value);

    public static readonly BindableProperty TemplateProperty = BindableProperty.CreateAttached(
        "Template",
        typeof(DataTemplate),
        typeof(LazyViewOption),
        null,
        propertyChanged: HandleTemplateChanged);

    public static DataTemplate? GetTemplate(BindableObject bindable) => (DataTemplate?)bindable.GetValue(TemplateProperty);

    public static void SetTemplate(BindableObject bindable, DataTemplate? value) => bindable.SetValue(TemplateProperty, value);

    private static void HandleLoadChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if ((bindable is LazyView view) && (newValue is true) && !view.HasLazyViewLoaded)
        {
            _ = LoadAsync(view);
        }
        else if ((bindable is ContentView content) && (newValue is true))
        {
            LoadTemplate(content);
        }
    }

    private static void HandleTemplateChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if ((bindable is ContentView content) && GetLoad(content))
        {
            LoadTemplate(content);
        }
    }

    private static void LoadTemplate(ContentView view)
    {
        if ((view.Content is null) && (GetTemplate(view) is { } template))
        {
            view.Content = (View)template.CreateContent();
        }
    }

    private static async Task LoadAsync(LazyView view) => await view.LoadViewAsync().ConfigureAwait(true);
}
