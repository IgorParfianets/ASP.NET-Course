using AspNetArticle.Core.DataTransferObjects;

namespace AspNetArticle.Core.Abstractions;

public interface IArticleService
{
    Task<ArticleDto> GetArticleByIdAsync(Guid id);
    Task<IEnumerable<ArticleDto>> GetAllArticlesAsync();
    Task<List<ArticleDto>> GetArticlesByNameAndSourcesAsync(string? name, Guid? category);

    Task RemoveArticleToArchiveByIdAsync(Guid id);

    Task AggregateArticlesFromExternalSourcesAsync();
    Task AddArticlesDataAsync();

    //Task AddArticleTextAndFixShortDescriptionToArticlesOnlinerAsync();
    //Task AddArticleImageUrlToArticlesOnlinerAsync();
    //Task AddArticleTextToArticlesDevIoAsync();
    

}
