using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return (await _context.Roles.FirstOrDefaultAsync(role => request.RoleName.Equals(role.Name)))?.Id;
        }
    }
}
