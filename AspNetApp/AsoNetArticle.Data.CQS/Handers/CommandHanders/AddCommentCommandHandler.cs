using AspNetArticle.Database;
using MediatR;

namespace AsoNetArticle.Data.CQS.Handers.CommandHanders
{
    public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, int>
    {
        public readonly AggregatorContext _context;

        public AddCommentCommandHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            await _context.Comments.AddAsync(request.Comment);
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
