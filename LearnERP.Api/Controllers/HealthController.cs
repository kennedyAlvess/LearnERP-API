using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LearnERP.Api.Controllers;

[ApiController]
[Route("health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
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
