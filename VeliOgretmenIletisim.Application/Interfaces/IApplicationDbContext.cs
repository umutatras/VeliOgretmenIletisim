using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<AppUser> AppUsers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
