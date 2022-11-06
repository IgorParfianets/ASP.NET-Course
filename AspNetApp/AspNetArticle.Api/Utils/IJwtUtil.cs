using AspNetArticle.Api.Models.Response;
using AspNetArticle.Core.DataTransferObjects;

namespace AspNetArticle.Api.Utils
{
    public interface IJwtUtil
    {
        TokenResponse GenerateToken(UserDto dto);
    }
}
