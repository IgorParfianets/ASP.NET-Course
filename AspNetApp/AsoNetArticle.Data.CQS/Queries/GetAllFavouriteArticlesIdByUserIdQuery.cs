using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetAllFavouriteArticlesIdByUserIdQuery : IRequest<IEnumerable<Guid>>
    {
        public Guid UserId { get; set; }
    }
}
