using MediatR;

namespace AspNetSample.Data.CQS.Commands;

public class DeleteRefreshTokenCommand : IRequest
{
    public Guid TokenValue;
}