namespace PolicyBenchmark
{
    using System;
    using System.Collections.Generic;

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
        private static readonly Type TypeKey = typeof(object);

        private readonly Func<Type, string> factory = x => x.FullName;

        private readonly Dictionary<Type, string> dictionary = new Dictionary<Type, string>();
        private readonly ThreadsafeTypeHashArrayMap<string> hashArrayMap = new ThreadsafeTypeHashArrayMap<string>();
        private readonly IMetadataFactory simpleMetadataFactory = new SimpleMetadataFactory();
        private readonly IMetadataFactory policyMetadataFactory = new PolicyMetadataFactory<DefaultPolicy>();

        [GlobalSetup]
        public void Setup()
        {
            dictionary[TypeKey] = TypeKey.ToString();
            hashArrayMap.AddIfNotExist(TypeKey, TypeKey.ToString());
            simpleMetadataFactory.Create<object>();
            policyMetadataFactory.Create<object>();
        }

        [Benchmark]
        public string Dictionary()
        {
            if (!dictionary.TryGetValue(TypeKey, out var name))
            {
                name = TypeKey.ToString();
                dictionary[TypeKey] = name;
            }

            return name;
        }

        [Benchmark]
        public string DictionaryWithLock()
        {
            lock (dictionary)
            {
                if (!dictionary.TryGetValue(TypeKey, out var name))
                {
                    name = TypeKey.ToString();
                    dictionary[TypeKey] = name;
                }

                return name;
            }
        }

        [Benchmark]
        public string ThreadsafeHashArray()
        {
            return hashArrayMap.AddIfNotExist(TypeKey, factory);
        }

        [Benchmark]
        public string SimpleMetadataFactory()
        {
            return simpleMetadataFactory.Create<object>();
        }

        [Benchmark]
        public string PolicyMetadataFactory()
        {
            return policyMetadataFactory.Create<object>();
        }
    }

    public interface IMetadataFactory
    {
        string Create<T>();
    }

    public sealed class SimpleMetadataFactory : IMetadataFactory
    {
        private static class MetadataHolder<T>
        {
            public static string Value { get; }

            static MetadataHolder()
            {
                Value = typeof(T).FullName;
            }
        }

        public string Create<T>()
        {
            return MetadataHolder<T>.Value;
        }
    }

    public interface IPolicy
    {
        string ToName(string source);
    }

    public sealed class DefaultPolicy : IPolicy
    {
        public string ToName(string source) => source;
    }

    public sealed class PolicyMetadataFactory<TPolicy> : IMetadataFactory
        where TPolicy : IPolicy
    {
        private static readonly IPolicy Policy = (IPolicy)Activator.CreateInstance(typeof(TPolicy));

        private static class MetadataHolder<T>
        {
            public static string Value { get; }

            static MetadataHolder()
            {
                Value = Policy.ToName(typeof(T).FullName);
            }
        }

        public string Create<T>()
        {
            return MetadataHolder<T>.Value;
        }
    }
}