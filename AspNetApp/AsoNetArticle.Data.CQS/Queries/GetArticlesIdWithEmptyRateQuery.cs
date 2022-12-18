using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetArticlesIdWithEmptyRateQuery : IRequest<IEnumerable<Guid>?>
    {
    }
}
