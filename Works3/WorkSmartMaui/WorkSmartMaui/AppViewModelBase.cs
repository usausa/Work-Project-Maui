namespace WorkSmartMaui;

using Smart.Maui.ViewModels;

internal class AppViewModelBase : ExtendViewModelBase, IValidatable
{
    public void Validate(string name)
    {
        Errors.AddError(name, "Error!");
    }
}
