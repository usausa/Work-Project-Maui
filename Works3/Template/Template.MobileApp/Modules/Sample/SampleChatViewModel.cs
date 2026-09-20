namespace Template.MobileApp.Modules.Sample;

using System.Collections.ObjectModel;
using System.Text;

using Microsoft.Extensions.AI;

using OllamaSharp;
using OllamaSharp.Models.Exceptions;

using Template.MobileApp.Models.Sample.Chat;

using AiMessage = Microsoft.Extensions.AI.ChatMessage;

public sealed partial class SampleChatViewModel : AppViewModelBase
{
    private readonly ISpeechService speech;

    private readonly IChatClient chatClient;

    private readonly string model;

    private readonly List<AiMessage> history = [];

    private bool responding;

    [ObservableProperty]
    public partial string InputText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsListening { get; private set; }

    public ObservableCollection<AiChatMessage> Messages { get; } = [];

    public IObserveCommand SendCommand { get; }

    public IObserveCommand VoiceCommand { get; }

    public SampleChatViewModel(
        ISpeechService speech,
        Settings settings)
    {
        this.speech = speech;

        chatClient = new OllamaApiClient(new Uri(settings.OllamaEndPoint), settings.OllamaModel);
        model = settings.OllamaModel;

        SendCommand = MakeAsyncCommand(SendAsync, () => !responding && !String.IsNullOrWhiteSpace(InputText));
        VoiceCommand = MakeAsyncCommand(ToggleVoiceAsync);

        Disposables.Add(speech.RecognizedAsObservable().ObserveOnCurrentContext().Subscribe(x =>
        {
            if (!IsListening)
            {
                return;
            }

            if (!String.IsNullOrEmpty(x.Text))
            {
                InputText = x.Text;
            }

            if (x.Complete)
            {
                IsListening = false;
            }
        }));

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(InputText))
            {
                SendCommand.RaiseCanExecuteChanged();
            }
        };
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            chatClient.Dispose();
        }

        base.Dispose(disposing);
    }

    public override Task OnNavigatedToAsync(INavigationContext context)
    {
        if (Messages.Count == 0)
        {
            Messages.Add(new AiChatMessage
            {
                Role = AiChatRole.Assistant,
                Text = $"こんにちは!AI アシスタントです。開発に関する質問をどうぞ 🤖\n(Ollama: {model})"
            });
        }
        return Task.CompletedTask;
    }

    public override Task OnNavigatingFromAsync(INavigationContext context) => CancelVoiceAsync();

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.SampleMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();

    private async Task SendAsync()
    {
        await CancelVoiceAsync();

        var text = InputText.Trim();
        InputText = string.Empty;
        Messages.Add(new AiChatMessage { Role = AiChatRole.User, Text = text });

        responding = true;
        SendCommand.RaiseCanExecuteChanged();
        try
        {
            await RespondAsync(text);
        }
        finally
        {
            responding = false;
            SendCommand.RaiseCanExecuteChanged();
        }
    }

    private async Task RespondAsync(string text)
    {
        var message = new AiChatMessage { Role = AiChatRole.Assistant, IsTyping = true };
        Messages.Add(message);

        history.Add(new AiMessage(ChatRole.User, text));
        var builder = new StringBuilder();
        try
        {
            await foreach (var update in chatClient.GetStreamingResponseAsync(history).ConfigureAwait(true))
            {
                builder.Append(update.Text);
                message.IsTyping = false;
                message.Text = builder.ToString();
            }

            history.Add(new AiMessage(ChatRole.Assistant, builder.ToString()));
        }
        catch (Exception ex) when (ex is HttpRequestException or OllamaException or OperationCanceledException)
        {
            history.RemoveAt(history.Count - 1);
            message.IsTyping = false;
            message.Text = $"応答を取得できませんでした。\n{ex.Message}";
        }
    }

    private async Task ToggleVoiceAsync()
    {
        if (IsListening)
        {
            await speech.RecognizeStopAsync();
            return;
        }

        if (!await Permissions.RequestMicrophoneAsync())
        {
            return;
        }

        IsListening = true;
        if (!await speech.RecognizeAsync(CultureInfo.CurrentCulture))
        {
            IsListening = false;
        }
    }

    private async Task CancelVoiceAsync()
    {
        if (IsListening)
        {
            IsListening = false;
            await speech.RecognizeCancelAsync();
        }
    }
}
