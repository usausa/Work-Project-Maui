namespace OnyxSample.Messaging;

using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives;

using OnyxSample.Helpers;

public sealed class CameraPreviewEventArgs : TaskEventArgs
{
    public bool Enable { get; set; }
}

public sealed class CameraCaptureEventArgs : TaskEventArgs<Stream?>
{
    public CancellationToken Token { get; set; } = CancellationToken.None;
}

public interface ICameraController
{
    event EventHandler<CameraPreviewEventArgs> PreviewRequest;

    event EventHandler<CameraCaptureEventArgs> CaptureRequest;

    // Property

    CameraInfo? CameraInfo { get; set; }

    CameraFlashMode CameraFlashMode { get; set; }

    Size Resolution { get; set; }

    float Zoom { get; set; }

    bool IsTorchOn { get; set; }
}

public sealed class CameraController : NotificationObject, ICameraController
{
    private event EventHandler<CameraPreviewEventArgs>? PreviewRequestHandler;

    private event EventHandler<CameraCaptureEventArgs>? CaptureRequestHandler;

    event EventHandler<CameraPreviewEventArgs> ICameraController.PreviewRequest
    {
        add => PreviewRequestHandler += value;
        remove => PreviewRequestHandler -= value;
    }

    event EventHandler<CameraCaptureEventArgs> ICameraController.CaptureRequest
    {
        add => CaptureRequestHandler += value;
        remove => CaptureRequestHandler -= value;
    }

    // Property

    private CameraInfo? cameraInfo;

    public CameraInfo? CameraInfo
    {
        get => cameraInfo;
        set => SetProperty(ref cameraInfo, value);
    }

    private CameraFlashMode cameraFlashMode;

    public CameraFlashMode CameraFlashMode
    {
        get => cameraFlashMode;
        set => SetProperty(ref cameraFlashMode, value);
    }

    private Size resolution;

    public Size Resolution
    {
        get => resolution;
        set => SetProperty(ref resolution, value);
    }

    private float zoom = 1f;

    public float Zoom
    {
        get => zoom;
        set
        {
            if (CameraInfo is null)
            {
                value = 1;
            }
            else if (value < CameraInfo.MinimumZoomFactor)
            {
                value = CameraInfo.MinimumZoomFactor;
            }
            else if (value > CameraInfo.MaximumZoomFactor)
            {
                value = CameraInfo.MaximumZoomFactor;
            }

            SetProperty(ref zoom, value);
        }
    }

    private bool isTorchOn;

    public bool IsTorchOn
    {
        get => isTorchOn;
        set => SetProperty(ref isTorchOn, value);
    }

    // Message

    public Task StartPreviewAsync()
    {
        var args = new CameraPreviewEventArgs { Enable = true };
        PreviewRequestHandler?.Invoke(this, args);
        return args.Task;
    }

    public Task StopPreviewAsync()
    {
        var args = new CameraPreviewEventArgs();
        PreviewRequestHandler?.Invoke(this, args);
        return args.Task;
    }

    public Task<Stream?> CaptureAsync(CancellationToken token = default)
    {
        var args = new CameraCaptureEventArgs
        {
            Token = token
        };
        CaptureRequestHandler?.Invoke(this, args);
        return args.Task;
    }
}
