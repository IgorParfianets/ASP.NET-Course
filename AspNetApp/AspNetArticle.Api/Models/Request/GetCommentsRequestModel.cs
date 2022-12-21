namespace AspNetArticle.Api.Models.Request
{
    /// <summary>
    /// Request model for getting all comments for particular article page
    /// </summary>
    public class GetCommentsRequestModel
    {
        public Guid ArticleId { get; set; }
        public Guid UserId { get; set; }
    }
}
