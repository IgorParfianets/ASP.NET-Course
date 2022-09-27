using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database.Entities;
using AutoMapper;

namespace AspNetArticle.MvcApp.MappingProfiles;

public class SourceProfile : Profile
{
    public SourceProfile()
    {
        CreateMap<Source, SourceDto>()
            .ForMember(dto => dto.Id, opt => opt.MapFrom(source => source.Id))
            .ForMember(dto => dto.Name, opt => opt.MapFrom(source => source.Name))
            .ForMember(dto => dto.SourceType, opt => opt.MapFrom(source => source.SourceType))
            .ForMember(dto => dto.Url, opt => opt.MapFrom(source => source.Url))
            .ForMember(dto => dto.Articles, opt => opt.MapFrom(source => source.Articles));

        CreateMap<SourceDto, Source>()
            .ForMember(source => source.Id, opt => opt.MapFrom(dto => dto.Id))
            .ForMember(source => source.Name, opt => opt.MapFrom(dto => dto.Name))
            .ForMember(source => source.SourceType, opt => opt.MapFrom(dto => dto.SourceType))
            .ForMember(source => source.Url, opt => opt.MapFrom(dto => dto.Url))
            .ForMember(source => source.Articles, opt => opt.MapFrom(dto => dto.Articles));
    }
}
