namespace DataMap.FormsApp.Services
{
    using Smart.Data;

    public class DataService
    {
        private readonly IConnectionFactory connectionFactory;

        public DataService(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public void Initialize()
        {
            connectionFactory.Using(con =>
            {
                using (var cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS Item (Id int PRIMARY KEY, Name text, Price int)";
                    cmd.ExecuteNonQuery();
                }
            });
        }
    }
}
