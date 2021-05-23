namespace WorkCustomMove
{
    using System;

    public class ForwardEventArgs : EventArgs
    {
        public bool Forward { get; }

        public bool Handled { get; set; }

        public ForwardEventArgs(bool forward)
        {
            Forward = forward;
        }
    }

    public class KeyManager
    {
        public event EventHandler<ForwardEventArgs> Forward;

        public bool RaiseForward(bool forward)
        {
            var args = new ForwardEventArgs(forward);
            Forward?.Invoke(this, args);
            return args.Handled;
        }
    }
}
