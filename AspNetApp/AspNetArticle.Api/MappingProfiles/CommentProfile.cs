using AspNetArticle.Api.Models.Request;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database.Entities;
using AutoMapper;

namespace AspNetArticle.MvcApp.MappingProfiles;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, CommentDto>()
            .ForMember(dto => dto.Id, opt => opt.MapFrom(comment => comment.Id))
            .ForMember(dto => dto.Description, opt => opt.MapFrom(comment => comment.Description))
            .ForMember(dto => dto.PublicationDate, opt => opt.MapFrom(comment => comment.PublicationDate));

        CreateMap<CommentDto, Comment>()
            .ForMember(comment => comment.Id, opt => opt.MapFrom(dto => dto.Id))
            .ForMember(comment => comment.Description, opt => opt.MapFrom(dto => dto.Description))
            .ForMember(comment => comment.PublicationDate, opt => opt.MapFrom(dto => dto.PublicationDate));

        CreateMap<AddCommentRequestModel, CommentDto>()
            .ForMember(comment => comment.Id, opt => opt.MapFrom(dto => Guid.NewGuid()))
            .ForMember(comment => comment.Description, opt => opt.MapFrom(dto => dto.Description))
            .ForMember(comment => comment.PublicationDate, opt => opt.MapFrom(dto => DateTime.Now))
            .ForMember(comment => comment.ArticleId, opt => opt.MapFrom(dto => dto.ArticleId));

        CreateMap<UpdateCommentRequestModel, CommentDto>()
             .ForMember(comment => comment.PublicationDate, opt => opt.MapFrom(dto => DateTime.Now))
             .ForMember(comment => comment.IsEdited, opt => opt.MapFrom(dto => true));
    }
}
