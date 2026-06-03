using System.ComponentModel.DataAnnotations;

namespace TekNokta.Application.DTOs.Auth;

public sealed class LoginRequestDto
{
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public required string Email { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Password { get; set; }
}
