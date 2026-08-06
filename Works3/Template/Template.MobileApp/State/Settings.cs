namespace Template.MobileApp.State;

public sealed class Settings
{
    private const string AIServiceKeyName = "AIServiceKey";

    private readonly IPreferences preferences;

    private readonly ISecureStorage secureStorage;

    public Settings(IPreferences preferences, ISecureStorage secureStorage)
    {
        this.preferences = preferences;
        this.secureStorage = secureStorage;
    }

    // Id

    public string UniqueId
    {
        get => preferences.Get<string>(nameof(UniqueId), default!);
        set => preferences.Set(nameof(UniqueId), value);
    }

    // API

    public string ApiEndPoint
    {
        get => preferences.Get<string>(nameof(ApiEndPoint), default!);
        set => preferences.Set(nameof(ApiEndPoint), value);
    }

    public string MonitorEndPoint
    {
        get => preferences.Get<string>(nameof(MonitorEndPoint), default!);
        set => preferences.Set(nameof(MonitorEndPoint), value);
    }

    // AI Service

    public string AIServiceEndPoint
    {
        get => preferences.Get<string>(nameof(AIServiceEndPoint), default!);
        set => preferences.Set(nameof(AIServiceEndPoint), value);
    }

    // キー類は平文のPreferencesではなくSecureStorageに保存する
    public async ValueTask<string?> GetAIServiceKeyAsync()
    {
        // 旧バージョンがPreferencesに保存した値はSecureStorageへ移行する
        var legacy = preferences.Get<string?>(AIServiceKeyName, null);
        if (!String.IsNullOrEmpty(legacy))
        {
            await secureStorage.SetAsync(AIServiceKeyName, legacy);
            preferences.Remove(AIServiceKeyName);
            return legacy;
        }

        return await secureStorage.GetAsync(AIServiceKeyName);
    }

    public async ValueTask SetAIServiceKeyAsync(string value)
    {
        // SetAsyncは空文字を受け付けないため、クリアは削除として扱う
        if (String.IsNullOrEmpty(value))
        {
            secureStorage.Remove(AIServiceKeyName);
            return;
        }

        await secureStorage.SetAsync(AIServiceKeyName, value);
    }
}
