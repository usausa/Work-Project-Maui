namespace BindingTest
{
    using System.ComponentModel;

    using Smart.Forms.Interactivity;

    using Xamarin.Forms;

    public class ViewPropertyBehavior : BehaviorBase<ContentView>
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

            System.Diagnostics.Debug.WriteLine($"Content changed {AssociatedObject.Content.GetHashCode()}");

            var element = AssociatedObject.Parent;
            while (element != null)
            {
                if (element.BindingContext is ViewPropertyModel vm)
                {
                    vm.Title = ViewProperty.GetTitle(AssociatedObject.Content);
                    break;
                }

                element = element.Parent;
            }
        }
    }
}