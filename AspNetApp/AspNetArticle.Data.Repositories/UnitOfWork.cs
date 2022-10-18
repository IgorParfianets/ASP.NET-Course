using AspNetArticle.Data.Abstractions;
using AspNetArticle.Data.Abstractions.Repositories;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;

namespace AspNetArticle.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AggregatorContext _dataBase;
    public IRepository<Article> Articles { get; }
    public IRepository<User> Users { get; }

    public UnitOfWork(AggregatorContext context, IRepository<Article> articles, IRepository<User> users)
    {
        _dataBase = context;
        Articles = articles;
        Users = users;
    }

    public async Task<int> Commit()
    {
       return await _dataBase.SaveChangesAsync();
    }
}
