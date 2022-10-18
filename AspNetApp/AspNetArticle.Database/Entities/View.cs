namespace AspNetArticle.Database.Entities;

public class View : IBaseEntity
{
    public Guid Id { get; set; }
    public string IpAddress { get; set; }
    public DateTime DateOfView { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid ArticleId { get; set; }
    public Article Article { get; set; }
}
