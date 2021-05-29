namespace KeySample.FormsApp.Models.Input
{
    using System;
    using System.ComponentModel;
    using System.Windows.Input;

    using Smart.ComponentModel;

    public class InputCompleteEvent
    {
        public bool HasError { get; set; }
    }

    public interface IInputController : INotifyPropertyChanged
    {
        public event EventHandler<EventArgs> FocusRequested;

        public string Text { get; set; }

        public bool Enable { get; set; }

        public void HandleCompleted(InputCompleteEvent e);
    }

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

        void IInputController.HandleCompleted(InputCompleteEvent e)
        {
            if ((command is not null) && command.CanExecute(e))
            {
                command.Execute(e);
            }
        }
    }
}
