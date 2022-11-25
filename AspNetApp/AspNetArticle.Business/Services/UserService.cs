using AspNetArticle.Core;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Data.Abstractions;
using AspNetArticle.Database.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AspNetArticle.Business.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public UserService(IMapper mapper, 
        IUnitOfWork unitOfWork, 
        IConfiguration configuration)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<int> RegisterUserAsync(UserDto userDto, string password) 
    {
        var entity = _mapper.Map<User>(userDto);
        entity.PasswordHash = CreateMd5(password); 

        await _unitOfWork.Users.AddAsync(entity);
        var result = _unitOfWork.Commit();

        return await result;
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var entity = await _unitOfWork.Users.GetByIdAsync(id);
        var user = _mapper.Map<UserDto>(entity);

        return user;
    }

    public async Task<int> UpdateUserAsync(Guid id, UserDto userDto) 
    {
        var entity = await _unitOfWork.Users.GetByIdAsync(id);

        var patchList = new List<PatchModel>();

        if (userDto != null && entity != null)
        {
            if (!userDto.UserName
                    .Equals(entity.UserName))
            {
                patchList.Add(new PatchModel()
                {
                    PropertyName = nameof(entity.UserName),
                    PropertyValue = userDto.UserName
                });
            }

            if (!(CreateMd5(userDto.Password)
                    .Equals(entity.PasswordHash))) // todo for update password need separate method
            {
                patchList.Add(new PatchModel()
                {
                    PropertyName = nameof(entity.PasswordHash),
                    PropertyValue = CreateMd5(userDto.Password)
                });
            }

            if (!userDto.Spam.Equals(entity.Spam))
            {
                patchList.Add(new PatchModel()
                {
                    PropertyName = nameof(entity.Spam),
                    PropertyValue = userDto.Spam
                });
            }
        }
        await _unitOfWork.Users.PatchAsync(id, patchList);
        return await _unitOfWork.Commit();
    }

    public async Task<bool> CheckUserByEmailAndPasswordAsync(string email, string password)
    {
        var user = await _unitOfWork.Users
            .Get()
            .FirstOrDefaultAsync(user => 
            user.Email.Equals(email));

        if(user != null && user.PasswordHash.Equals(CreateMd5(password)))
        {
            user.LastVisit = DateTime.Now;
            await _unitOfWork.Commit();
            return true;
        }
        return false;
    }

    public async Task<bool> IsExistUserEmailAsync(string email) 
    {
        return await _unitOfWork.Users
            .Get()
            .AnyAsync(user => 
                user.Email.Equals(email));
    }

    public async Task<bool> IsExistUsernameAsync(string newUsername) 
    {
        return await _unitOfWork.Users
          .Get()
          .AnyAsync(user =>
             user.UserName.Equals(newUsername));
    }

    public async Task<UserDto> GetUserByEmailAsync(string email)
    {
        var user = await _unitOfWork.Users
            .FindBy(us => us.Email.Equals(email),
                us => us.Role)
            .Select(user => _mapper.Map<UserDto>(user))
            .FirstOrDefaultAsync();

        return user;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _unitOfWork.Users
            .Get()
            .Include(dto => dto.Role)
            .Select(user => _mapper.Map<UserDto>(user))
            .ToListAsync();

        return users;
    }

    private string CreateMd5(string password)
    {
        var passwordSalt = _configuration["Secrets:PasswordSalt"];

        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(password + passwordSalt);
            var hashBytes = md5.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes);
        }
    }
}
