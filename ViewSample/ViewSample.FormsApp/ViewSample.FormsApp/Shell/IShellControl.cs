namespace ViewSample.FormsApp.Shell
{
    using Smart.ComponentModel;

    public interface IShellControl
    {
        NotificationValue<string> Title { get; }
    }
}
