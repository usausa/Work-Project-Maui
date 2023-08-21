namespace WorkCamera;

using System.Windows.Input;

using Camera.MAUI;
using Camera.MAUI.ZXingHelper;

using Smart.ComponentModel;
using Smart.Maui.Interactivity;

public static class CameraBind
{
    public static readonly BindableProperty ControllerProperty = BindableProperty.CreateAttached(
        "Controller",
        typeof(ICameraController),
        typeof(CameraBind),
        null,
        propertyChanged: BindChanged);

    public static ICameraController GetController(BindableObject bindable) =>
        (ICameraController)bindable.GetValue(ControllerProperty);

    public static void SetController(BindableObject bindable, ICameraController value) =>
        bindable.SetValue(ControllerProperty, value);

    private static void BindChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (bindable is not CameraView view)
        {
            return;
        }

        if (oldValue is not null)
        {
            var behavior = view.Behaviors.FirstOrDefault(static x => x is CameraBindBehavior);
            if (behavior is not null)
            {
                view.Behaviors.Remove(behavior);
            }
        }

        if (newValue is not null)
        {
            view.Behaviors.Add(new CameraBindBehavior());
        }
    }

    private sealed class CameraBindBehavior : BehaviorBase<CameraView>
    {
        private ICameraController? controller;

        protected override void OnAttachedTo(CameraView bindable)
        {
            base.OnAttachedTo(bindable);

            controller = GetController(bindable);
            if (controller is not null)
            {
                controller.PositionRequest += ControllerOnPositionRequest;
                controller.TakePhotoRequest += ControllerOnTakePhotoRequest;
                controller.SaveSnapshotRequest += ControllerOnSaveSnapshotRequest;
                controller.FocusRequest += ControllerOnFocusRequest;
            }

            bindable.CamerasLoaded += BindableOnCamerasLoaded;
            bindable.BarcodeDetected += BindableOnBarcodeDetected;
        }

        private void BindableOnBarcodeDetected(object sender, BarcodeEventArgs args)
        {
            if ((controller is null) || (args.Result.Length == 0))
            {
                return;
            }

            controller.HandleBarcodeDetected(args.Result[0]);
        }

        protected override void OnDetachingFrom(CameraView bindable)
        {
            if (controller is not null)
            {
                controller.PositionRequest -= ControllerOnPositionRequest;
                controller.TakePhotoRequest -= ControllerOnTakePhotoRequest;
                controller.SaveSnapshotRequest -= ControllerOnSaveSnapshotRequest;
                controller.FocusRequest -= ControllerOnFocusRequest;
            }

            bindable.CamerasLoaded -= BindableOnCamerasLoaded;
            bindable.BarcodeDetected -= BindableOnBarcodeDetected;

            bindable.RemoveBinding(CameraView.CameraProperty);
            bindable.RemoveBinding(CameraView.AutoStartPreviewProperty);
            bindable.RemoveBinding(CameraView.TorchEnabledProperty);
            bindable.RemoveBinding(CameraView.MirroredImageProperty);
            bindable.RemoveBinding(CameraView.FlashModeProperty);
            bindable.RemoveBinding(CameraView.ZoomFactorProperty);
            bindable.RemoveBinding(CameraView.BarCodeDetectionEnabledProperty);

            controller = null;

            base.OnDetachingFrom(bindable);
        }

        private void BindableOnCamerasLoaded(object? sender, EventArgs e)
        {
            if (AssociatedObject is null)
            {
                return;
            }

            if (controller is null)
            {
                return;
            }

            var camera = controller.DefaultPosition is not null
                ? AssociatedObject.Cameras.FirstOrDefault(x => x.Position == controller.DefaultPosition)
                : AssociatedObject.Cameras.FirstOrDefault();
            AssociatedObject.Camera = camera;

            AssociatedObject.SetBinding(
                CameraView.AutoStartPreviewProperty,
                new Binding(nameof(ICameraController.Preview), source: controller));

            AssociatedObject.SetBinding(
                CameraView.TorchEnabledProperty,
                new Binding(nameof(ICameraController.Torch), source: controller));
            AssociatedObject.SetBinding(
                CameraView.MirroredImageProperty,
                new Binding(nameof(ICameraController.Mirror), source: controller));
            AssociatedObject.SetBinding(
                CameraView.FlashModeProperty,
                new Binding(nameof(ICameraController.FlashMode), source: controller));
            AssociatedObject.SetBinding(
                CameraView.ZoomFactorProperty,
                new Binding(nameof(ICameraController.Zoom), source: controller));
            AssociatedObject.SetBinding(
                CameraView.BarCodeDetectionEnabledProperty,
                new Binding(nameof(ICameraController.BarcodeDetection), source: controller));

            controller.UpdateCamera(camera);
        }

        private void ControllerOnPositionRequest(object? sender, PositionEventArgs e)
        {
            if ((AssociatedObject is null) || (controller is null))
            {
                return;
            }

            e.Task = PositionRequest(AssociatedObject, controller, e.Position);
        }

        private static async Task PositionRequest(CameraView cameraView, ICameraController controller, CameraPosition? position)
        {
            CameraInfo? newCamera;
            if (position is not null)
            {
                newCamera = cameraView.Cameras.FirstOrDefault(x => x.Position == position);
            }
            else
            {
                var current = cameraView.Cameras.IndexOf(cameraView.Camera);
                newCamera = (current >= 0) && (current + 1 < cameraView.Cameras.Count)
                    ? cameraView.Cameras[current + 1]
                    : cameraView.Cameras.FirstOrDefault();
            }

            if (cameraView.Camera == newCamera)
            {
                return;
            }

            var start = cameraView.AutoStartPreview;
            if (start)
            {
                await cameraView.StopCameraAsync();
            }

            cameraView.Camera = newCamera;

            if (start)
            {
                await cameraView.StartCameraAsync();
            }

            controller.UpdateCamera(newCamera);
        }

        private void ControllerOnTakePhotoRequest(object? sender, TakePhotoEventArgs e)
        {
            var cameraView = AssociatedObject;
            if (cameraView is null)
            {
                return;
            }

            e.Task = cameraView.TakePhotoAsync(e.Format);
        }

        private void ControllerOnSaveSnapshotRequest(object? sender, SaveSnapshotEventArgs e)
        {
            var cameraView = AssociatedObject;
            if (cameraView is null)
            {
                return;
            }

            e.Task = cameraView.SaveSnapShot(e.Format, e.Path);
        }

        private void ControllerOnFocusRequest(object? sender, EventArgs e)
        {
            AssociatedObject?.ForceAutoFocus();
        }
    }
}

