namespace Example.Server.Core.Components.Storage
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class LocalStoage : IStorage
    {
        private readonly string root;

        public LocalStoage(LocalStoageSettings settings)
        {
            root = Path.GetFullPath(settings.Root).TrimEnd('\\', '/');
        }

        private string NormarizePath(string path)
        {
            if (path == null)
            {
                path = string.Empty;
            }
            else if (!path.StartsWith("\\") && !path.StartsWith("/"))
            {
                path = "\\" + path;
            }

            var fullPath = Path.GetFullPath(root + path);
            return fullPath.Length >= root.Length ? fullPath : null;
        }

        public Task<string[]> List(string path)
        {
            path = NormarizePath(path);
            if (path == null)
            {
                return Task.FromResult((string[])null);
            }

            if (!Directory.Exists(path))
            {
                return Task.FromResult((string[])null);
            }

            return Task.FromResult(Directory.GetFiles(path).Select(Path.GetFileName).ToArray());
        }

        public Task<Stream> Read(string path)
        {
            path = NormarizePath(path);
            if (path == null)
            {
                return Task.FromResult((Stream)null);
            }

            if (!File.Exists(path))
            {
                return Task.FromResult((Stream)null);
            }

            return Task.FromResult((Stream)File.OpenRead(path));
        }

        public Task<bool> Write(string path, Stream stream)
        {
            path = NormarizePath(path);
            if (path == null)
            {
                return Task.FromResult(false);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (var fs = File.OpenWrite(path))
            {
                stream.CopyTo(fs);
            }

            return Task.FromResult(true);
        }
    }
}
