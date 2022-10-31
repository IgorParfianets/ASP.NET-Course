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

    public UserService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    //------------------------------------------  Registration
    public async Task<int> RegisterUserAsync(UserDto userDto) 
    {
        var entity = _mapper.Map<User>(userDto);
        entity.PasswordHash = CreateMd5(userDto.Password); 

        await _unitOfWork.Users.AddAsync(entity);
        var result = _unitOfWork.Commit();

        return await result;
    }

    //------------------------------------------  Get User
    public async Task<UserDto?> GetUserAsync(Guid id)
    {
        var entity = await _unitOfWork.Users.GetByIdAsync(id);
        var user = _mapper.Map<UserDto>(entity);

        return user;
    }

    //------------------------------------------  Edit
    public async Task<int> UpdateUserAsync(Guid id, UserDto userDto) 
    {
        var entity = await _unitOfWork.Users.GetByIdAsync(id);

        var patchList = new List<PatchModel>();

        if (userDto != null)
        {
            if (!userDto.UserName.Equals(entity.UserName))
            {
                patchList.Add(new PatchModel()
                {
                    PropertyName = nameof(userDto.UserName),
                    PropertyValue = userDto.UserName
                });
            }

            if (!(CreateMd5(userDto.Password)
                    .Equals(entity.PasswordHash))) // todo for update password need separate method
            {
                patchList.Add(new PatchModel()
                {
                    PropertyName = nameof(userDto.Password),
                    PropertyValue = userDto.Password
                });
            }

            if (!userDto.Spam.Equals(entity.Spam))
            {
                patchList.Add(new PatchModel()
                {
                    PropertyName = nameof(userDto.Spam),
                    PropertyValue = userDto.Spam
                });
            }
        }
        await _unitOfWork.Articles.PatchAsync(id, patchList);
        return await _unitOfWork.Commit();
    }

    //------------------------------------------  Login
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

    //------------------------------------------  CheckEmail and UserName is Exist
    public async Task<bool> IsExistUserEmailAsync(string email) // для Remote (Check Email)
    {
        return await _unitOfWork.Users
            .Get()
            .AnyAsync(user => 
                user.Email.Equals(email));
    }

    public async Task<bool> IsExistUserNameAsync(string name)
    {
        return await _unitOfWork.Users
            .Get()
            .AnyAsync(user => 
                user.UserName.Equals(name));
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


    //---------------------------------- For Hashing Password
    private string CreateMd5(string password)
    {
        var passwordSalt = _configuration["SaltForHashing"];

        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(password + passwordSalt);
            var hashBytes = md5.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes);
        }
    }
}
