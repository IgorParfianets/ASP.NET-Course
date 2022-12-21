using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetCommentsByUserIdAndArticleIdQuery : IRequest<IEnumerable<Comment>>
    {
        public Guid UserId { get; set; }
        public Guid ArticleId { get; set; }
    }
}
