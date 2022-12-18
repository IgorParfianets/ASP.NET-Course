using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetAllArticlesQueryHandler : IRequestHandler<GetAllArticlesQuery, IQueryable<Article>>
    {
        private readonly AggregatorContext _context;

        public GetAllArticlesQueryHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<Article>> Handle(GetAllArticlesQuery request, CancellationToken cancellationToken)
        {
            return _context.Articles.AsNoTracking();
        }
    }
}
