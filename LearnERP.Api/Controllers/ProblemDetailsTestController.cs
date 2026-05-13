using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace LearnERP.Api.Controllers;

[ApiController]
[Route("v1/problem-details-test")]
public sealed class ProblemDetailsTestController : ControllerBase
{
    [HttpPost("validation")]
    public IActionResult Validate([FromBody] ValidationRequest request)
    {
        return Ok(request);
    }

    [HttpGet("exception")]
    public IActionResult ThrowException()
    {
        throw new InvalidOperationException("Boom");
    }

    public sealed class ValidationRequest
    {
        [Required]
        [StringLength(10)]
        public string? Name { get; init; }
    }
}
