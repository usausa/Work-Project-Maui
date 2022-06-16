namespace GraphicExample;

using Smart.Maui.Interactivity;

public class GraphicBehavior : BehaviorBase<GraphicsView>
{
    private readonly IDrawable drawable;

    private readonly IGraphicSource source;

    public GraphicBehavior(IDrawable drawable, IGraphicSource source)
    {
        this.drawable = drawable;
        this.source = source;
    }

    protected override void OnAttachedTo(GraphicsView bindable)
    {
        base.OnAttachedTo(bindable);

        AssociatedObject!.Drawable = drawable;
        source.InvalidateRequest += InvalidateRequest;
    }

    protected override void OnDetachingFrom(GraphicsView bindable)
    {
        source.InvalidateRequest -= InvalidateRequest;
        AssociatedObject!.Drawable = null;

        base.OnDetachingFrom(bindable);
    }

    private void InvalidateRequest(object? sender, EventArgs e)
    {
        AssociatedObject!.Invalidate();
    }
}

public static class Graphic
{
    public static readonly BindableProperty SourceProperty = BindableProperty.CreateAttached(
        "Source",
        typeof(IGraphicSource),
        typeof(Graphic),
        null,
        propertyChanged: BindChanged);

    public static IGraphicSource? GetSource(BindableObject bindable) =>
        (IGraphicSource?)bindable.GetValue(SourceProperty);

    public static void SetSource(BindableObject bindable, IGraphicSource? value) =>
        bindable.SetValue(SourceProperty, value);

    public static readonly BindableProperty DrawerProperty = BindableProperty.CreateAttached(
        "Drawer",
        typeof(Type),
        typeof(Graphic),
        null,
        propertyChanged: BindChanged);

    public static Type? GetDrawer(BindableObject bindable) =>
        (Type?)bindable.GetValue(DrawerProperty);

    public static void SetDrawer(BindableObject bindable, Type? value) =>
        bindable.SetValue(DrawerProperty, value);

    private static void BindChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (bindable is not GraphicsView entry)
        {
            return;
        }

        var behavior = entry.Behaviors.FirstOrDefault(x => x is GraphicBehavior);
        if (behavior is not null)
        {
            entry.Behaviors.Remove(behavior);
        }

        var drawer = GetDrawer(bindable);
        var source = GetSource(bindable);
        if ((drawer is not null) && (source is not null))
        {
            var drawable = (IDrawable)Activator.CreateInstance(drawer, source.Source)!;
            entry.Behaviors.Add(new GraphicBehavior(drawable, source));
        }
    }
}
