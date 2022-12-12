using AspNetArticle.Data.Abstractions;
using AspNetArticle.Data.Abstractions.Repositories;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;

namespace AspNetArticle.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AggregatorContext _dataBase;
    public IExtendedArticleRepository Articles { get; }
    public IRepository<User> Users { get; }
    public IRepository<Role> Roles { get; }
    public IRepository<Source> Sources { get; }
    public IRepository<Comment> Comments { get; }
    public IRepository<FavouriteArticle> FavouriteArticle { get; }

    public UnitOfWork(AggregatorContext context,
        IExtendedArticleRepository articles,
        IRepository<User> users,
        IRepository<Role> roles,
        IRepository<Source> sources, 
        IRepository<Comment> comments,
        IRepository<FavouriteArticle> favouriteArticle)
    {
        _dataBase = context;
        Articles = articles;
        Users = users;
        Roles = roles;
        Sources = sources;
        Comments = comments;
        FavouriteArticle = favouriteArticle;
    }

    public async Task<int> Commit()
    {
       return await _dataBase.SaveChangesAsync();
    }
}
