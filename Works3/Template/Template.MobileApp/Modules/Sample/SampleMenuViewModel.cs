namespace Template.MobileApp.Modules.Sample;

using Template.MobileApp.Services;

public sealed class SampleMenuViewModel : AppViewModelBase
{
    public IObserveCommand ForwardCommand { get; }

    public IObserveCommand ChatCommand { get; }

    public SampleMenuViewModel(
        IDialog dialog,
        AiChatClientFactory chatClientFactory)
    {
        ForwardCommand = MakeAsyncCommand<ViewId>(x => Navigator.ForwardAsync(x));
        ChatCommand = MakeAsyncCommand(async () =>
        {
            if (!chatClientFactory.IsConfigured)
            {
                await dialog.InformationAsync("Ollama end point is not configured.");
                return;
            }

            await Navigator.ForwardAsync(ViewId.SampleChat);
        });
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.Menu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
