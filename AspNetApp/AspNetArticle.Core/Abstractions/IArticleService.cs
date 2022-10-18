using AspNetArticle.Core.DataTransferObjects;

namespace AspNetArticle.Core.Abstractions;

public interface IArticleService
{
    Task<ArticleDto> GetArticleByIdAsync(Guid id);
    Task<int> CreateArticleAsync(ArticleDto article);
}
