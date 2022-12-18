using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetAllSourcesQuery : IRequest<IEnumerable<Source>>
    {
    }
}
