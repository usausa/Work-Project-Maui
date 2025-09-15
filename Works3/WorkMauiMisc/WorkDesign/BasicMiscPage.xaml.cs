namespace WorkDesign;

using System.Diagnostics;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

public partial class BasicMiscPage : ContentPage
{
	public BasicMiscPage()
	{
		InitializeComponent();
	}
}

public sealed partial class BasicMiscPageViewModel : ExtendViewModelBase
{
    [ObservableProperty]
    public partial List<AddressGroup> List { get; set; } = [];

    public ICommand ToggleCommand { get; }

    public ICommand MailCommand { get; }
    public ICommand PhoneCommand { get; }


    public BasicMiscPageViewModel()
    {
        List.Add(CreateGroup("あ", ["浅井 長政", "安国寺 恵瓊", "井伊 直政", "石田 三成", "上杉 景勝", "宇喜多 秀家"]));
        List.Add(CreateGroup("か", ["加藤 清正", "黒田 長政", "小早川 秀秋"]));
        List.Add(CreateGroup("さ", ["佐竹 義宣", "島 左近", "島津 義弘"]));
        List.Add(CreateGroup("た", ["滝川 一益", "立花 宗茂", "長曾我部 盛親"]));
        List.Add(CreateGroup("な", ["直江 兼続", "鍋島 勝茂"]));
        List.Add(CreateGroup("は", ["平塚 為広", "本多 忠勝", "細川 忠興"]));
        List.Add(CreateGroup("ま", ["前田 利長", "最上 義光", "毛利 輝元"]));
        List.Add(CreateGroup("や", ["山内 一豊"]));

        ToggleCommand = MakeDelegateCommand<AddressGroup>(g => g.IsExpanded = !g.IsExpanded);

        MailCommand = MakeDelegateCommand<AddressRow>(x =>
        {
            var item = x.Value;
            Debug.WriteLine($"Mail: {item.Name} {item.MailAddress}");
        });
        PhoneCommand = MakeDelegateCommand<AddressRow>(x =>
        {
            var item = x.Value;
            Debug.WriteLine($"Phone: {item.Name} {item.PhoneNumber}");
        });
    }

    private static AddressGroup CreateGroup(string key, IEnumerable<string> names) =>
        new(key, names.Select(static (x, i) => new AddressRow(new AddressEntity
        {
            Image = "account.png",
            Name = x,
            PhoneNumber = "090-0000-0000",
            MailAddress = "user@example.com"
        }) { IsEven = i % 2 == 0 }));
}

public sealed class AddressEntity
{
    public string Image { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string PhoneNumber { get; set; } = default!;

    public string MailAddress { get; set; } = default!;
}

public sealed class AddressRow : AlternateRow<AddressEntity>
{
    public AddressRow(AddressEntity value)
        : base(value)
    {
    }
}

public sealed class AddressGroup : CollectionGroup<string, AddressRow>
{
    public AddressGroup(string key)
        : base(key)
    {
    }

    public AddressGroup(string key, IEnumerable<AddressRow> items, bool isExpanded = true)
        : base(key, items, isExpanded)
    {
    }
}

public interface IAlternateRow
{
    public bool IsEven { get; }
}

public partial class AlternateRow<T> : ObservableObject, IAlternateRow
{
    public T Value { get; }

    [ObservableProperty]
    public partial bool IsEven { get; set; }

    public AlternateRow(T value)
    {
        Value = value;
    }
}

public class CollectionGroup<TKey, TItem> : IReadOnlyList<TItem>, INotifyPropertyChanged, INotifyCollectionChanged
{
    // ReSharper disable StaticMemberInGenericType
    private static readonly PropertyChangedEventArgs IsExpandedChangedEventArgs = new(nameof(IsExpanded));

    private static readonly NotifyCollectionChangedEventArgs ResetEventArgs = new(NotifyCollectionChangedAction.Reset);
    // ReSharper restore StaticMemberInGenericType

    public event PropertyChangedEventHandler? PropertyChanged;

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    private readonly List<TItem> sourceItems;

    private List<TItem> displayItems;

    private bool isExpanded;

    public TKey Key { get; }

    public IReadOnlyList<TItem> SourceItems => sourceItems.AsReadOnly();

    public int SourceCount => sourceItems.Count;

    public bool IsExpanded
    {
        get => isExpanded;
        set
        {
            if (isExpanded != value)
            {
                isExpanded = value;

                displayItems = isExpanded ? sourceItems : [];

                PropertyChanged?.Invoke(this, IsExpandedChangedEventArgs);
                CollectionChanged?.Invoke(this, ResetEventArgs);
            }
        }
    }

    public int Count => displayItems.Count;

    public TItem this[int index] => displayItems[index];

    public CollectionGroup(TKey key)
    {
        Key = key;
        sourceItems = [];
        displayItems = [];
    }

    public CollectionGroup(TKey key, IEnumerable<TItem> items, bool isExpanded = true)
    {
        Key = key;
        this.isExpanded = isExpanded;
        sourceItems = items.ToList();
        displayItems = isExpanded ? sourceItems : [];
    }

    public IEnumerator<TItem> GetEnumerator() => displayItems.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => displayItems.GetEnumerator();

    // ------------------------------------------------------------
    // Source
    // ------------------------------------------------------------

    public int IndexOfSource(TItem item) => sourceItems.IndexOf(item);

    public void ClearSource()
    {
        sourceItems.Clear();
        if (isExpanded)
        {
            CollectionChanged?.Invoke(this, ResetEventArgs);
        }
    }

    public void AddToSource(TItem item)
    {
        sourceItems.Add(item);
        if (isExpanded)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, sourceItems.Count - 1));
        }
    }

    public void AddRangeToSource(IEnumerable<TItem> items)
    {
        var list = items.ToList();
        if (list.Count == 0)
        {
            return;
        }

        var index = sourceItems.Count;
        sourceItems.AddRange(list);
        if (isExpanded)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, index));
        }
    }

    public void InsertToSource(int index, TItem item)
    {
        sourceItems.Insert(index, item);
        if (isExpanded)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }
    }

    public void ReplaceSource(TItem oldItem, TItem newItem)
    {
        var index = sourceItems.IndexOf(oldItem);
        if (index < 0)
        {
            return;
        }

        sourceItems[index] = newItem;
        if (isExpanded)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index));
        }
    }

    public bool RemoveFromSource(TItem item)
    {
        var index = sourceItems.IndexOf(item);
        if (index < 0)
        {
            return false;
        }

        sourceItems.RemoveAt(index);
        if (isExpanded)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        return true;
    }

    public bool RemoveAtSource(int index)
    {
        var item = sourceItems[index];

        sourceItems.RemoveAt(index);
        if (isExpanded)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        return true;
    }
}
