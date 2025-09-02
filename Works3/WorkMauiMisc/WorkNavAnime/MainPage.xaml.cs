namespace WorkNavAnime;

public partial class MainPage : ContentPage
{
    private ContentView? currentPage;

    private bool navigating;

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnPushClicked(object? sender, EventArgs e) => Navigate(new Page2(), new PushTransition());

    private void OnPopClicked(object? sender, EventArgs e) => Navigate(new Page1(), new PopTransition());

    private void OnOpenClicked(object? sender, EventArgs e) => Navigate(new Page2(), new DialogOpenTransition());

    private void OnCloseClicked(object? sender, EventArgs e) => Navigate(new Page1(), new DialogCloseTransition());

    private void OnUpClicked(object? sender, EventArgs e) => Navigate(new Page2(), new BottomUpTransition());

    private void OnDownClicked(object? sender, EventArgs e) => Navigate(new Page1(), new BottomDownTransition());

    private async void Navigate(ContentView newPage, INavigationEffect effect)
    {
        if (navigating || currentPage?.GetType() == newPage.GetType())
        {
            return;
        }

        navigating = true;

        var oldPage = currentPage;

        Container.Children.Add(newPage);

        // [MEMO] 実際の実装ではオーダーをどうするかは要検討
        if (oldPage != null)
        {
            effect.ReOrder(newPage, oldPage);
        }

        var inTask = effect.AnimateInAsync(newPage);
        var outTask = oldPage != null ? effect.AnimateOutAsync(oldPage) : Task.CompletedTask;
        await Task.WhenAll(inTask, outTask);

        if (oldPage != null)
        {
            Container.Children.Remove(oldPage);
        }

        // Reset
        newPage.ZIndex = 0;

        newPage.TranslationX = 0;
        newPage.TranslationY = 0;
        newPage.Scale = 1;
        newPage.Opacity = 1;

        currentPage = newPage;
        navigating = false;
    }
}

//----------------------------------------------------------------

// TODO 不要箇所再確認、呼び出し側でのリセット？

public interface INavigationEffect
{
    void ReOrder(ContentView newPage, ContentView oldPage);

    Task AnimateInAsync(ContentView page);

    Task AnimateOutAsync(ContentView page);
}

public class PushTransition : INavigationEffect
{
    private const int Duration = 250;

    public void ReOrder(ContentView newPage, ContentView oldPage)
    {
        // 既定: newPageは追加順で最前面。調整不要。
    }

    public Task AnimateInAsync(ContentView page)
    {
        var w = page.Parent is View parentView ? parentView.Width : 0;
        page.Opacity = 0;
        page.TranslationX = w;
        return Task.WhenAll(
            page.FadeTo(1, Duration, Easing.CubicOut),
            page.TranslateTo(0, 0, Duration, Easing.CubicOut)
        );
    }

    public Task AnimateOutAsync(ContentView page)
    {
        var w = page.Parent is View parentView ? parentView.Width : 0;
        return Task.WhenAll(
            page.FadeTo(0, Duration, Easing.CubicIn),
            page.TranslateTo(-w, 0, Duration, Easing.CubicOut)
        );
    }
}

public class PopTransition : INavigationEffect
{
    private const int Duration = 250;

    public void ReOrder(ContentView newPage, ContentView oldPage)
    {
        oldPage.ZIndex = (newPage.ZIndex >= 0 ? newPage.ZIndex : 0) + 1;
    }

    public Task AnimateInAsync(ContentView page)
    {
        var w = page.Parent is View parentView ? parentView.Width : 0;
        page.Opacity = 0;
        page.TranslationX = -w;
        return Task.WhenAll(
            page.FadeTo(1, Duration, Easing.CubicOut),
            page.TranslateTo(0, 0, Duration, Easing.CubicOut)
        );
    }

    public Task AnimateOutAsync(ContentView page)
    {
        var w = page.Parent is View parentView ? parentView.Width : 0;
        return Task.WhenAll(
            page.FadeTo(0, Duration, Easing.CubicOut),
            page.TranslateTo(w, 0, Duration, Easing.CubicOut)
        );
    }
}

public class DialogOpenTransition : INavigationEffect
{
    private const int Duration = 250;

    public void ReOrder(ContentView newPage, ContentView oldPage)
    {
        // Z-indexの調整は不要
    }

    public Task AnimateInAsync(ContentView page)
    {
        page.Opacity = 0;
        page.Scale = 0.8;
        return Task.WhenAll(
            page.FadeTo(1, Duration, Easing.CubicOut),
            page.ScaleTo(1.0, Duration, Easing.CubicOut)
        );
    }

    public Task AnimateOutAsync(ContentView page)
    {
        return Task.CompletedTask;
    }
}

public class DialogCloseTransition : INavigationEffect
{
    private const int Duration = 250;

    public void ReOrder(ContentView newPage, ContentView oldPage)
    {
        oldPage.ZIndex = (newPage.ZIndex >= 0 ? newPage.ZIndex : 0) + 1;
    }

    public Task AnimateInAsync(ContentView page)
    {
        return Task.CompletedTask;
    }

    public Task AnimateOutAsync(ContentView page)
    {
        return Task.WhenAll(
            page.FadeTo(0, Duration, Easing.CubicIn),
            page.ScaleTo(0.8, Duration, Easing.CubicOut)
        );
    }
}

public class BottomUpTransition : INavigationEffect
{
    private const int Duration = 250;

    public void ReOrder(ContentView newPage, ContentView oldPage)
    {
        // Z-indexの調整は不要
    }

    public Task AnimateInAsync(ContentView page)
    {
        var h = page.Parent is View parentView ? parentView.Height : 0;

        page.Opacity = 0;
        page.TranslationY = h;
        return Task.WhenAll(
            page.FadeTo(1, Duration, Easing.CubicOut),
            page.TranslateTo(0, 0, Duration, Easing.CubicOut)
        );
    }

    public Task AnimateOutAsync(ContentView page)
    {
        return Task.CompletedTask;
    }
}

public class BottomDownTransition : INavigationEffect
{
    private const int Duration = 250;

    public void ReOrder(ContentView newPage, ContentView oldPage)
    {
        oldPage.ZIndex = (newPage.ZIndex >= 0 ? newPage.ZIndex : 0) + 1;
    }

    public Task AnimateInAsync(ContentView page)
    {
        return Task.CompletedTask;
    }

    public Task AnimateOutAsync(ContentView page)
    {
        var h = page.Parent is View parentView ? parentView.Height : 0;

        return Task.WhenAll(
            page.FadeTo(0, Duration, Easing.CubicIn),
            page.TranslateTo(0, h, Duration, Easing.CubicOut)
        );
    }
}