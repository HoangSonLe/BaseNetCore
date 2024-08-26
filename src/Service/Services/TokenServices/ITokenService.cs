using System.IdentityModel.Tokens.Jwt;

namespace Application.Services.TokenServices
{
    public interface ITokenService
    {
        JwtViewModel GenerateJwtToken(string userId);
        string GenerateRefreshToken();
        bool VerifyJwtToken(string token);
        JwtSecurityToken GetJwtToken(string token);
    }
}
