using Microsoft.Extensions.Options;
using TekNokta.Application.Common.Results;
using TekNokta.Application.DTOs.Auth;
using TekNokta.Application.Repositories;
using TekNokta.Application.Services;
using TekNokta.Domain.Entities;
using TekNokta.Infrastructure.Authentication;

namespace TekNokta.Infrastructure.Services;

public sealed class AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IOptions<JwtSettings> jwtOptions) : IAuthService
{
    private readonly JwtSettings jwtSettings = jwtOptions.Value;

    public async Task<Result<AuthResponseDto>> RegisterAsync(
        RegisterRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(request.Email);

        if (await userRepository.ExistsByEmailAsync(normalizedEmail, cancellationToken))
        {
            return Result<AuthResponseDto>.Failure("Bu e-posta adresi zaten kullanılıyor.");
        }

        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = normalizedEmail,
            PasswordHash = passwordHasher.HashPassword(request.Password),
            PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim()
        };

        await userRepository.AddAsync(user, cancellationToken);

        var response = CreateAuthResponse(user);

        return Result<AuthResponseDto>.Success(response);
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(request.Email);
        var user = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null)
        {
            return Result<AuthResponseDto>.Failure("E-posta veya şifre hatalı.");
        }

        if (!user.IsActive)
        {
            return Result<AuthResponseDto>.Failure("Kullanıcı hesabı aktif değil.");
        }

        if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result<AuthResponseDto>.Failure("E-posta veya şifre hatalı.");
        }

        user.LastLoginAt = DateTime.UtcNow;
        await userRepository.UpdateAsync(user, cancellationToken);

        var response = CreateAuthResponse(user);

        return Result<AuthResponseDto>.Success(response);
    }

    private AuthResponseDto CreateAuthResponse(User user)
    {
        return new AuthResponseDto
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            AccessToken = tokenService.GenerateAccessToken(user),
            ExpiresAt = DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenExpirationMinutes)
        };
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
}
