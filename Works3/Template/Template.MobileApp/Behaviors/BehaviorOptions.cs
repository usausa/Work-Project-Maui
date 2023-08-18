namespace Template.MobileApp.Behaviors;

public sealed class BehaviorOptions
{
    public bool Border { get; set; } = true;

    // Entry

    public bool HandleEnter { get; set; } = true;

    public bool DisableShowSoftInputOnFocus { get; set; } = true;

    public bool SelectAllOnFocus { get; set; } = true;

    public bool NoBorder { get; set; } = true;

    public bool InputFilter { get; set; } = true;

    // ListView

    public bool DisableOverScroll { get; set; } = true;
}
