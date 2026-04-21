using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Infrastructure.Persistence.Context;

namespace VeliOgretmenIletisim.Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

    public IQueryable<T> GetAll() => _dbSet.AsNoTracking();

    public IQueryable<T> Where(Expression<Func<T, bool>> predicate) => _dbSet.Where(predicate);

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Delete(T entity) => _dbSet.Remove(entity);
}
