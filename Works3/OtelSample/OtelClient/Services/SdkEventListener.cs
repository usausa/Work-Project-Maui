namespace OtelClient.Services;

using System.Diagnostics.Tracing;
using System.Globalization;

// OpenTelemetry SDK / エクスポーターの自己診断 (EventSource) を拾う。送信の失敗は例外にならずここに出る
// Android は EventSourceSupport が既定で false のため csproj で有効化している
public sealed class SdkEventListener : EventListener
{
    private const string Prefix = "OpenTelemetry";

    private const int MaxLength = 300;

    public event EventHandler<SdkEventArgs>? Written;

    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        if (eventSource.Name.StartsWith(Prefix, StringComparison.Ordinal))
        {
            EnableEvents(eventSource, EventLevel.Warning);
        }

        base.OnEventSourceCreated(eventSource);
    }

    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    {
        var message = eventData.Message ?? eventData.EventName ?? string.Empty;
        if ((eventData.Payload is { Count: > 0 }) && (eventData.Message is not null))
        {
            try
            {
                message = String.Format(CultureInfo.InvariantCulture, eventData.Message, [.. eventData.Payload]);
            }
            catch (FormatException)
            {
                // 書式が合わないときは生のまま
            }
        }

        // 例外の文字列はスタックトレースを含むため 1 行目だけにする
        var end = message.IndexOfAny(['\r', '\n']);
        if (end >= 0)
        {
            message = message[..end];
        }

        if (message.Length > MaxLength)
        {
            message = message[..MaxLength];
        }

        Written?.Invoke(this, new SdkEventArgs(eventData.EventSource.Name, eventData.Level, message));
    }
}

public sealed class SdkEventArgs : EventArgs
{
    public string Source { get; }

    public EventLevel Level { get; }

    public string Message { get; }

    public SdkEventArgs(string source, EventLevel level, string message)
    {
        Source = source;
        Level = level;
        Message = message;
    }
}
