using System.Diagnostics;

namespace WorkControlSquare;

using Smart.Maui.Interactivity;

public sealed class SquareSizeBehavior : BehaviorBase<View>
{
    private View? parent;

    protected override void OnAttachedTo(View bindable)
    {
        base.OnAttachedTo(bindable);

        bindable.ParentChanged += OnParentChanged;

        // TODO?
    }

    protected override void OnDetachingFrom(View bindable)
    {
        bindable.ParentChanged -= OnParentChanged;

        if (parent is not null)
        {
            parent.SizeChanged -= OnParentSizeChanged;
        }

        base.OnDetachingFrom(bindable);
    }

    private void OnParentChanged(object sender, EventArgs e)
    {
        Debug.WriteLine("* OnParentChanged");

        if (parent is not null)
        {
            parent.SizeChanged -= OnParentSizeChanged;
            Debug.WriteLine("* OnParentChanged : Remove old parent event");
        }

        parent = AssociatedObject?.Parent as View;

        if (parent is not null)
        {
            parent.SizeChanged += OnParentSizeChanged;
            Debug.WriteLine("* OnParentChanged : Add new parent event");
        }
    }

    private void OnParentSizeChanged(object sender, EventArgs e)
    {
        Debug.WriteLine("* OnParentSizeChanged");

        if ((AssociatedObject is null) || (parent is null))
        {
            return;
        }

        var size = Math.Min(parent.Width, parent.Height);
        AssociatedObject.WidthRequest = size;
        AssociatedObject.HeightRequest = size;
        Debug.WriteLine($"* OnParentSizeChanged : size={size}");
    }
}
