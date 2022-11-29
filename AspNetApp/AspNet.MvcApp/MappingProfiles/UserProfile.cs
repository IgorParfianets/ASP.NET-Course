using AspNetArticle.Core;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database.Entities;
using AspNetArticle.MvcApp.Models;
using AutoMapper;

namespace AspNetArticle.MvcApp.MappingProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dto => dto.Id,
                opt => 
                    opt.MapFrom(user => user.Id))
            .ForMember(dto => dto.UserName, 
                opt => 
                    opt.MapFrom(user => user.UserName))
            .ForMember(dto => dto.Email,
                opt =>
                    opt.MapFrom(user => user.Email))
            .ForMember(dto => dto.Spam,
                opt =>
                    opt.MapFrom(user => user.Spam))
             .ForMember(dto => dto.Avatar,
                opt =>
                    opt.MapFrom(user => user.Avatar))
            .ForMember(dto => dto.RoleName,
                opt =>
                    opt.MapFrom(user => user.Role.Name));

        CreateMap<UserDto, User>() 
            .ForMember(user => user.Id, 
                opt => 
                    opt.MapFrom(dto => dto.Id))
            .ForMember(user => user.UserName, 
                opt => 
                    opt.MapFrom(dto => dto.UserName))
            .ForMember(user => user.PasswordHash, 
                opt => 
                    opt.MapFrom(dto => dto.Password))
            .ForMember(user => user.Email, 
                opt => 
                    opt.MapFrom(dto => dto.Email))
             .ForMember(dto => dto.Avatar,
                opt =>
                    opt.MapFrom(user => user.Avatar))
            .ForMember(user => user.AccountCreated, 
                opt => 
                    opt.MapFrom(dto => DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm"))) 
            .ForMember(user => user.LastVisit, 
                opt => 
                    opt.MapFrom(dto => DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm")))
            .ForMember(user => user.Spam,
                opt
                => opt.MapFrom(dto => dto.Spam))
            .ForMember(user => user.RoleId, 
                opt
                => opt.MapFrom(dto => dto.RoleId));

        CreateMap<UserRegistrationViewModel, UserDto>()                     
           .ForMember(dto => dto.Id, 
               opt => 
                   opt.MapFrom(user => Guid.NewGuid()))
           .ForMember(dto => dto.UserName, 
               opt => 
                   opt.MapFrom(user => user.UserName))
           .ForMember(dto => dto.Password,
               opt => 
                   opt.MapFrom(user => user.Password))
           .ForMember(dto => dto.Email, 
               opt => 
                   opt.MapFrom(user => user.Email))
           .ForMember(dto => dto.Spam, 
               opt => 
                   opt.MapFrom(user => user.Spam));


        CreateMap<UserLoginViewModel, UserDto>()
           .ForMember(dto => dto.Password,
               opt =>
                   opt.MapFrom(user => user.Password))
           .ForMember(dto => dto.Email,
               opt =>
                   opt.MapFrom(user => user.Email));

        CreateMap<UserDto, UserModel>().ConvertUsing(new UserModelMembershipConverter());

        CreateMap<UserDto, UserEditViewModel>().ConvertUsing(new FromUserDtoToUserEditConverter());
        CreateMap<UserEditViewModel, UserDto>().ConvertUsing(new FromUserEditToUserDtoConverter());
    }

    private class FromUserEditToUserDtoConverter : ITypeConverter<UserEditViewModel, UserDto>
    {
        public UserDto Convert(UserEditViewModel source, UserDto destination, ResolutionContext context)
        {
            UserDto userDto = new UserDto
            {
                Id = source.Id,
                UserName = source.UserName,
                Email = source.Email,
                Spam = source.Spam,
            };

            if (source.SaveAvatar != null && source.SaveAvatar.Length > 0)
            {
                byte[] imageData;
                var saveAvatar = source.SaveAvatar;

                using (var binaryReader = new BinaryReader(saveAvatar.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)saveAvatar.Length);
                }

                userDto.Avatar = imageData;
            }
            return userDto;
        }
    }

    private class FromUserDtoToUserEditConverter : ITypeConverter<UserDto, UserEditViewModel>
    {
        public UserEditViewModel Convert(UserDto source, UserEditViewModel destination, ResolutionContext context)
        {
            UserEditViewModel userModel = new UserEditViewModel
            {
                Id = source.Id,
                UserName = source.UserName,
                Email = source.Email,
                Spam = source.Spam,
            };

            var days = (DateTime.Now - source.AccountCreated).Days;

            switch (days)
            {
                case <= 7:
                    userModel.Status = MembershipStatus.Новичек;
                    break;
                case <= 30 when days > 7:
                    userModel.Status = MembershipStatus.Местный;
                    break;
                case <= 180 when days > 30:
                    userModel.Status = MembershipStatus.Опытный;
                    break;
                case > 180:
                    userModel.Status = MembershipStatus.Ветеран;
                    break;
            }

            if(source.Avatar != null)
            {
                var loadAvatar = System.Convert.ToBase64String(source.Avatar);
                userModel.LoadAvatar = loadAvatar;
            }
            return userModel;
        }
    }

    private class UserModelMembershipConverter : ITypeConverter<UserDto, UserModel>
    {
        public UserModel Convert(UserDto source, UserModel destination, ResolutionContext context)
        {
            UserModel userModel = new UserModel
            {
                Id = source.Id,
                UserName = source.UserName,
                Email = source.Email,
                Spam = source.Spam,
                LastVisit = source.LastVisit,
                AccountCreated = source.AccountCreated
            };

            var days = (DateTime.Now - source.AccountCreated).Days;

            switch (days)
            {
                case <= 7:
                    userModel.Status = MembershipStatus.Новичек;
                    break;
                case <= 30 when days > 7:
                    userModel.Status = MembershipStatus.Местный;
                    break;
                case <= 180 when days > 30:
                    userModel.Status = MembershipStatus.Опытный;
                    break;
                case > 180:
                    userModel.Status = MembershipStatus.Ветеран;
                    break;
            }
            return userModel;
        }
    }
}
