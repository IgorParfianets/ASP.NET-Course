using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database.Entities;
using AutoMapper;

namespace AspNetArticle.MvcApp.MappingProfiles;

public class ViewProfile : Profile
{
    public ViewProfile()
    {
        CreateMap<View, ViewDto>()
            .ForMember(dto => dto.Id, opt => opt.MapFrom(view => view.Id))
            .ForMember(dto => dto.IpAddress, opt => opt.MapFrom(view => view.IpAddress))
            .ForMember(dto => dto.DateOfView, opt => opt.MapFrom(view => view.DateOfView));

        CreateMap<ViewDto, View>()
            .ForMember(view => view.Id, opt => opt.MapFrom(dto => dto.Id))
            .ForMember(view => view.IpAddress, opt => opt.MapFrom(dto => dto.IpAddress))
            .ForMember(view => view.DateOfView, opt => opt.MapFrom(dto => dto.DateOfView));
    }
}
