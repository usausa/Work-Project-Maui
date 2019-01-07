namespace Business.FormsApp.Components.Device
{
    using System;
    using System.Reactive.Subjects;

    public abstract class DeviceManagerBase : IDeviceManager
    {
        private BehaviorSubject<bool> keyboardState = new BehaviorSubject<bool>(false);

        private Subject<bool> screenState = new Subject<bool>();

        private bool lastKeyboardState;

        public IObservable<bool> KeyboardState => keyboardState;

        public IObservable<bool> ScreenState => screenState;

        protected void RaiseKeyboardState(bool visible)
        {
            if (lastKeyboardState != visible)
            {
                lastKeyboardState = visible;
                keyboardState.OnNext(visible);
            }
        }

        protected void RaiseScreenState(bool on)
        {
            screenState.OnNext(on);
        }
    }
}
