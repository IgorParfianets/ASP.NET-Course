namespace AspNetArticle.Api.Models.Request
{
    /// <summary>
    /// Request model for updating comment
    /// </summary>
    public class UpdateCommentRequestModel
    {
        public Guid Id { get; set; }
        public Guid ArticleId { get; set; }
        public string Description { get; set; }
    }
}
