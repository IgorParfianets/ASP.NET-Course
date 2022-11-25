using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetArticleCategoriesQuery : IRequest<IEnumerable<string?>>
    {
    }
}
