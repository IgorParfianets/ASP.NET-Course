using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    public class GetArticlesIdWithEmptyRateQueryHandler : IRequestHandler<GetArticlesIdWithEmptyRateQuery, IEnumerable<Guid>?>
    {
        private readonly AggregatorContext _context;

        public GetArticlesIdWithEmptyRateQueryHandler(AggregatorContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Guid>?> Handle(GetArticlesIdWithEmptyRateQuery request, CancellationToken cancellationToken)
        {
            return await _context.Articles.Where(art => art.Rate == null && !string.IsNullOrEmpty(art.Text))
                    .Select(art => art.Id)
                    .ToListAsync(cancellationToken);
        }
    }
}
