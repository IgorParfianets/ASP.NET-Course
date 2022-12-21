using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    public class GetAllCommentsWithUsersByArticleIdQueryHandler : 
        IRequestHandler<GetAllCommentsWithUsersByArticleIdQuery, IEnumerable<Comment>>
    {
        private readonly AggregatorContext _context;

        public GetAllCommentsWithUsersByArticleIdQueryHandler(AggregatorContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Comment>> Handle(GetAllCommentsWithUsersByArticleIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Comments
                .AsNoTracking()
                .Where(com => com.ArticleId.Equals(request.ArticleId))
                .Include(com => com.User)
                .ToArrayAsync(cancellationToken);
        }
    }
}
