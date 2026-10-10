namespace Template.MobileApp.Models.Sample;

public enum KitPeriod
{
    Day,
    Week,
    Month
}

public enum KitSleepStage
{
    Awake,
    Rem,
    Light,
    Deep
}

public sealed record KitSeries(IReadOnlyList<double> Values, IReadOnlyList<string> Labels);

public sealed record KitSleepSpan(DateTime Start, TimeSpan Length, KitSleepStage Stage);

public sealed record KitSleepTotal(KitSleepStage Stage, TimeSpan Length);

// 見本の活動の記録 (歩数・心拍・睡眠) と配達中の注文。日時は今日からの相対
public sealed class KitActivity
{
    private const int MonthDays = 30;

    // 1 時間ごとの歩数の割合 (0 時〜23 時)
    private static readonly int[] HourWeights = [0, 0, 0, 0, 0, 0, 2, 9, 12, 4, 3, 4, 10, 6, 3, 3, 4, 8, 11, 7, 5, 3, 1, 0];

    // 1 時間ごとの心拍の目安 (0 時〜23 時)
    private static readonly int[] HourHeart = [56, 55, 54, 54, 55, 57, 63, 78, 82, 72, 70, 71, 76, 73, 70, 69, 72, 80, 96, 84, 76, 70, 64, 60];

    // 昨夜の睡眠の段階 (寝てからの分と段階)
    private static readonly (int Minutes, KitSleepStage Stage)[] SleepPlan =
    [
        (18, KitSleepStage.Light), (42, KitSleepStage.Deep), (24, KitSleepStage.Light), (16, KitSleepStage.Rem),
        (30, KitSleepStage.Light), (34, KitSleepStage.Deep), (22, KitSleepStage.Light), (24, KitSleepStage.Rem),
        (4, KitSleepStage.Awake), (38, KitSleepStage.Light), (18, KitSleepStage.Deep), (26, KitSleepStage.Light),
        (30, KitSleepStage.Rem), (5, KitSleepStage.Awake), (40, KitSleepStage.Light), (26, KitSleepStage.Rem),
        (37, KitSleepStage.Light), (10, KitSleepStage.Awake)
    ];

    private readonly DateTime today;

    public int Steps { get; } = 7852;

    public int StepGoal { get; } = 10000;

    public int HeartRate { get; } = 72;

    public int Calories { get; } = 412;

    public string OrderNumber { get; } = "#A1284";

    // 配達の予定は次の 30 分の区切りの 30 分後
    public DateTime OrderEta { get; }

    public IReadOnlyList<KitSleepSpan> Sleep { get; }

    public double StepRate => (double)Steps / StepGoal;

    public int StepRemain => Math.Max(0, StepGoal - Steps);

    public double Distance => Math.Round(Steps * 0.00072, 1);

    public DateTime BedTime => Sleep[0].Start;

    public DateTime WakeTime => Sleep[^1].Start + Sleep[^1].Length;

    public TimeSpan SleepLength => WakeTime - BedTime;

    public KitActivity(DateTime now)
    {
        today = now.Date;

        var slot = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute >= 30 ? 30 : 0, 0);
        OrderEta = slot.AddMinutes(60);

        var start = today.AddDays(-1).AddHours(23).AddMinutes(12);
        var spans = new List<KitSleepSpan>();
        foreach (var (minutes, stage) in SleepPlan)
        {
            spans.Add(new KitSleepSpan(start, TimeSpan.FromMinutes(minutes), stage));
            start = start.AddMinutes(minutes);
        }
        Sleep = spans;
    }

    public IReadOnlyList<KitSleepTotal> SleepTotals() =>
        Enum.GetValues<KitSleepStage>()
            .Reverse()
            .Select(x => new KitSleepTotal(x, TimeSpan.FromMinutes(Sleep.Where(y => y.Stage == x).Sum(static y => y.Length.TotalMinutes))))
            .ToList();

    //--------------------------------------------------------------------------------
    // Series
    //--------------------------------------------------------------------------------

    public KitSeries StepSeries(KitPeriod period)
    {
        if (period == KitPeriod.Day)
        {
            // 割合で分けた端数は、いちばん多い時間に足して合計を今日の歩数にそろえる
            var total = HourWeights.Sum();
            var hours = HourWeights.Select(x => Math.Floor((double)Steps * x / total)).ToArray();
            hours[Array.IndexOf(HourWeights, HourWeights.Max())] += Steps - hours.Sum();
            return new KitSeries(hours, Enumerable.Range(0, 24).Select(static x => $"{x}時").ToList());
        }

        return DailySeries(period, StepsOf);
    }

    public KitSeries HeartSeries(KitPeriod period)
    {
        if (period == KitPeriod.Day)
        {
            return new KitSeries(
                HourHeart.Select(static x => (double)x).ToList(),
                Enumerable.Range(0, 24).Select(static x => $"{x}時").ToList());
        }

        return DailySeries(period, HeartOf);
    }

    // 週は曜日、月は日付の名前
    private KitSeries DailySeries(KitPeriod period, Func<DateTime, int> valueOf)
    {
        var count = period == KitPeriod.Week ? 7 : MonthDays;
        var format = period == KitPeriod.Week ? "ddd" : "M/d";
        var days = Enumerable.Range(0, count).Select(x => today.AddDays(x - count + 1)).ToList();
        return new KitSeries(
            days.Select(x => (double)valueOf(x)).ToList(),
            days.Select(x => x.ToString(format, CultureInfo.CurrentCulture)).ToList());
    }

    // 日ごとの値は周期の違う波を重ねて日付でずらす (今日は今の値)
    private int StepsOf(DateTime date) =>
        date == today ? Steps : 7400 + (int)((2600 * Math.Sin(date.DayOfYear * 1.37)) + (1400 * Math.Sin(date.DayOfYear * 0.53)));

    private int HeartOf(DateTime date) =>
        date == today ? HeartRate : 70 + (int)Math.Round((3 * Math.Sin(date.DayOfYear * 1.13)) + (2 * Math.Sin(date.DayOfYear * 0.41)));
}
