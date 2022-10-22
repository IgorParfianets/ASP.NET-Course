using AspNetArticle.Data.Abstractions.Repositories;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;

namespace AspNetArticle.Data.Abstractions;

public interface IUnitOfWork
{
    IRepository<Article> Articles { get; }
    IRepository<User> Users { get; }
    IRepository<Role> Roles { get; }
    Task<int> Commit();
}
