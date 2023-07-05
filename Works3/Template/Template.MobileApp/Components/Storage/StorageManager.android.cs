namespace Template.MobileApp.Components.Storage;

public sealed partial class StorageManager
{
    private partial string ResolvePublicFolder() => AndroidHelper.GetExternalFilesDir();
}
