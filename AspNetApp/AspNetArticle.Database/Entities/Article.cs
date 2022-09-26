using System.Xml.Linq;

namespace AspNetArticle.Database.Entities;

public class Article
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string ShortDescription { get; set; }
    public string FullText { get; set; }
    public DateTime PublicationDate { get; set; }

    public Guid SourceId { get; set; }
    public Source Source { get; set; }

    public List<Comment> Comments { get; set; }
    public List<View> Views { get; set; }
}
