namespace AnimationTest
{
    using System.Threading.Tasks;
    using Xamarin.Forms;

    public static class Animations
    {
        public static Task<bool> Slide(VisualElement element)
        {
            var width = element.Width;
            var tcs = new TaskCompletionSource<bool>();
            element.Animate("Slide", x =>
            {

            }, 16U, 1000U, Easing.Linear, (v, c) => tcs.SetResult(c));
            return tcs.Task;
        }
    }
}
