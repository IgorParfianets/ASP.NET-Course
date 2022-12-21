using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database.Entities;
using AutoMapper;

namespace AspNetArticle.MvcApp.MappingProfiles
{
    public class FavouriteArticleProfile : Profile
    {
        public FavouriteArticleProfile()
        {
            CreateMap<FavouriteArticleDto, FavouriteArticle>()
                .ForMember(entity => entity.Id, opt => opt.MapFrom(dto => dto.Id))
                .ForMember(entity => entity.UserId, opt => opt.MapFrom(dto => dto.UserId))
                .ForMember(entity => entity.ArticleId, opt => opt.MapFrom(dto => dto.ArticleId));
        }
    }
}
