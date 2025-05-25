namespace WorkServer.Controllers;

public class ServerController : BaseApiController
{
    [HttpGet]
    public IActionResult Time()
    {
        return Ok(new ServerTimeResponse { DateTime = DateTime.Now });
    }
}
