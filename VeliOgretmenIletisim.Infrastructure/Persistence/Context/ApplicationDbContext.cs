using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Common;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Infrastructure.Persistence.Context;

public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    private readonly ICurrentUserService _currentUserService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Parent> Parents => Set<Parent>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<StudentTeacher> StudentTeachers => Set<StudentTeacher>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Availability> Availabilities => Set<Availability>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<MeetingNote> MeetingNotes => Set<MeetingNote>();
    public DbSet<ComplaintRequest> ComplaintRequests => Set<ComplaintRequest>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);

        // Global Soft Delete Filter
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var filter = Expression.Lambda(
                    Expression.Equal(Expression.Property(parameter, nameof(ISoftDelete.IsDeleted)), Expression.Constant(false)),
                    parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId?.ToString() ?? "System";
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            // Soft Delete Logic (Junction table'ı hariç tutuyoruz çünkü M-N güncellemede sorun çıkarıyor)
            if (entry.State == EntityState.Deleted && entry.Entity is ISoftDelete softDelete)
            {
                if (entry.Entity is StudentTeacher)
                {
                    // StudentTeacher kayıtlarını gerçekten silsin (Hard Delete)
                    continue;
                }

                entry.State = EntityState.Modified;
                softDelete.IsDeleted = true;
            }

            // Audit Properties Logic
            if (entry.Entity is IAuditEntity audit)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        audit.CreatedDate = now;
                        audit.CreatedBy = currentUserId;
                        break;
                    case EntityState.Modified:
                        audit.UpdatedDate = now;
                        audit.UpdatedBy = currentUserId;
                        break;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
