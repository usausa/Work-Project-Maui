namespace WorkNavAnime;

public partial class MainPage : ContentPage
{
    private ContentView? currentPage;

    private bool navigating;

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnPushClicked(object? sender, EventArgs e) => Navigate(new Page1(), new PushTransition());

    private void OnPopClicked(object? sender, EventArgs e) => Navigate(new Page2(), new PopTransition());

    private void OnOpenClicked(object? sender, EventArgs e) => Navigate(new Page1(), new DialogOpenTransition());

    private void OnCloseClicked(object? sender, EventArgs e) => Navigate(new Page2(), new DialogCloseTransition());

    private void OnUpClicked(object? sender, EventArgs e) => Navigate(new Page1(), new BottomUpTransition());

    private void OnDownClicked(object? sender, EventArgs e) => Navigate(new Page2(), new BottomDownTransition());

    private const int CloseDuration = 100;
    private const int OpenDuration = 250;

    private async void Navigate(ContentView newPage, INavigationEffect effect)
    {
        if (navigating || currentPage?.GetType() == newPage.GetType())
        {
            return;
        }

        navigating = true;

        if (currentPage != null)
        {
            var oldPage = currentPage;

            await effect.AnimateOutAsync(oldPage);
            Container.Children.Remove(oldPage);
        }

        Container.Children.Add(newPage);

        // 新しい画面の初期状態を設定
        newPage.Opacity = 0;
        newPage.TranslationX = 0;
        newPage.TranslationY = 0;
        newPage.Scale = 1;

        await effect.AnimateInAsync(newPage);

        // Stackされるなら元画面もリセット

        currentPage = newPage;
        navigating = false;
    }
}

//----------------------------------------------------------------

public interface INavigationEffect
{
    Task AnimateInAsync(ContentView page);

    Task AnimateOutAsync(ContentView page);
}

public class PushTransition : INavigationEffect
{
    private const int OpenDuration = 500;
    private const int CloseDuration = 200;

    public Task AnimateInAsync(ContentView page)
    {
        page.TranslationX = page.Parent is View parentView ? parentView.Width : 0;
        return Task.WhenAll(
            page.FadeTo(1, OpenDuration, Easing.CubicIn),
            page.TranslateTo(0, 0, OpenDuration, Easing.CubicOut)
        );
    }

    public Task AnimateOutAsync(ContentView page)
    {
        return Task.WhenAll(
            page.FadeTo(0, CloseDuration, Easing.CubicIn),
            page.TranslateTo(-(page.Width / 4), 0, CloseDuration, Easing.CubicOut)
        );
    }
}

public class PopTransition : INavigationEffect
{
    private const int OpenDuration = 500;
    private const int CloseDuration = 200;

    public Task AnimateInAsync(ContentView page)
    {
        page.TranslationX = page.Parent is View parentView ? -parentView.Width : 0;
        return Task.WhenAll(
            page.FadeTo(1, OpenDuration, Easing.CubicIn),
            page.TranslateTo(0, 0, OpenDuration, Easing.CubicOut)
        );
    }

    public Task AnimateOutAsync(ContentView page)
    {
        return Task.WhenAll(
            page.FadeTo(0, CloseDuration, Easing.CubicIn),
            page.TranslateTo(page.Width / 4, 0, CloseDuration, Easing.CubicOut)
        );
    }
}
public class DialogOpenTransition : INavigationEffect
{
    private const int OpenDuration = 500;
    private const int CloseDuration = 200;

    public Task AnimateInAsync(ContentView page)
    {
        page.Scale = 0.8;
        return Task.WhenAll(
            page.FadeTo(1, OpenDuration, Easing.CubicIn),
            page.ScaleTo(1.0, OpenDuration, Easing.CubicOut)
        );
    }

    public Task AnimateOutAsync(ContentView page)
    {
        return Task.WhenAll(
            page.FadeTo(0, CloseDuration, Easing.CubicIn),
            page.ScaleTo(0.8, CloseDuration, Easing.CubicOut)
        );
    }
}

public class DialogCloseTransition : INavigationEffect
{
    //private const int OpenDuration = 250;
    private const int CloseDuration = 200;

    // 画面を開く際のアニメーション (新しいページが出現)
    public Task AnimateInAsync(ContentView page)
    {
        // 閉じるアニメーションなので、このメソッドは通常使用されません
        // 必要に応じて、何もしないか、あるいは瞬時に表示するロジックを実装
        page.Opacity = 1;
        page.Scale = 1;
        return Task.CompletedTask;
    }

    // 画面を閉じる際のアニメーション (現在のページが消える)
    public Task AnimateOutAsync(ContentView page)
    {
        return Task.WhenAll(
            page.FadeTo(0, CloseDuration, Easing.CubicIn),
            page.ScaleTo(0.8, CloseDuration, Easing.CubicOut) // 縮小しながら消える
        );
    }
}

public class BottomUpTransition : INavigationEffect
{
    private const int OpenDuration = 250;
    private const int CloseDuration = 100;

    public Task AnimateInAsync(ContentView page)
    {
        page.TranslationY = page.Parent is View parentView ? parentView.Height : 0;
        return Task.WhenAll(
            page.FadeTo(1, OpenDuration, Easing.CubicIn),
            page.TranslateTo(0, 0, OpenDuration, Easing.CubicOut)
        );
    }

    public Task AnimateOutAsync(ContentView page)
    {
        return Task.WhenAll(
            page.FadeTo(0, CloseDuration, Easing.CubicIn),
            page.TranslateTo(0, page.Height, CloseDuration, Easing.CubicOut)
        );
    }
}

public class BottomDownTransition : INavigationEffect
{
    //private const int OpenDuration = 250;
    private const int CloseDuration = 100;

    // 画面を開く際のアニメーション (新しいページが出現)
    public Task AnimateInAsync(ContentView page)
    {
        // 閉じるアニメーションなので、このメソッドは通常使用されません
        page.Opacity = 1;
        page.TranslationY = 0;
        return Task.CompletedTask;
    }

    // 画面を閉じる際のアニメーション (現在のページが消える)
    public Task AnimateOutAsync(ContentView page)
    {
        return Task.WhenAll(
            page.FadeTo(0, CloseDuration, Easing.CubicIn),
            page.TranslateTo(0, page.Parent is View parentView ? parentView.Height : 0, CloseDuration, Easing.CubicOut) // 下にスライドして消える
        );
    }
}