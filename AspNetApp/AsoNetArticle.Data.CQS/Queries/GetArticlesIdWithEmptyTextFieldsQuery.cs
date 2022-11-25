using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetArticlesIdWithEmptyTextFieldsQuery : IRequest<IEnumerable<Guid>?>
    {
        public Guid SourceId { get; set; }
    }
}
