namespace Template.MobileApp.Modules.Control;

using Template.MobileApp.Models.Control;

// CollectionView をカード一覧として使う例。行タップで選択、右端で展開、並替パネルで複数キーのソート
public sealed partial class ControlCardListViewModel : AppViewModelBase
{
    private const int VisitCount = 40;

    private const int MaxSortKeys = 3;

    private readonly IDialog dialog;

    private readonly List<VisitCard> source = [];

    // 並替パネルで選んだ順 (先頭が第 1 キー)
    private readonly List<VisitSortKey> activeKeys = [];

    public ObservableCollection<VisitCard> Cards { get; } = [];

    public IReadOnlyList<VisitSortKey> SortKeys { get; } =
    [
        new("予定", static (x, y) => x.ScheduledAt.CompareTo(y.ScheduledAt)),
        new("名前", static (x, y) => StringComparer.CurrentCulture.Compare(x.Name, y.Name)),
        new("状態", static (x, y) => x.Status.CompareTo(y.Status)),
        new("担当", static (x, y) => StringComparer.CurrentCulture.Compare(x.Staff, y.Staff)),
        new("区分", static (x, y) => StringComparer.CurrentCulture.Compare(x.Category, y.Category)),
        new("コード", static (x, y) => String.CompareOrdinal(x.Code, y.Code))
    ];

    [ObservableProperty]
    public partial bool IsSortPanelOpen { get; set; }

    [ObservableProperty]
    public partial bool IsAllExpanded { get; set; }

    [ObservableProperty]
    public partial bool IsDescending { get; set; }

    [ObservableProperty]
    public partial int PlannedCount { get; set; }

    [ObservableProperty]
    public partial int VisitedCount { get; set; }

    [ObservableProperty]
    public partial int RevisitCount { get; set; }

    [ObservableProperty]
    public partial int AbsentCount { get; set; }

    [ObservableProperty]
    public partial int SelectedCount { get; set; }

    [ObservableProperty]
    public partial string SortText { get; set; } = string.Empty;

    public IObserveCommand ToggleSelectCommand { get; }

    public IObserveCommand ToggleExpandCommand { get; }

    public IObserveCommand ToggleSortPanelCommand { get; }

    public IObserveCommand SelectSortKeyCommand { get; }

    public IObserveCommand ToggleDirectionCommand { get; }

    public IObserveCommand ToggleExpandAllCommand { get; }

    public IObserveCommand ReloadCommand { get; }

    public ControlCardListViewModel(IDialog dialog)
    {
        this.dialog = dialog;

        ToggleSelectCommand = MakeDelegateCommand<VisitCard>(x =>
        {
            x.IsSelected = !x.IsSelected;
            UpdateSelectedCount();
        });
        ToggleExpandCommand = MakeDelegateCommand<VisitCard>(x => x.IsExpanded = !x.IsExpanded);
        ToggleSortPanelCommand = MakeDelegateCommand(() => IsSortPanelOpen = !IsSortPanelOpen);
        SelectSortKeyCommand = MakeDelegateCommand<VisitSortKey>(SelectSortKey);
        ToggleDirectionCommand = MakeDelegateCommand(ToggleDirection);
        ToggleExpandAllCommand = MakeDelegateCommand(ToggleExpandAll);
        ReloadCommand = MakeDelegateCommand(Load);

        activeKeys.Add(SortKeys[0]);
        UpdateSortKeys();
    }

