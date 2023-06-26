namespace Template.MobileApp.State;

public class Configuration
{
    private readonly IPreferences preferences;

    public Configuration(IPreferences preferences)
    {
        this.preferences = preferences;
    }

    public string ServerAddress
    {
        get => preferences.Get<string>(nameof(ServerAddress), default!);
        set => preferences.Set(nameof(ServerAddress), value);
    }
}
