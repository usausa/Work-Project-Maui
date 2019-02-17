namespace Smart.Data.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public class DynamicParameter
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
                return default(T);
            }

            return (T)value;
        }

        // TODO Names?

        // TODO Build

        //private readonly DbTypeMap dbTypeMap;

        //public DynamicParameter()
        //dbTypeMap = DbTypeMap.Default;

        //public DynamicParameter(DbTypeMap dbTypeMap)
        //    this.dbTypeMap = dbTypeMap;

        //public IEnumerable<string> ParameterNames
        //{
        //    get { return parameters.Select(x => x.Key); }
        //}

        //void IDynamicParameter.BuildParameters(IDbCommand cmd)
        //{
        //    foreach (var param in parameters.Values)
        //    {
        //        var parameter = cmd.CreateParameter();
        //        cmd.Parameters.Add(parameter);

        //        param.AttachedParam = parameter;

        //        parameter.ParameterName = param.Name;
        //        parameter.Value = param.Value ?? DBNull.Value;
        //        parameter.DbType = param.DbType ?? dbTypeMap.LookupDbType(param.Value);
        //        if (param.Size.HasValue)
        //        {
        //            parameter.Size = param.Size.Value;
        //        }
        //        parameter.Direction = param.Direction;
        //    }
        //}
    }
}
