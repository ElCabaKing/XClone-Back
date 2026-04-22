
using System.Linq.Expressions;
using Domain.Interfaces;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GenericRepository<T>(XDbContext context) : IGenericRepository<T> where T : class
{
    protected readonly XDbContext _context = context;
    public async Task<T> Create(T entity)
    {
        _context.Set<T>().Add(entity);
        return entity;
    }

    public async Task<bool> Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
        return true;
    }

    public IQueryable<T> Queryable()
    {
        return _context.Set<T>().AsQueryable();
    }

    public async Task<T> Update(T entity)
    {
        _context.Set<T>().Update(entity);
        return entity;
    }

    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return _context.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public async Task<List<T>> ToListAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().Where(predicate).ToListAsync();
    }

    public async Task<bool> IfExists(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().AnyAsync(predicate) ;
    }

    public async Task<T[]?> GetAll()
    {
        return await _context.Set<T>().ToArrayAsync();
    }
}