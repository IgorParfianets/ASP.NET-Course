namespace AspNetArticle.Core.DataTransferObjects;

public class ArticleDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string ShortDescription { get; set; }
    public string FullText { get; set; }
    public DateTime PublicationDate { get; set; }
    public List<CommentDto> Comments { get; set; }
    public List<ViewDto> Views { get; set; }
}
