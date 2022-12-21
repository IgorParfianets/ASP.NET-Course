using AspNetArticle.Database;
using AspNetSample.Data.CQS.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AspNetSample.Data.CQS.Handlers.CommandHandlers;

public class DeleteRefreshTokenCommandHandler
    : IRequestHandler<DeleteRefreshTokenCommand, Unit>
{
    private readonly AggregatorContext _context;

    public DeleteRefreshTokenCommandHandler(AggregatorContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteRefreshTokenCommand command, CancellationToken token)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => command.TokenValue.Equals(rt.Token),
                token);
        _context.RefreshTokens.Remove(refreshToken);
        await _context.SaveChangesAsync(token);
        return Unit.Value;
    }
}