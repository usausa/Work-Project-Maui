namespace ColumnBenchmark
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Running;

    using Dapper;

    using Microsoft.Data.Sqlite;

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
        private readonly Dictionary<string, object> meta2 = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", null },
            { "Data", null },
        };
        private readonly Dictionary<string, object> meta10 = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", null },
            { "D1", null },
            { "D2", null },
            { "D3", null },
            { "D4", null },
            { "D5", null },
            { "D6", null },
            { "D7", null },
            { "D8", null },
            { "D9", null },
        };

        private readonly Dictionary<string, object[]> accessors2 = new Dictionary<string, object[]>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, object[]> accessors10 = new Dictionary<string, object[]>(StringComparer.OrdinalIgnoreCase);

        private readonly Func<string[], object[]> accessorFactory;

        private readonly AccessorCache cache = new AccessorCache();

        private SqliteConnection con;

        private IDataReader reader2;

        private IDataReader reader10;

        public Benchmark()
        {
            accessorFactory = CreateAccessor;
        }

        public object[] CreateAccessor(string[] columns)
        {
            return new object[columns.Length];
        }

        [GlobalSetup]
        public void Setup()
        {
            con = new SqliteConnection("Data Source=:memory:");
            con.Open();
            con.Execute("CREATE TABLE IF NOT EXISTS Item2 (Id int PRIMARY KEY, Data text)");
            con.Execute("CREATE TABLE IF NOT EXISTS Item10 (" +
                        "Id int PRIMARY KEY, D1 text, D2 text, D3 text, D4 text, " +
                        "D5 text, D6 text, D7 text, D8 text, D9 text)");

            con.Execute("INSERT INTO Item2 (Id, Data) VALUES (1, NULL)");
            con.Execute("INSERT INTO Item10 (Id, D1, D2, D3, D4, D5, D6, D7, D8, D9) " +
                        "VALUES (1, '', '', '', '', NULL, NULL, NULL, NULL, NULL)");

            reader2 = con.ExecuteReader("SELECT * FROM Item2");
            reader10 = con.ExecuteReader("SELECT * FROM Item10");

            reader2.Read();
            reader10.Read();

            accessors2[CreateKey(reader2)] = new object[reader2.FieldCount];
            accessors10[CreateKey(reader10)] = new object[reader10.FieldCount];

            cache.AddIfNotExist(reader2, accessorFactory);
            cache.AddIfNotExist(reader10, accessorFactory);
        }

        private string CreateKey(IDataReader reader)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                sb.Append(reader.GetName(i));
                sb.Append(",");
            }

            sb.Length = sb.Length - 1;

            return sb.ToString();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            reader2.Close();
            reader10.Close();
            con.Close();
        }

        // Runtime

        [Benchmark]
        public void Runtime2GetName()
        {
            for (var i = 0; i < reader2.FieldCount; i++)
            {
                reader2.GetName(i);
            }
        }

        [Benchmark]
        public void Runtime2GetNameAndMeta()
        {
            for (var i = 0; i < reader2.FieldCount; i++)
            {
                var name = reader2.GetName(i);
                meta2.TryGetValue(name, out _);
            }
        }

        [Benchmark]
        public void Runtime2GetValue()
        {
            for (var i = 0; i < reader2.FieldCount; i++)
            {
                reader2.GetValue(i);
            }
        }

        [Benchmark]
        public void Runtime2()
        {
            for (var i = 0; i < reader2.FieldCount; i++)
            {
                var name = reader2.GetName(i);
                meta2.TryGetValue(name, out _);
                reader2.GetValue(i);
            }
        }

        [Benchmark]
        public void Runtime10()
        {
            for (var i = 0; i < reader10.FieldCount; i++)
            {
                var name = reader10.GetName(i);
                meta10.TryGetValue(name, out _);
                reader10.GetValue(i);
            }
        }

        // Meta once

        [Benchmark(OperationsPerInvoke = 10)]
        public void RuntimeFirst2N()
        {
            var accessors = new object[reader2.FieldCount];
            for (var i = 0; i < accessors.Length; i++)
            {
                meta2.TryGetValue(reader2.GetName(i), out accessors[i]);
            }

            for (var loop = 0; loop < 10; loop++)
            {
                for (var i = 0; i < accessors.Length; i++)
                {
                    reader2.GetValue(i);
                }
            }
        }

        [Benchmark(OperationsPerInvoke = 10)]
        public void RuntimeFirst10N()
        {
            var accessors = new object[reader10.FieldCount];
            for (var i = 0; i < accessors.Length; i++)
            {
                meta10.TryGetValue(reader10.GetName(i), out accessors[i]);
            }

            for (var loop = 0; loop < 10; loop++)
            {
                for (var i = 0; i < accessors.Length; i++)
                {
                    reader10.GetValue(i);
                }
            }
        }

        // Cache

        [Benchmark]
        public void Cache2()
        {
            var key = CreateKey(reader2);
            if (!accessors2.TryGetValue(key, out var accessors))
            {
                accessors = new object[reader2.FieldCount];
                accessors2[key] = null;
            }

            for (var i = 0; i < accessors.Length; i++)
            {
                reader2.GetValue(i);
            }
        }

        [Benchmark]
        public void Cache10()
        {
            var key = CreateKey(reader10);
            if (!accessors10.TryGetValue(key, out var accessors))
            {
                accessors = new object[reader10.FieldCount];
                accessors10[key] = null;
            }

            for (var i = 0; i < accessors.Length; i++)
            {
                reader10.GetValue(i);
            }
        }

        // Cache special

        [Benchmark]
        public void CacheSpecial2()
        {
            if (!cache.TryGetValue(reader2, out var accessors))
            {
                accessors = cache.AddIfNotExist(reader2, accessorFactory);
            }

            for (var i = 0; i < accessors.Length; i++)
            {
                reader2.GetValue(i);
            }
        }

        [Benchmark]
        public void CacheSpecial10()
        {
            if (!cache.TryGetValue(reader10, out var accessors))
            {
                accessors = cache.AddIfNotExist(reader10, accessorFactory);
            }

            for (var i = 0; i < accessors.Length; i++)
            {
                reader10.GetValue(i);
            }
        }
    }

    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    public class AccessorCache
    {
        private const int InitialSize = 256;

        private const double Factor = 2;

        private static readonly Node[] EmptyNodes = new Node[0];

        private readonly object sync = new object();

        private Table table;

        //--------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------

        public AccessorCache()
        {
            table = CreateInitialTable();
        }

        //--------------------------------------------------------------------------------
        // Private
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CalculateHash(string[] columns)
        {
            unchecked
            {
                // TODO Hash
                var hash = columns.Length;
                for (var i = 0; i < columns.Length; i++)
                {
                    hash = (hash * 31) + columns[i].GetHashCode();
                }
                return hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CalculateHash(IDataReader reader)
        {
            unchecked
            {
                // TODO Hash
                var hash = reader.FieldCount;
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    hash = (hash * 31) + reader.GetName(i).GetHashCode();
                }
                return hash;
            }
        }

        private static uint CalculateSize(int count)
        {
            uint size = 0;

            for (var i = 1L; i < count; i *= 2)
            {
                size = (size << 1) + 1;
            }

            return size + 1;
        }

        private static Table CreateInitialTable()
        {
            var mask = InitialSize - 1;
            var nodes = new Node[InitialSize][];

            for (var i = 0; i < nodes.Length; i++)
            {
                nodes[i] = EmptyNodes;
            }

            return new Table(mask, nodes, 0);
        }

        private static Node[] AddNode(Node[] nodes, Node addNode)
        {
            if (nodes == null)
            {
                return new[] { addNode };
            }

            var newNodes = new Node[nodes.Length + 1];
            Array.Copy(nodes, 0, newNodes, 0, nodes.Length);
            newNodes[nodes.Length] = addNode;

            return newNodes;
        }

        private static void RelocateNodes(Node[][] nodes, Node[][] oldNodes, int mask)
        {
            for (var i = 0; i < oldNodes.Length; i++)
            {
                for (var j = 0; j < oldNodes[i].Length; j++)
                {
                    var node = oldNodes[i][j];
                    var relocateIndex = CalculateHash(node.Columns) & mask;
                    nodes[relocateIndex] = AddNode(nodes[relocateIndex], node);
                }
            }
        }

        private static void FillEmptyIfNull(Node[][] nodes)
        {
            for (var i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] == null)
                {
                    nodes[i] = EmptyNodes;
                }
            }
        }

        private Table CreateAddTable(Table oldTable, Node node)
        {
            var requestSize = Math.Max(InitialSize, (int)Math.Ceiling((oldTable.Count + 1) * Factor));

            var size = CalculateSize(requestSize);
            var mask = (int)(size - 1);
            var newNodes = new Node[size][];

            RelocateNodes(newNodes, oldTable.Nodes, mask);

            var index = CalculateHash(node.Columns) & mask;
            newNodes[index] = AddNode(newNodes[index], node);

            FillEmptyIfNull(newNodes);

            return new Table(mask, newNodes, oldTable.Count + 1);
        }


        //--------------------------------------------------------------------------------
        // Public
        //--------------------------------------------------------------------------------

        public int Count => table.Count;

        public void Clear()
        {
            lock (sync)
            {
                var newTable = CreateInitialTable();
                Interlocked.MemoryBarrier();
                table = newTable;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryGetValueInternal(Table targetTable, IDataReader reader, out object[] value)
        {
            var index = CalculateHash(reader) & targetTable.HashMask;
            var array = targetTable.Nodes[index];
            for (var i = 0; i < array.Length; i++)
            {
                var node = array[i];
                if (IsMatchColumn(node.Columns, reader))
                {
                    value = node.Value;
                    return true;
                }
            }

            value = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsMatchColumn(string[] columns, IDataReader reader)
        {
            if (columns.Length != reader.FieldCount)
            {
                return false;
            }

            for (var i = 0; i < columns.Length; i++)
            {
                if (String.Compare(columns[i], reader.GetName(i), StringComparison.OrdinalIgnoreCase) != 0)
                {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(IDataReader reader, out object[] value)
        {
            return TryGetValueInternal(table, reader, out value);
        }

        public object[] AddIfNotExist(IDataReader reader, Func<string[], object[]> valueFactory)
        {
            lock (sync)
            {
                // Double checked locking
                if (TryGetValueInternal(table, reader, out var currentValue))
                {
                    return currentValue;
                }

                var columns = new string[reader.FieldCount];
                for (var i = 0; i < columns.Length; i++)
                {
                    columns[i] = reader.GetName(i);
                }

                var value = valueFactory(columns);

                // Check if added by recursive
                if (TryGetValueInternal(table, reader, out currentValue))
                {
                    return currentValue;
                }

                // Rebuild
                var newTable = CreateAddTable(table, new Node(columns, value));
                Interlocked.MemoryBarrier();
                table = newTable;

                return value;
            }
        }

        //--------------------------------------------------------------------------------
        // Inner
        //--------------------------------------------------------------------------------

        private class Node
        {
            public string[] Columns { get; }

            public object[] Value { get; }

            public Node(string[] columns, object[] value)
            {
                Columns = columns;
                Value = value;
            }
        }

        private class Table
        {
            public int HashMask { get; }

            public Node[][] Nodes { get; }

            public int Count { get; }

            public Table(int hashMask, Node[][] nodes, int count)
            {
                HashMask = hashMask;
                Nodes = nodes;
                Count = count;
            }
        }
    }
}
