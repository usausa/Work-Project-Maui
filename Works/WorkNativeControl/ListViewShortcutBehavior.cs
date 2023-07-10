#nullable enable
namespace WorkNativeControl;

using System.Collections;
using System.Windows.Input;

using Smart.Maui.Interactivity;

public sealed partial class ListViewShortcutBehavior : BehaviorBase<ListView>
{
    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command),
        typeof(ICommand),
        typeof(ListViewShortcutBehavior));

    public ICommand? Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    protected override void OnAttachedTo(ListView bindable)
    {
        base.OnAttachedTo(bindable);

        EventHub.Default.Handle += Handle;
    }

    protected override void OnDetachingFrom(ListView bindable)
    {
        EventHub.Default.Handle -= Handle;

        base.OnDetachingFrom(bindable);
    }

    private void Handle(object? sender, EventArgs e)
    {
        if (AssociatedObject is null)
        {
            return;
        }

        var command = Command;
        if (command is null)
        {
            return;
        }

        var item = AssociatedObject.SelectedItem;
        if ((item is not null) && command.CanExecute(item))
        {
            command.Execute(item);
        }
    }
}
