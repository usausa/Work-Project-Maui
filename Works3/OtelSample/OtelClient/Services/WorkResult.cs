namespace OtelClient.Services;

// 処理 (スパンをつくる操作) の結果
public sealed record WorkResult(bool Succeeded, string Message);
