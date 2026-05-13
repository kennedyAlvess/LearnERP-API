using Microsoft.AspNetCore.Mvc;

namespace LearnERP.Api.Controllers;

[ApiController]
[Route("health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var status = new
        {
            status = "ok",
            timestamp = DateTimeOffset.UtcNow
        };

        return Ok(status);
    }
}
