namespace Template.MobileApp.State;

// アプリの Settings のうち、コピーした AzureVisionUsecase と ChatConsole が使うメンバーだけを同じ形で持つ。
// 値は Preferences / SecureStorage ではなく起動時の引数 (appsettings.json / 環境変数 / コマンドライン) から入れる
public sealed class Settings
{
    // AI Service (Azure AI Vision)

    public string AIServiceEndPoint { get; set; } = string.Empty;

    public string AIServiceKey { get; set; } = string.Empty;

    // Ollama (Chat)

    public string OllamaEndPoint { get; set; } = string.Empty;

    public string OllamaModel { get; set; } = string.Empty;

    public ValueTask<string?> GetAIServiceKeyAsync() => new(AIServiceKey);
}
