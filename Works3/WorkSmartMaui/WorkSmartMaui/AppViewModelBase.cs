namespace WorkSmartMaui;

using System.ComponentModel.DataAnnotations;

using Smart.Mvvm.Resolver;
using Smart.Maui.ViewModels;

using WorkSmartMaui.Shell;

internal class AppViewModelBase : ExtendViewModelBase, IValidatable
{
    private List<ValidationResult>? validationResults;

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
