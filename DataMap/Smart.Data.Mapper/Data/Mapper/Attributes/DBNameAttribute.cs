namespace Smart.Data.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class DBNameAttribute : Attribute
    {
        public string Name { get; }

        public DBNameAttribute(string name)
        {
            Name = name;
        }
    }
}
