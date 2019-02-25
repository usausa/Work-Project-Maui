namespace Smart.Data.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public sealed class DynamicParameter : IDynamicParameter
    {
        private readonly Dictionary<string, ParameterInfo> parameters = new Dictionary<string, ParameterInfo>();

        private class ParameterInfo
        {
            public string Name { get; set; }

            public object Value { get; set; }

            public DbType? DbType { get; set; }

            public int? Size { get; set; }

            public ParameterDirection Direction { get; set; }

            public IDbDataParameter AttachedParam { get; set; }
        }

        public void Add(string name, object value, DbType? dbType = null, int? size = null, ParameterDirection direction = ParameterDirection.Input)
        {
            parameters[name] = new ParameterInfo { Name = name, Value = value, DbType = dbType, Size = size, Direction = direction };
        }

        public T Get<T>(string name)
        {
            var value = parameters[name].AttachedParam.Value;
            if (value == DBNull.Value)
            {
                return default;
            }

            return (T)value;
        }

        public void Build(ISqlMapperConfig config, IDbCommand cmd)
        {
            foreach (var parameter in parameters.Values)
            {
                var param = cmd.CreateParameter();
                param.ParameterName = parameter.Name;

                var value = parameter.Value ?? DBNull.Value;
                param.Value = value;

                if (value != DBNull.Value)
                {
                    param.DbType = parameter.DbType ?? config.LookupDbType(value);
                }

                if (parameter.Size.HasValue)
                {
                    param.Size = parameter.Size.Value;
                }

                param.Direction = parameter.Direction;

                cmd.Parameters.Add(param);
                parameter.AttachedParam = param;
            }
        }
    }
}
