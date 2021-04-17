namespace ViewSample.FormsApp.Messaging
{
    using System;
    using System.Threading.Tasks;

    using Smart.Forms.Messaging;

    public sealed class CameraCaptureRequest : IEventRequest<CameraCaptureEventArgs>
    {
        public event EventHandler<CameraCaptureEventArgs> Requested;

        public Task<byte[]> CaptureAsync()
        {
            var args = new CameraCaptureEventArgs();
            Requested?.Invoke(this, args);
            return args.CompletionSource.Task;
        }
    }
}
