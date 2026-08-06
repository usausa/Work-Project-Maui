namespace Template.MobileApp.Services;

public static class ApiNames
{
    public const string Default = nameof(Default);

    // ファイル転送用 (タイムアウトなし。中断はCancellationTokenで制御)
    public const string Transfer = nameof(Transfer);
}
