namespace Smart.Data.Mapper
{
    public static class ExtensionSlotManager
    {
        private static readonly object Sync = new object();

        private static int next;

        public static int Allocate()
        {
            lock (Sync)
            {
                next++;
                return next;
            }
        }
    }
}
