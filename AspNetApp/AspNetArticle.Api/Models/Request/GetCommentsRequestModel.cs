namespace AspNetArticle.Api.Models.Request
{
    public class GetCommentsRequestModel
    {
        public Guid? ArticleId { get; set; }
        public Guid? UserId { get; set; }
    }
}
