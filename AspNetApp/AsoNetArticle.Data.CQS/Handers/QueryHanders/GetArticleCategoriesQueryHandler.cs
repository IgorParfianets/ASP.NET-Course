using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    public class GetArticleCategoriesQueryHandler : IRequestHandler<GetArticleCategoriesQuery, IEnumerable<string?>>
    {
        private readonly AggregatorContext _context;

        public GetArticleCategoriesQueryHandler(AggregatorContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<string?>> Handle(GetArticleCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Articles
                .AsNoTracking()
                .Where(art => art.PublicationDate.AddDays(7) >= DateTime.UtcNow)
                .Select(art => art.Category)
                .Distinct()
                .ToListAsync(cancellationToken);
        }
    }
}
