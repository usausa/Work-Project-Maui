namespace OnyxSample.Behaviors;

using CommunityToolkit.Maui.Views;

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
                controller.PreviewRequest += ControllerOnPreviewRequest;
                controller.CaptureRequest += ControllerOnCaptureRequest;

                AssociatedObject!.SetBinding(
                    CameraView.SelectedCameraProperty,
                    new Binding(nameof(ICameraController.CameraInfo), source: controller));
                AssociatedObject.SetBinding(
                    CameraView.CameraFlashModeProperty,
                    new Binding(nameof(ICameraController.CameraFlashMode), source: controller));
                AssociatedObject.SetBinding(
                    CameraView.ImageCaptureResolutionProperty,
                    new Binding(nameof(ICameraController.Resolution), source: controller));
                AssociatedObject.SetBinding(
                    CameraView.ZoomFactorProperty,
                    new Binding(nameof(ICameraController.Zoom), source: controller));
                AssociatedObject.SetBinding(
                    CameraView.IsTorchOnProperty,
                    new Binding(nameof(ICameraController.IsTorchOn), source: controller));
            }
        }

        protected override void OnDetachingFrom(CameraView bindable)
        {
            if (controller is not null)
            {
                controller.PreviewRequest -= ControllerOnPreviewRequest;
                controller.CaptureRequest -= ControllerOnCaptureRequest;
            }

            bindable.RemoveBinding(CameraView.SelectedCameraProperty);
            bindable.RemoveBinding(CameraView.CameraFlashModeProperty);
            bindable.RemoveBinding(CameraView.ImageCaptureResolutionProperty);
            bindable.RemoveBinding(CameraView.ZoomFactorProperty);
            bindable.RemoveBinding(CameraView.IsTorchOnProperty);

            controller = null;

            base.OnDetachingFrom(bindable);
        }

        private void ControllerOnPreviewRequest(object? sender, CameraPreviewEventArgs e)
        {
            if (AssociatedObject is null)
            {
                return;
            }

            e.Task = e.Enable ? StartCameraPreview(AssociatedObject) : StopCameraPreview(AssociatedObject);
        }

        private static async Task StartCameraPreview(CameraView cameraView)
        {
            await cameraView.StartCameraPreview(CancellationToken.None);
        }

        private static Task StopCameraPreview(CameraView cameraView)
        {
            cameraView.StopCameraPreview();
            return Task.CompletedTask;
        }

        private void ControllerOnCaptureRequest(object? sender, CameraCaptureEventArgs e)
        {
            var cameraView = AssociatedObject;
            if (cameraView is null)
            {
                return;
            }

            var capture = new CaptureObject(cameraView);
            e.Task = capture.CaptureAsync(e.Token);
        }

        private sealed class CaptureObject
        {
            private readonly TaskCompletionSource<Stream?> result = new();

            private readonly CameraView view;

            public CaptureObject(CameraView view)
            {
                this.view = view;
            }

            public async Task<Stream?> CaptureAsync(CancellationToken token)
            {
                view.MediaCaptured += OnMediaCaptured;
                view.MediaCaptureFailed += OnMediaCaptureFailed;
                await view.CaptureImage(token);
                return await result.Task;
            }

            private void OnMediaCaptured(object? sender, MediaCapturedEventArgs e) => OnMediaCaptured(e.Media);

            private void OnMediaCaptureFailed(object? sender, MediaCaptureFailedEventArgs e) => OnMediaCaptured(null);

            private void OnMediaCaptured(Stream? stream)
            {
                view.MediaCaptured -= OnMediaCaptured;
                view.MediaCaptureFailed -= OnMediaCaptureFailed;
                result.TrySetResult(stream);
            }
        }
    }
}
