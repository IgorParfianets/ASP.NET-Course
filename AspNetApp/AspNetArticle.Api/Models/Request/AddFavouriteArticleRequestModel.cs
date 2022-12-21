namespace AspNetArticle.Api.Models.Request
{
    /// <summary>
    /// Request model for adding favourite article
    /// </summary>
    public class AddFavouriteArticleRequestModel
    {
        public bool Answer { get; set; }
        public Guid ArticleId { get; set; }
    }
}