public class TaskEventArgs : EventArgs
{
    public Task Task { get; set; } = Task.CompletedTask;
}

public class TaskEventArgs<T> : EventArgs
{
    private static readonly Task<T> CompletedTask = System.Threading.Tasks.Task.FromResult(default(T)!);

    public Task<T> Task { get; set; } = CompletedTask;
}

public sealed class PositionEventArgs : TaskEventArgs
{
    public CameraPosition? Position { get; set; }
}

public sealed class TakePhotoEventArgs : TaskEventArgs<Stream?>
{
    public ImageFormat Format { get; set; } = ImageFormat.JPEG;
}

public sealed class SaveSnapshotEventArgs : TaskEventArgs<bool>
{
    public string Path { get; set; } = default!;

    public ImageFormat Format { get; set; } = ImageFormat.JPEG;
}

public interface ICameraController
{
    event EventHandler<PositionEventArgs> PositionRequest;

    event EventHandler<TakePhotoEventArgs> TakePhotoRequest;

    event EventHandler<SaveSnapshotEventArgs> SaveSnapshotRequest;

    event EventHandler<EventArgs> FocusRequest;

    CameraPosition? DefaultPosition { get; }

    CameraInfo? Camera { get; }

    void UpdateCamera(CameraInfo? value);

    bool Preview { get; set; }

    bool Torch { get; set; }

    bool Mirror { get; set; }

    FlashMode FlashMode { get; set; }

    float Zoom { get; set; }

    bool BarcodeDetection { get; set; }

    Task<Stream?> TakePhotoAsync(ImageFormat imageFormat = ImageFormat.JPEG);

    Task<bool> SaveSnapshotAsync(string path, ImageFormat imageFormat = ImageFormat.JPEG);

