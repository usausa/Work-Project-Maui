namespace PolicyExample
{
    using System;

    public interface IPolicy
    {
        string ToName(string source);
    }

    public sealed class DefaultPolicy : IPolicy
    {
        public string ToName(string source) => source;
    }

    public sealed class LowerPolicy : IPolicy
    {
        public string ToName(string source) => source.ToLower();
    }

    public abstract class SeparatorPolicy : IPolicy
    {
        public sealed class PathPolicy : SeparatorPolicy
        {
            public PathPolicy() : base(true, "\\")
            {
            }
        }

        public sealed class UnderlinePolicy : SeparatorPolicy
        {
            public UnderlinePolicy() : base(false, "_")
            {
            }
        }

        private readonly bool root;

        private readonly string separator;

        private SeparatorPolicy(bool root, string separator)
        {
            this.root = root;
            this.separator = separator;
        }

        public string ToName(string source) => (root ? separator : "") + source.Replace(".", separator);
    }


    public class Metadata
    {
        public string Name { get; }

        public Metadata(string name)
        {
            Name = name;
        }
    }

    public interface IMetadataFactory
    {
        Metadata Create<T>();
    }

    public sealed class StandardMetadataFactory<TPolicy> : IMetadataFactory
        where TPolicy : IPolicy
    {
        private static readonly IPolicy Policy = (IPolicy)Activator.CreateInstance(typeof(TPolicy));

        private static class MetadataHolder<T>
        {
            public static Metadata Value { get; }

            static MetadataHolder()
            {
                Value = new Metadata(Policy.ToName(typeof(T).FullName));
            }
        }

        public Metadata Create<T>()
        {
            return MetadataHolder<T>.Value;
        }
    }

    public static class MetadataFactories
    {
        public static IMetadataFactory Default { get; } = new StandardMetadataFactory<DefaultPolicy>();

        public static IMetadataFactory Lower { get; } = new StandardMetadataFactory<LowerPolicy>();

        public static IMetadataFactory Path { get; } = new StandardMetadataFactory<SeparatorPolicy. PathPolicy>();

        public static IMetadataFactory Underline { get; } = new StandardMetadataFactory<SeparatorPolicy.UnderlinePolicy>();
    }

    public class Component
    {
        public IMetadataFactory MetadataFactory { get; set; } = MetadataFactories.Default;

        public void ShowName<T>()
        {
            Console.WriteLine(MetadataFactory.Create<T>().Name);
        }
    }


    public static class Program
    {
        public static void Main()
        {
            var component = new Component();

            // System.String
            component.ShowName<string>();

            // system.string
            component.MetadataFactory = MetadataFactories.Lower;
            component.ShowName<string>();

            // \System\String
            component.MetadataFactory = MetadataFactories.Path;
            component.ShowName<string>();

            // System_String
            component.MetadataFactory = MetadataFactories.Underline;
            component.ShowName<string>();
        }
    }
}
