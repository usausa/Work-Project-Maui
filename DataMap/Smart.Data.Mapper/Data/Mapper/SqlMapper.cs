namespace Smart.Data.Mapper
{
    using System;
    using System.Data;

    public static class SqlMapper
    {
        public static SqlMapperConfig DefaultConfig { get; } = new SqlMapperConfig();

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

        // TODO with config

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
    }
}
