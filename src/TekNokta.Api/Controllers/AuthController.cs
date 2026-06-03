using Microsoft.AspNetCore.Mvc;
using TekNokta.Application.DTOs.Auth;
using TekNokta.Application.Services;

namespace TekNokta.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return Unauthorized(new { message = result.Error });
        }

        return Ok(result.Value);
    }
}
