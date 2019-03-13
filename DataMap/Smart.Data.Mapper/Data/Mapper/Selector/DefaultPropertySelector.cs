namespace Smart.Data.Mapper.Selector
{
    using System.Reflection;

    public sealed class DefaultPropertySelector : IPropertySelector
    {
        public static DefaultPropertySelector Instance { get; } = new DefaultPropertySelector();

        private DefaultPropertySelector()
        {
        }

        public PropertyInfo Select(PropertyInfo[] properties, string name)
        {
            throw new System.NotImplementedException();
        }
    }
}
