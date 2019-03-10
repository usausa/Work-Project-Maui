namespace Smart.Data.Mapper
{
    using Microsoft.Data.Sqlite;

    using Xunit;

    public class SqlMapperExecuteTest
    {
        [Fact]

        public void ExecuteByObjectParameter()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Table1 (Id int PRIMARY KEY, Data text)");

                con.Execute("INSERT INTO Table1 (Id, Data) VALUES (@Id, @Data)", new { Id = 1, Data = "test" });

                // TODO Assert
            }
        }
    }
}
