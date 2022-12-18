using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    public class GetArticlesIdWithEmptyTextQueryHandler : IRequestHandler<GetArticlesIdWithEmptyTextQuery, IEnumerable<Guid>?>
    {
        private readonly AggregatorContext _context;

        public GetArticlesIdWithEmptyTextQueryHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Guid>?> Handle(GetArticlesIdWithEmptyTextQuery request, CancellationToken cancellationToken)
        {
            return await _context.Articles
            .Where(article => string.IsNullOrEmpty(article.Text))
            .Select(article => article.Id)
            .ToListAsync(cancellationToken);
        }
    }
}
// article.SourceId.Equals(request.SourceId) &&