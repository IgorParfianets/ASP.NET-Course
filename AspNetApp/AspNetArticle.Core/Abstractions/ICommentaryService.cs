using AspNetArticle.Core.DataTransferObjects;

namespace AspNetArticle.Core.Abstractions
{
    public interface ICommentaryService
    {
        Task<CommentDto> GetCommentByIdAsync(Guid id);
        Task<int> CreateCommentAsync(CommentDto dto); 
        Task<int> UpdateCommentAsync(CommentDto dto);
        Task<IEnumerable<CommentDto>> GetAllCommentsByUserIdAsync(Guid id);
        Task<IEnumerable<CommentaryWithUserDto>> GetAllCommentsWithUsersByArticleIdAsync(Guid id);
        Task DeleteCommentById(Guid id);

        //specific methods
        Task<IEnumerable<CommentDto>> GetCommentsByUserIdAndArticleIdTask(Guid? article, Guid? user);
    }
}
