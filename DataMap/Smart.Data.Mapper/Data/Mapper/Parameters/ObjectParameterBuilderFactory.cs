namespace Smart.Data.Mapper.Parameters
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    using Smart.Data.Mapper.Attributes;
    using Smart.Data.Mapper.Handlers;

    public sealed class ObjectParameterBuilderFactory : IParameterBuilderFactory
    {
        public static ObjectParameterBuilderFactory Instance { get; } = new ObjectParameterBuilderFactory();

        private ObjectParameterBuilderFactory()
        {
        }

        public bool IsMatch(Type type) => true;

        public ParameterBuilder CreateBuilder(ISqlMapperConfig config, Type type)
        {
            var entries = CreateParameterEntries(config, type);

            return new ParameterBuilder(
                (cmd, parameter) =>
                {
                    for (var i = 0; i < entries.Length; i++)
                    {
                        var entry = entries[i];
                        var param = cmd.CreateParameter();

                        param.ParameterName = entry.Name;

                        var value = entry.Getter(parameter);

                        if (value is null)
                        {
                            param.Value = DBNull.Value;
                        }
                        else
                        {
                            param.DbType = entry.DbType;
                            if (entry.Handler != null)
                            {
                                entry.Handler.SetValue(param, value);
                            }
                            else
                            {
                                param.Value = value;
                            }
                        }

                        cmd.Parameters.Add(param);
                    }
                },
                null);
        }

        private static ParameterEntry[] CreateParameterEntries(ISqlMapperConfig config, Type type)
        {
            var list = new List<ParameterEntry>();
            foreach (var pi in type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(IsTargetProperty))
            {
                var getter = config.CreateGetter(pi);
                var entry = config.LookupTypeHandle(pi.PropertyType);
                list.Add(new ParameterEntry(pi.Name, getter, entry.DbType, entry.TypeHandler));
            }

            return list.ToArray();
        }

        private static bool IsTargetProperty(PropertyInfo pi)
        {
            return pi.CanRead && (pi.GetCustomAttribute<IgnoreAttribute>() == null);
        }

        private sealed class ParameterEntry
        {
            public string Name { get; }

            public Func<object, object> Getter { get; }

            public DbType DbType { get; }

            public ITypeHandler Handler { get; }

            public ParameterEntry(string name, Func<object, object> getter, DbType dbType, ITypeHandler handler)
            {
                Name = name;
                Getter = getter;
                DbType = dbType;
                Handler = handler;
            }
        }
    }
}
