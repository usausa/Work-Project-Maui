namespace PushServer;

// 配信 API の要求と結果 (JSON)
// 要求は値型にする (シリアライザが生成するだけの型は CA1812 の対象になるため)
internal readonly record struct PushRequest(string Title, string? Body);

internal sealed record PushResponse(DateTimeOffset SentAt, int Connections);
