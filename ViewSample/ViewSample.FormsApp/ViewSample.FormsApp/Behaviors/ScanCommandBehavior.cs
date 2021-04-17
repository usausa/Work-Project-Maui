namespace ViewSample.FormsApp.Behaviors
{
    using System;
    using System.Windows.Input;

    using Smart.Forms.Interactivity;

    using Xamarin.Essentials;
    using Xamarin.Forms;

    using ZXing;
    using ZXing.Net.Mobile.Forms;

    public sealed class ScanCommandBehavior : BehaviorBase<ZXingScannerView>
    {
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
            nameof(Command),
            typeof(ICommand),
            typeof(ScanCommandBehavior),
            null,
            propertyChanged: HandleCommandPropertyChanged);

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void OnAttachedTo(ZXingScannerView bindable)
        {
            base.OnAttachedTo(bindable);

            bindable.OnScanResult += OnScanResult;
        }

        protected override void OnDetachingFrom(ZXingScannerView bindable)
        {
            bindable.OnScanResult -= OnScanResult;

            if (Command is not null)
            {
                Command.CanExecuteChanged -= CommandOnCanExecuteChanged;
            }

            base.OnDetachingFrom(bindable);
        }

        private static void HandleCommandPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((ScanCommandBehavior)bindable).OnCommandPropertyChanged(oldValue as ICommand, newValue as ICommand);
        }

        private void OnCommandPropertyChanged(ICommand oldValue, ICommand newValue)
        {
            if (oldValue == newValue)
            {
                return;
            }

            if (oldValue is not null)
            {
                oldValue.CanExecuteChanged -= CommandOnCanExecuteChanged;
            }

            if (newValue is not null)
            {
                newValue.CanExecuteChanged += CommandOnCanExecuteChanged;
            }
        }

        private void CommandOnCanExecuteChanged(object sender, EventArgs e)
        {
            if (AssociatedObject is null)
            {
                return;
            }

            var command = Command;
            if (command is not null)
            {
                AssociatedObject.IsAnalyzing = command.CanExecute(null);
            }
        }

        private void OnScanResult(Result result)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var command = Command;
                if (command?.CanExecute(result.Text) ?? false)
                {
                    Vibration.Vibrate(TimeSpan.FromMilliseconds(250));

                    command.Execute(result.Text);
                }
            });
        }
    }
}
