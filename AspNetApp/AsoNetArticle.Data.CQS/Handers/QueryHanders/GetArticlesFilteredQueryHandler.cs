using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Core;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    public class GetArticlesFilteredQueryHandler : 
        IRequestHandler<GetArticlesFilteredQuery, IEnumerable<Article>>
    {
        private readonly AggregatorContext _context;

        public GetArticlesFilteredQueryHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Article>> Handle(GetArticlesFilteredQuery request, CancellationToken cancellationToken)
        {
            var articles = _context.Articles
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.SearchString))
            {
                articles = articles.Where(art => art.Title.Contains(request.SearchString));
            }

            if (!string.IsNullOrEmpty(request.SelectedCategory) && articles.Any(art => art.Category != null))
            {
                articles = articles.Where(art => art.Category.Equals(request.SelectedCategory));
            }

            if (!request.SelectedRaiting.Equals(Raiting.None))
            {
                if (request.SelectedRaiting.Equals(Raiting.TopRaiting))
                {
                    articles = articles.Where(art => art.Rate >= 0).OrderByDescending(art => art.Rate);
                }
                else
                {
                    articles = articles.Where(art => art.Rate < 0).OrderBy(art => art.Rate);
                }
            }
            return await articles.ToArrayAsync(cancellationToken);
        }
    }
}