using AspNetArticle.Core.DataTransferObjects;

namespace AspNetArticle.Core.Abstractions;

public interface IArticleService
{
    Task<ArticleDto> GetArticleByIdAsync(Guid id);
    Task<IEnumerable<ArticleDto>> GetAllArticlesAsync();
    Task<List<ArticleDto>> GetArticlesByNameAndSourcesAsync(string? name, Guid? category);

    Task RemoveArticleByIdSourceAsync(Guid id);


    Task GetAllArticleDataFromOnlinerRssAsync(Guid sourceId, string? sourceRssUrl);
    Task GetAllArticleDataFromDevIoRssAsync(Guid soirceId, string? sourceRssUrl);



    Task AddArticleTextAndFixShortDescriptionToArticlesOnlinerAsync();
    Task AddArticleImageUrlToArticlesOnlinerAsync();
    Task AddArticleTextToArticlesDevIoAsync();
    

}
