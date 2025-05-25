namespace WorkServer.Controllers;

using Smart.AspNetCore.Filters;

[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class StorageController : ControllerBase
{
    private const string ContextType = "application/octet-stream";

    private ILogger<StorageController> log;

    public StorageController(ILogger<StorageController> log)
    {
        this.log = log;
    }

    [HttpGet("{**path}")]
    public IActionResult Get([FromRoute] string? path = "/")
    {
        log.LogInformation("Storage download. path=[{path}]", path);

        return File(new byte[1024 * 1024], ContextType);
    }

    [HttpPost("{**path}")]
    [ReadableBodyStream]
    public async ValueTask<IActionResult> Post([FromRoute] string path)
    {
        using var ms = new MemoryStream();
        await Request.Body.CopyToAsync(ms).ConfigureAwait(false);

        log.LogInformation("Storage upload. path=[{path}], size=[{size}]", path, ms.Length);

        return Ok();
    }
}
