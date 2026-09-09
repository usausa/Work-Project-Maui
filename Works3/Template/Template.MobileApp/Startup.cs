namespace Template.MobileApp;

using System.Diagnostics;

using Fonts;

using Microsoft.Extensions.DependencyInjection;

// 起動時に用意する static なリソースをまとめる
#pragma warning disable CA1724
public sealed class Startup
{
    // アイコンフォントは初回参照時に Typeface の生成とグリフのビットマップ化が走る。
    // Button.ImageSource への反映は非同期のため、温めていないと最初に表示する画面で
    // アイコンが遅れて入り、ボタン内の文字位置が動いて見える。
    // グリフのビットマップはサイズと色を含むキーでキャッシュされるため、XAML の指定と揃える

    // 一度に投げる要求数。初期表示の後ろで動かす分が UI スレッドを長く占有しないように区切る
    private const int BatchSize = 16;

    private const double MenuIconSize = 24d;

    private const double SmallIconSize = 18d;

    private const double LargeIconSize = 36d;

    // markup:MenuIcon 相当 (Material / 24 / White)
    private static readonly string[] StartupGlyphs =
    [
        // MenuView
        MaterialIcons.Widgets,
        MaterialIcons.Navigation,
        MaterialIcons.Devices,
        MaterialIcons.Storage,
        MaterialIcons.Cloud,
        MaterialIcons.Layers,
        MaterialIcons.Palette,
        MaterialIcons.Insights,
        MaterialIcons.Science,
        MaterialIcons.Apps,
        MaterialIcons.Settings
    ];

    private static readonly string[] MenuIconGlyphs =
    [
        MaterialIcons.Account_balance_wallet,
        MaterialIcons.Account_circle,
        MaterialIcons.Account_tree,
        MaterialIcons.Archive,
        MaterialIcons.Arrow_back,
        MaterialIcons.Arrow_downward,
        MaterialIcons.Arrow_forward,
        MaterialIcons.Arrow_upward,
        MaterialIcons.Article,
        MaterialIcons.Attractions,
        MaterialIcons.Av_timer,
        MaterialIcons.Bar_chart,
        MaterialIcons.Block,
        MaterialIcons.Bolt,
        MaterialIcons.Calendar_month,
        MaterialIcons.Category,
        MaterialIcons.Chat,
        MaterialIcons.Crop,
        MaterialIcons.Dashboard,
        MaterialIcons.Delete,
        MaterialIcons.Document_scanner,
        MaterialIcons.Explore,
        MaterialIcons.Extension,
        MaterialIcons.Face,
        MaterialIcons.Filter_list,
        MaterialIcons.Flight_takeoff,
        MaterialIcons.Flip,
        MaterialIcons.Grid_view,
        MaterialIcons.Hearing,
        MaterialIcons.Language,
        MaterialIcons.Live_tv,
        MaterialIcons.Login,
        MaterialIcons.Mail,
        MaterialIcons.Map,
        MaterialIcons.Memory,
        MaterialIcons.Mood,
        MaterialIcons.Movie,
        MaterialIcons.Opacity,
        MaterialIcons.People,
        MaterialIcons.Pets,
        MaterialIcons.Picture_as_pdf,
        MaterialIcons.Point_of_sale,
        MaterialIcons.Radar,
        MaterialIcons.Rotate_right,
        MaterialIcons.Schedule,
        MaterialIcons.Sell,
        MaterialIcons.Sensors,
        MaterialIcons.Smart_toy,
        MaterialIcons.Speed,
        MaterialIcons.Sports_esports,
        MaterialIcons.Storefront,
        MaterialIcons.Timeline,
        MaterialIcons.Tune,
        MaterialIcons.Vertical_align_bottom,
        MaterialIcons.View_timeline,
        MaterialIcons.Web,
        MaterialIcons.Zoom_in
    ];

