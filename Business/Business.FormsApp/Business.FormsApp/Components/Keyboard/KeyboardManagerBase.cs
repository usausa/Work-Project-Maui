namespace Business.FormsApp.Components.Keyboard
{
    using System;
    using System.Reactive.Subjects;

    public abstract class KeyboardManagerBase : IKeyboardManager
    {
        private BehaviorSubject<bool> stateChanged = new BehaviorSubject<bool>(false);

        private bool lastValue;

        public IObservable<bool> StateChanged => stateChanged;

        protected void RaiseStateChanged(bool visible)
        {
            if (lastValue != visible)
            {
                lastValue = visible;
                stateChanged.OnNext(visible);
            }
        }
    }
}
