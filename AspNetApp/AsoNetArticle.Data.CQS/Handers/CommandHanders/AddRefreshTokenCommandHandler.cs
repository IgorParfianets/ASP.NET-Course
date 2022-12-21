using AspBetSample.DataBase.Entities;
using AspNetArticle.Database;
using MediatR;
using AspNetSample.Data.CQS.Commands;

namespace AspNetSample.Data.CQS.Handlers.CommandHandlers;

public class AddRefreshTokenCommandHandler
    : IRequestHandler<AddRefreshTokenCommand, Unit>
{
    private readonly AggregatorContext _context;

    public AddRefreshTokenCommandHandler(AggregatorContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(AddRefreshTokenCommand command, CancellationToken token)
    {
        var rt = new RefreshToken()
        {
            Id = Guid.NewGuid(),
            Token = command.TokenValue,
            UserId = command.UserId
        };
        await _context.RefreshTokens.AddAsync(rt, token);
        await _context.SaveChangesAsync(token);
        return Unit.Value;
    }
}