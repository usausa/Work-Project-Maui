namespace DataMapperWork
{
    using System;
    using System.Data;
    using System.Linq;

    using Microsoft.Data.Sqlite;

    using Smart.Data.Mapper;
    using Smart.Data.Mapper.Attributes;

    public static class Program
    {
        public static void Main(string[] args)
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Table1 (Id int PRIMARY KEY, Data text)");

                con.Execute("INSERT INTO Table1 (Id, Data) VALUES (@Id, @Data)", new { Id = 1, Data = "test" });

                var count = con.ExecuteScalar<int>("SELECT COUNT(*) FROM Table1");
                Console.WriteLine(count);

                var list = con.Query<Table>("SELECT * FROM Table1").ToList();
                foreach (var entity in list)
                {
                    Console.WriteLine($"{entity.Id} : {entity.Data}");
                }
            }

            //using (var con = new SqlConnection("Data Source=10.13.8.21;MultipleActiveResultSets=True;Initial Catalog=Test;User ID=test;Password=test"))
            //{
            //    con.Open();

            //    var param = new TestProcParameter { InParam = 1 };
            //    con.Execute("TestProc", param, commandType: CommandType.StoredProcedure);
            //    Console.WriteLine(param.OutParam);
            //}
        }
    }

    public class TestProcParameter
    {
        public int InParam { get; set; }

        [DbType(DbType.Int32)]
        [Direction(ParameterDirection.Output)]
        public long OutParam { get; set; }
    }

    public class Table
    {
        public int Id { get; set; }

        public string Data { get; set; }
    }
}
