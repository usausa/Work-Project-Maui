namespace Smart.Data.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    using Smart.Collections.Concurrent;
    using Smart.Converter;
    using Smart.Data.Mapper.Handlers;
    using Smart.Data.Mapper.Mappers;
    using Smart.Data.Mapper.Parameters;
    using Smart.Reflection;

    // TODO インターフェース抽出？、メタデータが取得できれば良いだけか？

    public sealed class SqlMapperConfig : ISqlMapperConfig
    {
        private static readonly IParameterBuilderFactory[] DefaultParameterBuilders =
        {
            new DynamicParameterBuilderFactory(),
            new ObjectParameterBuilderFactory()
        };

        private static readonly IResultMapperFactory[] DefaultResultMappers =
        {
            new ObjectResultMapperFactory()
        };

        private readonly ThreadsafeTypeHashArrayMap<Action<IDbCommand, object>> parameterBuilderCache = new ThreadsafeTypeHashArrayMap<Action<IDbCommand, object>>();

        private readonly ResultMapperCache resultMapperCache = new ResultMapperCache();

        // TODO Custom ? withHandlers ?
        private readonly Dictionary<Type, DbType> typeMap = new Dictionary<Type, DbType>();

        private IParameterBuilderFactory[] parameterBuilderFactories;

        private IResultMapperFactory[] resultMappers;

        //--------------------------------------------------------------------------------
        // Property
        //--------------------------------------------------------------------------------

        public static SqlMapperConfig Default { get; } = new SqlMapperConfig();

        public IList<IParameterBuilderFactory> ParameterBuilders { get; } = new List<IParameterBuilderFactory>();

        //public Dictionary<Type, object> Plugin, Addを用意して、拡張メソッドでIFをキーとするメソッドを！、一応Lock

        // -----------------------------

        // TODO メタデータはFactory側で保持、Parameter、HandlerのキャッシュはConfigで保持か？

        // TODO ClearCache? これ自体は保持せずFactoryに分離？、QueryとParameter？
        // TODO MetadataFactory + Naming (exception if exist) ?
        // TODO MetadataRepository

        // TODO Micro components style? デフォルトのは置換で? 順番の問題があるからダメか？

        public IDelegateFactory DelegateFactory { get; set; } = Smart.Reflection.DelegateFactory.Default;

        public IObjectConverter Converter { get; set; } = ObjectConverter.Default;

        //--------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------

        public SqlMapperConfig()
        {
            ResetParameterBuilderFactories();
            ResetResultMappers();
            ResetTypeMap();
        }

        //--------------------------------------------------------------------------------
        // Config
        //--------------------------------------------------------------------------------

        public SqlMapperConfig ResetParameterBuilderFactories()
        {
            // TODO clear cache?
            parameterBuilderFactories = DefaultParameterBuilders;
            return this;
        }

        public SqlMapperConfig AddParameterBuilderFactory(IParameterBuilderFactory factory)
        {
            // TODO clear cache?
            var builders = new IParameterBuilderFactory[parameterBuilderFactories.Length + 1];
            Array.Copy(parameterBuilderFactories, 0, builders, 0, parameterBuilderFactories.Length - DefaultParameterBuilders.Length);
            builders[parameterBuilderFactories.Length - DefaultParameterBuilders.Length] = factory;
            Array.Copy(DefaultParameterBuilders, 0, builders, builders.Length - DefaultParameterBuilders.Length, DefaultParameterBuilders.Length);
            return this;
        }

        public SqlMapperConfig ResetResultMappers()
        {
            // TODO clear cache?
            resultMappers = DefaultResultMappers;
            return this;
        }

        public SqlMapperConfig AddResultMappers(IResultMapperFactory mapper)
        {
            // TODO clear cache?
            var builders = new IResultMapperFactory[resultMappers.Length + 1];
            Array.Copy(resultMappers, 0, builders, 0, resultMappers.Length - DefaultResultMappers.Length);
            builders[resultMappers.Length - DefaultResultMappers.Length] = mapper;
            Array.Copy(DefaultResultMappers, 0, builders, builders.Length - DefaultResultMappers.Length, DefaultResultMappers.Length);
            return this;
        }

        public SqlMapperConfig ResetTypeMap()
        {
            typeMap.Clear();
            typeMap[typeof(byte)] = DbType.Byte;
            typeMap[typeof(sbyte)] = DbType.SByte;
            typeMap[typeof(short)] = DbType.Int16;
            typeMap[typeof(ushort)] = DbType.UInt16;
            typeMap[typeof(int)] = DbType.Int32;
            typeMap[typeof(uint)] = DbType.UInt32;
            typeMap[typeof(long)] = DbType.Int64;
            typeMap[typeof(ulong)] = DbType.UInt64;
            typeMap[typeof(float)] = DbType.Single;
            typeMap[typeof(double)] = DbType.Double;
            typeMap[typeof(decimal)] = DbType.Decimal;
            typeMap[typeof(bool)] = DbType.Boolean;
            typeMap[typeof(string)] = DbType.String;
            typeMap[typeof(char)] = DbType.StringFixedLength;
            typeMap[typeof(Guid)] = DbType.Guid;
            typeMap[typeof(DateTime)] = DbType.DateTime;
            typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
            typeMap[typeof(TimeSpan)] = DbType.Time;
            typeMap[typeof(byte[])] = DbType.Binary;
            typeMap[typeof(byte?)] = DbType.Byte;
            typeMap[typeof(sbyte?)] = DbType.SByte;
            typeMap[typeof(short?)] = DbType.Int16;
            typeMap[typeof(ushort?)] = DbType.UInt16;
            typeMap[typeof(int?)] = DbType.Int32;
            typeMap[typeof(uint?)] = DbType.UInt32;
            typeMap[typeof(long?)] = DbType.Int64;
            typeMap[typeof(ulong?)] = DbType.UInt64;
            typeMap[typeof(float?)] = DbType.Single;
            typeMap[typeof(double?)] = DbType.Double;
            typeMap[typeof(decimal?)] = DbType.Decimal;
            typeMap[typeof(bool?)] = DbType.Boolean;
            typeMap[typeof(char?)] = DbType.StringFixedLength;
            typeMap[typeof(Guid?)] = DbType.Guid;
            typeMap[typeof(DateTime?)] = DbType.DateTime;
            typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
            typeMap[typeof(TimeSpan?)] = DbType.Time;
            typeMap[typeof(object)] = DbType.Object;
            return this;
        }

        //--------------------------------------------------------------------------------
        // ISqlMapperConfig
        //--------------------------------------------------------------------------------

        public Func<object, object> CreateGetter(PropertyInfo pi)
        {
            return DelegateFactory.CreateGetter(pi);
        }

        public Func<object, object> CreateParser(Type sourceType, Type destinationType)
        {
            // TODO ITypeHandler, zero no lookup ?
            return Converter.CreateConverter(sourceType, destinationType);
        }

        public DbType LookupDbType(Type type, out ITypeHandler handler)
        {
            handler = null;

            //var nullUnderlyingType = Nullable.GetUnderlyingType(type);
            //if (nullUnderlyingType != null)
            //{
            //    type = nullUnderlyingType;
            //}

            if (typeMap.TryGetValue(type, out var dbType))
            {
                return dbType;
            }

            //        if (type.IsEnum && snapShot.TryGetValue(Enum.GetUnderlyingType(type), out dbType))
            //        {
            //            return dbType;
            //        }

            //        throw new ArgumentException($"Type {type.FullName} can't be used", nameof(type));

            // TODO Handler with Selector with default?
            throw new System.NotImplementedException();
        }

        public Func<IDataRecord, T> CreateMapper<T>()
        {
            throw new NotImplementedException();
        }

        public Action<IDbCommand, object> CreateParameterBuilder(Type type)
        {
            if (!parameterBuilderCache.TryGetValue(type, out var parameterBuilder))
            {
                parameterBuilder = parameterBuilderCache.AddIfNotExist(type, CreateParameterBuilderInternal);
            }

            return parameterBuilder;
        }

        private Action<IDbCommand, object> CreateParameterBuilderInternal(Type type)
        {
            foreach (var factory in parameterBuilderFactories)
            {
                if (factory.IsMatch(type))
                {
                    return factory.CreateBuilder(this, type);
                }
            }

            throw new SqlMapperException($"Parameter type is not supported. type=[{type.FullName}]");
        }
    }
}
