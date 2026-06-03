using TekNokta.Domain.Entities;

namespace TekNokta.Application.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
}
