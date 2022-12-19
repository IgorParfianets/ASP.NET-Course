using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetFavouriteArticleByUserIdAndArticleIdQuery : IRequest<FavouriteArticle?>
    {
        public Guid UserId { get; set; }
        public Guid ArticleId { get; set; }
    }
}
