namespace AspNetArticle.Core.DataTransferObjects;

public class SourceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public SourceType SourceType { get; set; }
    public List<ArticleDto> Articles { get; set; }
}
