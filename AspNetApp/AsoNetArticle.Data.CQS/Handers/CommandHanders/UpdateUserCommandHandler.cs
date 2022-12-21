using AsoNetArticle.Data.CQS.Commands;
using AspNetArticle.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.CommandHanders
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, int>
    {
        private readonly AggregatorContext _context;

        public UpdateUserCommandHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Users.FirstOrDefaultAsync(entity => entity.Id.Equals(request.UserId));

            var nameValuePropertiesPairs = request.PatchData
                .ToDictionary(
                patchModel => patchModel.PropertyName,
                patchModel => patchModel.PropertyValue);

            var dbEntityEntry = _context.Entry(entity);
            dbEntityEntry.CurrentValues.SetValues(nameValuePropertiesPairs);
            dbEntityEntry.State = EntityState.Modified;

            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
