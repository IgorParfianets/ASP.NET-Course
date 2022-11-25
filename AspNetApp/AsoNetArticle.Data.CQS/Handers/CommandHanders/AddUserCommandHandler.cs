
using AsoNetArticle.Data.CQS.Commands;
using AspNetArticle.Database;
using MediatR;

namespace AsoNetArticle.Data.CQS.Handers.CommandHanders
{
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, int>
    {
        private readonly AggregatorContext _context;

        public AddUserCommandHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(request.User);
            return await _context.SaveChangesAsync();
        }
    }
}
