using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    public class GetUserByUsernameQueryHandler : IRequestHandler<GetUserByUsernameQuery, User?>
    {
        private readonly AggregatorContext _context;

        public GetUserByUsernameQueryHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<User?> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(user => request.Username.Equals(user.UserName));
        }
    }
}
