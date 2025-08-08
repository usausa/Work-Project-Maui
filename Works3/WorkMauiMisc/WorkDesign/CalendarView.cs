namespace WorkDesign;
using Microsoft.Maui.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CalendarView : GraphicsView
{
    public static readonly BindableProperty SelectedDateProperty =
        BindableProperty.Create(nameof(SelectedDate), typeof(DateTime), typeof(CalendarView),
            DateTime.Now, propertyChanged: OnSelectedDateChanged);

    public DateTime SelectedDate
    {
        get => (DateTime)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    private static void OnSelectedDateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CalendarView)bindable;
        control.Invalidate();
    }

    private readonly CalendarDrawable _drawable;

    public CalendarView()
    {
        _drawable = new CalendarDrawable(this);
        Drawable = _drawable;

        // タッチイベントを処理
        StartInteraction += CalendarView_StartInteraction;
    }

    private void CalendarView_StartInteraction(object? sender, TouchEventArgs e)
    {
        var point = e.Touches[0];
        var date = _drawable.GetDateFromPoint(point.X, point.Y);

        if (date.HasValue)
        {
            SelectedDate = date.Value;
        }
    }

    // カレンダーのタイトルを取得するメソッド
    public string GetCalendarTitle()
    {
        return SelectedDate.ToString("yyyy年MM月");
    }

    // 前の月に移動
    public void PreviousMonth()
    {
        SelectedDate = SelectedDate.AddMonths(-1);
    }

    // 次の月に移動
    public void NextMonth()
    {
        SelectedDate = SelectedDate.AddMonths(1);
    }
}

public class CalendarDrawable : IDrawable
{
    private readonly CalendarView _calendarView;
    private float _cellWidth;
    private float _cellHeight;
    private readonly List<CalendarCell> _cells = new List<CalendarCell>();

    public CalendarDrawable(CalendarView calendarView)
    {
        _calendarView = calendarView;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        _cells.Clear();

        var date = _calendarView.SelectedDate;
        var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        // セルのサイズを計算
        _cellWidth = dirtyRect.Width / 7; // 7列（日〜土）
        _cellHeight = dirtyRect.Height / 7; // 6行（ヘッダー + 5行の週）

        // ヘッダー（曜日）を描画
        DrawHeader(canvas, dirtyRect);

        // カレンダーのセルを描画
        DrawCalendarCells(canvas, dirtyRect, firstDayOfMonth, lastDayOfMonth);
    }

    private void DrawHeader(ICanvas canvas, RectF dirtyRect)
    {
        string[] daysOfWeek = { "日", "月", "火", "水", "木", "金", "土" };

        canvas.FontSize = 14;
        canvas.FontColor = Colors.Black;

        for (int i = 0; i < 7; i++)
        {
            var x = i * _cellWidth;
            var textColor = i == 0 ? Colors.Red : (i == 6 ? Colors.Blue : Colors.Black);

            canvas.FontColor = textColor;
            canvas.DrawString(daysOfWeek[i], x + _cellWidth / 2, _cellHeight / 2, HorizontalAlignment.Center);
        }
    }

    private void DrawCalendarCells(ICanvas canvas, RectF dirtyRect, DateTime firstDayOfMonth, DateTime lastDayOfMonth)
    {
        // 表示開始日（前月の日を含む）
        var startDate = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek);

        // 5行分のカレンダーセルを描画（日曜始まり）
        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                var currentDate = startDate.AddDays(row * 7 + col);
                var x = col * _cellWidth;
                var y = (row + 1) * _cellHeight; // ヘッダー分を考慮

                bool isCurrentMonth = currentDate.Month == firstDayOfMonth.Month;
                bool isToday = currentDate.Date == DateTime.Today;
                bool isSelected = currentDate.Date == _calendarView.SelectedDate.Date;

                DrawCell(canvas, x, y, _cellWidth, _cellHeight, currentDate, isCurrentMonth, isToday, isSelected);

                // 日付情報を保存
                _cells.Add(new CalendarCell
                {
                    Date = currentDate,
                    Bounds = new RectF(x, y, _cellWidth, _cellHeight)
                });
            }
        }
    }

    private void DrawCell(ICanvas canvas, float x, float y, float width, float height,
                          DateTime date, bool isCurrentMonth, bool isToday, bool isSelected)
    {
        // セルの背景
        if (isSelected)
        {
            canvas.FillColor = Colors.LightBlue;
            canvas.FillRectangle(x, y, width, height);
        }
        else if (isToday)
        {
            canvas.FillColor = Colors.LightYellow;
            canvas.FillRectangle(x, y, width, height);
        }

        // セルの枠線
        canvas.StrokeColor = Colors.Gray;
        canvas.StrokeSize = 1;
        canvas.DrawRectangle(x, y, width, height);

        // 日付のテキスト
        if (isCurrentMonth)
        {
            // 日曜日は赤、土曜日は青
            if (date.DayOfWeek == DayOfWeek.Sunday)
                canvas.FontColor = Colors.Red;
            else if (date.DayOfWeek == DayOfWeek.Saturday)
                canvas.FontColor = Colors.Blue;
            else
                canvas.FontColor = Colors.Black;
        }
        else
        {
            // 前月・翌月の日付は薄いグレー
            canvas.FontColor = Colors.LightGray;
        }

        canvas.FontSize = 16;
        canvas.DrawString(date.Day.ToString(), x + width / 2, y + height / 2, HorizontalAlignment.Center);
    }

    public DateTime? GetDateFromPoint(double x, double y)
    {
        foreach (var cell in _cells)
        {
            if (cell.Bounds.Contains((float)x, (float)y))
            {
                return cell.Date;
            }
        }

        return null;
    }
}

public class CalendarCell
{
    public DateTime Date { get; set; }
    public RectF Bounds { get; set; }
}