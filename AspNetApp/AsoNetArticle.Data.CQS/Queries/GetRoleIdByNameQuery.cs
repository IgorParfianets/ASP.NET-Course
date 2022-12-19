using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetRoleIdByNameQuery : IRequest<Guid?>
    {
        public string RoleName { get; set; }
    }
}
