using AsoNetArticle.Data.CQS.Commands;
using AspNetArticle.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.CommandHanders
{
    public class DeleteCommentByIdCommandHandler : IRequestHandler<DeleteCommentByIdCommand, Unit>
    {
        private readonly AggregatorContext _context;

        public DeleteCommentByIdCommandHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteCommentByIdCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Comments.FirstOrDefaultAsync(com => com.Id.Equals(request.Id));

            if(entity != null)
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
            }              
            return Unit.Value;
        }
    }
}
