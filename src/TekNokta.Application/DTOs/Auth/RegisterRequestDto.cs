namespace TekNokta.Application.DTOs.Auth;

public sealed class RegisterRequestDto
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public required string Email { get; set; }

    public required string Password { get; set; }

    public string? PhoneNumber { get; set; }
}
