namespace KeySample.FormsApp.Modules.Control
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using KeySample.FormsApp.Models.Input;

    using Smart.Navigation;

    public class ControlEntryViewModel : AppViewModelBase
    {
        public InputModel Input1 { get; }
        public InputModel Input2 { get; }
        public InputModel Input3 { get; }

        public ICommand SwitchCommand { get; }
        public ICommand SetCommand { get; }

        public ICommand BackCommand { get; }

        public ControlEntryViewModel(
            ApplicationState applicationState)
            : base(applicationState)
        {
            Input1 = new InputModel(MakeDelegateCommand<InputCompleteEvent>(Input1Complete));
            Input2 = new InputModel(MakeDelegateCommand<InputCompleteEvent>(Input2Complete));
            Input3 = new InputModel(MakeDelegateCommand<InputCompleteEvent>(Input3Complete));

            SwitchCommand = MakeDelegateCommand(() => Input1.Enable = !Input1.Enable);
            SetCommand = MakeDelegateCommand(() => Input3.Text = "123");

            BackCommand = MakeAsyncCommand(OnNotifyBackAsync);
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.Menu);
        }

        private void Input1Complete(InputCompleteEvent ice)
        {
            ice.HasError = String.IsNullOrEmpty(Input1.Text);
            Debug.WriteLine($"**** Input1 completed {Input1.Text}");
        }

        private void Input2Complete(InputCompleteEvent ice)
        {
            ice.HasError = String.IsNullOrEmpty(Input2.Text);
            Debug.WriteLine($"**** Input2 completed {Input2.Text}");
        }

        private void Input3Complete(InputCompleteEvent ice)
        {
            ice.HasError = String.IsNullOrEmpty(Input3.Text);
            Debug.WriteLine($"**** Input3 completed {Input3.Text}");
        }
    }
}
