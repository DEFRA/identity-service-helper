using System.Linq.Expressions;

namespace Livestock.Auth.Services;

public interface IRepository<TEntity> where TEntity : class
{
    Task<List<TEntity>> GetAll();
    Task<TEntity?> Get(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> Create(TEntity entity);
    Task<TEntity> Update(TEntity entity);
    Task<bool> Delete(Func<TEntity, bool> predicate);
}