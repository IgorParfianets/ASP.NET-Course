using AsoNetArticle.Data.CQS.Commands;
using AspNetArticle.Database;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsoNetArticle.Data.CQS.Handers.CommandHanders
{
    public class AddFavouriteArticleCommandHandler : IRequestHandler<AddFavouriteArticleCommand, Unit>
    {
        private readonly AggregatorContext _context;

        public AddFavouriteArticleCommandHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(AddFavouriteArticleCommand request, CancellationToken cancellationToken)
        {
            await _context.FavouriteArticles.AddAsync(request.FavouriteArticle);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
