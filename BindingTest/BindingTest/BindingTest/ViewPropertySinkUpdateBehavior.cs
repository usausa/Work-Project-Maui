namespace BindingTest
{
    using System.ComponentModel;

    using Smart.Forms.Interactivity;

    using Xamarin.Forms;

    public class ViewPropertySinkUpdateBehavior : BehaviorBase<ContentView>
    {
        protected override void OnAttachedTo(ContentView bindable)
        {
            base.OnAttachedTo(bindable);

            bindable.PropertyChanged += BindableOnPropertyChanged;
        }

        protected override void OnDetachingFrom(ContentView bindable)
        {
            bindable.PropertyChanged -= BindableOnPropertyChanged;

            base.OnDetachingFrom(bindable);
        }

        private void BindableOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName != nameof(ContentView.Content))
            {
                return;
            }

            var sync = ViewProperty.GetSink(AssociatedObject);
            if (sync != null)
            {
                sync.Title = ViewProperty.GetTitle(AssociatedObject.Content);
            }
        }
    }
}