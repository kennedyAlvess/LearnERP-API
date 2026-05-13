using LearnERP.Api.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnERP.Api.Controllers;

[ApiController]
[Route("v1/auth")]
public sealed class AuthPingController(ITenantContextAccessor tenantContextAccessor) : ControllerBase
{
    [HttpGet("ping")]
    [Authorize]
    public IActionResult Get()
    {
        var response = new
        {
            status = "ok",
            tenantId = tenantContextAccessor.TenantId
        };

        return Ok(response);
    }
}
