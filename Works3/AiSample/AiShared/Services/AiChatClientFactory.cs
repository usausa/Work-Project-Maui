namespace Template.MobileApp.Services;

using Microsoft.Extensions.AI;

using OllamaSharp;

// チャット (LLM) のクライアントを設定から生成する。画面は IChatClient にだけ依存し、接続先の種類 (Ollama / OpenAI / Azure OpenAI) はここで差し替える
public sealed class AiChatClientFactory
{
    private readonly Settings settings;

    public AiChatClientFactory(Settings settings)
    {
        this.settings = settings;
    }

    // 表示用のモデル名 (未設定は空)
    public string Model => settings.OllamaModel;

    // 未設定なら null (呼び出し側は疑似応答にする)。返したクライアントは呼び出し側が破棄する
    public IChatClient? Create()
    {
        if (String.IsNullOrEmpty(settings.OllamaModel) || !Uri.TryCreate(settings.OllamaEndPoint, UriKind.Absolute, out var uri))
        {
            return null;
        }

        return new OllamaApiClient(uri, settings.OllamaModel);
    }
}
