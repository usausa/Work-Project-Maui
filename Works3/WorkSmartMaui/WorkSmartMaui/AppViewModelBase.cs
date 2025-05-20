using Smart.Mvvm.Resolver;

using WorkSmartMaui.Shell;

namespace WorkSmartMaui;

using System.ComponentModel.DataAnnotations;

using Smart.Maui.ViewModels;

internal class AppViewModelBase : ExtendViewModelBase, IValidatable
{
    private List<ValidationResult>? validationResults;

    protected OverlayController Overlay { get; } = OverlayController.Instance;

    public void Validate(string name)
    {
        var pi = GetType().GetProperty(name);
        if (pi is null)
        {
            return;
        }

        validationResults ??= new List<ValidationResult>();

        var value = pi.GetValue(this, null);
        var context = new ValidationContext(this, DefaultResolveProvider.Default, null)
        {
            MemberName = name
        };
        if (!Validator.TryValidateProperty(value, context, validationResults))
        {
            Errors.AddError(name, validationResults[0].ErrorMessage!);
        }

        validationResults.Clear();
    }
}
