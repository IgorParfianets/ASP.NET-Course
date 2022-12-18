using AspNetArticle.Database.Entities;

namespace AspNetArticle.Data.Abstractions.Repositories
{
    public interface IExtendedArticleRepository : IRepository<Article>
    {
        Task UpdateArticleTextAsync(Guid id, string text);
        Task UpdateArticleImageUrlAsync(Guid id, string imageUrl);
        //Task UpdateArticleShortDescriptionAsync(Guid id, string shortDescription);
        Task UpdateArticleRateAsync(Guid id, double rate);
    }
}
