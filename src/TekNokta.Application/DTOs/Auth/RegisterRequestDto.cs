using System.ComponentModel.DataAnnotations;

namespace TekNokta.Application.DTOs.Auth;

public sealed class RegisterRequestDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public required string Email { get; set; }

    [Required]
    [MinLength(6)]
    [MaxLength(100)]
    public required string Password { get; set; }

    [MaxLength(30)]
    public string? PhoneNumber { get; set; }
}
