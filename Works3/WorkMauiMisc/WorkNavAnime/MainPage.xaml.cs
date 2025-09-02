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

        var animationTasks = new List<Task>
        {
            effect.AnimateInAsync(newPage)
        };
        if (oldPage != null)
        {
            animationTasks.Add(effect.AnimateOutAsync(oldPage));
        }

        await Task.WhenAll(animationTasks);

        if (oldPage != null)
        {
            Container.Children.Remove(oldPage);
        }

        currentPage = newPage;
        navigating = false;
    }
}

//----------------------------------------------------------------

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
        // Children.Add()のデフォルトの挙動を利用するため、Z-indexの調整は不要
    }

    public Task AnimateInAsync(ContentView page)
    {
        page.TranslationX = page.Parent is View parentView ? parentView.Width : 0;
        return Task.WhenAll(
            page.FadeTo(1, Duration, Easing.CubicIn),
            page.TranslateTo(0, 0, Duration, Easing.CubicOut)
        );
    }

    public Task AnimateOutAsync(ContentView page)
    {
        return Task.WhenAll(
            page.FadeTo(0, Duration, Easing.CubicIn),
            page.TranslateTo(-(page.Width), 0, Duration, Easing.CubicOut)
        );
    }
}

public class PopTransition : INavigationEffect
{
    private const int Duration = 250;

    public void ReOrder(ContentView newPage, ContentView oldPage)
    {
        oldPage.ZIndex = newPage.ZIndex + 1;
    }

    public Task AnimateInAsync(ContentView page)
    {
        page.TranslationX = page.Parent is View parentView ? -parentView.Width : 0;
        return Task.WhenAll(
            page.FadeTo(1, Duration, Easing.CubicIn),
            page.TranslateTo(0, 0, Duration, Easing.CubicOut)
        );
    }

    public Task AnimateOutAsync(ContentView page)
    {
        return Task.WhenAll(
            page.FadeTo(0, Duration, Easing.CubicIn),
            page.TranslateTo(page.Width, 0, Duration, Easing.CubicOut)
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
        page.Scale = 0.8;
        return Task.WhenAll(
            page.FadeTo(1, Duration, Easing.CubicIn),
            page.ScaleTo(1.0, Duration, Easing.CubicOut)
        );
    }

    public Task AnimateOutAsync(ContentView page)
    {
        page.Opacity = 1;
        page.TranslationY = 0;
        return Task.CompletedTask;
    }
}

public class DialogCloseTransition : INavigationEffect
{
    private const int Duration = 250;

    public void ReOrder(ContentView newPage, ContentView oldPage)
    {
        oldPage.ZIndex = newPage.ZIndex + 1;
    }

    public Task AnimateInAsync(ContentView page)
    {
        page.Opacity = 1;
        page.TranslationY = 0;
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
        page.TranslationY = page.Parent is View parentView ? parentView.Height : 0;
        return Task.WhenAll(
            page.FadeTo(1, Duration, Easing.CubicIn),
            page.TranslateTo(0, 0, Duration, Easing.CubicOut)
        );
    }

    public Task AnimateOutAsync(ContentView page)
    {
        return page.FadeTo(0, Duration, Easing.CubicIn);
    }
}

public class BottomDownTransition : INavigationEffect
{
    private const int Duration = 250;

    public void ReOrder(ContentView newPage, ContentView oldPage)
    {
        oldPage.ZIndex = newPage.ZIndex + 1;
    }

    public Task AnimateInAsync(ContentView page)
    {
        page.Opacity = 1;
        page.TranslationY = 0;
        return Task.CompletedTask;
    }

    public Task AnimateOutAsync(ContentView page)
    {
        return Task.WhenAll(
            page.FadeTo(0, Duration, Easing.CubicIn),
            page.TranslateTo(0, page.Parent is View parentView ? parentView.Height : 0, Duration, Easing.CubicOut)
        );
    }
}