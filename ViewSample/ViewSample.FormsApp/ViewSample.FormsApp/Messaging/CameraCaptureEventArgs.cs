namespace ViewSample.FormsApp.Messaging
{
    using System;
    using System.Threading.Tasks;

    public sealed class CameraCaptureEventArgs : EventArgs
    {
        public TaskCompletionSource<byte[]> CompletionSource { get; } = new();
    }
}
