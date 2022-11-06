using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AspNetArticle.Api.Models.Response;
using AspNetArticle.Core.DataTransferObjects;
using Microsoft.IdentityModel.Tokens;

namespace AspNetArticle.Api.Utils
{
    public class JwtUtil : IJwtUtil
    {
        private readonly IConfiguration _configuration;

        public JwtUtil(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public TokenResponse GenerateToken(UserDto dto)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:JwtSecret"]));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var nowUtc = DateTime.UtcNow;
            var exp = nowUtc.AddMinutes(double.Parse(_configuration["Token:ExpiryMinutes"]))
                .ToUniversalTime();

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, dto.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("D")), //jwt uniq id from spec
                new Claim(ClaimTypes.NameIdentifier, dto.Id.ToString("D")),
                new Claim(ClaimTypes.Role, dto.RoleName),
            };

            var jwtToken = new JwtSecurityToken(_configuration["Token:Issuer"],
                _configuration["Token:Issuer"],
                claims,
                expires: exp,
                signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return new TokenResponse()
            {
                AccessToken = accessToken,
                Role = dto.RoleName,
                TokenExpiration = jwtToken.ValidTo,
                UserId = dto.Id
            };
        }
    }
}
