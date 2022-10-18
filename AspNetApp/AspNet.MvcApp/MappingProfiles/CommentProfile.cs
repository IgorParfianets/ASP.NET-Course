using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database.Entities;
using AutoMapper;

namespace AspNetArticle.MvcApp.MappingProfiles;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        // For Entity -> Dto & Dto -> Entity
        CreateMap<Comment, CommentDto>()
            .ForMember(dto => dto.Id, opt => opt.MapFrom(comment => comment.Id))
            .ForMember(dto => dto.Description, opt => opt.MapFrom(comment => comment.Description))
            .ForMember(dto => dto.PublicationDate, opt => opt.MapFrom(comment => comment.PublicationDate));

        CreateMap<CommentDto, Comment>()
            .ForMember(comment => comment.Id, opt => opt.MapFrom(dto => dto.Id))
            .ForMember(comment => comment.Description, opt => opt.MapFrom(dto => dto.Description))
            .ForMember(comment => comment.PublicationDate, opt => opt.MapFrom(dto => dto.PublicationDate));
    }
}
