using System;
using System.Diagnostics;
using System.Windows.Input;

using Smart.ComponentModel;
using Smart.Forms.ViewModels;

namespace WorkEntry
{
    public class MainPageViewModel : ViewModelBase
    {
        public InputModel Input1 { get; }
        public InputModel Input2 { get; }
        public InputModel Input3 { get; }

        public ICommand SwitchCommand { get; }
        public ICommand TextCommand { get; }

        public MainPageViewModel()
        {
            Input1 = new InputModel(MakeDelegateCommand<InputCompleteEvent>(x =>
            {
                Debug.WriteLine($"**** Input1 completed {Input1.Text}");
            }));
            Input2 = new InputModel(MakeDelegateCommand<InputCompleteEvent>(x =>
            {
                x.HasError = String.IsNullOrEmpty(Input2.Text);
                Debug.WriteLine($"**** Input2 completed {Input2.Text}");
            }));
            Input3 = new InputModel(MakeDelegateCommand<InputCompleteEvent>(x =>
            {
                Debug.WriteLine($"**** Input3 completed {Input3.Text}");
            }));

            SwitchCommand = MakeDelegateCommand(() =>
            {
                Input1.Enable = !Input1.Enable;
            });
            TextCommand = MakeDelegateCommand(() =>
            {
                Input3.Text = "123";
            });
        }
    }
}
