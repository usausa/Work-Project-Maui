namespace Smart.Data.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TableAttribute : Attribute
    {
        public string Name { get; }

        public TableAttribute(string name)
        {
            Name = name;
        }
    }
}
