using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Data.Abstractions;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AspNetArticle.Business.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> RegisterUser(UserDto userDto) // Реализовать атрибут Remote для UserName и Email
    {
        var entity = _mapper.Map<User>(userDto);
        await _unitOfWork.Users.AddAsync(entity);
        var result = _unitOfWork.Commit();

        return await result;
    }

    // Получение User
    public async Task<UserDto?> GetUser(UserDto userDto)
    {
        var entity = await _unitOfWork.Users.GetByIdAsync(userDto.Id);

        if(entity != null)
        {
            var user = _mapper.Map<UserDto>(entity);

            if (user != null)
                return user;
        }

        return null;
    }

    // Edit User
    public Task<UserDto> UpdateUser(UserDto userDto) // не реализован
    {
        throw new Exception(nameof(userDto));
        
    }
    // For Login 
    public async Task<UserDto?> GetUserByEmailAndPassword(string email, string password)
    {
        var entity = await _unitOfWork.Users
            .Get()
            .FirstOrDefaultAsync(user => 
            user.Email.Equals(email) 
            && user.PasswordHash.Equals(password));

        if(entity != null)
        {
            entity.LastVisit = DateTime.Now;
            var userDto = _mapper.Map<UserDto>(entity);

            await _unitOfWork.Commit();

            return userDto;
        }
        return null;
    }

    // For CheckEmail is Exist
    public async Task<bool> IsExistUserEmailAsync(string email) // для Remote (Check Email)
    {
        return await _unitOfWork.Users
            .Get()
            .AnyAsync(user =>
            user.Email.Equals(email));
    }
}
