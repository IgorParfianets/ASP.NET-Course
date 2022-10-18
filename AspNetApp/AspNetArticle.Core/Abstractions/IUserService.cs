using AspNetArticle.Core.DataTransferObjects;
namespace AspNetArticle.Core.Abstractions;

public interface IUserService
{
    Task<int> RegisterUser(UserDto userDto);
    Task<UserDto?> GetUser(UserDto userDto);
    Task<UserDto> UpdateUser(UserDto userDto);
    Task<UserDto?> GetUserByEmailAndPassword(string email, string password);
    Task<bool> IsExistUserEmailAsync(string email); // для Remote (Check Email)
}
