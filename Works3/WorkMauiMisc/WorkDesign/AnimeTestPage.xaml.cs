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

        animation.WithConcurrent(v => AnimatedBox.TranslationX = v, 0, 180, Easing.CubicOut);

        animation.Commit(AnimatedBox, "Test", 16, 1000);
        // TODO anime
    }
}