using AutoSchool.Data.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AutoSchool.Data.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext Context;
    protected DbSet<TEntity> Set => Context.Set<TEntity>();

    public Repository(ApplicationDbContext context)
    {
        Context = context;
    }

    public IQueryable<TEntity> Query(bool asNoTracking = false)
        => asNoTracking ? Set.AsNoTracking() : Set;

    public TEntity? Find(params object[] keyValues)
        => Set.Find(keyValues);

    public void Add(TEntity entity) => Set.Add(entity);
    public void Remove(TEntity entity) => Set.Remove(entity);
}