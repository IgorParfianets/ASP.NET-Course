using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsoNetArticle.Data.CQS.Handers.QueryHanders
{
    public class GetSourceByIdQueryHandler : IRequestHandler<GetSourceByIdQuery, Source?>
    {
        private readonly AggregatorContext _context;

        public GetSourceByIdQueryHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<Source?> Handle(GetSourceByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Sources.FirstOrDefaultAsync(source => source.Id.Equals(request.Id));
        }
    }
}
