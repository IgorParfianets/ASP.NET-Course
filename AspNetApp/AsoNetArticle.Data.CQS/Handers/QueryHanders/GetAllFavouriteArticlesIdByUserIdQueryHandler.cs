using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    public class GetAllFavouriteArticlesIdByUserIdQueryHandler :
        IRequestHandler<GetAllFavouriteArticlesIdByUserIdQuery, IEnumerable<Guid>>
    {
        private readonly AggregatorContext _context;

        public GetAllFavouriteArticlesIdByUserIdQueryHandler(AggregatorContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Guid>> Handle(GetAllFavouriteArticlesIdByUserIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.FavouriteArticles
                .AsNoTracking()
                .Where(art => request.UserId.Equals(art.UserId))
                .Select(art => art.ArticleId)
                .ToArrayAsync(cancellationToken);
        }
    }
}
