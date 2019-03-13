namespace Smart.Data.Mapper.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    using Smart.Data.Mapper.Attributes;

    public sealed class ObjectResultMapperFactory : IResultMapperFactory
    {
        public static ObjectResultMapperFactory Instance { get; } = new ObjectResultMapperFactory();

        private static readonly Action<object, object> Nop = (obj, value) => { };

        private ObjectResultMapperFactory()
        {
        }

        public bool IsMatch(Type type) => true;

        public Func<IDataRecord, T> CreateMapper<T>(ISqlMapperConfig config, Type type, ColumnInfo[] columns)
        {
            var objectFactory = config.CreateFactory<T>();
            var entries = CreateMapEntries(config, type, columns);

            return record =>
            {
                var obj = objectFactory();

                for (var i = 0; i < entries.Length; i++)
                {
                    entries[i](obj, record.GetValue(i));
                }

                return obj;
            };
        }

        private static Action<object, object>[] CreateMapEntries(ISqlMapperConfig config, Type type, ColumnInfo[] columns)
        {
            // TODO delete Naming ?
            var namingConverter = config.GetNameConverter();
            var targetProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(IsTargetProperty)
                .Select(x =>
                {
                    var attr = x.GetCustomAttribute<DBNameAttribute>();
                    return new
                    {
                        Name = attr != null ? attr.Name : namingConverter(x.Name),
                        Property = x
                    };
                })
                .ToArray();

            var list = new List<Action<object, object>>();
            foreach (var column in columns)
            {
                // TODO column to pascal and ordinal & ignore ?
                var entry = targetProperties.FirstOrDefault(x => String.Equals(x.Name, column.Name, StringComparison.Ordinal)) ??
                            targetProperties.FirstOrDefault(x => String.Equals(x.Name, column.Name, StringComparison.OrdinalIgnoreCase));
                if (entry == null)
                {
                    list.Add(Nop);
                    continue;
                }

                var pi = entry.Property;
                var setter = config.CreateSetter(pi);
                var defaultValue = pi.PropertyType.GetDefaultValue();

                if (pi.PropertyType == column.Type)
                {
                    list.Add((obj, value) => setter(obj, value is DBNull ? defaultValue : value));
                }
                else
                {
                    var parser = config.CreateParser(column.Type, pi.PropertyType);
                    list.Add((obj, value) => setter(obj, parser(value is DBNull ? defaultValue : value)));
                }
            }

            return list.ToArray();
        }

        private static bool IsTargetProperty(PropertyInfo pi)
        {
            return pi.CanWrite && (pi.GetCustomAttribute<IgnoreAttribute>() == null);
        }
    }
}
