using AspNetArticle.Core;

namespace AspNetArticle.Database.Entities;

public class Source : IBaseEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; } 
    public string? RssUrl { get; set; } 
    public SourceType SourceType { get; set; }

    public List<Article> Articles { get; set; }
}
