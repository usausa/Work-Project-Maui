namespace KeyboardSample.Droid.Helpers
{
    using System;

    public static class JavaObjectExtensions
    {
        public static bool IsDisposed(this Java.Lang.Object obj)
        {
            return obj.Handle == IntPtr.Zero;
        }
    }
}
