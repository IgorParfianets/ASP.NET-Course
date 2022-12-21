using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    public class GetAllCommentsByUserIdQueryHandler : IRequestHandler<GetAllCommentsByUserIdQuery, IEnumerable<Comment>>
    {
        private readonly AggregatorContext _context;

        public GetAllCommentsByUserIdQueryHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comment>> Handle(GetAllCommentsByUserIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Comments
                .AsNoTracking()
                .Where(com => com.UserId.Equals(request.UserId))
                .Include(com => com.Article)
                .ToArrayAsync(cancellationToken);
        }
    }
}
