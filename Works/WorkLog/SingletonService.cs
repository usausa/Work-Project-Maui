using Microsoft.Extensions.Logging;

namespace WorkLog;

public class SingletonService
{
    private readonly ILogger<SingletonService> log;

    public SingletonService(ILogger<SingletonService> log)
    {
        this.log = log;
    }

    public void Execute()
    {
        log.LogTrace($"{nameof(SingletonService)} trace.");
        log.LogDebug($"{nameof(SingletonService)} debug.");
        log.LogInformation($"{nameof(SingletonService)} information.");
        log.LogWarning($"{nameof(SingletonService)} warning.");
        log.LogError($"{nameof(SingletonService)} error.");
        log.LogCritical($"{nameof(SingletonService)} critical.");
    }
}
