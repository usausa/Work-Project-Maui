using System;

namespace ArrayAllocBenchmark
{
    using System.Buffers;
    using System.Threading;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Running;

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
        private static readonly ThreadLocal<string[]> StaticArray = new ThreadLocal<string[]>(() => new string[10]);

        [ThreadStatic]
        private static string[] staticArray2;

        [Benchmark]
        public void Alloc()
        {
            var array = new string[10];

            for (var i = 0; i < array.Length; i++)
            {
                array[i] = string.Empty;
            }
        }

        [Benchmark]
        public void Pool()
        {
            var array = ArrayPool<string>.Shared.Rent(10);

            for (var i = 0; i < array.Length; i++)
            {
                array[i] = string.Empty;
            }

            ArrayPool<string>.Shared.Return(array);
        }

        [Benchmark]
        public void ThreadStatic()
        {
            var array = StaticArray.Value;
            if (array.Length < 10)
            {
                array = new string[10];
                StaticArray.Value = array;
            }

            for (var i = 0; i < 10; i++)
            {
                array[i] = string.Empty;
            }
        }

        [Benchmark]
        public void ThreadStatic2()
        {
            var array = staticArray2;
            if ((array == null) || (array.Length < 10))
            {
                array = new string[10];
                staticArray2 = array;
            }

            for (var i = 0; i < 10; i++)
            {
                array[i] = string.Empty;
            }
        }
    }
}
