[assembly: Xamarin.Forms.ExportEffect(typeof(Smart.Forms.Effects.MaxLengthPlatformEffect), "MaxLengthEffect")]

namespace Smart.Forms.Effects
{
    using Android.Text;
    using Android.Widget;

    using Xamarin.Forms.Platform.Android;

    public class MaxLengthPlatformEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            var length = MaxLengthEffect.GetMaxLength(Element);

            var textView = Control as TextView;
            textView?.SetFilters(new IInputFilter[]
            {
                new InputFilterLengthFilter(length)
            });
        }

        protected override void OnDetached()
        {
        }
    }
}