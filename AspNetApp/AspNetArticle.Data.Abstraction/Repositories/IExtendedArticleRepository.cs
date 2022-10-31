using AspNetArticle.Database.Entities;

namespace AspNetArticle.Data.Abstractions.Repositories
{
    public interface IExtendedArticleRepository : IRepository<Article>
    {
        Task UpdateArticleTextAsync(Guid id, string text);
    }
}
