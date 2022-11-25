
using AsoNetArticle.Data.CQS.Commands;
using AspNetArticle.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.CommandHanders
{
    public class UpdateArticleImageUrlOnlinerCommandHandler : IRequestHandler<UpdateArticleImageUrlOnlinerCommand, Unit>
    {
        private readonly AggregatorContext _context;

        public UpdateArticleImageUrlOnlinerCommandHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateArticleImageUrlOnlinerCommand request, CancellationToken cancellationToken)
        {
            var article = await _context.Articles.FirstOrDefaultAsync(a => a.Id.Equals(request.ArticleId), cancellationToken);

            if (article != null)
            {
                article.ImageUrl = request.ImageUrl;
            }

            _context.SaveChanges();
            return Unit.Value;
        }
    }
}
