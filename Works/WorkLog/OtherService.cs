using Microsoft.Extensions.Logging;

namespace Other;

public class OtherService
{
    private readonly ILogger<OtherService> log;

    public OtherService(ILogger<OtherService> log)
    {
        this.log = log;
    }

    public void Execute()
    {
        log.LogTrace($"{nameof(OtherService)} trace.");
        log.LogDebug($"{nameof(OtherService)} debug.");
        log.LogInformation($"{nameof(OtherService)} information.");
        log.LogWarning($"{nameof(OtherService)} warning.");
        log.LogError($"{nameof(OtherService)} error.");
        log.LogCritical($"{nameof(OtherService)} critical.");
    }
}
