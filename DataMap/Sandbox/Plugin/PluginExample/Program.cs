namespace PluginExample
{
    using System;
    using System.Diagnostics;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var config = new Config();
            config.AddPlugin(new ProcedurePlugin());

            config.Execute();
        }
    }

    public static class ExtensionSlotManager
    {
        private static readonly object Sync = new object();

        private static int next;

        public static int Allocate()
        {
            lock (Sync)
            {
                return next++;
            }
        }
    }

    public sealed class SlotHolder<T>
    {
        public static Type SlotType { get; }

        public static int Slot { get; }

        static SlotHolder()
        {
            SlotType = typeof(T);
            Slot = ExtensionSlotManager.Allocate();
        }
    }

    public class Config
    {
        private object[] plugins = new object[0];

        public void AddPlugin<T>(T plugin)
        {
            var slot = SlotHolder<ProcedurePlugin>.Slot;
            if (slot >= plugins.Length)
            {
                var newPlugins = new object[slot + 1];
                Array.Copy(plugins, 0, newPlugins, 0, plugins.Length);
                plugins = newPlugins;
            }

            plugins[slot] = plugin;
        }

        public T GetPlugin<T>(int slot)
        {
            return (T)plugins[slot];
        }
    }

    public class ProcedurePlugin
    {
        public void Execute()
        {
            Debug.WriteLine("Call");
        }
    }

    public static class EngineProcedureExtension
    {
        private static readonly int Slot;

        static EngineProcedureExtension()
        {
            Slot = SlotHolder<ProcedurePlugin>.Slot;
        }

        public static void Execute(this Config config)
        {
            var plugin = config.GetPlugin<ProcedurePlugin>(Slot);
            plugin.Execute();
        }
    }
}
