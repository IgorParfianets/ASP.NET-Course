using AsoNetArticle.Data.CQS.Commands;
using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Core;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Data.Abstractions;
using AspNetArticle.Database.Entities;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace AspNetArticle.Business.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public UserService(IMapper mapper,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        IMediator mediator)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _mediator = mediator;
    }

    public async Task<int> RegisterUserAsync(UserDto userDto, string password) 
    {
        var entity = _mapper.Map<User>(userDto);

        if (entity == null)
            throw new NullReferenceException($"{nameof(entity)} is null");
        entity.PasswordHash = CreateMd5(password);

        ////await _unitOfWork.Users.AddAsync(entity);
        ////var result = _unitOfWork.Commit();
        var result = await _mediator.Send(new AddUserCommand() { User = entity });

        return result;
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        //var entity = await _unitOfWork.Users.GetByIdAsync(id);
        var entity = await _mediator.Send(new GetUserByIdQuery() { UserId = id });
        if (entity == null)
            throw new NullReferenceException($"{nameof(entity)} is null");

        var user = _mapper.Map<UserDto>(entity);
        if (user == null)
            throw new NullReferenceException($"{nameof(user)} is null");

        return user;
    }

    public async Task<int> UpdateUserAsync(Guid id, UserDto userDto) 
    {
        //var entity = await _unitOfWork.Users.GetByIdAsync(id);
        var entity = await _mediator.Send(new GetUserByIdQuery() { UserId = id });
        if (entity == null)
            throw new NullReferenceException($"{nameof(entity)} is null");

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

            if (!string.IsNullOrEmpty(userDto.Password) && !(CreateMd5(userDto.Password)
                    .Equals(entity.PasswordHash))) 
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

            if (userDto.Avatar != null && !userDto.Avatar.Equals(entity.Avatar)) 
            {
                patchList.Add(new PatchModel()
                {
                    PropertyName = nameof(entity.Avatar),
                    PropertyValue = userDto.Avatar
                });
            }
        }
        var result = await _mediator.Send(new UpdateUserCommand() { UserId = id, PatchData = patchList });
        return result;
        //await _unitOfWork.Users.PatchAsync(id, patchList);
        //return await _unitOfWork.Commit();
    }

    public async Task<bool> CheckUserByEmailAndPasswordAsync(string email, string password)
    {
        var user = await _mediator.Send(new GetUserByEmailQuery() { Email = email });

        //var user = await _unitOfWork.Users
        //     .Get()
        //     .FirstOrDefaultAsync(user =>
        //     user.Email.Equals(email));

        if (user != null && user.PasswordHash.Equals(CreateMd5(password))) 
        {
            user.LastVisit = DateTime.Now;
            await _unitOfWork.Commit(); //что то сделать _unitOfWork
            return true;
        }
        return false;
    }

    public async Task<bool> IsExistUserEmailAsync(string email) // check debug
    {
        var user = await _mediator.Send(new GetUserByEmailQuery() { Email = email });

        return user != null;
        //return await _unitOfWork.Users
        //    .Get()
        //    .AnyAsync(user => 
        //        user.Email.Equals(email));
    }

    public async Task<bool> IsExistUsernameAsync(string newUsername) 
    {
        var user = await _mediator.Send(new GetUserByUsernameQuery() { Username = newUsername });

        return user != null;
        //return await _unitOfWork.Users
        //  .Get()
        //  .AnyAsync(user =>
        //     user.UserName.Equals(newUsername));
    }

    public async Task<UserDto> GetUserByEmailAsync(string email)
    {
        var user = await _mediator.Send(new GetUserByEmailQuery() { Email = email });

        if (user == null)
            throw new NullReferenceException($"{nameof(user)} is null");


        //var user = await _unitOfWork.Users
        //    .FindBy(us => us.Email.Equals(email),
        //        us => us.Role)
        //    .Select(user => _mapper.Map<UserDto>(user))
        //    .FirstOrDefaultAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _mediator.Send(new GetAllUsersQuery());

        if (users == null)
            throw new NullReferenceException($"{nameof(users)} is null");

        //var users = await _unitOfWork.Users
        //    .Get()
        //    .Include(dto => dto.Role)
        //    .Select(user => _mapper.Map<UserDto>(user))
        //    .ToListAsync();

        return users.Select(user => _mapper.Map<UserDto>(user));
    }

    public async Task<UserDto?> GetUserByRefreshTokenAsync(Guid token)
    {
        var user = await _mediator.Send(new GetUserByRefreshTokenQuery() { RefreshToken = token });

        if (user == null)
            throw new NullReferenceException($"{nameof(user)} is null");

        return _mapper.Map<UserDto>(user);
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
