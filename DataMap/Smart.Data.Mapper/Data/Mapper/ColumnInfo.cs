namespace Smart.Data.Mapper
{
    using System;

    public sealed class ColumnInfo
    {
        public string Name { get; }

        public Type Type { get; }

        public ColumnInfo(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}
