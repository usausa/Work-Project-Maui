namespace KeySample.FormsApp.Input
{
    using System.Collections.Generic;

    public class KeyInputManager
    {
        public static KeyInputManager Default { get; } = new();

        private readonly List<IKeyInputHandler> handlers = new();

        public void PushHandler(IKeyInputHandler handler)
        {
            handlers.Add(handler);
        }

        public void PopHandler(IKeyInputHandler handler)
        {
            handlers.Remove(handler);
        }

        public bool Process(KeyCode key)
        {
            return handlers.Count > 0 && handlers[^1].Handle(key);
        }
    }
}
