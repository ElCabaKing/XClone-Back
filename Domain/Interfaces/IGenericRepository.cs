
using System.Linq.Expressions;

namespace Domain.Interfaces;

public interface IGenericRepository<T> where T : class
{

    Task<T> Create(T entity);
    Task<T> Update(T entity);
    IQueryable<T> Queryable();
    Task<bool> Delete(T entity);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<List<T>> ToListAsync(Expression<Func<T, bool>> predicate);
    Task<bool> IfExists(Expression<Func<T, bool>> predicate);
    Task<T[]?> GetAll();
}