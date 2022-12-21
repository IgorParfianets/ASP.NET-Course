using AspNetArticle.Core.DataTransferObjects;

namespace AspNetArticle.Core.Abstractions;

public interface IArticleService
{
    Task<ArticleDto> GetArticleByIdAsync(Guid id); 
    Task<IEnumerable<ArticleDto>> GetAllArticlesAsync();
    Task<IEnumerable<ArticleDto>> GetFilteredArticles(string category, Raiting raiting, string searchString); 
    Task<IEnumerable<string>> GetArticlesCategoryAsync(); 
    Task<Guid?> GetArticleIdByCommentId(Guid commentId);
    Task AggregateArticlesFromExternalSourcesAsync();
    Task AddArticlesDataAsync();
}
