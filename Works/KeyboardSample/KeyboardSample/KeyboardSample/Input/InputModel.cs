namespace KeyboardSample.Input
{
    using System;
    using System.Windows.Input;

    using Smart.ComponentModel;

    public class InputModel : NotificationObject, IInputController
    {
        private event EventHandler<EventArgs> Requested;

        private readonly ICommand command;

        private string text;

        private bool enable;

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        public bool Enable
        {
            get => enable;
            set => SetProperty(ref enable, value);
        }

        public InputModel()
            : this(true, null)
        {
        }

        public InputModel(bool enable)
            : this(enable, null)
        {
        }

        public InputModel(ICommand command)
            : this(true, command)
        {
        }

        public InputModel(bool enable, ICommand command)
        {
            this.enable = enable;
            this.command = command;
        }

        public void FocusRequest()
        {
            Requested?.Invoke(this, EventArgs.Empty);
        }

        event EventHandler<EventArgs> IInputController.FocusRequested
        {
            add => Requested += value;
            remove => Requested -= value;
        }

        void IInputController.OnCompleted(string value)
        {
            if ((command != null) && command.CanExecute(value))
            {
                command.Execute(value);
            }
        }
    }
}
