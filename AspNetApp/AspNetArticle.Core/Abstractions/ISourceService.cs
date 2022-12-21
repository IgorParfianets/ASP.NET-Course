using AspNetArticle.Core.DataTransferObjects;

namespace AspNetArticle.Core.Abstractions
{
    public interface ISourceService
    {
        Task<IEnumerable<SourceDto>> GetSourcesAsync();
        Task<SourceDto> GetSourcesByIdAsync(Guid id);
        Task RemoveSourceById(Guid id);
    }
}
