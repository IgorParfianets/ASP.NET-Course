using AspNetArticle.Database;
using MediatR;

namespace AsoNetArticle.Data.CQS.Handers.CommandHanders
{
    public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, Unit>
    {
        public readonly AggregatorContext _context;

        public AddCommentCommandHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            await _context.Comments.AddAsync(request.Comment);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
