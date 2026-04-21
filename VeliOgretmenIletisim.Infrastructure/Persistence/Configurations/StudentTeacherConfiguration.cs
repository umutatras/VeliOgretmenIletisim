using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Infrastructure.Persistence.Configurations;

public class StudentTeacherConfiguration : IEntityTypeConfiguration<StudentTeacher>
{
    public void Configure(EntityTypeBuilder<StudentTeacher> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Student)
            .WithMany(s => s.StudentTeachers)
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Teacher)
            .WithMany(t => t.StudentTeachers)
            .HasForeignKey(x => x.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Property(x => x.IsPrimary).HasDefaultValue(false);
    }
}
