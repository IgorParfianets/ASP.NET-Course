using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetUserByEmailQuery : IRequest<User?>
    {
        public string Email { get; set; }
    }
}
