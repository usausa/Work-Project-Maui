namespace WorkServer.Controllers;


public class SecretController : BaseApiController
{
    [HttpGet]
    [Authorize]
    public IActionResult Message()
    {
        return Ok(new SecretMessageResponse
        {
            Message = "ok"
        });
    }
}
