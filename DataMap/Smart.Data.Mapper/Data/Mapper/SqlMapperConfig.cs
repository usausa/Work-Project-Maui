namespace Smart.Data.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    using Smart.Converter;
    using Smart.Data.Mapper.Handlers;
    using Smart.Data.Mapper.Mappers;
    using Smart.Data.Mapper.Parameters;
    using Smart.Reflection;

    // TODO インターフェース抽出？、メタデータが取得できれば良いだけか？

    public sealed class SqlMapperConfig : ISqlMapperConfig
    {
        private static readonly IParameterBuilder[] DefaultParameterBuilders =
        {
            new DynamicParameterBuilder(),
            new ObjectParameterBuilder()
        };

        private static readonly IResultMapper[] DefaultResultMappers =
        {
            new ObjectResultMapper()
        };

        // TODO Custom ? withHandlers ?
        private readonly Dictionary<Type, DbType> typeMap = new Dictionary<Type, DbType>();

        private IParameterBuilder[] parameterBuilders;

        private IResultMapper[] resultMappers;

        //--------------------------------------------------------------------------------
        // Property
        //--------------------------------------------------------------------------------

        public static SqlMapperConfig Default { get; } = new SqlMapperConfig();

        public IList<IParameterBuilder> ParameterBuilders { get; } = new List<IParameterBuilder>();

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
            ResetParameterBuilders();
            ResetResultMappers();
            ResetTypeMap();
        }

        //--------------------------------------------------------------------------------
        // Config
        //--------------------------------------------------------------------------------

        public SqlMapperConfig ResetParameterBuilders()
        {
            parameterBuilders = DefaultParameterBuilders;
            return this;
        }

        public SqlMapperConfig AddParameterBuilders(IParameterBuilder builder)
        {
            var builders = new IParameterBuilder[parameterBuilders.Length + 1];
            Array.Copy(parameterBuilders, 0, builders, 0, parameterBuilders.Length - DefaultParameterBuilders.Length);
            builders[parameterBuilders.Length - DefaultParameterBuilders.Length] = builder;
            Array.Copy(DefaultParameterBuilders, 0, builders, builders.Length - DefaultParameterBuilders.Length, DefaultParameterBuilders.Length);
            return this;
        }

        public SqlMapperConfig ResetResultMappers()
        {
            resultMappers = DefaultResultMappers;
            return this;
        }

        public SqlMapperConfig AddResultMappers(IResultMapper mapper)
        {
            var builders = new IResultMapper[resultMappers.Length + 1];
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

        public bool BuildCommand(IDbCommand cmd, object param)
        {
            for (var i = 0; i < parameterBuilders.Length; i++)
            {
                if (parameterBuilders[i].Handle(this, cmd, param))
                {
                    return true;
                }
            }

            return false;
        }

        public Func<object, object> CreateParser(Type sourceType, Type destinationType)
        {
            // TODO ITypeHandler
            return Converter.CreateConverter(sourceType, destinationType);
        }
    }
}
