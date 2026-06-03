using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TekNokta.Api.Controllers;

[Route("api/test")]
[ApiController]
public sealed class TestController : ControllerBase
{
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult Public()
    {
        return Ok("Public endpoint çalışıyor.");
    }

    [HttpGet("protected")]
    [Authorize]
    public IActionResult Protected()
    {
        return Ok("Protected endpoint çalışıyor.");
    }
}
