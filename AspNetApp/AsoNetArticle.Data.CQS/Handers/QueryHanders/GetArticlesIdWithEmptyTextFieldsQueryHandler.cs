using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    public class GetArticlesIdWithEmptyTextFieldsQueryHandler : IRequestHandler<GetArticlesIdWithEmptyTextFieldsQuery, IEnumerable<Guid>?>
    {
        private readonly AggregatorContext _context;

        public GetArticlesIdWithEmptyTextFieldsQueryHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Guid>?> Handle(GetArticlesIdWithEmptyTextFieldsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Articles
            .Where(article => article.SourceId.Equals(request.SourceId) && string.IsNullOrEmpty(article.Text))
            .Select(article => article.Id)
            .ToListAsync(cancellationToken);
        }
    }
}
