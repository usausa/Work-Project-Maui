namespace DialogExample.Components.Dialog;

using Android.App;

public static class CurrentActivity
{
    public static Activity Activity { get; private set; } = default!;

    public static void Init(Activity activity)
    {
        Activity = activity;
    }
}
