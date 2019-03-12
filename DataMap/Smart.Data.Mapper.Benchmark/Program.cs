namespace Smart.Data.Mapper.Benchmark
{
    using System;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Running;

    using Smart.Mock.Data;

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
            Add(Job.LongRun);
        }
    }

    [Config(typeof(BenchmarkConfig))]
    public class Benchmark
    {
        private MockDbConnection mockExecute;

        private MockDbConnection mockExecuteScalar;

        [IterationSetup]
        public void IterationSetup()
        {
            mockExecute = new MockDbConnection();
            mockExecute.SetupCommand(cmd => cmd.SetupResult(1));
            mockExecuteScalar = new MockDbConnection();
            mockExecuteScalar.SetupCommand(cmd => cmd.SetupResult(1L));
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            mockExecute.Close();
            mockExecuteScalar.Close();
        }

        //--------------------------------------------------------------------------------
        // Execute
        //--------------------------------------------------------------------------------

        private const string ExecuteSql =
            "INSERT INTO Table (Id, Data) VALUES (@Id, @Data)";

        [Benchmark]
        public void MockExecuteDapper()
        {
            Dapper.SqlMapper.Execute(mockExecute, ExecuteSql, new { Id = 1, Data = "test" });
        }

        [Benchmark]
        public void MockExecuteSmart()
        {
            mockExecute.Execute(ExecuteSql, new { Id = 1, Data = "test" });
        }

        private const string ExecuteWithParameter10Sql =
            "INSERT INTO Table (Id, Name, Amount, Qty, Flag1, Flag2, DateTimeOffset, CreatedBy, UpdatedAt, UpdatedBy) " +
            "VALUES (@Id, @Name, @Amount, @Qty, @Flag1, @Flag2, @DateTimeOffset, @CreatedBy, @UpdatedAt, @UpdatedBy)";

        [Benchmark]
        public void MockExecuteDapperWithParameter10()
        {
            Dapper.SqlMapper.Execute(mockExecute, ExecuteWithParameter10Sql, new Table());
        }

        [Benchmark]
        public void MockExecuteSmartWithParameter10()
        {
            mockExecute.Execute(ExecuteWithParameter10Sql, new Table());
        }

        [Benchmark]
        public void MockExecuteDapperWithOverParameter()
        {
            // [MEMO] Dapper optimize parameters
            Dapper.SqlMapper.Execute(mockExecute, ExecuteSql, new Table());
        }

        [Benchmark]
        public void MockExecuteSmartWithOverParameter()
        {
            mockExecute.Execute(ExecuteSql, new Table());
        }

        //--------------------------------------------------------------------------------
        // ExecuteScalar
        //--------------------------------------------------------------------------------

        private const string ExecuteScalarSql =
            "SELECT COUNT(*) FROM Table";

        [Benchmark]
        public long MockExecuteScalarDapper()
        {
            return Dapper.SqlMapper.ExecuteScalar<long>(mockExecuteScalar, ExecuteScalarSql);
        }

        [Benchmark]
        public long MockExecuteScalarSmart()
        {
            return mockExecuteScalar.ExecuteScalar<long>(ExecuteScalarSql);
        }

        [Benchmark]
        public long MockExecuteScalarDapperWithConvert()
        {
            return Dapper.SqlMapper.ExecuteScalar<int>(mockExecuteScalar, ExecuteScalarSql);
        }

        [Benchmark]
        public long MockExecuteScalarSmartWithConvert()
        {
            return mockExecuteScalar.ExecuteScalar<int>(ExecuteScalarSql);
        }

        //--------------------------------------------------------------------------------
        // Query
        //--------------------------------------------------------------------------------

        // TODO 2

        // TODO first?, reader?

        //private MockDbConnection mockQuery;

        //[GlobalSetup]
        //public void GlobalSetup()
        //{
        //    sqlite = new SqliteConnection("Data Source=:memory:");
        //    sqlite.Open();
        //    sqlite.Execute("CREATE TABLE IF NOT EXISTS Table1 (Id int PRIMARY KEY, Data text)");
        //    sqlite.Execute("CREATE TABLE IF NOT EXISTS Table2 (Id int, Data text)");

        //    for (var i = 1; i <= 100; i++)
        //    {
        //        sqlite.Execute("INSERT INTO Table1 (Id, Data) VALUES (@Id, @Data)", new { Id = i, Data = "test" });
        //    }

        //    mockQuery = new MockDbConnection();
        //    mockExecute = new MockDbConnection();
        //}

        //[IterationSetup]
        //public void IterationSetup()
        //{
        //    sqlite.Execute("DELETE FROM Table2");

        //    var columns = new[]
        //    {
        //        new MockColumn(typeof(long), "Id"),
        //        new MockColumn(typeof(string), "Data")
        //    };
        //    var rows = new List<object[]>();
        //    for (var i = 1; i <= 100; i++)
        //    {
        //        rows.Add(new object[] { (long)i, "test" });
        //    }
        //    mockQuery.SetupCommand(cmd => cmd.SetupResult(new MockDataReader(columns, rows)));

        //    mockExecute.SetupCommand(cmd => cmd.SetupResult(1));

        //// Mock

        //[Benchmark]
        //public void MockQuery()
        //{
        //    foreach (var _ in mockQuery.Query<Table>("SELECT * FROM Table1", buffered: false))
        //    {
        //    }
        //}
    }

    public class Table
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public int Amount { get; set; }

        public int Qty { get; set; }

        public bool Flag1 { get; set; }

        public bool Flag2 { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public string UpdatedBy { get; set; }
    }
}
