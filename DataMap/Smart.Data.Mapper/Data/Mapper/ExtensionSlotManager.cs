namespace Smart.Data.Mapper
{
    public static class ExtensionSlotManager
    {
        private static readonly object Sync = new object();

        private static int next;

        private static int Allocate()
        {
            lock (Sync)
            {
                next++;
                return next;
            }
        }

        // ReSharper disable once UnusedTypeParameter
        private static class SlotHolder<T>
        {
            public static int Slot { get; }

            static SlotHolder()
            {
                Slot = Allocate();
            }
        }

        public static int GetSlot<T>()
        {
            return SlotHolder<T>.Slot;
        }
    }
}
