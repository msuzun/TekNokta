namespace TekNokta.Application.DTOs.Auth;

public sealed class LoginRequestDto
{
    public required string Email { get; set; }

    public required string Password { get; set; }
}
