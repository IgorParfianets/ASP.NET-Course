using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    public class GetRoleIdByNameQueryHandler : IRequestHandler<GetRoleIdByNameQuery, Guid?>
    {
        private readonly AggregatorContext _context;
        public GetRoleIdByNameQueryHandler(AggregatorContext context)
        {
            _context = context;
        }
        public async Task<Guid?> Handle(GetRoleIdByNameQuery request, CancellationToken cancellationToken)
        {
            return (await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(role => request.RoleName.Equals(role.Name), cancellationToken))?.Id;
        }
    }
}
////var role = await _unitOfWork.Roles
//    .FindBy(role => 
//        role.Name.Equals(name))
//    .FirstOrDefaultAsync();