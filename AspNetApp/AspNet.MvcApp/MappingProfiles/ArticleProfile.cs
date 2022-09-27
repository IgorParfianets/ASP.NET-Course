using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database.Entities;
using AutoMapper;

namespace AspNetArticle.MvcApp.MappingProfiles;

public class ArticleProfile : Profile //to do     ещё не закончено
{
    public ArticleProfile()
    {
        CreateMap<Article, ArticleDto>()
            .ForMember(dto => dto.Id, opt => opt.MapFrom(article => article.Id))
            .ForMember(dto => dto.Title, opt => opt.MapFrom(article => article.Title))
            .ForMember(dto => dto.ShortDescription, opt => opt.MapFrom(article => article.ShortDescription))
            .ForMember(dto => dto.FullText, opt => opt.MapFrom(article => article.FullText))
            .ForMember(dto => dto.PublicationDate, opt => opt.MapFrom(article => article.PublicationDate))
            .ForMember(dto => dto.Views, opt => opt.MapFrom(article => article.Views))
            .ForMember(dto => dto.Comments, opt => opt.MapFrom(article => article.Comments));

        CreateMap<ArticleDto, Article>()
            .ForMember(article => article.Id, opt => opt.MapFrom(dto => dto.Id))
            .ForMember(article => article.Title, opt => opt.MapFrom(dto => dto.Title))
            .ForMember(article => article.ShortDescription, opt => opt.MapFrom(dto => dto.ShortDescription))
            .ForMember(article => article.FullText, opt => opt.MapFrom(dto => dto.FullText))
            .ForMember(article => article.PublicationDate, opt => opt.MapFrom(dto => dto.PublicationDate))
            .ForMember(article => article.Views, opt => opt.MapFrom(dto => dto.Views))
            .ForMember(article => article.Comments, opt => opt.MapFrom(dto => dto.Comments));

    }
}
