using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    public class GetArticleIdByCommentIdQueryHandler : IRequestHandler<GetArticleIdByCommentIdQuery, Guid?>
    {
        private readonly AggregatorContext _context;

        public GetArticleIdByCommentIdQueryHandler(AggregatorContext context)
        {
            _context = context;
        }
        public async Task<Guid?> Handle(GetArticleIdByCommentIdQuery request, CancellationToken cancellationToken)
        {
            return (await _context.Comments
                .AsNoTracking()
                .FirstOrDefaultAsync(com => com.Id.Equals(request.CommentId), cancellationToken))
                ?.ArticleId;
        }
    }
}
