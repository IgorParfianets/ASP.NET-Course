using AspNetArticle.Core.DataTransferObjects;

namespace AspNetArticle.Core.Abstractions
{
    public interface ICommentaryService
    {
        Task<CommentDto> GetCommentByIdAsync(Guid id);
        Task CreateCommentAsync(CommentDto dto); 
        Task UpdateCommentAsync(CommentDto dto);
        Task<IEnumerable<CommentDto>> GetAllCommentsByUserIdAsync(Guid id);
        Task<IEnumerable<CommentDto>> GelAllCommentsAsync();
        Task<IEnumerable<CommentaryWithUserDto>> GetAllCommentsWithUsersByArticleIdAsync(Guid id);
        Task DeleteCommentById(Guid id);
        Task<IEnumerable<CommentDto>> GetCommentsByUserIdAndArticleId(Guid article, Guid user);
    }
}
