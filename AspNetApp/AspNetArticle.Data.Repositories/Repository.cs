using AspNetArticle.Core;
using AspNetArticle.Data.Abstractions.Repositories;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AspNetArticle.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class, IBaseEntity
{
    protected readonly AggregatorContext DataBase;
    protected readonly DbSet<T> DbSet;

    public Repository(AggregatorContext dataBase)
    {
        DataBase = dataBase;
        DbSet = dataBase.Set<T>();
    }

    //Add
    public virtual async Task AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await DbSet.AddRangeAsync(entities);
    }

    // Find
    public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> searchExpression, 
        params Expression<Func<T, object>>[] includes)
    {
        var result = DbSet.Where(searchExpression);

        if (includes.Any())
        {
            result = includes.Aggregate(result, (current, includes) =>
            current.Include(includes));
        }

        return result;
    }

    // Get
    public virtual IQueryable<T> Get()
    {
        return DbSet;
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await DbSet.FirstOrDefaultAsync(entiry => entiry.Id.Equals(id));

    }

    //Update
    public virtual void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public virtual async Task PatchAsync(Guid id, List<PatchModel> patchData)
    {
        var model = await DbSet.FirstOrDefaultAsync(entity => entity.Id.Equals(id));

        var nameValuePropertiesPairs = patchData
            .ToDictionary(
            patchModel => patchModel.PropertyName,
            patchModel => patchModel.PropertyValue);

        var dbEntityEntry = DataBase.Entry(model);
        dbEntityEntry.CurrentValues.SetValues(nameValuePropertiesPairs);
        dbEntityEntry.State = EntityState.Modified;
    }

    //Delete
    public virtual void Remove(T entity)
    {
        DbSet.Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
       DbSet.RemoveRange(entities);
    }
}
