using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    public class GetFavouriteArticleByUserIdAndArticleIdQueryHandler :
        IRequestHandler<GetFavouriteArticleByUserIdAndArticleIdQuery, FavouriteArticle?>
    {
        private readonly AggregatorContext _context;

        public GetFavouriteArticleByUserIdAndArticleIdQueryHandler(AggregatorContext context)
        {
            _context = context;
        }
        public async Task<FavouriteArticle?> Handle(GetFavouriteArticleByUserIdAndArticleIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.FavouriteArticles
                .AsNoTracking()
                .FirstOrDefaultAsync(fav => request.UserId.Equals(fav.UserId)
                && request.ArticleId.Equals(fav.ArticleId), cancellationToken);
        }
    }
}
