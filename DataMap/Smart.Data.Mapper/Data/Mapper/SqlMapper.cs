namespace Smart.Data.Mapper
{
    using System;
    using System.Data;

    public static partial class SqlMapper
    {
        //--------------------------------------------------------------------------------
        // Core
        //--------------------------------------------------------------------------------

        private static IDbCommand SetupCommand(IDbConnection con, IDbTransaction transaction, string sql, object param, int? commandTimeout, CommandType? commandType)
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

            // TODO
            //if (param != null)
            //{
            //    var builder = parameterBuilders.FirstOrDefault(x => x.IsMatch(param));
            //    if (builder == null)
            //    {
            //        throw new SqlMapperException("Parameter can't build.");
            //    }

            //    builder.BuildParameters(cmd, param);
            //}

            return cmd;
        }

        private static void Cleanup(bool wasClosed, IDbConnection con, IDbCommand cmd)
        {
            foreach (var parameter in cmd.Parameters)
            {
                (parameter as IDisposable)?.Dispose();
            }

            if (wasClosed)
            {
                con.Close();
            }
        }

        //--------------------------------------------------------------------------------
        // Extensions
        //--------------------------------------------------------------------------------

        // TODO with config, inline? (without config is inline?, check)

        public static int Execute(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(con, transaction, sql, param, commandTimeout, commandType))
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

        // TODO
        //private static T ExecuteScalarImpl<T>(this IDbConnection con, string sql, object param, IDbTransaction transaction, int? commandTimeout, CommandType? commandType)
        //{
        //    var wasClosed = con.State == ConnectionState.Closed;
        //    using (var cmd = SetupCommand(con, transaction, sql, param, commandTimeout, commandType))
        //    {
        //        try
        //        {
        //            if (wasClosed)
        //            {
        //                con.Open();
        //            }

        //            var result = cmd.ExecuteScalar();

        //            return result == DBNull.Value ? default(T) : (T)Converter.Convert(result, typeof(T));
        //        }
        //        finally
        //        {
        //            Cleanup(wasClosed, con, cmd);
        //        }
        //    }
        //}

        private static IDataReader ExecuteReaderImpl(this IDbConnection con, string sql, object param, IDbTransaction transaction, int? commandTimeout, CommandType? commandType, CommandBehavior commandBehavior)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(con, transaction, sql, param, commandTimeout, commandType))
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

        // TODO QueryImpl

        // TODO QueryFirstOrDefault, Query
        // Lookup回数？、パラメータ、戻り値、カラムのマップはQueryなら一端配列を作って？ Stackで？

        ////--------------------------------------------------------------------------------

        //private static IEnumerable<T> QueryImpl<T>(this IDbConnection con, Func<T> factory, string sql, object param, IDbTransaction transaction, int? commandTimeout, CommandType? commandType)
        //{
        //    var wasClosed = con.State == ConnectionState.Closed;
        //    using (var cmd = SetupCommand(con, transaction, sql, param, commandTimeout, commandType))
        //    {
        //        try
        //        {
        //            var type = typeof(T);
        //            var queryHandler = queryHandlers.FirstOrDefault(x => x.IsMatch(type));
        //            if (queryHandler == null)
        //            {
        //                throw new SqlMapperException(String.Format(CultureInfo.InvariantCulture, "Type {0} can't handle", type.FullName));
        //            }

        //            if (wasClosed)
        //            {
        //                con.Open();
        //            }

        //            var reader = cmd.ExecuteReader(wasClosed ? CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess : CommandBehavior.SequentialAccess);
        //            wasClosed = false;

        //            return queryHandler.Handle(factory, reader, Converter);
        //        }
        //        finally
        //        {
        //            Cleanup(wasClosed, con, cmd);
        //        }
        //    }
        //}

        ////--------------------------------------------------------------------------------
        //// ExecuteScalar
        //public static T ExecuteScalar<T>(this IDbConnection con, string sql)
        //    return ExecuteScalarImpl<T>(con, sql, null, null, null, null);

        ////--------------------------------------------------------------------------------
        //// ExecuteReader
        //public static IDataReader ExecuteReader(this IDbConnection con, string sql)
        //    return ExecuteReaderImpl(con, sql, null, null, null, null, CommandBehavior.Default);

        ////--------------------------------------------------------------------------------
        //// Query
        //public static IEnumerable<T> Query<T>(this IDbConnection con, Func<T> factory, string sql)
        //    return QueryImpl(con, factory, sql, null, null, null, null);

        //// where T : new()

        //public static IEnumerable<T> Query<T>(this IDbConnection con, string sql) where T : new()
        //    return QueryImpl(con, () => new T(), sql, null, null, null, null);

        //// Dictionary<string, object>

        //public static IEnumerable<Dictionary<string, object>> Query(this IDbConnection con, string sql)
        //    return QueryImpl(con, () => new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), sql, null, null, null, null);
    }
}
