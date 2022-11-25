using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Commands
{
    public class AddUserCommand : IRequest<int>
    {
        public User User { get; set; }
    }
}
