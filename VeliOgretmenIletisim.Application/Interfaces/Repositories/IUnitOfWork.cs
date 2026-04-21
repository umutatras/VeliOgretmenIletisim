namespace VeliOgretmenIletisim.Application.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T> GetRepository<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
