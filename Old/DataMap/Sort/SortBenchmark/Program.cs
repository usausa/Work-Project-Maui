namespace SortBenchmark
{
    using System;
    using System.Linq;

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
        private string[] columns;
        private string[] sorted;
        private string[] reverse;

        [GlobalSetup]
        public void Setup()
        {
            columns = new [] { "Id", "Name", "Amount", "Qty", "Data1", "Data2", "Flag1", "Flag2", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy" };
            sorted = columns.OrderBy(x => x).ToArray();
            reverse = columns.OrderByDescending(x => x).ToArray();
        }

        // 400ns

        [Benchmark]
        public void ArraySortDefaultBinary() => Array.Sort(columns, StringComparer.Ordinal);

        [Benchmark]
        public void ArraySortSortedBinary() => Array.Sort(sorted, StringComparer.Ordinal);

        [Benchmark]
        public void ArraySortReverseBinary() => Array.Sort(reverse, StringComparer.Ordinal);

        // 650ns

        [Benchmark]
        public void ArraySortDefault() => Array.Sort(columns, StringComparer.OrdinalIgnoreCase);

        [Benchmark]
        public void ArraySortSorted() => Array.Sort(sorted, StringComparer.OrdinalIgnoreCase);

        [Benchmark]
        public void ArraySortReverse() => Array.Sort(reverse, StringComparer.OrdinalIgnoreCase);
    }
}
