namespace AspNetArticle.Api.Models.Request
{
    /// <summary>
    /// Request model for creating comment
    /// </summary>
    public class AddCommentRequestModel
    {
        public string Description { get; set; }
        public Guid UserId { get; set; }
        public Guid ArticleId { get; set; }
    }
}
