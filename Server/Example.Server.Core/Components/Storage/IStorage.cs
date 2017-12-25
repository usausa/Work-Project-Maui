namespace Example.Server.Core.Components.Storage
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IStorage
    {
        Task<string[]> List(string path);

        Task<Stream> Read(string path);

        Task<bool> Write(string path, Stream stream);
    }
}
