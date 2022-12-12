using AspNetArticle.Data.Abstractions.Repositories;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;

namespace AspNetArticle.Data.Abstractions;

public interface IUnitOfWork
{
    IExtendedArticleRepository Articles { get; }
    IRepository<Source> Sources { get; }
    IRepository<User> Users { get; }
    IRepository<Role> Roles { get; }
    IRepository<Comment> Comments { get; }
    IRepository<FavouriteArticle> FavouriteArticle { get; }

    Task<int> Commit();
}
