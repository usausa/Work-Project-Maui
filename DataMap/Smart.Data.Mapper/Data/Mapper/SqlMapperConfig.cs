namespace Smart.Data.Mapper
{
    using Smart.Converter;
    using Smart.Data.Mapper.Metadata;
    using Smart.Data.Mapper.Selectors;

    // TODO インターフェース抽出？、メタデータが取得できれば良いだけか？

    public sealed class SqlMapperConfig : ISqlMapperConfig
    {
        public SqlMapperConfig Default { get; } = new SqlMapperConfig();

        public IDbTypeSelector DbTypeSelector { get; set; } // = defaultにする？

        public ITypeMetadataFactory MetadataFactory { get; set; }

        //public Dictionary<Type, object> Plugin, Addを用意して、拡張メソッドでIFをキーとするメソッドを！、一応Lock

        // -----------------------------

        // TODO メタデータはFactory側で保持、Parameter、HandlerのキャッシュはConfigで保持か？

        // TODO ClearCache? これ自体は保持せずFactoryに分離？、QueryとParameter？
        // TODO MetadataFactory + Naming (exception if exist) ?
        // TODO MetadataRepository

        // TODO Micro components style? デフォルトのは置換で? 順番の問題があるからダメか？

        public IObjectConverter Converter { get; set; } = ObjectConverter.Default;

        // TODO Parameter
        //public static class DefaultParameterBuilders
        //{
        //    public static IList<IParameterBuilder> Create()
        //    {
        //        return new List<IParameterBuilder>
        //        {
        //            new DynamicParameterParameterBuilder(),
        //            new DictionaryParameterBuilder(DbTypeMap.Default),
        //            new ObjectParameterBuilder(DbTypeMap.Default, DefaultTypeMetaDataFactory.Default)
        //        };
        //    }
        //}

        // TODO Query
        //public static class DefaultQueryHandlers
        //{
        //    public static IList<IQueryHandler> Create()
        //    {
        //        return new List<IQueryHandler>
        //        {
        //            new DictionaryQueryHandler(),
        //            new ObjectQueryHandler(DefaultTypeMetaDataFactory.Default)
        //        };
        //    }
        //}
    }
}
