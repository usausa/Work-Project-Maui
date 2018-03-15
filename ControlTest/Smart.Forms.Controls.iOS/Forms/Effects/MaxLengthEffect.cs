[assembly: Xamarin.Forms.ExportEffect(typeof(Smart.Forms.Effects.MaxLengthPlatformEffect), "MaxLengthEffect")]

namespace Smart.Forms.Effects
{
    using Foundation;
    using UIKit;

    using Xamarin.Forms.Platform.iOS;

    public class MaxLengthPlatformEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            if (Control is UITextField textField)
            {
                textField.ShouldChangeCharacters += ShouldChangeCharacters;
            }
        }

        protected override void OnDetached()
        {
            if (Control is UITextField textField)
            {
                // ReSharper disable once DelegateSubtraction
                textField.ShouldChangeCharacters -= ShouldChangeCharacters;
            }
        }

        private bool ShouldChangeCharacters(UITextField field, NSRange range, string replacementString)
        {
            var newLength = field.Text.Length + replacementString.Length - range.Length;
            return newLength <= MaxLengthEffect.GetMaxLength(Element);
        }
    }
}
