using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    internal class GetFavouriteArticleByUserIdAndArticleIdQueryHandler :
        IRequestHandler<GetFavouriteArticleByUserIdAndArticleIdQuery, FavouriteArticle?>
    {
        private readonly AggregatorContext _context;

        public GetFavouriteArticleByUserIdAndArticleIdQueryHandler(AggregatorContext context)
        {
            _context = context;
        }
        public async Task<FavouriteArticle?> Handle(GetFavouriteArticleByUserIdAndArticleIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.FavouriteArticles.FirstOrDefaultAsync(fav => request.UserId.Equals(fav.UserId)
                && request.ArticleId.Equals(fav.ArticleId));
        }
    }
}
