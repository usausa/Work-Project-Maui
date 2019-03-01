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
            Add(Job.ShortRun);
        }
    }


    [Config(typeof(BenchmarkConfig))]
    public class Benchmark
    {
        private SqliteConnection con;

        [GlobalSetup]
        public void Setup()
        {
            con = new SqliteConnection("Data Source=:memory:");
            con.Open();
            con.Execute("CREATE TABLE IF NOT EXISTS Table1 (Id int PRIMARY KEY, Data text)");
            con.Execute("CREATE TABLE IF NOT EXISTS Table2 (Id int, Data text)");

            for (var i = 1; i <= 100; i++)
            {
                con.Execute("INSERT INTO Table1 (Id, Data) VALUES (@Id, @Data)", new { Id = i, Data = "test" });
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            con.Close();
        }

        [Benchmark]
        public void QueryFirst()
        {
            con.QueryFirstOrDefault<Table>("SELECT * FROM Table1 WHERE Id = @Id", new { Id = 1 });
        }

        [Benchmark]
        public void Query()
        {
            foreach (var _ in con.Query<Table>("SELECT * FROM Table1", buffered: false))
            {
            }
        }

        [Benchmark]
        public void Scalar()
        {
            con.ExecuteScalar<int>("SELECT COUNT(*) FROM Table1");
        }

        [Benchmark]
        public void Execute()
        {
            con.Execute("INSERT INTO Table2 (Id, Data) VALUES (@Id, @Data)", new { Id = 1, Data = "test" });
        }
    }

    public class Table
    {
        public long Id { get; set; }

        public string Data { get; set; }
    }
}
