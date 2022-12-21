using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, User?>
    {
        private readonly AggregatorContext _context;

        public GetUserByEmailQueryHandler(AggregatorContext context)
        {
            _context = context;
        }
        public async Task<User?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(user => request.Email.Equals(user.Email))
                .Include(user => user.Role)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
