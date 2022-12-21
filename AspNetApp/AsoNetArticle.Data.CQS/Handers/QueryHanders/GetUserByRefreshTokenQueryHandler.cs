using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders;

public class GetUserByRefreshTokenQueryHandler : IRequestHandler<GetUserByRefreshTokenQuery, User?>
{
    private readonly AggregatorContext _context;

    public GetUserByRefreshTokenQueryHandler(AggregatorContext context)
    {
        _context = context;
    }

    public async Task<User?> Handle(GetUserByRefreshTokenQuery request,
        CancellationToken cancellationToken)
    {
        var user = (await _context.RefreshTokens
            .Include(token => token.User)
            .ThenInclude(user => user.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(token => token.Token.Equals(request.RefreshToken),
                cancellationToken))?.User;

        return user;
    }
}