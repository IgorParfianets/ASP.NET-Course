using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public  class GetUserEmailsForSendSpamQuery : IRequest<IEnumerable<string>>
    {
    }
}
