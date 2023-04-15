using System;

namespace SlotExample
{
    public static class SlotManager
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

    public class Extension1
    {
        private static readonly int Slot;

        static Extension1()
        {
            Slot = SlotManager.Allocate();
        }

        public int GetSlot()
        {
            return Slot;
        }
    }

    public class Extension2
    {
        private static readonly int Slot;

        static Extension2()
        {
            Slot = SlotManager.Allocate();
        }

        public int GetSlot()
        {
            return Slot;
        }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            var extension2 = new Extension2();
            var extension1 = new Extension1();
            Console.WriteLine(extension1.GetSlot());
            Console.WriteLine(extension2.GetSlot());
        }
    }
}
