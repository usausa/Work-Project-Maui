namespace WorkDesign;

public partial class AnimeTestPage : ContentPage
{
	public AnimeTestPage()
	{
		InitializeComponent();
	}

    private void OnTest1Clicked(object? sender, EventArgs e)
    {
        var animation = new Animation();

        animation.WithConcurrent(v => AnimatedBox.TranslationX = v, 0, 150, Easing.CubicOut);

        animation.Commit(AnimatedBox, "TranslationX", 16, 2000);
    }

    private void OnTest2Clicked(object? sender, EventArgs e)
    {
        var animation = new Animation();

        animation.WithConcurrent(v => AnimatedBox.TranslationY = v, 0, 150, Easing.CubicOut);

        animation.Commit(AnimatedBox, "TranslationY", 16, 2000);
    }

    private void OnTest3Clicked(object? sender, EventArgs e)
    {
        var animation = new Animation();

        animation.WithConcurrent(v => AnimatedBox.TranslationX = v, 0, 100, Easing.CubicOut);
        animation.WithConcurrent(v => AnimatedBox.TranslationY = v, 0, -40, new Easing(t => Math.Sin(t * Math.PI))); // 上に持ち上げて戻る

        animation.Commit(AnimatedBox, "Mixed", 16, 1000);
    }

    private async void OnTest4Clicked(object? sender, EventArgs e)
    {
        await AnimatedBox.RotateTo(360, 2000, Easing.Linear);
        AnimatedBox.Rotation = 0;
    }

    private Animation? reuseAnimation;

    private void OnTest5Clicked(object? sender, EventArgs e)
    {
        reuseAnimation ??= new Animation();

        reuseAnimation.WithConcurrent(v => AnimatedBox.TranslationX = v, 0, 150, Easing.CubicOut);

        reuseAnimation.Commit(AnimatedBox, "TranslationX", 16, 1000);
    }

    private async void OnTest6Clicked(object? sender, EventArgs e)
    {
        await AnimatedBox.RelRotateTo(90, 400, Easing.SinInOut);
    }

    private async void OnTest7Clicked(object? sender, EventArgs e)
    {
        await AnimatedBox.ScaleTo(1.3, 250, Easing.CubicOut);
        await AnimatedBox.ScaleTo(1.0, 250, Easing.CubicIn);
    }

    private async void OnTest8Clicked(object? sender, EventArgs e)
    {
        for (int i = 0; i < 3; i++)
        {
            await AnimatedBox.FadeTo(0.2, 180, Easing.Linear);
            await AnimatedBox.FadeTo(1.0, 180, Easing.Linear);
        }
    }

    private async void OnTest9Clicked(object? sender, EventArgs e)
    {
        // Parallel
        var t1 = AnimatedBox.TranslateTo(120, -40, 600, Easing.SinOut);
        var t2 = AnimatedBox.RotateTo(270, 600, Easing.CubicInOut);
        var t3 = AnimatedBox.ScaleTo(1.4, 600, Easing.CubicOut);
        await Task.WhenAll(t1, t2, t3);

        await AnimatedBox.ScaleTo(1.0, 300);
        await AnimatedBox.RotateTo(0, 300);
        await AnimatedBox.TranslateTo(0, 0, 300);
    }

    private async void OnTest10Clicked(object? sender, EventArgs e)
    {
        // Sequence
        await AnimatedBox.TranslateTo(0, -80, 400, Easing.CubicOut);
        await AnimatedBox.ScaleTo(0.5, 300, Easing.SinInOut);
        await AnimatedBox.RotateTo(180, 400, Easing.SinInOut);

        await AnimatedBox.RotateTo(0, 300);
        await AnimatedBox.ScaleTo(1.0, 300);
        await AnimatedBox.TranslateTo(0, 0, 400, Easing.BounceOut);
    }

    private async void OnTest11Clicked(object? sender, EventArgs e)
    {
        // 擬似バネ: 位置 + 縮小/拡大
        double target = 150;
        await Task.WhenAll(
            AnimatedBox.TranslateTo(target, 0, 500, Easing.SinOut),
            AnimatedBox.ScaleTo(0.85, 300, Easing.CubicOut)
        );
        await AnimatedBox.ScaleTo(1.05, 250, Easing.CubicIn);

        await AnimatedBox.ScaleTo(1.0, 200);
        await AnimatedBox.TranslateTo(0, 0, 400, Easing.SpringOut);
    }

    private async void OnTest12Clicked(object? sender, EventArgs e)
    {
        // 変化の仕方が異なる
        await AnimatedBox.ScaleTo(1.6, 600, Easing.SpringOut);
        await AnimatedBox.ScaleTo(1.0, 500, Easing.SpringIn);
    }

    private async void OnTest13Clicked(object? sender, EventArgs e)
    {
        await AnimatedBox.ScaleTo(1.3, 250).ContinueWith(async t =>
        {
            await AnimatedBox.RotateTo(90, 250);
            await AnimatedBox.RotateTo(180, 250);
        }).Unwrap().ContinueWith(async t =>
        {
            await AnimatedBox.TranslateTo(100, -60, 400, Easing.SinOut);
        }).Unwrap();

        await AnimatedBox.ScaleTo(1.0, 250);
        await AnimatedBox.RotateTo(0, 250);
        await AnimatedBox.TranslateTo(0, 0, 300);

    }
}