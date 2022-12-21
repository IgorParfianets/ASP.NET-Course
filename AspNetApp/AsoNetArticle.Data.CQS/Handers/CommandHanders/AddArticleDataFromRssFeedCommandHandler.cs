using AsoNetArticle.Data.CQS.Commands;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.CommandHanders
{
    public class AddArticleDataFromRssFeedCommandHandler : IRequestHandler<AddArticleDataFromRssFeedCommand, Unit>
    {
        private readonly AggregatorContext _context;

        public AddArticleDataFromRssFeedCommandHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(AddArticleDataFromRssFeedCommand request, CancellationToken cancellationToken)
        {
            var oldArticleUrls = await _context.Articles
            .Select(art => art.SourceUrl)
            .ToArrayAsync(cancellationToken);

            var entity = request.Articles
           .Where(art => !oldArticleUrls.Contains(art.SourceUrl))
           .ToArray();

            await _context.Articles.AddRangeAsync(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
