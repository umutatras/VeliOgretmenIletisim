using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Infrastructure.Persistence.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.AppUser)
            .WithOne(u => u.TeacherProfile)
            .HasForeignKey<Teacher>(x => x.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Department)
            .WithMany(d => d.Teachers)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
