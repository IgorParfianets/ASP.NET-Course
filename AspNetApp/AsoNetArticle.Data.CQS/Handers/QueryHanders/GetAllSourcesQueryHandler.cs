using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    public class GetAllSourcesQueryHandler : IRequestHandler<GetAllSourcesQuery, IEnumerable<Source>>
    {
        private readonly AggregatorContext _context;

        public GetAllSourcesQueryHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Source>> Handle(GetAllSourcesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Sources
                .AsNoTracking()
                .ToArrayAsync();
        }
    }
}
