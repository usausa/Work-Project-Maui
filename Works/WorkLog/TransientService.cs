using Microsoft.Extensions.Logging;

namespace WorkLog;

public class TransientService
{
    private readonly ILogger<TransientService> log;

    public TransientService(ILogger<TransientService> log)
    {
        this.log = log;
    }

    public void Execute()
    {
        log.LogTrace($"{nameof(TransientService)} trace.");
        log.LogDebug($"{nameof(TransientService)} debug.");
        log.LogInformation($"{nameof(TransientService)} information.");
        log.LogWarning($"{nameof(TransientService)} warning.");
        log.LogError($"{nameof(TransientService)} error.");
        log.LogCritical($"{nameof(TransientService)} critical.");
    }
}
