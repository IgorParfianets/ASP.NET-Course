using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetAllCommentsWithUsersByArticleIdQuery : IRequest<IEnumerable<Comment>>
    {
        public Guid ArticleId { get; set; }
    }
}
