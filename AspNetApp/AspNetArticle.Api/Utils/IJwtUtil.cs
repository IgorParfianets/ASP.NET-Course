using AspNetArticle.Api.Models.Response;
using AspNetArticle.Core.DataTransferObjects;

namespace AspNetArticle.Api.Utils
{
    public interface IJwtUtil
    {
        Task<TokenResponse> GenerateTokenAsync(UserDto dto);
        Task RemoveRefreshTokenAsync(Guid requestRefreshToken);
    }
}
