namespace Smart.Data.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.CompilerServices;

    public static partial class SqlMapper
    {
        //--------------------------------------------------------------------------------
        // Core
        //--------------------------------------------------------------------------------

        private static IDbCommand SetupCommand(ISqlMapperConfig config, IDbConnection con, IDbTransaction transaction, string sql, object param, int? commandTimeout, CommandType? commandType)
        {
            var cmd = con.CreateCommand();

            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }

            cmd.CommandText = sql;

            if (commandTimeout.HasValue)
            {
                cmd.CommandTimeout = commandTimeout.Value;
            }

            if (commandType.HasValue)
            {
                cmd.CommandType = commandType.Value;
            }

            if (param != null)
            {
                var builder = config.CreateParameterBuilder(param.GetType());
                builder(cmd, param);
            }

            return cmd;
        }

        private static void Cleanup(bool wasClosed, IDbConnection con, IDbCommand cmd)
        {
            cmd.Dispose();

            if (wasClosed)
            {
                con.Close();
            }
        }

        //--------------------------------------------------------------------------------
        // Execute
        //--------------------------------------------------------------------------------

        public static int Execute(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(config, con, transaction, sql, param, commandTimeout, commandType))
            {
                try
                {
                    if (wasClosed)
                    {
                        con.Open();
                    }

                    var result = cmd.ExecuteNonQuery();

                    return result;
                }
                finally
                {
                    Cleanup(wasClosed, con, cmd);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Execute(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Execute(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType);
        }

        //--------------------------------------------------------------------------------
        // ExecuteScalar
        //--------------------------------------------------------------------------------

        public static T ExecuteScalar<T>(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(config, con, transaction, sql, param, commandTimeout, commandType))
            {
                try
                {
                    if (wasClosed)
                    {
                        con.Open();
                    }

                    var result = cmd.ExecuteScalar();

                    if (result is DBNull)
                    {
                        return default;
                    }

                    if (result is T scalar)
                    {
                        return scalar;
                    }

                    var parser = config.CreateParser(result.GetType(), typeof(T));
                    return (T)parser(result);
                }
                finally
                {
                    Cleanup(wasClosed, con, cmd);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ExecuteScalar<T>(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return ExecuteScalar<T>(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType);
        }

        //--------------------------------------------------------------------------------
        // ExecuteReader
        //--------------------------------------------------------------------------------

        public static IDataReader ExecuteReader(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandBehavior commandBehavior = CommandBehavior.Default)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(config, con, transaction, sql, param, commandTimeout, commandType))
            {
                try
                {
                    if (wasClosed)
                    {
                        con.Open();
                    }

                    var reader = cmd.ExecuteReader(commandBehavior);
                    wasClosed = false;

                    return reader;
                }
                finally
                {
                    Cleanup(wasClosed, con, cmd);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDataReader ExecuteReader(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandBehavior commandBehavior = CommandBehavior.Default)
        {
            return ExecuteReader(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType, commandBehavior);
        }

        //--------------------------------------------------------------------------------
        // Query
        //--------------------------------------------------------------------------------

        public static IEnumerable<T> Query<T>(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(config, con, transaction, sql, param, commandTimeout, commandType))
            {
                try
                {
                    if (wasClosed)
                    {
                        con.Open();
                    }

                    using (var reader = cmd.ExecuteReader(wasClosed ? CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess : CommandBehavior.SequentialAccess))
                    {
                        wasClosed = false;

                        // TODO

                        while (reader.Read())
                        {
                            // TODO
                            yield return default;
                        }
                    }
                }
                finally
                {
                    Cleanup(wasClosed, con, cmd);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> Query<T>(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Query<T>(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType);
        }

        public static T QueryFirstOrDefault<T>(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(config, con, transaction, sql, param, commandTimeout, commandType))
            {
                try
                {
                    if (wasClosed)
                    {
                        con.Open();
                    }

                    using (var reader = cmd.ExecuteReader(wasClosed ? CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess : CommandBehavior.SequentialAccess))
                    {
                        wasClosed = false;

                        // TODO

                        if (reader.Read())
                        {
                            // TODO
                            return default;
                        }

                        return default;
                    }
                }
                finally
                {
                    Cleanup(wasClosed, con, cmd);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T QueryFirstOrDefault<T>(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return QueryFirstOrDefault<T>(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType);
        }
    }
}
