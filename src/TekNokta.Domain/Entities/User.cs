using TekNokta.Domain.Common;

namespace TekNokta.Domain.Entities;

public sealed class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? LastLoginAt { get; set; }
}
