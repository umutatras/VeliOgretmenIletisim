using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Availability)
            .WithMany(a => a.Appointments)
            .HasForeignKey(x => x.AvailabilityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Parent)
            .WithMany()
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
