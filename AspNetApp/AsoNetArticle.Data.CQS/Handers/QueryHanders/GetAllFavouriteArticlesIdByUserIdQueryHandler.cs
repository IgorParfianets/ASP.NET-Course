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
                .Where(art => request.UserId.Equals(art.UserId))
                .Select(art => art.ArticleId)
                .ToArrayAsync();
        }
    }
}
