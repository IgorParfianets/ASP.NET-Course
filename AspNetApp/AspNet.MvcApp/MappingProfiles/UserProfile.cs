using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database.Entities;
using AutoMapper;

namespace AspNetArticle.MvcApp.MappingProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dto => dto.Id, opt => opt.MapFrom(user => user.Id))
            .ForMember(dto => dto.UserName, opt => opt.MapFrom(user => user.UserName))
            .ForMember(dto => dto.Password, opt => opt.MapFrom(user => user.Password))
            .ForMember(dto => dto.Email, opt => opt.MapFrom(user => user.Email))
            .ForMember(dto => dto.AccountCreated, opt => opt.MapFrom(user => user.AccountCreated))
            .ForMember(dto => dto.LastVisit, opt => opt.MapFrom(user => user.LastVisit))
            .ForMember(dto => dto.Spam, opt => opt.MapFrom(user => user.Spam))
            .ForMember(dto => dto.Comments, opt => opt.MapFrom(user => user.Comments));

        CreateMap<UserDto, User>()
            .ForMember(user => user.Id, opt => opt.MapFrom(dto => dto.Id))
            .ForMember(user => user.UserName, opt => opt.MapFrom(dto => dto.UserName))
            .ForMember(user => user.Password, opt => opt.MapFrom(dto => dto.Password))
            .ForMember(user => user.Email, opt => opt.MapFrom(dto => dto.Email))
            .ForMember(user => user.AccountCreated, opt => opt.MapFrom(dto => dto.AccountCreated))
            .ForMember(dto => dto.LastVisit, opt => opt.MapFrom(user => user.LastVisit))
            .ForMember(dto => dto.Spam, opt => opt.MapFrom(user => user.Spam))
            .ForMember(dto => dto.Comments, opt => opt.MapFrom(user => user.Comments));
    }
}
