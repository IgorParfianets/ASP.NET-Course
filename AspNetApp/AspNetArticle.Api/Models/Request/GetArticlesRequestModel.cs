namespace AspNetArticle.Api.Models.Request
{
    /// <summary>
    /// Request model for getting all articles from database
    /// </summary>
    public class GetArticlesRequestModel
    {
        public string? Name { get; set; }
        public Guid? SourceId { get; set; }
    }
}
