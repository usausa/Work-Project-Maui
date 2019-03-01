namespace Smart.Data.Mapper.Parameters
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public sealed class DictionaryParameterBuilder : IParameterBuilder
    {
        public bool Handle(ISqlMapperConfig config, IDbCommand cmd, object parameter)
        {
            if (parameter is IDictionary<string, object> dictionary)
            {
                foreach (var keyValue in dictionary)
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

                return true;
            }

            return false;
        }
    }
}
