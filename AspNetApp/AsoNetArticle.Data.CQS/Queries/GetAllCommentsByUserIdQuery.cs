using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetAllCommentsByUserIdQuery : IRequest<IEnumerable<Comment>>
    {
        public Guid UserId { get; set; }
    }
}
