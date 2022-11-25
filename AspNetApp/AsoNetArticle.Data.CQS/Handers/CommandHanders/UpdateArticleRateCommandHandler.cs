using AsoNetArticle.Data.CQS.Commands;
using AspNetArticle.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.CommandHanders
{
    public class UpdateArticleRateCommandHandler : IRequestHandler<UpdateArticleRateCommand, int>
    {
        private readonly AggregatorContext _context;

        public UpdateArticleRateCommandHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(UpdateArticleRateCommand request, CancellationToken cancellationToken)
        {
            var article = await _context.Articles.FirstOrDefaultAsync(a => a.Id.Equals(request.ArticleId));

            if (article != null)
            {
                article.Rate = request.Rate;
            }
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
