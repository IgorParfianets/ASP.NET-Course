using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database.Entities;
using AspNetArticle.MvcApp.Models;
using AutoMapper;

namespace AspNetArticle.MvcApp.MappingProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // For Entity -> Dto & Dto -> Entity
        CreateMap<User, UserDto>()
            .ForMember(dto => dto.Id,
                opt => 
                    opt.MapFrom(user => user.Id))
            .ForMember(dto => dto.UserName, 
                opt => 
                    opt.MapFrom(user => user.UserName))
            .ForMember(dto => dto.Password,
                opt =>
                    opt.MapFrom(user => user.PasswordHash))
            .ForMember(dto => dto.Email,
                opt =>
                    opt.MapFrom(user => user.Email))
            .ForMember(dto => dto.Spam,
                opt =>
                    opt.MapFrom(user => user.Spam))
            .ForMember(dto => dto.RoleName,
                opt =>
                    opt.MapFrom(user => user.Role.Name));

        CreateMap<UserDto, User>() // FROM dto TO entity
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
            .ForMember(user => user.AccountCreated, //for entity
                opt => 
                    opt.MapFrom(dto => DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm"))) // from dto
            .ForMember(user => user.LastVisit, 
                opt => 
                    opt.MapFrom(dto => DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm")))
            .ForMember(user => user.Spam,
                opt
                => opt.MapFrom(dto => dto.Spam))
            .ForMember(user => user.RoleId, 
                opt
                => opt.MapFrom(dto => dto.RoleId));

        // For Dto -> Model & Model -> Dto
        CreateMap<UserRegistrationModel, UserDto>()                        //todo Можно отказаться от некоторых полей
           .ForMember(dto => dto.Id, opt => opt.MapFrom(user => Guid.NewGuid()))
           .ForMember(dto => dto.UserName, opt => opt.MapFrom(user => user.UserName))
           .ForMember(dto => dto.Password, opt => opt.MapFrom(user => user.Password))
           .ForMember(dto => dto.Email, opt => opt.MapFrom(user => user.Email))
           .ForMember(dto => dto.Spam, opt => opt.MapFrom(user => user.Spam));

        CreateMap<UserDto, UserRegistrationModel>()
            .ForMember(user => user.UserName, opt => opt.MapFrom(dto => dto.UserName))
            .ForMember(user => user.Password, opt => opt.MapFrom(dto => dto.Password))
            .ForMember(user => user.Email, opt => opt.MapFrom(dto => dto.Email))
            .ForMember(user => user.Spam, opt => opt.MapFrom(dto => dto.Spam));



        //todo For UserLoginModel -> Dto НАДО ОНО ИЛИ НЕТ ???
        CreateMap<UserLoginModel, UserDto>()
           .ForMember(dto => dto.Password,
               opt =>
                   opt.MapFrom(user => user.Password))
           .ForMember(dto => dto.Email,
               opt =>
                   opt.MapFrom(user => user.Email));

        CreateMap<UserDto, UserEditModel>()
            .ForMember(user => user.Id,
                opt =>
                    opt.MapFrom(dto => dto.Id))
            .ForMember(user => user.UserName,
                opt =>
                    opt.MapFrom(dto => dto.UserName))
            .ForMember(user => user.OldPassword,
                opt =>
                    opt.MapFrom(dto => dto.Password))
            .ForMember(user => user.Spam, opt
                => opt.MapFrom(dto => dto.Spam));

        CreateMap<UserDto, UserDataModel>()
            .ForMember(user => user.RoleName,
                opt =>
                    opt.MapFrom(dto => dto.RoleName));
    }
}
