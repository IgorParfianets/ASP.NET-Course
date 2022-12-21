using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AspNetArticle.Api.Models.Response;
using AspNetArticle.Core.DataTransferObjects;
using AspNetSample.Data.CQS.Commands;
using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace AspNetArticle.Api.Utils
{
    /// <summary>
    /// Jwt util for generate and refresh tokens
    /// </summary>
    public class JwtUtil : IJwtUtil
    {
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="mediator"></param>
        public JwtUtil(IConfiguration configuration, 
            IMediator mediator)
        {
            _configuration = configuration;
            _mediator = mediator;
        }

        /// <summary>
        /// Generate token for user
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<TokenResponse> GenerateTokenAsync(UserDto dto)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:JwtSecret"]));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var nowUtc = DateTime.UtcNow;
            var exp = nowUtc.AddMinutes(double.Parse(_configuration["Token:ExpiryMinutes"]))
                .ToUniversalTime();

            var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, dto.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("D")), 
            new Claim(ClaimTypes.NameIdentifier, dto.Id.ToString("D")),
            new Claim(ClaimTypes.Role, dto.RoleName),
        };

            var jwtToken = new JwtSecurityToken(_configuration["Token:Issuer"],
                _configuration["Token:Issuer"],
                claims,
                expires: exp,
                signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var refreshTokenValue = Guid.NewGuid();

            await _mediator.Send(new AddRefreshTokenCommand()
            {
                UserId = dto.Id,
                TokenValue = refreshTokenValue
            });

            return new TokenResponse()
            {
                AccessToken = accessToken,
                Role = dto.RoleName,
                TokenExpiration = jwtToken.ValidTo,
                UserId = dto.Id,
                RefreshToken = refreshTokenValue
            };
        }

        /// <summary>
        /// Remove user's token
        /// </summary>
        /// <param name="requestRefreshToken"></param>
        /// <returns></returns>
        public async Task RemoveRefreshTokenAsync(Guid requestRefreshToken)
        {
            await _mediator.Send(new DeleteRefreshTokenCommand()
            {
                TokenValue = requestRefreshToken
            });
        }
    }
}
