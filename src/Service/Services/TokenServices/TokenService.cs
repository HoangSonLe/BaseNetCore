using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services.TokenServices
{
    public class JwtViewModel
    {
        public string JwtToken { get; set; }
        public DateTime? Expires { get; set; }
    }
    public class TokenService : ITokenService
    {
        private readonly JwtIssuerSettings _jwtIssuerSettings;
        private readonly TokenSettings _tokenSettings;

        public TokenService(
             IOptions<TokenSettings> tokenOptions)
        {
            _tokenSettings = tokenOptions.Value ?? throw new ArgumentNullException(nameof(tokenOptions));

            _jwtIssuerSettings = new JwtIssuerSettings()
            {
                SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_tokenSettings.SecretKey)),
                SecurityAlgorithms.HmacSha256Signature),
                ValidFor = new TimeSpan(0, int.Parse(_tokenSettings.ExpireTimeInMinutes), 0),
            };
        }

        public JwtViewModel GenerateJwtToken(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("UserID", userId) }),
                Expires = _jwtIssuerSettings.Expiration,
                SigningCredentials = _jwtIssuerSettings.SigningCredentials,
                Audience = _jwtIssuerSettings.Audience,
                Issuer = _jwtIssuerSettings.Issuer,
                IssuedAt = _jwtIssuerSettings.IssuedAt,
                NotBefore = _jwtIssuerSettings.NotBefore
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new JwtViewModel()
            {
                JwtToken = tokenHandler.WriteToken(token),
                Expires = tokenDescriptor.Expires
            };

        }

        public string GenerateRefreshToken()
        {

            //using (var rng = RandomNumberGenerator.Create())
            //{
            //    var randomNumber = new byte[32];
            //    rng.GetBytes(randomNumber);
            //    return Convert.ToBase64String(randomNumber);
            //}

            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);

            }
        }

        public JwtSecurityToken GetJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_tokenSettings.SecretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero

                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                return jwtToken;
            }
            catch (Exception e) { return null; }

        }

        public bool VerifyJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_tokenSettings.SecretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                // return account id from JWT token if validation successful
                return true;
            }
            catch
            {
                // return null if validation fails
                return false;
            }
        }

        //// --------------- Private Method --------------------  //
        //private string GenerateJwtTokenWithClaims(List<Claim> claims)
        //{

        //    var creds = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha512Signature);
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(claims),
        //        Expires = _jwtIssuerSettings.Expiration,
        //        SigningCredentials = creds,
        //        Audience = _jwtIssuerSettings.Audience,
        //        Issuer = _jwtIssuerSettings.Issuer
        //    };

        //    var tokenHandler = new JwtSecurityTokenHandler();

        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    var timeLife = tokenHandler.TokenLifetimeInMinutes;
        //    return tokenHandler.WriteToken(token);
        //}

        //private ClaimsPrincipal GetPrincipalFromToken(string token)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();

        //    var tokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
        //        ValidateIssuer = false,
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = _securityKey,
        //        ValidateLifetime = true, //check the token's expiration date
        //                                 // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
        //        ClockSkew = TimeSpan.Zero
        //    };
        //    var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

        //    return principal;
        //}
    }
}