    public override Task OnNavigatingToAsync(INavigationContext context)
    {
        if (!context.Attribute.IsRestore())
        {
            Load();
        }

        return Task.CompletedTask;
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.ControlMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();

    protected override Task OnNotifyFunction2()
    {
        IsSortPanelOpen = !IsSortPanelOpen;
        return Task.CompletedTask;
    }

    // 未訪問を一括選択 (すべて選択済みなら解除)
    protected override Task OnNotifyFunction3()
    {
        var planned = source.Where(static x => x.Status == VisitStatus.Planned).ToList();
        var select = planned.Any(static x => !x.IsSelected);
        foreach (var card in planned)
        {
            card.IsSelected = select;
        }

        UpdateSelectedCount();
        return Task.CompletedTask;
    }

    protected override async Task OnNotifyFunction4()
    {
        var selected = source.Where(static x => x.IsSelected).ToList();
        if (selected.Count == 0)
        {
            await dialog.InformationAsync("訪問先が選択されていません。");
            return;
        }

        var names = String.Join("\n", selected.Take(5).Select(static x => $"・{x.Code} {x.Name}"));
        var rest = selected.Count > 5 ? $"\n…他 {selected.Count - 5} 件" : string.Empty;
        await dialog.InformationAsync($"{selected.Count} 件を確定します。\n{names}{rest}");
    }

    //--------------------------------------------------------------------------------
    // Operation
    //--------------------------------------------------------------------------------

    private void Load()
    {
        source.Clear();
        source.AddRange(VisitSamples.Create(VisitCount).Select(static x => new VisitCard(x)));
        PlannedCount = source.Count(static x => x.Status == VisitStatus.Planned);
        VisitedCount = source.Count(static x => x.Status == VisitStatus.Visited);
        RevisitCount = source.Count(static x => x.Status == VisitStatus.Revisit);
        AbsentCount = source.Count(static x => x.Status == VisitStatus.Absent);
        IsAllExpanded = false;
        UpdateSelectedCount();
        ApplySort();
    }

    // 第 1 キーの再選択は昇降の反転、それ以外は先頭に積む (最大 3 キー)
    private void SelectSortKey(VisitSortKey key)
    {
        if ((activeKeys.Count > 0) && ReferenceEquals(activeKeys[0], key))
        {
            key.Descending = !key.Descending;
        }
        else
        {
            activeKeys.Remove(key);
            activeKeys.Insert(0, key);
            while (activeKeys.Count > MaxSortKeys)
            {
                activeKeys.RemoveAt(activeKeys.Count - 1);
            }
        }

        UpdateSortKeys();
        ApplySort();
    }

    private void ToggleDirection()
    {
        activeKeys[0].Descending = !activeKeys[0].Descending;
        UpdateSortKeys();
        ApplySort();
    }

    private void ToggleExpandAll()
    {
        IsAllExpanded = !IsAllExpanded;
        foreach (var card in source)
        {
            card.IsExpanded = IsAllExpanded;
        }
    }

    private void UpdateSortKeys()
    {
        foreach (var key in SortKeys)
        {
            var index = activeKeys.IndexOf(key);
            key.Priority = index + 1;
            key.IsActive = index >= 0;
        }

        IsDescending = activeKeys[0].Descending;
        SortText = String.Join(" › ", activeKeys.Select(static x => x.Name + (x.Descending ? " ↓" : " ↑")));
    }

    // 選択・展開の状態はカードが持つため、並べ替えでは順番だけを差し替える
    private void ApplySort()
    {
        var comparer = Comparer<VisitCard>.Create((x, y) =>
        {
            foreach (var key in activeKeys)
            {
                var result = key.Compare(x.Visit, y.Visit);
                if (result != 0)
                {
                    return key.Descending ? -result : result;
                }
            }

            return String.CompareOrdinal(x.Code, y.Code);
        });

        Cards.Clear();
        foreach (var card in source.OrderBy(static x => x, comparer))
        {
            Cards.Add(card);
        }
    }

    private void UpdateSelectedCount() => SelectedCount = source.Count(static x => x.IsSelected);
}

public sealed partial class VisitCard : ObservableObject
{
    public Visit Visit { get; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    [ObservableProperty]
    public partial bool IsExpanded { get; set; }

    public string Code => Visit.Code;

    public string Name => Visit.Name;

    public string Address => Visit.Address;

    public string Staff => Visit.Staff;

    // 担当者のアバター (先頭 1 文字)
    public string StaffInitial => Visit.Staff[..1];

    public string Category => Visit.Category;

    public string CategoryText => Visit.Category switch
    {
        "定期" => "🗓 定期",
        "新規" => "✨ 新規",
        "点検" => "🔧 点検",
        _ => "💰 集金"
    };

    public string Phone => Visit.Phone;

    public string Note => Visit.Note;

    public bool HasNote => !String.IsNullOrEmpty(Visit.Note);

    public bool IsPriority => Visit.IsPriority;

    public bool IsToday => Visit.ScheduledAt.Date == DateTime.Today;

    public bool IsFirstVisit => Visit.LastVisitedAt is null;

    public VisitStatus Status => Visit.Status;

    public string StatusText => Visit.Status switch
    {
        VisitStatus.Planned => "⏳ 未訪問",
        VisitStatus.Visited => "✅ 訪問済",
        VisitStatus.Revisit => "🔁 再訪問",
        _ => "🚫 不在"
    };

    public string ScheduledText => Visit.ScheduledAt.ToString("MM/dd (ddd) HH:mm", CultureInfo.CurrentCulture);

    public string LastVisitedText => Visit.LastVisitedAt?.ToString("yyyy/MM/dd", CultureInfo.CurrentCulture) ?? "なし";

    public VisitCard(Visit visit)
    {
        Visit = visit;
    }
}

public sealed partial class VisitSortKey : ObservableObject
{
    public string Name { get; }

    public Comparison<Visit> Compare { get; }

    // 並替パネルに出す順位 (0 = 未使用)
    [ObservableProperty]
    public partial int Priority { get; set; }

    [ObservableProperty]
    public partial bool IsActive { get; set; }

    [ObservableProperty]
    public partial bool Descending { get; set; }

    public VisitSortKey(string name, Comparison<Visit> compare)
    {
        Name = name;
        Compare = compare;
    }
}
