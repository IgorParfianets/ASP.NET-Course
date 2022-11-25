using AsoNetArticle.Data.CQS.Commands;
using AspNetArticle.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.CommandHanders
{
    public class UpdateArticleTextAndShortDescriptionOnlinerCommandHandler : 
        IRequestHandler<UpdateArticleTextAndShortDescriptionOnlinerCommand, Unit>
    {
        private readonly AggregatorContext _context;

        public UpdateArticleTextAndShortDescriptionOnlinerCommandHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateArticleTextAndShortDescriptionOnlinerCommand request, CancellationToken cancellationToken)
        {
            var article = await _context.Articles.FirstOrDefaultAsync(a => a.Id.Equals(request.ArticleId), cancellationToken);

            if (article != null)
            {
                article.Text = request.Text;
                article.ShortDescription = request.ShortDescription;
            }

            _context.SaveChanges();
            return Unit.Value;
        }
    }
}