    // markup:Material の Size=18 指定 (Basic / Device の操作ボタン)
    private static readonly string[] SmallIconGlyphs =
    [
        MaterialIcons.Brightness_high,
        MaterialIcons.Brightness_low,
        MaterialIcons.Do_not_disturb,
        MaterialIcons.Flashlight_off,
        MaterialIcons.Flashlight_on,
        MaterialIcons.Input,
        MaterialIcons.Mic,
        MaterialIcons.Record_voice_over,
        MaterialIcons.Screenshot,
        MaterialIcons.Stay_current_landscape,
        MaterialIcons.Stay_current_portrait,
        MaterialIcons.Text_format,
        MaterialIcons.Touch_app,
        MaterialIcons.Vibration,
        MaterialIcons.Visibility,
        MaterialIcons.Visibility_off,
        MaterialIcons.Voice_over_off
    ];

    private readonly IServiceProvider provider;

    private readonly ILogger<Startup> log;

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public Startup(IServiceProvider provider, ILogger<Startup> log)
    {
        this.provider = provider;
        this.log = log;
    }

    //--------------------------------------------------------------------------------
    // Prepare
    //--------------------------------------------------------------------------------

    // 最初の画面を表示する前に必要な分
    public async ValueTask PrepareAsync()
    {
        var watch = Stopwatch.StartNew();
        WarmTypefaces();
        log.DebugFontWarmup("typeface", watch.ElapsedMilliseconds);

        watch.Restart();
        await WarmGlyphsAsync([new(MenuIconSize, Colors.White, StartupGlyphs)]);
        log.DebugFontWarmup("glyph(startup)", watch.ElapsedMilliseconds);
    }

    //--------------------------------------------------------------------------------
    // Warmup
    //--------------------------------------------------------------------------------

    // 残りのボタンアイコン。初期表示を待たせないよう後から温める
    public async ValueTask WarmupAsync()
    {
        var watch = Stopwatch.StartNew();
        await WarmGlyphsAsync(
            [
                new(MenuIconSize, Colors.White, MenuIconGlyphs),
                new(SmallIconSize, ResourceColor("BlueGrayDarken1"), SmallIconGlyphs),
                new(SmallIconSize, ResourceColor("RedDefault"), [MaterialIcons.Error]),
                new(SmallIconSize, ResourceColor("GreenDefault"), [MaterialIcons.Check_circle]),
                new(LargeIconSize, Colors.White, [MaterialIcons.Add])
            ]);
        log.DebugFontWarmup("glyph(rest)", watch.ElapsedMilliseconds);
    }

    //--------------------------------------------------------------------------------
    // Font
    //--------------------------------------------------------------------------------

    // Typeface はフォント毎に FontManager がキャッシュする
    private void WarmTypefaces()
    {
        var fontManager = provider.GetRequiredService<IFontManager>();
        fontManager.GetTypeface(Microsoft.Maui.Font.OfSize(MaterialIcons.FontFamily, MenuIconSize));
        fontManager.GetTypeface(Microsoft.Maui.Font.OfSize(FluentUI.FontFamily, MenuIconSize));
    }

    private async ValueTask WarmGlyphsAsync(IEnumerable<GlyphSet> sets)
    {
#if ANDROID
        var imageSourceServiceProvider = provider.GetService<IImageSourceServiceProvider>();
        if (imageSourceServiceProvider is null)
        {
            return;
        }

        var context = Android.App.Application.Context;
        var pending = new List<Task>(BatchSize);
        foreach (var set in sets)
        {
            foreach (var glyph in set.Glyphs)
            {
                var source = new FontImageSource
                {
                    FontFamily = MaterialIcons.FontFamily,
                    Glyph = glyph,
                    Size = set.Size,
                    Color = set.Color
                };
                var service = imageSourceServiceProvider.GetRequiredImageSourceService(source);
                pending.Add(service.GetDrawableAsync(source, context));

                if (pending.Count == BatchSize)
                {
                    await Task.WhenAll(pending).ConfigureAwait(true);
                    pending.Clear();
                }
            }
        }

        if (pending.Count > 0)
        {
            await Task.WhenAll(pending).ConfigureAwait(true);
        }
#endif
    }

    private static Color ResourceColor(string key)
    {
        var resources = Application.Current?.Resources;
        if ((resources is not null) && resources.TryGetValue(key, out var value) && (value is Color color))
        {
            return color;
        }

        return Colors.White;
    }

    private readonly record struct GlyphSet(double Size, Color Color, string[] Glyphs);
}
#pragma warning restore CA1724
