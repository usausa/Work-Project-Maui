namespace Template.MobileApp.Behaviors;

using Smart.Maui.Interactivity;

using Template.MobileApp.Helpers;
using Template.MobileApp.Messaging;

public static class EntryBind
{
    public static readonly BindableProperty ControllerProperty = BindableProperty.CreateAttached(
        "Controller",
        typeof(IEntryController),
        typeof(EntryBind),
        null,
        propertyChanged: BindChanged);

    public static IEntryController GetController(BindableObject bindable) =>
        (IEntryController)bindable.GetValue(ControllerProperty);

    public static void SetController(BindableObject bindable, IEntryController value) =>
        bindable.SetValue(ControllerProperty, value);

    private static void BindChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (bindable is not Entry entry)
        {
            return;
        }

        if (oldValue is not null)
        {
            var behavior = entry.Behaviors.FirstOrDefault(static x => x is EntryBindBehavior);
            if (behavior is not null)
            {
                entry.Behaviors.Remove(behavior);
            }
        }

        if (newValue is not null)
        {
            entry.Behaviors.Add(new EntryBindBehavior());
        }
    }

    private sealed class EntryBindBehavior : BehaviorBase<Entry>
    {
        private bool updating;

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);

            var controller = GetController(bindable);
            bindable.Completed += BindableOnCompleted;
            bindable.TextChanged += BindableOnTextChanged;
            controller.FocusRequested += ControllerOnFocusRequested;
            controller.PropertyChanged += ControllerOnPropertyChanged;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            var controller = GetController(bindable);
            bindable.Completed -= BindableOnCompleted;
            bindable.TextChanged -= BindableOnTextChanged;
            controller.FocusRequested -= ControllerOnFocusRequested;
            controller.PropertyChanged -= ControllerOnPropertyChanged;

            base.OnDetachingFrom(bindable);
        }

        private void ControllerOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var entry = AssociatedObject;
            if (entry is null)
            {
                return;
            }

            if (e.PropertyName == nameof(EntryController.Text))
            {
                var controller = GetController(entry);
                updating = true;
                entry.Text = controller.Text;
                updating = false;
            }
            else if (e.PropertyName == nameof(EntryController.Enable))
            {
                var controller = GetController(entry);
                entry.IsEnabled = controller.Enable;
            }
        }

        private void ControllerOnFocusRequested(object? sender, EventArgs e)
        {
            AssociatedObject?.Focus();
        }

        private void BindableOnTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (updating)
            {
                return;
            }

            var entry = (Entry)sender!;
            var controller = GetController(entry);
            controller.Text = e.NewTextValue;
        }

        private static void BindableOnCompleted(object? sender, EventArgs e)
        {
            var entry = (Entry)sender!;
            var controller = GetController(entry);
            var ice = new EntryCompleteEvent();
            controller.HandleCompleted(ice);
            if (!ice.HasError)
            {
                ElementHelper.MoveFocusInRoot(entry, true);
            }
        }
    }
}
