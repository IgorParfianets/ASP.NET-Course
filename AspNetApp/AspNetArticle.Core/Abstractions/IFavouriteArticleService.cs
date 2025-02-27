﻿using AspNetArticle.Core.DataTransferObjects;

namespace AspNetArticle.Core.Abstractions
{
    public interface IFavouriteArticleService
    {
        Task CreateFavouriteArticle(FavouriteArticleDto favouriteArticleDto);
        Task RemoveFavouriteArticle(FavouriteArticleDto favouriteArticleDto);
        Task<bool> CheckFavouriteArticle(Guid userId, Guid articleId);
        Task<IEnumerable<ArticleDto>> GetAllFavouriteArticles(Guid userId);
    }
}
