namespace AnimationTest
{
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Xamarin.Forms;

    public static class Animations
    {
        public static Task<bool> Slide(AbsoluteLayout container, View oldView, View newView, uint length = 250U)
        {
            var width = container.Width;

            var tcs = new TaskCompletionSource<bool>();
            newView.Animate(
                "Slide",
                x =>
                {
                    var oldMargin = (int)(x * width);
                    var newMargin = width - oldMargin;
                    oldView.Margin = new Thickness(-oldMargin, 0, oldMargin, 0);
                    newView.Margin = new Thickness(newMargin, 0, -newMargin, 0);

                },
                16U,
                length,
                Easing.Linear,
                (v, c) =>
                {
                    Debug.WriteLine("Animation end");
                    tcs.SetResult(c);
                });

            return tcs.Task;
        }
    }
}
