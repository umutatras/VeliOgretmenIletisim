using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VeliOgretmenIletisim.Application.Interfaces.Notifications;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;
using VeliOgretmenIletisim.Infrastructure.Persistence.Context;
using VeliOgretmenIletisim.Infrastructure.Persistence.UnitOfWork;
using VeliOgretmenIletisim.Infrastructure.Services;

namespace VeliOgretmenIletisim.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Identity
        services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Hangfire
        services.AddHangfire(config => config.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));
        services.AddHangfireServer();

        // Unit of Work & Identity Services
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IJwtProvider, JwtService>();

        // Notification Services
        services.AddScoped<IEmailService, MockEmailService>();
        services.AddScoped<ISmsService, MockSmsService>();
        services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
        services.AddScoped<INotificationService, SignalRNotificationService>();
        services.AddScoped<VeliOgretmenIletisim.Application.Interfaces.Files.IFileService, LocalFileService>();

        services.AddSignalR();

        return services;
    }
}
