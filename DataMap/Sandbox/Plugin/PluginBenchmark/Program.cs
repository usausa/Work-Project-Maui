namespace PluginBenchmark
{
    using System;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Running;

    using Smart.Collections.Concurrent;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<Benchmark>();
        }
    }

    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            Add(MarkdownExporter.Default, MarkdownExporter.GitHub);
            Add(MemoryDiagnoser.Default);
            Add(Job.ShortRun);
        }
    }

    [Config(typeof(BenchmarkConfig))]
    public class Benchmark
    {
        private readonly Config config = new Config();

        [GlobalSetup]
        public void Setup()
        {
            config.AddHashPlugin(new ProcedurePlugin());
            config.AddArrayPlugin(new ProcedurePlugin());
        }

        [Benchmark]
        public void Hash()
        {
            config.ExecuteHash();
        }

        [Benchmark]
        public void Array()
        {
            config.ExecuteArray();
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
        private readonly ThreadsafeTypeHashArrayMap<object> hash = new ThreadsafeTypeHashArrayMap<object>();

        private object[] plugins = new object[0];

        public void AddHashPlugin<T>(T plugin)
        {
            hash.AddIfNotExist(typeof(T), plugin);
        }

        public T GetHashPlugin<T>()
        {
            return (T)hash.GetValueOrDefault(typeof(T));
        }

        public void AddArrayPlugin<T>(T plugin)
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

        public T GetArrayPlugin<T>(int slot)
        {
            return (T)plugins[slot];
        }
    }

    public class ProcedurePlugin
    {
        public void Execute()
        {
        }
    }

    public static class EngineProcedureExtension
    {
        private static readonly int Slot;

        static EngineProcedureExtension()
        {
            Slot = SlotHolder<ProcedurePlugin>.Slot;
        }

        public static void ExecuteHash(this Config config)
        {
            var plugin = config.GetHashPlugin<ProcedurePlugin>();
            plugin.Execute();
        }

        public static void ExecuteArray(this Config config)
        {
            var plugin = config.GetArrayPlugin<ProcedurePlugin>(Slot);
            plugin.Execute();
        }
    }
}