    void HandleBarcodeDetected(ZXing.Result result);
}

public sealed class CameraController : NotificationObject, ICameraController
{
    private event EventHandler<PositionEventArgs>? PositionRequestHandler;

    private event EventHandler<TakePhotoEventArgs>? TakePhotoRequestHandler;

    private event EventHandler<SaveSnapshotEventArgs>? SaveSnapshotRequestHandler;

    private event EventHandler<EventArgs>? FocusRequestHandler;

    event EventHandler<PositionEventArgs> ICameraController.PositionRequest
    {
        add => PositionRequestHandler += value;
        remove => PositionRequestHandler -= value;
    }

    event EventHandler<TakePhotoEventArgs> ICameraController.TakePhotoRequest
    {
        add => TakePhotoRequestHandler += value;
        remove => TakePhotoRequestHandler -= value;
    }

    event EventHandler<SaveSnapshotEventArgs> ICameraController.SaveSnapshotRequest
    {
        add => SaveSnapshotRequestHandler += value;
        remove => SaveSnapshotRequestHandler -= value;
    }

    event EventHandler<EventArgs> ICameraController.FocusRequest
    {
        add => FocusRequestHandler += value;
        remove => FocusRequestHandler -= value;
    }

    private readonly ICommand? command;

    private readonly CameraPosition? defaultPosition;

    CameraPosition? ICameraController.DefaultPosition => defaultPosition;

    private CameraInfo? camera;

    public CameraInfo? Camera => camera;

    private bool preview;

    public bool Preview
    {
        get => preview;
        set => SetProperty(ref preview, value);
    }

    private bool torch;

    public bool Torch
    {
        get => torch;
        set => SetProperty(ref torch, value);
    }

    private bool mirror;

    public bool Mirror
    {
        get => mirror;
        set => SetProperty(ref mirror, value);
    }

    private FlashMode flashMode;

    public FlashMode FlashMode
    {
        get => flashMode;
        set => SetProperty(ref flashMode, value);
    }

    private float zoom = 1f;

    public float Zoom
    {
        get => zoom;
        set
        {
            if (Camera is null)
            {
                value = 1f;
            }
            else
            {
                if (value < Camera.MinZoomFactor)
                {
                    value = Camera.MinZoomFactor;
                }
                else if (value > Camera.MaxZoomFactor)
                {
                    value = Camera.MaxZoomFactor;
                }
            }

            SetProperty(ref zoom, value);
        }
    }

    private bool barcodeDetection;

    public bool BarcodeDetection
    {
        get => barcodeDetection;
        set => SetProperty(ref barcodeDetection, value);
    }

    public CameraController(CameraPosition? position = null, ICommand? command = null)
    {
        defaultPosition = position;
        this.command = command;
    }

    public async Task ResetPositionAsync()
    {
        var args = new PositionEventArgs { Position = defaultPosition };
        PositionRequestHandler?.Invoke(this, args);
        await args.Task;
    }

    public Task SwitchPositionAsync(CameraPosition? position = null)
    {
        var args = new PositionEventArgs { Position = position };
        PositionRequestHandler?.Invoke(this, args);
        return args.Task;
    }

    void ICameraController.UpdateCamera(CameraInfo? value)
    {
        camera = value;
        RaisePropertyChanged(nameof(Camera));
    }

    public Task<Stream?> TakePhotoAsync(ImageFormat imageFormat = ImageFormat.JPEG)
    {
        var args = new TakePhotoEventArgs { Format = imageFormat };
        TakePhotoRequestHandler?.Invoke(this, args);
        return args.Task;
    }

    public Task<bool> SaveSnapshotAsync(string path, ImageFormat imageFormat = ImageFormat.JPEG)
    {
        var args = new SaveSnapshotEventArgs { Path = path, Format = imageFormat };
        SaveSnapshotRequestHandler?.Invoke(this, args);
        return args.Task;
    }

    public void FocusRequest()
    {
        FocusRequestHandler?.Invoke(this, EventArgs.Empty);
    }

    void ICameraController.HandleBarcodeDetected(ZXing.Result result)
    {
        if ((command is not null) && command.CanExecute(result))
        {
            command.Execute(result);
        }
    }
}
