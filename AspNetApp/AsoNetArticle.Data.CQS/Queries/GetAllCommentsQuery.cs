using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetAllCommentsQuery : IRequest<IEnumerable<Comment>>
    {
    }
}
