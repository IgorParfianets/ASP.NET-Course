using AspNetArticle.Core.DataTransferObjects;
namespace AspNetArticle.Core.Abstractions;

public interface IUserService
{
    Task<int> RegisterUser(UserDto userDto);
    Task<UserDto?> GetUser(Guid id);
    Task<int> UpdateUser(Guid id, UserDto userDto);
    Task<bool> CheckUserByEmailAndPasswordAsync(string email, string password);
    Task<bool> IsExistUserEmailAsync(string email); // для Remote (Check Email)
    Task<bool> IsExistUserNameAsync(string name); // для Remote (Check Username)
    Task<UserDto> GetUserByEmailAsync(string email);
}

