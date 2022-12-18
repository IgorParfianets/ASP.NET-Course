using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetAllUsersQuery : IRequest<IEnumerable<User>?>
    {
    }
}
