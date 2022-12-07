using AspNetArticle.Core.DataTransferObjects;

namespace AspNetArticle.Core.Abstractions;

public interface IArticleService
{
    Task<ArticleDto> GetArticleByIdAsync(Guid id); // +
    Task<IEnumerable<ArticleDto>> GetAllArticlesAsync(); // +
    Task<List<ArticleDto>> GetArticlesByNameAndSourcesAsync(string? name, Guid? category); // todo remove unnecessary method
    Task<IEnumerable<ArticleDto>> GetFilteredArticles(string category, Raiting raiting, string searchString); // Home/Index
    Task RemoveArticleToArchiveByIdAsync(Guid id); // unnecessary because no have archive
    Task<IEnumerable<string>> GetArticlesCategoryAsync(); // +
    Task<Guid?> GetArticleIdByCommentId(Guid commentId); // +

    Task AggregateArticlesFromExternalSourcesAsync();
    Task AddArticlesDataAsync();
}
