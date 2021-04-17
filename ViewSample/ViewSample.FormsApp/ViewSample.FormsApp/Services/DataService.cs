namespace ViewSample.FormsApp.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.Data.Sqlite;

    using Smart.Data;
    using Smart.Data.Mapper;
    using Smart.Data.Mapper.Builders;

    using ViewSample.FormsApp.Helpers;
    using ViewSample.FormsApp.Models.Entity;

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

        public void DeleteAll()
        {
            if (File.Exists(options.Path))
            {
                File.Delete(options.Path);
            }
        }

        public async ValueTask PrepareAsync()
        {
            if (File.Exists(options.Path))
            {
                return;
            }

            await provider.UsingAsync(async con =>
            {
                await con.ExecuteAsync("PRAGMA AUTO_VACUUM=1");
                await con.ExecuteAsync(SqlHelper.MakeCreate<DataEntity>());
                await con.ExecuteAsync(SqlHelper.MakeCreate<BulkDataEntity>());
            });
        }

        // CRUD

        public async ValueTask<bool> InsertDataAsync(DataEntity entity)
        {
            return await provider.UsingAsync(async con =>
            {
                try
                {
                    await con.ExecuteAsync(
                        SqlInsert<DataEntity>.Values(),
                        entity);

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

        public ValueTask<int> UpdateDataAsync(long id, string name) =>
            provider.UsingAsync(con =>
                con.ExecuteAsync(
                    SqlUpdate<DataEntity>.Set("Name = @Name", "Id = @Id"),
                    new { Id = id, Name = name }));

        public ValueTask<int> DeleteDataAsync(long id) =>
            provider.UsingAsync(con =>
                con.ExecuteAsync(
                    SqlDelete<DataEntity>.ByKey(),
                    new { Id = id }));

        public ValueTask<DataEntity> QueryDataAsync(long id) =>
            provider.UsingAsync(con =>
                con.QueryFirstOrDefaultAsync<DataEntity>(
                    SqlSelect<DataEntity>.ByKey(),
                    new { Id = id }));

        // Bulk

        public ValueTask<int> CountBulkDataAsync() =>
            provider.UsingAsync(con =>
                con.ExecuteScalarAsync<int>(
                    SqlCount<BulkDataEntity>.All()));

        public void InsertBulkDataEnumerable(IEnumerable<BulkDataEntity> source)
        {
            provider.UsingTx((con, tx) =>
            {
                foreach (var entity in source)
                {
                    con.Execute(SqlInsert<BulkDataEntity>.Values(), entity, tx);
                }

                tx.Commit();
            });
        }

        public ValueTask<int> DeleteAllBulkDataAsync() =>
            provider.UsingAsync(con => con.ExecuteAsync("DELETE FROM BulkData"));

        public List<BulkDataEntity> QueryAllBulkDataList() =>
            provider.Using(con =>
                con.QueryList<BulkDataEntity>(
                    SqlSelect<BulkDataEntity>.All()));
    }
}
