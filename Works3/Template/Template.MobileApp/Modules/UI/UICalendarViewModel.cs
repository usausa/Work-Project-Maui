namespace Template.MobileApp.Modules.UI;

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

using Template.MobileApp.Models.Sample.Calendar;
using Template.MobileApp.Services;

public sealed partial class UICalendarViewModel : AppViewModelBase
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Today);

    private readonly ScheduleService scheduleService = new();
    private readonly HolidayService holidayService = new();

    private MonthViewBuilder builder = new(DayOfWeek.Monday);
    private int currentYear;
    private int currentMonth;

    [ObservableProperty]
    public partial MonthView? MonthView { get; private set; }

    private DayOfWeek firstDayOfWeek = DayOfWeek.Monday;

    public DayOfWeek FirstDayOfWeek
    {
        get => firstDayOfWeek;
        set
        {
            if (firstDayOfWeek == value)
            {
                return;
            }
            firstDayOfWeek = value;
            RaisePropertyChanged(new PropertyChangedEventArgs(nameof(FirstDayOfWeek)));
            builder = new MonthViewBuilder(value);
            LoadMonth(currentYear, currentMonth);
        }
    }

    [ObservableProperty]
    public partial CalendarSelectionMode SelectionMode { get; set; } = CalendarSelectionMode.None;

    [ObservableProperty]
    public partial DateOnly? SelectedDate { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<DateOnly> SelectedDates { get; set; } = [];

    [ObservableProperty]
    public partial DateOnly? SelectedStartDate { get; set; }

    [ObservableProperty]
    public partial DateOnly? SelectedEndDate { get; set; }

    [ObservableProperty]
    public partial DateOnly? MinDate { get; set; }

    [ObservableProperty]
    public partial DateOnly? MaxDate { get; set; }

    [ObservableProperty]
    public partial CultureInfo? Culture { get; set; }

    public IObserveCommand PrevMonthCommand { get; }
    public IObserveCommand NextMonthCommand { get; }
    public IObserveCommand GoToTodayCommand { get; }
    public IObserveCommand DayTappedCommand { get; }
    public IObserveCommand EventTappedCommand { get; }

    public UICalendarViewModel()
    {
        PrevMonthCommand = MakeDelegateCommand(OnPrevMonth);
        NextMonthCommand = MakeDelegateCommand(OnNextMonth);
        GoToTodayCommand = MakeDelegateCommand(OnGoToToday);
        DayTappedCommand = MakeDelegateCommand<DayView>(OnDayTapped);
        EventTappedCommand = MakeDelegateCommand<ScheduleEvent>(OnEventTapped);

        currentYear = Today.Year;
        currentMonth = Today.Month;
    }

    public override Task OnNavigatedToAsync(INavigationContext context)
    {
        if (MonthView is null)
        {
            LoadMonth(currentYear, currentMonth);
        }
        return Task.CompletedTask;
    }

    private void OnPrevMonth()
    {
        var prev = new DateOnly(currentYear, currentMonth, 1).AddMonths(-1);
        currentYear = prev.Year;
        currentMonth = prev.Month;
        LoadMonth(currentYear, currentMonth);
    }

    private void OnNextMonth()
    {
        var next = new DateOnly(currentYear, currentMonth, 1).AddMonths(1);
        currentYear = next.Year;
        currentMonth = next.Month;
        LoadMonth(currentYear, currentMonth);
    }

    private void OnGoToToday()
    {
        currentYear = Today.Year;
        currentMonth = Today.Month;
        LoadMonth(currentYear, currentMonth);
    }

    private void LoadMonth(int year, int month)
    {
        var sw = Stopwatch.StartNew();
        var (rangeStart, rangeEnd) = builder.GetDisplayRange(year, month);
        var events = scheduleService.GetEvents(rangeStart, rangeEnd);
        var stamps = scheduleService.GetStamps(rangeStart, rangeEnd);
        var holidays = holidayService.GetHolidays(rangeStart, rangeEnd);

        MonthView = builder.Build(year, month, Today, events, stamps, holidays);
        Debug.WriteLine($"[LoadMonth] {year}/{month:D2} | Total: {sw.Elapsed.TotalMilliseconds:F2}ms");
    }

    private static void OnDayTapped(DayView? day)
    {
        if (day is not null)
        {
            Debug.WriteLine($"Day tapped: {day.Date:yyyy-MM-dd}");
        }
    }

    private static void OnEventTapped(ScheduleEvent? evt)
    {
        if (evt is not null)
        {
            Debug.WriteLine($"Event tapped: {evt.Id} {evt.Title}");
        }
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.UIMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
