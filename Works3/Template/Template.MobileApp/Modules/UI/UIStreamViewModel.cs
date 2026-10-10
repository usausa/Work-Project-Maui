namespace Template.MobileApp.Modules.UI;

public sealed partial class UIStreamViewModel : AppViewModelBase
{
    // トップの作品を次へ送るまでの秒数 (手で送った後も数え直す)
    private const int AdvanceSeconds = 5;

    private readonly IDispatcherTimer timer;

    private int idleSeconds;

    [Scope]
    [ObservableProperty]
    public partial UIStreamContext Context { get; set; } = default!;

    [ObservableProperty]
    public partial IReadOnlyList<UIStreamWork> Featured { get; set; } = [];

    [ObservableProperty]
    public partial int FeaturedPosition { get; set; }

    [ObservableProperty]
    public partial IReadOnlyList<UIStreamWork> Continue { get; set; } = [];

    [ObservableProperty]
    public partial IReadOnlyList<UIStreamSection> Sections { get; set; } = [];

    public IObserveCommand DetailCommand { get; }

    public IObserveCommand MyListCommand { get; }

    public IObserveCommand CurrentChangedCommand { get; }

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public UIStreamViewModel(IDispatcher dispatcher)
    {
        DetailCommand = MakeAsyncCommand<UIStreamWork>(OpenAsync);
        MyListCommand = MakeDelegateCommand<UIStreamWork>(static x => x.InMyList = !x.InMyList);
        CurrentChangedCommand = MakeDelegateCommand(() => idleSeconds = 0);

        timer = dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        Disposables.Add(timer.TickAsObservable().Subscribe(_ => Advance()));
    }

    //--------------------------------------------------------------------------------
    // Navigation
    //--------------------------------------------------------------------------------

    // 詳細から戻ったときは、トップの位置とマイリストを残したまま
    public override Task OnNavigatingToAsync(INavigationContext context)
    {
        if (!context.Attribute.IsRestore())
        {
            var works = Context.Works;
            Featured = works.Where(static x => x.Work.IsFeatured).ToArray();
            Continue = works.Where(static x => x.HasProgress).ToArray();
            Sections =
            [
                new() { Shelf = StreamShelf.TopRated, Items = works.OrderByDescending(static x => x.Work.Rating).ToArray(), Delay = 0 },
                new() { Shelf = StreamShelf.Original, Items = works.Where(static x => x.Work.IsOriginal).ToArray(), Delay = 80 },
                new() { Shelf = StreamShelf.Trending, Items = works.OrderByDescending(static x => x.Work.Year).ThenByDescending(static x => x.Work.Match).ToArray(), Delay = 160 },
                new() { Shelf = StreamShelf.Action, Items = works.Where(static x => x.Work.Genre is StreamGenre.Action or StreamGenre.Sf or StreamGenre.Fantasy).ToArray(), Delay = 240 }
            ];
        }

        return Task.CompletedTask;
    }

    public override Task OnNavigatedToAsync(INavigationContext context)
    {
        idleSeconds = 0;
        timer.Start();
        return Task.CompletedTask;
    }

    public override Task OnNavigatingFromAsync(INavigationContext context)
    {
        timer.Stop();
        return Task.CompletedTask;
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.UIMenu1);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();

    //--------------------------------------------------------------------------------
    // Operation
    //--------------------------------------------------------------------------------

    // 送りの途中で画面を離れると、戻ったときに位置と表示がずれるので先に止める
    private Task OpenAsync(UIStreamWork work)
    {
        timer.Stop();
        Context.Open(work);
        return Navigator.PushAsync(ViewId.UIStreamDetail);
    }

    //--------------------------------------------------------------------------------
    // Event
    //--------------------------------------------------------------------------------

    private void Advance()
    {
        idleSeconds++;
        if (idleSeconds >= AdvanceSeconds)
        {
            idleSeconds = 0;
            FeaturedPosition = (FeaturedPosition + 1) % Featured.Count;
        }
    }
}
