namespace WorkDesign;

public class BasicEntity
{
    public int Id { get; set; }

    public string Group { get; set; } = default!;

    public string Name { get; set; } = default!;
}

public class BasicService
{
    public static IEnumerable<BasicEntity> GetData()
    {
        for (var x = 1; x <= 5; x++)
        {
            for (var y = 1; y <= 3; y++)
            {
                yield return new BasicEntity
                {
                    Id = x * 10 + y,
                    Group = $"{x}",
                    Name = $"Name-{x}{y}"
                };
            }
        }
    }
}

public sealed class BasicGroup : CollectionGroup<string, BasicEntity>
{
    public BasicGroup(string key)
        : base(key)
    {
    }

    public BasicGroup(string key, IEnumerable<BasicEntity> items, bool isExpanded = true)
        : base(key, items, isExpanded)
    {
    }
}

