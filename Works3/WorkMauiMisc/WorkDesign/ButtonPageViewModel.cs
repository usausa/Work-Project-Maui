using System.Diagnostics;

namespace WorkDesign;

using System.Globalization;
using System.Windows.Input;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

public enum SelectedPage
{
    Home,
    Search,
    Notifications,
    Account
}

public sealed partial class ButtonPageViewModel : ExtendViewModelBase
{
    [ObservableProperty]
    public partial SelectedPage Selected { get; set; }

    public ICommand ChangeCommand { get; }

    public ButtonPageViewModel()
    {
        Selected = SelectedPage.Home;

        ChangeCommand = new Command<SelectedPage>(page => Selected = page);
    }
}

public sealed class SelectedToImageSourceConverter : IValueConverter
{
    public SelectedPage Page { get; set; }

    public ImageSource Default { get; set; } = default!;

    public ImageSource Selected { get; set; } = default!;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Equals(value, Page) ? Selected : Default;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

[AcceptEmptyServiceProvider]
public sealed class SelectedToImageSourceExtension : IMarkupExtension<SelectedToImageSourceConverter>
{
    public SelectedPage Page { get; set; }

    public ImageSource Default { get; set; } = default!;

    public ImageSource Selected { get; set; } = default!;

    public SelectedToImageSourceConverter ProvideValue(IServiceProvider serviceProvider) =>
        new() { Page = Page, Default = Default, Selected = Selected };

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
}

public sealed class SelectedToColorConverter : IValueConverter
{
    public SelectedPage Page { get; set; }

    public Color Default { get; set; } = default!;

    public Color Selected { get; set; } = default!;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Equals(value, Page) ? Selected : Default;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

[AcceptEmptyServiceProvider]
public sealed class SelectedToColorExtension : IMarkupExtension<SelectedToColorConverter>
{
    public SelectedPage Page { get; set; }

    public Color Default { get; set; } = default!;

    public Color Selected { get; set; } = default!;

    public SelectedToColorConverter ProvideValue(IServiceProvider serviceProvider) =>
        new() { Page = Page, Default = Default, Selected = Selected };

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
}
