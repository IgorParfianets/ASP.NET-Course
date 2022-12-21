using AspNetArticle.Core.DataTransferObjects;
namespace AspNetArticle.Core.Abstractions;

public interface IUserService
{
    Task<int> RegisterUserAsync(UserDto userDto, string password);
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<int> UpdateUserAsync(Guid id, UserDto userDto);
    Task<bool> CheckUserByEmailAndPasswordAsync(string email, string password);
    Task<bool> IsExistUserEmailAsync(string email); 
    Task<bool> IsExistUsernameAsync(string newUsername); 
    Task<UserDto> GetUserByEmailAsync(string email);
    Task<UserDto?> GetUserByRefreshTokenAsync(Guid token);
    Task<IEnumerable<UserDto>> GetAllUsersAsync(); 
}

