using System.Collections.Generic;
using Smart.Mock.Data;

namespace DataBenchmark
{
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
            Add(Job.MediumRun);
        }
    }


    [Config(typeof(BenchmarkConfig))]
    public class Benchmark
    {
        private SqliteConnection sqlite;

        private MockDbConnection mockQuery;

        private MockDbConnection mockExecute;

        [GlobalSetup]
        public void GlobalSetup()
        {
            sqlite = new SqliteConnection("Data Source=:memory:");
            sqlite.Open();
            sqlite.Execute("CREATE TABLE IF NOT EXISTS Table1 (Id int PRIMARY KEY, Data text)");
            sqlite.Execute("CREATE TABLE IF NOT EXISTS Table2 (Id int, Data text)");

            for (var i = 1; i <= 100; i++)
            {
                sqlite.Execute("INSERT INTO Table1 (Id, Data) VALUES (@Id, @Data)", new { Id = i, Data = "test" });
            }

            mockQuery = new MockDbConnection();
            mockExecute = new MockDbConnection();
        }

        [IterationSetup]
        public void IterationSetup()
        {
            sqlite.Execute("DELETE FROM Table2");

            var columns = new[]
            {
                new MockColumn(typeof(long), "Id"),
                new MockColumn(typeof(string), "Data")
            };
            var rows = new List<object[]>();
            for (var i = 1; i <= 100; i++)
            {
                rows.Add(new object[] { (long)i, "test" });
            }
            mockQuery.SetupCommand(cmd => cmd.SetupResult(new MockDataReader(columns, rows)));

            mockExecute.SetupCommand(cmd => cmd.SetupResult(1));
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            sqlite.Close();
            mockQuery.Close();
            mockExecute.Close();
        }

        // SQLite

        [Benchmark]
        public void SqliteQueryFirst()
        {
            sqlite.QueryFirstOrDefault<Table>("SELECT * FROM Table1 WHERE Id = @Id", new { Id = 1 });
        }

        [Benchmark]
        public void SqliteQuery()
        {
            foreach (var _ in sqlite.Query<Table>("SELECT * FROM Table1", buffered: false))
            {
            }
        }

        [Benchmark]
        public void SqliteScalar()
        {
            sqlite.ExecuteScalar<int>("SELECT COUNT(*) FROM Table1");
        }

        [Benchmark]
        public void SqliteExecute()
        {
            sqlite.Execute("INSERT INTO Table2 (Id, Data) VALUES (@Id, @Data)", new { Id = 1, Data = "test" });
        }

        // Mock

        [Benchmark]
        public void MockQuery()
        {
            foreach (var _ in mockQuery.Query<Table>("SELECT * FROM Table1", buffered: false))
            {
            }
        }

        [Benchmark]
        public void MockExecute()
        {
            mockExecute.Execute("INSERT INTO Table2 (Id, Data) VALUES (@Id, @Data)", new { Id = 1, Data = "test" });
        }
    }

    public class Table
    {
        public long Id { get; set; }

        public string Data { get; set; }
    }
}
