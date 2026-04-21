using System.Linq.Expressions;

namespace VeliOgretmenIletisim.Application.Interfaces.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<T> GetByIdAsync(Guid id);
    IQueryable<T> GetAll();
    IQueryable<T> Where(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
