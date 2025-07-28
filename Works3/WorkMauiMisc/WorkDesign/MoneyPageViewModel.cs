namespace WorkDesign;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

using Syncfusion.Maui.Toolkit.EffectsView;

using System.Globalization;
using System.Windows.Input;

public sealed partial class MoneyPage2ViewModel : ExtendViewModelBase
{
    [ObservableProperty]
    public partial MoneyPage Selected { get; set; }

    [ObservableProperty]
    public partial int NotificationCount { get; set; }

    [ObservableProperty]
    public partial bool HasAccountAlert { get; set; }

    public ICommand PageCommand { get; }

    public MoneyPage2ViewModel()
    {
        Selected = MoneyPage.Home;

        PageCommand = MakeDelegateCommand<MoneyPage>(page => Selected = page);

        NotificationCount = 99;
        HasAccountAlert = true;
    }
}

public enum MoneyPage
{
    Home,
    Search,
    Notifications,
    Account
}

[AcceptEmptyServiceProvider]
public sealed class MoneyPageToColorExtension : IMarkupExtension<MoneyPageToColorConverter>
{
    public MoneyPage Page { get; set; }

    public Color Default { get; set; } = default!;

    public Color Selected { get; set; } = default!;

    public MoneyPageToColorConverter ProvideValue(IServiceProvider serviceProvider) =>
        new() { Page = Page, Default = Default, Selected = Selected };

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
}

[AcceptEmptyServiceProvider]
public sealed class MoneyPageToImageSourceExtension : IMarkupExtension<MoneyPageToImageSourceConverter>
{
    public MoneyPage Page { get; set; }

    public ImageSource Default { get; set; } = default!;

    public ImageSource Selected { get; set; } = default!;

    public MoneyPageToImageSourceConverter ProvideValue(IServiceProvider serviceProvider) =>
        new() { Page = Page, Default = Default, Selected = Selected };

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
}


public abstract class MoneyPageToConverter<T> : IValueConverter
{
    public MoneyPage Page { get; set; }

    public T Default { get; set; } = default!;

    public T Selected { get; set; } = default!;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Equals(value, Page) ? Selected : Default;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

public sealed class MoneyPageToColorConverter : MoneyPageToConverter<Color>
{
}

public sealed class MoneyPageToImageSourceConverter : MoneyPageToConverter<ImageSource>
{
}
