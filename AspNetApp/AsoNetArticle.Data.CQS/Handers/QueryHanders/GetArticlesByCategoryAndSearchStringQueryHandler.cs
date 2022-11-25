using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    public class GetArticlesByCategoryAndSearchStringQueryHandler : 
        IRequestHandler<GetArticlesByCategoryAndSearchStringQuery, IEnumerable<Article>>
    {
        private readonly AggregatorContext _context;

        public GetArticlesByCategoryAndSearchStringQueryHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Article>> Handle(GetArticlesByCategoryAndSearchStringQuery request, CancellationToken cancellationToken)
        {
            var articles = _context.Articles
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.SearchString))
            {
                articles = articles.Where(art => art.Title.Contains(request.SearchString));
            }

            if (!string.IsNullOrEmpty(request.Category))
            {
                articles = articles.Where(art => art.Category.Equals(request.Category)); 
            }

            return await articles.ToListAsync(cancellationToken);
        }
    }
}