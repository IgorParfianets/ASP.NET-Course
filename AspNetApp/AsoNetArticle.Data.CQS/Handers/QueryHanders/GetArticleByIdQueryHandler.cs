using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    public class GetArticleByIdQueryHandler : IRequestHandler<GetArticleByIdQuery, Article?>
    {
        private readonly AggregatorContext _context;

        public GetArticleByIdQueryHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<Article?> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Articles
                .AsNoTracking()
                .FirstOrDefaultAsync(art => art.Id.Equals(request.Id), cancellationToken);
        }
    }
}
