namespace PushClient;

using System.Text.Json.Serialization;

// サーバの配信 API の要求 (JSON)
internal sealed record PushRequest(string Title, string Body);

// トリミング時にもリフレクション無しでシリアライズできるようソース生成する
[JsonSerializable(typeof(PushRequest))]
internal sealed partial class PushJsonContext : JsonSerializerContext;
