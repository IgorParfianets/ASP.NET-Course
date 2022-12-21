using AspNetArticle.Api.Models.Request;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database.Entities;
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
            .ForMember(dto => dto.RoleId,
                opt =>
                    opt.MapFrom(user => user.RoleId))
            .ForMember(dto => dto.RoleName,
                opt =>
                    opt.MapFrom(user => user.Role.Name));



        CreateMap<UserDto, User>() // FROM dto TO entity
            .ForMember(user => user.Id, 
                opt => 
                    opt.MapFrom(dto => Guid.NewGuid()))
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

        CreateMap<RegisterUserRequestModel, UserDto>()
            .ForMember(user => user.UserName,
                opt =>
                    opt.MapFrom(dto => dto.Username))
            .ForMember(user => user.Email,
                opt =>
                    opt.MapFrom(dto => dto.Email))
            .ForMember(user => user.Password,
                opt =>
                    opt.MapFrom(dto => dto.Password));

        CreateMap<UpdateUserRequestModel, UserDto>();
        //    .ForMember(user => user.Id,
        //        opt =>
        //            opt.MapFrom(dto => dto.Id))
        //    .ForMember(user => user.Email,
        //        opt =>
        //            opt.MapFrom(dto => dto.Email))
        //    .ForMember(user => user.Password,
        //        opt =>
        //            opt.MapFrom(dto => dto.OldPassword))
        //    .ForMember(user => user.Spam,
        //        opt =>
        //            opt.MapFrom(dto => dto.Spam))
        //    .ForMember(user => user.Email,
        //        opt =>
        //            opt.MapFrom(dto => dto.Email))
        //    .ForMember(user => user.Password,
        //        opt =>
        //            opt.MapFrom(dto => dto.OldPassword));
    }
}
