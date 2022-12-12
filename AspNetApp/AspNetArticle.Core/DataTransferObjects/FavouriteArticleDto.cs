
namespace AspNetArticle.Core.DataTransferObjects
{
    public class FavouriteArticleDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ArticleId { get; set; }
    }
}
