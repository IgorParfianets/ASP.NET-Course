using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Queries;

public class GetUserByRefreshTokenQuery : IRequest<User?>
{
    public Guid RefreshToken { get; set; }
}