using System.Linq.Expressions;
using Inventory.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(long id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
}

public class Repository<T>(InventoryDbContext context) : IRepository<T> where T : class
{
    protected readonly InventoryDbContext _context = context;

    public async Task<T?> GetByIdAsync(long id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().Where(predicate).ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return _context.Set<T>().AnyAsync(predicate);
    }
}