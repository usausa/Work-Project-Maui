namespace Smart.Data.Mapper.Metadata
{
    using System;
    using Smart.Data.Mapper.Namings;

    public sealed class StandardTableInfoFactory<TNaming> : ITableInfoFactory
        where TNaming : INaming
    {
        private static readonly INaming Naming = (INaming)Activator.CreateInstance(typeof(TNaming));

        private static class TableInfoHolder<T>
        {
            public static TableInfo Value { get; }

            static TableInfoHolder()
            {
                // TODO
                Value = new TableInfo(typeof(T), Naming);
            }
        }

        public TableInfo Create<T>()
        {
            return TableInfoHolder<T>.Value;
        }
    }
}
