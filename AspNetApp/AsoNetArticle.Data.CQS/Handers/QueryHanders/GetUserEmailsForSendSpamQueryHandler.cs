using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    public class GetUserEmailsForSendSpamQueryHandler : IRequestHandler<GetUserEmailsForSendSpamQuery, IEnumerable<string>>
    {
        private readonly AggregatorContext _context;

        public GetUserEmailsForSendSpamQueryHandler(AggregatorContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<string>> Handle(GetUserEmailsForSendSpamQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(user => user.Spam)
                .Select(user => user.Email)
                .ToArrayAsync();
        }
    }
}
