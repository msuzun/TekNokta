using Microsoft.AspNetCore.Identity;
using TekNokta.Application.Services;
using TekNokta.Domain.Entities;

namespace TekNokta.Infrastructure.Services;

public sealed class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> passwordHasher = new();

    public string HashPassword(string password)
    {
        return passwordHasher.HashPassword(user: null!, password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var result = passwordHasher.VerifyHashedPassword(user: null!, passwordHash, password);

        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
