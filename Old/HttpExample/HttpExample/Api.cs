namespace HttpExample;

public class ItemResponse
{
    public string Code { get; set; } = default!;

    public string Name { get; set; } = default!;
}

public class EchoRequest
{
    public string Message { get; set; } = default!;
}

public class EchoResponse
{
    public string Message { get; set; } = default!;
}
