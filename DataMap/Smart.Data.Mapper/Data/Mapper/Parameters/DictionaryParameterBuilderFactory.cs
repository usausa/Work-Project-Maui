namespace Smart.Data.Mapper.Parameters
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public sealed class DictionaryParameterBuilderFactory : IParameterBuilderFactory
    {
        public bool IsMatch(Type type)
        {
            return typeof(IDictionary<string, object>).IsAssignableFrom(type);
        }

        public Action<IDbCommand, object> CreateBuilder(ISqlMapperConfig config, Type type)
        {
            return (cmd, parameter) =>
            {
                foreach (var keyValue in (IDictionary<string, object>)parameter)
                {
                    var param = cmd.CreateParameter();
                    param.ParameterName = keyValue.Key;

                    var value = keyValue.Value;
                    if (value is null)
                    {
                        param.Value = DBNull.Value;
                    }
                    else
                    {
                        param.DbType = config.LookupDbType(value.GetType(), out var handler);
                        if (handler != null)
                        {
                            handler.SetValue(param, value);
                        }
                        else
                        {
                            param.Value = value;
                        }
                    }

                    cmd.Parameters.Add(param);
                }
            };
        }
    }
}
