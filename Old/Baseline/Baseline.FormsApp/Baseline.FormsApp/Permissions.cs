namespace Baseline.FormsApp
{
    using System.Threading.Tasks;

    using Plugin.Permissions;
    using Plugin.Permissions.Abstractions;

    public static class Permissions
    {
        public static readonly Permission[] RequiredPermissions =
        {
            Permission.Camera,
            Permission.Location,
            Permission.Storage
        };

        public static async Task<bool> IsPermissionRequired()
        {
            foreach (var permission in RequiredPermissions)
            {
                var result = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
                if (result != PermissionStatus.Granted)
                {
                    return true;
                }
            }

            return false;
        }

        public static async Task<bool> RequestPermissions()
        {
            var result = await CrossPermissions.Current.RequestPermissionsAsync(RequiredPermissions);
            foreach (var permission in RequiredPermissions)
            {
                if (!result.TryGetValue(permission, out var status) ||
                    (status != PermissionStatus.Granted))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
