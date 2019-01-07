namespace Business.FormsApp.Components.Keyboard
{
    using System;

    public interface IKeyboardManager
    {
        IObservable<bool> StateChanged { get; }
    }
}
