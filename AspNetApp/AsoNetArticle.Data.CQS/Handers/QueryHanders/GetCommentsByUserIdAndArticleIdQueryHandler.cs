using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    public class GetCommentsByUserIdAndArticleIdQueryHandler : 
        IRequestHandler<GetCommentsByUserIdAndArticleIdQuery, IEnumerable<Comment>>
    {
        private readonly AggregatorContext _context;

        public GetCommentsByUserIdAndArticleIdQueryHandler(AggregatorContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Comment>> Handle(GetCommentsByUserIdAndArticleIdQuery request, CancellationToken cancellationToken)
        {
            var entities = _context.Comments.AsQueryable();

            if (!Guid.Empty.Equals(request.ArticleId))
            {
                entities = entities.Where(ent => ent.ArticleId.Equals(request.ArticleId));
            }

            if (!Guid.Empty.Equals(request.UserId))
            {
                entities = entities.Where(ent => ent.UserId.Equals(request.UserId));
            }

            return await entities.ToArrayAsync();
        }
    }
}
