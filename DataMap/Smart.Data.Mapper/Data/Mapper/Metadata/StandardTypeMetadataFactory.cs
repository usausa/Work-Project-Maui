namespace Smart.Data.Mapper.Metadata
{
    public sealed class StandardTypeMetadataFactory : ITypeMetadataFactory
    {
        public TypeMetadata Create<T>()
        {
            return new TypeMetadata();
        }
    }
}
