using Smart.Maui.Input;
using Smart.Mvvm;

using System.Runtime.CompilerServices;

namespace WorkDesign;

using Java.Time;

using Smart.Maui.ViewModels;

using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Globalization;

public partial class MailPage : ContentPage
{
	public MailPage()
	{
		InitializeComponent();
	}
}

public partial class MailPageViewModel : ExtendViewModelBase
{
    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    public ObservableCollection<MailMessage> Messages { get; } = [];

    public MailPageViewModel()
    {
        _ = Task.Run(InitializeData);
    }

    private void InitializeData()
    {
        IsLoading = true;

        Messages.Add(new MailMessage
        {
            DateTime = DateTime.Now,
            Image = ImageSource.FromFile("profile.jpg"),
            From = "山奥通信",
            Title = "タイトルだよもんタイトルだよもんタイトルだよもんタイトルだよもんタイトルだよもん",
            Body = "こんにちは。\nうさうさです、どうぞよろしくお願いしますだよもん。\n文章はまだ続きます。"
        });
        Messages.Add(new MailMessage
        {
            DateTime = DateTime.Now.AddHours(-3).AddMinutes(-12),
            Image = ImageSource.FromFile("genbaneko.jpg"),
            From = "現場猫bot",
            Title = "作業前安全確認",
            Body = "今日も一日ゼロ災ヨシ！\nあああああああああああ!!!!!!!!!!"
        });
        Messages.Add(new MailMessage
        {
            DateTime = DateTime.Now.AddHours(-6).AddMinutes(-15),
            Image = ImageSource.FromFile("profile.jpg"),
            From = "山奥通信",
            Title = "タイトルだよもんタイトルだよもんタイトルだよもんタイトルだよもんタイトルだよもん",
            Body = "こんにちは。\n私はうさうさです。\nどうぞよろしくお願いしますだよもん。\n文章はまだ続きます。"
        });
        Messages.Add(new MailMessage
        {
            DateTime = DateTime.Now.AddDays(-1),
            Image = ImageSource.FromFile("usausa.jpg"),
            From = "うさうさメープルフレンチトースト",
            Title = "Re: Re: おはようございます！",
            Body = "こんにちは！\n先日はありがとうございました"
        });
        Messages.Add(new MailMessage
        {
            DateTime = DateTime.Now.AddDays(-2),
            Image = ImageSource.FromFile("genbaneko.jpg"),
            From = "現場猫bot",
            Title = "作業前安全確認",
            Body = "今日も一日ゼロ災ヨシ！\nあああああああああああ!!!!!!!!!!"
        });
        Messages.Add(new MailMessage
        {
            DateTime = DateTime.Now.AddDays(-5),
            Image = ImageSource.FromFile("genbaneko.jpg"),
            From = "現場猫bot",
            Title = "作業前安全確認",
            Body = "今日も一日ゼロ災ヨシ！\nあああああああああああ!!!!!!!!!!"
        });
        Messages.Add(new MailMessage
        {
            DateTime = DateTime.Now.AddDays(-10),
            Image = ImageSource.FromFile("genbaneko.jpg"),
            From = "現場猫bot",
            Title = "作業前安全確認",
            Body = "今日も一日ゼロ災ヨシ！\nあああああああああああ!!!!!!!!!!"
        });
        Messages.Add(new MailMessage
        {
            DateTime = DateTime.Now.AddDays(-20),
            Image = ImageSource.FromFile("genbaneko.jpg"),
            From = "現場猫bot",
            Title = "作業前安全確認",
            Body = "今日も一日ゼロ災ヨシ！\nあああああああああああ!!!!!!!!!!"
        });

        IsLoading = false;
    }
}

public sealed class MailMessage
{
    public DateTime DateTime { get; set; }

    public ImageSource Image { get; set; } = default!;

    public string From { get; set; } = default!;

    public string Title { get; set; } = default!;

    public string Body { get; set; } = default!;
}

public class MailDateTimeStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            var now = DateTime.Now;
            var today = now.Date;
            var oneWeekAgo = today.AddDays(-7);

            if (dateTime.Date == today)
            {
                return dateTime.ToString("HH:mm", culture);
            }
            if (dateTime.Date > oneWeekAgo)
            {
                return dateTime.ToString("dddd", culture);
            }
            return dateTime.ToString("MM/dd", culture);
        }

        return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
