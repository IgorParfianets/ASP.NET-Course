using AsoNetArticle.Data.CQS.Commands;
using AspNetArticle.Database;
using MediatR;

namespace AsoNetArticle.Data.CQS.Handers.CommandHanders
{
    public class DeleteFavouriteArticleCommandHandler : IRequestHandler<DeleteFavouriteArticleCommand, Unit>
    {
        private readonly AggregatorContext _context;

        public DeleteFavouriteArticleCommandHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteFavouriteArticleCommand request, CancellationToken cancellationToken)
        {
            if (request.FavouriteArticle != null)
            {
                _context.Remove(request.FavouriteArticle);
                await _context.SaveChangesAsync();
            }
            return Unit.Value;
        }
    }
}
