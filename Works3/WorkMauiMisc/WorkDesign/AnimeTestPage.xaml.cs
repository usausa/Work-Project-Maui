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

        animation.WithConcurrent(v => AnimatedBox.TranslationX = v, 0, 100, Easing.CubicOut);

        animation.Commit(AnimatedBox, "Test", 16, 1000);
        // TODO anime
    }

    private void OnTest2Clicked(object? sender, EventArgs e)
    {

    }

    private void OnTest3Clicked(object? sender, EventArgs e)
    {

    }

    private void OnTest4Clicked(object? sender, EventArgs e)
    {

    }
}