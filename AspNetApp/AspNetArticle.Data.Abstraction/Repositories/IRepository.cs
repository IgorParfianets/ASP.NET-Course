using AspNetArticle.Core;
using AspNetArticle.Database.Entities;
using System.Linq.Expressions;

namespace AspNetArticle.Data.Abstractions.Repositories;

public interface IRepository<T> where T : IBaseEntity
{
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    IQueryable<T> Get();
    IQueryable<T> FindBy(Expression<Func<T, bool>> searchExpression, 
        params Expression<Func<T, object>>[] includes);
    void Update(T entity);
    Task PatchAsync(Guid id, List<PatchModel> patchData);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}
