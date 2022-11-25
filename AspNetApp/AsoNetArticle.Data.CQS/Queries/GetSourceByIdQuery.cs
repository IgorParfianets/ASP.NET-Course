using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetSourceByIdQuery : IRequest<Source?>
    {
        public Guid Id { get; set; }
    }
}
