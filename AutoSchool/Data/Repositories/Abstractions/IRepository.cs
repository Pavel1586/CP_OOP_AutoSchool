using System.Linq;

namespace AutoSchool.Data.Repositories.Abstractions;

public interface IRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> Query(bool asNoTracking = false);

    TEntity? Find(params object[] keyValues);
    void Add(TEntity entity);
    void Remove(TEntity entity);
}