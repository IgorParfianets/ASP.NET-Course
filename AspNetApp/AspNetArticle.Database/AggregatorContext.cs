using AspNetArticle.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace AspNetArticle.Database;

public class AggregatorContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<Source> Sources { get; set; }
    public DbSet<View> Views { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Role> Roles { get; set; }

    public AggregatorContext(DbContextOptions<AggregatorContext> options) : base(options)
    {
    }
}