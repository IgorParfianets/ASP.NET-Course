using AspNetArticle.Core.DataTransferObjects;

namespace AspNetArticle.Core.Abstractions
{
    public interface ICommentaryService
    {
        Task<CommentDto> GetCommentByIdAsync(Guid id);
        Task<int> CreateCommentAsync(CommentDto dto); 
        Task<int> UpdateCommentAsync(CommentDto dto);
        Task<IEnumerable<CommentDto>> GetAllCommentsByUserIdAsync(Guid id);
        Task<IEnumerable<CommentDto>> GetAllCommentsByArticleIdAsync(Guid id);
        Task DeleteCommentById(Guid id);

        //specific methods
        Task<IEnumerable<CommentDto>> GetCommentsByUserIdAndArticleName(Guid? article, Guid? user);
    }
}
