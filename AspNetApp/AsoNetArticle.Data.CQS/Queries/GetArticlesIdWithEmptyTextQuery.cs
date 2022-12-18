using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetArticlesIdWithEmptyTextQuery : IRequest<IEnumerable<Guid>?>
    {
        //public Guid SourceId { get; set; }
    }
}
