namespace AspNetArticle.Core.DataTransferObjects;

public class ArticleDto // Можно рабить на 2 класса ArticleAndCommentariesDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? ShortDescription { get; set; }
    public string? Text { get; set; }
    public string? Category { get; set; }
    public string SourceUrl { get; set; }
    public DateTime PublicationDate { get; set; }
    public Guid SourceId { get; set; }
    public string? ImageUrl { get; set; }
    public double? Rate { get; set; }

    public List<CommentDto> Comments { get; set; }
}
