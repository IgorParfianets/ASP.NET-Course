using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetArticleIdByCommentIdQuery : IRequest<Guid?>
    {
        public Guid CommentId { get; set; }
    }
}
