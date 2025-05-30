using CommunityToolkit.Maui.Core;

using System.ComponentModel;

namespace WorkNewCamera;

using CommunityToolkit.Maui.Core.Primitives;

using Smart.Mvvm;

public abstract class ValueTaskEventArgs : EventArgs
{
    public ValueTask Task { get; set; } = ValueTask.CompletedTask;
}

public abstract class ValueTaskEventArgs<T> : EventArgs
{
    private static readonly ValueTask<T> CompletedTask = ValueTask.FromResult(default(T)!);

    public ValueTask<T> Task { get; set; } = CompletedTask;
}

public sealed class CameraPreviewEventArgs : ValueTaskEventArgs
{
    public bool Enable { get; set; }
}

public sealed class CameraCaptureEventArgs : ValueTaskEventArgs<Stream?>
{
    public CancellationToken Token { get; set; } = CancellationToken.None;
}

public sealed class CameraGetAvailableListEventArgs : ValueTaskEventArgs<IReadOnlyList<CameraInfo>>
{
    public CancellationToken Token { get; set; } = CancellationToken.None;

    public IReadOnlyList<CameraInfo> CameraList { get; set; } = [];
}

public sealed partial class CameraController : ObservableObject
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler<CameraGetAvailableListEventArgs>? GetAvailableListRequest;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler<CameraPreviewEventArgs>? PreviewRequest;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler<CameraCaptureEventArgs>? CaptureRequest;

    [ObservableProperty]
    public partial bool IsAvailable { get; set; }

    [ObservableProperty]
    public partial bool IsCameraBusy { get; set; }

    [ObservableProperty]
    public partial CameraInfo? Selected { get; set; }

    [ObservableProperty]
    public partial CameraFlashMode CameraFlashMode { get; set; } = CameraViewDefaults.CameraFlashMode;

    [ObservableProperty]
    public partial Size CaptureResolution { get; set; } = CameraViewDefaults.ImageCaptureResolution;

    [ObservableProperty]
    public partial float ZoomFactor { get; set; } = CameraViewDefaults.ZoomFactor;

    [ObservableProperty]
    public partial bool IsTorchOn { get; set; }

    // Message

    public ValueTask<IReadOnlyList<CameraInfo>> GetAvailableListAsync(CancellationToken token = default)
    {
        var args = new CameraGetAvailableListEventArgs
        {
            Token = token,
            CameraList = []
        };
        GetAvailableListRequest?.Invoke(this, args);
        return args.Task;
    }

    public ValueTask StartPreviewAsync()
    {
        var args = new CameraPreviewEventArgs { Enable = true };
        PreviewRequest?.Invoke(this, args);
        return args.Task;
    }

    public ValueTask StopPreviewAsync()
    {
        var args = new CameraPreviewEventArgs();
        PreviewRequest?.Invoke(this, args);
        return args.Task;
    }

    public ValueTask<Stream?> CaptureAsync(CancellationToken token = default)
    {
        var args = new CameraCaptureEventArgs
        {
            Token = token
        };
        CaptureRequest?.Invoke(this, args);
        return args.Task;
    }
}
