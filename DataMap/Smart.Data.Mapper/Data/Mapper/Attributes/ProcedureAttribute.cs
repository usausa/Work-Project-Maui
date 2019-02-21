namespace Smart.Data.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ProcedureAttribute : Attribute
    {
        public string Name { get; }

        public ProcedureAttribute(string name)
        {
            Name = name;
        }
    }
}
