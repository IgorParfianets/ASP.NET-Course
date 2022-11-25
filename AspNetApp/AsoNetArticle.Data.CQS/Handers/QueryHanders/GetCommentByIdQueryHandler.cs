using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    public class GetCommentByIdQueryHandler : IRequestHandler<GetCommentByIdQuery, Comment?>
    {
        private readonly AggregatorContext _context;

        public GetCommentByIdQueryHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<Comment?> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Comments.FirstOrDefaultAsync(com => com.Id.Equals(request.Id), cancellationToken);
        }
    }
}
