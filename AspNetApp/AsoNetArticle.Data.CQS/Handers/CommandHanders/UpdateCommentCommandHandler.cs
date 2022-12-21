using AsoNetArticle.Data.CQS.Commands;
using AspNetArticle.Database;
using MediatR;

namespace AsoNetArticle.Data.CQS.Handers.CommandHanders
{
    public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, int>
    {
        private readonly AggregatorContext _context;

        public UpdateCommentCommandHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            _context.Comments.Update(request.Comment);
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
