using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetUserByUsernameQuery : IRequest<User?>
    {
        public string Username { get; set; }
    }
}
