using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetAllArticlesQueryHandler : IRequestHandler<GetAllArticlesQuery, IEnumerable<Article>>
    {
        private readonly AggregatorContext _context;

        public GetAllArticlesQueryHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Article>> Handle(GetAllArticlesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Articles
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);
        }
    }
}
