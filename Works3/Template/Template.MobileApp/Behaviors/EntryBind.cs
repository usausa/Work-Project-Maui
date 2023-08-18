namespace Template.MobileApp.Behaviors;

using Smart.Maui.Interactivity;

using Template.MobileApp.Helpers;
using Template.MobileApp.Messaging;

public static class EntryBind
{
    public static readonly BindableProperty MessengerProperty = BindableProperty.CreateAttached(
        "Messenger",
        typeof(IEntryMessenger),
        typeof(EntryBind),
        null,
        propertyChanged: BindChanged);

    public static IEntryMessenger GetMessenger(BindableObject bindable) =>
        (IEntryMessenger)bindable.GetValue(MessengerProperty);

    public static void SetMessenger(BindableObject bindable, IEntryMessenger value) =>
        bindable.SetValue(MessengerProperty, value);

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
        private IEntryMessenger? controller;

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);

            controller = GetMessenger(bindable);
            if (controller is not null)
            {
                controller.FocusRequested += MessengerOnFocusRequested;
            }

            bindable.Completed += BindableOnCompleted;

            bindable.SetBinding(
                Entry.TextProperty,
                new Binding(nameof(IEntryMessenger.Text), source: controller));
            bindable.SetBinding(
                VisualElement.IsEnabledProperty,
                new Binding(nameof(IEntryMessenger.Enable), source: controller));
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            if (controller is not null)
            {
                controller.FocusRequested -= MessengerOnFocusRequested;
            }

            bindable.Completed -= BindableOnCompleted;

            bindable.RemoveBinding(Entry.TextProperty);
            bindable.RemoveBinding(VisualElement.IsEnabledProperty);

            base.OnDetachingFrom(bindable);
        }

        private void MessengerOnFocusRequested(object? sender, EventArgs e)
        {
            AssociatedObject?.Focus();
        }

        private void BindableOnCompleted(object? sender, EventArgs e)
        {
            if (controller is null)
            {
                return;
            }

            var entry = (Entry)sender!;
            var ice = new EntryCompleteEvent();
            controller.HandleCompleted(ice);
            if (!ice.HasError)
            {
                ElementHelper.MoveFocusInRoot(entry, true);
            }
        }
    }
}
