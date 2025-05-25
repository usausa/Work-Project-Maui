namespace WorkServer.Controllers;


public class DataController : BaseApiController
{
    [HttpGet]
    public IActionResult List()
    {
        return Ok(new DataListResponse
        {
            Entries = Enumerable.Range(1, 10).Select(static x => new DataListResponseEntry { Id = x, Name = $"Data-{x}" }).ToArray()
        });
    }
}
