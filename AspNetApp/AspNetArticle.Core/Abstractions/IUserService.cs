using AspNetArticle.Core.DataTransferObjects;
namespace AspNetArticle.Core.Abstractions;

public interface IUserService
{
    Task<int> RegisterUserAsync(UserDto userDto);
    Task<UserDto?> GetUserByIdTaskAsync(Guid id);
    Task<int> UpdateUserAsync(Guid id, UserDto userDto);
    Task<bool> CheckUserByEmailAndPasswordAsync(string email, string password);
    Task<bool> IsExistUserEmailAsync(string email); // для Remote (Check Email)
    Task<bool> IsExistUserNameAsync(string name); // для Remote (Check Username)
    Task<UserDto> GetUserByEmailAsync(string email);



    Task<IEnumerable<UserDto>> GetAllUsersAsync(); // беспонтовый
}

