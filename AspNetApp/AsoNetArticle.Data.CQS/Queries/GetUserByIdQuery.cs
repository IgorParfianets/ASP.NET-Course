using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetUserByIdQuery : IRequest<User?>
    {
        public Guid UserId { get; set; }
    }
}
