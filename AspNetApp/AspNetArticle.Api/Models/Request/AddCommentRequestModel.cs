namespace AspNetArticle.Api.Models.Request
{
    public class AddCommentRequestModel
    {
        public string Description { get; set; }
        public Guid UserId { get; set; }
        public Guid ArticleId { get; set; }
    }
}
