using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    public class GetAllCommentsQueryHandler : IRequestHandler<GetAllCommentsQuery, IEnumerable<Comment>>
    {
        private readonly AggregatorContext _context;

        public GetAllCommentsQueryHandler(AggregatorContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Comment>> Handle(GetAllCommentsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Comments
                .AsNoTracking()
                .Include(com => com.User)
                .Include(com => com.Article)
                .ToArrayAsync(cancellationToken);
        }
    }
}
