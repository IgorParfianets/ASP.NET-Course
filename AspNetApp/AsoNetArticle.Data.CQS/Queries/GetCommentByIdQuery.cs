using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetCommentByIdQuery : IRequest<Comment?>
    {
        public Guid Id { get; set; }
    }
}
