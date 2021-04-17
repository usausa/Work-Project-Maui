namespace ViewSample.FormsApp.Behaviors
{
    using Smart.Forms.Interactivity;
    using Smart.Forms.Messaging;

    using ViewSample.FormsApp.Helpers;
    using ViewSample.FormsApp.Messaging;

    using Xamarin.CommunityToolkit.UI.Views;
    using Xamarin.Forms;

    public sealed class CameraCaptureBehavior : BehaviorBase<CameraView>
    {
        public static readonly BindableProperty RequestProperty = BindableProperty.Create(
            nameof(Request),
            typeof(IEventRequest<CameraCaptureEventArgs>),
            typeof(CameraCaptureBehavior),
            null,
            propertyChanged: HandleRequestPropertyChanged);

        public static readonly BindableProperty MaxSizeProperty = BindableProperty.CreateAttached(
            nameof(MaxSize),
            typeof(int),
            typeof(CameraCaptureBehavior),
            1024);

        public static readonly BindableProperty QualityProperty = BindableProperty.CreateAttached(
            nameof(Quality),
            typeof(int),
            typeof(CameraCaptureBehavior),
            85);

        public IEventRequest<CameraCaptureEventArgs> Request
        {
            get => (IEventRequest<CameraCaptureEventArgs>)GetValue(RequestProperty);
            set => SetValue(RequestProperty, value);
        }

        public int MaxSize
        {
            get => (int)GetValue(MaxSizeProperty);
            set => SetValue(MaxSizeProperty, value);
        }

        public int Quality
        {
            get => (int)GetValue(QualityProperty);
            set => SetValue(QualityProperty, value);
        }

        protected override void OnDetachingFrom(CameraView bindable)
        {
            if (Request is not null)
            {
                Request.Requested -= EventRequestOnRequested;
            }

            base.OnDetachingFrom(bindable);
        }

        private static void HandleRequestPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((CameraCaptureBehavior)bindable).OnMessengerPropertyChanged(oldValue as IEventRequest<CameraCaptureEventArgs>, newValue as IEventRequest<CameraCaptureEventArgs>);
        }

        private void OnMessengerPropertyChanged(IEventRequest<CameraCaptureEventArgs> oldValue, IEventRequest<CameraCaptureEventArgs> newValue)
        {
            if (oldValue == newValue)
            {
                return;
            }

            if (oldValue is not null)
            {
                oldValue.Requested -= EventRequestOnRequested;
            }

            if (newValue is not null)
            {
                newValue.Requested += EventRequestOnRequested;
            }
        }

        private void EventRequestOnRequested(object sender, CameraCaptureEventArgs ea)
        {
            async void MediaCaptured(object s, MediaCapturedEventArgs e)
            {
                var camera = (CameraView)s;
                camera.MediaCaptured -= MediaCaptured;
                camera.MediaCaptureFailed -= MediaCaptureFailed;

                var image = await ImageHelper.NormalizeImageAsync(e.ImageData, MaxSize, e.Rotation, Quality);
                ea.CompletionSource.TrySetResult(image);
            }

            void MediaCaptureFailed(object s, string e)
            {
                var camera = (CameraView)s;
                camera.MediaCaptured -= MediaCaptured;
                camera.MediaCaptureFailed -= MediaCaptureFailed;

                ea.CompletionSource.TrySetResult(null);
            }

            AssociatedObject.MediaCaptured += MediaCaptured;
            AssociatedObject.MediaCaptureFailed += MediaCaptureFailed;

            AssociatedObject.Shutter();
        }
    }
}
