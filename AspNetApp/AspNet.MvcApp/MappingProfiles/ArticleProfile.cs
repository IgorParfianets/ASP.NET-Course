using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database.Entities;
using AspNetArticle.MvcApp.Models;
using AutoMapper;

namespace AspNetArticle.MvcApp.MappingProfiles;

//public static class Mapping
//{
//    private static readonly Lazy<IMapper> Lazy = new Lazy<IMapper>(() =>
//    {
//        var config = new MapperConfiguration(cfg => {
//            // This line ensures that internal properties are also mapped over.
//            cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
//            cfg.AddProfile<ArticleProfile>();
//            cfg.AddProfile<CommentProfile>();
//            cfg.AddProfile<SourceProfile>();
//            cfg.AddProfile<UserProfile>();
//            cfg.AddProfile<ViewProfile>();
//        });
//        var mapper = config.CreateMapper();
//        return mapper;
//    });

//    public static IMapper Mapper => Lazy.Value;
//}
public class ArticleProfile : Profile //to do     ещё не закончено
{
    public ArticleProfile()
    {
        // For Entity -> Dto & Dto -> Entity
        CreateMap<Article, ArticleDto>()
            .ForMember(dto => dto.Id, opt => opt.MapFrom(article => article.Id))
            .ForMember(dto => dto.Title, opt => opt.MapFrom(article => article.Title))
            .ForMember(dto => dto.ShortDescription, opt => opt.MapFrom(article => article.ShortDescription))
            .ForMember(dto => dto.Text, opt => opt.MapFrom(article => article.Text))
            .ForMember(dto => dto.PublicationDate, opt => opt.MapFrom(article => article.PublicationDate))
            .ForMember(dto => dto.Views, opt => opt.MapFrom(article => article.Views))
            .ForMember(dto => dto.Comments, opt => opt.MapFrom(article => article.Comments));

        CreateMap<ArticleDto, Article>()
            .ForMember(article => article.Id, opt => opt.MapFrom(dto => dto.Id))
            .ForMember(article => article.Title, opt => opt.MapFrom(dto => dto.Title))
            .ForMember(article => article.ShortDescription, opt => opt.MapFrom(dto => dto.ShortDescription))
            .ForMember(article => article.Text, opt => opt.MapFrom(dto => dto.Text))
            .ForMember(article => article.PublicationDate, opt => opt.MapFrom(dto => dto.PublicationDate))
            .ForMember(article => article.Views, opt => opt.MapFrom(dto => dto.Views))
            .ForMember(article => article.Comments, opt => opt.MapFrom(dto => dto.Comments));

        // For ArticleDto -> ArticleModel 

        CreateMap<ArticleDto, ArticleModel>()
            .ForMember(article => article.Id, opt => opt.MapFrom(dto => dto.Id))
            .ForMember(article => article.Title, opt => opt.MapFrom(dto => dto.Title))
            .ForMember(article => article.ShortDescription, opt => opt.MapFrom(dto => dto.ShortDescription))
            .ForMember(article => article.Text, opt => opt.MapFrom(dto => dto.Text))
            .ForMember(article => article.PublicationDate, opt => opt.MapFrom(dto => dto.PublicationDate));

        CreateMap<ArticleModel,ArticleDto>()
            .ForMember(article => article.Id, opt => opt.MapFrom(dto => dto.Id))
            .ForMember(article => article.Title, opt => opt.MapFrom(dto => dto.Title))
            .ForMember(article => article.ShortDescription, opt => opt.MapFrom(dto => dto.ShortDescription))
            .ForMember(article => article.Text, opt => opt.MapFrom(dto => dto.Text))
            .ForMember(article => article.PublicationDate, opt => opt.MapFrom(dto => DateTime.Now));
    }
}
