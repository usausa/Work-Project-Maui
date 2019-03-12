namespace Smart.Data.Mapper
{
    using Microsoft.Data.Sqlite;

    using Xunit;

    public class SqlMapperExecuteScalarTest
    {
        [Fact]

        public void ExecuteScalarByObjectParameter()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Table1 (Id int PRIMARY KEY, Data text)");
                con.Execute("INSERT INTO Table1 (Id, Data) VALUES (1, 'test1')");
                con.Execute("INSERT INTO Table1 (Id, Data) VALUES (2, 'test')");

                var count = con.ExecuteScalar<long>("SELECT COUNT(*) FROM Table1 WHERE Id = @Id", new { Id = 1 });

                Assert.Equal(1, count);
            }
        }
    }
}
