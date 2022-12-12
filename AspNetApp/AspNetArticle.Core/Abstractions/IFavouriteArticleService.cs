using AspNetArticle.Core.DataTransferObjects;

namespace AspNetArticle.Core.Abstractions
{
    public interface IFavouriteArticleService
    {
        Task<int> CreateFavouriteArticle(FavouriteArticleDto favouriteArticleDto);
        Task RemoveFavouriteArticle(FavouriteArticleDto favouriteArticleDto);

        Task<bool> CheckFavouriteArticle(Guid userId, Guid articleId);

        Task<List<ArticleDto>> GetAllFavouriteArticles(Guid userId);
    }
}
