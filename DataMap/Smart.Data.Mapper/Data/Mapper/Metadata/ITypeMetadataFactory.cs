namespace Smart.Data.Mapper.Metadata
{
    public interface ITypeMetadataFactory
    {
        TypeMetadata Create<T>();
    }
}
