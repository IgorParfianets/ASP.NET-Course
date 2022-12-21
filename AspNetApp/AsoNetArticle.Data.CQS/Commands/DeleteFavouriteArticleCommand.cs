using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Commands
{
    public class DeleteFavouriteArticleCommand : IRequest
    {
        public FavouriteArticle FavouriteArticle { get; set; }
    }
}
