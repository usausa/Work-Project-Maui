namespace KeyboardSample.Input
{
    using System;

    public interface IInputController
    {
        public event EventHandler<EventArgs> FocusRequested;

        public void OnCompleted(string value);
    }
}
