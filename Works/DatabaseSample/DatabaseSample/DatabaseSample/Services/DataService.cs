namespace DatabaseSample.Services
{
    using System.IO;
    using System.Threading.Tasks;

    using DatabaseSample.Helpers;
    using DatabaseSample.Models;

    using Microsoft.Data.Sqlite;

    using Smart.Data;
    using Smart.Data.Mapper;
    using Smart.Data.Mapper.Builders;

    public class DataServiceOptions
    {
        public string Path { get; set; }
    }

    public class DataService
    {
        private readonly DataServiceOptions options;

        private readonly DelegateDbProvider provider;

        public DataService(DataServiceOptions options)
        {
            this.options = options;

            var connectionString = $"Data Source={options.Path}";
            provider = new DelegateDbProvider(() => new SqliteConnection(connectionString));
        }

        public async ValueTask RebuildAsync()
        {
            if (File.Exists(options.Path))
            {
                File.Delete(options.Path);
            }

            await provider.UsingAsync(async con =>
            {
                await con.ExecuteAsync("PRAGMA AUTO_VACUUM=1");
                await con.ExecuteAsync(SqlHelper.MakeCreate<DataEntity>());
            });
        }

        // CRUD

        public async ValueTask<bool> InsertDataAsync(DataEntity entity)
        {
            return await provider.UsingAsync(async con =>
            {
                try
                {
                    await con.ExecuteAsync(SqlInsert<DataEntity>.Values(), entity);

                    return true;
                }
                catch (SqliteException e)
                {
                    if (e.SqliteErrorCode == SQLitePCL.raw.SQLITE_CONSTRAINT)
                    {
                        return false;
                    }
                    throw;
                }
            });
        }

        public async ValueTask<int> UpdateDataAsync(long id, string name)
        {
            return await provider.UsingAsync(con =>
                con.ExecuteAsync(
                    SqlUpdate<DataEntity>.Set("Name = @Name", "Id = @Id"),
                    new { Id = id, Name = name }));
        }

        public async ValueTask<int> DeleteDataAsync(long id)
        {
            return await provider.UsingAsync(con =>
                con.ExecuteAsync(
                    SqlDelete<DataEntity>.ByKey(),
                    new { Id = id }));
        }

        public async ValueTask<DataEntity> QueryDataAsync(long id)
        {
            return await provider.UsingAsync(con =>
                con.QueryFirstOrDefaultAsync<DataEntity>(
                    SqlSelect<DataEntity>.ByKey(),
                    new { Id = id }));
        }

        // Bulk
    }
}
