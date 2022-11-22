using AspNetArticle.Core.DataTransferObjects;

namespace AspNetArticle.Core.Abstractions;

public interface IArticleService
{
    Task<ArticleDto> GetArticleByIdAsync(Guid id);
    Task<IEnumerable<ArticleDto>> GetAllArticlesAsync();
    Task<List<ArticleDto>> GetArticlesByNameAndSourcesAsync(string? name, Guid? category); // todo remove unnecessary method
    Task<IEnumerable<ArticleDto>> GetArticlesByCategoryAndSearchStringAsync(string category, string searchString);
    Task RemoveArticleToArchiveByIdAsync(Guid id);
    Task<IEnumerable<string>> GetArticlesCategoryAsync();


    Task AggregateArticlesFromExternalSourcesAsync();
    Task AddArticlesDataAsync();

    //Task AddArticleTextAndFixShortDescriptionToArticlesOnlinerAsync();
    //Task AddArticleImageUrlToArticlesOnlinerAsync();
    //Task AddArticleTextToArticlesDevIoAsync();
    

}
