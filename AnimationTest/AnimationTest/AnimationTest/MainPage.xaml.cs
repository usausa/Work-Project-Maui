namespace AnimationTest
{
    using System;
    using System.Threading.Tasks;

    using Xamarin.Forms;

    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnTest1Clicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Before Animate");
            await AnimatePop();
            System.Diagnostics.Debug.WriteLine("After Animate");
        }

        private async void OnTest2Clicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Before Animate");
            await AnimateClose();
            await AnimatePop();
            System.Diagnostics.Debug.WriteLine("After Animate");
        }

        private async void OnTest3Clicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Before Animate");
            await AnimateAppear();
            System.Diagnostics.Debug.WriteLine("After Animate");
        }

        private async void OnTest4Clicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Before Animate");
            await AnimateDisappear();
            System.Diagnostics.Debug.WriteLine("After Animate");
        }

        private Task AnimatePop()
        {
            var tcs = new TaskCompletionSource<bool>();

            var animation = new Animation();
            animation.WithConcurrent(x => Container.Opacity = x, 0.0, 1.0);
            animation.WithConcurrent(x => Container.Scale = x, 0.8, 1.0);

            Container.Animate("AnimatePop", animation, 16U, 250U, null, (v, c) => tcs.SetResult(c));

            return tcs.Task;
        }

        private Task AnimateClose()
        {
            var tcs = new TaskCompletionSource<bool>();

            var animation = new Animation();
            animation.WithConcurrent(x => Container.Opacity = x, 1.0, 0.0);
            animation.WithConcurrent(x => Container.Scale = x, 1.0, 0.8);

            Container.Animate("AnimateClose", animation, 16U, 100U, null, (v, c) => tcs.SetResult(c));

            return tcs.Task;
        }

        private Task AnimateAppear()
        {
            var tcs = new TaskCompletionSource<bool>();

            var animation = new Animation();
            animation.WithConcurrent(x => Container.Opacity = x, 0.0, 1.0);
            animation.WithConcurrent(x => Container.TranslationX = x, -Container.Width, 0.0);

            Container.Animate("AnimatePop", animation, 16U, 600U, Easing.Linear, (v, c) => tcs.SetResult(c));

            return tcs.Task;
        }

        private Task AnimateDisappear()
        {
            var tcs = new TaskCompletionSource<bool>();

            var animation = new Animation();
            animation.WithConcurrent(x => Container.Opacity = x, 1.0, 0.0);
            animation.WithConcurrent(x => Container.TranslationX = x, 0.0, -Container.Width);

            Container.Animate("AnimatePop", animation, 16U, 600U, Easing.Linear, (v, c) => tcs.SetResult(c));

            return tcs.Task;
        }
    }
}
