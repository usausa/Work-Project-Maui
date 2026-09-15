#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace OtelClient.Services;

using Android.Runtime;

public sealed partial class TelemetryHost
{
    // Java 側へ伝播する前に呼ばれる。ここで送っておかないとプロセスが落ちて失われる
    partial void PlatformRegisterCrashHandler()
    {
        AndroidEnvironment.UnhandledExceptionRaiser += OnAndroidUnhandledException;
    }

    private void OnAndroidUnhandledException(object? sender, RaiseThrowableEventArgs e)
    {
        ReportCrash(e.Exception);
    }
}
