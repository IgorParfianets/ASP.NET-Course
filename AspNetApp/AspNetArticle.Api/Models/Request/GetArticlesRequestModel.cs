namespace AspNetArticle.Api.Models.Request
{
    public class GetArticlesRequestModel
    {
        public string? Name { get; set; }
        public Guid? SourceId { get; set; }
    }
}
