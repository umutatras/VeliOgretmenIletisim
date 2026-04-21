using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using VeliOgretmenIletisim.Application.Interfaces.Security;

namespace VeliOgretmenIletisim.Infrastructure.Persistence.Context;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Build configuration from WebAPI appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../VeliOgretmenIletisim.WebAPI"))
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        builder.UseSqlServer(connectionString);

        // Dummy service for design time
        return new ApplicationDbContext(builder.Options, new DesignTimeCurrentUserService());
    }
}

// Design-time implementation of ICurrentUserService
public class DesignTimeCurrentUserService : ICurrentUserService
{
    public Guid? UserId => null;
    public string? UserName => "DesignUser";
    public string? Role => "Admin";
}
